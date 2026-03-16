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

    public partial class Patient : UserControl
    {
        public int PatientID;

        public Patient()
        {
            InitializeComponent();

            lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }

        private void Patient_Load(object sender, EventArgs e)
        {

        }

        public void SetPatient(string PatientID,string name, string gender, int age, string contact)
        {
            DPpatientid.Text = PatientID;
            lblName.Text = name;
            lblGenderAge.Text = gender + " • " + age + " yrs old";
            lblContact.Text = contact;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Patient ID: " + PatientID);

            btnView.BackgroundImage = Properties.Resources.view;
            // later you can open full patient details form
        }

        private void btnView_MouseEnter(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view2;
        }

        private void btnView_MouseLeave(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view;
        }

        private void ArchivePatient()
        {
            string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // UPDATE to archive instead of DELETE
                    string query = "UPDATE Patients SET IsArchived = 1 WHERE PatientID = @PatientID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Patient archived successfully!");
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Archive this patient record?",
        "Archive Patient",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ArchivePatient();
            }
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.archive2;
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.archive;
        }
    }
}
