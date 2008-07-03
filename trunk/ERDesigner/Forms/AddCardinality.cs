using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ERDesigner.Shape;
using DevExpress.XtraEditors;

namespace ERDesigner
{
    public partial class AddCardinality : Form
    {
        public EntityShape SelectedEntity1 = null;
        public EntityShape SelectedEntity2 = null;
        Control.ControlCollection Control;
        RelationshipShape Relationship;
        public CardinalityShape cardi1;
        public CardinalityShape cardi2;

        public AddCardinality(RelationshipShape relationship)
        {
            InitializeComponent();

            Relationship = new RelationshipShape(relationship.sName, new Point(160, 15), relationship.type);
            SelectedEntity1 = new EntityShape(relationship.cardinalities[0].entity.sName, new Point(20, 15), relationship.cardinalities[0].entity.type);
            SelectedEntity2 = new EntityShape(relationship.cardinalities[1].entity.sName, new Point(300, 15), relationship.cardinalities[1].entity.type);

            cardi1 = new CardinalityShape(SelectedEntity1);
            cardi1.setValue(relationship.cardinalities[0].MinCardinality, relationship.cardinalities[0].MaxCardinality);

            cardi2 = new CardinalityShape(SelectedEntity2);
            cardi2.setValue(relationship.cardinalities[1].MinCardinality, relationship.cardinalities[1].MaxCardinality);

            Relationship.addCardinality(cardi1);
            Relationship.addCardinality(cardi2);

            //SelectedEntity1.insertCardinality(1, 0, cardi1); //0,1,2,3
            //SelectedEntity2.insertCardinality(3, 0, cardi2);

            panelDoubleBuffered1.Controls.Add(Relationship);
            panelDoubleBuffered1.Controls.Add(SelectedEntity1);
            panelDoubleBuffered1.Controls.Add(SelectedEntity2);
            panelDoubleBuffered1.Refresh();

            int min1, max1, min2, max2;
            min1 = Relationship.cardinalities[0].MinCardinality;
            max1 = Relationship.cardinalities[0].MaxCardinality;

            min2 = Relationship.cardinalities[1].MinCardinality;
            max2 = Relationship.cardinalities[1].MaxCardinality;

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

        private void btnOK_Click(object sender, EventArgs e)
        {
            
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
            RenderPreview(imageComboBoxEdit2, cardi2);
        }
        private void imageComboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderPreview(imageComboBoxEdit1, cardi1);
        }
        private void RenderPreview(ComboBoxEdit imageComboBox, CardinalityShape cardi)
        {
            switch (imageComboBox.EditValue.ToString())
            {
                case "0, 1": cardi.setValue(0, 1);
                    break;
                case "1, 1": cardi.setValue(1, 1);
                    break;
                case "0, n": cardi.setValue(0, -1);
                    break;
                case "1, n": cardi.setValue(1, -1);
                    break;
            }

            panelDoubleBuffered1.Refresh();
        }

        
    }
}