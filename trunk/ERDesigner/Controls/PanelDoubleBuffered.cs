using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ERDesigner.Shape;
using System.Drawing;
using ERDesigner;
using System.Drawing.Drawing2D;

namespace ERDesigner
{
    public class PanelDoubleBuffered : Panel
    {
        //This object variable for tracking Drawing Relationship
        private EntityShape FirstEntity;
        private EntityShape SecondEntity;
        private EntityShape ThirdEntity;

        bool First_Mouse_Click = false;
        bool Second_Mouse_Click = false;
        bool Third_Mouse_Click = false;
        //End Declare

        public List<MetaData> UndoList = new List<MetaData>();
        public int currentUndoState = -1;

        public bool allowDrawing;
        ShapeBase currentShape = null;
        ShapeBase AffectingShape = null;

        Point First_Mouse_Pos = new Point(-1, -1);
        Point Last_Mouse_Pos = new Point(-1, -1);

        private bool isMoving = false;
        private bool isResizing = false;

        private bool isSelecting = false;

        public bool isDrawingAtt = false;
        public bool isDrawEntity = false;
        public bool isDrawRelationship = false;

        public bool isDrawing = false;
        public bool isNaming = false;

        public string DrawingShapeState;

        public int degreeOfRelationship = 2;

        public PanelDoubleBuffered()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            saveUndoList();
        }

        //Panel Event
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //Click vô panel khi đang sửa tên -> dừng sửa
            if (isNaming)
            {
                AffectingShape.endEditName();
                return;
            }
            if (e.Button == MouseButtons.Right)
            {
                CancelDrawing();
                return;
            }
            if (isDrawing)
            {
                if (isDrawEntity)
                {
                    currentShape = new EntityShape();
                    ((EntityShape)currentShape).type = DrawingShapeState;

                }

                if (isDrawingAtt && AffectingShape != null)
                {
                    if (AffectingShape is AttributeShape && ((AttributeShape)AffectingShape).isComposite && ((AttributeShape)AffectingShape).type != AttributeType.Derived && DrawingShapeState == AttributeType.Child)
                    {
                        currentShape = new AttributeShape();

                        ((AttributeShape)currentShape).type = AttributeType.Simple;

                        ((AttributeShape)currentShape).isComposite = false;
                        ((AttributeShape)AffectingShape).addAttribute((AttributeShape)currentShape);
                    }
                    else
                    {
                        if ((AffectingShape is EntityShape || AffectingShape is RelationshipShape) && DrawingShapeState != AttributeType.Child)
                        {
                            currentShape = new AttributeShape();

                            if (DrawingShapeState == AttributeType.Key)
                                ((AttributeShape)currentShape).allowNull = false;
                            ((AttributeShape)currentShape).type = DrawingShapeState;

                            if (AffectingShape is EntityShape)
                            {
                                ((EntityShape)AffectingShape).addAttribute((AttributeShape)currentShape);
                            }
                            else if (AffectingShape is RelationshipShape)
                            {
                                RelationshipShape relshape = (RelationshipShape)AffectingShape;
                                if ((relshape.cardinalities[0].MaxCardinality != -1 || relshape.cardinalities[1].MaxCardinality != -1) && ((AttributeShape)currentShape).type == AttributeType.Key)
                                {
                                    MessageBox.Show("Relationship couldn't have an Identifier Attribute");
                                    currentShape = null;
                                }
                                else
                                    relshape.addAttribute((AttributeShape)currentShape);
                            }
                        }
                    }

                }

                if (isDrawEntity || (isDrawingAtt && AffectingShape != null) && currentShape != null)
                {
                    Point mouse = this.PointToClient(Control.MousePosition);
                    currentShape.CenterPoint = mouse;
                    this.Controls.Add(currentShape);
                    beginRename(currentShape);
                }
                else
                {
                    //đang vẽ relationship (không vẽ shape, vẽ đoạn nối 2 entity)
                    //phải click lên control, click lên panel thì sai, không cho vẽ nữa
                    CancelDrawing();
                }
            }
            //click panel mà không phải là đang drawing
            else
            {
                //mark flag để vẽ rubber band
                isSelecting = true;

                First_Mouse_Pos.X = e.X;
                First_Mouse_Pos.Y = e.Y;

                Last_Mouse_Pos.X = -1;
                Last_Mouse_Pos.Y = -1;
            }

