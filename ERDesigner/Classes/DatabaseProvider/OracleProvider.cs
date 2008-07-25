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
                    break;
                case StandardDataType.DateTime:
                    newdatatype = "date";
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
            try
            {
                string[] listQuery = sqlQuery.Replace("\r\n", "").Split(';');

                foreach (string query in listQuery)
                {
                    if(query.Length > 10)
                    {
                        OracleCommand Command = new OracleCommand(query, conn);
                        Command.ExecuteNonQuery();
                    }
                }
                
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
