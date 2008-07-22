using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ERDesigner.Shape
{
    public class EntityShape : ShapeBase, INotation
    {
        public string type;
        static int number = 0;
        public List<AttributeShape> attributes;
        public List<CardinalityShape>[] cardinalities;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EntityShape
            // 
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.DoubleBuffered = true;
            this.Name = "EntityShape";
            this.Size = new System.Drawing.Size(130, 60);
            this.ResumeLayout(false);

        }
        public EntityShape()
        {
            type = EntityType.Strong;
            attributes = new List<AttributeShape>();
            cardinalities = new List<CardinalityShape>[4];
            cardinalities[0] = new List<CardinalityShape>();
            cardinalities[1] = new List<CardinalityShape>();
            cardinalities[2] = new List<CardinalityShape>();
            cardinalities[3] = new List<CardinalityShape>();

            Size = new Size(ThongSo.ShapeW, ThongSo.ShapeH);
            sName = "ENTITY_" + number++;

            this.Disposed += new EventHandler(EntityShape_Disposed);
            refreshPath();
        }
        public EntityShape(string name, Point loc, string t)
        {
            type = t;
            attributes = new List<AttributeShape>();
            cardinalities = new List<CardinalityShape>[4];
            cardinalities[0] = new List<CardinalityShape>();
            cardinalities[1] = new List<CardinalityShape>();
            cardinalities[2] = new List<CardinalityShape>();
            cardinalities[3] = new List<CardinalityShape>();

            Size = new Size(ThongSo.ShapeW, ThongSo.ShapeH);
            sName = name;
            this.CenterPoint = loc;

            this.Disposed += new EventHandler(EntityShape_Disposed);
            refreshPath();
        }
        void EntityShape_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].Dispose();
                i--;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                //auto change attribute display key if this is weak entity
                foreach (AttributeShape att in this.attributes)
                {
                    if (att.type == AttributeType.Key)
                        att.Invalidate();
                }
            }
        }
        public override ShapeBase Clone()
        {
            EntityShape entity = new EntityShape();
            entity.sName = sName;
            entity.type = type;

            entity.Size = this.Size;
            entity.Location = Location;

            foreach (AttributeShape att in attributes)
            {
                entity.addAttribute((AttributeShape)att.Clone());
            }

            for (int i = 0; i < 4; i++)
                foreach (CardinalityShape cardi in cardinalities[i])
                {
                    entity.AddCardiPlace((CardinalityShape)cardi.Clone());
                }

            return (ShapeBase)entity;
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
            int EdgeCardiPlace = DrawingSupport.CalculateCardiDirection(this, cardi.Relationship);
            int index = CalculateCardiPosition(cardi, EdgeCardiPlace);
            cardinalities[EdgeCardiPlace - 1].Insert(index, cardi);
            cardi.Disposed += new EventHandler(cardi_Disposed);
        }
        void cardi_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                cardinalities[i].Remove((CardinalityShape)sender);
            }
        }
        public void addAttribute(AttributeShape att)
        {
            attributes.Add(att);
            att.Entity = this;
            att.Disposed += new EventHandler(att_Disposed);
        }
        void att_Disposed(object sender, EventArgs e)
        {
            attributes.Remove((AttributeShape)sender);
        }

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);

            path.AddRectangle(rect);

            this.Region = new Region(path);
        }
        public override void DrawSelf(Graphics g)
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            Rectangle rect1 = new Rectangle(this.ClientRectangle.X + 5, this.ClientRectangle.Y + 5, this.ClientRectangle.Width - 11, this.ClientRectangle.Height - 11);

            if (type == EntityType.Strong)
                g.DrawRectangle(ThongSo.JPen, rect);
            else
            {
                g.DrawRectangle(ThongSo.JPen, rect);
                g.DrawRectangle(ThongSo.JPen, rect1);
            }
        }
        public override void dinhviTextBox(TextBox txtName)
        {
            txtName.Location = new Point(0, this.ClientRectangle.Height / 2 - txtName.Size.Height / 2);
            txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtName.Size = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);
            txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
        }

        public void UpdateCardinalityPosition()
        {
            for (int i = 0; i < 4; i++)
            {
                //update all cardinality of this entity
                for (int j = 0; j < this.cardinalities[i].Count; j++)
                {
                    CardinalityShape cardi = this.cardinalities[i][j];

                    RelationshipShape rel = cardi.Relationship;

                    int oldEdgeCardiPlace = i + 1;
                    int newEdgeCardiPlace = DrawingSupport.CalculateCardiDirection(this, rel);

                    //Chổ này làm kỹ, không remove ra rồi add đại vô được
                    //không thôi nó giựt wài ghét lắm

                    //Nếu vị trí mới tính được khác vị trí cũ
                    if (oldEdgeCardiPlace != newEdgeCardiPlace)
                    {
                        //remove old and add new
                        this.cardinalities[oldEdgeCardiPlace - 1].Remove(cardi);

                        int index = CalculateCardiPosition(cardi, newEdgeCardiPlace);
                        this.cardinalities[newEdgeCardiPlace - 1].Insert(index, cardi);
                    }
                    else //Nếu vẫn là vị trí cũ, thì tính lại thứ tự trong vị trí đó
                    {
                        //just update the position
                        for (int k = 0; k < this.cardinalities[oldEdgeCardiPlace - 1].Count - 1; k++)
                        {
                            if (oldEdgeCardiPlace == 1 || oldEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                            {
                                if (this.cardinalities[oldEdgeCardiPlace - 1][k].Relationship.Location.X > this.cardinalities[oldEdgeCardiPlace - 1][k + 1].Relationship.Location.X)
                                    this.cardinalities[oldEdgeCardiPlace - 1].Reverse(k, 2);
                            }
                            else //right hoặc left thì so sánh y
                            {
                                if (this.cardinalities[oldEdgeCardiPlace - 1][k].Relationship.Location.Y > this.cardinalities[oldEdgeCardiPlace - 1][k + 1].Relationship.Location.Y)
                                    this.cardinalities[oldEdgeCardiPlace - 1].Reverse(k, 2);
                            }
                        }
                    }
                }
            }
        }
        private int CalculateCardiPosition(CardinalityShape cardi, int newEdgeCardiPlace)
        {
            int index = 0;
            foreach (CardinalityShape cardiInEntity in this.cardinalities[newEdgeCardiPlace - 1])
            {
                if (newEdgeCardiPlace == 1 || newEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                {
                    if (cardi.Relationship.Location.X > cardiInEntity.Relationship.Location.X)
                        index++;
                }
                else //right hoặc left thì so sánh y
                {
                    if (cardi.Relationship.Location.Y > cardiInEntity.Relationship.Location.Y)
                        index++;
                }
            }
            return index;
        }

        #region INotation Members

        public void DrawConnectiveLines(Graphics g)
        {
            foreach (AttributeShape att in this.attributes)
            {
                g.DrawLine(new Pen(Color.Black, 1), this.CenterPoint, att.CenterPoint);
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < this.cardinalities[i].Count; j++)
                {
                    DrawingSupport.DrawCardinalitiesLine(g, this.cardinalities[i][j], i + 1, j + 1, this.cardinalities[i].Count);
                }
            }
        }

        #endregion
    }
}
