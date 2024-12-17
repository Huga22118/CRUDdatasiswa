using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Register : Form
    {
        Menu Menu;
        SqlConnection conn = null;
        DatabaseConnect db = new DatabaseConnect();

        void Menu_Closed(object sender, EventArgs e)
        {
            Menu = null;
        }
        public Register()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = TXTBusername.Text;
            string password = TXTBpassword.Text;
            string repeatPass = TXTBrepeat.Text;
            try
            {
                if (db == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(repeatPass))
                {
                    if (password == repeatPass)
                    {
                        if (GetAccountToCheck(username))
                        {
                            MessageBox.Show("Akun sudah terdaftar", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logger.Log("Account already registered!");
                            return;
                        }
                        conn = db.sqlconn();
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("Insert into loginDb (Username,Password,IsAdmin) Values (@Username,@Password,@IsAdmin)", conn);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@IsAdmin", 0);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registrasi berhasil!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Logger.Log("Opening Main Menu form...");
                        if (Menu == null)
                        {
                            Menu = new Menu();
                            Menu.FormClosed += Menu_Closed;
                            Menu.LoggedInAs = username;
                            this.Hide();
                            Menu.ShowDialog();
                            this.Close();
                        }

                    }
                    else
                    {
                        MessageBox.Show("Repeat Password tidak sama dengan Password!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Isi yang kosong!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    repeatPass = "";
                }
            }
        }

        private bool GetAccountToCheck(string check)
        {
            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    conn = db.sqlconn();
                    conn.Open();
                }
                string query = "select count(*) from loginDB where Username=@Username";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", check);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;

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

        private void Register_Load(object sender, EventArgs e)
        {
            Logger.Log("Register form launched.");
        }
    }
}
