using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;

namespace ERDesigner
{
    class OracleProvider : IDatabase
    {
        OracleConnection conn;

        public OracleProvider()
        {

        }
        private string getConnectionString(string DatabaseName)
        {
            return "Data Source =" + ThongSo.DB_Server + "; User ID = " + ThongSo.DB_UserName + "; Password = " + ThongSo.DB_Password;
        }

        #region IDatabase Members
         
        public bool TestConnection()
        {
            try
            {
                OracleConnection tmpConn = new OracleConnection(getConnectionString("oracle"));
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
                    newdatatype = "number";
                    break;
                case StandardDataType.Text:
                    newdatatype = "varchar2";
                    break;
                case StandardDataType.LongText:
                    newdatatype = "clob";
                    break;
                case StandardDataType.Decimal:
                    newdatatype = "decimal";
                    break;
                case StandardDataType.DateTime:
                    newdatatype = "date";
                    break;
                case StandardDataType.Boolean:
                    newdatatype = "number";
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
                conn = new OracleConnection(getConnectionString(DatabaseName));
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
            return TestConnection();
        }

        public bool Execute(string sqlQuery)
        {
            string[] listQuery = sqlQuery.Replace("\r\n", "").Split(';');
            bool success = true;

            foreach (string query in listQuery)
            {
                
                if (query.Length > 10)
                {
                    try
                    {
                        OracleCommand Command = new OracleCommand(query, conn);
                        Command.ExecuteNonQuery();
                    }
                    catch
                    {
                        success = false;
                    }
                }
            }
            return success;
        }

        public void Close()
        {
            conn.Close();
        }

        #endregion
    }
}
