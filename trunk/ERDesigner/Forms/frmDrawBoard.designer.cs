using ERDesigner;
namespace ERDesigner
{
    partial class frmDrawBoard
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
            this.pnlDrawBoard = new ERDesigner.PanelDoubleBuffered();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.xtraScrollableControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDrawBoard
            // 
            this.pnlDrawBoard.AutoScrollMargin = new System.Drawing.Size(100, 100);
            this.pnlDrawBoard.BackColor = System.Drawing.Color.White;
            this.pnlDrawBoard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDrawBoard.Location = new System.Drawing.Point(0, 0);
            this.pnlDrawBoard.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDrawBoard.Name = "pnlDrawBoard";
            this.pnlDrawBoard.Size = new System.Drawing.Size(2480, 3508);
            this.pnlDrawBoard.TabIndex = 0;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.pnlDrawBoard);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(792, 566);
            this.xtraScrollableControl1.TabIndex = 1;
            // 
            // frmDrawBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.xtraScrollableControl1);
            this.DoubleBuffered = true;
            this.Name = "frmDrawBoard";
            this.Text = "Draw Board";
            this.xtraScrollableControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public PanelDoubleBuffered pnlDrawBoard;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;




    }
}

