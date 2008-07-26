using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ERDesigner.Shape;
using System.Drawing;
using ERDesigner;
using System.Drawing.Drawing2D;
using DevExpress.XtraBars;

namespace ERDesigner
{
    public class PanelDoubleBuffered : Panel
    {
        //variable for Subtype
        public string SubTypeCompleteness = SubTypeConnectorType.TotalSpecialization;
        public string SubTypeDisjointness = SubTypeConnectorType.DisjointConstraint;
        //End Declare

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
        public ShapeBase AffectingShape = null;

        Point First_Mouse_Pos = new Point(-1, -1);
        Point Last_Mouse_Pos = new Point(-1, -1);

        private bool isMoving = false;
        private bool isResizing = false;

        private bool isSelecting = false;

        public bool isDrawingAtt = false;
        public bool isDrawEntity = false;
        public bool isDrawRelationship = false;
        public bool isDrawSubType = false;

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
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Relationship couldn't have an Identifier Attribute");
                                    currentShape = null;
                                }
                                else
                                    relshape.addAttribute((AttributeShape)currentShape);
                            }
                        }
                    }

                }
                if ((isDrawEntity || isDrawSubType || (isDrawingAtt && AffectingShape != null)) && currentShape != null)
                {
                    Point mouse = this.PointToClient(Control.MousePosition);
                    currentShape.CenterPoint = mouse;
                    this.Controls.Add(currentShape);
                    shapeDoubleClick(currentShape, e);
                }
                else
                {
                    //đang vẽ sub type
                    if (isDrawSubType && AffectingShape != null)
                    {
                        if (AffectingShape is EntityShape)
                        {
                            CreateSubTypeConnector((EntityShape)AffectingShape);
                        }

                    }
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
                ContextMenuStrip ctmn = getContexMenuForControl(currentCtrl);
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
            ((ShapeBase)sender).doDoubleClick(this, (ShapeBase)sender);
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
            String type = ((ToolStripMenuItem)sender).Text;
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
                case "Associative Entity": 
                    ((RelationshipShape)AffectingShape).type = RelationshipType.AssociativeEntity;
                    break;
                case "Total Specialization":
                    ((SubTypeConnector)AffectingShape).completeness = SubTypeConnectorType.TotalSpecialization;
                    this.Invalidate();
                    break;
                case "Partial Specialization":
                    ((SubTypeConnector)AffectingShape).completeness = SubTypeConnectorType.PartialSpecialization;
                    this.Invalidate();
                    break;
                case "Disjoint Constraint":
                    ((SubTypeConnector)AffectingShape).disjointness = SubTypeConnectorType.DisjointConstraint;
                    break;
                case "Overlap Constraint":
                    ((SubTypeConnector)AffectingShape).disjointness = SubTypeConnectorType.OverlapConstraint;
                    break;

            }
            AffectingShape.Invalidate();
        }
        void editRel_Click(object sender, EventArgs e)
        {
            //hiển thị Dialog
            RelationshipShape relationship = null;
            if (AffectingShape != null && AffectingShape is RelationshipShape)
            {
                relationship = (RelationshipShape)AffectingShape;
                if (relationship.cardinalities.Count < 3)
                {
                    AddCardinality addCardiDialog = new AddCardinality(relationship);

                    if (addCardiDialog.ShowDialog() == DialogResult.OK)
                    {
                        relationship.cardinalities[0].setValue(addCardiDialog.cardi1.MinCardinality, addCardiDialog.cardi1.MaxCardinality);
                        relationship.cardinalities[1].setValue(addCardiDialog.cardi2.MinCardinality, addCardiDialog.cardi2.MaxCardinality);

                        if ((relationship.cardinalities[0].MaxCardinality != -1 || relationship.cardinalities[1].MaxCardinality != -1) && relationship.type == RelationshipType.AssociativeEntity)
                            if (relationship.cardinalities[0].Entity.type != relationship.cardinalities[1].Entity.type)
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
        void editSub_Click(object sender, EventArgs e)
        {
            //hiển thị Dialog
            if (AffectingShape != null && AffectingShape is SubTypeConnector)
            {
                SubTypeConnector sub = (SubTypeConnector)AffectingShape;
                EditSubtypeConnector editSub = new EditSubtypeConnector(sub);
                if (editSub.ShowDialog() == DialogResult.OK)
                {
                    sub.discriminators = editSub.Discriminators;
                    sub.AttributeDiscriminator = editSub.attDis;
                    this.Invalidate();
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
        public void CancelDrawing()
        {
            isDrawing = false;
            isDrawEntity = false;
            isDrawingAtt = false;
            isDrawRelationship = false;
            isDrawSubType = false;

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
        public void EndEditDataType()
        {
            if (AffectingShape != null && AffectingShape.GetType().Name == "AttributeShape" && isNaming)
            {
                ((ShapeBase)AffectingShape).endEditName();
            }
        }

        //Draw support
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

        //Draw all relation line
        private void DrawConnectiveLines(Graphics g)
        {
            //Tính lại tất cả vị trí
            UpdateAllPosition();

            //Băt đầu vẽ
            foreach (Control s in this.Controls)
            {
                if (s is INotation)
                {
                    (s as INotation).DrawConnectiveLines(g);
                }
            }
        }
        void UpdateAllPosition()
        {
            foreach (Control s in this.Controls)
            {
                if (s is EntityShape)
                {
                    EntityShape entity = s as EntityShape;
                    entity.UpdateCardinalityPosition();
                }
                else if (s is RelationshipShape && (s as RelationshipShape).type == RelationshipType.AssociativeEntity)
                {
                    RelationshipShape rel = s as RelationshipShape;
                    rel.UpdateCardinalityPosition();
                }
            }
        }

        //Delete and Undo
        public bool deleteAffectingShape()
        {
            if (AffectingShape != null && !isDrawing && !isNaming && !isMoving && !isResizing && !isSelecting)
            {
                if (AffectingShape.GetType().Name == "EntityShape")
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure want to delete " + AffectingShape.sName + " Entity ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
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

        //Auto Layout
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

                    foreach (CardinalityShape cardi2 in entity.cardinalities[i][j].Relationship.cardinalities)
                    {
                        if (cardi2 != entity.cardinalities[i][j])
                            cardi = cardi2;
                    }

                    EntityShape entity2 = cardi.Entity;
                    entity2.Location = new Point(stepX, 150 * (line + 1));
                    stepX += this.Width / (degree + 1);

                    Point centerEntity1 = new Point(entity.Location.X + entity.Width / 2, entity.Location.Y + entity.Height / 2);
                    Point centerEntity2 = new Point(entity2.Location.X + entity2.Width / 2, entity2.Location.Y + entity2.Height / 2);

                    Point center = new Point(Math.Abs(centerEntity1.X + centerEntity2.X) / 2, Math.Abs(centerEntity1.Y + centerEntity2.Y) / 2);

                    cardi.Relationship.Location = new Point(center.X - cardi.Relationship.Width / 2, center.Y - cardi.Relationship.Height / 2);

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

        //Get, Load Metadata
        public MetaData getMetaData()
        {
            MetaData ERD = new MetaData();
            foreach (Control c in this.Controls)
            {
                if (c is EntityShape || c is RelationshipShape || c is SubTypeConnector)
                {
                    ShapeBase s = (ShapeBase)c;
                    if (s is EntityShape)
                    {
                        EntityShape en = (EntityShape)s;
                        EntityData entity = (EntityData)en.getMetaData();

                        foreach (AttributeShape att in en.attributes)
                        {
                            AttributeData attribute = (AttributeData)att.getMetaData();
                            if (att.isComposite)
                            {
                                attribute.isComposite = true;
                                foreach (AttributeShape attchild in att.attributeChilds)
                                {
                                    AttributeData attributechild = (AttributeData)attchild.getMetaData();

                                    attribute.AttributeChilds.Add(attributechild);
                                }
                            }
                            entity.Attributes.Add(attribute);
                        }

                        ERD.Entities.Add(entity);
                    }
                    if (s is RelationshipShape)
                    {
                        RelationshipShape rel = (RelationshipShape)s;
                        RelationshipData relationship = (RelationshipData)rel.getMetaData();

                        foreach (AttributeShape att in rel.attributes)
                        {
                            AttributeData attribute = (AttributeData)att.getMetaData();
                            if (att.isComposite)
                            {
                                attribute.isComposite = true;
                                foreach (AttributeShape attchild in att.attributeChilds)
                                {
                                    AttributeData attributechild = (AttributeData)attchild.getMetaData();

                                    attribute.AttributeChilds.Add(attributechild);
                                }
                            }
                            relationship.Attributes.Add(attribute);
                        }

                        foreach (CardinalityShape cardi in rel.cardinalities)
                        {
                            CardinalityData cardinality = new CardinalityData(cardi.Entity.sName, cardi.MinCardinality, cardi.MaxCardinality);

                            relationship.Cardinalities.Add(cardinality);
                        }
                        ERD.Relationships.Add(relationship);
                    }
                    if (s is SubTypeConnector)
                    {
                        SubTypeConnectorData subtypeconnectordata = (SubTypeConnectorData)((SubTypeConnector)s).getMetaData();
                        ERD.SubTypeConnectors.Add(subtypeconnectordata);
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
                EntityShape entity = (EntityShape)en.createNotation();
                this.Controls.Add(entity);

                foreach (AttributeData att in en.Attributes)
                {
                    AttributeShape attribute = (AttributeShape)att.createNotation();

                    if (att.isComposite)
                    {
                        foreach (AttributeData attChild in att.AttributeChilds)
                        {
                            AttributeShape attributeChild = (AttributeShape)attChild.createNotation();
                            attribute.addAttribute(attributeChild);
                            this.Controls.Add(attributeChild);
                        }
                    }
                    entity.addAttribute(attribute);
                    this.Controls.Add(attribute);
                }
            }

            //Phù, mệt wá, ráng thôi, Vẽ tiếp relationship, attribute và cardinality của nó
            //Xài interface, hết mệt
            foreach (RelationshipData rel in ERD.Relationships)
            {
                RelationshipShape relationship = (RelationshipShape)rel.createNotation();
                this.Controls.Add(relationship);

                foreach (AttributeData att in rel.Attributes)
                {
                    AttributeShape attribute = (AttributeShape)att.createNotation();

                    if (att.isComposite)
                    {
                        foreach (AttributeData attChild in att.AttributeChilds)
                        {
                            AttributeShape attributeChild = (AttributeShape)attChild.createNotation();
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
                            relationship.CreateCardinality((EntityShape)s, cardi.MinCardinality, cardi.MaxCardinality);
                            break;
                        }
                    }
                }
            }
            foreach (SubTypeConnectorData subtypeconnectordata in ERD.SubTypeConnectors)
            {
                SubTypeConnector subtypeconnector = (SubTypeConnector)subtypeconnectordata.createNotation();
                foreach (ShapeBase s in this.Controls)
                {
                    if (s is EntityShape && s.sName == subtypeconnectordata.SuperType)
                    {
                        subtypeconnector.supertype = (EntityShape)s;
                        ((EntityShape)s).SubtypeConnector = subtypeconnector;
                    }
                }
                foreach (string subtypename in subtypeconnectordata.SubTypes)
                {
                    foreach (ShapeBase s in this.Controls)
                    {
                        if (s is EntityShape && s.sName == subtypename)
                        {
                            subtypeconnector.addSubType((EntityShape)s);
                        }
                    }
                }
                this.Controls.Add(subtypeconnector);

            }
            this.Refresh();
        }

        //Verify Model
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
                        if (entity.type == EntityType.Strong && !entity.isSubType)
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
                                    if (cardi.Relationship.type != RelationshipType.Identifier)
                                    {
                                        errorList.Add("Relationship '" + cardi.Relationship.sName + "' must be an Identify Relationship");
                                    }
                                }
                        }
                        break;

                    case "RelationshipShape": RelationshipShape rel = (RelationshipShape)c;
                        //mối kết hợp xác định có hợp lý chưa
                        if (rel.type == RelationshipType.Identifier)
                        {
                            EntityShape entity1 = rel.cardinalities[0].Entity;
                            EntityShape entity2 = rel.cardinalities[1].Entity;

                            if (entity1.type == entity2.type)
                                errorList.Add("Identify Relationship '" + rel.sName + "' must connect weak entity and strong entity");
                            else
                            {
                                if (entity1.type == EntityType.Strong && rel.cardinalities[0].MaxCardinality != 1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Strong Entity " + entity1.sName + " must be one");
                                if (entity2.type == EntityType.Strong && rel.cardinalities[1].MaxCardinality != 1)
                                    errorList.Add("Max Cardinality of '" + rel.sName + "' with Strong Entity " + entity2.sName + " must be one");

                                //if (entity1.type == EntityType.Weak && rel.cardinalities[0].MaxCardinality != -1)
                                //    errorList.Add("Max Cardinality of '" + rel.sName + "' with Weak Entity " + entity1.sName + " must be many");
                                //if (entity2.type == EntityType.Weak && rel.cardinalities[1].MaxCardinality != -1)
                                //    errorList.Add("Max Cardinality of '" + rel.sName + "' with Weak Entity " + entity2.sName + " must be many");
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

                        //mối kết hợp thường không được có key attribute
                        if (rel.type == RelationshipType.Normal)
                        {
                            bool haveKey = false;
                            foreach (AttributeShape att in rel.attributes)
                            {
                                if (att.type == AttributeType.Key)
                                {
                                    haveKey = true;
                                    break;
                                }
                            }
                            if (haveKey)
                                errorList.Add("Relationship '" + rel.sName + "' must not have any key attributes");
                        }


                        //supertype không được nối với subtype
                        foreach (CardinalityShape cardi in rel.cardinalities)
                        {
                            EntityShape supertype = cardi.Entity;
                            if (supertype.SubtypeConnector != null)
                            {
                                foreach (CardinalityShape cardi2 in rel.cardinalities)
                                {
                                    EntityShape subtype = cardi2.Entity;
                                    if (subtype != supertype)
                                    {
                                        if (supertype.SubtypeConnector.subtypes.Contains(subtype))
                                        {
                                            errorList.Add("Supertype '" + supertype.sName + "' and subtype '" + subtype.sName + "' must not have any other relationships except supertype/subtype relationship");
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    case "AttributeShape": AttributeShape attribute = (AttributeShape)c;
                        //Kiểm tra thuộc tính composite có ít nhất 2 con
                        if (attribute.attributeChilds.Count == 1)
                            errorList.Add("Composite Attribute '" + attribute.sName + "' must have more than one child");
                        break;
                }
            }

            //Kiểm tra đồ thị liên thông
            if (ThongSo.checkIsolation)
                errorList.AddRange(CheckConnectivity());

            return errorList;
        }
        public List<string> CheckConnectivity()
        {
            List<ShapeBase> listShape = new List<ShapeBase>();
            Queue<ShapeBase> listChecking = new Queue<ShapeBase>();
            List<string> errorList = new List<string>();

            foreach (ShapeBase shape in this.Controls)
            {
                listShape.Add(shape);
            }

            if (listShape.Count > 0)
            {
                foreach (ShapeBase shape in listShape)
                {
                    if (shape is EntityShape)
                    {
                        listChecking.Enqueue(shape);
                        break;
                    }
                }

                while (listChecking.Count > 0)
                {
                    ShapeBase shape = listChecking.Dequeue();

                    switch (shape.GetType().Name)
                    {
                        case "EntityShape":
                            EntityShape entity = (EntityShape)shape;
                            foreach (AttributeShape att in entity.attributes)
                            {
                                if (att.isComposite)
                                {
                                    foreach (AttributeShape attChild in att.attributeChilds)
                                    {
                                        listShape.Remove((ShapeBase)attChild);
                                    }
                                }
                                listShape.Remove((ShapeBase)att);
                            }
                            for (int i = 0; i < 4; i++)
                                foreach (CardinalityShape cardi in entity.cardinalities[i])
                                {
                                    if (!listChecking.Contains((ShapeBase)cardi.Relationship) && listShape.Contains((ShapeBase)cardi.Relationship))
                                        listChecking.Enqueue((ShapeBase)cardi.Relationship);
                                }

                            if (entity.SubtypeConnector != null)
                            {
                                foreach (EntityShape subtype in entity.SubtypeConnector.subtypes)
                                {
                                    if (!listChecking.Contains((ShapeBase)subtype) && listShape.Contains((ShapeBase)subtype))
                                        listChecking.Enqueue((ShapeBase)subtype);
                                }
                                listShape.Remove((ShapeBase)entity.SubtypeConnector);
                            }

                            if (entity.supertypeconnector != null)
                            {
                                EntityShape supertype = (EntityShape)entity.supertypeconnector.supertype;
                                if (!listChecking.Contains((ShapeBase)supertype) && listShape.Contains((ShapeBase)supertype))
                                    listChecking.Enqueue((ShapeBase)supertype);
                            }

                            listShape.Remove((ShapeBase)entity);
                            break;
                        case "RelationshipShape":
                            RelationshipShape rel = (RelationshipShape)shape;
                            foreach (AttributeShape att in rel.attributes)
                            {
                                if (att.isComposite)
                                {
                                    foreach (AttributeShape attChild in att.attributeChilds)
                                    {
                                        listShape.Remove((ShapeBase)attChild);
                                    }
                                }
                                listShape.Remove((ShapeBase)att);
                            }
                            foreach (CardinalityShape cardi in rel.cardinalities)
                            {
                                if (!listChecking.Contains((ShapeBase)cardi.Entity) && listShape.Contains((ShapeBase)cardi.Entity))
                                    listChecking.Enqueue((ShapeBase)cardi.Entity);
                            }
                            listShape.Remove((ShapeBase)rel);
                            break;
                        case "AttributeShape":
                            break;
                        case "SubTypeConnector":
                            break;
                    }
                }

                List<ShapeBase> tempList = new List<ShapeBase>();

                //Nếu isolate nhiều hơn không isolate -> ngược lại
                if (listShape.Count > this.Controls.Count - listShape.Count)
                {
                    foreach (ShapeBase shape in this.Controls)
                    {
                        if (!listShape.Contains(shape))
                            tempList.Add(shape);
                    }

                    foreach (ShapeBase shape in tempList)
                    {
                        errorList.Add("'" + shape.sName + "' is isolated");
                    }
                }
                else
                {
                    foreach (ShapeBase shape in listShape)
                    {
                        errorList.Add("'" + shape.sName + "' is isolated");
                    }
                }
            }

            return errorList;
        }

        //Generate Context Menu
        private ContextMenuStrip getContexMenuForControl(ShapeBase control)
        {
            ContextMenuStrip ctmn = new ContextMenuStrip();

            ToolStripMenuItem del = new ToolStripMenuItem("Delete");
            del.Click += new EventHandler(del_Click);

            if (control is EntityShape)
            {
                ToolStripMenuItem StrongEntity = new ToolStripMenuItem("Strong Entity Type");
                StrongEntity.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem WeakEntity = new ToolStripMenuItem("Weak Entity Type");
                WeakEntity.Click += new EventHandler(Change_Type_Click);

                ctmn.Items.Add(StrongEntity);
                ctmn.Items.Add(WeakEntity);
                ctmn.Items.Add(new ToolStripSeparator());

                ctmn.Items.Add(del);
            }

            if (control is RelationshipShape)
            {
                ToolStripMenuItem NormalRelation = new ToolStripMenuItem("Normal Relationship");
                NormalRelation.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem IdRelation = new ToolStripMenuItem("Identifier Relationship");
                IdRelation.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem AssoRelation = new ToolStripMenuItem("Associative Entity");
                AssoRelation.Click += new EventHandler(Change_Type_Click);

                ctmn.Items.Add(NormalRelation);
                ctmn.Items.Add(IdRelation);
                ctmn.Items.Add(AssoRelation);

                ctmn.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem editRel = new ToolStripMenuItem("Properties");
                editRel.Click += new EventHandler(editRel_Click);
                
                ctmn.Items.Add(del);
                ctmn.Items.Add(editRel);
            }

            if (control is AttributeShape && ((AttributeShape)control).isComposite)
            {
                ToolStripMenuItem KeyAtt = new ToolStripMenuItem("Key Attribute");
                KeyAtt.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem SimpleAtt = new ToolStripMenuItem("Simple Attribute");
                SimpleAtt.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem MultiAtt = new ToolStripMenuItem("Multi-Valued Attribute");
                MultiAtt.Click += new EventHandler(Change_Type_Click);

                ctmn.Items.Add(SimpleAtt);
                ctmn.Items.Add(KeyAtt);
                ctmn.Items.Add(MultiAtt);

                if (((AttributeShape)control).attributeChilds.Count == 0)
                {
                    ToolStripMenuItem DerivedAtt = new ToolStripMenuItem("Derived Attribute");
                    DerivedAtt.Click += new EventHandler(Change_Type_Click);
                    ctmn.Items.Add(DerivedAtt);
                }
                ctmn.Items.Add(new ToolStripSeparator());
                ctmn.Items.Add(del);
            }

            if (control is SubTypeConnector)
            {
                ToolStripMenuItem Total = new ToolStripMenuItem("Total Specialization");
                Total.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem Partial = new ToolStripMenuItem("Partial Specialization");
                Partial.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem Disjoint = new ToolStripMenuItem("Disjoint Constraint");
                Disjoint.Click += new EventHandler(Change_Type_Click);

                ToolStripMenuItem Overlap = new ToolStripMenuItem("Overlap Constraint");
                Overlap.Click += new EventHandler(Change_Type_Click);

                ctmn.Items.Add(Total);
                ctmn.Items.Add(Partial);
                ctmn.Items.Add(Disjoint);
                ctmn.Items.Add(Overlap);

                ctmn.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem editSub = new ToolStripMenuItem("Properties");
                editSub.Click += new EventHandler(editSub_Click);

                ctmn.Items.Add(del);
                ctmn.Items.Add(editSub);
            }

            return ctmn;
        }

        //Set currsor
        public void setAllControlToDefaultCursor()
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr is ShapeBase)
                    (ctr as ShapeBase).CurrentCursor = Cursors.Default;
            }
        } //Default
        public void setAllControlToNoCursor()
        {
            foreach (Control ctr in this.Controls)
            {
                if (ctr is ShapeBase)
                    (ctr as ShapeBase).CurrentCursor = Cursors.No;
            }
        } //Banned
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

        //Create Relationship
        private void CreateUnaryRelationship()
        {
            RelationshipShape rel = new RelationshipShape();
            rel.type = DrawingShapeState;

            rel.Location = new Point(FirstEntity.Location.X + FirstEntity.Width + ThongSo.ShapeW, FirstEntity.Location.Y);

            CardinalityShape car1 = rel.CreateCardinality(FirstEntity, 0, 0);
            CardinalityShape car2 = rel.CreateCardinality(FirstEntity, 0, 0);

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

            CardinalityShape car1 = rel.CreateCardinality(FirstEntity, 0, 0);
            CardinalityShape car2 = rel.CreateCardinality(SecondEntity, 0, 0);

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

            CardinalityShape car1 = rel.CreateCardinality(FirstEntity, 0, 0);
            CardinalityShape car2 = rel.CreateCardinality(SecondEntity, 0, 0);
            CardinalityShape car3 = rel.CreateCardinality(ThirdEntity, 0, 0);

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

        //Create SubTypeConnector
        private void CreateSubTypeConnector(EntityShape supertype)
        {
            if (supertype.SubtypeConnector == null)
            {
                EntityShape subtype = new EntityShape();
                subtype.CenterPoint = this.PointToClient(Control.MousePosition);

                Point CenterTwoEntity = new Point(Math.Abs(supertype.CenterPoint.X + subtype.CenterPoint.X) / 2, Math.Abs(supertype.CenterPoint.Y + subtype.CenterPoint.Y) / 2);

                SubTypeConnector subconnector = new SubTypeConnector(supertype, subtype, CenterTwoEntity, SubTypeCompleteness, SubTypeDisjointness);
                supertype.SubtypeConnector = subconnector;

                this.Controls.Add(subconnector);
                this.Controls.Add(subtype);
            }
            else
            {
                if (supertype.SubtypeConnector.completeness == SubTypeCompleteness && supertype.SubtypeConnector.disjointness == SubTypeDisjointness)
                {
                    EntityShape subtype = new EntityShape();
                    subtype.CenterPoint = this.PointToClient(Control.MousePosition);

                    supertype.SubtypeConnector.addSubType((EntityShape)subtype);

                    this.Controls.Add(subtype);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("The constraint of the Sub Type you want to add must be the same to existed constraint in this Super Type", "Warning");
                }
            }
        }

        //Other Event
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
