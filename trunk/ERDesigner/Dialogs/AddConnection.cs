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
            cboDatasource.SelectedIndex = (int)ThongSo.DB_Mode;
            txtServer.Text = ThongSo.DB_Server;
            txtUserName.Text = ThongSo.DB_UserName;
            txtPassword.Text = ThongSo.DB_Password;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ThongSo.DB_Mode = (DBMS)cboDatasource.SelectedIndex;
            ThongSo.DB_Server = txtServer.Text;
            ThongSo.DB_UserName = txtUserName.Text;
            ThongSo.DB_Password = txtPassword.Text;

            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            ThongSo.DB_Mode = (DBMS)cboDatasource.SelectedIndex;
            ThongSo.DB_Server = txtServer.Text;
            ThongSo.DB_UserName = txtUserName.Text;
            ThongSo.DB_Password = txtPassword.Text;

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
                txtServer.Enabled = false;
                btnTest.Enabled = false;
                txtUserName.Enabled = false;
                txtPassword.Enabled = false;
            }
            else
            {
                btnOK.Enabled = false;
                txtServer.Enabled = true;
                btnTest.Enabled = true;
                txtUserName.Enabled = true;
                txtPassword.Enabled = true;
            }
        }
    }
}