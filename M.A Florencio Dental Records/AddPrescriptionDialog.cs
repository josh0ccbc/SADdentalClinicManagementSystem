using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class AddPrescriptionDialog : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int PatientID { get; set; }

        public AddPrescriptionDialog(int patientID)
        {
            InitializeComponent();
            PatientID = patientID;
        }

        private void AddPrescriptionDialog_Load(object sender, EventArgs e)
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500, Primary.Teal700, Primary.Teal200, Accent.Teal200, TextShade.WHITE);

            this.StartPosition = FormStartPosition.CenterScreen;
            dtpVisitDate.Value = DateTime.Today;
        }

        // ✅ SAVE PRESCRIPTION BUTTON
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                MessageBox.Show("Enter diagnosis!"); return;
            }
            if (string.IsNullOrEmpty(txtProcedure.Text))
            {
                MessageBox.Show("Enter procedure!"); return;
            }
            if (string.IsNullOrEmpty(txtMedication.Text))
            {
                MessageBox.Show("Enter medication!"); return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: Create proper MedicalRecord
                    string recordQuery = @"
                INSERT INTO MedicalRecords 
                (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                VALUES 
                (@PatientID, NULL, @VisitDate, @Diagnosis, @Procedure, @Notes);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                    SqlCommand recordCmd = new SqlCommand(recordQuery, conn);
                    recordCmd.Parameters.AddWithValue("@PatientID", PatientID);
                    recordCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                    recordCmd.Parameters.AddWithValue("@Diagnosis", txtDiagnosis.Text);
                    recordCmd.Parameters.AddWithValue("@Procedure", txtProcedure.Text);
                    recordCmd.Parameters.AddWithValue("@Notes", txtNotes.Text ?? "");

                    int newRecordID = (int)recordCmd.ExecuteScalar();

                    // Step 2: Insert Prescription linked to that record
                    string prescQuery = @"
                INSERT INTO Prescription 
                (record_id, medication, prescription_date, med_instructions)
                VALUES 
                (@RecordID, @Medication, @PrescDate, @Instructions)";

                    SqlCommand prescCmd = new SqlCommand(prescQuery, conn);
                    prescCmd.Parameters.AddWithValue("@RecordID", newRecordID);
                    prescCmd.Parameters.AddWithValue("@Medication", txtMedication.Text);
                    prescCmd.Parameters.AddWithValue("@PrescDate", dtpVisitDate.Value.Date);
                    prescCmd.Parameters.AddWithValue("@Instructions", txtInstructions.Text ?? "");
                    prescCmd.ExecuteNonQuery();

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(txtMedication.Text))
            {
                MessageBox.Show("Enter medication name!");
                return false;
            }

            if (string.IsNullOrEmpty(txtInstructions.Text))
            {
                MessageBox.Show("Enter medication instructions!");
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}