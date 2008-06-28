using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class Table
    {
        public string name;
        public List<Column> columns = new List<Column>();
        public int x, y, w, h;

        //Constructors
        public Table() { }               
        public Table(string n, int X, int Y, int Weidth, int Height)
        {
            name = n;
            x = X;
            y = Y;
            w = Weidth;
            h = Height;
        }

        //Methods
        
        public void addColumn(Column col)
        {
            col.isSimpleKey();
            this.columns.Add(col);
        }
        public void addColumn(string name, string dataType, int lenght, bool allowNull)
        {
            Column col = new Column(name, dataType, lenght, allowNull);
            this.columns.Add(col);
        }
        public void addColumn(List<Column> listColumn)
        {
            foreach (Column col in listColumn)
            {
                if(col.PrimaryKey != true && col.ForeignKey!= true)
                {
                    this.columns.Add(col);
                }
            }
        }
        public void addColumnFK(Column col)
        {
            col.isForeignKey();
            this.columns.Add(col);
        }
        public void addColumnFK(string name, string dataType, int lenght, bool allowNull)
        {
            this.columns.Add(new Column(name,dataType,lenght,allowNull,false,true));
        }
        public void addColumnFK(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                col.isForeignKey();
                addColumnFK(col);
            }
        }

        public void addPrimaryKey(Column col)
        {
            col.isPrimaryKey();
            this.columns.Add(col);
        }
        public void addPrimaryKey(string name, string dataType, int lenght)
        {
            this.columns.Add(new Column(name, dataType, lenght, false,true,false));
        }
        public void addPrimaryKey(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                if (!col.PrimaryKey)
                    col.isPrimaryKey();
                this.columns.Add(col);
            }
        }

        public void addPrimaryKeyForeignKey(Column col)
        {
            col.isForeignKey();
            col.isPrimaryKey();
            this.columns.Add(col);            
        }
        public void addPrimaryKeyForeignKey(string name, string dataType, int lenght)
        {
            this.columns.Add(new Column(name,dataType,lenght,false,true,true));
        }
        public void addPrimaryKeyForeignKey(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                col.isPrimaryKeyForeignKey();
                this.columns.Add(col);
            }
        }

        public int countNumberPrimaryKey()
        {
            int temp = 0;
            foreach (Column c in columns)
            {
                if (c.PrimaryKey)
                    temp++;
            }
            return temp;
        }

        public List<Column> getPrimaryKey()
        {
            List<Column> temp = new List<Column>();
            foreach (Column c in columns)            
                if (c.PrimaryKey)               
                    temp.Add(new Column(c.Name,c.DataType,c.Length,c.AlowNull,c.PrimaryKey,false));                       
            return temp;
        }
        public List<Column> getPrimaryKey(EntityData ed)
        {
            List<Column> listPK = new List<Column>();
            foreach (AttributeData ad in ed.Attributes)
                if (ad.type == AttributeType.Key)
                {
                    if (ad.AttributeChilds.Count > 0)
                    {
                        foreach (AttributeData adChild in ad.AttributeChilds)
                            listPK.Add(new Column(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, true, false));
                    }
                    else
                        listPK.Add(new Column(ad.name, ad.DataType, ad.Length, ad.AllowNull, true, false));

                }
            return listPK;            
        }               
    }
    public class WeakTable
    {

    }
    public class Column
    {
        public string Name;
        public string DataType;
        public int Length;
        public bool AlowNull = true;
        public bool PrimaryKey = false;
        public bool ForeignKey = false;

        //Constructors
        public Column() { }  
        public Column(string name, string dataType, int lenght, bool allowNull)
        {
            Name = name;
            DataType = dataType;
            Length = lenght;
            AlowNull = allowNull;
            PrimaryKey = false;
            ForeignKey = false;
        }       
        
        public Column(string name, string dataType, int lenght, bool allowNull, bool primaryKey, bool foreignKey)
        {
            Name = name;
            DataType = dataType;
            Length = lenght;
            AlowNull = allowNull;
            PrimaryKey = primaryKey;
            ForeignKey = foreignKey;
        }

        //Methods               
        public void isSimpleKey()
        {
            ForeignKey = false;
            PrimaryKey = false;
        }
        public void isForeignKey()
        {
            ForeignKey = true;
            PrimaryKey = false;
        }
        public void isPrimaryKey()
        {
            ForeignKey = false;
            PrimaryKey = true;
            AlowNull = false;
        }
        public void isPrimaryKeyForeignKey()
        {
            ForeignKey = true;
            PrimaryKey = true; 
            AlowNull = false;
        }

        public static List<Column> copyColumn(List<Column> listCol)
        {
            List<Column> listTemp = new List<Column>();
            foreach (Column c in listCol)
            {
                Column colTemp = new Column(c.Name, c.DataType, c.Length, c.AlowNull, c.PrimaryKey, c.ForeignKey);
                listTemp.Add(colTemp);
            }
            return listTemp;
        }
    }

    public class ForeignKey
    {
        public string Name;
        public string ParentTable;
        public List<string> ParentColumn = new List<string>();
        public string ChildTable;
        public List<string> ChildColumn = new List<string>();        
    }   
    public class MetaDataPhysical
    {
        //Variables
        public List<Table> Tables = new List<Table>();
        public List<ForeignKey> ForeignKeys = new List<ForeignKey>();     

        //Methods
        public Table searchTable(string nameTable)
        {
            Table temp = new Table();
            foreach (Table t in Tables)
            {
                if (t.name == nameTable)
                {
                    temp = t;
                    break;
                }
            }
            return temp;
        }
        public bool hasTable(string tableName)
        {
            bool flag = false;
            foreach (Table t in Tables)
            {
                if (t.name == tableName)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }
        public void addTable(Table t)
        {           
            this.Tables.Add(t);
        }       
        public void addForeignKey(string relationshipName,Table parentTable, List<Column> listColumnPK,Table childTable,List<Column> listColumnFK)
        {
            string nameFK = "";
            if (relationshipName != "")
                nameFK = "fk" + "_" + parentTable.name + "_" + relationshipName + "_" + childTable.name;
            else
                nameFK = "fk" + "_" + parentTable.name + "_" + childTable.name;

            ForeignKey fk = new ForeignKey();

            List<string> listColumnPKName = new List<string>();
            List<string> listColumnFKName = new List<string>();

            foreach (Column col1 in listColumnPK)
                listColumnPKName.Add(col1.Name);

            foreach (Column col2 in listColumnFK)
                listColumnFKName.Add(col2.Name);
           
            fk.Name = nameFK;
            fk.ParentTable = parentTable.name;
            fk.ParentColumn = listColumnPKName;
            fk.ChildTable = childTable.name;
            fk.ChildColumn = listColumnFKName;
            this.ForeignKeys.Add(fk);
        }

        public void convertEntityStrongToTable(EntityData ed)
        {           
            Table t = new Table(ed.name, ed.x, ed.y, ed.w, ed.h);
            Table t2 = new Table();

            bool flagAttMuliValue = false;

            //Process Column
            foreach (AttributeData ad in ed.Attributes)
            {
                //Composite Attribute
                if (ad.AttributeChilds.Count > 0)
                {
                    bool isTypeColumn = false;//False->Simple, True->Key
                    if (ad.type == AttributeType.Key)
                        isTypeColumn = true;
                    if (ad.type == AttributeType.Simple)
                        isTypeColumn = false;
                    foreach (AttributeData ac in ad.AttributeChilds)
                    {
                        if (!isTypeColumn)//Simple      
                            t.addColumn(ac.name, ac.DataType, ac.Length, ac.AllowNull);                          
                        if (isTypeColumn)//PK
                            t.addPrimaryKey(ac.name, ac.DataType, ac.Length);                        
                    }
                }
                //Key, Simple, Multi Attribute
                else
                {                
                    switch (ad.type)
                    {
                        case AttributeType.Key:
                            t.addPrimaryKey(ad.name, ad.DataType, ad.Length);
                            break;
                        case AttributeType.Simple:
                            t.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);                            
                            break;
                        case AttributeType.MultiValued:
                            //Xử lý cái Table thứ hai

                            //Table chứa thuộc tính đa trị và khóa chính
                            string nameTable = ed.name + "_" + (ad.name).ToUpper();
                            t2.name = nameTable;
                            t2.x = t.x+20;
                            t2.y = t.y-40;
                            t2.h = t.h;
                            t2.w = t.w;
                            //Tim column Key.
                            List<Column> listPK2 = t2.getPrimaryKey(ed);
                            
                            //Column Multi Value là khóa chính
                            Column c3 = new Column(ad.name, ad.DataType, ad.Length,false,true,false);
                           
                            //Thêm 2 column trên vào Table t2
                            t2.addPrimaryKeyForeignKey(listPK2);
                            t2.addPrimaryKey(c3);

                            //Bật cờ báo hiệu Entity này có Attribute MultiValue
                            flagAttMuliValue = true;
                            break;
                    }
                }
            }
            if (flagAttMuliValue)
            {                
                this.addTable(t);
                this.addTable(t2);
                this.addForeignKey("", t, t.getPrimaryKey(), t2, t.getPrimaryKey());
            }
            else
            {
                this.addTable(t);
            }            
        }
    }
    
}
