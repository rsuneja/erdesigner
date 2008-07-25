using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace ERDesigner
{
    class MSSQLServer2000Provider : IDatabase
    {
        SqlConnection conn;

        public MSSQLServer2000Provider()
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
                SqlConnection tmpConn = new SqlConnection(getConnectionString("master"));
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
            switch(datatype)
            {
                case StandardDataType.Number:
                    newdatatype = "int";
                    break;
                case StandardDataType.Text:
                    newdatatype = "nvarchar";
                    break;
                case StandardDataType.LongText:
                    newdatatype = "ntext";
                    break;
                case StandardDataType.Decimal:
                    newdatatype = "decimal";
                    break;
                case StandardDataType.DateTime:
                    newdatatype = "datetime";
                    break;
                case StandardDataType.Binary:
                    newdatatype = "image";
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
                conn = new SqlConnection(getConnectionString(DatabaseName));
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
            SqlConnection tmpConn = new SqlConnection(getConnectionString("master"));
            string sqlCreateDBQuery = "";
            if (FolderPath != "")
                sqlCreateDBQuery = " CREATE DATABASE "
                               + DatabaseName
                               + " ON PRIMARY "
                               + " (NAME = " + DatabaseName + ", "
                               + " FILENAME = '" + FolderPath + "\\" + DatabaseName + "_DATA.MDF', "
                               + " SIZE = 1MB) "
                               + " LOG ON (NAME =" + DatabaseName + "LOG, "
                               + " FILENAME = '" + FolderPath + "\\" + DatabaseName + "_LOG.LDF', "
                               + " SIZE = 1MB) ";
            else
                sqlCreateDBQuery = " CREATE DATABASE " + DatabaseName;

            try
            {
                tmpConn.Open();
                SqlCommand Command = new SqlCommand(sqlCreateDBQuery, tmpConn);
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
                sqlQuery = sqlQuery.Replace("GO", ";");
                SqlCommand Command = new SqlCommand(sqlQuery, conn);
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
