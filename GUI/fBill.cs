using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

using BUS;

namespace GUI
{
    public partial class fBill : DevExpress.XtraEditors.XtraForm
    {
        public fBill()
        {
            InitializeComponent();
        }

        private void fBill_Load(object sender, EventArgs e)
        {
            DateTime today = DateTime.Now;
            deFromDate.EditValue = new DateTime(today.Year, today.Month, 1);
            deToDate.EditValue = deFromDate.DateTime.AddMonths(1).AddDays(-1);
            btnRemove.Enabled = false;
        }

        private void btnShowBill_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            LoadListBillByDate((DateTime)deFromDate.EditValue, (DateTime)deToDate.EditValue);
            SplashScreenManager.CloseForm();
        }

        void LoadListBillByDate(DateTime fromDate, DateTime toDate)
        {
            try
            {
                gcBill.DataSource = BillBUS.Instance.GetListBillByDate(fromDate, toDate);
                gvBill.Columns[0].Caption = "Bill ID";
                gvBill.Columns[1].Caption = "Table number";
                gvBill.Columns[2].Caption = "Date";
                gvBill.Columns[3].Caption = "Discount";
                gvBill.Columns[4].Caption = "Total";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex);
            }
        }

        private void gvBill_DoubleClick(object sender, EventArgs e)
        {
            btnRemove.Enabled = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            int id = (int)gvBill.GetRowCellValue(gvBill.FocusedRowHandle, gvBill.Columns[0]);

            if (XtraMessageBox.Show("Delete bill " + id + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    BillInfoBUS.Instance.DeleteBillInfoByBillID(id);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Error: " + ex);
                }

                if (BillBUS.Instance.DeleteBill(id))
                {
                    btnShowBill_Click(sender, e);
                    SplashScreenManager.CloseForm();
                    XtraMessageBox.Show("Bill deleted", "Notification");
                    Log.WriteLog("delete Bill, ID = " + id);
                }
                else
                    XtraMessageBox.Show("Unable to delete bill", "Error");
            }
            btnRemove.Enabled = false;
        }
    }
}