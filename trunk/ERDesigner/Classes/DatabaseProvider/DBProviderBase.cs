using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    class DBProviderBase
    {
        IDatabase database;
        public DBProviderBase()
        {
            switch (ThongSo.DB_Mode)
            {
                case DBMS.MSSQLServer2000:
                    database = new MSSQLServer2000Provider();
                    break;
                case DBMS.Oracle:
                    database = new OracleProvider();
                    break;
                case DBMS.MySql:
                    database = new MySQLProvider();
                    break;
                case DBMS.Access:
                    database = new MSAccessProvider();
                    break;
                default:
                    database = new MSSQLServer2000Provider();
                    break;
            }
        }

        public bool TestConnection()
        {
            return database.TestConnection();
        }

        public bool Connect(string DatabaseName)
        {
            return database.Connect(DatabaseName);
        }

        public bool CreateDatabase(string DatabaseName, string FilePath)
        {
            return database.CreateDatabase(DatabaseName, FilePath);
        }

        public bool Execute(string sqlQuery)
        {
            sqlQuery = sqlQuery.Replace("GO", ";");
            return database.Execute(sqlQuery);
        }

        public void Close()
        {
            database.Close();
        }
    }
}
