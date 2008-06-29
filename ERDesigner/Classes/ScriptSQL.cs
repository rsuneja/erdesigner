using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{ 
    class ScriptSQL
    {
        //getAlowNull truyền vào giá trị bool trả về chuỗi tương ứng: True - null, False - not null
        private static string getAlowNull(bool p)
        {
            string strAlowNull = "";
            if (p)
                strAlowNull = "null";
            else
                strAlowNull = "not null";
            return strAlowNull;
        }        

        //Create
        public static string createDataBase(string dbName)
        {
            string script = "";
            script += "Use Master" + "\r\n";
            script += "GO" + "\r\n";
            script += ScriptSQL.dropDatabase(dbName);
            script += "Create database [" + dbName + "]\r\n";
            script += "GO" + "\r\n";
            script += "Use [" + dbName + "]\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        public static string createTable(string tableName,List<Column> listColumn)
        {
            
            string temp1 = "";
            temp1 = "/*" + temp1.PadRight(50, '=') + "*/ \r\n";
            string temp2 = "Table: "+tableName+"";
            temp2 = "/*" + temp2.PadRight(50, ' ') + "*/ \r\n";
            
            string script = "";
            script += temp1;
            script += temp2;
            script += temp1;
            script += ScriptSQL.dropTable(tableName);
            script += "Create table [" + tableName + "]\r\n";
            script += "(" + "\r\n";
            int i;
            string dataType = "";
            for(i=0;i<listColumn.Count-1;i++)
            {
                Column col = listColumn[i];
                string strAlowNull = getAlowNull(col.AlowNull);
                dataType = dataTypeLenght(col.DataType, col.Length.ToString());
                script += "\t" + col.Name + " " + dataType + " " + strAlowNull + " ," + "\r\n";
            }
            dataType = dataTypeLenght(listColumn[i].DataType, listColumn[i].Length.ToString());
            script += "\t" + listColumn[i].Name + " " + dataType + " " + getAlowNull(listColumn[i].AlowNull) + "\r\n";
            script += ")" + "\r\n"; 
            script += "GO" + "\r\n\r\n";
            return script;
        }      
        public static string createPrimaryKey(string tableName, List<string> columnName)
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
        public static string createForeignKey(string fkName,string parentTable,List<string> parentColumn,string childTable,List<string>  childColumn)
        {
            string script = "";
            script += "Alter table [" + childTable + "]\r\n";
            script += "   add constraint " + fkName + " foreign key (";
            int i=0,j=0;
            for (i = 0; i < childColumn.Count - 1; i++)
            {
                script += childColumn[i] + ",";
            }
            script += childColumn[i] + ") references [" + parentTable + "] (" ;
            for (j = 0; j < parentColumn.Count - 1; j++)
            {
                script += parentColumn[j] + ",";
            }
            script += parentColumn[j] + " )" + "\r\n";
            
            script += "GO" + "\r\n\r\n";
            return script;
        }
        
        //Drop
        public static string dropDatabase(string dbName)
        {
            string script = "If EXISTS (Select NAME From SYSDATABASES Where NAME ='" + dbName + "')" + "\r\n";
            script += "\t" + "Drop database " + dbName + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }
        public static string dropTable(string tableName)
        {
            string script = "";
            script += "If EXISTS (Select NAME From SYSOBJECTS Where NAME ='" + tableName + "')" + "\r\n";
            script += "\t" + "Drop table [" + tableName + "]\r\n"; ;
            script += "GO" + "\r\n";
            return script;
        }
        public static string dropForeignKey(string fkName,string tableReference)
        {            
            string script = "";
            script += "If EXISTS (Select NAME From SYSOBJECTS Where NAME ='" + fkName + "')" + "\r\n";
            script += "\t" + "Alter table [dbo].[" + tableReference + "] drop constraint [" + fkName + "]" + "\r\n";
            script += "GO" + "\r\n\r\n";
            return script;
        }

        //Generate
        public static string generateTables(List<string> scriptsCreateTable, List<string> scriptsCreatePK)
        {
            string script = "";
            for (int i = 0; i < scriptsCreateTable.Count; i++)
            {
                script += scriptsCreateTable[i];
                script += scriptsCreatePK[i];
            }
            return script;
        }
        public static string generateDropForeignKeys(List<string> scriptsDropForeignKey)
        {
            string script = "";
            foreach (string str in scriptsDropForeignKey)
                script += str;
            return script;
        }
        public static string generateForeignKeys(List<string> scriptsCreateForeignKey)
        {
            string script = "";
            script += "/*==============================================================*/" + "\r\n";
            script += "/* Foreign Keys                                                 */" + "\r\n";
            script += "/*==============================================================*/" + "\r\n";
            foreach (string str in scriptsCreateForeignKey)
                script += str;
            return script;
        }
        private static string dataTypeLenght(string dataType,string dataTypeLenght)
        {
            string temp ="";
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
