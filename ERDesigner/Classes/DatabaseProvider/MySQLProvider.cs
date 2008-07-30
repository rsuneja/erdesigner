using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace ERDesigner
{
    class MySQLProvider : IDatabase
    {
        MySqlConnection conn;

        public MySQLProvider()
        {

        }
        private string getConnectionString(string DatabaseName)
        {
            return "Server=" + ThongSo.DB_Server + "; Database = " + DatabaseName + "; User ID = " + ThongSo.DB_UserName + "; Password = " + ThongSo.DB_Password;
        }

        #region IDatabase Members
        
        public bool TestConnection()
        {
            try
            {
                MySqlConnection tmpConn = new MySqlConnection(getConnectionString("mysql"));
                tmpConn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string getDataType(string datatype)
        {
            string newdatatype = "";
            switch (datatype)
            {
                case StandardDataType.Number:
                    newdatatype = "integer";
                    break;
                case StandardDataType.Text:
                    newdatatype = "varchar";
                    break;
                case StandardDataType.LongText:
                    newdatatype = "longtext";
                    break;
                case StandardDataType.Decimal:
                    newdatatype = "decimal";
                    break;
                case StandardDataType.DateTime:
                    newdatatype = "datetime";
                    break;
                case StandardDataType.Boolean:
                    newdatatype = "boolean";
                    break;
                case StandardDataType.Binary:
                    newdatatype = "blob";
                    break;
                default:
                    newdatatype = datatype;
                    break;
            }
            return newdatatype;
        }

        public bool Connect(string DatabaseName)
        {
            try
            {
                conn = new MySqlConnection(getConnectionString(DatabaseName));
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateDatabase(string DatabaseName, string FolderPath)
        {
            MySqlConnection tmpConn = new MySqlConnection(getConnectionString("mysql"));
            string sqlCreateDBQuery = " CREATE DATABASE " + DatabaseName;
            
            try
            {
                tmpConn.Open();
                MySqlCommand Command = new MySqlCommand(sqlCreateDBQuery, tmpConn);
                Command.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            tmpConn.Close();
            
            return true;
        }

        public bool Execute(string sqlQuery)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand(sqlQuery, conn);
                Command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Close()
        {
            conn.Close();
        }

        #endregion
    }
}
