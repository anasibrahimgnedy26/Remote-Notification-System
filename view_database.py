import pyodbc
import pandas as pd

# Database connection settings
# Note: We use TrustServerCertificate=Yes and Encrypt=No to bypass SSL issues
# Also we connect through the forwarded port 1434 on 127.0.0.1
conn_str = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=127.0.0.1,1434;"
    "DATABASE=NotificationSystemDb;"
    "UID=sa;"
    "PWD=SuperStrongP@ssw0rd!;"
    "TrustServerCertificate=Yes;"
    "Encrypt=Optional;"
)

def view_table(table_name, title, sort_column=None):
    print(f"\n{'='*20} {title} {'='*20}")
    try:
        with pyodbc.connect(conn_str) as conn:
            cursor = conn.cursor()
            query = f"SELECT * FROM {table_name}"
            if sort_column:
                query += f" ORDER BY {sort_column} DESC"
            
            cursor.execute(query)
            columns = [column[0] for column in cursor.description]
            rows = cursor.fetchall()
            
            if not rows:
                print(f"No records found in {table_name}.")
                return

            # Print Header
            header = " | ".join(columns)
            print(header)
            print("-" * len(header))
            
            # Print Rows (Limit to 15)
            for row in rows[:15]:
                formatted_row = []
                for val in row:
                    s_val = str(val)
                    if len(s_val) > 40:
                        s_val = s_val[:37] + "..."
                    formatted_row.append(s_val)
                print(" | ".join(formatted_row))
                
    except Exception as e:
        print(f"Error querying {table_name}: {e}")

def main():
    print("Remote Notification System - Database Viewer (v2.0)")
    
    view_table("Devices", "Registered Devices", sort_column="RegisteredAt")
    view_table("Notifications", "Sent Notifications", sort_column="CreatedAt")
    view_table("NotificationLogs", "Delivery Logs", sort_column="SentAt")
    
    print("\nEnd of report.")

if __name__ == "__main__":
    main()
