namespace ERDesigner
{
    partial class AddCardinality
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCardinality));
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.labEntity2 = new DevExpress.XtraEditors.LabelControl();
            this.labEntity11 = new DevExpress.XtraEditors.LabelControl();
            this.imageComboBoxEdit2 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.imageCollection2 = new DevExpress.Utils.ImageCollection(this.components);
            this.imageComboBoxEdit1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
            this.labRel = new DevExpress.XtraEditors.LabelControl();
            this.labRel2 = new DevExpress.XtraEditors.LabelControl();
            this.panelDoubleBuffered1 = new ERDesigner.PanelDoubleBuffered();
            this.labEntity1 = new System.Windows.Forms.GroupBox();
            this.labEntity22 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).BeginInit();
            this.labEntity1.SuspendLayout();
            this.labEntity22.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(141, 217);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(222, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            // 
            // labEntity2
            // 
            this.labEntity2.AutoSize = true;
            this.labEntity2.Location = new System.Drawing.Point(284, 25);
            this.labEntity2.Name = "labEntity2";
            this.labEntity2.Size = new System.Drawing.Size(39, 13);
            this.labEntity2.TabIndex = 10;
            this.labEntity2.Text = "Entity2";
            // 
            // labEntity11
            // 
            this.labEntity11.AutoSize = true;
            this.labEntity11.Location = new System.Drawing.Point(284, 26);
            this.labEntity11.Name = "labEntity11";
            this.labEntity11.Size = new System.Drawing.Size(39, 13);
            this.labEntity11.TabIndex = 10;
            this.labEntity11.Text = "Entity1";
            // 
            // imageComboBoxEdit2
            // 
            this.imageComboBoxEdit2.Location = new System.Drawing.Point(148, 22);
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
            this.imageComboBoxEdit1.Location = new System.Drawing.Point(148, 23);
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
            // labRel
            // 
            this.labRel.AutoSize = true;
            this.labRel.Location = new System.Drawing.Point(6, 25);
            this.labRel.Name = "labRel";
            this.labRel.Size = new System.Drawing.Size(65, 13);
            this.labRel.TabIndex = 10;
            this.labRel.Text = "Relationship";
            // 
            // labRel2
            // 
            this.labRel2.AutoSize = true;
            this.labRel2.Location = new System.Drawing.Point(6, 26);
            this.labRel2.Name = "labRel2";
            this.labRel2.Size = new System.Drawing.Size(65, 13);
            this.labRel2.TabIndex = 10;
            this.labRel2.Text = "Relationship";
            // 
            // panelDoubleBuffered1
            // 
            this.panelDoubleBuffered1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelDoubleBuffered1.Location = new System.Drawing.Point(12, 12);
            this.panelDoubleBuffered1.Name = "panelDoubleBuffered1";
            this.panelDoubleBuffered1.Size = new System.Drawing.Size(421, 70);
            this.panelDoubleBuffered1.TabIndex = 12;
            // 
            // labEntity1
            // 
            this.labEntity1.Controls.Add(this.labRel);
            this.labEntity1.Controls.Add(this.imageComboBoxEdit2);
            this.labEntity1.Controls.Add(this.labEntity2);
            this.labEntity1.Location = new System.Drawing.Point(12, 156);
            this.labEntity1.Name = "labEntity1";
            this.labEntity1.Size = new System.Drawing.Size(421, 52);
            this.labEntity1.TabIndex = 13;
            this.labEntity1.TabStop = false;
            this.labEntity1.Text = "Entity1";
            // 
            // labEntity22
            // 
            this.labEntity22.Controls.Add(this.labRel2);
            this.labEntity22.Controls.Add(this.labEntity11);
            this.labEntity22.Controls.Add(this.imageComboBoxEdit1);
            this.labEntity22.Location = new System.Drawing.Point(12, 98);
            this.labEntity22.Name = "labEntity22";
            this.labEntity22.Size = new System.Drawing.Size(421, 52);
            this.labEntity22.TabIndex = 13;
            this.labEntity22.TabStop = false;
            this.labEntity22.Text = "Entity2";
            // 
            // AddCardinality
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 255);
            this.Controls.Add(this.panelDoubleBuffered1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.labEntity1);
            this.Controls.Add(this.labEntity22);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCardinality";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cardinalities";
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).EndInit();
            this.labEntity1.ResumeLayout(false);
            this.labEntity1.PerformLayout();
            this.labEntity22.ResumeLayout(false);
            this.labEntity22.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.LabelControl labEntity2;
        private DevExpress.XtraEditors.LabelControl labEntity11;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit2;
        private DevExpress.Utils.ImageCollection imageCollection2;
        private DevExpress.XtraEditors.ImageComboBoxEdit imageComboBoxEdit1;
        private DevExpress.XtraEditors.LabelControl labRel;
        private DevExpress.XtraEditors.LabelControl labRel2;
        private ERDesigner.PanelDoubleBuffered panelDoubleBuffered1;
        private System.Windows.Forms.GroupBox labEntity1;
        private System.Windows.Forms.GroupBox labEntity22;
    }
}