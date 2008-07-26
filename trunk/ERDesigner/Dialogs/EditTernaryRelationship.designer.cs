namespace ERDesigner
{
    partial class EditTernaryRelationship
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTernaryRelationship));
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.imageComboBoxEdit2 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.imageCollection2 = new DevExpress.Utils.ImageCollection(this.components);
            this.imageComboBoxEdit1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.labEntity1 = new System.Windows.Forms.GroupBox();
            this.labEntity2 = new System.Windows.Forms.GroupBox();
            this.imageComboBoxEdit3 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.labEntity3 = new System.Windows.Forms.GroupBox();
            this.panelDoubleBuffered1 = new ERDesigner.PanelDoubleBuffered();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).BeginInit();
            this.labEntity1.SuspendLayout();
            this.labEntity2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit3.Properties)).BeginInit();
            this.labEntity3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(169, 307);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(46, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(221, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(50, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            // 
            // imageComboBoxEdit2
            // 
            this.imageComboBoxEdit2.Location = new System.Drawing.Point(9, 20);
            this.imageComboBoxEdit2.Name = "imageComboBoxEdit2";
            this.imageComboBoxEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.imageComboBoxEdit2.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional One", "0, 1", 1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory One", "1, 1", 3),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional Many", "0, n", 0),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory Many", "1, n", 2)});
            this.imageComboBoxEdit2.Properties.SmallImages = this.imageCollection2;
            this.imageComboBoxEdit2.Size = new System.Drawing.Size(130, 20);
            this.imageComboBoxEdit2.TabIndex = 11;
            this.imageComboBoxEdit2.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxEdit2_SelectedIndexChanged);
            // 
            // imageCollection2
            // 
            this.imageCollection2.ImageSize = new System.Drawing.Size(20, 15);
            this.imageCollection2.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection2.ImageStream")));
            // 
            // imageComboBoxEdit1
            // 
            this.imageComboBoxEdit1.Location = new System.Drawing.Point(9, 19);
            this.imageComboBoxEdit1.Name = "imageComboBoxEdit1";
            this.imageComboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.imageComboBoxEdit1.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional One", "0, 1", 1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory One", "1, 1", 3),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional Many", "0, n", 0),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory Many", "1, n", 2)});
            this.imageComboBoxEdit1.Properties.SmallImages = this.imageCollection2;
            this.imageComboBoxEdit1.Size = new System.Drawing.Size(130, 20);
            this.imageComboBoxEdit1.TabIndex = 11;
            this.imageComboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxEdit1_SelectedIndexChanged);
            // 
            // labEntity1
            // 
            this.labEntity1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labEntity1.Controls.Add(this.imageComboBoxEdit1);
            this.labEntity1.Location = new System.Drawing.Point(152, 12);
            this.labEntity1.Name = "labEntity1";
            this.labEntity1.Size = new System.Drawing.Size(145, 52);
            this.labEntity1.TabIndex = 13;
            this.labEntity1.TabStop = false;
            this.labEntity1.Text = "Entity 1";
            // 
            // labEntity2
            // 
            this.labEntity2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labEntity2.Controls.Add(this.imageComboBoxEdit2);
            this.labEntity2.Location = new System.Drawing.Point(12, 290);
            this.labEntity2.Name = "labEntity2";
            this.labEntity2.Size = new System.Drawing.Size(145, 52);
            this.labEntity2.TabIndex = 13;
            this.labEntity2.TabStop = false;
            this.labEntity2.Text = "Entity 2";
            // 
            // imageComboBoxEdit3
            // 
            this.imageComboBoxEdit3.Location = new System.Drawing.Point(9, 20);
            this.imageComboBoxEdit3.Name = "imageComboBoxEdit3";
            this.imageComboBoxEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.imageComboBoxEdit3.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[] {
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional One", "0, 1", 1),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory One", "1, 1", 3),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Optional Many", "0, n", 0),
            new DevExpress.XtraEditors.Controls.ImageComboBoxItem("Mandatory Many", "1, n", 2)});
            this.imageComboBoxEdit3.Properties.SmallImages = this.imageCollection2;
            this.imageComboBoxEdit3.Size = new System.Drawing.Size(130, 20);
            this.imageComboBoxEdit3.TabIndex = 11;
            this.imageComboBoxEdit3.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxEdit3_SelectedIndexChanged);
            // 
            // labEntity3
            // 
            this.labEntity3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labEntity3.Controls.Add(this.imageComboBoxEdit3);
            this.labEntity3.Location = new System.Drawing.Point(287, 290);
            this.labEntity3.Name = "labEntity3";
            this.labEntity3.Size = new System.Drawing.Size(145, 52);
            this.labEntity3.TabIndex = 13;
            this.labEntity3.TabStop = false;
            this.labEntity3.Text = "Entity 3";
            // 
            // panelDoubleBuffered1
            // 
            this.panelDoubleBuffered1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelDoubleBuffered1.Location = new System.Drawing.Point(11, 76);
            this.panelDoubleBuffered1.Name = "panelDoubleBuffered1";
            this.panelDoubleBuffered1.Size = new System.Drawing.Size(421, 202);
            this.panelDoubleBuffered1.TabIndex = 12;
            // 
            // EditTernaryRelationship
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 354);
            this.Controls.Add(this.panelDoubleBuffered1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labEntity3);
            this.Controls.Add(this.labEntity2);
            this.Controls.Add(this.labEntity1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditTernaryRelationship";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit Tenary Relationship";
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).EndInit();
            this.labEntity1.ResumeLayout(false);
            this.labEntity2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit3.Properties)).EndInit();
            this.labEntity3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit2;
        private DevExpress.Utils.ImageCollection imageCollection2;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit1;
        private ERDesigner.PanelDoubleBuffered panelDoubleBuffered1;
        private System.Windows.Forms.GroupBox labEntity1;
        private System.Windows.Forms.GroupBox labEntity2;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit3;
        private System.Windows.Forms.GroupBox labEntity3;
    }
}