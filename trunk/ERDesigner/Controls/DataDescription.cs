using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ERDesigner
{
    public partial class DataDescription : UserControl
    {
        public DataDescription()
        {
            InitializeComponent();
        }
        private void cboDataType_MouseUp(object sender, MouseEventArgs e)
        {
            changeLengh();
        }

        private void changeLengh()
        {
            //switch (cboDataType.SelectedItem.ToString())
            //{
            //    case "bigint": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "binary": txtLength.Text = "50"; txtLength.Enabled = true; //1 - 8000
            //        break;
            //    case "bit": txtLength.Text = "1"; txtLength.Enabled = false;
            //        break;
            //    case "char": txtLength.Text = "10"; txtLength.Enabled = true; //1 - 8000
            //        break;
            //    case "datetime": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "decimal": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "float": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "image": txtLength.Text = "16"; txtLength.Enabled = false;
            //        break;
            //    case "int": txtLength.Text = "4"; txtLength.Enabled = false;
            //        break;
            //    case "money": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "nchar": txtLength.Text = "10"; txtLength.Enabled = true;//1 - 4000
            //        break;
            //    case "ntext": txtLength.Text = "16"; txtLength.Enabled = false;//1 - 8000
            //        break;
            //    case "numeric": txtLength.Text = "10"; txtLength.Enabled = false;
            //        break;
            //    case "nvarchar": txtLength.Text = "50"; txtLength.Enabled = true;//1 - 4000
            //        break;
            //    case "real": txtLength.Text = "4"; txtLength.Enabled = false;
            //        break;
            //    case "smalldatetime": txtLength.Text = "4"; txtLength.Enabled = false;
            //        break;
            //    case "smallint": txtLength.Text = "2"; txtLength.Enabled = false;
            //        break;
            //    case "sqlvariant": txtLength.Text = ""; txtLength.Enabled = false;
            //        break;
            //    case "text": txtLength.Text = "16"; txtLength.Enabled = false;
            //        break;
            //    case "timestamp": txtLength.Text = "8"; txtLength.Enabled = false;
            //        break;
            //    case "tinyint": txtLength.Text = "1"; txtLength.Enabled = false;
            //        break;
            //    case "uniqueidentifier": txtLength.Text = "16"; txtLength.Enabled = false;
            //        break;
            //    case "varbinary": txtLength.Text = "50"; txtLength.Enabled = true;//1 - 8000
            //        break;
            //    case "varchar": txtLength.Text = "50"; txtLength.Enabled = true;//1 - 8000
            //        break;
            //}
            switch (cboDataType.SelectedItem.ToString())
            {
                case "Number": 
                    txtLength.Text = "4"; txtLength.Enabled = false;
                    break;
                case "Text":
                    txtLength.Text = "50"; txtLength.Enabled = true;
                    break;
                case "LongText":
                    txtLength.Text = "8000"; txtLength.Enabled = true;
                    break;
                case "Decimal":
                    txtLength.Text = "8"; txtLength.Enabled = false;
                    break;
                case "DateTime":
                    txtLength.Text = "8"; txtLength.Enabled = false;
                    break;
                case "Binary":
                    txtLength.Text = "8000"; txtLength.Enabled = false;
                    break;
            }
        }

        private void cboDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            changeLengh();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if(btnExpand.Text == "...")
            {
                this.Height = 172;
                btnExpand.Text = "^";
                txtDescription.Focus();
            }
            else
            {
                this.Height = 109;
                btnExpand.Text = "...";
                btnOK.Focus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
        }
    }
}
