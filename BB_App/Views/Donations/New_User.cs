﻿using System;
using System.Windows.Forms; using BB_App.Models;
using MySql.Data.MySqlClient;

namespace BB_App.Views.Donations
{
    public partial class NewUser : UserControl
    {

        public NewUser()
        {
            InitializeComponent();
        }

        #region Methods

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            closeButton.Image = Properties.Resources.Delete_Hover_18px;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            closeButton.Image = Properties.Resources.Delete_18px;
        }

        private void menuButton_MouseEnter(object sender, EventArgs e)
        {
            menuButton.Image = Properties.Resources.Left_Hover_16px;
        }

        private void menuButton_MouseLeave(object sender, EventArgs e)
        {
            menuButton.Image = Properties.Resources.Left_15px;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            ((Main)ParentForm).LoadForm(new AddType());
        }

        /// <summary>
        /// Insert user into the database.
        /// </summary>
        private void AddUser()
        {

            if (SqlConnection.Connect(Properties.Settings.Default.server, Properties.Settings.Default.db_user, Properties.Settings.Default.db_pwd, Properties.Settings.Default.db_name))
            {
                var date = kryptonDateTimePicker1.Value.Year.ToString() + "-" + kryptonDateTimePicker1.Value.Month.ToString() + "-" + kryptonDateTimePicker1.Value.Day.ToString();
                var query = "INSERT INTO users(name, phone, bloodgroup, birthdate, gender, city) VALUES (@name, @phone, @blood, @birth, @gender, @city);";
                var cmd = new MySqlCommand(query, SqlConnection.Conn);
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@name", username.Text);
                cmd.Parameters.AddWithValue("@phone", phone.Text);
                cmd.Parameters.AddWithValue("@blood", bloodGD.Text);
                cmd.Parameters.AddWithValue("@birth", date);
                cmd.Parameters.AddWithValue("@gender", genderD.Text);
                cmd.Parameters.AddWithValue("@city", cityD.Text);

                try
                {
                    cmd.ExecuteNonQuery();  // Adding user

                    var id = 0;
                    var query2 = "SELECT id_user FROM users WHERE phone = " + phone.Text;
                    var cmd2 = new MySqlCommand(query2, SqlConnection.Conn);

                    var result = cmd2.ExecuteScalar();   // Loading user id for donation informations

                    if (result != null)
                        id = Convert.ToInt32(result);
                    else
                        MessageBox.Show("Can't retrieve user id, Try creating the user again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    ((Main)ParentForm).LoadForm(new DonationInformations(new User(id)));
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Can't create the user. Error " + ex.ErrorCode + " : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            bloodGD.Text = e.ClickedItem.Text;
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            genderD.Text = e.ClickedItem.Text;
        }

        private void contextMenuStrip3_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            cityD.Text = e.ClickedItem.Text;
        }

        private void bunifuFlatButton1_Click(object sender, EventArgs e)
        {
            AddUser();
        }

        #endregion

    }
}
