using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ERDesigner
{
    public enum DBMS
    {
        MSSQLServer2000 = 0,
        Oracle,
        MySql,
        Access        
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

        public void GenerateScript(string FilePath, string dbName)
        {
            GenerateDDL generate = new GenerateDDL(mdp, ThongSo.DB_Mode, txtDBName.Text);
            List<string> listScript = generate.Process(); 
            if (FilePath != "")
            {
                FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(listScript[0]);  //Create DB             
                sw.Write(listScript[1]);  //Create Table
                sw.Write(listScript[2]);  //Create Foreign Key
                sw.Close();
                fs.Close();
            }          
            if(ThongSo.DB_Mode == DBMS.MSSQLServer2000 && FilePath == "")
            {
                return;
            }
            
            GenerateDirect(FilePath.Substring(0, FilePath.LastIndexOf('\\')), dbName, listScript[1] + listScript[2]);
        }
        public void GenerateDirect(string FolderPath, string dbName, string script)
        {
            DBProviderBase database = new DBProviderBase();

            if (database.TestConnection())
            {
                if (!database.CreateDatabase(dbName, FolderPath))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show("This database already existed\nDo you want to overwrite it ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                if (database.Connect(dbName))
                {
                    database.Execute(script);
                }
            }
            else
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Cannot connect to database\nYou must start the database first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
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