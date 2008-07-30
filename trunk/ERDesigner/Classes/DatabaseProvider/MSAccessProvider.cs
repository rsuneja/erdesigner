using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace ERDesigner
{
    class MSAccessProvider : IDatabase
    {
        OleDbConnection conn;

        public MSAccessProvider()
        {
            conn = new OleDbConnection();
        }

        private string GetStringCreateDB(string dbName)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbName + ";Jet OLEDB:Engine Type=5";
        }
        private string getConnectionString(string DatabaseName)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + DatabaseName + ";User Id=" + ThongSo.DB_UserName + ";Password=" + ThongSo.DB_Password + ";";
        }

        #region IDatabase Members

        public bool TestConnection()
        {
            return true;
        }

        public string getDataType(string datatype)
        {
            string newdatatype = "";
            switch (datatype)
            {
                case StandardDataType.Number:
                    newdatatype = "int";
                    break;
                case StandardDataType.Text:
                    newdatatype = "text";
                    break;
                case StandardDataType.LongText:
                    newdatatype = "memo";
                    break;
                case StandardDataType.Decimal:
                    newdatatype = "double";
                    break;
                case StandardDataType.DateTime:
                    newdatatype = "datetime";
                    break;
                case StandardDataType.Binary:
                    newdatatype = "binary";
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
                conn = new OleDbConnection(getConnectionString(DatabaseName+".mdb"));
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CreateDatabase(string DatabaseName, string FilePath)
        {

            try
            {
                ADOX.CatalogClass cat = new ADOX.CatalogClass();
                cat.Create(GetStringCreateDB(FilePath + "\\" + DatabaseName + ".mdb"));
                Marshal.ReleaseComObject(cat);
                cat = null;
                GC.Collect();
                return true;
            }
            catch
            {                
                return false;
            }
        }

        public bool Execute(string sqlQuery)
        {
            string[] listQuery = sqlQuery.Replace("\r\n", "").Split(';');
            bool success = true;

            foreach (string query in listQuery)
            {
                //Replace "\t"
                if (query.Length > 10)
                {
                    try
                    {
                        OleDbCommand Command = new OleDbCommand(query, conn);
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
