﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AboutApp : Form
    {
        SqlConnection conn = null;
        DatabaseConnect db = new DatabaseConnect();

        public AboutApp()
        {
            InitializeComponent();
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
    }
}