using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace ERDesigner.Shape
{
    public class AttributeShape : ShapeBase, INotation
    {
        public String type;
        static int number = 0;
        StringFormat st; //chữ gạch dưới
        public bool isComposite;
        public bool allowNull = true;
        public string dataType = "varchar";
        public int dataLength = 50;
        public string description = "";
        public EntityShape Entity;
        public List<AttributeShape> attributeChilds;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AttributeShape
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "AttributeShape";
            this.Size = new System.Drawing.Size(130, 60);
            this.ResumeLayout(false);

        }
        public AttributeShape()
        {
            sName = "Attribute_" + number++;
            type = AttributeType.Simple;

            isComposite = true;
            attributeChilds = new List<AttributeShape>();

            st = new StringFormat();
            st.Alignment = StringAlignment.Center;
            st.LineAlignment = StringAlignment.Center;

            this.Size = new Size(ThongSo.ShapeW, ThongSo.ShapeH);
            this.Disposed += new EventHandler(AttributeShape_Disposed);
            refreshPath();
        }
        void AttributeShape_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < attributeChilds.Count; i++)
            {
                attributeChilds[i].Dispose();
                i--;
            }
        }
        public override ShapeBase Clone()
        {
            AttributeShape att = new AttributeShape();
            att.sName = sName;
            att.type = type;
            att.isComposite = isComposite;
            att.Size = this.Size;
            att.Location = Location;

            if (isComposite)
            {
                foreach (AttributeShape attchild in attributeChilds)
                {
                    att.addAttribute((AttributeShape)attchild.Clone());
                }
            }

            return (ShapeBase)att;
        }

        public void addAttribute(AttributeShape att)
        {
            attributeChilds.Add(att);
            att.Disposed += new EventHandler(att_Disposed);
        }
        void att_Disposed(object sender, EventArgs e)
        {
            attributeChilds.Remove((AttributeShape)sender);
        }

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
            path.AddEllipse(rect);

            this.Region = new Region(path);
        }
        public override void DrawSelf(Graphics g)
        {
            Rectangle rect = new Rectangle(this.ClientRectangle.X + 1, this.ClientRectangle.Y + 1, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);
            Rectangle rect1 = new Rectangle(this.ClientRectangle.X + 6, this.ClientRectangle.Y + 6, this.ClientRectangle.Width - 12, this.ClientRectangle.Height - 12);

            if (type == AttributeType.Simple)
                g.DrawEllipse(ThongSo.JPen, rect);
            else if (type == AttributeType.MultiValued)
            {
                g.DrawEllipse(ThongSo.JPen, rect);
                g.DrawEllipse(ThongSo.JPen, rect1);
            }
            else if (type == AttributeType.Derived)
            {
                g.DrawEllipse(ThongSo.getDashPen(), rect);
            }
            else if (type == AttributeType.Key)
            {
                g.DrawEllipse(ThongSo.JPen, rect);

                Font uFont = new Font(ThongSo.JFont.Name, ThongSo.JFont.Size, FontStyle.Underline);

                g.DrawString(sName, uFont, ThongSo.JBrush, new RectangleF(rect.Location.X + 10, rect.Y, rect.Width - 20, rect.Height), st);

                if (Entity != null && Entity.type == EntityType.Weak)
                {
                    SizeF nameSize = g.MeasureString(sName, uFont);
                    Point CenterShape = new Point(rect.Location.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    g.DrawLine(ThongSo.JPen, CenterShape.X - nameSize.Width / 2 + 3, CenterShape.Y + nameSize.Height / 2 + 2, CenterShape.X + nameSize.Width / 2 - 3, CenterShape.Y + nameSize.Height / 2 + 2);
                }
            }

        }
        public override void dinhviTextBox(TextBox txtName)
        {
            txtName.Location = new Point(0, this.ClientRectangle.Height / 2 - txtName.Size.Height / 2);
            txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtName.Size = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        #region INotation Members

        public void DrawConnectiveLines(Graphics g)
        {
            foreach (AttributeShape att in this.attributeChilds)
            {
                g.DrawLine(new Pen(Color.Black, 1), this.CenterPoint, att.CenterPoint);
            }
        }
        public IMetaData getMetaData()
        {
            AttributeData attribute = new AttributeData(this.sName, this.type, this.Location.X, this.Location.Y, this.Width, this.Height, this.dataType, this.dataLength, this.allowNull, this.description);
            return (IMetaData)attribute;
        }

        #endregion

    }
}
