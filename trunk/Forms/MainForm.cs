using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using ERDesigner.Shape;
using System.IO;

namespace ERDesigner
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ProjectData currentProject;
        JSerializer js = new JSerializer();

        public MainForm()
        {
            SplashScreen sp = new SplashScreen();
            sp.Show();

            InitializeComponent();
            
            sp.Dispose();
       }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Khởi tạo thêm
            initColor();
        }

        //Khởi tạo gallery màu
        void initColor()
        {
            gddColor.BeginUpdate();
            //add web color vô gallery thường và gallery drop down
            foreach (Color color in DevExpress.XtraEditors.Popup.ColorListBoxViewInfo.WebColors)
            {
                if (color == Color.Transparent) continue;

                GalleryItem item = new GalleryItem();
                item.Caption = color.Name;
                item.Tag = color;
                item.Hint = color.Name;

                gddColor.Gallery.Groups[0].Items.Add(item);
                galColor.Gallery.Groups[0].Items.Add(item);
            }
            //add system color vô gallery drop down
            foreach (Color color in DevExpress.XtraEditors.Popup.ColorListBoxViewInfo.SystemColors)
            {
                GalleryItem item = new GalleryItem();
                item.Caption = color.Name;
                item.Tag = color;
                gddColor.Gallery.Groups[1].Items.Add(item);
            }
            gddColor.EndUpdate();
        }

        //tô màu cho từng item trong gallery
        private void gddColor_GalleryCustomDrawItemImage(object sender, GalleryItemCustomDrawEventArgs e)
        {
            Color clr = (Color)e.Item.Tag;
            using (Brush brush = new SolidBrush(clr))
            {
                e.Cache.FillRectangle(brush, e.Bounds);
                e.Handled = true;
            }
        }

        private void btnClear_Click(object sender, ItemClickEventArgs e)
        {
            if (MessageBox.Show("Are you sure want to clear ?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                ((frmDrawBoard)ActiveMdiChild).Clear();
            }
        }
        private void chkApplySkin_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (((BarCheckItem)sender).Checked)
                ShapeBase.skin = true;
            else
                ShapeBase.skin = false;

            if (ActiveMdiChild != null)
            {
                ActiveMdiChild.Refresh();
            }
        }
        private void btnExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            Application.Exit();
        }
        private void btnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                ActiveMdiChild.Close();
            }
        }
        private void btnVerifyModel_ItemClick(object sender, ItemClickEventArgs e)
        {
            verifyModel();
        }
        private void btnUndo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.doUndo();
            }
        }
        private void btnDelete_Click(object sender, ItemClickEventArgs e)
        {
            deleteShape();
        }
        private void btnStrongEntity_LinkPressed(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmDrawBoard")
            {
                ((frmDrawBoard)ActiveMdiChild).prepairDrawing(((DevExpress.XtraNavBar.NavBarItem)sender).Caption);
            }
        }
        private void btnPointer_LinkPressed(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            UncheckAll();
        }
        private void btnGeneratePhysical_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmDrawBoard")
            {
                if (verifyModel())
                {
                    ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.EndEditDataType();

                    MetaData ERD = ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.getMetaData();
                    frmPhysicalDrawBoard frmPhysical = new frmPhysicalDrawBoard(ERD);
                    frmPhysical.Text = ActiveMdiChild.Text+"Physical";
                    frmPhysical.MdiParent = this;
                    frmPhysical.Show();

                    frmPhysical.FormClosing += new FormClosingEventHandler(drawboard_FormClosing);
                }
            }
        }

        //My function
        public void UncheckAll()
        {
            if (ActiveMdiChild != null)
            {
                navBarOther.SelectedLinkIndex = 0;
                ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.Cursor = Cursors.Arrow;
            }
        }
        private bool verifyModel()
        {
            if (ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmDrawBoard")
            {
                List<string> lstErr = ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.VerifyModel();
                listErrorList.Items.Clear();

                if (lstErr.Count > 0)
                {
                    listErrorList.ForeColor = Color.Red;
                    listErrorList.Items.AddRange(lstErr.ToArray());
                    dockErrorList.Show();
                    return false;
                }
                else
                {
                    listErrorList.ForeColor = Color.Black;
                    listErrorList.Items.Add("Verification Successful...");
                    listErrorList.Items.Add("Your model now can be generated to Physical Model");
                    dockErrorList.Show();
                    return true;
                }
            }
            return false;
        }
        public void deleteShape()
        {
            if (ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmDrawBoard")
            {
                if (((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.deleteAffectingShape())
                    btnUndo.Enabled = true;
            }
        }
        void refreshTreeView()
        {
            if (currentProject != null)
            {
                SaveProject();

                treeView1.Nodes.Clear();
                TreeNode rootnode = new TreeNode(currentProject.ProjectName);
                
                foreach (Model m in currentProject.Models)
                {
                    TreeNode node = new TreeNode(m.ModelName);
                    rootnode.Nodes.Add(node);
                    node.ContextMenuStrip = contextMenuStrip1;
                }
                treeView1.Nodes.Add(rootnode);
                
                treeView1.ExpandAll();
            }
        }
        private void drawboard_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult ds = MessageBox.Show("Do you want to save " + ((Form)sender).Text + " ?", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (ds == DialogResult.Yes)
            {
                SaveModel(((Form)sender));
            }
            else if (ds == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void btnNewProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (currentProject == null)
            {
                New_Project np = new New_Project();
                np.ShowDialog();
                if (np.txtProjectName.Text != "")
                {
                    currentProject = new ProjectData(np.txtProjectName.Text, np.browseFileControl1.txtFileName.Text);
                    if (!Directory.Exists(currentProject.ProjectPath))
                    {
                        Directory.CreateDirectory(currentProject.ProjectPath);
                        NewModel();
                        refreshTreeView();
                    }
                    else
                    {
                        string s = "Project location was existed in system, do you want to replace it ?";
                        s += "\nWarning: This action can replace your file in that location";
                        s += "\n\nDo you want to contitnue ?";
                        if (MessageBox.Show(s, "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                        {
                            NewModel();
                            refreshTreeView();
                        }
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Do you want to close current project ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    btnCloseProject_ItemClick(sender, e);
                }
            }
        }
        private void btnOpenProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentProject = (ProjectData)js.loadProjectFromXML(openFileDialog1.FileName); //Load xml lên
                    currentProject.ProjectPath = openFileDialog1.FileName.Substring(0,openFileDialog1.FileName.LastIndexOf('\\'));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File has invalid format");
                    return;
                }
                refreshTreeView();
                //this.Text = FileName.Substring(FileName.LastIndexOf('\\') + 1);
            }
        }
        private void btnSaveProjectAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            SaveProject();
        }

        private void SaveProject()
        {
            if (currentProject != null)
            {
                js.saveProjectToXML(currentProject.ProjectPath + "/project.xml", currentProject);
            }
        }
        
        private void btnNewConcept_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (currentProject != null)
            {
                NewModel();
            }
            else
            {
                MessageBox.Show("You have to create new project first", "Warning");
            }
        }
        private void btnOpenModel_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
        private void btnSaveModelAs_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ActiveMdiChild != null && currentProject != null)
            {
                SaveModel((Form)ActiveMdiChild);
            }
        }

        
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView1.Nodes.Count > 0 && treeView1.SelectedNode != null)
            {
                string model = treeView1.SelectedNode.Text;
                bool isOpened = false;
                foreach (Form f in this.MdiChildren)
                {
                    if (model == f.Text)
                    {
                        f.Focus();
                        isOpened = true;
                        break;
                    }
                }

                if (!isOpened)
                {
                    foreach (Model m in currentProject.Models)
                    {
                        if (m.ModelName == model)
                        {
                            OpenModel(model, m.ModelType);
                            break;
                        }
                    }
                }
            }
        }

        private void NewModel()
        {
            Model m = new Model("", ModelType.Conceptual);
            currentProject.Models.Add(m);

            frmDrawBoard drawboard = new frmDrawBoard();
            drawboard.Text = m.ModelName;
            drawboard.MdiParent = this;
            drawboard.Show();

            SaveModel((Form)drawboard);

            drawboard.FormClosing+=new FormClosingEventHandler(drawboard_FormClosing);
            refreshTreeView();
        }
        private void OpenModel(string name, string type)
        {
            string pathfile;
            if (type == ModelType.Conceptual)
                pathfile = currentProject.ProjectPath + "/" + name + ModelType.ConceptualExtention;
            else
                pathfile = currentProject.ProjectPath + "/" + name + ModelType.PhysicalExtention;

            if (checkExistFile(name, type))
            {
                if (type == ModelType.Conceptual)
                {
                    frmDrawBoard drawboard = new frmDrawBoard();
                    drawboard.Text = name;
                    drawboard.MdiParent = this;
                    drawboard.Show();

                    drawboard.pnlDrawBoard.drawMetaData(js.loadConceptualFromXML(pathfile));
                    drawboard.pnlDrawBoard.UndoList.Clear();
                    drawboard.pnlDrawBoard.saveUndoList();

                    drawboard.FormClosing += new FormClosingEventHandler(drawboard_FormClosing);
                }
                else
                {
                    frmPhysicalDrawBoard drawboard = new frmPhysicalDrawBoard();
                    drawboard.Text = name;
                    drawboard.MdiParent = this;
                    drawboard.Show();

                    drawboard.pnlPhysical.drawMetaDataPhysical(js.loadPhysicalFromXML(pathfile));

                    drawboard.FormClosing += new FormClosingEventHandler(drawboard_FormClosing);
                }
            }
        }

        private bool checkExistFile(string name, string type)
        {
            string pathfile;
            if (type == ModelType.Conceptual)
                pathfile = currentProject.ProjectPath + "/" + name + ModelType.ConceptualExtention;
            else
                pathfile = currentProject.ProjectPath + "/" + name + ModelType.PhysicalExtention;

            if (File.Exists(pathfile))
            {
                return true;
            }
            else
            {
                if (MessageBox.Show("This model canot be found, do you want to delete it from this project ?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    foreach (Model m in currentProject.Models)
                    {
                        if (m.ModelName == name)
                        {
                            currentProject.Models.Remove(m);
                            break;
                        }
                    }
                    refreshTreeView();
                }
                return false;
            }
        }
        private void SaveModel(Form form)
        {
            if (form.GetType().Name == "frmDrawBoard")
            {
                js.saveConceptualToXML(currentProject.ProjectPath + "/" + form.Text + ModelType.ConceptualExtention, ((frmDrawBoard)form).pnlDrawBoard.getMetaData());

                Model m = new Model(form.Text, ModelType.Conceptual);
                bool isInProject = false;
                foreach(Model m2 in currentProject.Models)
                {
                    if (m2.ModelName == m.ModelName)
                    {
                        isInProject = true;
                        break;
                    }
                }
                if (!isInProject)
                {
                    currentProject.Models.Add(m);
                    refreshTreeView();
                }
            }
            else
            {
                js.savePhysicalToXML(currentProject.ProjectPath + "/" + form.Text + ModelType.PhysicalExtention, ((frmPhysicalDrawBoard)form).pnlPhysical.getMetaDataPhysical());

                Model m = new Model(form.Text, ModelType.Physical);
                bool isInProject = false;
                foreach (Model m2 in currentProject.Models)
                {
                    if (m2.ModelName == m.ModelName)
                    {
                        isInProject = true;
                        break;
                    }
                }
                if (!isInProject)
                {
                    currentProject.Models.Add(m);
                    refreshTreeView();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentProject != null)
            {
                //if (MessageBox.Show("Do you want to save this project ?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                //{

                //        js.saveProjectToXML(currentProject.ProjectPath + "/project.xml", currentProject);
                //}
            }
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string oldname = treeView1.SelectedNode.Text;
            string newname = e.Label;

            if (newname != null)
            {
                TreeNode node = (TreeNode)e.Node;
                if (node != treeView1.Nodes[0])
                {
                    foreach (Model m in currentProject.Models)
                    {
                        if (m.ModelName == oldname)
                        {
                            m.ModelName = newname;

                            if (checkExistFile(oldname, m.ModelType))
                            {
                                if (m.ModelType == ModelType.Conceptual)
                                    File.Move(currentProject.ProjectPath + "/" + oldname + ModelType.ConceptualExtention, currentProject.ProjectPath + "/" + newname + ModelType.ConceptualExtention);
                                else
                                    File.Move(currentProject.ProjectPath + "/" + oldname + ModelType.PhysicalExtention, currentProject.ProjectPath + "/" + newname + ModelType.PhysicalExtention);
                            }
                            break;
                        }
                    }
                    foreach (Form f in this.MdiChildren)
                    {
                        if (f.Text == oldname)
                        {
                            f.Text = newname;
                            break;
                        }
                    }
                }
                else
                    currentProject.ProjectName = newname;

                SaveProject();
            }
        }

        private void btnGenerateSQl_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmPhysicalDrawBoard")
            {
                MetaDataPhysical mdp = ((frmPhysicalDrawBoard)ActiveMdiChild).pnlPhysical.getMetaDataPhysical();
                GenerateScriptSQL gpm = new GenerateScriptSQL(this, mdp);
                gpm.MdiParent = this;
                gpm.Show();
            }
            else if (ActiveMdiChild != null && ActiveMdiChild.GetType().Name == "frmDrawBoard")
            {
                if (verifyModel())
                {
                    ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.EndEditDataType();

                    MetaData ERD = ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.getMetaData();
                    frmPhysicalDrawBoard frmPhysical = new frmPhysicalDrawBoard(ERD);
                    frmPhysical.Text = ActiveMdiChild.Text + "Physical";
                    frmPhysical.MdiParent = this;
                    frmPhysical.Show();

                    frmPhysical.FormClosing += new FormClosingEventHandler(drawboard_FormClosing);

                    MetaDataPhysical mdp = frmPhysical.pnlPhysical.getMetaDataPhysical();
                    GenerateScriptSQL gpm = new GenerateScriptSQL(this, mdp);
                    gpm.MdiParent = this;
                    gpm.Show();
                }
            }
        }

        private void btnCloseProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Close();
            }
            treeView1.Nodes.Clear();
            currentProject = null;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                Model model = null;
                foreach (Model m in currentProject.Models)
                {
                    if (treeView1.SelectedNode.Text == m.ModelName)
                    {
                        model = m;
                        break;
                    }
                }
                if (model != null)
                {
                    string pathfile;
                    if (model.ModelType == ModelType.Conceptual)
                        pathfile = currentProject.ProjectPath + "/" + model.ModelName + ModelType.ConceptualExtention;
                    else
                        pathfile = currentProject.ProjectPath + "/" + model.ModelName + ModelType.PhysicalExtention;
                    File.Delete(pathfile);
                    currentProject.Models.Remove(model);
                    refreshTreeView();
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.BeginEdit();
            }
        }

        private void btnRedo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                ((frmDrawBoard)ActiveMdiChild).pnlDrawBoard.doRedo();
                
            }
        }

    }
}