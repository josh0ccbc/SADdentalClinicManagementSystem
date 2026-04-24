using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Collections.Generic;

namespace M.A_Florencio_Dental_Records
{
    public partial class AddPrescriptionDialog : MaterialForm
    {
        public int PatientID { get; set; }

        public AddPrescriptionDialog(int patientID)
        {
            InitializeComponent();
            PatientID = patientID;

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(button1, "View Medical History");
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

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.health_check2;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.health_check;
        }

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null) return "";
            string value = dbValue.ToString();
            if (string.IsNullOrEmpty(value)) return "";
            try { return CryptoHelper.Decrypt(value); }
            catch { return value; } // fallback for old unencrypted data
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();

                // ─── Get Name and Age from Patients table ──────────────────────
                string patientQuery = "SELECT FullName, Age FROM Patients WHERE PatientID = @PatientID";
                SqlCommand patientCmd = new SqlCommand(patientQuery, conn);
                patientCmd.Parameters.AddWithValue("@PatientID", PatientID);
                SqlDataReader patientReader = patientCmd.ExecuteReader();

                string patientName = "Unknown";
                string age = "N/A";

                if (patientReader.Read())
                {
                    patientName = patientReader["FullName"].ToString();
                    age = patientReader["Age"].ToString();
                }
                patientReader.Close();

                // ─── Get Medical History ────────────────────────────────────────
                string mhQuery = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand mhCmd = new SqlCommand(mhQuery, conn);
                mhCmd.Parameters.AddWithValue("@PatientID", PatientID);
                SqlDataReader reader = mhCmd.ExecuteReader();

                int lineWidth = 45;
                string divider = new string('═', lineWidth);
                string thin = new string('─', lineWidth);

                string details = "";
                details += divider + "\n";
                details += "       PATIENT MEDICAL HISTORY\n";
                details += divider + "\n";
                details += $"Name   : {patientName}\n";   // ✅ Fixed
                details += $"Age    : {age}\n";
                details += thin + "\n";

