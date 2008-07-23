using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ERDesigner;

namespace ERDesigner.Shape
{
    public class SubTypeConnector : ShapeBase, INotation
    {
        public string completeness;
        public string disjointness;
        public EntityShape subpertype;
        public List<EntityShape> subtypes;
        public List<string> discriminators;

        public SubTypeConnector(EntityShape super, EntityShape sub, Point loc, string comp, string disj)
        {
            subtypes = new List<EntityShape>();
            discriminators = new List<string>();

            subpertype = super;
            addSubType(sub);

            completeness = comp;
            disjointness = disj;

            this.Size = new Size(ThongSo.ShapeH / 2, ThongSo.ShapeH / 2);
            this.CenterPoint = loc;

            this.Disposed += new EventHandler(SubTypeConnector_Disposed);
            subpertype.Disposed += new EventHandler(subpertype_Disposed);
            refreshPath();
        }
        void subpertype_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
        }
        void SubTypeConnector_Disposed(object sender, EventArgs e)
        {

        }
        public override ShapeBase Clone()
        {
            return null;
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
        public void addSubType(EntityShape entity)
        {
            subtypes.Add(entity);
            entity.Disposed += new EventHandler(subtype_Disposed);
        }
        void subtype_Disposed(object sender, EventArgs e)
        {
            subtypes.Remove((EntityShape)sender);
            if (subtypes.Count == 0)
                this.Dispose();
        }

        protected override void refreshPath()
        {
            path = new GraphicsPath();
            Rectangle rect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);

            path.ClearMarkers();
            path.AddEllipse(rect);

            this.Region = new Region(path);
        }
        public override void DrawSelf(Graphics g)
        {
            refreshPath();

            Rectangle rect = new Rectangle(this.ClientRectangle.X + 1, this.ClientRectangle.Y + 1, this.ClientRectangle.Width - 2, this.ClientRectangle.Height - 2);

            StringFormat st = new StringFormat();
            st.Alignment = StringAlignment.Center;
            st.LineAlignment = StringAlignment.Center;

            string strconstraint = "";
            if (disjointness == SubTypeConnectorType.DisjointConstraint)
                strconstraint = "d";
            else
                strconstraint = "O";

            g.DrawString(strconstraint, ThongSo.JFont, ThongSo.JBrush, rect, st);
            g.DrawEllipse(ThongSo.JPen, rect);
        }

        #region INotation Members

        public void DrawConnectiveLines(Graphics g)
        {
            if(completeness == SubTypeConnectorType.TotalSpecialization)
            {
                
            }
            else
            {
                g.DrawLine(ThongSo.JPen, subpertype.CenterPoint, this.CenterPoint);
            }

            foreach (EntityShape sub in subtypes)
            {
                g.DrawLine(ThongSo.JPen, this.CenterPoint, sub.CenterPoint);
            }
        }
        public IMetaData getMetaData()
        {
            return null;
        }
        #endregion
    }
}
