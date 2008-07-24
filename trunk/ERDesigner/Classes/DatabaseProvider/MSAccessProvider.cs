using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace ERDesigner
{
    class MSAccessProvider : IDatabase
    {
        SqlConnection conn;

        public MSAccessProvider()
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
            string sqlCreateDBQuery = " CREATE DATABASE "
                               + DatabaseName
                               + " ON PRIMARY "
                               + " (NAME = " + DatabaseName + ", "
                               + " FILENAME = '" + FolderPath + "\\" + DatabaseName + "_DATA.MDF', "
                               + " SIZE = 1MB) "
                               + " LOG ON (NAME =" + DatabaseName + "LOG, "
                               + " FILENAME = '" + FolderPath + "\\" + DatabaseName + "_LOG.LDF', "
                               + " SIZE = 1MB) ";
            
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
