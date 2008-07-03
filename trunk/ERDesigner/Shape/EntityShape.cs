using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ERDesigner.Shape
{
    public class EntityShape:ShapeBase
    {
        public string type;
        static int number = 0;
        public List<AttributeShape> attributes;
        public List<CardinalityShape>[] cardinalities;

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
            this.Location = loc;

            this.Disposed += new EventHandler(EntityShape_Disposed);
            refreshPath();
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

            for(int i=0; i<4; i++)
                foreach (CardinalityShape cardi in cardinalities[i])
                {
                    entity.insertCardinality(i, 0, (CardinalityShape)cardi.Clone());
                }

           return (ShapeBase)entity;
        }

        void EntityShape_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].Dispose();
                i--;
            }
        }

        public void insertCardinality(int edgePlace, int i, CardinalityShape cardi)
        {
            cardinalities[edgePlace].Insert(i, cardi);
            cardi.Disposed += new EventHandler(cardi_Disposed);
        }

        void cardi_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                cardinalities[i].Remove((CardinalityShape)sender);
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
        //end

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X,this.ClientRectangle.Y,this.ClientRectangle.Width, this.ClientRectangle.Height);

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
    }
}
