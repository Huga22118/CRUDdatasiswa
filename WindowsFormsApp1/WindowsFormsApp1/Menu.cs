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
using System.Runtime.CompilerServices;
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
        private StringBuilder inputBuffer = new StringBuilder(); // Buffer untuk menyimpan input keyboard
        public string LoggedInAs
        {
            get; set;
        }

        private Timer rotationTimer;
        private int rotationAngle = 0;

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
            isMusicPlaying = false;
            player.Stop();
            label4.Text = "Music: ";
            this.FormClosing += Menu_FormClosing;
            rotationTimer = new Timer();
            rotationTimer.Interval = 100;
            rotationTimer.Tick += RotationTimer_Tick;
        }

        private void RotationTimer_Tick(object sender, EventArgs e)
        {
            rotationAngle = (rotationAngle + 10) % 360; // Increase angle by 10 degrees
            pictureBox2.Image = RotateImage(Properties.Resources.pause, rotationAngle);
        }

        private Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                g.RotateTransform(rotationAngle);
                g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                g.DrawImage(img, new Point(0, 0));
            }
            return bmp;
        }


        private void Menu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isMusicPlaying)
            {
                player.Stop();
                isMusicPlaying = false;
                Logger.Log("Music disabled.");
            }
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
                Logger.Log("Main menu form launched.");
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
                if (getAdminStatus)
                {
                    // Admin: bisa mengedit tabel
                    dataGridView1.ReadOnly = false;
                    dataGridView1.AllowUserToAddRows = true;
                    dataGridView1.AllowUserToDeleteRows = true;
                    dataGridView1.MultiSelect = false;
                }
                else
                {
                    // Guest: hanya bisa melihat tabel
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = true;
                }
                // Menghapus highlight biru default
                dataGridView1.ClearSelection();
                GetRefreshDataGrid();
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

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    MessageBox.Show("Masukkan data!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("Input the data!");
                    return;
                }


                MessageBox.Show("Data berhasil diinput!", "Data", MessageBoxButtons.OK);
                Logger.Log("Data has been added to the table.");
                GetRefreshDataGrid();
                Logger.Log("Table Refreshed!");





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
                Logger.Log("Data has been updated.");
                GetRefreshDataGrid();
                Logger.Log("Table Refreshed!");
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
                Logger.Log("Data has been deleted!");
                GetRefreshDataGrid();
                Logger.Log("Table Refreshed!");
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
                        Logger.Log($"Data found! Name: {searchName}, Kelas: {searchKelas}, Absen: {searchAbsen}");
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
                pictureBox3.Visible = false;
                pictureBox2.Visible = true;
                label4.Text = $"Music: {Path.GetFileNameWithoutExtension(loc)}";
                isMusicPlaying = true;
                rotationTimer.Start();
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
                pictureBox2.Visible = false;
                pictureBox3.Visible = true;
                isMusicPlaying = false;
                rotationTimer.Stop();
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
                if (isMusicPlaying)
                {
                    isMusicPlaying = false;
                    player.Stop();
                }
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

        /*protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (Menu f in this.OwnedForms)
            {
                f.Close();
            }

            base.OnFormClosing(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            foreach (Menu f in this.OwnedForms)
            {
                switch (this.WindowState)
                {
                    case FormWindowState.Minimized:
                    case FormWindowState.Normal:
                        f.WindowState = this.WindowState;
                        break;

                    case FormWindowState.Maximized:
                        // just restore owned forms to their original sizes when parent form is maximized
                        f.WindowState = FormWindowState.Normal;
                        break;
                }

                // OnSizeChanged must be called, as changing WindowState property
                // does not raise SizeChanged event
                f.OnSizeChanged(EventArgs.Empty);
            }
        }*/

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (AboutApp == null)
                {
                    AboutApp = new AboutApp(LoggedInAs, getAdminStatus, this);
                    AboutApp.Owner = this;
                    //AboutApp.FormClosed += AboutApp_Closed;
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
