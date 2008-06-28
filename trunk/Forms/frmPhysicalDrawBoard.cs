using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERDesigner
{
    public partial class frmPhysicalDrawBoard : Form
    {

        public GeneratePhysicalModel generate;
        public frmPhysicalDrawBoard(MetaData md)
        {
            InitializeComponent();
            generate = new GeneratePhysicalModel(md);
            generate.process();           
            pnlPhysical.drawMetaDataPhysical(generate.mdp); 
        }

        public frmPhysicalDrawBoard()
        {
            InitializeComponent();
            //generate = new GeneratePhysicalModel(@"C:\erd_1n.xml");
            //generate.process();
            //pnlPhysical.drawMetaDataPhysical(generate.mdp);
        }
    }
}