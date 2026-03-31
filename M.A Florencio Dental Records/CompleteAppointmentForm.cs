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
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

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

            // Create prescription item
            string prescText = txtMedication.Text + " (" + dtpPrescDate.Value.ToString("MMM dd, yyyy") + ")";

            // Store in list with instructions
            lstPrescriptions.Items.Add(new PrescriptionData
            {
                Medication = txtMedication.Text,
                PrescriptionDate = dtpPrescDate.Value,
                Instructions = txtMedInstructions.Text ?? ""
            });

            // Clear fields
            txtMedication.Text = "";
            txtMedInstructions.Text = "";
            dtpPrescDate.Value = DateTime.Today;
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

        // ✅ SAVE EVERYTHING
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
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: INSERT Medical Record
                    string recordQuery = @"
                        INSERT INTO MedicalRecords
                        (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                        VALUES
                        (@PatientID, @AppointmentID, @VisitDate, @Diagnosis, @Procedure, @Notes);
                        
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    SqlCommand recordCmd = new SqlCommand(recordQuery, conn);
                    recordCmd.Parameters.AddWithValue("@PatientID", PatientID);
                    recordCmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                    recordCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                    recordCmd.Parameters.AddWithValue("@Diagnosis", txtDiagnosis.Text);
                    recordCmd.Parameters.AddWithValue("@Procedure", txtProcedure.Text);
                    recordCmd.Parameters.AddWithValue("@Notes", txtNotes.Text ?? "");

                    int recordID = (int)recordCmd.ExecuteScalar();

                    // Step 2: INSERT Prescriptions (if any)
                    foreach (PrescriptionData presc in lstPrescriptions.Items)
                    {
                        string prescQuery = @"
                            INSERT INTO Prescription
                            (record_id, medication, prescription_date, med_instructions)
                            VALUES
                            (@RecordID, @Medication, @PrescDate, @Instructions)";

                        SqlCommand prescCmd = new SqlCommand(prescQuery, conn);
                        prescCmd.Parameters.AddWithValue("@RecordID", recordID);
                        prescCmd.Parameters.AddWithValue("@Medication", presc.Medication);
                        prescCmd.Parameters.AddWithValue("@PrescDate", presc.PrescriptionDate);
                        prescCmd.Parameters.AddWithValue("@Instructions", presc.Instructions);

                        prescCmd.ExecuteNonQuery();
                    }

                    // Step 3: UPDATE Appointment Status to "Done"
                    string updateApptQuery = "UPDATE Appointments SET Status = 'Done' WHERE AppointmentID = @AppointmentID";
                    SqlCommand updateApptCmd = new SqlCommand(updateApptQuery, conn);
                    updateApptCmd.Parameters.AddWithValue("@AppointmentID", AppointmentID);
                    updateApptCmd.ExecuteNonQuery();

                    conn.Close();

                    MessageBox.Show("Appointment completed!\nMedical record and prescriptions saved.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
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