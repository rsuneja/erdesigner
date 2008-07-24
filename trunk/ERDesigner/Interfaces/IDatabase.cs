using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    interface IDatabase
    {
        bool TestConnection();
        bool Connect(string DatabaseName);
        bool CreateDatabase(string DatabaseName, string FilePath);
        bool Execute(string sqlQuery);
        void Close();
    }
}
