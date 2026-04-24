using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class CompleteAppointmentForm : MaterialForm
    {
        public int AppointmentID { get; set; }
        public AppointmentDetail Appointment { get; set; }
        public int PatientID { get; set; }

        public CompleteAppointmentForm(int appointmentID, AppointmentDetail appointment, int patientID)
        {
            InitializeComponent();
            AppointmentID = appointmentID;
            Appointment = appointment;
            PatientID = patientID;

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnViewMH, "View Medical History");
        }

        private void CompleteAppointmentForm_Load(object sender, EventArgs e)
        {

            dtpVisitDate.Value = Appointment.AppointmentDate;

            LoadProcedureDropdown();

            string timeStr = DateTime.Today.Add(Appointment.AppointmentTime).ToString("hh:mm tt");
            this.Text = $"{Appointment.PatientName} · {Appointment.AppointmentDate:MMM dd} {timeStr} · {Appointment.ServiceName}";
        }

        private void LoadProcedureDropdown()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT ServiceID, ServiceName FROM DentalServices ORDER BY ServiceName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                cmbProcedure.Items.Clear();

                while (reader.Read())
                {
                    cmbProcedure.Items.Add(new ServiceItem
                    {
                        ServiceID = Convert.ToInt32(reader["ServiceID"]),
                        ServiceName = reader["ServiceName"].ToString()
                    });
                }
            }

            cmbProcedure.DisplayMember = "ServiceName";

            // ✅ Auto-select the appointed service
            foreach (ServiceItem item in cmbProcedure.Items)
            {
                if (item.ServiceName == Appointment.ServiceName)
                {
                    cmbProcedure.SelectedItem = item;
                    break;
                }
            }

            // Fallback: select first if none matched
            if (cmbProcedure.SelectedIndex < 0 && cmbProcedure.Items.Count > 0)
                cmbProcedure.SelectedIndex = 0;
        }

        // ✅ ADD PRESCRIPTION BUTTON
        private void btnAddPrescription_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMedication.Text))
            {
                MessageBox.Show("Enter medication name");
                return;
            }

            if (string.IsNullOrEmpty(txtMedInstructions.Text))
            {
                MessageBox.Show("Enter medication instructions");
                return;
            }

            lstPrescriptions.Items.Add(new PrescriptionData
            {
                Medication = txtMedication.Text,
                PrescriptionDate = dtpPrescDate.Value,
                Instructions = txtMedInstructions.Text
            });

            txtMedication.Text = "";
            txtMedInstructions.Text = "";
            dtpPrescDate.Value = DateTime.Today;

            MessageBox.Show("Prescription added!");
        }

        // ✅ REMOVE PRESCRIPTION BUTTON
        private void btnRemovePrescription_Click(object sender, EventArgs e)
        {
            if (lstPrescriptions.SelectedIndex >= 0)
            {
                lstPrescriptions.Items.RemoveAt(lstPrescriptions.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Select a prescription to remove");
            }
        }

        // ✅ SAVE
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                MessageBox.Show("Enter diagnosis");
                return;
            }

            if (cmbProcedure.SelectedItem == null)
            {
                MessageBox.Show("Select a procedure");
                return;
            }

            string selectedProcedure = (cmbProcedure.SelectedItem as ServiceItem)?.ServiceName ?? "";

            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG: Saving appointment");
                System.Diagnostics.Debug.WriteLine($"  PatientID: {PatientID}");
                System.Diagnostics.Debug.WriteLine($"  AppointmentID: {AppointmentID}");

                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection opened");

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine("→ Inserting medical record...");

                            string recordQuery = @"
                        INSERT INTO MedicalRecords
                        (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                        VALUES
                        (@PatientID, @AppointmentID, @VisitDate, @Diagnosis, @Procedure, @Notes);
                        SELECT SCOPE_IDENTITY();";

                            SqlCommand recordCmd = new SqlCommand(recordQuery, conn, trans);
                            recordCmd.Parameters.AddWithValue("@PatientID", PatientID);
                            recordCmd.Parameters.AddWithValue("@AppointmentID",
                                AppointmentID > 0 ? (object)AppointmentID : DBNull.Value);
                            recordCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                            recordCmd.Parameters.AddWithValue("@Diagnosis", CryptoHelper.Encrypt(txtDiagnosis.Text));
                            recordCmd.Parameters.AddWithValue("@Procedure", CryptoHelper.Encrypt(selectedProcedure));
                            recordCmd.Parameters.AddWithValue("@Notes", CryptoHelper.Encrypt(txtNotes.Text ?? ""));

                            // ✅ ExecuteScalar instead of ExecuteReader
                            int recordID = Convert.ToInt32(recordCmd.ExecuteScalar());

                            if (recordID == 0)
                                throw new Exception("Medical record insert failed - no ID returned");

                            System.Diagnostics.Debug.WriteLine($"✅ Medical Record Created: ID={recordID}");

                            // Insert prescriptions
                            if (lstPrescriptions.Items.Count > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"→ Inserting {lstPrescriptions.Items.Count} prescriptions...");

                                foreach (PrescriptionData presc in lstPrescriptions.Items)
                                {
                                    string prescQuery = @"
                                INSERT INTO Prescription
                                (record_id, medication, prescription_date, med_instructions)
                                VALUES
                                (@RecordID, @Medication, @PrescDate, @Instructions)";

                                    SqlCommand prescCmd = new SqlCommand(prescQuery, conn, trans);
                                    prescCmd.Parameters.AddWithValue("@RecordID", recordID);
                                    prescCmd.Parameters.AddWithValue("@Medication", CryptoHelper.Encrypt(presc.Medication));
                                    prescCmd.Parameters.AddWithValue("@PrescDate", presc.PrescriptionDate);
                                    prescCmd.Parameters.AddWithValue("@Instructions", CryptoHelper.Encrypt(presc.Instructions));

                                    prescCmd.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"  ✅ Prescription: {presc.Medication}");
                                }
                            }

                            // Update appointment status to Done
                            System.Diagnostics.Debug.WriteLine("→ Updating appointment status...");

                            string updateApptQuery = "UPDATE Appointments SET Status = 'Done' WHERE AppointmentID = @AppointmentID";
                            SqlCommand updateApptCmd = new SqlCommand(updateApptQuery, conn, trans);
                            updateApptCmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                            updateApptCmd.ExecuteNonQuery();

                            trans.Commit();
                            System.Diagnostics.Debug.WriteLine("✅ TRANSACTION COMMITTED!");

                            MessageBox.Show(
                                $"✅ Appointment completed successfully!\n\n" +
                                $"Medical Record ID: {recordID}\n" +
                                $"Procedure: {selectedProcedure}\n" +
                                $"Prescriptions: {lstPrescriptions.Items.Count}\n\n" +
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
                                errorMsg += "\n\n⚠️ Syntax Error - Check column names";
                            else if (sqlEx.Number == 547)
                                errorMsg += "\n\n⚠️ Foreign Key Constraint - Invalid PatientID or AppointmentID";
                            else if (sqlEx.Number == 515)
                                errorMsg += "\n\n⚠️ NULL in NOT NULL column";
                            else if (sqlEx.Number == 137)
                                errorMsg += "\n\n⚠️ Variable not declared";

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbProcedure_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reserved for future use
        }

        private void txtDiagnosis_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnViewMH_MouseEnter(object sender, EventArgs e)
        {
            btnViewMH.BackgroundImage = Properties.Resources.health_check2;
        }

        private void btnViewMH_MouseLeave(object sender, EventArgs e)
        {
            btnViewMH.BackgroundImage = Properties.Resources.health_check;
        }

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null) return "";
            string value = dbValue.ToString();
            if (string.IsNullOrEmpty(value)) return "";
            try { return CryptoHelper.Decrypt(value); }
            catch { return value; } // fallback for old unencrypted data
        }

        private void btnViewMH_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();

                // ─── Get Age from Patients table ───────────────────────────────
                string patientQuery = "SELECT Age FROM Patients WHERE PatientID = @PatientID";
                SqlCommand patientCmd = new SqlCommand(patientQuery, conn);
                patientCmd.Parameters.AddWithValue("@PatientID", PatientID);
                object ageResult = patientCmd.ExecuteScalar();
                string age = ageResult != null && ageResult != DBNull.Value ? ageResult.ToString() : "N/A";

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
                details += $"Name   : {Appointment.PatientName}\n";
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

        // ✅ PRESCRIPTION DATA CLASS
    public class PrescriptionData
    {
        public int PrescriptionID { get; set; }     // ← add this
        public bool IsExisting { get; set; }         // ← add this
        public string Medication { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Instructions { get; set; }

        public override string ToString() => Medication + " (" + PrescriptionDate.ToString("MMM dd, yyyy") + ")";
    }
}