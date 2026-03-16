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
    public partial class ArchivePatients : UserControl
    {
        public int PatientID;
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public ArchivePatients()
        {
            InitializeComponent();
            lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Patient ID: " + PatientID);
            btnView.BackgroundImage = Properties.Resources.view;
        }

        public void SetPatient(string PatientID, string name, string gender, int age, string contact)
        {
            this.PatientID = Convert.ToInt32(PatientID);
            lblName.Text = name;
            lblGenderAge.Text = gender + " • " + age + " yrs old";
            lblContact.Text = contact;
        }

        private void ArchivePatients_Load(object sender, EventArgs e)
        {

        }

        private void btnUnarchive_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Restore this patient to active records?",
                "Unarchive Patient",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                UnarchivePatient();
            }
        }
        private void UnarchivePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Set IsArchived back to 0
                    string query = "UPDATE Patients SET IsArchived = 0 WHERE PatientID = @PatientID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Patient restored to active records!");

                    // Refresh archive list
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
                "Are you sure you want to PERMANENTLY DELETE this patient?\n\nThis action CANNOT be undone!",
                "Permanent Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Double confirmation for safety
                DialogResult confirm = MessageBox.Show(
                    "All patient data and appointments will be deleted.\n\nAre you absolutely sure?",
                    "Confirm Permanent Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    PermanentlyDeletePatient();
                }
            }
        }
        private void PermanentlyDeletePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Then delete patient
                    string deletePatientQuery = "DELETE FROM Patients WHERE PatientID = @PatientID";
                    SqlCommand deletePatientCmd = new SqlCommand(deletePatientQuery, conn);
                    deletePatientCmd.Parameters.AddWithValue("@PatientID", PatientID);
                    deletePatientCmd.ExecuteNonQuery();

                    conn.Close();

                    MessageBox.Show("Patient permanently deleted!");

                    // Remove this card
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
