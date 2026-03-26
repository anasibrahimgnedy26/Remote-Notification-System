import pyodbc
import pandas as pd

conn_str = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=127.0.0.1,1434;"
    "DATABASE=NotificationSystemDb;"
    "UID=sa;"
    "PWD=SuperStrongP@ssw0rd!;"
    "TrustServerCertificate=Yes;"
    "Encrypt=Optional;"
)

def check_db():
    try:
        with pyodbc.connect(conn_str) as conn:
            print("--- Table: Devices ---")
            df_dev = pd.read_sql("SELECT COUNT(*) as Count FROM Devices", conn)
            print(df_dev)
            
            print("\n--- Table: Notifications (Most Recent 5) ---")
            df_notif = pd.read_sql("SELECT TOP 5 * FROM Notifications ORDER BY CreatedAt DESC", conn)
            print(df_notif)
            
            print("\n--- Current Time in DB ---")
            df_time = pd.read_sql("SELECT GETUTCDATE() as DB_UTC_Time", conn)
            print(df_time)
            
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    check_db()
