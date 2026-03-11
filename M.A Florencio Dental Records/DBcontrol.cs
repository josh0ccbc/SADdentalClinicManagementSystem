using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class DBcontrol : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public DBcontrol()
        {
            InitializeComponent();
            int totalPatients = GetPatientCount();
            lblPatientCount.Text = "" + totalPatients;
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DBcontrol_Load(object sender, EventArgs e)
        {

        }

        private void BTNAddPatient_Click(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.Button;
            Form2 frm = new Form2();
            frm.Show();
            this.Hide();
        }

        private void BTNAddPatient_MouseEnter(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.HoverButton;
        }

        private void BTNAddPatient_MouseLeave(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.Button;
        }

        public int GetPatientCount()
        {
            int count = 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Patients", con);
                    count = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return count;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
