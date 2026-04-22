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