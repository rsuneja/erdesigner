using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class GenerateDDL
    {
        private DBMS nameDBMS = DBMS.MSSQLServer2000;
        private string dbName;
        private MetaDataPhysical mdp;
        /// <summary>
        /// Phương thức khởi tạo GenerateDDL
        /// </summary>
        /// <param name="mdp">Meta data physical</param>
        /// <param name="nameDBMS">Loại Database Manage System</param>
        /// <param name="dbName">Tên Database</param>
        public GenerateDDL(MetaDataPhysical mdp, DBMS nameDBMS, string dbName)
        {
            this.mdp = mdp;
            this.nameDBMS = nameDBMS;
            this.dbName = dbName;
        }

        public List<string> Process()
        {
            List<string> listScript = new List<string>();
            if (nameDBMS == DBMS.MSSQLServer2000)
            {
                ScriptSQL sql = new ScriptSQL(mdp, dbName);
                listScript = sql.Process();
            }
            if (nameDBMS == DBMS.Oracle)
            {
                ScriptOracle oracle = new ScriptOracle(mdp, dbName);
                listScript = oracle.Process();
            }
            if (nameDBMS == DBMS.Access)
            {
                ScriptMSAccess access = new ScriptMSAccess(mdp, dbName);
                listScript = access.Process();
            }
            if (nameDBMS == DBMS.MySql)
            {
                ScriptMySql sql = new ScriptMySql(mdp, dbName);
                listScript = sql.Process();
            }
            return listScript;
        }
    }
}
