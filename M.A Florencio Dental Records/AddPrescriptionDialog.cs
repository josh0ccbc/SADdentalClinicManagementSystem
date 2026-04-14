using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class AddPrescriptionDialog : MaterialForm
    {
        public int PatientID { get; set; }

        public AddPrescriptionDialog(int patientID)
        {
            InitializeComponent();
            PatientID = patientID;
        }

        private void AddPrescriptionDialog_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            dtpVisitDate.Value = DateTime.Today;
        }

        // ✅ SAVE PRESCRIPTION BUTTON - WITH TRANSACTION & PROPER ENCRYPTION
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                MessageBox.Show("Enter diagnosis!");
                return;
            }
            if (string.IsNullOrEmpty(txtProcedure.Text))
            {
                MessageBox.Show("Enter procedure!");
                return;
            }
            if (string.IsNullOrEmpty(txtMedication.Text))
            {
                MessageBox.Show("Enter medication!");
                return;
            }
            if (string.IsNullOrEmpty(txtInstructions.Text))
            {
                MessageBox.Show("Enter medication instructions!");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG: Saving standalone prescription");
                System.Diagnostics.Debug.WriteLine($"  PatientID: {PatientID}");

                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    System.Diagnostics.Debug.WriteLine($"CONNECTION: {conn.ConnectionString}");
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection opened");

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // Step 1: Create MedicalRecord (standalone - no appointment)
                            System.Diagnostics.Debug.WriteLine("→ Inserting medical record...");

                            string recordQuery = @"
                                INSERT INTO MedicalRecords 
                                (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                                OUTPUT INSERTED.record_id
                                VALUES 
                                (@PatientID, NULL, @VisitDate, @Diagnosis, @Procedure, @Notes);";

                            SqlCommand recordCmd = new SqlCommand(recordQuery, conn, trans);
                            recordCmd.Parameters.AddWithValue("@PatientID", PatientID);
                            recordCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                            recordCmd.Parameters.AddWithValue("@Diagnosis", CryptoHelper.Encrypt(txtDiagnosis.Text));
                            recordCmd.Parameters.AddWithValue("@Procedure", CryptoHelper.Encrypt(txtProcedure.Text));
                            recordCmd.Parameters.AddWithValue("@Notes", CryptoHelper.Encrypt(txtNotes.Text ?? ""));

                            int newRecordID = 0;

                            using (SqlDataReader reader = recordCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    newRecordID = reader.GetInt32(0);
                                }
                                reader.Close(); // ✅ Explicitly close reader before next command
                            }

                            if (newRecordID == 0)
                            {
                                throw new Exception("Medical record insert failed - no ID returned");
                            }

                            System.Diagnostics.Debug.WriteLine($"✅ Medical Record Created: ID={newRecordID}");

                            // Step 2: Insert Prescription linked to that record
                            System.Diagnostics.Debug.WriteLine("→ Inserting prescription...");

                            string prescQuery = @"
                                INSERT INTO Prescription 
                                (record_id, medication, prescription_date, med_instructions)
                                VALUES 
                                (@RecordID, @Medication, @PrescDate, @Instructions)";

                            SqlCommand prescCmd = new SqlCommand(prescQuery, conn, trans);
                            prescCmd.Parameters.AddWithValue("@RecordID", newRecordID);
                            prescCmd.Parameters.AddWithValue("@Medication", CryptoHelper.Encrypt(txtMedication.Text));
                            prescCmd.Parameters.AddWithValue("@PrescDate", dtpVisitDate.Value.Date);
                            prescCmd.Parameters.AddWithValue("@Instructions", CryptoHelper.Encrypt(txtInstructions.Text));

                            prescCmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine($"✅ Prescription: {txtMedication.Text}");

                            // COMMIT
                            trans.Commit();
                            System.Diagnostics.Debug.WriteLine("✅ TRANSACTION COMMITTED!");

                            MessageBox.Show(
                                $"✅ Prescription saved successfully!\n\n" +
                                $"Medical Record ID: {newRecordID}\n\n" +
                                $"All data saved to database.",
                                "Success",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (SqlException sqlEx)
                        {
                            trans.Rollback();
                            System.Diagnostics.Debug.WriteLine($"❌ SQL Error #{sqlEx.Number}: {sqlEx.Message}");

                            string errorMsg = $"SQL Error #{sqlEx.Number}\n\n{sqlEx.Message}";

                            if (sqlEx.Number == 156)
                                errorMsg += "\n\n⚠️ Syntax Error - Check column names (use brackets for reserved keywords)";
                            else if (sqlEx.Number == 547)
                                errorMsg += "\n\n⚠️ Foreign Key Constraint - Invalid PatientID";
                            else if (sqlEx.Number == 515)
                                errorMsg += "\n\n⚠️ NULL in NOT NULL column - patient_id is required!";
                            else if (sqlEx.Number == 137)
                                errorMsg += "\n\n⚠️ Variable not declared - Check SQL syntax";

                            MessageBox.Show(errorMsg, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                            MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Outer Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtMedication_TextChanged(object sender, EventArgs e)
        {

        }
    }
}