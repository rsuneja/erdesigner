using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERDesigner
{
    public partial class AddConnection : DevExpress.XtraEditors.XtraForm
    {
        public AddConnection()
        {
            InitializeComponent();
        }

        private void AddConnection_Load(object sender, EventArgs e)
        {
            cboDatasource.SelectedIndex = 0;
        }
    }
}