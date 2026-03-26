# ============================================================
#  Remote Notification System — One-Click Startup Script
#  Run from PowerShell AS ADMINISTRATOR:
#    cd D:\ANAS\Programming\Bussma\Project-2\RemoteNotificationSystem
#    .\start-project.ps1
# ============================================================

$ROOT = "D:\ANAS\Programming\Bussma\Project-2\RemoteNotificationSystem"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Remote Notification System Launcher"   -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ----------------------------------------------------------
# 0. Kill existing processes to prevent conflicts
# ----------------------------------------------------------
Write-Host "[0/6] Cleaning up old processes..." -ForegroundColor Yellow
wsl -e bash -c "killall -9 dotnet 2>/dev/null"
wsl -e bash -c "killall -9 node 2>/dev/null"
wsl -e bash -c "pkill -f localtunnel 2>/dev/null"
Start-Sleep -Seconds 2
Write-Host "  [OK] Old processes cleaned" -ForegroundColor Green

# ----------------------------------------------------------
# 1. Start Docker Daemon
# ----------------------------------------------------------
Write-Host "[1/6] Starting Docker daemon..." -ForegroundColor Yellow
wsl -e bash -c "echo 'anasunix1234' | sudo -S nohup dockerd > /dev/null 2>&1 &"
Start-Sleep -Seconds 3
Write-Host "  [OK] Docker daemon started" -ForegroundColor Green

# ----------------------------------------------------------
# 2. Start Docker SQL Server
# ----------------------------------------------------------
Write-Host "[2/6] Starting SQL Server Docker container..." -ForegroundColor Yellow
wsl -e bash -c "echo 'anasunix1234' | sudo -S docker start mssql 2>/dev/null || echo 'anasunix1234' | sudo -S docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=SuperStrongP@ssw0rd!' -p 1434:1433 --name mssql -d mcr.microsoft.com/mssql/server:2022-latest"

# Wait for SQL Server to be ready
Write-Host "  Waiting for SQL Server to be ready..." -ForegroundColor DarkGray
$retries = 0
$maxRetries = 15
$sqlReady = $false
while (-not $sqlReady -and $retries -lt $maxRetries) {
    Start-Sleep -Seconds 3
    $retries++
    $result = wsl -e bash -c "echo 'anasunix1234' | sudo -S docker exec mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'SuperStrongP@ssw0rd!' -Q 'SELECT 1' -C 2>/dev/null | grep '1 rows'" 2>$null
    if ($result) { $sqlReady = $true }
    else { Write-Host "  Retry $retries/$maxRetries..." -ForegroundColor DarkGray }
}
if ($sqlReady) {
    Write-Host "  [OK] SQL Server is ready on port 1434" -ForegroundColor Green
}
else {
    Write-Host "  [WARN] SQL Server may not be ready yet. Continuing anyway..." -ForegroundColor Red
}

# ----------------------------------------------------------
# 3. Get WSL IP and set up port forwarding
# ----------------------------------------------------------
Write-Host "[3/6] Setting up port forwarding (WSL -> Windows)..." -ForegroundColor Yellow
$wslIp = (wsl -e bash -c "hostname -I | awk '{print `$1}'").Trim()
Write-Host "  WSL IP: $wslIp" -ForegroundColor DarkGray

try {
    # Forward Port 5000 (Backend API)
    netsh interface portproxy delete v4tov4 listenport=5000 listenaddress=0.0.0.0 2>$null | Out-Null
    netsh interface portproxy add v4tov4 listenport=5000 listenaddress=0.0.0.0 connectport=5000 connectaddress=$wslIp | Out-Null
    
    # Forward Port 4200 (Angular UI)
    netsh interface portproxy delete v4tov4 listenport=4200 listenaddress=0.0.0.0 2>$null | Out-Null
    netsh interface portproxy add v4tov4 listenport=4200 listenaddress=0.0.0.0 connectport=4200 connectaddress=$wslIp | Out-Null

    # Firewall Rules
    netsh advfirewall firewall delete rule name="Notification System Ports" 2>$null | Out-Null
    netsh advfirewall firewall add rule name="Notification System Ports" dir=in action=allow protocol=TCP localport=5000, 4200 | Out-Null
    
    Write-Host "  [OK] Ports Forwarded: 5000 (API) and 4200 (UI)" -ForegroundColor Green
}
catch {
    Write-Host "  [ERROR] Failed to set port proxy. MUST RUN AS ADMINISTRATOR!" -ForegroundColor Red
    exit
}

# ----------------------------------------------------------
# 4. Start .NET Backend API (New Window)
# ----------------------------------------------------------
Write-Host "[4/6] Starting .NET Backend API..." -ForegroundColor Yellow
Start-Process -FilePath "wsl" -ArgumentList "-e bash -c `"cd /mnt/d/ANAS/Programming/Bussma/Project-2/RemoteNotificationSystem/backend/NotificationSystem.API && dotnet run > api_debug.log 2>&1`"" -WindowStyle Normal
Write-Host "  [OK] Backend window opened" -ForegroundColor Green

# ----------------------------------------------------------
# 5. Start Angular Admin Panel (New Window)
# ----------------------------------------------------------
Write-Host "[5/6] Starting Angular Admin Panel..." -ForegroundColor Yellow
Start-Process -FilePath "wsl" -ArgumentList "-e bash -l -c `"source ~/.nvm/nvm.sh && cd /mnt/d/ANAS/Programming/Bussma/Project-2/RemoteNotificationSystem/admin-panel/angular-notification-admin && npx ng serve --host 0.0.0.0`"" -WindowStyle Normal
Write-Host "  [OK] Angular window opened" -ForegroundColor Green

# ----------------------------------------------------------
# 6. Start Public Tunnel (Localtunnel)
# ----------------------------------------------------------
Write-Host "[6/6] Starting Public Tunnel (Localtunnel)..." -ForegroundColor Yellow
Start-Process -FilePath "powershell" -ArgumentList "-NoExit", "-Command", "npx -y localtunnel --port 5000" -WindowStyle Normal
Write-Host "  [OK] Localtunnel window opened. CHECK THE WINDOW FOR THE PUBLIC URL." -ForegroundColor Green
# ----------------------------------------------------------
# 7. Summary
# ----------------------------------------------------------
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  All Services Started!"                 -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Access URLs:" -ForegroundColor White
Write-Host "    Admin Panel    : http://localhost:4200" -ForegroundColor DarkGray
Write-Host "    Swagger UI     : http://localhost:5000" -ForegroundColor DarkGray
Write-Host "    Phone (LAN)    : http://192.168.137.1:5000/api" -ForegroundColor DarkGray
Write-Host "    Phone (Public) : check Localtunnel window for URL" -ForegroundColor DarkGray
Write-Host ""
Write-Host "  Wait about 10-15 seconds for everything to finish loading." -ForegroundColor Yellow
Write-Host "  Close the separate windows to stop the services." -ForegroundColor Yellow
Write-Host ""