            if (!isNaming)
            {
                //Focus để làm mất selection hoặc kết thúc đặt tên
                this.Focus();
                AffectingShape = null;
                this.Invalidate();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point ptCurrent = new Point(e.X, e.Y);

            if (isDrawing && isDrawRelationship && First_Mouse_Pos.X != -1 && degreeOfRelationship == 2) // vẽ đường relationship
            {
                if (Last_Mouse_Pos.X != -1)
                    JDrawReversibleLine(First_Mouse_Pos, Last_Mouse_Pos);

                JDrawReversibleLine(First_Mouse_Pos, ptCurrent);

                Last_Mouse_Pos = ptCurrent;
            }
            else if (e.Button == MouseButtons.Left) //vẽ khung rubber band
            {
                if (isSelecting)
                {
                    if (Last_Mouse_Pos.X != -1)
                    {
                        JDrawReversibleRectangle(First_Mouse_Pos, Last_Mouse_Pos);
                    }

                    JDrawReversibleRectangle(First_Mouse_Pos, ptCurrent);

                    Last_Mouse_Pos = ptCurrent;
                }
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isDrawing)
            {
                CancelDrawing();
                currentShape = null;
            }
            if (isSelecting)
            {
                isSelecting = false;

                if (Last_Mouse_Pos.X != -1)
                {
                    JDrawReversibleRectangle(First_Mouse_Pos, Last_Mouse_Pos);
                }

                First_Mouse_Pos.X = -1;
                First_Mouse_Pos.Y = -1;
                Last_Mouse_Pos.X = -1;
                Last_Mouse_Pos.Y = -1;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            ControlPaint.DrawGrid(g, this.ClientRectangle, new Size(5, 5), Color.Black);
            //ControlPaint.DrawGrid(g, new Rectangle(this.Location, this.Size), new Size(50, 50), Color.Black);

            //Vẽ đường kết nối giữa các Shape
            DrawConnectiveLines(g);

            //Vẽ Đường bao Shape đang select
            DrawSelectionRubber(g);
        }

        //Shape (User control) Event
        void shapeMouseDown(object sender, MouseEventArgs e)
        {
            if (isNaming)
            {
                AffectingShape.endEditName();
                return;
            }

            ShapeBase currentCtrl = (ShapeBase)sender;

            //Đang vẽ relationship
            if (e.Button == MouseButtons.Left && isDrawing && isDrawRelationship)
            {
                //Nếu chưa có điểm đầu
                if (First_Mouse_Click == false)
                {
                    if (currentCtrl is EntityShape)
                    {
                        First_Mouse_Click = true;

                        //Tracking First position for Drawing Virtual Line
                        First_Mouse_Pos.X = e.X + currentCtrl.Location.X;
                        First_Mouse_Pos.Y = e.Y + currentCtrl.Location.Y;

                        FirstEntity = (EntityShape)currentCtrl;

                        if (degreeOfRelationship == 1)
                        {
                            //Create Unary Relationship
                            CreateUnaryRelationship();
                            CancelDrawing();
                            return;
                        }
                    }
                    else
                        CancelDrawing();
                }
                else
                {
                    //Ðã có điểm đầu, click lần thứ 2
                    if (Second_Mouse_Click == false)
                    {
                        if (currentCtrl is EntityShape)
                        {
                            Second_Mouse_Click = true;
                            SecondEntity = (EntityShape)currentCtrl;

                            if (degreeOfRelationship == 2)
                            {
                                //Create Binary Relationship
                                CreateBinaryRelationship();
                                CancelDrawing();
                                return;
                            }
                        }
                        else
                            CancelDrawing();
                    }
                    else
                    {
                        //Ðã có điểm thứ 1 và 2, click lần 3
                        if (Third_Mouse_Click == false)
                        {
                            if (currentCtrl is EntityShape)
                            {
                                Third_Mouse_Click = true;
                                ThirdEntity = (EntityShape)currentCtrl;

                                if (degreeOfRelationship == 3)
                                {
                                    //Create Ternary Relationship
                                    CreateTernaryRelationship();
                                    CancelDrawing();
                                    return;
                                }
                            }
                            else
                                CancelDrawing();
                        }
                        else
                            CancelDrawing();
                    }
                }
            }//End Ðang vẽ relationship

            //Lưu vết lại Shape đang được tác động (Click)
            //và set cho shape đó nằm trên cùng
            AffectingShape = currentCtrl;
            this.Controls.SetChildIndex(AffectingShape, 0);

            // click trái: Resizing hoặc Moving hoặc Selecting
            // Nếu chuột ở mép (border) của Shape thì là resizing
            // bình thường thì moving
            if (e.Button == MouseButtons.Left && !isDrawing)
            {
                Last_Mouse_Pos.X = e.X;
                Last_Mouse_Pos.Y = e.Y;

                if ((e.X + 5) > currentCtrl.Width || (e.Y + 5) > currentCtrl.Height)
                    isResizing = true;
                else
                    isMoving = true;
            }
            // click phải: hiện context menu
            else if (e.Button == MouseButtons.Right)
            {
                ContextMenu ctmn = getContexMenuForControl(currentCtrl);
                ctmn.Show(currentCtrl, new Point(e.X, e.Y));
            }
            this.Invalidate();
        }
        void shapeMouseMove(object sender, MouseEventArgs e)
        {
            if (!isNaming)
            {
                Control currentCtrl = (Control)sender;

                if (isDrawing && isDrawRelationship && First_Mouse_Pos.X != -1 && degreeOfRelationship == 2)
                {
                    Point ptCurrent = new Point(currentCtrl.Location.X + e.X, currentCtrl.Location.Y + e.Y);
                    if (Last_Mouse_Pos.X != -1)
                        JDrawReversibleLine(First_Mouse_Pos, Last_Mouse_Pos);

                    JDrawReversibleLine(First_Mouse_Pos, ptCurrent);

                    Last_Mouse_Pos = ptCurrent;
                }

                if (isMoving) //đang trong chế độ moving
                {
                    //Nếu thực sự có Move 1 khoảng khác 0
                    if (e.X - Last_Mouse_Pos.X != 0 || e.Y - Last_Mouse_Pos.Y != 0)
                    {
                        int left = currentCtrl.Left + (e.X - Last_Mouse_Pos.X);
                        int top = currentCtrl.Top + (e.Y - Last_Mouse_Pos.Y);
                        int right = left + currentCtrl.Width;
                        int bottom = top + currentCtrl.Height;

                        if (left > 0 && right < this.Width)
                            currentCtrl.Left = currentCtrl.Left + (e.X - Last_Mouse_Pos.X);
                        if (top > 0 && bottom < this.Height)
                            currentCtrl.Top = currentCtrl.Top + (e.Y - Last_Mouse_Pos.Y);

                        this.Invalidate(); // Update đường nối
                    }
                }
                else if (isResizing) //đang trong chế độ Resizing
                {
                    //Cursor sẽ tự đổi khi đụng mép Shape (UserControl) -- phần dưới
                    //Kiểm tra cursor đang là hình gì
                    string sName = ((ShapeBase)currentCtrl).sName;
                    Graphics g = CreateGraphics();
                    SizeF nameSize = g.MeasureString(sName, ThongSo.JFont);
                    nameSize.Width += 30;

                    if (currentCtrl.Width != e.X || currentCtrl.Height != e.Y)//có thay đổi
                    {
                        if (currentCtrl.Cursor == Cursors.SizeNWSE)
                        {
                            if (e.X > ThongSo.ShapeW && e.X > nameSize.Width)
                                currentCtrl.Width = e.X;
                            if (e.Y > ThongSo.ShapeH)
                                currentCtrl.Height = e.Y;
                        }
                        else if (currentCtrl.Cursor == Cursors.SizeNS)
                        {
                            if (e.Y > ThongSo.ShapeH)
                                currentCtrl.Height = e.Y;
                        }
                        else if (currentCtrl.Cursor == Cursors.SizeWE)
                        {
                            if (e.X > ThongSo.ShapeW && e.X > nameSize.Width)
                                currentCtrl.Width = e.X;
                        }

                        this.Invalidate();
                    }
                }
                else
                {
                    //đang rê chuột tính resize
                    if (!isDrawing)
                    {
                        if (e.X + 5 > currentCtrl.Width && e.Y + 5 > currentCtrl.Height) // góc
                        {
                            currentCtrl.Cursor = Cursors.SizeNWSE;
                        }
                        else if ((e.X + 5) > currentCtrl.Width) // dọc
                        {
                            currentCtrl.Cursor = Cursors.SizeWE;
                        }
                        else if ((e.Y + 5) > currentCtrl.Height) // ngang
                        {
                            currentCtrl.Cursor = Cursors.SizeNS;
                        }
                        else //Bình thường
                        {
                            currentCtrl.Cursor = (currentCtrl as ShapeBase).CurrentCursor;
                        }
                    }
                    else
                        currentCtrl.Cursor = (currentCtrl as ShapeBase).CurrentCursor;
                }
            }
        }
        void shapeMouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing && !isDrawRelationship)
            {
                First_Mouse_Pos.X = -1;
                First_Mouse_Pos.Y = -1;
                Last_Mouse_Pos.X = -1;
                Last_Mouse_Pos.Y = -1;
            }
            isMoving = false;
            isResizing = false;

        }
        void shapeDoubleClick(object sender, MouseEventArgs e)
        {
            beginRename(sender);
        }
        void shape_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                ((MainForm)TopLevelControl).deleteShape();
            }
        }

        //Menu Click Event
        void Change_Type_Click(object sender, EventArgs e)
        {
            String type = ((MenuItem)sender).Text;
            switch (type)
            {
                case "Strong Entity Type": ((EntityShape)AffectingShape).Type = EntityType.Strong;
                    break;
                case "Weak Entity Type": ((EntityShape)AffectingShape).Type = EntityType.Weak;
                    break;
                case "Simple Attribute": ((AttributeShape)AffectingShape).type = AttributeType.Simple; ((AttributeShape)AffectingShape).allowNull = true;
                    break;
                case "Multi-Valued Attribute": ((AttributeShape)AffectingShape).type = AttributeType.MultiValued; ((AttributeShape)AffectingShape).allowNull = true;
                    break;
                case "Derived Attribute": ((AttributeShape)AffectingShape).type = AttributeType.Derived; ((AttributeShape)AffectingShape).allowNull = true;
                    break;
                case "Key Attribute": ((AttributeShape)AffectingShape).type = AttributeType.Key; ((AttributeShape)AffectingShape).allowNull = false;
                    break;
                case "Normal Relationship": ((RelationshipShape)AffectingShape).type = RelationshipType.Normal;
                    break;
                case "Identifier Relationship": ((RelationshipShape)AffectingShape).type = RelationshipType.Identifier;
                    break;
                case "Associative Entity": ((RelationshipShape)AffectingShape).type = RelationshipType.AssociativeEntity;
                    break;

            }
            AffectingShape.Invalidate();
        }
        void editCardi_Click(object sender, EventArgs e)
        {
            //hiển thị Dialog
            RelationshipShape relationship = null;
            if (AffectingShape != null && AffectingShape is RelationshipShape)
            {
                relationship = (RelationshipShape)AffectingShape;
                if(relationship.cardinalities.Count < 3)
                {
                    AddCardinality addCardiDialog = new AddCardinality(relationship);

                    if (addCardiDialog.ShowDialog() == DialogResult.OK)
                    {
                        relationship.cardinalities[0].setValue(addCardiDialog.cardi1.MinCardinality, addCardiDialog.cardi1.MaxCardinality);
                        relationship.cardinalities[1].setValue(addCardiDialog.cardi2.MinCardinality, addCardiDialog.cardi2.MaxCardinality);

                        if ((relationship.cardinalities[0].MaxCardinality != -1 || relationship.cardinalities[1].MaxCardinality != -1) && relationship.type == RelationshipType.AssociativeEntity)
                            if (relationship.cardinalities[0].entity.type != relationship.cardinalities[1].entity.type)
                                relationship.type = RelationshipType.Identifier;
                            else
                                relationship.type = RelationshipType.Normal;

                        if (relationship.cardinalities[0].MaxCardinality == -1 && relationship.cardinalities[1].MaxCardinality == -1 && relationship.attributes.Count > 0)
                            relationship.type = RelationshipType.AssociativeEntity;

                        this.Invalidate();
                    }
                }
                else
                {
                    EditTernaryRelationship editRelDialog = new EditTernaryRelationship(relationship);

                    if (editRelDialog.ShowDialog() == DialogResult.OK)
                    {
                        relationship.cardinalities[0].setValue(editRelDialog.cardi1.MinCardinality, editRelDialog.cardi1.MaxCardinality);
                        relationship.cardinalities[1].setValue(editRelDialog.cardi2.MinCardinality, editRelDialog.cardi2.MaxCardinality);
                        relationship.cardinalities[2].setValue(editRelDialog.cardi3.MinCardinality, editRelDialog.cardi3.MaxCardinality);

                        if (relationship.cardinalities[0].MaxCardinality == -1 && relationship.cardinalities[1].MaxCardinality == -1 && relationship.cardinalities[2].MaxCardinality == -1 && relationship.attributes.Count > 0)
                            relationship.type = RelationshipType.AssociativeEntity;
                        else
                            relationship.type = RelationshipType.Normal;

                        this.Invalidate();
                    }
                }
                
            }

        }
        void addAtt_Click(object sender, EventArgs e)
        {
            CancelDrawing();
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            isDrawing = true;
            isDrawingAtt = true;
        }
        void del_Click(object sender, EventArgs e)
        {
            deleteAffectingShape();
        }

        //My function
        private void beginRename(object sender)
        {
            if (isNaming)
            {
                AffectingShape.endEditName();
                return;
            }
            ((ShapeBase)sender).xulyDoubleClick(this, (ShapeBase)sender);
            AffectingShape = (ShapeBase)sender;
            isNaming = true;
        }
        private void JDrawReversibleRectangle(Point p1, Point p2)
        {
            Rectangle rc = new Rectangle();

            // Convert the points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc, Color.Black, FrameStyle.Dashed);
        }
        private void JDrawReversibleLine(Point p1, Point p2)
        {
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);

            ControlPaint.DrawReversibleLine(p1, p2, Color.Black);
        }
        private void DrawSelectionRubber(Graphics g)
        {
            if (AffectingShape != null)
            {
                Point p1 = PointToScreen(new Point(AffectingShape.Location.X, AffectingShape.Location.Y));

                Rectangle selectRect = new Rectangle(AffectingShape.Location, AffectingShape.Size);
                Rectangle selectOutRect = new Rectangle(selectRect.X - 5, selectRect.Y - 5, selectRect.Width + 10, selectRect.Height + 10);

                Rectangle node1 = new Rectangle(selectOutRect.X, selectOutRect.Y, 5, 5);
                Rectangle node2 = new Rectangle(selectOutRect.X + selectOutRect.Width / 2, selectOutRect.Y, 5, 5);
                Rectangle node3 = new Rectangle(selectOutRect.X + selectOutRect.Width - 5, selectOutRect.Y, 5, 5);
                Rectangle node4 = new Rectangle(selectOutRect.X + selectOutRect.Width - 5, selectOutRect.Y + selectOutRect.Height / 2, 5, 5);
                Rectangle node5 = new Rectangle(selectOutRect.X + selectOutRect.Width - 5, selectOutRect.Y + selectOutRect.Height - 5, 5, 5);
                Rectangle node6 = new Rectangle(selectOutRect.X + selectOutRect.Width / 2, selectOutRect.Y + selectOutRect.Height - 5, 5, 5);
                Rectangle node7 = new Rectangle(selectOutRect.X, selectOutRect.Y + selectOutRect.Height - 5, 5, 5);
                Rectangle node8 = new Rectangle(selectOutRect.X, selectOutRect.Y + selectOutRect.Height / 2, 5, 5);

                ControlPaint.DrawSelectionFrame(g, true, selectOutRect, selectRect, Color.Empty);
                ControlPaint.DrawVisualStyleBorder(g, selectOutRect);
                ControlPaint.DrawVisualStyleBorder(g, selectRect);

                g.FillRectangle(Brushes.Black, node1);
                g.FillRectangle(Brushes.Black, node2);
                g.FillRectangle(Brushes.Black, node3);
                g.FillRectangle(Brushes.Black, node4);
                g.FillRectangle(Brushes.Black, node5);
                g.FillRectangle(Brushes.Black, node6);
                g.FillRectangle(Brushes.Black, node7);
                g.FillRectangle(Brushes.Black, node8);

                //ControlPaint.DrawVisualStyleBorder(g, selectRect);
                //ControlPaint.DrawLockedFrame(g, selectRect, false);
                //ControlPaint.DrawFocusRectangle(g, new Rectangle(selectRect.X - 5, selectRect.Y - 5, selectRect.Width + 10, selectRect.Height + 10));
                //ControlPaint.DrawSizeGrip(g, Color.Empty, new Rectangle(selectRect.X - 5, selectRect.Y - 5, selectRect.Width + 10, selectRect.Height + 10));

            }
        }
        private void DrawConnectiveLines(Graphics g)
        {
            //Tính lại tất cả vị trí
            UpdateAllPosition();

            //Băt đầu vẽ
            foreach (Control s in this.Controls)
            {
                if (s.GetType().Name == "EntityShape" || s.GetType().Name == "RelationshipShape" || s.GetType().Name == "AttributeShape")
                {
                    if (s.GetType().Name == "EntityShape")
                    {
                        foreach (AttributeShape att in ((EntityShape)s).attributes)
                        {
                            Point centerEntity = new Point(s.Location.X + s.Width / 2, s.Location.Y + s.Height / 2);
                            Point centerAtt = new Point(att.Location.X + att.Width / 2, att.Location.Y + att.Height / 2);
                            g.DrawLine(new Pen(Color.Black, 1), centerEntity.X, centerEntity.Y, centerAtt.X, centerAtt.Y);
                        }
                        EntityShape entity = (EntityShape)s;
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < entity.cardinalities[i].Count; j++)
                            {
                                DrawCardinalitiesLine(g, entity.cardinalities[i][j], i + 1, j + 1, entity.cardinalities[i].Count);
                            }
                        }
                    }
                    else if (s.GetType().Name == "RelationshipShape")
                    {
                        foreach (AttributeShape att in ((RelationshipShape)s).attributes)
                        {
                            Point centerRelationship = new Point(s.Location.X + s.Width / 2, s.Location.Y + s.Height / 2);
                            Point centerAtt = new Point(att.Location.X + att.Width / 2, att.Location.Y + att.Height / 2);
                            g.DrawLine(new Pen(Color.Black, 1), centerRelationship.X, centerRelationship.Y, centerAtt.X, centerAtt.Y);
                        }
                    }
                    else if (s.GetType().Name == "AttributeShape" && ((AttributeShape)s).isComposite)
                    {
                        foreach (AttributeShape att in ((AttributeShape)s).attributeChilds)
                        {
                            Point centerAttComposite = new Point(s.Location.X + s.Width / 2, s.Location.Y + s.Height / 2);
                            Point centerAttChild = new Point(att.Location.X + att.Width / 2, att.Location.Y + att.Height / 2);
                            g.DrawLine(new Pen(Color.Black, 1), centerAttComposite.X, centerAttComposite.Y, centerAttChild.X, centerAttChild.Y);
                        }
                    }
                }
            }
        }
        public bool deleteAffectingShape()
        {
            if (AffectingShape != null && !isDrawing && !isNaming && !isMoving && !isResizing && !isSelecting)
            {
                if (AffectingShape.GetType().Name == "EntityShape")
                {
                    if (MessageBox.Show("Are you sure want to delete " + AffectingShape.sName + " Entity ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        saveUndoList();
                        AffectingShape.Dispose();
                        AffectingShape = null;

                        this.Invalidate();
                    }
                }
                else
                {
                    saveUndoList();
                    AffectingShape.Dispose();
                    AffectingShape = null;

                    this.Invalidate();

                }
                return true;
            }
            return false;
        }
        public void saveUndoList()
        {
            MetaData ERD = this.getMetaData();
            currentUndoState++;
            //UndoList.RemoveRange(currentUndoState, UndoList.Count - currentUndoState);
            UndoList.Add(ERD);

        }
        public bool doUndo()
        {
            if (UndoList.Count > 0 && currentUndoState > 0)
            {
                this.drawMetaData(UndoList[currentUndoState - 1]);
                //UndoList.RemoveAt(UndoList.Count - 1);
                currentUndoState--;

                if (currentUndoState == 0)
                    return false;
                else
                    return true;
            }
            return false;
        }
        public bool doRedo()
        {
            if (UndoList.Count > 0 && UndoList.Count - 1 > currentUndoState)
            {
                this.drawMetaData(UndoList[currentUndoState + 1]);
                currentUndoState++;

                if (currentUndoState == UndoList.Count)
                    return false;
                else
                    return true;
            }
            return false;
        }
        void UpdateAllPosition()
        {
            foreach (Control s in this.Controls)
            {
                if (s.GetType().Name == "EntityShape")
                {
                    EntityShape entity = (EntityShape)s;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < entity.cardinalities[i].Count; j++)
                        {
                            CardinalityShape cardi = entity.cardinalities[i][j];

                            RelationshipShape rel = cardi.relationship;

                            Point centerRelationship = new Point(rel.Location.X + rel.Width / 2, rel.Location.Y + rel.Height / 2);
                            Point centerEntity = new Point(entity.Location.X + entity.Width / 2, entity.Location.Y + entity.Height / 2);

                            int oldEdgeCardiPlace = i + 1;
                            int newEdgeCardiPlace = CalculateEdgePlace(entity, centerRelationship, centerEntity);

                            //Chổ này làm kỹ, không thôi nó giựt wài ghét lắm

                            //Nếu vị trí mới tính được khác vị trí cũ
                            if (oldEdgeCardiPlace != newEdgeCardiPlace)
                            {
                                entity.cardinalities[oldEdgeCardiPlace - 1].Remove(cardi);

                                int index = 0;

                                foreach (CardinalityShape cardiInEntity in entity.cardinalities[newEdgeCardiPlace - 1])
                                {
                                    if (newEdgeCardiPlace == 1 || newEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                                    {
                                        if (cardi.relationship.Location.X > cardiInEntity.relationship.Location.X)
                                            index++;
                                    }
                                    else //right hoặc left thì so sánh y
                                    {
                                        if (cardi.relationship.Location.Y > cardiInEntity.relationship.Location.Y)
                                            index++;
                                    }
                                }

                                entity.insertCardinality(newEdgeCardiPlace - 1, index, cardi);
                            }
                            else //Nếu vẫn là vị trí cũ, thì tính lại thứ tự trong vị trí đó
                            {
                                for (int k = 0; k < entity.cardinalities[oldEdgeCardiPlace - 1].Count - 1; k++)
                                {
                                    if (oldEdgeCardiPlace == 1 || oldEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                                    {
                                        if (entity.cardinalities[oldEdgeCardiPlace - 1][k].relationship.Location.X > entity.cardinalities[oldEdgeCardiPlace - 1][k + 1].relationship.Location.X)
                                            entity.cardinalities[oldEdgeCardiPlace - 1].Reverse(k, 1);
                                    }
                                    else //right hoặc left thì so sánh y
                                    {
                                        if (entity.cardinalities[oldEdgeCardiPlace - 1][k].relationship.Location.Y > entity.cardinalities[oldEdgeCardiPlace - 1][k + 1].relationship.Location.Y)
                                            entity.cardinalities[oldEdgeCardiPlace - 1].Reverse(k, 1);
                                    }
                                }
                            }

                            //Nhắm mắt add cardi vô cạnh mới khỏi tính gì cả - hix
                            //xóa cardi ở cạnh cũ đi
                            //for (int k = 0; k < 4; k++)
                            //    entity.cardinalities[k].Remove(cardi);

                            //add cardi vào cạnh mới
                            //entity.insertCardinality(newEdgeCardiPlace - 1, index, cardi);


                            if (rel.type == RelationshipType.AssociativeEntity)
                            {
                                newEdgeCardiPlace = CalculateEdgePlace(rel, centerEntity, centerRelationship);

                                //Tính lại thứ tự của cardi trong Asso En
                                int index = 0;
                                foreach (CardinalityShape cardiInEntity in rel.cardiplaces[newEdgeCardiPlace - 1])
                                {
                                    if (newEdgeCardiPlace == 1 || newEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                                    {
                                        if (cardi.entity.Location.X > cardiInEntity.entity.Location.X)
                                            index++;
                                    }
                                    else //right hoặc left thì so sánh y
                                    {
                                        if (cardi.entity.Location.Y > cardiInEntity.entity.Location.Y)
                                            index++;
                                    }
                                }

                                //Nhắm mắt add cardi vô cạnh mới khỏi tính gì cả - hix
                                for (int k = 0; k < 4; k++)
                                    rel.cardiplaces[k].Remove(cardi);

                                rel.insertCardiPlace(newEdgeCardiPlace - 1, index, cardi);
                            }
                        }
                    }
                }
            }
        }
        private void DrawCardinalitiesLine(Graphics g, CardinalityShape cardi, int edgeCardiPlace, int pos, int numCardi)
        {
            EntityShape entity = cardi.entity;
            RelationshipShape rel = cardi.relationship;

            Point centerRelationship = new Point(rel.Location.X + rel.Width / 2, rel.Location.Y + rel.Height / 2);
            Point centerEntity = new Point(entity.Location.X + entity.Width / 2, entity.Location.Y + entity.Height / 2);

            Point TopLeft = entity.Location;
            Point BottomRight = new Point(entity.Location.X + entity.Width, entity.Location.Y + entity.Height);

            int stepX = pos * entity.Width / (numCardi + 1);
            int stepY = pos * entity.Height / (numCardi + 1);

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                Bitmap myCardi = GetCardinalitiesShape(cardi, edgeCardiPlace);
                switch (edgeCardiPlace)
                {
                    case 1: g.DrawImage(myCardi, TopLeft.X + stepX - myCardi.Width / 2, TopLeft.Y - myCardi.Height);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, TopLeft.X + stepX, TopLeft.Y - myCardi.Height);
                        break;
                    case 2: g.DrawImage(myCardi, BottomRight.X, BottomRight.Y - (entity.Height - stepY) - myCardi.Height / 2);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, BottomRight.X + myCardi.Width, BottomRight.Y - (entity.Height - stepY));
                        break;
                    case 3: g.DrawImage(myCardi, BottomRight.X - (entity.Width - stepX) - myCardi.Width / 2 + 1, BottomRight.Y);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, BottomRight.X - (entity.Width - stepX), BottomRight.Y + myCardi.Height);
                        break;
                    case 4: g.DrawImage(myCardi, TopLeft.X - myCardi.Width, TopLeft.Y + stepY - myCardi.Height / 2 + 1);
                        g.DrawLine(ThongSo.ConectiveLinePen, centerRelationship.X, centerRelationship.Y, TopLeft.X - myCardi.Width, TopLeft.Y + stepY);
                        break;
                }
            }
            else //nếu relationship là Associative Entity
            {
                Point TopLeftRel = rel.Location;
                Point BottomRightRel = new Point(rel.Location.X + rel.Width, rel.Location.Y + rel.Height);

                int newEdgeCardiPlace = CalculateEdgePlace(rel, centerEntity, centerRelationship);

                //Tính lại thứ tự của cardi trong Asso En
                int index = 0;
                foreach (CardinalityShape cardiInEntity in rel.cardiplaces[newEdgeCardiPlace - 1])
                {
                    if (newEdgeCardiPlace == 1 || newEdgeCardiPlace == 3) //Up hoặc down thì so sánh x
                    {
                        if (cardi.entity.Location.X > cardiInEntity.entity.Location.X)
                            index++;
                    }
                    else //right hoặc left thì so sánh y
                    {
                        if (cardi.entity.Location.Y > cardiInEntity.entity.Location.Y)
                            index++;
                    }
                }

                //Nhắm mắt add cardi vô cạnh mới khỏi tính gì cả - hix
                for (int k = 0; k < 4; k++)
                    rel.cardiplaces[k].Remove(cardi);

                rel.insertCardiPlace(newEdgeCardiPlace - 1, index, cardi);

                int stepXRel = (index + 1) * rel.Width / (rel.cardiplaces[newEdgeCardiPlace - 1].Count + 1);
                int stepYRel = (index + 1) * rel.Height / (rel.cardiplaces[newEdgeCardiPlace - 1].Count + 1);

                //cardi 1, 1
                CardinalityShape cardi1 = new CardinalityShape();
                cardi1.setValue(1, 1);

                //lấy cardi của đầu bên kia Relationship
                CardinalityShape cardi2 = new CardinalityShape();
                foreach (CardinalityShape card in cardi.relationship.cardinalities)
                {
                    if (card != cardi)
                        cardi2 = card;
                }

                Bitmap CardiAtEn = GetCardinalitiesShape(cardi1, edgeCardiPlace);
                Bitmap CardiAtRel = GetCardinalitiesShape(cardi2, newEdgeCardiPlace);

                //Vẽ cardi cho En
                switch (edgeCardiPlace)
                {
                    case 1: g.DrawImage(CardiAtEn, TopLeft.X + stepX - CardiAtEn.Width / 2, TopLeft.Y - CardiAtEn.Height);
                        break;
                    case 2: g.DrawImage(CardiAtEn, BottomRight.X, BottomRight.Y - (entity.Height - stepY) - CardiAtEn.Height / 2);
                        break;
                    case 3: g.DrawImage(CardiAtEn, BottomRight.X - (entity.Width - stepX) - CardiAtEn.Width / 2 + 1, BottomRight.Y);
                        break;
                    case 4: g.DrawImage(CardiAtEn, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY - CardiAtEn.Height / 2 + 1);
                        break;
                }

                //Vẽ cardi cho Ass En
                switch (newEdgeCardiPlace)
                {
                    case 1: g.DrawImage(CardiAtRel, TopLeftRel.X + stepXRel - CardiAtRel.Width / 2, TopLeftRel.Y - CardiAtRel.Height);
                        break;
                    case 2: g.DrawImage(CardiAtRel, BottomRightRel.X, BottomRightRel.Y - (rel.Height - stepYRel) - CardiAtRel.Height / 2);
                        break;
                    case 3: g.DrawImage(CardiAtRel, BottomRightRel.X - (rel.Width - stepXRel) - CardiAtRel.Width / 2 + 1, BottomRightRel.Y);
                        break;
                    case 4: g.DrawImage(CardiAtRel, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel - CardiAtRel.Height / 2 + 1);
                        break;
                }

                //Vẽ Line
                switch (edgeCardiPlace)
                {
                    case 1:
                        switch (newEdgeCardiPlace)
                        {
                            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), TopLeft.X + stepX, TopLeft.Y - CardiAtEn.Height);
                                break;
                            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, TopLeft.X + stepX, TopLeft.Y - CardiAtRel.Height);
                                break;
                            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, TopLeft.X + stepX, TopLeft.Y - CardiAtRel.Height);
                                break;
                        }
                        break;
                    case 2:
                        switch (newEdgeCardiPlace)
                        {
                            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                                break;
                            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                                break;
                            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, BottomRight.X + CardiAtRel.Width, BottomRight.Y - (entity.Height - stepY));
                                break;
                        }
                        break;
                    case 3:
                        switch (newEdgeCardiPlace)
                        {
                            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtRel.Height);
                                break;
                            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtEn.Height);
                                break;
                            case 4: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X - CardiAtRel.Width, TopLeftRel.Y + stepYRel, BottomRight.X - (entity.Width - stepX), BottomRight.Y + CardiAtEn.Height);
                                break;
                        }
                        break;
                    case 4:
                        switch (newEdgeCardiPlace)
                        {
                            case 1: g.DrawLine(ThongSo.ConectiveLinePen, TopLeftRel.X + stepXRel, TopLeftRel.Y - CardiAtRel.Height, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                                break;
                            case 2: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X + CardiAtRel.Width, BottomRightRel.Y - (rel.Height - stepYRel), TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                                break;
                            case 3: g.DrawLine(ThongSo.ConectiveLinePen, BottomRightRel.X - (rel.Width - stepXRel), BottomRightRel.Y + CardiAtRel.Height, TopLeft.X - CardiAtEn.Width, TopLeft.Y + stepY);
                                break;
                        }
                        break;
                }

            }
        }
        public static int CalculateEdgePlace(ShapeBase entity, Point centerRelationship, Point centerEntity)
        {
            //Cạnh đặt Shape - > 1,2,3,4: Up, Right, Down, Left
            int edgeCardiPlace = -1;

            //Cách khác: phương trình qua 2 điểm centerRelationship và centerEntity (y = mx + b)
            double m = centerRelationship.Y - centerEntity.Y;
            if (centerRelationship.X != centerEntity.X)
                m /= centerRelationship.X - centerEntity.X;
            else
                m = 0;

            double b = centerRelationship.Y - m * centerRelationship.X;


            if (centerEntity.X < centerRelationship.X) //Relation ship nằm bên phải
            {
                //thế x của centerEntity vào phương trình để tính y
                //Nếu y vượt quá khoảng y của Entity thì đặt cardi ở up hoặc down: else đặt right
                double ytemp = m * (entity.Location.X + entity.Width) + b;

                if (ytemp < entity.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > entity.Location.Y + entity.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= entity.Location.Y && ytemp <= entity.Location.Y + entity.Height) edgeCardiPlace = 2; //Right
            }
            else
            {
                //thế x của centerEntity vào phương trình để tính y
                //Nếu y vượt quá khoảng y của Entity thì đặt cardi ở up hoặc down: else đặt right
                double ytemp = m * (entity.Location.X) + b;

                if (ytemp < entity.Location.Y) edgeCardiPlace = 1; //Up
                else if (ytemp > entity.Location.Y + entity.Height) edgeCardiPlace = 3; //Down

                if (ytemp >= entity.Location.Y && ytemp <= entity.Location.Y + entity.Height) edgeCardiPlace = 4; //Left
            }
            return edgeCardiPlace;
        }
        private Bitmap GetCardinalitiesShape(CardinalityShape cardi, int edgeCardiPlace)
        {
            Bitmap carshape = new Bitmap(20, 20);
            Graphics gimage = Graphics.FromImage(carshape);

            if (cardi.MaxCardinality == -1)
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(3, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 8), new Point(17, 20));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    gimage.FillEllipse(new SolidBrush(this.BackColor), 7, 0, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 0, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 5), new Point(17, 5));
            }
            else
            {
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 13), new Point(17, 13));
                gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(10, 0), new Point(10, 20));

                if (cardi.MinCardinality == 0)
                {
                    gimage.FillEllipse(new SolidBrush(this.BackColor), 7, 3, 6, 6);
                    gimage.DrawEllipse(ThongSo.ConectiveLinePen, 7, 3, 6, 6);
                }
                else
                    gimage.DrawLine(ThongSo.ConectiveLinePen, new Point(3, 8), new Point(17, 8));
            }
            switch (edgeCardiPlace)
            {
                case 1:
                    break;
                case 2: carshape.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 3: carshape.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4: carshape.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            return carshape;
        }
        public void CancelDrawing()
        {
            isDrawing = false;
            isDrawEntity = false;
            isDrawingAtt = false;
            isDrawRelationship = false;
            this.Cursor = Cursors.Default;
            setAllControlToDefaultCursor();

            First_Mouse_Pos.X = -1;
            First_Mouse_Pos.Y = -1;
            Last_Mouse_Pos.X = -1;
            Last_Mouse_Pos.Y = -1;

            First_Mouse_Click = false;
            Second_Mouse_Click = false;
            Third_Mouse_Click = false;

            FirstEntity = null;
            SecondEntity = null;
            ThirdEntity = null;

            ((MainForm)TopLevelControl).CancelDrawing();
            this.Refresh();
        }
        public void AutoLayout()
        {
            //B1: Lấy Entity Bậc cao nhất
            EntityShape MaxEntity = new EntityShape();
            int maxbac = -1;
            foreach (Control c in this.Controls)
            {
                if (c.GetType().Name == "EntityShape")
                {
                    int bac = degreeOfEntity((EntityShape)c);

                    if (bac > maxbac)
                    {
                        maxbac = bac;
                        MaxEntity = (EntityShape)c;
                    }
                }
            }

            //B2: Vẽ con của nó
            SetLocationTree(MaxEntity, maxbac, 1);
            this.Refresh();

        }
        void SetLocationTree(EntityShape entity, int degree, int line)
        {
            entity.Location = new Point(this.Width / 2 - entity.Width / 2, 150 * line);

            int stepX = this.Width / (degree + 1);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < entity.cardinalities[i].Count; j++)
                {
                    CardinalityShape cardi = new CardinalityShape();

                    foreach (CardinalityShape cardi2 in entity.cardinalities[i][j].relationship.cardinalities)
                    {
                        if (cardi2 != entity.cardinalities[i][j])
                            cardi = cardi2;
                    }

                    EntityShape entity2 = cardi.entity;
                    entity2.Location = new Point(stepX, 150 * (line + 1));
                    stepX += this.Width / (degree + 1);

                    Point centerEntity1 = new Point(entity.Location.X + entity.Width / 2, entity.Location.Y + entity.Height / 2);
                    Point centerEntity2 = new Point(entity2.Location.X + entity2.Width / 2, entity2.Location.Y + entity2.Height / 2);

                    Point center = new Point(Math.Abs(centerEntity1.X + centerEntity2.X) / 2, Math.Abs(centerEntity1.Y + centerEntity2.Y) / 2);

                    cardi.relationship.Location = new Point(center.X - cardi.relationship.Width / 2, center.Y - cardi.relationship.Height / 2);

                }
            }
        }
        int degreeOfEntity(EntityShape entity)
        {
            int bac = 0;
            bac += entity.cardinalities[0].Count;
            bac += entity.cardinalities[1].Count;
            bac += entity.cardinalities[2].Count;
            bac += entity.cardinalities[3].Count;

            return bac;
        }
        public MetaData getMetaData()
        {
            MetaData ERD = new MetaData();
            foreach (Control c in this.Controls)
            {
                if (c is EntityShape || c is RelationshipShape)
                {
                    ShapeBase s = (ShapeBase)c;
                    if (s is EntityShape)
                    {
                        EntityData entity = new EntityData(s.sName, ((EntityShape)s).type, s.Location.X, s.Location.Y, s.Width, s.Height);

                        foreach (AttributeShape att in ((EntityShape)s).attributes)
                        {
                            AttributeData attribute = new AttributeData(att.sName, ((AttributeShape)att).type, att.Location.X, att.Location.Y, att.Width, att.Height, att.dataType, att.dataLength, att.allowNull, att.description);
                            if (att.isComposite)
                            {
                                attribute.isComposite = true;
                                foreach (AttributeShape attchild in ((AttributeShape)att).attributeChilds)
                                {
                                    AttributeData attributechild = new AttributeData(attchild.sName, ((AttributeShape)attchild).type, attchild.Location.X, attchild.Location.Y, attchild.Width, attchild.Height, attchild.dataType, attchild.dataLength, attchild.allowNull, att.description);

                                    attribute.AttributeChilds.Add(attributechild);
                                }
                            }
                            entity.Attributes.Add(attribute);
                        }
                        ERD.Entities.Add(entity);
                    }
                    if (s is RelationshipShape)
                    {
                        RelationshipData relationship = new RelationshipData(s.sName, ((RelationshipShape)s).type, s.Location.X, s.Location.Y, s.Width, s.Height);

                        foreach (AttributeShape att in ((RelationshipShape)s).attributes)
                        {
                            AttributeData attribute = new AttributeData(att.sName, ((AttributeShape)att).type, att.Location.X, att.Location.Y, att.Width, att.Height, att.dataType, att.dataLength, att.allowNull, att.description);
                            if (att.isComposite)
                            {
                                attribute.isComposite = true;
                                foreach (AttributeShape attchild in ((AttributeShape)att).attributeChilds)
                                {
                                    AttributeData attributechild = new AttributeData(attchild.sName, ((AttributeShape)attchild).type, attchild.Location.X, attchild.Location.Y, attchild.Width, attchild.Height, attchild.dataType, attchild.dataLength, attchild.allowNull, att.description);

                                    attribute.AttributeChilds.Add(attributechild);
                                }
                            }
                            relationship.Attributes.Add(attribute);
                        }

                        foreach (CardinalityShape cardi in ((RelationshipShape)s).cardinalities)
                        {
                            CardinalityData cardinality = new CardinalityData(cardi.entity.sName, cardi.MinCardinality, cardi.MaxCardinality);

                            relationship.Cardinalities.Add(cardinality);
                        }
                        ERD.Relationships.Add(relationship);
                    }
                }
            }
            return ERD;
        }
        public void drawMetaData(MetaData ERD)
        {
            this.Controls.Clear(); //Xóa hết, vẽ lại 

            // Vẽ entity và Attribute của nó
            foreach (EntityData en in ERD.Entities)
            {
                EntityShape entity = new EntityShape();

                entity.sName = en.name;
                entity.type = en.type;
                entity.Location = new Point(en.x, en.y);
                entity.Size = new Size(en.w, en.h);

                this.Controls.Add(entity);

                foreach (AttributeData att in en.Attributes)
                {
                    AttributeShape attribute = new AttributeShape();

                    attribute.sName = att.name;
                    attribute.type = att.type;
                    attribute.Location = new Point(att.x, att.y);
                    attribute.Size = new Size(att.w, att.h);
                    attribute.dataType = att.DataType;
                    attribute.dataLength = att.Length;
                    attribute.allowNull = att.AllowNull;
                    attribute.description = att.Description;

                    if (att.isComposite)
                    {
                        foreach (AttributeData attChild in att.AttributeChilds)
                        {
                            AttributeShape attributeChild = new AttributeShape();

                            attributeChild.sName = attChild.name;
                            attributeChild.type = attChild.type;
                            attributeChild.Location = new Point(attChild.x, attChild.y);
                            attributeChild.Size = new Size(attChild.w, attChild.h);
                            attributeChild.isComposite = false;
                            attributeChild.dataType = attChild.DataType;
                            attributeChild.dataLength = attChild.Length;
                            attributeChild.allowNull = attChild.AllowNull;
                            attributeChild.description = attChild.Description;

                            attribute.addAttribute(attributeChild);

                            this.Controls.Add(attributeChild);
                        }
                    }

                    //Không xài hàm này nữa
                    //entity.attributes.Add(attribute);

                    //Thay bằng
                    entity.addAttribute(attribute);

                    this.Controls.Add(attribute);

                }
            }

            //Phù, mệt wá, ráng thôi, Vẽ tiếp relationship, attribute và cardinality của nó
            foreach (RelationshipData rel in ERD.Relationships)
            {
                RelationshipShape relationship = new RelationshipShape();

                relationship.sName = rel.name;
                relationship.type = rel.type;
                relationship.Location = new Point(rel.x, rel.y);
                relationship.Size = new Size(rel.w, rel.h);

                this.Controls.Add(relationship);

                foreach (AttributeData att in rel.Attributes)
                {
                    AttributeShape attribute = new AttributeShape();

                    attribute.sName = att.name;
                    attribute.type = att.type;
                    attribute.Location = new Point(att.x, att.y);
                    attribute.Size = new Size(att.w, att.h);
                    attribute.dataType = att.DataType;
                    attribute.dataLength = att.Length;
                    attribute.allowNull = att.AllowNull;
                    attribute.description = att.Description;
                    //relationship.attributes.Add(attribute);

                    if (att.isComposite)
                    {
                        foreach (AttributeData attChild in att.AttributeChilds)
                        {
                            AttributeShape attributeChild = new AttributeShape();

                            attributeChild.sName = attChild.name;
                            attributeChild.type = attChild.type;
                            attributeChild.Location = new Point(attChild.x, attChild.y);
                            attributeChild.Size = new Size(attChild.w, attChild.h);
                            attributeChild.isComposite = false;
                            attributeChild.dataType = attChild.DataType;
                            attributeChild.dataLength = attChild.Length;
                            attributeChild.allowNull = attChild.AllowNull;
                            attributeChild.description = attChild.Description;

                            attribute.addAttribute(attributeChild);

                            this.Controls.Add(attributeChild);
                        }
                    }

                    relationship.addAttribute(attribute);

                    this.Controls.Add(attribute);

                }

                foreach (CardinalityData cardi in rel.Cardinalities)
                {
                    //Tìm Entity để add vào Cardinality
                    foreach (ShapeBase s in this.Controls)
                    {
                        if (s.GetType().Name == "EntityShape" && s.sName == cardi.Entity)
                        {
                            CardinalityShape cardinality = new CardinalityShape((EntityShape)s);
                            //Bắt buộc add vô rel trước
                            relationship.addCardinality(cardinality);

                            cardinality.setValue(cardi.MinCardinality, cardi.MaxCardinality);
                            break;
                        }
                    }
                }
            }
            this.Refresh();
        }
        public List<string> VerifyModel()
        {
            List<string> errorList = new List<string>();

            foreach (Control c in this.Controls)
            {
                switch (c.GetType().Name)
                {
                    case "EntityShape": EntityShape entity = (EntityShape)c;
                        //thực thể đã có thuộc tính chưa
                        if (entity.attributes.Count == 0)
                            errorList.Add("Entity '" + entity.sName + "' doesn't have attribute yet");

                        //thực thể mạnh phải có thuộc tính khóa
                        if (entity.type == EntityType.Strong)
                        {
                            bool isError = true;
                            foreach (AttributeShape att in entity.attributes)
                                if (att.type == AttributeType.Key)
                                    isError = false;

                            if (isError)
                                errorList.Add("Entity '" + entity.sName + "' must have a key attribute");
                        }

                        //Thực thể yếu phải nối với identify relationship
                        if (entity.type == EntityType.Weak)
                        {
                            for (int i = 0; i < 4; i++)
                                foreach (CardinalityShape cardi in entity.cardinalities[i])
                                {
                                    if (cardi.relationship.type != RelationshipType.Identifier)
                                    {
                                        errorList.Add("Relationship '" + cardi.relationship.sName + "' must be an Identify Relationship");
                                    }
                                }
                        }
                        break;

                    case "RelationshipShape": RelationshipShape rel = (RelationshipShape)c;
                        //mối kết hợp xác định có hợp lý chưa
                        if (rel.type == RelationshipType.Identifier)
                        {
                            EntityShape entity1 = rel.cardinalities[0].entity;
                            EntityShape entity2 = rel.cardinalities[1].entity;

                            if (entity1.type == entity2.type)
                                errorList.Add("Identify Relationship '" + rel.sName + "' must connect weak entity anh strong entity");
                            else
                            {
                                if (entity1.type == EntityType.Strong && rel.cardinalities[0].MaxCardinality != 1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Strong Entity " + entity1.sName + " must be one");
                                if (entity2.type == EntityType.Strong && rel.cardinalities[1].MaxCardinality != 1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Strong Entity " + entity2.sName + " must be one");

                                if (entity1.type == EntityType.Weak && rel.cardinalities[0].MaxCardinality != -1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Weak Entity " + entity1.sName + " must be many");
                                if (entity2.type == EntityType.Weak && rel.cardinalities[1].MaxCardinality != -1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Weak Entity " + entity2.sName + " must be many");
                            }
                        }

                        //thực thể kết hợp phải có thuộc tính
                        if (rel.type == RelationshipType.AssociativeEntity)
                        {
                            if (rel.attributes.Count == 0)
                                errorList.Add("Associative Entity '" + rel.sName + "' doesn't have attribute yet");

                            if (rel.cardinalities[0].MaxCardinality != -1 || rel.cardinalities[1].MaxCardinality != -1)
                                errorList.Add("The Cardinalities of Associative Entity '" + rel.sName + "' must be many to many");
                        }

                        //Nếu cardinality là n:n thì relationship phải là Associative Entity
                        //if (rel.cardinalities[0].MaxCardinality == -1 && rel.cardinalities[1].MaxCardinality == -1 && rel.type != RelationshipType.AssociativeEntity)
                        //    errorList.Add("Relationship '" + rel.sName + "' should be changed to Associative Entity");

                        break;
                    case "AttributeShape": AttributeShape attribute = (AttributeShape)c;
                        //Kiểm tra thuộc tính composite có ít nhất 2 con
                        if (attribute.attributeChilds.Count == 1)
                            errorList.Add("Composite Attribute '" + attribute.sName + "' must have more than one child");
                        break;
                }
            }

            //Kiểm tra đồ thị liên thông

            return errorList;
        }
        public void EndEditDataType()
        {
            if (AffectingShape != null && AffectingShape.GetType().Name == "AttributeShape" && isNaming)
            {
                ((ShapeBase)AffectingShape).endEditName();
            }
        }
        private ContextMenu getContexMenuForControl(ShapeBase currentCtrl)
        {
            ContextMenu ctmn = new ContextMenu();

            MenuItem del = new MenuItem("Delete");
            del.Click += new EventHandler(del_Click);

            MenuItem addCardi = new MenuItem("Edit Cardinalitiy");
            addCardi.Click += new EventHandler(editCardi_Click);

            MenuItem ChangeType = new MenuItem("Change type");

            if (currentCtrl is EntityShape)
            {
                ChangeType.MenuItems.Clear();

                MenuItem StrongEntity = new MenuItem("Strong Entity Type");
                StrongEntity.Click += new EventHandler(Change_Type_Click);

                MenuItem WeakEntity = new MenuItem("Weak Entity Type");
                WeakEntity.Click += new EventHandler(Change_Type_Click);

                ChangeType.MenuItems.Add(StrongEntity);
                ChangeType.MenuItems.Add(WeakEntity);
            }

            if (currentCtrl is RelationshipShape)
            {
                ctmn.MenuItems.Add(addCardi);

                ChangeType.MenuItems.Clear();

                MenuItem NormalRelation = new MenuItem("Normal Relationship");
                NormalRelation.Click += new EventHandler(Change_Type_Click);

                MenuItem IdRelation = new MenuItem("Identifier Relationship");
                IdRelation.Click += new EventHandler(Change_Type_Click);

                MenuItem AssoRelation = new MenuItem("Associative Entity");
                AssoRelation.Click += new EventHandler(Change_Type_Click);

                ChangeType.MenuItems.Add(NormalRelation);
                ChangeType.MenuItems.Add(IdRelation);
                ChangeType.MenuItems.Add(AssoRelation);
            }

            ctmn.MenuItems.Add(ChangeType);

            if (currentCtrl is AttributeShape && ((AttributeShape)currentCtrl).isComposite)
            {
                ChangeType.MenuItems.Clear();

                MenuItem KeyAtt = new MenuItem("Key Attribute");
                KeyAtt.Click += new EventHandler(Change_Type_Click);

                MenuItem SimpleAtt = new MenuItem("Simple Attribute");
                SimpleAtt.Click += new EventHandler(Change_Type_Click);

                MenuItem MultiAtt = new MenuItem("Multi-Valued Attribute");
                MultiAtt.Click += new EventHandler(Change_Type_Click);

                if (((AttributeShape)currentCtrl).attributeChilds.Count == 0)
                {
                    MenuItem DerivedAtt = new MenuItem("Derived Attribute");
                    DerivedAtt.Click += new EventHandler(Change_Type_Click);
                    ChangeType.MenuItems.Add(DerivedAtt);
                }

                ChangeType.MenuItems.Add(KeyAtt);
                ChangeType.MenuItems.Add(SimpleAtt);
                ChangeType.MenuItems.Add(MultiAtt);

            }
            if (currentCtrl.GetType().Name == "AttributeShape" && !((AttributeShape)currentCtrl).isComposite)
            {
                ctmn.MenuItems.Remove(ChangeType);
            }
            ctmn.MenuItems.Add(del);

            return ctmn;
        }
        public void setAllControlToDefaultCursor()
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr is ShapeBase)
                    (ctr as ShapeBase).CurrentCursor = Cursors.Default;
            }
        }
        public void setAllControlToNoCursor()
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr is ShapeBase)
                    (ctr as ShapeBase).CurrentCursor = Cursors.No;
            }
        }
        public void setCursor(Cursor cursor, string objType)
        {
            this.Cursor = cursor;
            foreach (Control ctr in this.Controls)
            {
                if (ctr.GetType().Name == objType)
                {
                    (ctr as ShapeBase).CurrentCursor = cursor;
                }
            }
        }
        private void CreateUnaryRelationship()
        {
            RelationshipShape rel = new RelationshipShape();
            rel.type = DrawingShapeState;

            rel.Location = new Point(FirstEntity.Location.X + FirstEntity.Width + ThongSo.ShapeW, FirstEntity.Location.Y);

            CardinalityShape car1 = new CardinalityShape(FirstEntity);
            CardinalityShape car2 = new CardinalityShape(FirstEntity);

            //Bắt buộc phải add vô rel trước để tính toán vị trí (nhung rel phải có location rồi)
            rel.addCardinality(car1);
            rel.addCardinality(car2);

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                car1.setValue(1, 1);
                car2.setValue(0, -1);
            }
            else
            {
                car1.setValue(0, -1);
                car2.setValue(0, -1);
            }

            this.Controls.Add(rel);
        }
        private void CreateBinaryRelationship()
        {
            if (FirstEntity == SecondEntity)
            {
                return;
            }

            RelationshipShape rel = new RelationshipShape();
            rel.type = DrawingShapeState;

            //Gán location của relationship mới tạo là điểm giữa của 2 Entity

            rel.CenterPoint = new Point(Math.Abs(FirstEntity.CenterPoint.X + SecondEntity.CenterPoint.X) / 2, Math.Abs(FirstEntity.CenterPoint.Y + SecondEntity.CenterPoint.Y) / 2);

            CardinalityShape car1 = new CardinalityShape(FirstEntity);
            CardinalityShape car2 = new CardinalityShape(SecondEntity);

            //Bắt buộc phải add vô rel trước để tính toán vị trí (nhung rel phải có location rồi)
            rel.addCardinality(car1);
            rel.addCardinality(car2);

            if (FirstEntity.type != SecondEntity.type)
                rel.type = RelationshipType.Identifier;

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                if (rel.type == RelationshipType.Identifier)
                {
                    if (FirstEntity.type == EntityType.Strong)
                    {
                        car1.setValue(1, 1);
                        car2.setValue(0, -1);
                    }
                    else
                    {
                        car1.setValue(0, -1);
                        car2.setValue(1, 1);
                    }
                }
                else
                {
                    car1.setValue(1, 1);
                    car2.setValue(0, -1);
                }
            }
            else
            {
                car1.setValue(0, -1);
                car2.setValue(0, -1);
            }

            this.Controls.Add(rel);
        }
        private void CreateTernaryRelationship()
        {
            if (FirstEntity == SecondEntity || FirstEntity == ThirdEntity || SecondEntity == ThirdEntity)
            {
                return;
            }

            RelationshipShape rel = new RelationshipShape();
            rel.type = DrawingShapeState;

            //Gán location của relationship mới tạo là điểm giữa của 3 Entity
            int MinX = Math.Min(FirstEntity.CenterPoint.X, Math.Min(SecondEntity.CenterPoint.X, ThirdEntity.CenterPoint.X));
            int MinY = Math.Min(FirstEntity.CenterPoint.Y, Math.Min(SecondEntity.CenterPoint.Y, ThirdEntity.CenterPoint.Y));

            int MaxX = Math.Max(FirstEntity.CenterPoint.X, Math.Max(SecondEntity.CenterPoint.X, ThirdEntity.CenterPoint.X));
            int MaxY = Math.Max(FirstEntity.CenterPoint.Y, Math.Max(SecondEntity.CenterPoint.Y, ThirdEntity.CenterPoint.Y));

            rel.CenterPoint = new Point(MinX + (MaxX - MinX) / 2, MinY + (MaxY - MinY) / 2);

            CardinalityShape car1 = new CardinalityShape(FirstEntity);
            CardinalityShape car2 = new CardinalityShape(SecondEntity);
            CardinalityShape car3 = new CardinalityShape(ThirdEntity);

            //Bắt buộc phải add vô rel trước để tính toán vị trí (nhung rel phải có location rồi)
            rel.addCardinality(car1);
            rel.addCardinality(car2);
            rel.addCardinality(car3);

            if (rel.type != RelationshipType.AssociativeEntity)
            {
                car1.setValue(1, 1);
                car2.setValue(1, 1); 
                car3.setValue(1, 1);
            }
            else
            {
                car1.setValue(0, -1);
                car2.setValue(0, -1);
                car3.setValue(0, -1);
            }

            this.Controls.Add(rel);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (allowDrawing && e.Control.GetType().BaseType.Name == "ShapeBase")
            {
                ShapeBase shape = (ShapeBase)e.Control;

                shape.MouseDown += new MouseEventHandler(shapeMouseDown);
                shape.MouseMove += new MouseEventHandler(shapeMouseMove);
                shape.MouseUp += new MouseEventHandler(shapeMouseUp);
                shape.MouseDoubleClick += new MouseEventHandler(shapeDoubleClick);
                shape.KeyDown += new KeyEventHandler(shape_KeyDown);

                if (isDrawing)
                    saveUndoList();
            }
        }
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            //saveUndoList();
        }
    }
}
