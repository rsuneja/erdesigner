using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ERDesigner;

namespace ERDesigner.Shape
{
    public class CardinalityShape : ShapeBase
    {
        EntityShape _entity;
        public int MinCardinality;
        public int MaxCardinality;
        RelationshipShape _relationship;

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
        public CardinalityShape()
        {
        }
        public CardinalityShape(EntityShape en)
        {
            Entity = en;
        }
        public EntityShape Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                _entity.Disposed += new EventHandler(This_Dispose);
            }
        }
        public RelationshipShape Relationship
        {
            get { return _relationship; }
            set
            {
                _relationship = value;
                _relationship.Disposed += new EventHandler(This_Dispose);
            }
        }
        void This_Dispose(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public override ShapeBase Clone()
        {
            CardinalityShape cardi = new CardinalityShape();
            cardi.Entity = Entity;
            cardi.MinCardinality = MinCardinality;
            cardi.MaxCardinality = MaxCardinality;
            cardi._relationship = _relationship;

            return (ShapeBase)cardi;
        }

        public void setValue(int min, int max)
        {
            MinCardinality = min;
            MaxCardinality = max;
        }
    }
    public class RelationshipShape : ShapeBase, INotation
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
        public RelationshipShape(string name, Point loc, string t)
        {
            type = t;
            attributes = new List<AttributeShape>();
            cardinalities = new List<CardinalityShape>();

            cardiplaces = new List<CardinalityShape>[4];
            cardiplaces[0] = new List<CardinalityShape>();
            cardiplaces[1] = new List<CardinalityShape>();
            cardiplaces[2] = new List<CardinalityShape>();
            cardiplaces[3] = new List<CardinalityShape>();

            sName = name;

            this.Size = new Size(ThongSo.ShapeW, ThongSo.ShapeH);
            this.CenterPoint = loc;

            this.Disposed += new EventHandler(RelationshipShape_Disposed);
            refreshPath();
        }
        void RelationshipShape_Disposed(object sender, EventArgs e)
        {
            DisposeCardinalities();
            DisposeAttributes();
        }
        public void DisposeAttributes()
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].Dispose();
                i--;
            }
        }
        public void DisposeCardinalities()
        {
            //Xóa cardi trên những entity có quan hệ với relationship này
            foreach (CardinalityShape cardi in cardinalities)
                for (int j = 0; j < 4; j++)
                {
                    cardi.Entity.cardinalities[j].Remove(cardi);
                }
            cardinalities.Clear();

            for (int i = 0; i < 4; i++)
                cardiplaces[i].Clear();

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
                CardinalityShape newcardi = (CardinalityShape)cardi.Clone();
                rel.CreateCardinality(newcardi.Entity, newcardi.MinCardinality, newcardi.MaxCardinality);
            }

            return (ShapeBase)rel;
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
        public void AddCardiPlace(CardinalityShape cardi)
        {
            int EdgeCardiPlace = DrawingSupport.CalculateCardiDirection(this, cardi.Entity);
            int index = CalculateCardiPosition(cardi, EdgeCardiPlace);
            cardiplaces[EdgeCardiPlace - 1].Insert(index, cardi);
            cardi.Disposed += new EventHandler(cardiplace_Disposed);
        }
        void cardiplace_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                cardiplaces[i].Remove((CardinalityShape)sender);
            }
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
        public CardinalityShape CreateCardinality(EntityShape en, int min, int max)
        {
            CardinalityShape cardi = new CardinalityShape(en);
            cardi.setValue(min, max);
            cardinalities.Add(cardi);
            cardi.Relationship = this;

            en.AddCardiPlace(cardi);
            this.AddCardiPlace(cardi);

            cardi.Disposed += new EventHandler(cardi_Disposed);

            return cardi;
        }
        void cardi_Disposed(object sender, EventArgs e)
        {
            cardinalities.Remove((CardinalityShape)sender);
            if (cardinalities.Count <= 1)
            {
                this.Dispose();
            }
        }

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            Point p1 = new Point(rect.Width / 2, 0);
            Point p2 = new Point(rect.Width, rect.Height / 2);
            Point p3 = new Point(rect.Width / 2, rect.Height);
            Point p4 = new Point(0, rect.Height / 2);

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
            Point p2 = new Point(rect.Width, rect.Height / 2 + 1);
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
            if (type == RelationshipType.AssociativeEntity)
                txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
        }

        public void UpdateCardinalityPosition()
        {
            if (this.type == RelationshipType.AssociativeEntity)
            {
                for (int i = 0; i < 4; i++)
                {
                    //update all cardinality of this relationship
                    for (int j = 0; j < this.cardiplaces[i].Count; j++)
                    {
                        CardinalityShape cardi = this.cardiplaces[i][j];

                        EntityShape entity = cardi.Entity;

                        int oldEdgeCardiPlace = i + 1;
                        int newEdgeCardiPlace = DrawingSupport.CalculateCardiDirection(this, entity);

                        //Chổ này làm kỹ, không thôi nó giựt wài ghét lắm

                        //Nếu vị trí mới tính được khác vị trí cũ
                        if (oldEdgeCardiPlace != newEdgeCardiPlace)
                        {
                            //remove old and add new
                            this.cardiplaces[oldEdgeCardiPlace - 1].Remove(cardi);

                            int index = CalculateCardiPosition(cardi, newEdgeCardiPlace);
                            this.cardiplaces[newEdgeCardiPlace - 1].Insert(index, cardi);
                        }
                        else //Nếu vẫn là vị trí cũ, thì tính lại thứ tự trong vị trí đó
                        {
                            //just update the position
                            for (int k = 0; k < this.cardiplaces[oldEdgeCardiPlace - 1].Count - 1; k++)
                            {
                                if (oldEdgeCardiPlace == 1 || oldEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                                {
                                    if (this.cardiplaces[oldEdgeCardiPlace - 1][k].Relationship.Location.X > this.cardiplaces[oldEdgeCardiPlace - 1][k + 1].Relationship.Location.X)
                                        this.cardiplaces[oldEdgeCardiPlace - 1].Reverse(k, 2);
                                }
                                else //right hoặc left thì so sánh y
                                {
                                    if (this.cardiplaces[oldEdgeCardiPlace - 1][k].Relationship.Location.Y > this.cardiplaces[oldEdgeCardiPlace - 1][k + 1].Relationship.Location.Y)
                                        this.cardiplaces[oldEdgeCardiPlace - 1].Reverse(k, 2);
                                }
                            }
                        }
                    }
                }
            }
        }
        private int CalculateCardiPosition(CardinalityShape cardi, int newEdgeCardiPlace)
        {
            int index = 0;
            foreach (CardinalityShape cardiInRel in this.cardiplaces[newEdgeCardiPlace - 1])
            {
                if (newEdgeCardiPlace == 1 || newEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                {
                    if (cardi.Entity.Location.X > cardiInRel.Entity.Location.X)
                        index++;
                }
                else //right hoặc left thì so sánh y
                {
                    if (cardi.Entity.Location.Y > cardiInRel.Entity.Location.Y)
                        index++;
                }
            }
            return index;
        }
        public void getCardiPosition(CardinalityShape cardi, ref int direction, ref int index)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < cardiplaces[i].Count; j++)
                {
                    if (cardi == cardiplaces[i][j])
                    {
                        direction = i + 1;
                        index = j;
                    }
                }
        }
        #region INotation Members

        public void DrawConnectiveLines(Graphics g)
        {
            foreach (AttributeShape att in this.attributes)
            {
                g.DrawLine(new Pen(Color.Black, 1), this.CenterPoint, att.CenterPoint);
            }
        }
        public IMetaData getMetaData()
        {
            RelationshipData relationship = new RelationshipData(this.sName, this.type, this.Location.X, this.Location.Y, this.Width, this.Height);
            return (IMetaData)relationship;
        }
        #endregion
    }
}
