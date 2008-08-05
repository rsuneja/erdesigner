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
    public partial class GenerateScriptSQL : DevExpress.XtraEditors.XtraForm
    {
        private MetaDataPhysical mdp;
        private MainForm frmMain;
        private string urlFile;

        //Constructor
        public GenerateScriptSQL(MainForm f, MetaDataPhysical mdPhysical)
        {
            InitializeComponent();
            frmMain = f;
            mdp = mdPhysical;
        }

        //My Method

        public void SaveScript(string FilePath, string script)
        {
            if (FilePath != "")
            {
                FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(script);
                sw.Close();
                fs.Close();
            }
        }
        public void GenerateDirect(string FolderPath, string dbName, string script)
        {
            if (ThongSo.DB_Mode == DBMS.Access)
            {
                DBProviderBase database = new DBProviderBase();
                FileInfo fi = new FileInfo(FolderPath + "\\" + dbName + ".mdb");
                if (ThongSo.DB_AccessFile == String.Empty)
                {
                    if (fi.Exists)
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Existing file {0}\n Do you want delete it?", dbName + ".mdb"), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            fi.Delete();
                        }
                        else
                            return;

                    if (database.CreateDatabase(dbName, FolderPath))
                    {
                        if (database.Connect(FolderPath + "\\" + dbName))
                        {
                            if (!database.Execute(script))
                                DevExpress.XtraEditors.XtraMessageBox.Show("Execute script ddl error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                            DevExpress.XtraEditors.XtraMessageBox.Show("Connected to database is fail", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Create Database is successful", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Create Database is not successful", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (!database.Connect(ThongSo.DB_AccessFile))
                    {
                        if (!database.Execute(script))
                            DevExpress.XtraEditors.XtraMessageBox.Show("Script DDL execute error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            DevExpress.XtraEditors.XtraMessageBox.Show("Script DDL execute successful!", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else                    
                        DevExpress.XtraEditors.XtraMessageBox.Show("Connected to database is fail", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
            }
            else
            {
                if (ThongSo.DB_Server != "" && ThongSo.DB_Server != null)
                {
                    DBProviderBase database = new DBProviderBase();

                    if (database.TestConnection())
                    {
                        if (!database.Connect(dbName))
                        {
                            database.CreateDatabase(dbName, FolderPath);
                            if (database.Connect(dbName))
                            {
                                database.Execute(script);
                            }
                        }
                        else
                        {
                            if (ThongSo.DB_Mode == DBMS.Oracle)
                            {
                                database.Execute(script);
                            }
                            else if (DevExpress.XtraEditors.XtraMessageBox.Show("This database already existed\nDo you want to overwrite it ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                database.Execute(script);
                            }
                        }

                        database.Close();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Cannot connect to database\nYou must start the database first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cannot connect to database\nPlease specify connection setting", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
            LoadDefault();
            txtDBName.Text = frmMain.currentProject.ProjectName;
            txtFileName.Text = "SQLScript";
        }

        private void LoadDefault()
        {
            switch (ThongSo.DB_Mode)
            {
                case DBMS.MSSQLServer2000:
                    lblDBMS.Text = "Microsoft SQL Server (SqlClient)";
                    break;
                case DBMS.Oracle:
                    lblDBMS.Text = "Oracle Database (OracleClient)";
                    break;
                case DBMS.MySql:
                    lblDBMS.Text = "MySQL Database (MySQL Data Provider)";
                    break;
                case DBMS.Access:
                    lblDBMS.Text = "Microsoft Access Database File (OLE DB)";
                    break;
                default:
                    break;
            }

            txtDirectory.Text = frmMain.currentProject.ProjectPath;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            EnabledTextField();
            LoadDefault();
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
            if (ThongSo.DB_Mode != DBMS.Oracle && txtDBName.Text == "")
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Please enter Database name");
                return;
            }

            GenerateDDL generate = new GenerateDDL(mdp, ThongSo.DB_Mode, txtDBName.Text);
            List<string> listScript = generate.Process();

            if (ThongSo.DB_GenerateScriptFile)
            {
                if (txtDirectory.Text != "" && txtDBName.Text != "" && txtFileName.Text != "")
                {
                    string urlFileScript = txtDirectory.Text + "\\" + txtFileName.Text + ".sql";
                    SaveScript(urlFileScript, listScript[0] + listScript[1] + listScript[2]);
                    urlFile = urlFileScript;
                    btnPreview.Enabled = true;
                }
            }
            if (ThongSo.DB_GenerateDirect)
            {
                GenerateDirect(txtDirectory.Text, txtDBName.Text, listScript[1] + listScript[2]);
            }
            btnGenerate.Enabled = false;
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