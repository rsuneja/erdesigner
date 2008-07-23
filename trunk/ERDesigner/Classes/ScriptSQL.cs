using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class ScriptSQL
    {
        private string dbName;
        private MetaDataPhysical mdp;
        public ScriptSQL(MetaDataPhysical mdp, string dbName)
        {
            this.mdp = mdp;
            this.dbName = dbName;
        }
        public List<string> Process()
        {           
            List<string> listCreateTable = new List<string>();
            List<string> listPK = new List<string>();
            List<string> listFK = new List<string>();

            foreach (Table table in mdp.Tables)
            {
                string strCreateTable = CreateTable(table.name, table.columns);

                listCreateTable.Add(strCreateTable);               

                List<string> listPKName = new List<string>();
                foreach (Column col in table.columns)
                    if (col.PrimaryKey)
                    {
                        listPKName.Add(col.Name);                        
                    }
                string strPK = CreatePrimaryKey(table.name, listPKName);
                listPK.Add(strPK);

            }            
            foreach (ForeignKey fk in mdp.ForeignKeys)
            {               
                string strFK = CreateForeignKey(fk.Name, fk.ParentTable, fk.ParentColumn, fk.ChildTable, fk.ChildColumn);                
                listFK.Add(strFK);               
            }
            
            string strDB = CreateDataBase(dbName);
            string strTables = GenerateTables(listCreateTable, listPK);
            string strFKs = GenerateForeignKeys(listFK);
            List<string> listScript = new List<string>();
            listScript.Add(strDB);
            listScript.Add(strTables);
            listScript.Add(strFKs);
            return listScript;                      
        }

        //getAlowNull truyền vào giá trị bool trả về chuỗi tương ứng: True - null, False - not null
        private string getAlowNull(bool p)
        {
            string strAlowNull = "";
            if (p)
                strAlowNull = "null";
            else
                strAlowNull = "not null";
            return strAlowNull;
        }

        //Create
        private string CreateDataBase(string dbName)
        {
            string script = "";
            script += "Use Master" + "\r\n";
            script += "GO" + "\r\n";
            script += DropDatabase(dbName);
            script += "Create database [" + dbName + "]\r\n";
            script += "GO" + "\r\n";
            script += "Use [" + dbName + "]\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        private string CreateTable(string tableName, List<Column> listColumn)
        {

            string temp1 = "";
            temp1 = "/*" + temp1.PadRight(50, '=') + "*/ \r\n";
            string temp2 = "Table: " + tableName + "";
            temp2 = "/*" + temp2.PadRight(50, ' ') + "*/ \r\n";

            string script = "";
            script += temp1;
            script += temp2;
            script += temp1;
            script += DropTable(tableName);
            script += "Create table [" + tableName + "]\r\n";
            script += "(" + "\r\n";
            int i;
            string dataType = "";
            for (i = 0; i < listColumn.Count - 1; i++)
            {
                Column col = listColumn[i];
                string strAlowNull = getAlowNull(col.AlowNull);
                dataType = DataTypeLenght(col.DataType, col.Length.ToString());
                script += "\t" + col.Name + " " + dataType + " " + strAlowNull + " ," + "\r\n";
            }
            dataType = DataTypeLenght(listColumn[i].DataType, listColumn[i].Length.ToString());
            script += "\t" + listColumn[i].Name + " " + dataType + " " + getAlowNull(listColumn[i].AlowNull) + "\r\n";
            script += ")" + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        private string CreatePrimaryKey(string tableName, List<string> columnName)
        {
            string script = "";
            script += "Alter table [" + tableName + "]\r\n";
            script += "   add constraint " + "PK_" + tableName + " primary key (";
            int i = 0;
            for (i = 0; i < columnName.Count - 1; i++)
            {
                script += columnName[i] + ",";
            }
            script += columnName[i] + ")" + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        private string CreateForeignKey(string fkName, string parentTable, List<string> parentColumn, string childTable, List<string> childColumn)
        {
            string script = "";
            script += "Alter table [" + childTable + "]\r\n";
            script += "   add constraint " + fkName + " foreign key (";
            int i = 0, j = 0;
            for (i = 0; i < childColumn.Count - 1; i++)
            {
                script += childColumn[i] + ",";
            }
            script += childColumn[i] + ") references [" + parentTable + "] (";
            for (j = 0; j < parentColumn.Count - 1; j++)
            {
                script += parentColumn[j] + ",";
            }
            script += parentColumn[j] + " )" + "\r\n";

            script += "GO" + "\r\n\r\n";
            return script;
        }

        //Drop
        private string DropDatabase(string dbName)
        {
            string script = "If EXISTS (Select NAME From SYSDATABASES Where NAME ='" + dbName + "')" + "\r\n";
            script += "\t" + "Drop database " + dbName + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        private string DropTable(string tableName)
        {
            string script = "";
            script += "If EXISTS (Select NAME From SYSOBJECTS Where NAME ='" + tableName + "')" + "\r\n";
            script += "\t" + "Drop table [" + tableName + "]\r\n"; ;
            script += "GO" + "\r\n";
            return script;
        }
        private string DropForeignKey(string fkName, string tableReference)
        {
            string script = "";
            script += "If EXISTS (Select NAME From SYSOBJECTS Where NAME ='" + fkName + "')" + "\r\n";
            script += "\t" + "Alter table [dbo].[" + tableReference + "] drop constraint [" + fkName + "]" + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }

        //Generate
        private string GenerateTables(List<string> scriptsCreateTable, List<string> scriptsCreatePK)
        {
            string script = "";
            for (int i = 0; i < scriptsCreateTable.Count; i++)
            {
                script += scriptsCreateTable[i];
                script += scriptsCreatePK[i];
            }
            return script;
        }
        private string GenerateDropForeignKeys(List<string> scriptsDropForeignKey)
        {
            string script = "";
            foreach (string str in scriptsDropForeignKey)
                script += str;
            return script;
        }
        private string GenerateForeignKeys(List<string> scriptsCreateForeignKey)
        {
            string script = "";
            script += "/*==============================================================*/" + "\r\n";
            script += "/* Foreign Keys                                                 */" + "\r\n";
            script += "/*==============================================================*/" + "\r\n";
            foreach (string str in scriptsCreateForeignKey)
                script += str;
            return script;
        }
        private string DataTypeLenght(string dataType, string dataTypeLenght)
        {
            string temp = "";
            switch (dataType)
            {
                case "bigint":
                case "bit":
                case "datetime":
                case "decimal":
                case "float":
                case "image":
                case "int":
                case "money":
                case "ntext":
                case "numeric":
                case "real":
                case "smalldatetime":
                case "smallint":
                case "sqlvariant":
                case "text":
                case "timestamp":
                case "tinyint":
                case "uniqueidentifier": temp = dataType;
                    break;

                case "varbinary"://1 - 8000                   
                case "varchar"://1 - 8000                   
                case "binary": //1 - 8000                    
                case "char": //1 - 8000
                case "nchar": //1 - 4000                    
                case "nvarchar": temp = dataType + " (" + dataTypeLenght + ")";//1 - 4000
                    break;
            }
            return temp;
        }

    }
}
