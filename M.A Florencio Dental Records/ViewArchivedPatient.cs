using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public partial class ViewArchivedPatient : UserControl
    {
        public int PatientID { get; set; }

        public ViewArchivedPatient()
        {
            InitializeComponent();
        }

        private void ViewArchivedPatient_Load(object sender, EventArgs e)
        {
        }

        public void LoadPatientDetails(int patientID)
        {
            PatientID = patientID;

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblPatientID.Text = "Patient ID: " + reader["PatientID"].ToString();
                    lblFullName.Text = "Full Name: " + reader["FullName"].ToString();
                    lblGender.Text = "Gender: " + reader["Gender"].ToString();

                    DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);
                    lblBirthDate.Text = "Birth Date: " + birthDate.ToString("MMM dd, yyyy");

                    lblAge.Text = "Age: " + reader["Age"].ToString() + " years old";
                    lblContact.Text = "Contact: " + reader["ContactNumber"].ToString();
                    lblAddress.Text = "Address: " + reader["Address"].ToString();

                    string civilStatus = reader["CivilStatus"].ToString();
                    lblCivilStatus.Text = "Civil Status: " + (string.IsNullOrEmpty(civilStatus) ? "N/A" : civilStatus);

                    string religion = reader["Religion"].ToString();
                    lblReligion.Text = "Religion: " + (string.IsNullOrEmpty(religion) ? "N/A" : religion);

                    string guardianName = reader["GuardianName"].ToString();
                    if (!string.IsNullOrEmpty(guardianName))
                    {
                        lblGuardianName.Text = "Guardian Name: " + guardianName;
                        lblGuardianContact.Text = "Guardian Contact: " + reader["GuardianContact"].ToString();
                        lblGuardianRelationship.Text = "Guardian Relationship: " + reader["GuardianRelationship"].ToString();
                        panelGuardian.Visible = true;
                    }
                    else
                    {
                        panelGuardian.Visible = false;
                    }

                    lblMedicalHistory.Text = "Medical History: " + reader["MedicalHistory"].ToString();
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
            mainForm.LoadControl(new Archive());
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
                using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
                {
                    string query = "UPDATE Patients SET IsArchived = 0 WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Patient restored to active records!");

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