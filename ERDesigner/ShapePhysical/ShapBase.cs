using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ERDesigner
{
    public partial class ShapBase : UserControl
    {
        public GraphicsPath path;//Quản lý vùng cần vẽ
        
        public ShapBase()
        {            
            InitializeComponent();
            path = new GraphicsPath();
        }
        public virtual void refeshPath()
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (path != null)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black, 2), this.ClientRectangle);
                e.Graphics.FillPath(Brushes.White, path);
                e.Graphics.DrawPath(new Pen(Color.Black, 1), path);
                drawSelf(e.Graphics);
            }
        }
        
        protected virtual void drawSelf(Graphics g)
        {
        }


    }
}
