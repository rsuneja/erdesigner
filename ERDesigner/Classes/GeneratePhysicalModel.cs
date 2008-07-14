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
            erd = js.loadConceptualFromXML(url);
        }

        //Methods        
        public void preProcess()
        {
            foreach (RelationshipData rd in erd.Relationships)
            {
                //Duyệt xem thuộc quan hệ nào
                string cardinalityMin1 = "";
                string cardinalityMax1 = "";
                string nameTable1 = "";

                string cardinalityMin2 = "";
                string cardinalityMax2 = "";
                string nameTable2 = "";

                string nameRelationship = rd.name;
                for (int i = 0; i < rd.Cardinalities.Count; i++)
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
                if (rd.type == RelationshipType.Identifier)
                {
                    if (rd.Attributes.Count > 0)
                    {
                        EntityData weakEntity = new EntityData();
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "-1")                        
                            weakEntity = searchEntityData(nameTable2);                        
                        else
                            if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")                            
                                weakEntity = searchEntityData(nameTable1);
                        if (weakEntity != null)
                        {
                            foreach (AttributeData ad in rd.Attributes)
                            {
                                if (ad.AttributeChilds.Count > 0)//Composite
                                {
                                    if(ad.type == AttributeType.Simple)
                                        foreach (AttributeData adChild in ad.AttributeChilds)
                                            weakEntity.Attributes.Add(adChild);                                    
                                }
                                else
                                {
                                    weakEntity.Attributes.Add(ad);
                                }
                                
                            }
                        }
                        
                    }
                }
                if(rd.Attributes.Count>0)
                {
                    //Kiểm tra trường hợp có Multi Valued Attribute
                    bool flag = false;
                    foreach(AttributeData adChild in rd.Attributes)
                    {
                        if(adChild.type == AttributeType.MultiValued)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        //Trường hợp 2 ngôi
                        if (nameTable1 != nameTable2)
                        {
                            if (rd.type == RelationshipType.Normal)
                            {
                                //TH 1,n
                                if (cardinalityMax1 == "1" && cardinalityMax2 == "-1")
                                {
                                    changeLocationMultiValuedAttribute(rd.Attributes, nameTable2);
                                }
                                //TH n,1
                                if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")
                                {
                                    changeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                                //TH 0,1
                                if (cardinalityMax1 == "0" && cardinalityMax2 == "1")
                                {
                                    changeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                                //TH 1,0
                                if (cardinalityMax1 == "1" && cardinalityMax2 == "0")
                                {
                                    changeLocationMultiValuedAttribute(rd.Attributes, nameTable2);
                                }
                                //TH 1,1
                                if (cardinalityMax1 == "1" && cardinalityMax2 == "1")
                                {
                                    changeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                            }
                        }
                        //Trường hợp 1 ngôi
                        if (nameTable1 == nameTable2)
                        {
                            //TH 1:1, 0:1, 1:0, 1:n, n:1
                            if((cardinalityMax1 == "1" && cardinalityMax2 == "1") || (cardinalityMax1 == "0" && cardinalityMax2 == "1")||(cardinalityMax1 == "1" && cardinalityMax2 == "0")||(cardinalityMax1 == "1" && cardinalityMax2 == "-1")||(cardinalityMax1 == "-1" && cardinalityMax2 == "1"))
                                changeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                            
                        }
                    }//End if(flag)
                }//End if rd.Attributes.Count>0              
            }//End Foreach
        }
        public void process()
        {
            preProcess();
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
                if (ed.type == EntityType.Strong )//Entity Strong
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

                    string nEntityWeak = ed.name;
                    string nEntityParent = searchEntityParent(nEntityWeak);

                    //Kiểm tra xem thằng Parent có tồn tại hay chưa
                    //Nếu chưa thì phải tạo Parent trước
                    //Sau đó tạo Weak

                    if (!searchInList(entityHadCreated, nEntityParent))
                    {
                        EntityData parentEntity = searchEntityData(nEntityParent);
                        mdp.convertEntityStrongToTable(parentEntity);
                        entityHadCreated.Add(nEntityParent);
                    }                

                    Table weak = new Table(ed.name, ed.x, ed.y, ed.w, ed.h);                    
                    Table parent = mdp.searchTable(nEntityParent);
                                      
                    //Process Column
                    foreach (AttributeData ad in ed.Attributes)
                    {
                        //Composite Attribute
                        //Jelda: Chổ này cần kiểm tra đa trị phức hợp chứ không phải chỉ có key với simple
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
                                    weak.addColumn(ac.name, ac.DataType, ac.Length, ac.AllowNull, ac.Description);                                                                    
                                if (isTypeColumn)//PK
                                    weak.addPrimaryKey(ac.name, ac.DataType, ac.Length, ac.Description);                                  
                            }
                        }
                        //Key, Simple, Multi Attribute
                        else
                        {                           
                            switch (ad.type)
                            {
                                case AttributeType.Key:
                                    weak.addPrimaryKey(ad.name, ad.DataType, ad.Length, ad.Description);                                                               
                                    break;
                                case AttributeType.Simple:
                                    weak.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);                                    
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
            }//End Entities

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
                        if ((cardinalityMax1 == "1" && cardinalityMax2 == "-1") || (cardinalityMax1 == "-1" && cardinalityMax2 == "1") || (cardinalityMax1 == "1" && cardinalityMax2 == "1") || (cardinalityMax1 == "0" && cardinalityMax2 == "1") || (cardinalityMax1 == "1" && cardinalityMax2 == "0"))
                        {
                            Table t1 = mdp.searchTable(nameTable1);
                            List<Column> pk1 = t1.getPrimaryKey();

                            if (rd.Attributes.Count > 0)
                            {
                                foreach (AttributeData adChild in rd.Attributes)
                                {
                                    if (adChild.AttributeChilds.Count > 0)//Compsite
                                    {
                                        foreach (AttributeData var in adChild.AttributeChilds)
                                            t1.addColumn(var.name, var.DataType, var.Length, var.AllowNull, var.Description);
                                    }
                                    else//Simple
                                        if (adChild.type == AttributeType.Simple)
                                            t1.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                }
                            }//end if
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
                            if (rd.type == RelationshipType.Normal)
                            {
                                Table t1 = mdp.searchTable(nameTable1);
                                List<Column> pk1 = t1.getPrimaryKey();

                                Table t2 = new Table(nameRelationship, rd.x, rd.y, rd.w, rd.h);

                                t2.addPrimaryKeyForeignKey(pk1);
                                List<Column> pk2 = new List<Column>();
                                for (int i = 0; i < pk1.Count; i++)
                                {
                                    Column c = new Column(pk1[i].Name + "_" + i, pk1[i].DataType, pk1[i].Length, pk1[i].AlowNull, pk1[i].PrimaryKey, pk1[i].ForeignKey, pk1[i].Description);
                                    pk2.Add(c);
                                }
                                t2.addPrimaryKeyForeignKey(pk2);

                                //Add tất cả các Attribute có trên thuộc tính vào bên bản mới sinh
                                if (rd.Attributes.Count > 0)
                                {
                                    foreach (AttributeData adChild in rd.Attributes)
                                    {
                                        if (adChild.AttributeChilds.Count > 0)//Compsite
                                        {
                                            foreach (AttributeData var in adChild.AttributeChilds)
                                                t2.addColumn(var.name, var.DataType, var.Length, var.AllowNull, var.Description);
                                        }
                                        else//Simple
                                            if (adChild.type == AttributeType.Simple)
                                                t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                            else
                                                if (adChild.type == AttributeType.MultiValued)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                                
                                    }
                                }//end if
                                mdp.addTable(t2);
                                mdp.addForeignKey(nameRelationship + "_1", t1, pk1, t2, pk1);
                                mdp.addForeignKey(nameRelationship + "_2", t1, pk1, t2, pk2);

                            }
                            if (rd.type == RelationshipType.AssociativeEntity)
                            {
                                
                            }
                        }
                    }//End 1 ngôi
                    #endregion

                    #region Trường hợp 2 ngôi
                    if (nameTable1 != nameTable2)
                    {
                        #region Normal Relationship
                        //Trường hợp relationship bình thường
                        if (rd.type == RelationshipType.Normal)
                        {
                            //TH 1:n                            
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
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);

                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        }
                                    }
                                }
                                mdp.addForeignKey(nameRelationship, t1, pkT1, t2, pkT1);
                            }
                            //TH n,1
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
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);

                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        }
                                    }
                                }
                                mdp.addForeignKey(nameRelationship, t1, pkT1, t2, pkT1);
                            }
                            //TH n:n
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

                                Table temp = new Table((t1.name).ToUpper() + "_" + (t2.name).ToUpper(), rd.x, rd.y, rd.w, rd.h);

                                temp.addPrimaryKeyForeignKey(pk1);
                                temp.addPrimaryKeyForeignKey(pk2);

                                if (rd.Attributes.Count > 0)
                                {
                                    //Thiếu trường hợp Multivalue
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            temp.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                temp.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, ad.Description);
                                        }
                                    }
                                }
                                mdp.addTable(temp);
                                mdp.addForeignKey(nameRelationship, t1, pk1, temp, pk1);
                                mdp.addForeignKey(nameRelationship, t2, pk2, temp, pk2);
                            }
                            //TH 1,1
                            if (cardinalityMax1 == "1" && cardinalityMax2 == "1")
                            {
                                //TH 0:1
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
                                            if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                            }
                                        }
                                    }
                                    //Chuyển PK bảng 1 thành FK bảng 0
                                    t2.addColumnFK(pk1);
                                    mdp.addForeignKey(nameRelationship, t1, pk1, t2, pk1);
                                }
                                //TH 1:0
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
                                            if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
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
                                            if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                                t2.addColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                            //Composite Attribute
                                            if (ad.AttributeChilds.Count > 0)
                                            {
                                                foreach (AttributeData adChild in ad.AttributeChilds)
                                                    t2.addColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                            }
                                        }
                                    }
                                    //Chuyển PK bảng 1 thành FK bảng 0
                                    t2.addColumnFK(pk1);
                                    mdp.addForeignKey(nameRelationship, t1, pk1, t2, pk1);
                                }
                            }
                        }//End Normal Relationship
                        #endregion
                       
                        #region Associative Entity
                        //Trường hợp Associative Entity                       
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
                                    edTemp.Attributes.Add(new AttributeData(col1.Name, AttributeType.Key, 0, 0, 0, 0, col1.DataType, col1.Length, col1.AlowNull, col1.Description));
                                foreach (Column col2 in pk2)
                                    edTemp.Attributes.Add(new AttributeData(col2.Name, AttributeType.Key, 0, 0, 0, 0, col2.DataType, col2.Length, col2.AlowNull, col2.Description));

                            }
                            int numPK = 0;
                            int numSimple = 0;
                            int numMulitiValue = 0;
                            //Đếm loại Attribute
                            foreach (AttributeData var in rd.Attributes)
                            {
                                if (var.AttributeChilds.Count > 0)//Composite Attribute
                                {
                                    if (var.type == AttributeType.Key)
                                        numPK += var.AttributeChilds.Count;
                                    if (var.type == AttributeType.Simple)
                                        numSimple += var.AttributeChilds.Count;
                                }
                                else
                                    if (var.type == AttributeType.Key)
                                        numPK++;
                                    else
                                        if (var.type == AttributeType.Simple)
                                            numSimple++;
                                        else
                                            if (var.type == AttributeType.MultiValued)
                                                numMulitiValue++;

                            }
                            //TH Associative Entity có MultiValued và có số lượng Attribute nhỏ hơn 3
                            if (numSimple >= 0 && numSimple <= 1 && numMulitiValue >= 1)
                            {
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    AttributeData adTemp = new AttributeData();
                                    if (ad.type == AttributeType.MultiValued)
                                        adTemp = new AttributeData(ad.name, AttributeType.Simple, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                    else
                                    {
                                        //Xét Associative Enity có Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            adTemp = new AttributeData(ad.name, ad.type, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                            adTemp.isComposite = true;
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                adTemp.AttributeChilds.Add(adChild);
                                        }
                                        else
                                            adTemp = new AttributeData(ad.name, ad.type, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                    }
                                    edTemp.Attributes.Add(adTemp);
                                }

                            }
                            else//TH Associative Entity có MultiValued và có số lượng Attribute lớn hơn 3    
                            {
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    AttributeData adTemp = new AttributeData();
                                    //Xét Associative Enity có Composite Attribute
                                    if (ad.AttributeChilds.Count > 0)
                                    {
                                        adTemp = new AttributeData(ad.name, ad.type, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                        adTemp.isComposite = true;
                                        foreach (AttributeData adChild in ad.AttributeChilds)
                                            adTemp.AttributeChilds.Add(adChild);
                                    }
                                    else
                                        adTemp = new AttributeData(ad.name, ad.type, ad.x, ad.y, ad.w, ad.h, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                    edTemp.Attributes.Add(adTemp);
                                }
                            }
                            mdp.convertEntityStrongToTable(edTemp);

                            //Lấy Table Associative vừa tạo
                            Table temp = mdp.searchTable(edTemp.name);


                            if (!hasPK)//Không có PK, thì 2 khóa chính từ 2 bảng quan hệ chuyển thành PK,FK
                            {
                                foreach (Column col in temp.columns)
                                    if (col.PrimaryKey)
                                        col.isPrimaryKeyForeignKey();
                            }
                            else//Có khóa, thì 2 List PK1, PK2 của 2 bảng quan hệ được add vào Temp (bảng Associative)
                            {
                                temp.addColumnFK(pk1);
                                temp.addColumnFK(pk2);
                            }


                            mdp.addForeignKey("", t1, pk1, temp, pk1);
                            mdp.addForeignKey("", t2, pk2, temp, pk2);

                        }//End Associative Entity
                        #endregion
                    }//End TH 2 ngôi
                    #endregion
                   

                    
                }//End Relationships
            #endregion

        }//End Method Process

        private EntityData searchEntityData(string nameWeakTable)
        {
            EntityData temp = new EntityData();
            foreach (EntityData ed in erd.Entities)
            {
                if (ed.name == nameWeakTable)
                {
                    temp = ed;
                    break;
                }
            }
            return temp;
        }//End Method Process

        private void changeLocationMultiValuedAttribute(List<AttributeData> listAd,string tableName)
        {
            foreach (AttributeData adChild in listAd)
            {
                if (adChild.type == AttributeType.MultiValued)
                {
                    //Tìm Entity bên nhiều
                    EntityData ed = searchEntityFromERD(tableName);
                    //Gắn Multivalued vào bên tableName
                    ed.Attributes.Add(adChild);
                }
            }

        }
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
                    if (cd.MaxCardinality == 1 && cd.Entity != nameEntityWeak) temp = cd.Entity;
                }                      
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
            js.savePhysicalToXML(@"C:\aaa.xml", mdp);
        }
        
    }
}
