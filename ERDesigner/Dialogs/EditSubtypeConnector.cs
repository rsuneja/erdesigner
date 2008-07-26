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
    public partial class EditSubtypeConnector : DevExpress.XtraEditors.XtraForm
    {
        SubTypeConnector connector;
        List<DevExpress.XtraEditors.TextEdit> listTextBox = new List<DevExpress.XtraEditors.TextEdit>();
        public List<string> Discriminators = new List<string>();
        public string attDis = "";

        public EditSubtypeConnector(SubTypeConnector con)
        {
            connector = con;
            InitializeComponent();
        }

        private void EditSubtypeConnector_Load(object sender, EventArgs e)
        {
            pnlControl.Controls.Clear();

            foreach (AttributeShape att in connector.supertype.attributes)
            {
                cboSubtypeDis.Properties.Items.Add(att.sName);
            }
            if (cboSubtypeDis.Properties.Items.Count > 0)
            {
                if (connector.AttributeDiscriminator != "")
                    cboSubtypeDis.SelectedItem = connector.AttributeDiscriminator;
                else
                    cboSubtypeDis.SelectedIndex = 0;
            }

            for (int i = 0; i < connector.subtypes.Count; i++ )
            {
                DevExpress.XtraEditors.LabelControl lbSubtype = new DevExpress.XtraEditors.LabelControl();

                lbSubtype.Appearance.BackColor = System.Drawing.Color.White;
                lbSubtype.Appearance.Options.UseBackColor = true;
                lbSubtype.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
                lbSubtype.Cursor = System.Windows.Forms.Cursors.Default;
                lbSubtype.LineColor = System.Drawing.Color.White;
                lbSubtype.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
                lbSubtype.LineVisible = true;
                lbSubtype.Size = new System.Drawing.Size(107, 19);
                lbSubtype.Text = connector.subtypes[i].sName;

                pnlControl.Controls.Add(lbSubtype);

                DevExpress.XtraEditors.TextEdit txtDiscriminator = new DevExpress.XtraEditors.TextEdit();

                txtDiscriminator.Size = new System.Drawing.Size(140, 20);
                txtDiscriminator.TabIndex = i;
                txtDiscriminator.Text = connector.discriminators[i];

                pnlControl.Controls.Add(txtDiscriminator);
                listTextBox.Add(txtDiscriminator);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listTextBox.Count -1; i++ )
            {
                for (int j = i + 1; j < listTextBox.Count; j++ )
                    if(listTextBox[i].Text == listTextBox[j].Text)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Discriminator is duplicated");
                        listTextBox[i].Focus();
                        return;
                    }
            }

            attDis = cboSubtypeDis.EditValue.ToString();

            foreach (DevExpress.XtraEditors.TextEdit txtDiscriminator in listTextBox)
            {
                Discriminators.Add(txtDiscriminator.Text);
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}