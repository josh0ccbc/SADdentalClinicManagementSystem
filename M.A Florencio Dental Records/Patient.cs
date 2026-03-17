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

        public void SetPatient(DateTime BirthDate, string PatientID, string name, string gender, int age, string contact)
        {
            this.PatientID = int.Parse(PatientID);
            DPpatientid.Text = PatientID;
            lblName.Text = name;
            lblAge.Text = age + "";
            lblGenderAge.Text = gender;
            lblContact.Text = contact;
            BDAYage.Text = BirthDate.ToString("MMM dd, yyyy");
        }

        // FIXED: Correct way to access parent Form and LoadControl
        private void btnView_Click(object sender, EventArgs e)
        {
            // Get the main Form1
            Form1 mainForm = (Form1)this.FindForm();

            // Create the details control
            patientDetailsControl details = new patientDetailsControl();

            // Load patient details
            details.LoadPatientDetails(this.PatientID);

            // Load the control into Form1's panel
            mainForm.LoadControl(details);
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
                    string query = "UPDATE Patients SET IsArchived = 1 WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Patient archived successfully!");

                    Form1 mainForm = (Form1)this.FindForm();
                    mainForm.LoadControl(new patientControl());
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

        private void lblGenderAge_Click(object sender, EventArgs e)
        {
        }

        private void BTNedit_MouseEnter(object sender, EventArgs e)
        {
            BTNedit.BackgroundImage = Properties.Resources.edit2;
        }

        private void BTNedit_MouseLeave(object sender, EventArgs e)
        {
            BTNedit.BackgroundImage = Properties.Resources.edit;
        }
    }
}