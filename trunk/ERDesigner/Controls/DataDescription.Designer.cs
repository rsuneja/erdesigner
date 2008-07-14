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
            this.txtLength = new DevExpress.XtraEditors.TextEdit();
            this.lbLength = new System.Windows.Forms.Label();
            this.chkNull = new DevExpress.XtraEditors.CheckEdit();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.cboDataType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNull.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // cboDataType
            // 
            this.cboDataType.EditValue = "varchar";
            this.cboDataType.Location = new System.Drawing.Point(3, 22);
            this.cboDataType.Name = "cboDataType";
            this.cboDataType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDataType.Properties.Items.AddRange(new object[] {
            "bigint",
            "binary",
            "bit",
            "char",
            "datetime",
            "decimal",
            "float",
            "image",
            "int",
            "money",
            "nchar",
            "ntext",
            "numeric",
            "nvarchar",
            "real",
            "smalldatetime",
            "smallint",
            "sqlvariant",
            "text",
            "timestamp",
            "tinyint",
            "uniqueidentifier",
            "varbinary",
            "varchar"});
            this.cboDataType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboDataType.Size = new System.Drawing.Size(128, 20);
            this.cboDataType.TabIndex = 0;
            this.cboDataType.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cboDataType_MouseUp);
            this.cboDataType.SelectedIndexChanged += new System.EventHandler(this.cboDataType_SelectedIndexChanged);
            this.cboDataType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cboDataType_MouseUp);
            // 
            // txtLength
            // 
            this.txtLength.EditValue = "50";
            this.txtLength.Location = new System.Drawing.Point(46, 48);
            this.txtLength.Name = "txtLength";
            this.txtLength.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.txtLength.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.txtLength.Size = new System.Drawing.Size(85, 20);
            this.txtLength.TabIndex = 3;
            // 
            // lbLength
            // 
            this.lbLength.AutoSize = true;
            this.lbLength.Location = new System.Drawing.Point(0, 51);
            this.lbLength.Name = "lbLength";
            this.lbLength.Size = new System.Drawing.Size(40, 13);
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
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Data Description";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DataDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.chkNull);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbLength);
            this.Controls.Add(this.cboDataType);
            this.Name = "DataDescription";
            this.Size = new System.Drawing.Size(136, 98);
            ((System.ComponentModel.ISupportInitialize)(this.cboDataType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLength.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkNull.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public DevExpress.XtraEditors.ComboBoxEdit cboDataType;
        public DevExpress.XtraEditors.TextEdit txtLength;
        private System.Windows.Forms.Label lbLength;
        public DevExpress.XtraEditors.CheckEdit chkNull;
        public DevExpress.XtraEditors.SimpleButton btnOK;
        private System.Windows.Forms.Label label1;

    }
}
