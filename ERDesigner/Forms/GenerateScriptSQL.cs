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
    public partial class GenerateScriptSQL : Form
    {
        private MetaDataPhysical mdp;
        private MainForm frmMain;
        private string urlFile;
        private List<string> listProcess = new List<string>();
        //Constructor
        public GenerateScriptSQL(MainForm f,MetaDataPhysical mdPhysical)
        {                
            InitializeComponent();
            frmMain = f;
            mdp = mdPhysical;           
        }
                       
        //My Method
        public List<string> generateScript(string urlFileScript, string dbName)
        {
            //Local Constant             
            //Local Variable 
            List<string> listProcess = new List<string>();
            List<string> listCreateTable = new List<string>();
            List<string> listPK = new List<string>();
            List<string> listFK = new List<string>();
            //List<string> listDropTable = new List<string>();
            //List<string> listDropFK = new List<string>();

            //Local Process

            foreach (Table table in mdp.Tables)
            {
                //string strDropTable = ScriptSQL.dropTable(table.name);
                string strCreateTable = ScriptSQL.createTable(table.name, table.columns);

                listProcess.Add("Create Tables");
                //listDropTable.Add(strDropTable);
                listCreateTable.Add(strCreateTable);
                listProcess.Add("\t " + table.name);

                listProcess.Add("Create Primary Keys");
                List<string> listPKName = new List<string>();
                foreach (Column col in table.columns)
                    if (col.PrimaryKey)
                    {
                        listPKName.Add(col.Name);
                        listProcess.Add("\t PK_" + col.Name);
                    }

                string strPK = ScriptSQL.createPrimaryKey(table.name, listPKName);
                listPK.Add(strPK);

            }

            listProcess.Add("Create Foreign Keys");
            foreach (ForeignKey fk in mdp.ForeignKeys)
            {
                //string strDropFK = ScriptSQL.dropForeignKey(fk.Name, fk.ChildTable);
                string strFK = ScriptSQL.createForeignKey(fk.Name, fk.ParentTable, fk.ParentColumn, fk.ChildTable, fk.ChildColumn);
                //listDropFK.Add(strDropFK);
                listFK.Add(strFK);
                listProcess.Add("\t " + fk.Name);
            }

            string strDropDB = ScriptSQL.dropDatabase(dbName);
            string strDB = ScriptSQL.createDataBase(dbName);
            string strTables = ScriptSQL.generateTables(listCreateTable, listPK);
            string strFKs = ScriptSQL.generateForeignKeys(listFK);
            //string strDropFKs = ScriptSQL.generateDropForeignKeys(listDropFK);


            if (urlFileScript != "")
            {
                FileStream fs = new FileStream(urlFileScript, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(strDB);
                //sw.Write(strDropFKs);
                sw.Write(strTables);
                sw.Write(strFKs);
                sw.Close();
                fs.Close();
            }

            return listProcess;
        }

        private void disabledTextField()
        {
            txtDBName.Enabled = false;
            txtDirectory.Enabled = false;
            txtFileName.Enabled = false;
            btnBrowser.Enabled = false;
        }

        private void enabledTextField()
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
            enabledTextField();
            GenerateScriptSQL_Load(sender, e);
            listProcess = new List<string>();
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
                listProcess = generateScript(urlFileScript, txtDBName.Text);
                
                frmMain.listErrorList.Items.Clear();
                frmMain.listErrorList.Items.AddRange(listProcess.ToArray());

                btnPreview.Enabled = true;
                btnGenerate.Enabled = false;
                urlFile = urlFileScript;

                //this.Close();

                //frmEditDDLScript frmEdit = new frmEditDDLScript(urlFileScript);
                //frmEdit.Text = "Edit";
                //frmEdit.Show();
                //frmEdit.Focus();
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