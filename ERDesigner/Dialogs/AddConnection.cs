using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERDesigner
{
    public partial class AddConnection : DevExpress.XtraEditors.XtraForm
    {
        public AddConnection()
        {
            InitializeComponent();
        }

        private void AddConnection_Load(object sender, EventArgs e)
        {
            bfcFileBrowse.txtFileName.ReadOnly = true;

            cboDatasource.SelectedIndex = (int)ThongSo.DB_Mode;
            txtServer.Text = ThongSo.DB_Server;
            txtUserName.Text = ThongSo.DB_UserName;
            txtPassword.Text = ThongSo.DB_Password;

            if (ThongSo.DB_IsNewDatabase)
            {
                radNewDatabase.Checked = true;
            }
            else
            {
                radFile.Checked = true;
                bfcFileBrowse.txtFileName.Text = ThongSo.DB_AccessFile;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThongSo.DB_Mode = (DBMS)cboDatasource.SelectedIndex;
            ThongSo.DB_Server = txtServer.Text;
            ThongSo.DB_UserName = txtUserName.Text;
            ThongSo.DB_Password = txtPassword.Text;

            if (ThongSo.DB_Mode == DBMS.Access)
            {
                if (!ThongSo.DB_IsNewDatabase)
                {
                    if (bfcFileBrowse.txtFileName.Text == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("The File path must be specified", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    ThongSo.DB_AccessFile = bfcFileBrowse.txtFileName.Text;
                }
            }

            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            ThongSo.DB_Mode = (DBMS)cboDatasource.SelectedIndex;
            ThongSo.DB_Server = txtServer.Text;
            ThongSo.DB_UserName = txtUserName.Text;
            ThongSo.DB_Password = txtPassword.Text;
            ThongSo.DB_AccessFile = bfcFileBrowse.txtFileName.Text;

            DBProviderBase database = new DBProviderBase();
            if(database.TestConnection())
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Test Connection succecced");
            }
            else
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Test Connection failed","Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            if (txtUserName.Text.Length > 0)
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }

        private void cboDatasource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DBMS)cboDatasource.SelectedIndex == DBMS.Access)
            {
                btnOK.Enabled = true;
                txtServer.Visible = false;
                pnlAccess.Visible = true;
            }
            else
            {
                txtServer.Visible = true;
                pnlAccess.Visible = false;
            }
        }

        private void radFile_CheckedChanged(object sender, EventArgs e)
        {
            if(radFile.Checked)
            {
                bfcFileBrowse.Enabled = true;
                ThongSo.DB_IsNewDatabase = false;
                btnTest.Enabled = true;
            }
            else
            {
                bfcFileBrowse.Enabled = false;
                ThongSo.DB_IsNewDatabase = true;
                btnTest.Enabled = false;
            }
        }
    }
}