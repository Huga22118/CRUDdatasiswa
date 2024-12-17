using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Login : Form
    {
        Menu Menu;
        Register Register;
        SqlConnection conn = null;
        DatabaseConnect db = new DatabaseConnect();
        public bool isAdminLogin;
        void Menu_Closed(object sender, EventArgs e)
        {
            Menu = null;
        }
        void Register_Closed(object sender, EventArgs e)
        {
            Register = null;
        }



        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            Logger.Log("Login form launched.");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = TXTBlogUsername.Text;
            string password = TXTBlogpass.Text;
            try
            {
                if (db == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    if (GetAccountToCheck(username, password))
                    {
                        conn = db.sqlconn();
                        conn.Open();
                        MessageBox.Show("Log-in berhasil!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Logger.Log("Account Found, log-in Succeded! Opening Main Menu Form...");
                        if (isAdminLogin)
                        {
                            MessageBox.Show("Logged in as Admin.", "Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (Menu == null)
                        {
                            Menu = new Menu();
                            Menu.LoggedInAs = username;
                            Menu.getAdminStatus = isAdminLogin;
                            Menu.FormClosed += Menu_Closed;
                            this.Hide();
                            Menu.ShowDialog();
                            this.Close();
                        }
                    }
                    else
                    {
                        Logger.Log("Wrong Username or Password. Please try again!");
                        MessageBox.Show("Username atau Password salah!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }

                }
                else
                {
                    Logger.Log("Fill in the blanks!");
                    MessageBox.Show("Isi Username dan Password!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    username = "";
                    password = "";
                }
            }
        }

        private bool GetAccountToCheck(string check, string checkPass)
        {
            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    conn = db.sqlconn();
                    conn.Open();
                }

                string query = "select IsAdmin from loginDb where Username=@Username AND Password=@Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", check);
                cmd.Parameters.AddWithValue("@Password", checkPass);
                Logger.Log("Checking Account...");
                object count = cmd.ExecuteScalar();
                if (count != null)
                {
                    isAdminLogin = Convert.ToBoolean(count);
                    string texttt = isAdminLogin ? "Logged in as Admin" : "Logged in as Guest";
                    Logger.Log(texttt);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Logger.Log("Opening Register form...");
                if (Register == null)
                {
                    Register = new Register();
                    Register.FormClosed += Register_Closed;
                    this.Hide();
                    Register.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {

            }
        }
    }
}

