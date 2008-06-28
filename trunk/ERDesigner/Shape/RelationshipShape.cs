using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ERDesigner;

namespace ERDesigner.Shape
{
    public class CardinalityShape:ShapeBase
    {
        public EntityShape entity;
        public int MinCardinality;
        public int MaxCardinality;
        public RelationshipShape relationship;

        public EntityShape Entity
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public override ShapeBase Clone()
        {
            CardinalityShape cardi = new CardinalityShape();
            cardi.entity = entity;
            cardi.MinCardinality = MinCardinality;
            cardi.MaxCardinality = MaxCardinality;
            cardi.relationship = relationship;

            return (ShapeBase)cardi;
        }
        //Lại áp dụng cách này
        public void addEntity(EntityShape en)
        {
            entity = en;
            entity.Disposed += new EventHandler(entity_Disposed);

            //Add đại cardi vô List[cạnh] của entity
            entity.insertCardinality(0, 0, this);
        }

        void entity_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CardinalityShape
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "CardinalityShape";
            this.Size = new System.Drawing.Size(130, 60);
            this.ResumeLayout(false);
        }

    }
    public class RelationshipShape:ShapeBase
    {
        public List<AttributeShape> attributes;
        public List<CardinalityShape> cardinalities;
        public string type;
        static int number = 0;

        public List<CardinalityShape>[] cardiplaces;

        public RelationshipShape()
        {
            type = RelationshipType.Normal;
            attributes = new List<AttributeShape>();
            cardinalities = new List<CardinalityShape>();

            cardiplaces = new List<CardinalityShape>[4];
            cardiplaces[0] = new List<CardinalityShape>();
            cardiplaces[1] = new List<CardinalityShape>();
            cardiplaces[2] = new List<CardinalityShape>();
            cardiplaces[3] = new List<CardinalityShape>();

            sName = "Relationship_" + number++;

            this.Size = new Size(ThongSo.ShapeW, ThongSo.ShapeH);
            this.Disposed += new EventHandler(RelationshipShape_Disposed);
            refreshPath();
        }

        public CardinalityShape Cardinalities
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public AttributeShape Attributes
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        void RelationshipShape_Disposed(object sender, EventArgs e)
        {
            clearCardinalities();
            clearAttributes();
        }
        public override ShapeBase Clone()
        {
            RelationshipShape rel = new RelationshipShape();
            rel.sName = sName;
            rel.type = type;

            rel.Size = this.Size;
            rel.Location = Location;

            foreach (AttributeShape att in attributes)
            {
                rel.addAttribute((AttributeShape)att.Clone());
            }

            foreach (CardinalityShape cardi in cardinalities)
            {
                rel.addCardinality((CardinalityShape)cardi.Clone());
            }

            if (type == RelationshipType.AssociativeEntity)
            {
                for (int i = 0; i < 4; i++)
                    foreach (CardinalityShape cardi in cardiplaces[i])
                    {
                        rel.insertCardiPlace(i, 0, (CardinalityShape)cardi.Clone());
                    }
            }

            return (ShapeBase)rel;
        }
        public void insertCardiPlace(int edgePlace, int i, CardinalityShape cardi)
        {
            cardiplaces[edgePlace].Insert(i, cardi);
            cardi.Disposed += new EventHandler(cardiplace_Disposed);
        }

        void cardiplace_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                cardiplaces[i].Remove((CardinalityShape)sender);
            }
        }
        public void clearAttributes()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].Dispose();
                i--;
            }
        }

        //Đoạn này nhìn vậy thực ra kiếm cả ngày mới ra
        //Ý tưởng: trên Draw Board, khi user delete 1 Attribute bất kỳ
        //Phải kiểm tra xem Attribute đó thuộc Entity/Relationship nào để remove nó trong
        //list Attributes của Entity/Relationship đó đi

        //Vấn đề 1: từ Entity/Relationship biết Attribute, nhưng từ Attribute không biết được Entity
        // -> Tạo biến parent giữ Entity/Relationship --> code dài, phụ thuộc nhiều quá, chưa tính Relationship
        //Vấn đề 2: wá nhiều Entity, wá nhiều Attribute để kiểm tra
        // -> Mong muốn khi xóa Attribute thì những reference tới Attribute đó cũng xóa theo
        // -> Trong C dùng con trỏ là Ok rồi

        //--> Final Solution: Event + Callback Function, wá ngắn
        //www.msdner.com
        //Key: Can I dispose of instances without explicitly removing all references to them?

        public void clearCardinalities()
        {
            //Xóa cardi trên những entity liên quan
            foreach(CardinalityShape cardi in cardinalities)
                for (int j = 0; j < 4; j++)
                {
                    cardi.entity.cardinalities[j].Remove(cardi);
                }
            cardinalities.Clear();

            for (int i = 0; i < 4; i++)
                cardiplaces[i].Clear();
            
        }

        public void addAttribute(AttributeShape att)
        {
            attributes.Add(att);
            att.Disposed += new EventHandler(att_Disposed);
            if (attributes.Count >= 1)
            {
                if (this.cardinalities.Count > 1)
                {
                    if (this.cardinalities[0].MaxCardinality == -1 && this.cardinalities[1].MaxCardinality == -1)
                    {
                        this.type = RelationshipType.AssociativeEntity;
                        this.sName = this.sName.ToUpper();
                        Invalidate();
                    }
                }
            }
        }

        void att_Disposed(object sender, EventArgs e)
        {
            attributes.Remove((AttributeShape)sender);
            if (attributes.Count == 0)
            {
                this.type = RelationshipType.Normal;
                this.sName = this.sName.ToLower();
                this.sName = char.ToUpper(this.sName[0]) + this.sName.Substring(1);
                Invalidate();
            }
        }
        //end

        //cardinality
        public void addCardinality(CardinalityShape cardi)
        {
            cardinalities.Add(cardi);
            cardi.relationship = this;

            //Lưu vết để vẽ nhiều Cardinaltiy nối với AssociativeEntity sau này
            //Tính cardi của rel ở cạnh nào của rel
            EntityShape entity = cardi.entity;
            RelationshipShape relationship = this;

            relationship.insertCardiPlace(0, 0, cardi);
            cardi.Disposed += new EventHandler(cardi_Disposed);
        }

        void cardi_Disposed(object sender, EventArgs e)
        {
            cardinalities.Remove((CardinalityShape)sender);
            if (cardinalities.Count <= 1)
            {
                this.Dispose();
            }
        }
        //end

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X,this.ClientRectangle.Y,this.ClientRectangle.Width, this.ClientRectangle.Height);
            Point p1 = new Point(rect.Width/2, 0);
            Point p2 = new Point(rect.Width, rect.Height/2);
            Point p3 = new Point(rect.Width/2, rect.Height);
            Point p4 = new Point(0, rect.Height/2);

            if (type == RelationshipType.AssociativeEntity)
            {
                path.ClearMarkers();
                path.AddRectangle(rect);
            }
            else
            {
                path.ClearMarkers();
                path.AddPolygon(new Point[] { p1, p2, p3, p4 });
            }
                                   
            this.Region = new Region(path);
        }

        public override void DrawSelf(Graphics g)
        {
            refreshPath();

            Rectangle rect = new Rectangle(this.ClientRectangle.X + 1, this.ClientRectangle.Y + 1, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);
            
            Point p1 = new Point(rect.Width / 2, rect.Y);
            Point p2 = new Point(rect.Width , rect.Height / 2 + 1);
            Point p3 = new Point(rect.Width / 2, rect.Height);
            Point p4 = new Point(rect.X, rect.Height / 2 + 1);

            Point p5 = new Point(p1.X, p1.Y + 5);
            Point p6 = new Point(p2.X - 8, p2.Y);
            Point p7 = new Point(p3.X, p3.Y - 5);
            Point p8 = new Point(p4.X + 8, p4.Y);
            
            if (type == RelationshipType.Normal)
                g.DrawPolygon(ThongSo.JPen, new Point[] { p1, p2, p3, p4 });
            else if (type == RelationshipType.Identifier)
            {
                g.DrawPolygon(ThongSo.JPen, new Point[] { p1, p2, p3, p4 });

                g.DrawPolygon(ThongSo.JPen, new Point[] { p5, p6, p7, p8 });
            }
            else if (type == RelationshipType.AssociativeEntity)
            {
                g.DrawRectangle(ThongSo.JPen, rect);
                g.DrawPolygon(ThongSo.JPen, new Point[] { p1, p2, p3, p4 });
            }
        }

        public override void dinhviTextBox(TextBox txtName)
        {
            txtName.Location = new Point(0, this.ClientRectangle.Height / 2 - txtName.Size.Height / 2);
            txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtName.Size = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);
            if(type == RelationshipType.AssociativeEntity)
                txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
        }
    }
}
