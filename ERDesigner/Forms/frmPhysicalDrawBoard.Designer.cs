namespace ERDesigner
{
    partial class frmPhysicalDrawBoard
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
            this.pnlPhysical = new ERDesigner.PanelPhysical();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.xtraScrollableControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlPhysical
            // 
            this.pnlPhysical.BackColor = System.Drawing.Color.White;
            this.pnlPhysical.Location = new System.Drawing.Point(0, 0);
            this.pnlPhysical.Name = "pnlPhysical";
            this.pnlPhysical.Size = new System.Drawing.Size(2480, 3508);
            this.pnlPhysical.TabIndex = 0;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.pnlPhysical);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(784, 564);
            this.xtraScrollableControl1.TabIndex = 1;
            // 
            // frmPhysicalDrawBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 564);
            this.Controls.Add(this.xtraScrollableControl1);
            this.Name = "frmPhysicalDrawBoard";
            this.Text = "Physical Model";
            this.xtraScrollableControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public PanelPhysical pnlPhysical;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;

    }
}

