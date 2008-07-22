using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERDesigner
{
    public partial class New_Project : DevExpress.XtraEditors.XtraForm
    {
        public New_Project()
        {
            InitializeComponent();
        }

        private void New_Project_Load(object sender, EventArgs e)
        {
            txtProjectName.Text = "DatabaseDesign1";
            browseFileControl1.txtFileName.Text = Application.StartupPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (check())
                this.Close();
        }

        private bool check()
        {
            string error = "";
            if (txtProjectName.Text == "")
                error += "Project Name";
            if (browseFileControl1.txtFileName.Text == "")
                error += " and Project Location";

            if (error != "")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Please specify " + error, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
                return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //không có Project Name, Main form sẽ không tạo project
            txtProjectName.Text = "";
        }
    }
}