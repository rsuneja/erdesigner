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
    public partial class AddCardinality : Form
    {
        public EntityShape SelectedEntity1 = null;
        public int Min1, Max1;
        public EntityShape SelectedEntity2 = null;
        public int Min2, Max2;
        Control.ControlCollection Control;
        RelationshipShape Relationship;
        CardinalityShape cardi1 = new CardinalityShape();
        CardinalityShape cardi2 = new CardinalityShape();

        public AddCardinality(Control.ControlCollection control, RelationshipShape relationship)
        {
            InitializeComponent();

            Control = control;
            Relationship = relationship;
            
            drawPreview();

            int min1, max1, min2, max2;
            min1 = relationship.cardinalities[0].MinCardinality;
            max1 = relationship.cardinalities[0].MaxCardinality;

            min2 = relationship.cardinalities[1].MinCardinality;
            max2 = relationship.cardinalities[1].MaxCardinality;

            if (min1 == 0 && max1 == 1)
                imageComboBoxEdit1.SelectedIndex = 0;
            if (min1 == 1 && max1 == 1)
                imageComboBoxEdit1.SelectedIndex = 1;
            if (min1 == 0 && max1 == -1)
                imageComboBoxEdit1.SelectedIndex = 2;
            if (min1 == 1 && max1 == -1)
                imageComboBoxEdit1.SelectedIndex = 3;

            if (min2 == 0 && max2 == 1)
                imageComboBoxEdit2.SelectedIndex = 0;
            if (min2 == 1 && max2 == 1)
                imageComboBoxEdit2.SelectedIndex = 1;
            if (min2 == 0 && max2 == -1)
                imageComboBoxEdit2.SelectedIndex = 2;
            if (min2 == 1 && max2 == -1)
                imageComboBoxEdit2.SelectedIndex = 3;


            showText();
        }

        private void drawPreview()
        {
            RelationshipShape rel = new RelationshipShape();
            rel.sName = Relationship.sName;
            rel.Location = new Point(160, 15);
            rel.type = Relationship.type;

            EntityShape en1 = new EntityShape();
            en1.sName = Relationship.cardinalities[0].entity.sName;
            en1.type = Relationship.cardinalities[0].entity.type;
            en1.Location = new Point(20, 15);

            EntityShape en2 = new EntityShape();
            en2.sName = Relationship.cardinalities[1].entity.sName;
            en2.type = Relationship.cardinalities[1].entity.type;
            en2.Location = new Point(300, 15);

            
            cardi1.entity = en1;
            cardi1.MinCardinality = Relationship.cardinalities[0].MinCardinality;
            cardi1.MaxCardinality = Relationship.cardinalities[0].MaxCardinality;

            
            cardi2.entity = en2;
            cardi2.MinCardinality = Relationship.cardinalities[1].MinCardinality;
            cardi2.MaxCardinality = Relationship.cardinalities[1].MaxCardinality;

            rel.addCardinality(cardi1);
            rel.addCardinality(cardi2);

            en1.insertCardinality(1, 0, cardi1);
            en2.insertCardinality(3, 0, cardi2);
            
            panelDoubleBuffered1.Controls.Add(rel);
            panelDoubleBuffered1.Controls.Add(en1);
            panelDoubleBuffered1.Controls.Add(en2);
            panelDoubleBuffered1.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SelectedEntity1 = Relationship.cardinalities[0].entity;
            SelectedEntity2 = Relationship.cardinalities[1].entity;

            switch (imageComboBoxEdit1.EditValue.ToString())
            {
                case "0, 1": Min1 = 0; Max1 = 1;
                    break;
                case "1, 1": Min1 = 1; Max1 = 1;
                    break;
                case "0, n": Min1 = 0; Max1 = -1;
                    break;
                case "1, n": Min1 = 1; Max1 = -1;
                    break;
            }
            switch (imageComboBoxEdit2.EditValue.ToString())
            {
                case "0, 1": Min2 = 0; Max2 = 1;
                    break;
                case "1, 1": Min2 = 1; Max2 = 1;
                    break;
                case "0, n": Min2 = 0; Max2 = -1;
                    break;
                case "1, n": Min2 = 1; Max2 = -1;
                    break;
            }


        }

        void showText()
        {
            labEntity1.Text = Relationship.cardinalities[0].entity.sName;
            labEntity11.Text = Relationship.cardinalities[0].entity.sName;

            labEntity2.Text = Relationship.cardinalities[1].entity.sName;
            labEntity22.Text = Relationship.cardinalities[1].entity.sName;

            labRel.Text = Relationship.sName;
            labRel2.Text = Relationship.sName;
        }

        private void AddCardinality_Load(object sender, EventArgs e)
        {

        }

        private void imageComboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (imageComboBoxEdit2.EditValue.ToString())
            {
                case "0, 1": cardi2.MinCardinality = 0; cardi2.MaxCardinality = 1;
                    break;
                case "1, 1": cardi2.MinCardinality = 1; cardi2.MaxCardinality = 1;
                    break;
                case "0, n": cardi2.MinCardinality = 0; cardi2.MaxCardinality = -1;
                    break;
                case "1, n": cardi2.MinCardinality = 1; cardi2.MaxCardinality = -1;
                    break;
            }

            panelDoubleBuffered1.Refresh();
        }

        private void imageComboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (imageComboBoxEdit1.EditValue.ToString())
            {
                case "0, 1": cardi1.MinCardinality = 0; cardi1.MaxCardinality = 1;
                    break;
                case "1, 1": cardi1.MinCardinality = 1; cardi1.MaxCardinality = 1;
                    break;
                case "0, n": cardi1.MinCardinality = 0; cardi1.MaxCardinality = -1;
                    break;
                case "1, n": cardi1.MinCardinality = 1; cardi1.MaxCardinality = -1;
                    break;
            }

            panelDoubleBuffered1.Refresh();
        }
    }
}