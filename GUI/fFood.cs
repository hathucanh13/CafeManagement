﻿using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;

using BUS;
using DTO;

namespace GUI
{
    public partial class fFood : DevExpress.XtraEditors.XtraForm
    {
        Food curFood;

        public fFood()
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            InitializeComponent();
            btnRemove.Enabled = false;
            btnSearch.Enabled = false;
            LoadFoodToGridControl();
            SplashScreenManager.CloseForm();
        }

        private void LoadFoodToGridControl()
        {
            try
            {
                gcFood.DataSource = FoodBUS.Instance.GetAllFood();
                gvFood.Columns[0].Caption = "ID";
                gvFood.Columns[0].OptionsColumn.AllowEdit = false;
                gvFood.Columns[1].Caption = "Name";
                gvFood.Columns[2].Caption = "Category";
                gvFood.Columns[3].Caption = "Price";
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex);
            }

            RepositoryItemLookUpEdit myLookup = new RepositoryItemLookUpEdit();
            try
            {
                myLookup.DataSource = CategoryBUS.Instance.GetAllCategory();
                myLookup.DisplayMember = "Name";
                myLookup.ValueMember = "ID";
                myLookup.NullText = "-- Pick category --";
                gvFood.Columns[2].ColumnEdit = myLookup;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Error: " + ex);
            }
        }

        private void gvFood_ValidateRow(object sender, DevExpress.XtraGrid.Views.Base.ValidateRowEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.IsNewItemRow(e.RowHandle))
                AddFood(view, e.RowHandle);
            else
                UpdateFood(view, e.RowHandle);
        }

        private void AddFood(GridView view, int rowHandle)
        {
            string name = view.GetRowCellValue(rowHandle, view.Columns[1]).ToString();
            if (name == "")
            {
                XtraMessageBox.Show("Invalid name");
                return;
            }

            string categoryID = view.GetRowCellValue(rowHandle, view.Columns[2]).ToString();
            if (categoryID == "")
            {
                XtraMessageBox.Show("Please pick a category");
                return;
            }

            string priceTemp = view.GetRowCellValue(rowHandle, view.Columns[3]).ToString();
            if (priceTemp == "")
            {
                XtraMessageBox.Show("Price must not be null");
                return;
            }
            int price = int.Parse(priceTemp);
            if (price <= 0 || price > 10000000)
            {
                XtraMessageBox.Show("Invalid price");
                return;
            }

            Food newFood = new Food(name, int.Parse(categoryID), price);
            if (FoodBUS.Instance.InsertFood(newFood))
            {
                SplashScreenManager.ShowForm(typeof(WaitForm1));
                LoadFoodToGridControl();
                SplashScreenManager.CloseForm();
                Log.WriteLog("add new Food: " + name);
            }
            else
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show("Failed to add new Food", "Error");
            }
        }

        private void UpdateFood(GridView view, int rowHandle)
        {
            string id = view.GetRowCellValue(rowHandle, view.Columns[0]).ToString();
            if (id == "") // id = "" when data is not inserted into database
            {
                AddFood(view, rowHandle);
                return;
            }

            string name = view.GetRowCellValue(rowHandle, view.Columns[1]).ToString();
            if (name == "")
            {
                XtraMessageBox.Show("Invalid name");
                return;
            }

            string categoryID = view.GetRowCellValue(rowHandle, view.Columns[2]).ToString();
            if (categoryID == "")
            {
                XtraMessageBox.Show("Please pick a category");
                return;
            }

            string priceTemp = view.GetRowCellValue(rowHandle, view.Columns[3]).ToString();
            if (priceTemp == "")
            {
                XtraMessageBox.Show("Price must not be null");
                return;
            }
            int price = int.Parse(priceTemp);
            if (price <= 0 || price > 1000000)
            {
                XtraMessageBox.Show("Invalid price");
                return;
            }

            SplashScreenManager.ShowForm(typeof(WaitForm1));
            Food food = new Food(int.Parse(id), name, int.Parse(categoryID), price);
            if (FoodBUS.Instance.UpdateFood(food))
            {
                LoadFoodToGridControl();
                SplashScreenManager.CloseForm();
                Log.WriteLog("update Food: Name: " + curFood.Name + " -> " + name
                              + ", Category ID: " + curFood.CategoryID + " -> " + categoryID
                              + ", Price: " + curFood.Price + " -> " + price);
            }
            else
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show(" Fail to update food\n Unable to update food", "Error");
            }
        }

        private void gcFood_DoubleClick(object sender, EventArgs e)
        {
            if (gvFood.FocusedRowHandle >= 0)
                btnRemove.Enabled = true;
        }

        private void btnRemove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int id = (int)gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[0]);
            string name = gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[1]).ToString();

            if (XtraMessageBox.Show("Delete " + name + "?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (FoodBUS.Instance.DeleteFood(id))
                {
                    SplashScreenManager.ShowForm(typeof(WaitForm1));
                    LoadFoodToGridControl();
                    SplashScreenManager.CloseForm();
                    XtraMessageBox.Show("Deleted " + name, " Notification");
                    Log.WriteLog("delete Food: " + name);
                }
                else
                    XtraMessageBox.Show("Unable to delete food", "Error");
            }
            btnRemove.Enabled = false;
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SplashScreenManager.ShowForm(typeof(WaitForm1));
            LoadFoodToGridControl();
            SplashScreenManager.CloseForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(WaitForm1));
                gcFood.DataSource = FoodBUS.Instance.SearchFoodByName(txtSearchFood.Text);
                SplashScreenManager.CloseForm();
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm();
                XtraMessageBox.Show("Error: " + ex);
            }
        }

        private void txtSearchFood_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchFood.Text != "")
                btnSearch.Enabled = true;
            else
                btnSearch.Enabled = false;
        }

        private void gvFood_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            object id = gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[0]);
            if (id == null || id == DBNull.Value)
                return;

            object name = gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[1]);
            if (name == null || name == DBNull.Value)
                return;

            object categoryID = gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[2]);
            if (categoryID == null || categoryID == DBNull.Value)
                return;

            object price = gvFood.GetRowCellValue(gvFood.FocusedRowHandle, gvFood.Columns[3]);
            if (price == null || price == DBNull.Value)
                return;
                
            curFood = new Food(name.ToString(), (int)categoryID, (int)price);
        }
    }
}