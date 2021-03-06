﻿using System;
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
    public partial class EditTernaryRelationship : DevExpress.XtraEditors.XtraForm
    {
        public EntityShape SelectedEntity1 = null;
        public EntityShape SelectedEntity2 = null;
        public EntityShape SelectedEntity3 = null;
        RelationshipShape Relationship;

        public CardinalityShape cardi1;
        public CardinalityShape cardi2;
        public CardinalityShape cardi3;

        public EditTernaryRelationship(RelationshipShape relationship)
        {
            InitializeComponent();

            Relationship = new RelationshipShape(relationship.sName, new Point(panelDoubleBuffered1.Width / 2, panelDoubleBuffered1.Height / 2), relationship.type);
            SelectedEntity1 = new EntityShape(relationship.cardinalities[0].Entity.sName, new Point(panelDoubleBuffered1.Width / 2, ThongSo.ShapeH / 2), relationship.cardinalities[0].Entity.type);
            SelectedEntity2 = new EntityShape(relationship.cardinalities[1].Entity.sName, new Point(panelDoubleBuffered1.Width / 2 - ThongSo.ShapeW * 3/2, panelDoubleBuffered1.Height - ThongSo.ShapeH/2), relationship.cardinalities[1].Entity.type);
            SelectedEntity3 = new EntityShape(relationship.cardinalities[2].Entity.sName, new Point(panelDoubleBuffered1.Width / 2 + ThongSo.ShapeW * 3/2, panelDoubleBuffered1.Height - ThongSo.ShapeH/2), relationship.cardinalities[2].Entity.type);

            cardi1 = Relationship.CreateCardinality(SelectedEntity1, relationship.cardinalities[0].MinCardinality, relationship.cardinalities[0].MaxCardinality);
            cardi2 = Relationship.CreateCardinality(SelectedEntity2, relationship.cardinalities[1].MinCardinality, relationship.cardinalities[1].MaxCardinality);
            cardi3 = Relationship.CreateCardinality(SelectedEntity3, relationship.cardinalities[2].MinCardinality, relationship.cardinalities[2].MaxCardinality);
            
            panelDoubleBuffered1.Controls.Add(Relationship);
            panelDoubleBuffered1.Controls.Add(SelectedEntity1);
            panelDoubleBuffered1.Controls.Add(SelectedEntity2);
            panelDoubleBuffered1.Controls.Add(SelectedEntity3);
            panelDoubleBuffered1.Refresh();

            int min1, max1, min2, max2, min3, max3;
            min1 = Relationship.cardinalities[0].MinCardinality;
            max1 = Relationship.cardinalities[0].MaxCardinality;

            min2 = Relationship.cardinalities[1].MinCardinality;
            max2 = Relationship.cardinalities[1].MaxCardinality;

            min3 = Relationship.cardinalities[2].MinCardinality;
            max3 = Relationship.cardinalities[2].MaxCardinality;

            setCardinality(min1, max1, imageComboBoxEdit1);
            setCardinality(min2, max2, imageComboBoxEdit2);
            setCardinality(min3, max3, imageComboBoxEdit3);

            showText();
        }

        private void setCardinality(int min, int max, ComboBoxEdit imageComboBox)
        {
            if (min == 0 && max == 1)
                imageComboBox.SelectedIndex = 0;
            if (min == 1 && max == 1)
                imageComboBox.SelectedIndex = 1;
            if (min == 0 && max == -1)
                imageComboBox.SelectedIndex = 2;
            if (min == 1 && max == -1)
                imageComboBox.SelectedIndex = 3;
        }
        void showText()
        {
            labEntity1.Text = Relationship.cardinalities[0].Entity.sName;
            labEntity2.Text = Relationship.cardinalities[1].Entity.sName;
            labEntity3.Text = Relationship.cardinalities[2].Entity.sName;
        }

        private void imageComboBoxEdit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderPreview(imageComboBoxEdit2, cardi2);
        }
        private void imageComboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderPreview(imageComboBoxEdit1, cardi1);
        }
        private void imageComboBoxEdit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderPreview(imageComboBoxEdit3, cardi3);
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