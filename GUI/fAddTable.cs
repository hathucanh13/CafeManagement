using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace GUI
{
    public partial class fAddTable : DevExpress.XtraEditors.XtraForm
    {
        public int Table { get; set; }

        public fAddTable()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (txtTable.Text == "")
            {
                XtraMessageBox.Show("Please enter information!!");
                return;
            }

            int temp;
            if (int.TryParse(txtTable.Text, out temp) == false)
            {
                XtraMessageBox.Show("invalid information!!");
                txtTable.Focus();
                return;
            }
            else
            {
                Table = temp;
                this.Close();
            }
        }

        private void fAddTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtTable.Text == "")
                Table = -1;
        }
    }
}