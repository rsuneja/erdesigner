namespace ERDesigner
{
    partial class DataDescription
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboDataType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.lbLength = new DevExpress.XtraEditors.LabelControl();
            this.chkNull = new DevExpress.XtraEditors.CheckEdit();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new DevExpress.XtraEditors.LabelControl();
            this.label2 = new DevExpress.XtraEditors.LabelControl();
            this.btnExpand = new DevExpress.XtraEditors.SimpleButton();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtLength = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDataType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNull.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cboDataType
            // 
            this.cboDataType.EditValue = "Text";
            this.cboDataType.Location = new System.Drawing.Point(3, 22);
            this.cboDataType.Name = "cboDataType";
            this.cboDataType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDataType.Properties.Items.AddRange(new object[] {
            "Number",
            "Text",
            "LongText",
            "Decimal",
            "DateTime",
            "Binary"});
            this.cboDataType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboDataType.Size = new System.Drawing.Size(128, 20);
            this.cboDataType.TabIndex = 1;
            this.cboDataType.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboDataType_MouseUp);
            this.cboDataType.SelectedIndexChanged += new System.EventHandler(this.cboDataType_SelectedIndexChanged);
            this.cboDataType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cboDataType_MouseUp);
            // 
            // lbLength
            // 
            this.lbLength.Location = new System.Drawing.Point(7, 51);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(33, 13);
            this.lbLength.TabIndex = 2;
            this.lbLength.Text = "Length";
            // 
            // chkNull
            // 
            this.chkNull.EditValue = true;
            this.chkNull.Location = new System.Drawing.Point(1, 75);
            this.chkNull.Name = "chkNull";
            this.chkNull.Properties.Caption = "Allow Null";
            this.chkNull.Size = new System.Drawing.Size(75, 18);
            this.chkNull.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(82, 74);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(49, 19);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Appearance.ForeColor = System.Drawing.Color.White;
            this.label1.Appearance.Options.UseBackColor = true;
            this.label1.Appearance.Options.UseForeColor = true;
            this.label1.Appearance.Options.UseTextOptions = true;
            this.label1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.label1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data Description";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            // 
            // btnExpand
            // 
            this.btnExpand.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExpand.Appearance.Options.UseFont = true;
            this.btnExpand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnExpand.Location = new System.Drawing.Point(0, 95);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(136, 14);
            this.btnExpand.TabIndex = 6;
            this.btnExpand.Text = "...";
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(3, 112);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(128, 45);
            this.txtDescription.TabIndex = 6;
            // 
            // txtLength
            // 
            this.txtLength.EditValue = "50";
            this.txtLength.Location = new System.Drawing.Point(46, 48);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(85, 20);
            this.txtLength.TabIndex = 3;
            // 
            // DataDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkNull);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbLength);
            this.Controls.Add(this.cboDataType);
            this.Name = "DataDescription";
            this.Size = new System.Drawing.Size(136, 109);
            ((System.ComponentModel.ISupportInitialize)(this.cboDataType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNull.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.ComboBoxEdit cboDataType;
        private DevExpress.XtraEditors.LabelControl lbLength;
        public DevExpress.XtraEditors.CheckEdit chkNull;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.LabelControl label1;
        private DevExpress.XtraEditors.LabelControl label2;
        public DevExpress.XtraEditors.SimpleButton btnExpand;
        public System.Windows.Forms.TextBox txtDescription;
        public DevExpress.XtraEditors.TextEdit txtLength;

    }
}
