using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WindowsFormsApp1
{
    public partial class AboutApp : Form
    {
        SqlConnection conn = null;
        DatabaseConnect db = new DatabaseConnect();
        Menu Menu;
        Login Login;
        SoundPlayer player;
        private string getName;
        private bool getAdmin;

        public AboutApp(string name, bool getAdminStatus, Menu menu)
        {
            InitializeComponent();
            this.getName = name;
            this.getAdmin = getAdminStatus;
            this.Menu = menu;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string getUrl = "https://github.com/Huga22118";


            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = getUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)

            {
                MessageBox.Show($"Tidak dapat membuka link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutApp_Load(object sender, EventArgs e)
        {
            label8.Text = $"Logged in as {getName}";
            Logger.Log("About App form launched.");
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private bool CheckAccountToDeleteAdmin()
        {
            try
            {
                if (conn == null || conn.State != ConnectionState.Open)
                {
                    conn = db.sqlconn();
                    conn.Open();
                }

                if (!getAdmin)
                {
                    SqlCommand cmd = new SqlCommand("update loginDb set IsAdmin=@IsAdmin where Username=@Username", conn);
                    cmd.Parameters.AddWithValue("@Username", getName);
                    cmd.Parameters.AddWithValue("@IsAdmin", 1);
                    object count = cmd.ExecuteScalar();
                    getAdmin = Convert.ToBoolean(count);
                    Logger.Log("Admin Access Granted! Restarting the app...");
                    MessageBox.Show("Admin Access Granted");
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("Update loginDb set IsAdmin=@IsAdmin where Username=@Username", conn);
                    cmd.Parameters.AddWithValue("@Username", getName);
                    cmd.Parameters.AddWithValue("@IsAdmin", 0);
                    object count = cmd.ExecuteScalar();
                    getAdmin = Convert.ToBoolean(count);
                    Logger.Log("Admin Access Removed! Restarting the app...");
                    MessageBox.Show("Admin Access Removed");
                }

                return true;
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
        private void Login_Closed(object sender, EventArgs e)
        {
            Login = null;
        }
        private void Menu_Closed(object sender, EventArgs e)
        {
            Menu = null;
        }
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                if (db == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                CheckAccountToDeleteAdmin();


                if (Login == null)
                {
                    Login = new Login();
                    Login.FormClosed += Login_Closed;
                    player = new SoundPlayer();
                    player?.Stop();
                    this.Hide();
                    this.Owner?.Hide();
                    Login.ShowDialog();

                    this.Close();
                    this.Owner?.Close();
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
