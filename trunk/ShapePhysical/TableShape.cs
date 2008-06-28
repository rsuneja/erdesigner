using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ERDesigner
{
    class TableShape:ShapBase
    {
        public Table table;
        public List<FKShape> listFK;
        public TableShape()
        {
            listFK = new List<FKShape>();
        }
        public TableShape(Table t)
        {
            table = t;
            this.Font = ShapeSetting.fontTable;
            int widthShape = getWidthShape();

            this.Size = new Size(widthShape, (ShapeSetting.heightPieceShape * table.columns.Count) + ShapeSetting.heightPieceShape);
            listFK = new List<FKShape>();
            refeshPath();
        }
        //getWithShape trả về độ rộng của Shape dựa vào Attribute Name, DataType , 
        private int getWidthShape()
        {
            Graphics g = this.CreateGraphics();
            SizeF temp = new SizeF();           
            string strColumnNameMax = stringMaxLenght();
            temp += g.MeasureString(strColumnNameMax, this.Font);           
            temp += g.MeasureString(" datatype (pk)(fk)(fk) ", this.Font);   
      
            return (int)temp.Width;
        }
        public override void refeshPath()
        {
            path = new GraphicsPath();
            base.path.AddRectangle(new Rectangle(0,0,this.Size.Width,this.Size.Height));
            this.Region = new Region(path);            
        }
        
        protected override void drawSelf(Graphics g)
        {            
            int yRect = 0;
            g.DrawRectangle(new Pen(Color.Black, 1), 0, 0, this.Size.Width - 1, this.Size.Height - 1);

            //Vẽ TableName
            string nameTable = table.name;                                   
            g.DrawRectangle(new Pen(Color.Black, 1), 0, 0, this.Size.Width-1, ShapeSetting.heightPieceShape);
            g.FillRectangle(ShapeSetting.brushTableName,new Rectangle(1, 1, this.Size.Width-2, ShapeSetting.heightPieceShape-1));
            g.DrawString(nameTable, new Font("Arial", 10), ShapeSetting.brushText, new Rectangle(10, yRect, this.Size.Width-1, ShapeSetting.heightPieceShape));

            //Vẽ Attribute
            int maxLength = maxLengthText();
            foreach (Column c in table.columns)
            {
                yRect += 20;
                string strAttribute = "";
                strAttribute += c.Name.PadRight(maxLength);
                strAttribute += " " + c.DataType.PadRight(10);
                if (c.PrimaryKey)
                    strAttribute += "(pk)";
                if(c.ForeignKey)
                    strAttribute += "(fk)";                                
                g.DrawString(strAttribute, this.Font, ShapeSetting.brushText, new Rectangle(10, yRect, this.Size.Width - 1, ShapeSetting.heightPieceShape));
            }
        
        }
       
        private int maxLengthText()
        {
            int max = 0;
            foreach (Column c in table.columns)
            {
                if (max < c.Name.Length)
                    max = c.Name.Length;
            }
            return max;
        }
        private string stringMaxLenght()
        {
            string str = "";
            int max = 0;
            foreach (Column c in table.columns)
            {
                if (max < c.Name.Length)
                {
                    max = c.Name.Length;
                    str = c.Name;
                }
            }
            return str;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            refeshPath();
            this.Invalidate();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Table
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.LightGray;
            this.Name = "Table";
            this.ResumeLayout(false);

        }
    }
}
