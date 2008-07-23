using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using ERDesigner.Classes;

namespace ERDesigner
{
    public enum DBMS
    {
        MS_Server2000 = 1,
        Oracle,
        Access,
        MySql
    }
    public partial class GenerateScriptSQL : Form
    {
        private MetaDataPhysical mdp;
        private MainForm frmMain;
        private string urlFile;
       
        //Constructor
        public GenerateScriptSQL(MainForm f,MetaDataPhysical mdPhysical)
        {                
            InitializeComponent();
            frmMain = f;
            mdp = mdPhysical;           
        }
                       
        //My Method

        public void GenerateScript(string urlFileScript, string dbName)
        {
            GenerateDDL generate = new GenerateDDL(mdp, DBMS.MS_Server2000, txtDBName.Text);
            List<string> listScript = generate.Process(); 
            if (urlFileScript != "")
            {
                FileStream fs = new FileStream(urlFileScript, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(listScript[0]);  //Create DB             
                sw.Write(listScript[1]);  //Create Table
                sw.Write(listScript[2]);  //Create Foreign Key
                sw.Close();
                fs.Close();
            }          
        }

        private void DisabledTextField()
        {
            txtDBName.Enabled = false;
            txtDirectory.Enabled = false;
            txtFileName.Enabled = false;
            btnBrowser.Enabled = false;
        }

        private void EnabledTextField()
        {
            txtDBName.Enabled = true;
            txtDirectory.Enabled = true;
            txtFileName.Enabled = true;
            btnBrowser.Enabled = true;
            btnGenerate.Enabled = true;
        }

        //GenerateScriptSQL Form Events

        private void GenerateScriptSQL_Load(object sender, EventArgs e)
        {
            txtDBName.Text = frmMain.currentProject.ProjectName;
            txtDirectory.Text = frmMain.currentProject.ProjectPath;
            txtFileName.Text = "SQLScript";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            EnabledTextField();
            GenerateScriptSQL_Load(sender, e);
         
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {                
                txtDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (txtDirectory.Text != "" && txtDBName.Text != "" && txtFileName.Text != "")
            {
                string urlFileScript = txtDirectory.Text + "\\" + txtFileName.Text + ".sql";
                GenerateScript(urlFileScript, txtDBName.Text);
                                
                btnPreview.Enabled = true;
                btnGenerate.Enabled = false;
                urlFile = urlFileScript;
               
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (urlFile != "")
            {
                ProcessStartInfo info = new ProcessStartInfo("NotePad.exe", urlFile);
                Process.Start(info);
            }
        }
          
    }
}