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
        }

        private void CompleteAppointmentForm_Load(object sender, EventArgs e)
        {
            // Show appointment info
            lblAppointmentInfo.Text = Appointment.AppointmentDate.ToString("MMM dd, yyyy") + " | " +
                                      Appointment.ServiceName + " | " +
                                      Appointment.PatientName;

            // Set visit date to appointment date
            dtpVisitDate.Value = Appointment.AppointmentDate;
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

            // Store in list
            lstPrescriptions.Items.Add(new PrescriptionData
            {
                Medication = txtMedication.Text,
                PrescriptionDate = dtpPrescDate.Value,
                Instructions = txtMedInstructions.Text
            });

            // Clear fields
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

        // ✅ SAVE - WITH ALL FIXES
        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrEmpty(txtDiagnosis.Text))
            {
                MessageBox.Show("Enter diagnosis");
                return;
            }

            if (string.IsNullOrEmpty(txtProcedure.Text))
            {
                MessageBox.Show("Enter procedure");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"DEBUG: Saving appointment");
                System.Diagnostics.Debug.WriteLine($"  PatientID: {PatientID}");
                System.Diagnostics.Debug.WriteLine($"  AppointmentID: {AppointmentID}");

                using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
                {
                    System.Diagnostics.Debug.WriteLine($"CONNECTION: {conn.ConnectionString}");
                    System.Diagnostics.Debug.WriteLine($"STATE BEFORE OPEN: {conn.State}");
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine($"STATE AFTER OPEN: {conn.State}");
                    System.Diagnostics.Debug.WriteLine($"DATABASE: {conn.Database}");
                    System.Diagnostics.Debug.WriteLine($"DATASOURCE: {conn.DataSource}");
                    System.Diagnostics.Debug.WriteLine("✅ Database connection opened");

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // ✅ FIX #1: Use [procedure] with brackets
                            // ✅ FIX #2: Make sure to provide patient_id (it's NOT NULL)
                            System.Diagnostics.Debug.WriteLine("→ Inserting medical record...");

                            string recordQuery = @"
                                INSERT INTO MedicalRecords
                                (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                                OUTPUT INSERTED.record_id
                                VALUES
                                (@PatientID, @AppointmentID, @VisitDate, @Diagnosis, @Procedure, @Notes);";

                            SqlCommand recordCmd = new SqlCommand(recordQuery, conn, trans);
                            recordCmd.Parameters.AddWithValue("@PatientID", PatientID);  // ✅ REQUIRED - not optional
                            recordCmd.Parameters.AddWithValue("@AppointmentID",
                                AppointmentID > 0 ? (object)AppointmentID : DBNull.Value);
                            recordCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                            recordCmd.Parameters.AddWithValue("@Diagnosis", CryptoHelper.Encrypt(txtDiagnosis.Text));
                            recordCmd.Parameters.AddWithValue("@Procedure", CryptoHelper.Encrypt(txtProcedure.Text));  // ✅ Encrypted
                            recordCmd.Parameters.AddWithValue("@Notes", CryptoHelper.Encrypt(txtNotes.Text ?? ""));     // ✅ Encrypted

                            int recordID = 0;

                            using (SqlDataReader reader = recordCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    recordID = reader.GetInt32(0);
                                }
                                reader.Close(); // ✅ Explicitly close reader before next command
                            }

                            if (recordID == 0)
                            {
                                throw new Exception("Medical record insert failed - no ID returned");
                            }

                            System.Diagnostics.Debug.WriteLine($"✅ Medical Record Created: ID={recordID}");

                            // ✅ FIX #3: Declare @RecordID before using it in prescriptions
                            // Insert prescriptions with the recordID we just created
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
                                    prescCmd.Parameters.AddWithValue("@RecordID", recordID);  // ✅ Use the recordID from above
                                    prescCmd.Parameters.AddWithValue("@Medication", CryptoHelper.Encrypt(presc.Medication));
                                    prescCmd.Parameters.AddWithValue("@PrescDate", presc.PrescriptionDate);
                                    prescCmd.Parameters.AddWithValue("@Instructions", CryptoHelper.Encrypt(presc.Instructions));

                                    prescCmd.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"  ✅ Prescription: {presc.Medication}");
                                }
                            }

                            // Update appointment status
                            System.Diagnostics.Debug.WriteLine("→ Updating appointment status...");

                            string updateApptQuery = "UPDATE Appointments SET Status = 'Done' WHERE AppointmentID = @AppointmentID";
                            SqlCommand updateApptCmd = new SqlCommand(updateApptQuery, conn, trans);
                            updateApptCmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                            updateApptCmd.ExecuteNonQuery();

                            // COMMIT
                            trans.Commit();
                            System.Diagnostics.Debug.WriteLine("✅ TRANSACTION COMMITTED!");

                            MessageBox.Show(
                                $"✅ Appointment completed successfully!\n\n" +
                                $"Medical Record ID: {recordID}\n" +
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
                                errorMsg += "\n\n⚠️ Syntax Error - Check column names (use brackets for reserved keywords)";
                            else if (sqlEx.Number == 547)
                                errorMsg += "\n\n⚠️ Foreign Key Constraint - Invalid PatientID or AppointmentID";
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    // ✅ PRESCRIPTION DATA CLASS
    public class PrescriptionData
    {
        public string Medication { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string Instructions { get; set; }

        public override string ToString() => Medication + " (" + PrescriptionDate.ToString("MMM dd, yyyy") + ")";
    }
}