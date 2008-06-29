using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class GeneratePhysicalModel
    {
        //Variables
        public MetaDataPhysical mdp = new MetaDataPhysical();
        private MetaData erd= new MetaData();
        private JSerializer js = new JSerializer();

        //Constructors
        public GeneratePhysicalModel(MetaData md)
        {            
            erd = md;
        }
        public GeneratePhysicalModel(string url)
        {            
            erd = js.loadFromXML(url);
        }

        //Methods        
        public void process()
        {
            #region Variables
            List<string> entityHadCreated = new List<string>();
            #endregion

            //Process
            
            //Entities
            #region Entities
            foreach (EntityData ed in erd.Entities)
            {                
                //Kiểm tra coi Entity thuộc loại nào
                //Tiến hành xử lý , Entity Strong, Entity Weak
                //Entity Strong: Normal, Has Composite Attribute, Has MultiValue Attribute.
                                
                #region EntityStrong
                if (ed.type == EntityType.Strong)//Entity Strong
                {
                    //Kiểm tra Entity này tạo thành Table chưa, nếu chưa mới thực hiện
                    if (!searchInList(entityHadCreated, ed.name))
                    {
                        mdp.convertEntityStrongToTable(ed);
                        entityHadCreated.Add(ed.name);
                    }
                }
                #endregion

                #region EntityWeak
                if (ed.type == EntityType.Weak)//Entity Weak
                {
                    //Kiểm tra xem thằng Parent có tồn tại hay chưa
                    //Nếu chưa thì phải tạo Parent trước
                    //Sau đó tạo Weak

                    string nEntityWeak = ed.name;
                    string nEntityParent = searchEntityParent(nEntityWeak);

                    if (!mdp.hasTable(nEntityParent) && nEntityParent != "")
                    {
                        EntityData edParent = searchEntityFromERD(nEntityParent);
                        mdp.convertEntityStrongToTable(edParent);
                        entityHadCreated.Add(edParent.name);
                    }

                    Table weak = new Table(ed.name, ed.x, ed.y, ed.w, ed.h);                    
                    Table parent = mdp.searchTable(nEntityParent);
                                      
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
                                    weak.addColumn(ac.name, ac.DataType, ac.Length, ac.AllowNull);                                                                    
                                if (isTypeColumn)//PK
                                    weak.addPrimaryKey(ac.name, ac.DataType, ac.Length);                                  
                            }
                        }
                        //Key, Simple, Multi Attribute
                        else
                        {                           
                            switch (ad.type)
                            {
                                case AttributeType.Key:
                                    weak.addPrimaryKey(ad.name, ad.DataType, ad.Length);                                                               
                                    break;
                                case AttributeType.Simple:
                                    weak.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);                                    
                                    break;                               
                            }
                        }
                    }

                    //Chuyển PK bảng Parent thành FK bảng Weak
                    List<Column> pkParent = parent.getPrimaryKey();
                    weak.addPrimaryKeyForeignKey(pkParent);
                    //Add bảng Weak vào MetaDataPhysical
                    mdp.addTable(weak);                 
                    //Tạo quan hệ khóa ngoại cho bảng Parent và Weak
                    mdp.addForeignKey("", parent, pkParent, weak, pkParent);             
                }
                #endregion
            }
            #endregion

            #region Relationships
            foreach (RelationshipData rd in erd.Relationships)
            {
                //Lấy Name Relationship , Type Cardinality
                //Xét xem nằm loại nào : 1-n , n-n, 1-1.
                    string cardinalityMin1 = "";
                    string cardinalityMax1 = "";
                    string nameTable1 = "";

                    string cardinalityMin2 = "";
                    string cardinalityMax2 = "";
                    string nameTable2 = "";

                    string nameRelationship = rd.name;

                //Duyệt lấy các giá trị của 1 Relationship
                    for(int i=0; i<rd.Cardinalities.Count; i++)
                    {
                        if (i % 2 == 0)
                        {
                            cardinalityMin1 = rd.Cardinalities[i].MinCardinality.ToString();
                            cardinalityMax1 = rd.Cardinalities[i].MaxCardinality.ToString();
                            nameTable1 = rd.Cardinalities[i].Entity;
                        }
                        else
                        {
                            cardinalityMin2 = rd.Cardinalities[i].MinCardinality.ToString();
                            cardinalityMax2 = rd.Cardinalities[i].MaxCardinality.ToString();
                            nameTable2 = rd.Cardinalities[i].Entity;
                        }
                    }

                   
                    #region Trường hợp 1 ngôi
                    if (nameTable1 == nameTable2)
                    {
                        //Trường hợp 1:n, n:1
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "-1")
                        {
                            Table t1 = mdp.searchTable(nameTable1);                            
                            List<Column> pk1 = t1.getPrimaryKey();

                            if (rd.Attributes.Count > 0)
                            {
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    if (ad.type == AttributeType.Simple)
                                        t1.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);                                    
                                }
                            }
                            List<Column> listFK = new List<Column>();
                            for (int i = 0; i < pk1.Count; i++)
                            {   
                                Column col = new Column();
                                col.AlowNull = pk1[i].AlowNull;
                                col.DataType = pk1[i].DataType;
                                col.Length = pk1[i].Length;
                                col.PrimaryKey = pk1[i].PrimaryKey;
                                col.ForeignKey = pk1[i].ForeignKey;
                                if (i > 0)
                                    col.Name = nameRelationship + "_" + i;
                                else
                                    col.Name = nameRelationship;                                
                                listFK.Add(col);
                            }
                            t1.addColumnFK(listFK);
                            mdp.addForeignKey(nameRelationship, t1, pk1, t1, listFK);                           
                        }
                   
                        //Trường hợp n:n
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1")
                        {

                        }
                    }
                    #endregion

                    #region Trường hợp 2 ngôi
                    if (nameTable1 != nameTable2)
                    {
                        //Trường hợp relationship bình thường
                        if (rd.type == RelationshipType.Normal)
                        {
                            //Xét những trường hợp:
                            //1:n
                            //n:1                    
                            //n:n
                            //1:1
                            if (cardinalityMax1 == "1" && cardinalityMax2 == "-1")
                            {
                                //Tìm table tren MDP
                                //them coloum vao` table
                                //tao Relationship
                                //Them Relationship vao

                                Table t1 = mdp.searchTable(nameTable1);//Bảng một
                                Table t2 = mdp.searchTable(nameTable2);//Bảng nhiều

                                List<Column> pkT1 = t1.getPrimaryKey();
                                t2.addColumnFK(pkT1);
                                
                                if (rd.Attributes.Count > 0)
                                {
                                    //Thiếu trường hợp Multivalue
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple)
                                            t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);

                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)                                            
                                                t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);                                            
                                        }
                                    }
                                }
                                mdp.addForeignKey(nameRelationship, t1, pkT1, t2, pkT1);                                                               
                            }
                            if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")
                            {
                                Table t1 = mdp.searchTable(nameTable2);//Bảng một
                                Table t2 = mdp.searchTable(nameTable1);//Bảng nhiều

                                List<Column> pkT1 = t1.getPrimaryKey();
                                t2.addColumnFK(pkT1);

                                if (rd.Attributes.Count > 0)
                                {
                                    //Thiếu trường hợp Multivalue
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple)
                                            t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);

                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);
                                        }
                                    }
                                }
                                mdp.addForeignKey(nameRelationship, t1, pkT1, t2, pkT1);                                                               
                            }
                            if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1")
                            {
                                //Tìm Table tren MDP
                                //Create table trung gian (Relasionship)
                                //Tạo khóa chính gồm 2 khóa. 
                                //Thêm vào MDP   
                                //Tạo 2 fk vào MDP
                                Table t1 = mdp.searchTable(nameTable1);
                                Table t2 = mdp.searchTable(nameTable2);
                                                                
                                List<Column> pk1 = t1.getPrimaryKey();
                                List<Column> pk2 = t2.getPrimaryKey();

                                Table temp = new Table((t1.name).ToUpper() + "_" + (t2.name).ToUpper(),rd.x,rd.y,rd.w,rd.h);

                                temp.addColumnFK(pk1);
                                temp.addColumnFK(pk2);
                               
                                if (rd.Attributes.Count > 0)
                                {
                                    //Thiếu trường hợp Composite, Multivalue
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple)
                                            temp.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);
                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {                                                                                        
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                temp.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);                                            
                                        }                                        
                                    }                                    
                                }
                                mdp.addTable(temp);
                                mdp.addForeignKey(nameRelationship, t1, pk1, temp, pk1);
                                mdp.addForeignKey(nameRelationship, t2, pk2, temp, pk2);                                
                            }
                            if (cardinalityMax1 == "1" && cardinalityMax2 == "1")
                            {
                                //Trường hợp 0:1
                                if (cardinalityMin1 == "0" && cardinalityMin2 == "1")
                                {
                                    Table t1 = mdp.searchTable(nameTable2);//Bảng 1
                                    Table t2 = mdp.searchTable(nameTable1);//Bảng 0

                                    List<Column> pk1 = t1.getPrimaryKey();
                                    if (rd.Attributes.Count > 0)
                                    {
                                        foreach (AttributeData ad in rd.Attributes)
                                        {
                                            //Simple Attribute
                                            if (ad.type == AttributeType.Simple)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);
                                            }
                                        }
                                    }
                                    //Chuyển PK bảng 1 thành FK bảng 0
                                    t2.addColumnFK(pk1);
                                    mdp.addForeignKey(nameRelationship, t1, pk1, t2, pk1);                                
                                }
                                //Trường hợp 1:0
                                if (cardinalityMin2 == "0" && cardinalityMin1 == "1")
                                {
                                    Table t1 = mdp.searchTable(nameTable1);//Bảng 1
                                    Table t2 = mdp.searchTable(nameTable2);//Bảng 0

                                    List<Column> pk1 = t1.getPrimaryKey();
                                    if (rd.Attributes.Count > 0)
                                    {
                                        foreach (AttributeData ad in rd.Attributes)
                                        {
                                            //Simple Attribute
                                            if (ad.type == AttributeType.Simple)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);
                                            }                                                             
                                        }
                                    }
                                    //Chuyển PK bảng 1 thành FK bảng 0
                                    t2.addColumnFK(pk1);
                                    mdp.addForeignKey(nameRelationship, t1, pk1, t2, pk1);                                
                                }
                                //Trường hợp 1:1
                                if ((cardinalityMin1 == "1" && cardinalityMin2 == "1") || (cardinalityMin1 == "0" && cardinalityMin2 == "0"))
                                {
                                    Table t1 = mdp.searchTable(nameTable1);//Bảng 1
                                    Table t2 = mdp.searchTable(nameTable2);//Bảng 0

                                    List<Column> pk1 = t1.getPrimaryKey();
                                    if (rd.Attributes.Count > 0)
                                    {
                                        foreach (AttributeData ad in rd.Attributes)
                                        {
                                            //Simple Attribute
                                            if (ad.type == AttributeType.Simple)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull);
                                            }
                                        }
                                    }
                                    //Chuyển PK bảng 1 thành FK bảng 0
                                    t2.addColumnFK(pk1);
                                    mdp.addForeignKey(nameRelationship, t1, pk1, t2, pk1);                                
                                }
                            }
                        }

                        //Trường hợp Entity Relationship
                        if (rd.type == RelationshipType.AssociativeEntity)
                        {
                            //Nếu có khóa chính thì Add 2 key quan hệ là FK
                            //Không có khóa chính thì tạo Compsite Key từ 2 entity quan hệ                                                    

                            Table t1 = mdp.searchTable(nameTable1);
                            Table t2 = mdp.searchTable(nameTable2);

                            List<Column> pk1 = t1.getPrimaryKey();
                            List<Column> pk2 = t2.getPrimaryKey();

                            
                            bool hasPK = false;
                            foreach (AttributeData ad in rd.Attributes)
                            {
                                if (ad.type == AttributeType.Key)
                                {
                                    hasPK = true;
                                    break;
                                }
                            }

                            //Tạo Entity Data từ Associative Entity
                            EntityData edTemp = new EntityData(rd.name.ToUpper(), EntityType.Strong, rd.x, rd.y, rd.w, rd.h);
                            
                            if (!hasPK)//Assocaitive Entity ko có PK
                            {
                                foreach (Column col1 in pk1)
                                    edTemp.Attributes.Add(new AttributeData(col1.Name, AttributeType.Key, 0, 0, 0, 0, col1.DataType, col1.Length, col1.AlowNull));
                                foreach (Column col2 in pk2)
                                    edTemp.Attributes.Add(new AttributeData(col2.Name, AttributeType.Key, 0, 0, 0, 0, col2.DataType, col2.Length, col2.AlowNull));

                            }

                            foreach (AttributeData ad in rd.Attributes)
                            {
                                AttributeData adTemp = new AttributeData(ad.name, ad.type, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull);                               
                                edTemp.Attributes.Add(adTemp);
                            }
                            mdp.convertEntityStrongToTable(edTemp);
                       
                            

                            //Lấy Table Associative vừa tạo
                            Table temp = mdp.searchTable(edTemp.name);
                           

                            if (!hasPK)//Không có PK, thì 2 khóa chính từ 2 bảng quan hệ chuyển thành PK,FK
                            {
                                foreach (Column col in temp.columns)
                                    if(col.PrimaryKey)
                                        col.isPrimaryKeyForeignKey();
                            }
                            else//Có khóa, thì 2 List PK1, PK2 của 2 bảng quan hệ được add vào Temp (bảng Associative)
                            {
                                temp.addColumnFK(pk1);
                                temp.addColumnFK(pk2);
                            }
                            
                                            
                            mdp.addForeignKey("", t1, pk1, temp, pk1);
                            mdp.addForeignKey("", t2, pk2, temp, pk2);
                            
                        }
                        //Trường hợp Indentifier, ko cần xét trường hợp này vì trong TH Entity Weak đã xét rồi
                        if (rd.type == RelationshipType.Identifier)
                        {

                        }
                    }//End TH 2 ngôi
                    #endregion
                }//End Relationships
            #endregion

        }//End Method Process

        private string searchEntityParent(string nameEntityWeak)
        {
            //Tìm tên Parent dựa vào RelationShip Indentifier
            //nameEntity danh sách tên Entity trong quan hệ Indentifer
            //indexNameEntityParent là vị trí của Parent
            List<string> nameEntity = new List<string>();            
            int indexNameEntityParent = -1;
            string temp = "";
            int index = -1;
            for(int i = 0; i < erd.Relationships.Count; i++)
            {
                if (erd.Relationships[i].type == RelationshipType.Identifier)
                {
                    foreach (CardinalityData cd in erd.Relationships[i].Cardinalities)
                    {
                        if (cd.Entity == nameEntityWeak)
                        {
                            index = i;
                            break;
                        }
                    }
                    if(index>=0)
                        break;
                }
            }
            if (index >= 0)
            {
                foreach (CardinalityData cd in erd.Relationships[index].Cardinalities)
                {
                    nameEntity.Add(cd.Entity);
                    if (cd.Entity != nameEntityWeak)
                    {
                        indexNameEntityParent = 0;
                        break;
                    }
                    else
                    {
                        indexNameEntityParent = 1;
                    }
                }
                if (indexNameEntityParent == 0)
                    temp = nameEntity[0];
                if (indexNameEntityParent == 1)
                    temp = nameEntity[1];          
            }                    
            return temp;
        }

        private bool searchInList(List<string> list, string name)
        {
            bool t = false;
            foreach (string s in list)
            {
                if (s == name)
                {
                    t = true;
                    break;
                }
            }
            return t;
        }

        private EntityData searchEntityFromERD(string nEntity)
        {
            EntityData temp = new EntityData();
            foreach (EntityData ed in erd.Entities)
            {
                if (ed.name == nEntity)
                {
                    temp = ed;
                    break;
                }
            }
            return temp;
        }

        public void writeXML()
        {
            js.saveToXML(@"C:\aaa.xml", mdp);
        }
        
    }
}
