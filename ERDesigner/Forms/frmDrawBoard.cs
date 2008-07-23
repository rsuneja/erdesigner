using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ERDesigner.Shape;

namespace ERDesigner
{
    public partial class frmDrawBoard : Form
    {
        //Lớp đọc ghi XML
        JSerializer js = new JSerializer();
        MetaData ERD = new MetaData();

        public frmDrawBoard()
        {
            InitializeComponent();
            pnlDrawBoard.allowDrawing = true;
            
        }
        public void Clear()
        {
            pnlDrawBoard.Controls.Clear();
            pnlDrawBoard.Invalidate();
        }
        public void ApplySkin(bool isApply)
        {
            if (isApply)
                ShapeBase.skin = true;
            else
                ShapeBase.skin = false;

            pnlDrawBoard.Refresh();
        }

        public void prepairDrawing(string shape)
        {
            pnlDrawBoard.isDrawing = true;
            pnlDrawBoard.DrawingShapeState = shape;
            pnlDrawBoard.setAllControlToNoCursor();

            switch (shape)
            {
                case "Strong Entity":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "");
                    pnlDrawBoard.isDrawEntity = true;
                    break;
                case "Weak Entity":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "");
                    pnlDrawBoard.isDrawEntity = true;
                    break;
                case "Normal Relationship":
                    if(pnlDrawBoard.degreeOfRelationship == 2)
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Cross, "EntityShape");
                    else
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.isDrawRelationship = true;
                    break;
                case "Identify Relationship":
                    if (pnlDrawBoard.degreeOfRelationship == 2)
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Cross, "EntityShape");
                    else
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.isDrawRelationship = true;
                    break;
                case "Associative Entity":
                    if (pnlDrawBoard.degreeOfRelationship == 2)
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Cross, "EntityShape");
                    else
                        pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.isDrawRelationship = true;
                    break;
                case "Simple Attribute":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "RelationshipShape");
                    pnlDrawBoard.isDrawingAtt = true;
                    break;
                case "Key Attribute":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "RelationshipShape");
                    pnlDrawBoard.isDrawingAtt = true;
                    break;
                case "Multivalued Attribute":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "RelationshipShape");
                    pnlDrawBoard.isDrawingAtt = true;
                    break;
                case "Derived Attribute":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "RelationshipShape");
                    pnlDrawBoard.isDrawingAtt = true;
                    break;
                case "Child Attribute":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "AttributeShape");
                    pnlDrawBoard.isDrawingAtt = true;
                    break;
                case "Pointer": pnlDrawBoard.CancelDrawing();
                    break;
                case "Sub Type":
                    pnlDrawBoard.setCursor(System.Windows.Forms.Cursors.Hand, "EntityShape");
                    pnlDrawBoard.isDrawSubType = true;
                    break;
            }
            
        }
        public void changeDegree(int degree)
        {
            pnlDrawBoard.degreeOfRelationship = degree;

            if (pnlDrawBoard.isDrawRelationship)
            {
                if (degree == 2)
                    pnlDrawBoard.Cursor = System.Windows.Forms.Cursors.Cross;
                else
                    pnlDrawBoard.Cursor = System.Windows.Forms.Cursors.Hand;
            }
        }
        public void AutoLayout()
        {
            pnlDrawBoard.AutoLayout();
        }

        
    }
}