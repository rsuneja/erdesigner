using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ERDesigner;
using System.Text.RegularExpressions;

namespace ERDesigner.Shape
{
    public class ShapeBase :UserControl
    {
        //hình vẽ: là 1 đường khép kín
        protected GraphicsPath path = null;
        public String sName = "";
        public static bool skin = true;
        public Cursor CurrentCursor = Cursors.Default;
        PanelDoubleBuffered p;
        TextBox txtName;
        
        //override: lớp con sẽ tự vẽ
        protected virtual void refreshPath(){}
        
        protected override void OnResize(EventArgs e)
        {
 	        base.OnResize(e);
            refreshPath(); //cho class con định nghĩa lại path
            this.Invalidate();
        }

        public Point CenterPoint
        {
            get
            {
                return new Point(this.Location.X + this.Width / 2, this.Location.Y + this.Height / 2);
            }
            set
            {
                this.Location = new Point(value.X - this.Width / 2, value.Y - this.Height / 2);
            }
        }

        public ShapeBase()
        {
            this.BackColor = Color.WhiteSmoke;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            if (path != null)
            {
                //e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                //Giao diện sẽ xử lý sau
                //

                if (this.ClientRectangle.Width != 0 && this.ClientRectangle.Height != 0 && skin)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color.White, Color.CornflowerBlue, LinearGradientMode.Vertical);

                    //PathGradientBrush brush = new PathGradientBrush(path);
                    //brush.WrapMode = WrapMode.Tile;
                    //brush.SurroundColors = new Color[] { Color.White };
                    //brush.CenterColor = Color.Violet;

                    e.Graphics.FillPath(brush, path);
                }
                else
                    e.Graphics.FillPath(new SolidBrush(Color.White), path);

                e.Graphics.DrawPath(new Pen(Color.White,1), path); //vẽ dùm class con

                // vẽ tên ở giữa hình
                StringFormat st = new StringFormat();
                st.Alignment = StringAlignment.Center;
                st.LineAlignment = StringAlignment.Center;

                //SizeF nameSize = e.Graphics.MeasureString(sName, ThongSo.JFont);
                //if (nameSize.Width + 50 > this.Size.Width) this.Size = new Size((int)nameSize.Width + 50, this.Size.Height);

                RectangleF rect = this.ClientRectangle;

                if (!(this.GetType().Name == "AttributeShape" && ((AttributeShape)this).type == AttributeType.Key))
                    e.Graphics.DrawString(sName, ThongSo.JFont, ThongSo.JBrush, new RectangleF(rect.Location.X + 10, rect.Y, rect.Width - 20, rect.Height), st);

                DrawSelf(e.Graphics);
            }
        }
        public virtual void DrawSelf(Graphics g) { }

        public virtual void dinhviTextBox(TextBox txtName) { } //Để cho class con định vị

        ShapeBase namingShape;
        DataDescription ucDataDescription;

        public void xulyDoubleClick(PanelDoubleBuffered fmain, ShapeBase ns)
        {
            p = fmain;
            namingShape = ns;

            txtName = new TextBox();

            //cho class con định vị xong rồi add lên shape theo vị trí đó
            dinhviTextBox(txtName);

            txtName.Text = sName; //hiện tên cũ
            
            sName = "";
            this.Invalidate();

            this.Controls.Add(txtName);

            txtName.SelectAll();
            txtName.Focus();

            txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txtName_KeyPress);

            //Data type cho attribute
            if (this is AttributeShape)
            {
                if (((AttributeShape)this).attributeChilds.Count == 0)
                {
                    ucDataDescription = new DataDescription();
                    ucDataDescription.Location = new Point(this.Location.X + this.Width + 5, this.Location.Y);
                    p.Controls.Add(ucDataDescription);
                    
                    //Cho USC nằm lên trên.
                    p.Controls.SetChildIndex(ucDataDescription, 0);

                    if (((AttributeShape)this).type == AttributeType.Key)
                        ucDataDescription.chkNull.Enabled = false;

                    if(((AttributeShape)this).dataType != null)
                        ucDataDescription.cboDataType.SelectedItem = ((AttributeShape)this).dataType;
                    if(((AttributeShape)this).dataLength != 0)
                        ucDataDescription.txtLength.Text = ((AttributeShape)this).dataLength.ToString();

                    ucDataDescription.chkNull.Checked = ((AttributeShape)this).allowNull;

                    if (((AttributeShape)this).description != "")
                        ucDataDescription.txtDescription.Text = ((AttributeShape)this).description;

                    ucDataDescription.btnOK.Click += new EventHandler(btnOK_Click);
                }
            }
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            checkDuplicateName(txtName);
        }
        public void endEditName()
        {
            if(p.isNaming)
                checkDuplicateName(txtName);
        }
        void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                checkDuplicateName(txtName);
            }
        }
        private void checkDuplicateName(TextBox txtName)
        {
            Regex testName = new Regex("^[A-z][0-9A-z]*$");

            if (!testName.Match(txtName.Text).Success)
            {
                MessageBox.Show("Please enter valid name for this object\nThe name must be alphanumeric", "Warning");
                txtName.Focus();
                txtName.SelectAll();
                return;
            }

            bool isDuplicate = false;

            foreach (Control c in p.Controls)
            {
                //Kiểm tra tên đối với những Notation cùng loại
                if (c.GetType().Name == namingShape.GetType().Name && c != namingShape) 
                    if (((ShapeBase)c).sName.ToLower() == txtName.Text.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
            }
            if (!isDuplicate)
            {
                //nếu nó là attribute (viết tạm ở đây)
                if (this is AttributeShape)
                {
                    if (((AttributeShape)this).attributeChilds.Count == 0)
                    {
                        ((AttributeShape)this).dataType = ucDataDescription.cboDataType.SelectedItem.ToString();
                        if (ucDataDescription.txtLength.Text != "")
                        {
                            try
                            {
                                int l = int.Parse(ucDataDescription.txtLength.Text);
                                if ((((AttributeShape)this).dataType == "nvarchar" || ((AttributeShape)this).dataType == "nchar") && (l < 1 || l > 4000))
                                {
                                    MessageBox.Show("Please enter Length between 1 and 4000.", "Warning");
                                    return;
                                }
                                if (l < 1 || l > 8000)
                                {
                                    MessageBox.Show("Please enter Length between 1 and 8000.", "Warning");
                                    return;
                                }
                                ((AttributeShape)this).dataLength = l;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Please enter Length as positive number.", "Warning");
                                return;
                            }
                        }
                        else
                            ((AttributeShape)this).dataLength = 0;

                        ((AttributeShape)this).allowNull = ucDataDescription.chkNull.Checked;
                        ((AttributeShape)this).description = ucDataDescription.txtDescription.Text;

                        ucDataDescription.Dispose();
                    }
                }
                this.sName = txtName.Text;

                Graphics g = CreateGraphics();
                SizeF nameSize = g.MeasureString(sName, ThongSo.JFont);
                if(nameSize.Width + 30 > ThongSo.ShapeW)
                    this.Width = (int)nameSize.Width + 30;

                txtName.Dispose();
                p.isNaming = false;
                p.Refresh();
                this.Invalidate();
            }
            else
            {
                MessageBox.Show("Object " + txtName.Text + " already exist", "Warning");
                txtName.Focus();
                txtName.SelectAll();
            }
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ShapeBase
            // 
            this.BackColor = System.Drawing.Color.White;
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(100, 50);
            this.Name = "ShapeBase";
            this.Size = new System.Drawing.Size(100, 50);
            this.ResumeLayout(false);

        }

        public virtual ShapeBase Clone() { return this.Clone(); }
    }
}
