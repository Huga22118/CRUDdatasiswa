using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Media;
using System.IO;
using System.Security.Permissions;
namespace WindowsFormsApp1
{
    public partial class Menu : Form
    {
        SqlConnection sqlcon = null;
        DatabaseConnect conn = new DatabaseConnect();
        SoundPlayer player;
        private bool isMusicPlaying = false;
        private bool isMusicStopped = false;
        Register Register;
        Login Login;
        AboutApp AboutApp;
        public string LoggedInAs
        {
            get; set;
        }

        public string username;
        public bool getAdminStatus { get; set; }
        public Menu()
        {
            InitializeComponent();
            for (int i = 1; i <= 35; i++)
            {
                comboBox1.Items.Add(i);
            }
            player = new SoundPlayer();
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            label4.Text = "Music: ";

        }



        private void GetRefreshDataGrid()
        {
            try
            {
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                sqlcon.Open();

                // Ambil data dari tabel dan tampilkan di DataGridView
                SqlCommand cmd = new SqlCommand("SELECT * FROM UserTable", sqlcon);
                SqlDataAdapter cod = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                cod.Fill(dt);
                dataGridView1.DataSource = dt;

                // Menghapus highlight biru default
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                sqlcon.Open();

                Login log = new Login();

                label5.Text = getAdminStatus ? "Logged in as Admin" : $"Logged in as {LoggedInAs}";
                label5.Visible = getAdminStatus;
                button1.Enabled = getAdminStatus;
                button2.Enabled = getAdminStatus;
                button3.Enabled = getAdminStatus;



                // Ambil data dari tabel dan tampilkan di DataGridView
                SqlCommand cmd = new SqlCommand("SELECT * FROM UserTable", sqlcon);
                SqlDataAdapter cod = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                cod.Fill(dt);
                dataGridView1.DataSource = dt;

                // Menghapus highlight biru default
                dataGridView1.ClearSelection();

                loggedInAsToolStripMenuItem.Text = $"{LoggedInAs}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                SqlCommand cmd = new SqlCommand("INSERT INTO UserTable (Name,Kelas,Absen) Values (@Name,@Kelas,@Absen)", sqlcon);
                sqlcon.Open();
                cmd.Parameters.AddWithValue("@Name", TXTBnama.Text);
                cmd.Parameters.AddWithValue("@Kelas", TXTBkelas.Text);
                cmd.Parameters.AddWithValue("@Absen", int.Parse(comboBox1.Text));

                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil diinput!", "Data", MessageBoxButtons.OK);
                GetRefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                SqlCommand cmd = new SqlCommand("Update UserTable set Kelas=@Kelas, Absen=@Absen where Name=@Name", sqlcon);
                sqlcon.Open();
                cmd.Parameters.AddWithValue("@Name", TXTBnama.Text);
                cmd.Parameters.AddWithValue("@Kelas", TXTBkelas.Text);
                cmd.Parameters.AddWithValue("@Absen", int.Parse(comboBox1.Text));

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show("Error: There is no data to update!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Data berhasil dirubah!", "Data", MessageBoxButtons.OK);
                GetRefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                SqlCommand cmd = new SqlCommand("Delete UserTable where Name=@Name AND Kelas=@Kelas AND Absen=@Absen", sqlcon);
                sqlcon.Open();
                cmd.Parameters.AddWithValue("@Name", TXTBnama.Text);
                cmd.Parameters.AddWithValue("@Kelas", TXTBkelas.Text);
                cmd.Parameters.AddWithValue("@Absen", int.Parse(comboBox1.Text));

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show("Error: There is no data to be removed!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Data berhasil dihapus!", "Data", MessageBoxButtons.OK);
                GetRefreshDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                // Ambil nama dari TextBox pencarian
                string searchName = TXTBnama.Text.Trim();
                string searchKelas = TXTBkelas.Text.Trim();
                string searchAbsen = comboBox1.Text.Trim();

                if (string.IsNullOrEmpty(searchName))
                {
                    GetRefreshDataGrid();
                    //MessageBox.Show("Nama yang dicari tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Pastikan koneksi berhasil
                if (conn == null)
                {
                    MessageBox.Show("Koneksi database belum diinisialisasi.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                sqlcon = conn.sqlconn();
                sqlcon.Open();

                // Ambil data dari database berdasarkan nama
                SqlCommand cmd = new SqlCommand("SELECT * FROM UserTable WHERE Name LIKE @Name", sqlcon);
                cmd.Parameters.AddWithValue("@Name", "%" + searchName + "%");


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Data tidak ditemukan!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Tampilkan hasil pencarian ke DataGridView
                dataGridView1.DataSource = dt;
                dataGridView1.ClearSelection(); // Pastikan hasil pencarian yang salah tidak menghighlight nama
                // Cari dan highlight baris yang sesuai
                bool found = false;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["Name"].Value != null && row.Cells["Name"].Value.ToString() == searchName &&
                        row.Cells["Kelas"].Value != null && row.Cells["Kelas"].Value.ToString() == searchKelas &&
                        row.Cells["Absen"].Value != null && row.Cells["Absen"].Value.ToString() == searchAbsen)
                    {
                        row.Selected = true;
                        found = true;
                        break;
                    }

                }

                if (!found)
                {
                    MessageBox.Show("Data tidak ditemukan di DataGridView!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sqlcon != null && sqlcon.State == ConnectionState.Open)
                {
                    sqlcon.Close();
                }
            }
        }

        private void TXTBnama_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                if (isMusicPlaying)
                {
                    MessageBox.Show("Musik sudah dinyalakan", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                string loc = "FIFTY FIFTY - Cupid Twin Version Speed Up.wav";
                player.SoundLocation = loc;
                player.PlayLooping();
                pictureBox3.Visible = true;
                pictureBox2.Visible = false;
                label4.Text = $"Music: {Path.GetFileNameWithoutExtension(loc)}";
                isMusicPlaying = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (!isMusicPlaying)
                {
                    MessageBox.Show("Musik tidak dinyalakan!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // Hentikan pemutaran musik
                player.Stop();
                pictureBox2.Visible = true;
                pictureBox3.Visible = false;
                isMusicPlaying = false;
                //MessageBox.Show("Musik berhenti.", "Musik", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Register_Closed(object sender, EventArgs e)
        {
            Register = null;
        }

        private void Login_Closed(object sender, EventArgs e)
        {
            Login = null;
        }
        private void AboutApp_Closed(object sender, EventArgs e)
        {
            AboutApp = null;
        }

        private void loggedInAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void logoutExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Login == null)
                {
                    Login = new Login();
                    Login.FormClosed += Login_Closed;
                    this.Hide();
                    Login.ShowDialog();
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (AboutApp == null)
                {
                    AboutApp = new AboutApp(LoggedInAs);
                    AboutApp.FormClosed += AboutApp_Closed;
                    AboutApp.ShowDialog();
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
