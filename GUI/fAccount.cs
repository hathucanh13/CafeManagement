using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;

using BUS;

namespace GUI
{
    public partial class fAccount : DevExpress.XtraEditors.XtraForm
    {
        private string loginUserName;

        public string LoginUserName
        {
            get { return loginUserName; }
            set { loginUserName = value; }
        }

        public fAccount()
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            InitializeComponent();
            LoadAcount();
            btnRemove.Enabled = false;
            btnResetPassword.Enabled = false;
            btnSearch.Enabled = false;
            SplashScreenManager.CloseForm();
        }

        private void LoadAcount()
        {
            try
            {
                gcAccount.DataSource = AccountBUS.Instance.GetAllAcount();
                gvAccount.Columns[0].Caption = "Username";
                gvAccount.Columns[1].Caption = "Full name";
                gvAccount.Columns[2].Caption = "Account";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex);
            }

            RepositoryItemLookUpEdit myLookup = new RepositoryItemLookUpEdit();
            try
            {
                myLookup.DataSource = AccountTypeBUS.Instance.GetAllAccountType();
                myLookup.DisplayMember = "TypeName";
                myLookup.ValueMember = "ID";
                myLookup.NullText = "-- This account is for --";
                gvAccount.Columns[2].ColumnEdit = myLookup;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex);
            }
        }

        private bool CheckCharacter(string str)
        {
            string correctString = "1234567890 QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
            foreach (char character1 in str)
            {
                bool isCorrect = false;
                foreach (char character2 in correctString)
                {
                    if (character1 == character2)
                        isCorrect = true;
                }
                if (isCorrect == false)
                    return false;
            }
            return true;
        }

        private void gvAccount_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;

            if (view.IsNewItemRow(e.RowHandle))
                InsertAccount(view, e.RowHandle);
            else
            {
                string curUserName = view.GetRowCellValue(e.RowHandle, view.Columns[0]).ToString();
                for (int i = 0; i < view.RowCount - 1; i++)
                {
                    if (curUserName.Equals(view.GetRowCellValue(i, view.Columns[0]).ToString()))
                    {
                        view.SetRowCellValue(view.FocusedRowHandle, view.Columns[0], "");
                        
                        return;
                    }
                }
            }
        }

        private void InsertAccount(GridView view, int rowHandle)
        {
            string userName = view.GetRowCellValue(rowHandle, view.Columns[0]).ToString();
            if (userName == "" || CheckCharacter(userName) == false)
            {
                XtraMessageBox.Show("Invalid username");
                return;
            }

            for (int i = 0; i < view.RowCount - 1; i++)
            {
                if (userName.Equals(view.GetRowCellValue(i, view.Columns[0]).ToString()))
                {
                    view.SetRowCellValue(view.FocusedRowHandle, view.Columns[0], "");
                    XtraMessageBox.Show("This username is already taken!");
                    return;
                }
            }

            string displayName = view.GetRowCellValue(rowHandle, view.Columns[1]).ToString();
            if (displayName == "")
            {
                XtraMessageBox.Show("Invalid name!");
                return;
            }

            int accountType;
            if (int.TryParse(view.GetRowCellValue(rowHandle, view.Columns[2]).ToString(), out accountType) == false)
            {
                XtraMessageBox.Show("Please choose your account type");
                return;
            }

            if (AccountBUS.Instance.Insert(userName, displayName, accountType))
            {
                SplashScreenManager.ShowForm(typeof(WaitForm1));
                LoadAcount();
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show("New account added!\n Default password is '0'\n Please update your password", "Notifications");
                Log.WriteLog("add new Account: " + userName);
            }
            else
                XtraMessageBox.Show("Failed to add new account", "Error");
        }

        private void gcAccount_DoubleClick(object sender, EventArgs e)
        {
            if (gvAccount.FocusedRowHandle >= 0)
            {
                btnRemove.Enabled = true;
                btnResetPassword.Enabled = true;
            }
        }

        private void btnRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string userName = gvAccount.GetRowCellValue(gvAccount.FocusedRowHandle, gvAccount.Columns[0]).ToString();
            if (loginUserName.Equals(userName))
            {
                XtraMessageBox.Show("Cannot delete current account!!!");
                return;
            }

            if (XtraMessageBox.Show("Delete " + userName + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (AccountBUS.Instance.Delete(userName))
                {
                    SplashScreenManager.ShowForm(typeof(WaitForm1));
                    LoadAcount();
                    SplashScreenManager.CloseForm();
                    XtraMessageBox.Show("Account deleted!");
                    Log.WriteLog("delete Account: " + userName);
                }
                else
                    XtraMessageBox.Show("Failed to delete account", "Error");
            }
            btnRemove.Enabled = false;
        }

        private void btnResetPassword_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string userName = gvAccount.GetRowCellValue(gvAccount.FocusedRowHandle, gvAccount.Columns[0]).ToString();
            if (XtraMessageBox.Show("Đặt lại mật khẩu cho tài khoản " + userName + "?", "Xác nhận", MessageBoxButtons.YesNo)
                == DialogResult.Yes)
            {
                if (AccountBUS.Instance.ResetPassword(userName))
                {
                    SplashScreenManager.ShowForm(typeof(WaitForm1));
                    LoadAcount();
                    SplashScreenManager.CloseForm();
                    XtraMessageBox.Show("Your password has been reset!\n Default password is '0'");
                    Log.WriteLog("set password for Account: " + userName);
                }
                else
                    XtraMessageBox.Show("Failed to reset password", "Error");
            }
            btnResetPassword.Enabled = false;
        }

        private void txtSearchAccount_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchAccount.Text != "")
                btnSearch.Enabled = true;
            else
                btnSearch.Enabled = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(WaitForm1));
                gcAccount.DataSource = AccountBUS.Instance.SearchAccountByUserName(txtSearchAccount.Text);
                SplashScreenManager.CloseForm();
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show("Error: " + ex);
            }
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            LoadAcount();
            SplashScreenManager.CloseForm();
        }
    }
}