                if (!reader.Read())
                {
                    details += "No medical history recorded.\n";
                    details += divider;
                    MessageBox.Show(details, "Medical History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ─── General Health ─────────────────────────────────────────────
                details += "GENERAL HEALTH:\n";
                var generalHealth = new List<string>();
                if (reader["is_healthy"] != DBNull.Value && Convert.ToBoolean(reader["is_healthy"]))
                    generalHealth.Add("  ✓ Good Health");
                if (reader["under_treatment"] != DBNull.Value && Convert.ToBoolean(reader["under_treatment"]))
                {
                    generalHealth.Add("  ✓ Under Medical Treatment");
                    string d = SafeDecrypt(reader["treatment_details"]);
                    if (!string.IsNullOrEmpty(d)) generalHealth.Add("    Details: " + d);
                }
                if (reader["serious_illness"] != DBNull.Value && Convert.ToBoolean(reader["serious_illness"]))
                {
                    generalHealth.Add("  ✓ Serious Illness");
                    string d = SafeDecrypt(reader["illness_details"]);
                    if (!string.IsNullOrEmpty(d)) generalHealth.Add("    Details: " + d);
                }
                if (reader["recently_hospitalized"] != DBNull.Value && Convert.ToBoolean(reader["recently_hospitalized"]))
                {
                    generalHealth.Add("  ✓ Recently Hospitalized");
                    string d = SafeDecrypt(reader["hospitalization_details"]);
                    if (!string.IsNullOrEmpty(d)) generalHealth.Add("    Details: " + d);
                }
                details += generalHealth.Count > 0 ? string.Join("\n", generalHealth) + "\n" : "  None recorded\n";
                details += thin + "\n";

                // ─── Allergies ──────────────────────────────────────────────────
                details += "⚠ ALLERGIES:\n";
                var allergies = new List<string>();
                if (reader["LocalAestheticAllergy"] != DBNull.Value && Convert.ToBoolean(reader["LocalAestheticAllergy"])) allergies.Add("  • Local Anesthetic");
                if (reader["PenicillinAllergy"] != DBNull.Value && Convert.ToBoolean(reader["PenicillinAllergy"])) allergies.Add("  • Penicillin");
                if (reader["SulfaAllergy"] != DBNull.Value && Convert.ToBoolean(reader["SulfaAllergy"])) allergies.Add("  • Sulfa");
                if (reader["AspirinAllergy"] != DBNull.Value && Convert.ToBoolean(reader["AspirinAllergy"])) allergies.Add("  • Aspirin");
                if (reader["LatexAllergy"] != DBNull.Value && Convert.ToBoolean(reader["LatexAllergy"])) allergies.Add("  • Latex");
                string otherAllergies = SafeDecrypt(reader["OtherAllergies"]);
                if (!string.IsNullOrEmpty(otherAllergies)) allergies.Add("  • " + otherAllergies);
                details += allergies.Count > 0 ? string.Join("\n", allergies) + "\n" : "  None\n";
                details += thin + "\n";

                // ─── Medications ────────────────────────────────────────────────
                details += "MEDICATIONS:\n";
                bool takingMeds = reader["TakingPrescriptionMeds"] != DBNull.Value && Convert.ToBoolean(reader["TakingPrescriptionMeds"]);
                details += $"  Taking Prescription Meds: {(takingMeds ? "Yes" : "No")}\n";
                string medList = SafeDecrypt(reader["MedicationList"]);
                details += $"  List: {(string.IsNullOrEmpty(medList) ? "None" : medList)}\n";
                details += thin + "\n";

                // ─── Substances ─────────────────────────────────────────────────
                details += "SUBSTANCES:\n";
                var substances = new List<string>();
                if (reader["UsesTobacco"] != DBNull.Value && Convert.ToBoolean(reader["UsesTobacco"])) substances.Add("  • Tobacco");
                if (reader["UsesAlcoholDrugs"] != DBNull.Value && Convert.ToBoolean(reader["UsesAlcoholDrugs"])) substances.Add("  • Alcohol/Drugs");
                details += substances.Count > 0 ? string.Join("\n", substances) + "\n" : "  None\n";
                details += thin + "\n";

                // ─── Medical Conditions ─────────────────────────────────────────
                details += "MEDICAL CONDITIONS:\n";
                var conditions = new List<string>();
                if (reader["HighBP"] != DBNull.Value && Convert.ToBoolean(reader["HighBP"])) conditions.Add("  • High Blood Pressure");
                if (reader["LowBP"] != DBNull.Value && Convert.ToBoolean(reader["LowBP"])) conditions.Add("  • Low Blood Pressure");
                if (reader["HeartDisease"] != DBNull.Value && Convert.ToBoolean(reader["HeartDisease"])) conditions.Add("  • Heart Disease");
                if (reader["HeartMurmur"] != DBNull.Value && Convert.ToBoolean(reader["HeartMurmur"])) conditions.Add("  • Heart Murmur");
                if (reader["Diabetes"] != DBNull.Value && Convert.ToBoolean(reader["Diabetes"])) conditions.Add("  • Diabetes");
                if (reader["Thyroid"] != DBNull.Value && Convert.ToBoolean(reader["Thyroid"])) conditions.Add("  • Thyroid Problem");
                if (reader["Asthma"] != DBNull.Value && Convert.ToBoolean(reader["Asthma"])) conditions.Add("  • Asthma");
                if (reader["RespiratoryProblems"] != DBNull.Value && Convert.ToBoolean(reader["RespiratoryProblems"])) conditions.Add("  • Respiratory Problems");
                if (reader["Arthritis"] != DBNull.Value && Convert.ToBoolean(reader["Arthritis"])) conditions.Add("  • Arthritis");
                if (reader["KidneyDisease"] != DBNull.Value && Convert.ToBoolean(reader["KidneyDisease"])) conditions.Add("  • Kidney Disease");
                details += conditions.Count > 0 ? string.Join("\n", conditions) + "\n" : "  None\n";
                details += thin + "\n";

                // ─── Blood Type ─────────────────────────────────────────────────
                string bloodType = SafeDecrypt(reader["BloodType"]);
                details += $"BLOOD TYPE: {(string.IsNullOrEmpty(bloodType) ? "Not recorded" : bloodType)}\n";
                details += divider;

                MessageBox.Show(details, "Medical History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}