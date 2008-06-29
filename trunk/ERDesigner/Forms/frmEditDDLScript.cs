using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ERDesigner
{
    public partial class frmEditDDLScript : Form
    {
        private string urlFile = "";
        public frmEditDDLScript()
        {
            InitializeComponent();
        }
        public frmEditDDLScript(string url)
        {           
            urlFile = url;
            InitializeComponent();
        }

        private void frmEditDDLScript_Load(object sender, EventArgs e)
        {
            txtUrl.Text = urlFile;
            txtUrl.SelectAll();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (urlFile != "")
            {
                ProcessStartInfo info = new ProcessStartInfo("NotePad.exe", urlFile);
                Process.Start(info);
            }
            //Process[] processes = Process.GetProcessesByName("notepad");
            //foreach (Process p in processes)
            //{
            //    IntPtr pFoundWindow = p.MainWindowHandle;
                
            //}
        }
    }
}