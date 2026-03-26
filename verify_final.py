import pyodbc

conn_str = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=127.0.0.1,1434;"
    "DATABASE=NotificationSystemDb;"
    "UID=sa;"
    "PWD=SuperStrongP@ssw0rd!;"
    "TrustServerCertificate=Yes;"
    "Encrypt=Optional;"
)

def verify():
    try:
        with pyodbc.connect(conn_str) as conn:
            cursor = conn.cursor()
            cursor.execute("SELECT TOP 5 Id, Body, IsSent, CreatedAt FROM Notifications ORDER BY CreatedAt DESC")
            rows = cursor.fetchall()
            print("--- Recent Notifications ---")
            for row in rows:
                print(f"ID: {row[0]} | Body: {row[1]} | Sent: {row[2]} | Date: {row[3]}")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    verify()
