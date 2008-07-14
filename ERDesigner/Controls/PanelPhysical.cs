using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ERDesigner
{
    public class PanelPhysical:Panel
    {

        bool isResizing = false;
        bool isMoving = false;
        Point mousePoint = new Point();
        TextBox txtRename;
        Column colRenamed;
        TableShape tShapeRenamed;
        bool renamedTableName = false;
        bool renamedShape = false;
        DataDescription cbxDataType;

        //Constructor
        public PanelPhysical()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }
        
       
        //My Method
        public MetaDataPhysical getMetaDataPhysical()
        {            
            //Convert tất cả shape qua MetaDataPhysical
            MetaDataPhysical mdp = new MetaDataPhysical();

            //Duyệt tất cả controls trên Panel
            foreach (Control ctr in this.Controls)
            {
                TableShape tShape = (TableShape)ctr;
                mdp.Tables.Add(tShape.table);
                foreach (FKShape fkShape in tShape.listFK)
                {
                    ForeignKey fk = new ForeignKey();
                    fk.Name = fkShape.fkName;
                    fk.ParentTable = tShape.table.name;
                    fk.ParentColumn = fkShape.parentColumn;
                    fk.ChildTable = fkShape.tableReference.table.name;
                    fk.ChildColumn = fkShape.childColumn;
                    mdp.ForeignKeys.Add(fk);
                }
            }
            return mdp;
        }

        public void drawMetaDataPhysical(MetaDataPhysical mdp)
        {
            //Vẽ MetaDataPhysical lên this Panel
            foreach (Table t in mdp.Tables)
            {
                TableShape ts = new TableShape(t);
                ts.Location = new Point(t.x, t.y);                              
                this.Controls.Add(ts);
            }
            foreach (ForeignKey fk in mdp.ForeignKeys)
            {
                TableShape shapePrimary = searchShapeInPannel(fk.ParentTable);
                TableShape shapeReference = searchShapeInPannel(fk.ChildTable);
                FKShape fkShape = new FKShape();
                fkShape.fkName = fk.Name;
                fkShape.tableReference = shapeReference;
                fkShape.childColumn = fk.ChildColumn;
                fkShape.parentColumn = fk.ParentColumn;
                shapePrimary.listFK.Add(fkShape);
            }
            
            this.Invalidate();
        }

        private TableShape searchShapeInPannel(string tableName)
        {
            TableShape shape = new TableShape();
            foreach (Control ctr in this.Controls)
            {
                shape = (TableShape)ctr;
                if (shape.table.name == tableName)
                {
                    break;
                }

            }
            return shape;
        }

        private void endRename()
        {
            if (renamedTableName)
            {
                tShapeRenamed.table.name = txtRename.Text;
                renamedTableName = false;
            }
            else            
                colRenamed.Name = txtRename.Text;

            
            txtRename.Dispose();
            //Trước hủy Combox DataType cần phải lấy giá trị gán lại cho Column tương ứng
            if (cbxDataType != null)
            {
                colRenamed.DataType = cbxDataType.cboDataType.SelectedItem.ToString();
                colRenamed.Length = int.Parse(cbxDataType.txtLength.Text);
                colRenamed.AlowNull = cbxDataType.chkNull.Checked;
                cbxDataType.Dispose();
            }
            
            renamedShape = false;
        }

        //Event trên TextBox
        void txtRename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtRename.Text != "" && !(txtRename.Text.IndexOf(' ', 0, 1) == 0))
            {
                //Trước khi Dispose gán giá trị textBox đã Rename vào Column tương ứng.
                endRename();
            }
        }


        //Event trên Shape
        void shape_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TableShape shape = (TableShape)sender;
            int  indexColumn =0;

            //Bật trạng thái rename lên
            renamedShape = true;

            //Click vào Table Name
            if (e.Y > 0 && e.Y < ShapeSetting.heightPieceShape)
            {
                renamedTableName = true;
                txtRename = new TextBox();
                txtRename.Location = new Point(0, (indexColumn * ShapeSetting.heightPieceShape));
                txtRename.Width = shape.Width;
                txtRename.Text = shape.table.name;
                shape.Controls.Add(txtRename);
                tShapeRenamed = shape;
                txtRename.Focus();
                txtRename.SelectAll();
                txtRename.KeyDown += new KeyEventHandler(txtRename_KeyDown);
            }
            //Duyệt tất cả các Columns
            for (int i = 1; i <= shape.table.columns.Count; i++)
            {
                if (e.Y > (i * ShapeSetting.heightPieceShape) && e.Y < ((i + 1) * ShapeSetting.heightPieceShape))
                {
                    indexColumn = i;
                    break;
                }
            }
            if (indexColumn > 0)
            {
                //Tạo TextBox Rename
                txtRename = new TextBox();
                txtRename.Location= new Point(0, (indexColumn * ShapeSetting.heightPieceShape));
                txtRename.Width = shape.Width;
                txtRename.Text = shape.table.columns[indexColumn - 1].Name;                                
                shape.Controls.Add(txtRename);
                txtRename.Focus();
                txtRename.SelectAll();                
                colRenamed = shape.table.columns[indexColumn - 1]; //Lấy Column đang được Rename
                
                //Tạo Combox DataType
                cbxDataType = new DataDescription();
                cbxDataType.Location = new Point(shape.Location.X + shape.Width, shape.Location.Y+(indexColumn*ShapeSetting.heightPieceShape));
                this.Controls.Add(cbxDataType);

                this.Controls.SetChildIndex(cbxDataType, 0);

                cbxDataType.cboDataType.SelectedItem = shape.table.columns[indexColumn - 1].DataType;
                cbxDataType.txtLength.Text = shape.table.columns[indexColumn - 1].Length.ToString();
                cbxDataType.chkNull.Checked = shape.table.columns[indexColumn - 1].AlowNull;
                if (shape.table.columns[indexColumn - 1].PrimaryKey)
                {
                    cbxDataType.chkNull.Enabled = false;
                    cbxDataType.chkNull.Checked = false;
                }
                
                //Sinh Event KeyDown trên textBox
                txtRename.KeyDown += new KeyEventHandler(txtRename_KeyDown);
                cbxDataType.btnOK.Click +=new EventHandler(btnOK_Click);
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            endRename();
        }
        void shape_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
            isMoving = false;
        }

        void shape_MouseMove(object sender, MouseEventArgs e)
        {
            TableShape shape = (TableShape)sender;
            if (isResizing)
            {
                if (shape.Cursor == Cursors.SizeNWSE)
                {
                    shape.Width = e.X;
                    shape.Height = e.Y;
                }
                if (shape.Cursor == Cursors.SizeWE)
                    shape.Width = e.X;
                if (shape.Cursor == Cursors.SizeNS)
                    shape.Height = e.Y;
                this.Invalidate();
            }
            else
            {
                shape.Cursor = Cursors.Arrow;
                if ((e.X+5) >= shape.Width)
                    shape.Cursor = Cursors.SizeWE;
                if ((e.Y+5) >= shape.Height)
                    shape.Cursor = Cursors.SizeNS;
                if ((e.X +5)>= shape.Width && (e.Y+5) >= shape.Height)
                    shape.Cursor = Cursors.SizeNWSE;

            }
            if (isMoving)
            {
                shape.Left += e.X - mousePoint.X;
                shape.Top += e.Y - mousePoint.Y;
                this.Invalidate();
            }
          
        }

        void shape_MouseDown(object sender, MouseEventArgs e)
        {
            TableShape shape = (TableShape)sender;
            if (e.Button == MouseButtons.Left)//Đang Click chuột trái
            {
                mousePoint.X = e.X;
                mousePoint.Y = e.Y;
                if ((e.X + 5) >= shape.Width || (e.Y + 5) >= shape.Height)
                {
                    isResizing = true;
                }
                else
                {
                    isMoving = true;
                }
                
            }
            //Khi đang ở vùng của Shape và click chuột thì việc Rename hoàn tất 
            if (renamedShape && txtRename != null && txtRename.Text != "" && !(txtRename.Text.IndexOf(' ', 0, 1) == 0))
            {
                endRename();
            }

        }
       

        //Event trên Panel
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr.GetType().Name == "TableShape")
                {
                    Point centerTable1 = new Point(ctr.Location.X + ctr.Width / 2, ctr.Location.Y + ctr.Height / 2);
                    TableShape temp = (TableShape)ctr;
                    foreach (FKShape fk in temp.listFK)
                    {
                        Point centerTable2 = new Point(fk.tableReference.Location.X + fk.tableReference.Width / 2, fk.tableReference.Location.Y + fk.tableReference.Height / 2);
                        e.Graphics.DrawLine(new Pen(Color.Black, 1), centerTable1, centerTable2);
                    }
                }
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //Khi ra khỏi vùng của Shape và click chuột thì việc Rename hoàn tất 
            if (renamedShape && txtRename != null && txtRename.Text != "" && !(txtRename.Text.IndexOf(' ', 0, 1) == 0))
            {
                endRename();
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control.GetType().Name == "TableShape")
            {
                TableShape shape = (TableShape)e.Control;
                shape.MouseDown += new MouseEventHandler(shape_MouseDown);
                shape.MouseMove += new MouseEventHandler(shape_MouseMove);
                shape.MouseUp += new MouseEventHandler(shape_MouseUp);
                shape.MouseDoubleClick += new MouseEventHandler(shape_MouseDoubleClick);
            }

        }
    }
}
