using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    interface IDatabase
    {
        bool TestConnection();
        string getDataType(string datatype);
        bool Connect(string DatabaseName);
        bool CreateDatabase(string DatabaseName, string FilePath);
        //bool DropDatabase(string DatabaseName);
        bool Execute(string sqlQuery);
        void Close();
    }
}
