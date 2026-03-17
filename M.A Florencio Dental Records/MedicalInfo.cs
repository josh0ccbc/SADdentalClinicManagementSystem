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
    public partial class MedicalInfo : UserControl
    {
        public PatientData patientData;
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public MedicalInfo()
        {
            InitializeComponent();
        }

        private void MedicalInfo_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMedicalHistory.Text))
            {
                MessageBox.Show("Medical History is required!");
                txtMedicalHistory.Focus();
                return;
            }

            patientData.MedicalHistory = txtMedicalHistory.Text;
            SavePatientToDatabase();
        }

        private void SavePatientToDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Patients 
                            (FullName, Gender, BirthDate, Age, ContactNumber, Address, MedicalHistory,
                             CivilStatus, Religion, GuardianName, GuardianContact, GuardianRelationship)
                            VALUES
                            (@FullName, @Gender, @BirthDate, @Age, @ContactNumber, @Address, @MedicalHistory,
                             @CivilStatus, @Religion, @GuardianName, @GuardianContact, @GuardianRelationship)";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@FullName", patientData.FullName);
                    cmd.Parameters.AddWithValue("@Gender", patientData.Gender);
                    cmd.Parameters.AddWithValue("@BirthDate", patientData.BirthDate);
                    cmd.Parameters.AddWithValue("@Age", patientData.Age);
                    cmd.Parameters.AddWithValue("@ContactNumber", patientData.ContactNumber);
                    cmd.Parameters.AddWithValue("@Address", patientData.Address);
                    cmd.Parameters.AddWithValue("@MedicalHistory", patientData.MedicalHistory);
                    cmd.Parameters.AddWithValue("@CivilStatus", patientData.CivilStatus ?? "");
                    cmd.Parameters.AddWithValue("@Religion", patientData.Religion ?? "");
                    cmd.Parameters.AddWithValue("@GuardianName", patientData.GuardianName ?? "");
                    cmd.Parameters.AddWithValue("@GuardianContact", patientData.GuardianContact ?? "");
                    cmd.Parameters.AddWithValue("@GuardianRelationship", patientData.GuardianRelationship ?? "");

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Patient registered successfully!");

                    Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
                    if (mainForm != null)
                    {
                        mainForm.Show();
                        mainForm.LoadControl(new DBcontrol());
                    }

                    Form2 form2 = (Form2)this.FindForm();
                    form2.returningToMain = true;
                    form2.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Form2 form2 = (Form2)this.FindForm();
            AddPatientInfo personalInfo = new AddPatientInfo();
            personalInfo.patientData = patientData;  // ✅ Pass the data

            // Restore the fields with saved data
            personalInfo.txtFullName.Text = patientData.FullName;
            personalInfo.cmbGender.Text = patientData.Gender;
            personalInfo.dtpBirthDate.Value = patientData.BirthDate;
            personalInfo.txtAge.Text = patientData.Age.ToString();
            personalInfo.txtContact.Text = patientData.ContactNumber;
            personalInfo.txtAddress.Text = patientData.Address;
            personalInfo.cmbCivilStatus.Text = patientData.CivilStatus;
            personalInfo.txtReligion.Text = patientData.Religion;
            personalInfo.Gname.Text = patientData.GuardianName;
            personalInfo.Gcontact.Text = patientData.GuardianContact;
            personalInfo.Grelationship.Text = patientData.GuardianRelationship;

            form2.LoadControl(personalInfo);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

        }
    }
}