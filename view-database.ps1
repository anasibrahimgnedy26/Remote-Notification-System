# Database Viewer Script for Notification System
# This script uses docker exec via WSL to query the SQL Server database.

$ContainerName = "mssql"
$Password = "SuperStrongP@ssw0rd!"
$WslUserPass = "anasunix1234"
$SqlCmdPath = "/opt/mssql-tools18/bin/sqlcmd"

function Invoke-Query {
    param([string]$Header, [string]$Query)
    Write-Host "`n=== $Header ===" -ForegroundColor Cyan
    $fullCmd = "echo '$WslUserPass' | sudo -S docker exec -e SQLCMDPASSWORD='$Password' $ContainerName $SqlCmdPath -S localhost -d NotificationSystemDb -U sa -Q `"$Query`" -C"
    wsl -e bash -c "$fullCmd"
}

Invoke-Query "Registered Devices" "SET NOCOUNT ON; SELECT Id, LEFT(DeviceToken, 30) as Token, DeviceName, RegisteredAt FROM Devices"
Invoke-Query "Sent Notifications" "SET NOCOUNT ON; SELECT Id, Title, LEFT(Body, 40) as Body, SentBy, IsSent, CreatedAt FROM Notifications"
Invoke-Query "Notification Delivery Logs" "SET NOCOUNT ON; SELECT Id, NotificationId, DeviceId, Status, SentAt FROM NotificationLogs"

Write-Host "`nEnd of report." -ForegroundColor White
