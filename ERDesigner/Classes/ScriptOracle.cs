using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner.Classes
{
    class ScriptOracle
    {
        private MetaDataPhysical mdp;
        private string dbName;
        public ScriptOracle(MetaDataPhysical mdp, string dbName)
        {
            this.mdp = mdp;
            this.dbName = dbName;
        }
        private string CreateDataBase()
        {            
            return "";
        }
        private string CreateTable()
        {
            return "";
        }
        private string CreateForgienKey()
        {
            return "";
        }
        private string CreatePrimaryKey()
        {
            return "";
        }
        public string Process()
        {
            return "";

        }
    }
}
