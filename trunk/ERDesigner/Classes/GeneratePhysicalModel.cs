using System;
using System.Collections.Generic;
using System.Text;

namespace ERDesigner
{
    public class GeneratePhysicalModel
    {
        //Variables
        public MetaDataPhysical mdp = new MetaDataPhysical();
        private MetaData erd = new MetaData();
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
                            weakEntity = SearchEntityData(nameTable2);
                        else
                            if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")
                                weakEntity = SearchEntityData(nameTable1);
                        if (weakEntity != null)
                        {
                            foreach (AttributeData ad in rd.Attributes)
                            {
                                if (ad.AttributeChilds.Count > 0)//Composite
                                {
                                    if (ad.type == AttributeType.Simple)
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
                if (rd.Attributes.Count > 0)
                {
                    //Kiểm tra trường hợp có Multi Valued Attribute
                    bool flag = false;
                    foreach (AttributeData adChild in rd.Attributes)
                    {
                        if (adChild.type == AttributeType.MultiValued)
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
                                    ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable2);
                                }
                                //TH n,1
                                if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")
                                {
                                    ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                                //TH 0,1
                                if (cardinalityMax1 == "0" && cardinalityMax2 == "1")
                                {
                                    ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                                //TH 1,0
                                if (cardinalityMax1 == "1" && cardinalityMax2 == "0")
                                {
                                    ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable2);
                                }
                                //TH 1,1
                                if (cardinalityMax1 == "1" && cardinalityMax2 == "1")
                                {
                                    ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
                                }
                            }
                        }
                        //Trường hợp 1 ngôi
                        if (nameTable1 == nameTable2)
                        {
                            //TH 1:1, 0:1, 1:0, 1:n, n:1
                            if ((cardinalityMax1 == "1" && cardinalityMax2 == "1") || (cardinalityMax1 == "0" && cardinalityMax2 == "1") || (cardinalityMax1 == "1" && cardinalityMax2 == "0") || (cardinalityMax1 == "1" && cardinalityMax2 == "-1") || (cardinalityMax1 == "-1" && cardinalityMax2 == "1"))
                                ChangeLocationMultiValuedAttribute(rd.Attributes, nameTable1);
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
                if (ed.type == EntityType.Strong)//Entity Strong
                {
                    //Kiểm tra Entity này tạo thành Table chưa, nếu chưa mới thực hiện
                    if (!SearchInList(entityHadCreated, ed.name))
                    {
                        mdp.ConvertEntityStrongToTable(ed);
                        entityHadCreated.Add(ed.name);
                    }
                }
                #endregion

                #region EntityWeak
                if (ed.type == EntityType.Weak)//Entity Weak
                {

                    string nEntityWeak = ed.name;
                    string nEntityParent = SearchEntityParent(nEntityWeak);

                    //Kiểm tra xem thằng Parent có tồn tại hay chưa
                    //Nếu chưa thì phải tạo Parent trước
                    //Sau đó tạo Weak

                    if (!SearchInList(entityHadCreated, nEntityParent))
                    {
                        EntityData parentEntity = SearchEntityData(nEntityParent);
                        mdp.ConvertEntityStrongToTable(parentEntity);
                        entityHadCreated.Add(nEntityParent);
                    }

                    Table weak = new Table(ed.name, ed.x, ed.y, ed.w, ed.h);
                    Table parent = mdp.SearchTable(nEntityParent);

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
                                    weak.AddColumn(ac.name, ac.DataType, ac.Length, ac.AllowNull, ac.Description);
                                if (isTypeColumn)//PK
                                    weak.AddPrimaryKey(ac.name, ac.DataType, ac.Length, ac.Description);
                            }
                        }
                        //Key, Simple, Multi Attribute
                        else
                        {
                            switch (ad.type)
                            {
                                case AttributeType.Key:
                                    weak.AddPrimaryKey(ad.name, ad.DataType, ad.Length, ad.Description);
                                    break;
                                case AttributeType.Simple:
                                    weak.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                    break;
                            }
                        }
                    }

                    //Chuyển PK bảng Parent thành FK bảng Weak
                    List<Column> pkParent = parent.GetPrimaryKey();
                    weak.AddPrimaryKeyForeignKey(pkParent);
                    //Add bảng Weak vào MetaDataPhysical
                    mdp.AddTable(weak);
                    //Tạo quan hệ khóa ngoại cho bảng Parent và Weak
                    mdp.AddForeignKey("", parent, pkParent, weak, pkParent);
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

                string cardinalityMin3 = "";
                string cardinalityMax3 = "";
                string nameTable3 = "";

                string nameRelationship = rd.name;

                //Duyệt lấy các giá trị của 1 Relationship
                for (int i = 0; i < rd.Cardinalities.Count; i++)
                {
                    if (i == 0)
                    {
                        cardinalityMin1 = rd.Cardinalities[i].MinCardinality.ToString();
                        cardinalityMax1 = rd.Cardinalities[i].MaxCardinality.ToString();
                        nameTable1 = rd.Cardinalities[i].Entity;
                    }
                    else
                    {
                        if (i == 1)
                        {
                            cardinalityMin2 = rd.Cardinalities[i].MinCardinality.ToString();
                            cardinalityMax2 = rd.Cardinalities[i].MaxCardinality.ToString();
                            nameTable2 = rd.Cardinalities[i].Entity;
                        }
                        else
                        {
                            if (i == 2)
                            {
                                cardinalityMin3 = rd.Cardinalities[i].MinCardinality.ToString();
                                cardinalityMax3 = rd.Cardinalities[i].MaxCardinality.ToString();
                                nameTable3 = rd.Cardinalities[i].Entity;
                            }
                        }
                    }
                }

                #region Trường hợp 1 ngôi
                if (rd.Cardinalities.Count == 1)
                {
                    //Trường hợp 1:n, n:1
                    if ((cardinalityMax1 == "1" && cardinalityMax2 == "-1") || (cardinalityMax1 == "-1" && cardinalityMax2 == "1") || (cardinalityMax1 == "1" && cardinalityMax2 == "1") || (cardinalityMax1 == "0" && cardinalityMax2 == "1") || (cardinalityMax1 == "1" && cardinalityMax2 == "0"))
                    {
                        Table t1 = mdp.SearchTable(nameTable1);
                        List<Column> pk1 = t1.GetPrimaryKey();

                        if (rd.Attributes.Count > 0)
                        {
                            foreach (AttributeData adChild in rd.Attributes)
                            {
                                if (adChild.AttributeChilds.Count > 0)//Compsite
                                {
                                    foreach (AttributeData var in adChild.AttributeChilds)
                                        t1.AddColumn(var.name, var.DataType, var.Length, var.AllowNull, var.Description);
                                }
                                else//Simple
                                    if (adChild.type == AttributeType.Simple)
                                        t1.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
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
                        t1.AddColumnFK(listFK);
                        mdp.AddForeignKey(nameRelationship, t1, pk1, t1, listFK);
                    }


                    //Trường hợp n:n
                    if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1")
                    {
                        if (rd.type == RelationshipType.Normal)
                        {
                            Table t1 = mdp.SearchTable(nameTable1);
                            List<Column> pk1 = t1.GetPrimaryKey();

                            Table t2 = new Table(nameRelationship, rd.x, rd.y, rd.w, rd.h);

                            t2.AddPrimaryKeyForeignKey(pk1);
                            List<Column> pk2 = new List<Column>();
                            for (int i = 0; i < pk1.Count; i++)
                            {
                                Column c = new Column(pk1[i].Name + "_" + i, pk1[i].DataType, pk1[i].Length, pk1[i].AlowNull, pk1[i].PrimaryKey, pk1[i].ForeignKey, pk1[i].Description);
                                pk2.Add(c);
                            }
                            t2.AddPrimaryKeyForeignKey(pk2);

                            //Add tất cả các Attribute có trên thuộc tính vào bên bản mới sinh
                            if (rd.Attributes.Count > 0)
                            {
                                foreach (AttributeData adChild in rd.Attributes)
                                {
                                    if (adChild.AttributeChilds.Count > 0)//Compsite
                                    {
                                        foreach (AttributeData var in adChild.AttributeChilds)
                                            t2.AddColumn(var.name, var.DataType, var.Length, var.AllowNull, var.Description);
                                    }
                                    else//Simple
                                        if (adChild.type == AttributeType.Simple)
                                            t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        else
                                            if (adChild.type == AttributeType.MultiValued)
                                                t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);

                                }
                            }//end if
                            mdp.AddTable(t2);
                            mdp.AddForeignKey(nameRelationship + "_1", t1, pk1, t2, pk1);
                            mdp.AddForeignKey(nameRelationship + "_2", t1, pk1, t2, pk2);

                        }
                        if (rd.type == RelationshipType.AssociativeEntity)
                        {

                        }
                    }
                }//End 1 ngôi
                #endregion

                #region Trường hợp 2 ngôi
                if (rd.Cardinalities.Count == 2)
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

                            Table t1 = mdp.SearchTable(nameTable1);//Bảng một
                            Table t2 = mdp.SearchTable(nameTable2);//Bảng nhiều

                            List<Column> pkT1 = t1.GetPrimaryKey();
                            t2.AddColumnFK(pkT1);

                            if (rd.Attributes.Count > 0)
                            {
                                //Thiếu trường hợp Multivalue
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    //Simple Attribute
                                    if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                        t2.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);

                                    //Composite Attribute
                                    if (ad.AttributeChilds.Count > 0)
                                    {
                                        foreach (AttributeData adChild in ad.AttributeChilds)
                                            t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                    }
                                }
                            }
                            mdp.AddForeignKey(nameRelationship, t1, pkT1, t2, pkT1);
                        }
                        //TH n,1
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "1")
                        {
                            Table t1 = mdp.SearchTable(nameTable2);//Bảng một
                            Table t2 = mdp.SearchTable(nameTable1);//Bảng nhiều

                            List<Column> pkT1 = t1.GetPrimaryKey();
                            t2.AddColumnFK(pkT1);

                            if (rd.Attributes.Count > 0)
                            {
                                //Thiếu trường hợp Multivalue
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    //Simple Attribute
                                    if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                        t2.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);

                                    //Composite Attribute
                                    if (ad.AttributeChilds.Count > 0)
                                    {
                                        foreach (AttributeData adChild in ad.AttributeChilds)
                                            t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                    }
                                }
                            }
                            mdp.AddForeignKey(nameRelationship, t1, pkT1, t2, pkT1);
                        }
                        //TH n:n
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1")
                        {
                            //Tìm Table tren MDP
                            //Create table trung gian (Relasionship)
                            //Tạo khóa chính gồm 2 khóa. 
                            //Thêm vào MDP   
                            //Tạo 2 fk vào MDP
                            Table t1 = mdp.SearchTable(nameTable1);
                            Table t2 = mdp.SearchTable(nameTable2);

                            List<Column> pk1 = t1.GetPrimaryKey();
                            List<Column> pk2 = t2.GetPrimaryKey();

                            Table temp = new Table((t1.name).ToUpper() + "_" + (t2.name).ToUpper(), rd.x, rd.y, rd.w, rd.h);

                            temp.AddPrimaryKeyForeignKey(pk1);
                            temp.AddPrimaryKeyForeignKey(pk2);

                            if (rd.Attributes.Count > 0)
                            {
                                //Thiếu trường hợp Multivalue
                                foreach (AttributeData ad in rd.Attributes)
                                {
                                    //Simple Attribute
                                    if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                        temp.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                    //Composite Attribute
                                    if (ad.AttributeChilds.Count > 0)
                                    {
                                        foreach (AttributeData adChild in ad.AttributeChilds)
                                            temp.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, ad.Description);
                                    }
                                }
                            }
                            mdp.AddTable(temp);
                            mdp.AddForeignKey(nameRelationship, t1, pk1, temp, pk1);
                            mdp.AddForeignKey(nameRelationship, t2, pk2, temp, pk2);
                        }
                        //TH 1,1
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "1")
                        {
                            //TH 0:1
                            if (cardinalityMin1 == "0" && cardinalityMin2 == "1")
                            {
                                Table t1 = mdp.SearchTable(nameTable2);//Bảng 1
                                Table t2 = mdp.SearchTable(nameTable1);//Bảng 0

                                List<Column> pk1 = t1.GetPrimaryKey();
                                if (rd.Attributes.Count > 0)
                                {
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            t2.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        }
                                    }
                                }
                                //Chuyển PK bảng 1 thành FK bảng 0
                                t2.AddColumnFK(pk1);
                                mdp.AddForeignKey(nameRelationship, t1, pk1, t2, pk1);
                            }
                            //TH 1:0
                            if (cardinalityMin2 == "0" && cardinalityMin1 == "1")
                            {
                                Table t1 = mdp.SearchTable(nameTable1);//Bảng 1
                                Table t2 = mdp.SearchTable(nameTable2);//Bảng 0

                                List<Column> pk1 = t1.GetPrimaryKey();
                                if (rd.Attributes.Count > 0)
                                {
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            t2.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        }
                                    }
                                }
                                //Chuyển PK bảng 1 thành FK bảng 0
                                t2.AddColumnFK(pk1);
                                mdp.AddForeignKey(nameRelationship, t1, pk1, t2, pk1);
                            }
                            //Trường hợp 1:1
                            if ((cardinalityMin1 == "1" && cardinalityMin2 == "1") || (cardinalityMin1 == "0" && cardinalityMin2 == "0"))
                            {
                                Table t1 = mdp.SearchTable(nameTable1);//Bảng 1
                                Table t2 = mdp.SearchTable(nameTable2);//Bảng 0

                                List<Column> pk1 = t1.GetPrimaryKey();
                                if (rd.Attributes.Count > 0)
                                {
                                    foreach (AttributeData ad in rd.Attributes)
                                    {
                                        //Simple Attribute
                                        if (ad.type == AttributeType.Simple && ad.AttributeChilds.Count == 0)
                                            t2.AddColumn(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                                        //Composite Attribute
                                        if (ad.AttributeChilds.Count > 0)
                                        {
                                            foreach (AttributeData adChild in ad.AttributeChilds)
                                                t2.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                                        }
                                    }
                                }
                                //Chuyển PK bảng 1 thành FK bảng 0
                                t2.AddColumnFK(pk1);
                                mdp.AddForeignKey(nameRelationship, t1, pk1, t2, pk1);
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

                        Table t1 = mdp.SearchTable(nameTable1);
                        Table t2 = mdp.SearchTable(nameTable2);

                        List<Column> pk1 = t1.GetPrimaryKey();
                        List<Column> pk2 = t2.GetPrimaryKey();

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
                        mdp.ConvertEntityStrongToTable(edTemp);

                        //Lấy Table Associative vừa tạo
                        Table temp = mdp.SearchTable(edTemp.name);


                        if (!hasPK)//Không có PK, thì 2 khóa chính từ 2 bảng quan hệ chuyển thành PK,FK
                        {
                            foreach (Column col in temp.columns)
                                if (col.PrimaryKey)
                                    col.IsPrimaryKeyForeignKey();
                        }
                        else//Có khóa, thì 2 List PK1, PK2 của 2 bảng quan hệ được add vào Temp (bảng Associative)
                        {
                            temp.AddColumnFK(pk1);
                            temp.AddColumnFK(pk2);
                        }


                        mdp.AddForeignKey("", t1, pk1, temp, pk1);
                        mdp.AddForeignKey("", t2, pk2, temp, pk2);

                    }//End Associative Entity
                    #endregion
                }//End TH 2 ngôi
                #endregion

                #region Trường hợp 3 ngôi
                if (rd.Cardinalities.Count == 3)
                {
                    //Relationship Normal:1-1-1, 1-1-n, 1-n-m, n-m-l   

                    Table t1 = mdp.SearchTable(nameTable1);
                    Table t2 = mdp.SearchTable(nameTable2);
                    Table t3 = mdp.SearchTable(nameTable3);

                    if (rd.type == RelationshipType.Normal)
                    {
                        //TH 1-1-1
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "1" && cardinalityMax3 == "1")
                            ProcessTernary(rd, nameRelationship, t1, t2, t3);

                        //TH 1-1-n
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "1" && cardinalityMax3 == "-1")
                            ProcessTernary(rd, nameRelationship, t3, t2, t1);

                        //TH 1-n-1
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "-1" && cardinalityMax3 == "1")
                            ProcessTernary(rd, nameRelationship, t2, t1, t3);

                        //TH n-1-1
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "1" && cardinalityMax3 == "1")
                            ProcessTernary(rd, nameRelationship, t1, t2, t3);

                        //TH 1-n-m
                        if (cardinalityMax1 == "1" && cardinalityMax2 == "-1" && cardinalityMax3 == "-1")
                            ProcessTernary(rd, nameRelationship, t2, t3, t1);

                        //TH n-1-m
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "1" && cardinalityMax3 == "-1")
                            ProcessTernary(rd, nameRelationship, t1, t3, t1);

                        //TH n-m-1
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1" && cardinalityMax3 == "1")
                            ProcessTernary(rd, nameRelationship, t1, t2, t3);

                        //TH n-m-l
                        if (cardinalityMax1 == "-1" && cardinalityMax2 == "-1" && cardinalityMax3 == "-1")
                            ProcessTernary(rd, nameRelationship, t1, t2, t3, true);

                    }//End IF

                    //Relationship Associate
                    if (rd.type == RelationshipType.AssociativeEntity)
                    {
                        //xảy ra hai trường hợp
                        //TH1: không có khóa chính - tương tự như trường hợp n-m-l
                        //TH2: có khóa chính - lấy khóa chính, và còn lại là khóa ngoại
                        ProcessTernary(rd, nameRelationship, t1, t2, t3, true);
                    }
                }
                #endregion

            }//End Relationships
            #endregion

        }//End Method Process

        /// <summary>
        /// Xử lý các trường hợp mối liên kết ba ngôi: 1-1-1, 1-1-n, 1-n-m, n-m-l, associative entity
        /// Lưu ý: Thứ tự t1,t2,t3 ảnh hưởng/tác dụng tới từng trường hợp cụ thể
        ///     1-1-1: t1,t2,t3   |   
        ///     1-1-n: t3,t1,t2   |   
        ///     1-n-1: t2,t1,t3   |   
        ///     n-1-1: t1,t2,t3   |   
        ///     1-n-m: t2,t3,t1   |   
        ///     n-1-m: t1,t3,t2   |   
        ///     n-m-1: t1,t2,t3   |   
        ///     n-m-l: t1,t2,t3,true   |   
        ///     associative entity: t1,t2,t3,true   |   
        /// </summary>
        /// <param name="t1">Table 1 trong Relationship Ternary</param>
        /// <param name="t2">Table 2 trong Relationship Ternary</param>
        /// <param name="t3">Table 3 trong Relationship Ternary</param>
        /// <param name="f"> default : FALSE, f = true trường hợp n-m-l, f = false các trường hợp còn lại</param>        
        private void ProcessTernary(RelationshipData rd, string nameRelationship, Table t1, Table t2, Table t3, bool f)
        {
            List<Column> pk1 = t1.GetPrimaryKey();
            List<Column> pk2 = t2.GetPrimaryKey();
            List<Column> pk3 = t3.GetPrimaryKey();

            bool hasPrimaryKey = false;

            Table t4 = new Table(nameRelationship, rd.x, rd.y, rd.w, rd.h);

            foreach (AttributeData ad in rd.Attributes)
            {
                //Composite Attribute
                if (ad.AttributeChilds.Count > 0)
                {
                    foreach (AttributeData adChild in ad.AttributeChilds)
                        t4.AddColumn(adChild.name, adChild.DataType, adChild.Length, adChild.AllowNull, adChild.Description);
                }
                else
                {
                    if (ad.type == AttributeType.Simple || ad.type == AttributeType.MultiValued)
                    {
                        Column c = new Column(ad.name, ad.DataType, ad.Length, ad.AllowNull, ad.Description);
                        t4.AddColumn(c);
                    }
                    if (ad.type == AttributeType.Key)
                    {
                        hasPrimaryKey = true;
                        Column c = new Column(ad.name, ad.DataType, ad.Length, ad.AllowNull, true, false, ad.Description);
                        t4.AddPrimaryKey(c);
                    }
                }
            }

            if (hasPrimaryKey)
            {
                t4.AddColumnFK(pk1);
                t4.AddColumnFK(pk2);
                t4.AddColumnFK(pk3);
            }
            else
            {
                t4.AddPrimaryKeyForeignKey(pk1);
                t4.AddPrimaryKeyForeignKey(pk2);
                if (f)
                    t4.AddPrimaryKeyForeignKey(pk3);
                else
                    t4.AddColumnFK(pk3);
            }

            mdp.AddTable(t4);
            mdp.AddForeignKey(nameRelationship + "1", t1, pk1, t4, pk1);
            mdp.AddForeignKey(nameRelationship + "2", t2, pk2, t4, pk2);
            mdp.AddForeignKey(nameRelationship + "3", t3, pk3, t4, pk3);
        }

        private void ProcessTernary(RelationshipData rd, string nameRalationship, Table t1, Table t2, Table t3)
        {
            ProcessTernary(rd, nameRalationship, t1, t2, t3, false);
        }

        private EntityData SearchEntityData(string nameWeakTable)
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

        private void ChangeLocationMultiValuedAttribute(List<AttributeData> listAd, string tableName)
        {
            foreach (AttributeData adChild in listAd)
            {
                if (adChild.type == AttributeType.MultiValued)
                {
                    //Tìm Entity bên nhiều
                    EntityData ed = SearchEntityFromERD(tableName);
                    //Gắn Multivalued vào bên tableName
                    ed.Attributes.Add(adChild);
                }
            }

        }
        private string SearchEntityParent(string nameEntityWeak)
        {
            //Tìm tên Parent dựa vào RelationShip Indentifier
            //nameEntity danh sách tên Entity trong quan hệ Indentifer
            //indexNameEntityParent là vị trí của Parent
            List<string> nameEntity = new List<string>();
            int indexNameEntityParent = -1;
            string temp = "";
            int index = -1;
            for (int i = 0; i < erd.Relationships.Count; i++)
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
                    if (index >= 0)
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

        private bool SearchInList(List<string> list, string name)
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

        private EntityData SearchEntityFromERD(string nEntity)
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

        public void WriteXML()
        {
            js.savePhysicalToXML(@"C:\aaa.xml", mdp);
        }

    }
}
