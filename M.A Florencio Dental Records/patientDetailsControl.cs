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

namespace M.A_Florencio_Dental_Records
{
    public partial class patientDetailsControl : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int PatientID { get; set; }

        public patientDetailsControl()
        {
            InitializeComponent();
        }

        private void patientDetailsControl_Load(object sender, EventArgs e)
        {
        }

        public void LoadPatientDetails(int patientID)
        {
            PatientID = patientID;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    PlblPatientID.Text = "Patient ID: " + reader["PatientID"].ToString();
                    DlblName.Text = "Full Name: " + reader["FullName"].ToString();
                    PlblGender.Text = "Gender: " + reader["Gender"].ToString();

                    DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);
                    PlblBday.Text = "Birth Date: " + birthDate.ToString("MMM dd, yyyy");

                    PlblAge.Text = "Age: " + reader["Age"].ToString() + " years old";
                    PlblContact.Text = "Contact Number: " + reader["ContactNumber"].ToString();
                    PlblAddress.Text = "Address: " + reader["Address"].ToString();
                    PlblMH.Text = "Medical History: " + reader["MedicalHistory"].ToString();
                }
                else
                {
                    MessageBox.Show("Patient not found!");
                }

                conn.Close();
            }
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            Form1 mainForm = (Form1)this.FindForm();
            mainForm.LoadControl(new patientControl());
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit functionality coming soon!", "Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ArchivePatient()
        {
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

                    btnBack_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}