namespace ERDesigner
{
    partial class AddConnection
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
            this.label1 = new DevExpress.XtraEditors.LabelControl();
            this.label2 = new DevExpress.XtraEditors.LabelControl();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label3 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new DevExpress.XtraEditors.LabelControl();
            this.label4 = new DevExpress.XtraEditors.LabelControl();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOK = new DevExpress.XtraEditors.SimpleButton();
            this.btnTest = new DevExpress.XtraEditors.SimpleButton();
            this.cboDatasource = new DevExpress.XtraEditors.ComboBoxEdit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDatasource.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.label1.Location = new System.Drawing.Point(12, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter information to connect to the selected data source or choose a different da" +
                "ta source";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Data source:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(15, 105);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(289, 20);
            this.txtServer.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Server name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Location = new System.Drawing.Point(15, 141);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 90);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log on to the database";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(6, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "User name:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(73, 51);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(210, 20);
            this.txtPassword.TabIndex = 3;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(73, 25);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(210, 20);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.TextChanged += new System.EventHandler(this.txtUserName_TextChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(229, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(148, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(15, 246);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(102, 23);
            this.btnTest.TabIndex = 6;
            this.btnTest.Text = "Test connection";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // cboDatasource
            // 
            this.cboDatasource.Location = new System.Drawing.Point(15, 58);
            this.cboDatasource.Name = "cboDatasource";
            this.cboDatasource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboDatasource.Properties.Items.AddRange(new object[] {
            "Microsoft SQL Server (SqlClient)",
            "Oracle Database (OracleClient)",
            "MySQL Database (MySQL Data Provider)",
            "Microsoft Access Database File (OLE DB)"});
            this.cboDatasource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cboDatasource.Size = new System.Drawing.Size(289, 20);
            this.cboDatasource.TabIndex = 2;
            this.cboDatasource.SelectedIndexChanged += new System.EventHandler(this.cboDatasource_SelectedIndexChanged);
            // 
            // AddConnection
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(316, 288);
            this.Controls.Add(this.cboDatasource);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddConnection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AddConnection";
            this.Load += new System.EventHandler(this.AddConnection_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboDatasource.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl label1;
        private DevExpress.XtraEditors.LabelControl label2;
        private System.Windows.Forms.TextBox txtServer;
        private DevExpress.XtraEditors.LabelControl label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnTest;
        private DevExpress.XtraEditors.LabelControl label5;
        private DevExpress.XtraEditors.LabelControl label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUserName;
        private DevExpress.XtraEditors.ComboBoxEdit cboDatasource;
    }
}