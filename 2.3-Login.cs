using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Session2_Redo
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the Cancel button is clicked,
        /// it closes the entire application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            using (var context = new Session2Entities())
            {
                if (txtUsername.Text.Trim() == "" || txtPassword.Text.Trim() == "")
                {
                    MessageBox.Show("Please key in your credentials!", "Empty Fields",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var getUser = (from x in context.Employees
                                   where x.Username == txtUsername.Text
                                   select x).FirstOrDefault();
                    if (getUser == null)
                    {
                        MessageBox.Show("User does not exist or has not been created!", "Invalid user",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (getUser.Password != txtPassword.Text)
                    {
                        MessageBox.Show("Password is incorrect!", "Incorrect password",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (getUser.isAdmin == true)
                        {
                            MessageBox.Show($"Welcome {getUser.FirstName} {getUser.LastName}!", "Login successful",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            (new EMMAdmin(getUser.ID)).ShowDialog();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show($"Welcome {getUser.FirstName} {getUser.LastName}!", "Login successful",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Hide();
                            (new EMMnon(getUser.ID)).ShowDialog();
                            this.Close();
                        }
                    }
                }
            }
        }
    }
}
