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
        
        public void AddColumn(Column col)
        {
            col.IsSimpleKey();
            this.columns.Add(col);
        }
        public void AddColumn(AttributeData ad)
        {
            Column col = new Column(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
            this.columns.Add(col);
        }
        //public void AddColumn(AttributeData ad)
        //{
        //    AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
        //}
        public void AddColumn(List<Column> listColumn)
        {
            foreach (Column col in listColumn)
            {
                if(col.PrimaryKey != true && col.ForeignKey!= true)
                {
                    this.columns.Add(col);
                }
            }
        }
        public void AddColumnFK(Column col)
        {
            col.IsForeignKey();
            this.columns.Add(col);
        }
        public void AddColumnFK(string name, string dataType, int lenght, bool allowNull, string description)
        {
            this.columns.Add(new Column(name, dataType, lenght, allowNull, false, true, description));
        }
        public void AddColumnFK(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                col.IsForeignKey();
                AddColumnFK(col);
            }
        }

        public void AddPrimaryKey(Column col)
        {
            col.IsPrimaryKey();
            this.columns.Add(col);
        }
        public void AddPrimaryKey(string name, string dataType, int lenght, string description)
        {
            this.columns.Add(new Column(name, dataType, lenght, false, true, false, description));
        }
        public void AddPrimaryKey(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                if (!col.PrimaryKey)
                    col.IsPrimaryKey();
                this.columns.Add(col);
            }
        }

        public void AddPrimaryKeyForeignKey(Column col)
        {
            col.IsForeignKey();
            col.IsPrimaryKey();
            this.columns.Add(col);            
        }
        public void AddPrimaryKeyForeignKey(string name, string dataType, int lenght, string description)
        {
            this.columns.Add(new Column(name, dataType, lenght, false, true, true, description));
        }
        public void AddPrimaryKeyForeignKey(List<Column> listCol)
        {
            foreach (Column col in listCol)
            {
                col.IsPrimaryKeyForeignKey();
                this.columns.Add(col);
            }
        }

        public int CountNumberPrimaryKey()
        {
            int temp = 0;
            foreach (Column c in columns)
            {
                if (c.PrimaryKey)
                    temp++;
            }
            return temp;
        }

        public List<Column> GetPrimaryKey()
        {
            List<Column> temp = new List<Column>();
            foreach (Column c in columns)            
                if (c.PrimaryKey)               
                    temp.Add(new Column(c.Name,c.DataType,c.Length,c.AlowNull,c.PrimaryKey,false,c.Description));                       
            return temp;
        }
        public List<Column> GetPrimaryKey(EntityData ed)
        {
            List<Column> listPK = new List<Column>();
            foreach (AttributeData ad in ed.Attributes)
                if (ad.type == AttributeType.Key)
                {
                    if (ad.AttributeChilds.Count > 0)
                    {
                        foreach (AttributeData adChild in ad.AttributeChilds)
                            listPK.Add(new Column(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, true, false, adChild.Description));
                    }
                    else
                        listPK.Add(new Column(ad.name, ad.DataType, ad.Length, ad.AllowNull, true, false, ad.Description));

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
        public string Description;
        public bool PrimaryKey = false;
        public bool ForeignKey = false;

        //Constructors
        public Column() { }  
        public Column(string name, string dataType, int lenght, bool allowNull, string description)
        {
            Name = name;
            DataType = dataType;
            Length = lenght;
            AlowNull = allowNull;
            Description = description;
            PrimaryKey = false;
            ForeignKey = false;
        }       
        
        public Column(string name, string dataType, int lenght, bool allowNull, bool primaryKey, bool foreignKey, string description)
        {
            Name = name;
            DataType = dataType;
            Length = lenght;
            AlowNull = allowNull;
            Description = description;
            PrimaryKey = primaryKey;
            ForeignKey = foreignKey;
        }

        //Methods               
        public void IsSimpleKey()
        {
            ForeignKey = false;
            PrimaryKey = false;
        }
        public void IsForeignKey()
        {
            ForeignKey = true;
            PrimaryKey = false;
        }
        public void IsPrimaryKey()
        {
            ForeignKey = false;
            PrimaryKey = true;
            AlowNull = false;
        }
        public void IsPrimaryKeyForeignKey()
        {
            ForeignKey = true;
            PrimaryKey = true; 
            AlowNull = false;
        }
        public string DBMSDataType
        {
            get
            {
                return new DBProviderBase().getDataType(DataType);
            }
        }
        public static List<Column> CopyColumn(List<Column> listCol)
        {
            List<Column> listTemp = new List<Column>();
            foreach (Column c in listCol)
            {
                Column colTemp = new Column(c.Name, c.DataType, c.Length, c.AlowNull, c.PrimaryKey, c.ForeignKey, c.Description);
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
        public Table SearchTable(string nameTable)
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
        public bool HasTable(string tableName)
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
        public void AddTable(Table t)
        {           
            this.Tables.Add(t);
        }       
        public void AddForeignKey(string relationshipName,Table parentTable, List<Column> listColumnPK,Table childTable,List<Column> listColumnFK)
        {
            string nameFK = "";
            //Tên dài quá Oracle không chạy được
            //if (relationshipName != "")
            //{
            //    if (listColumnFK.Count == 1)
            //        nameFK = "fk" + "_" + listColumnFK[0].Name + "_" + parentTable.name.Substring(0, 5) + "_" + listColumnPK[0].Name;
            //    else
            //        nameFK = "fk" + "_" + childTable.name.Substring(0, 5) + "_" + relationshipName.Substring(0,5) + "_" + parentTable.name.Substring(0, 5) + "_" + listColumnPK.Count;
            //}
            //else
            //{
            //    if (listColumnFK.Count == 1)
            //        nameFK = "fk" + "_" + listColumnFK[0].Name + "_" + parentTable.name.Substring(0, 5) + "_" + listColumnPK[0].Name;
            //    else
            //        nameFK = "fk" + "_" + childTable.name.Substring(0, 5) + "_" + parentTable.name.Substring(0, 5) + "_" + listColumnPK.Count;
            //}

            nameFK = "fk" + "_" + parentTable.name.Substring(0, 5) + System.Guid.NewGuid().ToString().Replace("-","").Substring(10);
            
            
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

        public void ConvertEntityStrongToTable(EntityData ed)
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
                            t.AddColumn(ac);                          
                        if (isTypeColumn)//PK
                            t.AddPrimaryKey(ac.name, ac.DataType, ac.Length, ac.Description);                        
                    }
                }
                //Key, Simple, Multi Attribute
                else
                {                
                    switch (ad.type)
                    {
                        case AttributeType.Key:
                            t.AddPrimaryKey(ad.name, ad.DataType, ad.Length, ad.Description);
                            break;
                        case AttributeType.Simple:
                            t.AddColumn(ad);                            
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
                            List<Column> listPK2 = t2.GetPrimaryKey(ed);
                            
                            //Column Multi Value là khóa chính
                            Column c3 = new Column(ad.name, ad.DataType, ad.Length,false,true,false, ad.Description);
                           
                            //Thêm 2 column trên vào Table t2
                            t2.AddPrimaryKeyForeignKey(listPK2);
                            t2.AddPrimaryKey(c3);

                            //Bật cờ báo hiệu Entity này có Attribute MultiValue
                            flagAttMuliValue = true;
                            break;
                    }
                }
            }
            if (flagAttMuliValue)
            {                
                this.AddTable(t);
                this.AddTable(t2);
                this.AddForeignKey("", t, t.GetPrimaryKey(), t2, t.GetPrimaryKey());
            }
            else
            {
                this.AddTable(t);
            }            
        }
    }
    
}
