﻿using System; using static BB_App.Helpers.FormsHelpers;
using System.Windows.Forms;
using BB_App.Core.Models;
using BB_App.Core.Properties;

namespace BB_App.Core.Views.Bloods
{
    public partial class Purchases_Report_Form : UserControl
    {
        public Purchases_Report_Form()
        {
            InitializeComponent();
        }

        private void Purchases_Report_Form_Load(object sender, EventArgs e)
        {
            reportDGV.DataSource = BloodPurchase.LoadReports();
            reportDGV.DataMember = "sales";
            reportDGV.ReadOnly = true;
            reportDGV.Columns[0].HeaderText = @"Purchase ID";
            reportDGV.Columns[1].HeaderText = @"Bloodgroup";
            reportDGV.Columns[2].HeaderText = @"Purchase Date";
            reportDGV.Columns[3].HeaderText = @"Units";
            reportDGV.Columns[4].HeaderText = @"Total Price";

            foreach (DataGridViewRow t in reportDGV.Rows)
                t.Cells[1].Value = Commons.Format(t.Cells[1].Value.ToString());
        }

        #region UI Methods

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.Image = Resources.Delete_Hover_18px;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Image = Resources.Delete_18px;
        }

        private void menuButton_MouseEnter(object sender, EventArgs e)
        {
            menuButton.Image = Resources.Left_Hover_16px;
        }

        private void menuButton_MouseLeave(object sender, EventArgs e)
        {
            menuButton.Image = Resources.Left_15px;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            LoadForm(((Main)ParentForm).frmContainer, new BloodDashboard());
        }

        #endregion
    }
}