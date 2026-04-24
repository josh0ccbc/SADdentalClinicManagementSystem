using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class ArchivePatients : UserControl
    {
        public int PatientID;

        public ArchivePatients()
        {
            InitializeComponent();
            lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnUnarchive, "Unarchive Patient");
            toolTip.SetToolTip(btnDelete, "Delete Patient");
            toolTip.SetToolTip(btnView, "View Patient Full Information");
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Patient ID: " + PatientID);
            btnView.BackgroundImage = Properties.Resources.view;
        }

        private string GetCurrentUserRole()
        {
            return LoginForm.CurrentUserRole;
        }

        public void SetPatient(string PatientID, string name, string gender, int age, string contact)
        {
            this.PatientID = Convert.ToInt32(PatientID);
            lblName.Text = name;
            lblContact.Text = SafeDecrypt(contact);
        }

        private void ArchivePatients_Load(object sender, EventArgs e)
        {

        }

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null)
                return "";

            string value = dbValue.ToString();

            if (string.IsNullOrEmpty(value))
                return "";

            try
            {
                return CryptoHelper.Decrypt(value);
            }
            catch
            {
                // If decryption fails, return the value as-is (for old unencrypted data)
                return value;
            }
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
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    // Set IsArchived back to 0
                    string query = "UPDATE Patients SET IsArchived = 0 WHERE PatientID = @PatientID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Patient restored to active records!");

                    // Refresh archive list
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Are you sure you want to PERMANENTLY DELETE this patient?\n\nThis action CANNOT be undone!",
        "Permanent Delete",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            DialogResult confirm = MessageBox.Show(
                "All patient data and appointments will be deleted.\n\nAre you absolutely sure?",
                "Confirm Permanent Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            // ✅ If Staff, require admin password before deleting
            if (GetCurrentUserRole() == "Staff")
            {
                AdminPasswordDialog adminPrompt = new AdminPasswordDialog();
                if (adminPrompt.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Delete cancelled. Admin authorization required.",
                        "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // ✅ Proceed with deletion
            PermanentlyDeletePatient();
        }

        private void PermanentlyDeletePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();

                    // 1. Delete Prescriptions linked to this patient's medical records
                    string deletePrescriptions = @"
                DELETE FROM Prescription 
                WHERE record_id IN (
                    SELECT record_id FROM MedicalRecords WHERE patient_id = @PatientID
                )";
                    new SqlCommand(deletePrescriptions, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 2. Delete Medical Records
                    string deleteMedical = "DELETE FROM MedicalRecords WHERE patient_id = @PatientID";
                    new SqlCommand(deleteMedical, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 3. Delete Medical History
                    string deleteHistory = "DELETE FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                    new SqlCommand(deleteHistory, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 4. Delete Appointments  ← THIS WAS MISSING, CAUSING YOUR ERROR
                    string deleteAppointments = "DELETE FROM Appointments WHERE PatientID = @PatientID";
                    new SqlCommand(deleteAppointments, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    // 5. Finally, delete the Patient
                    string deletePatient = "DELETE FROM Patients WHERE PatientID = @PatientID";
                    new SqlCommand(deletePatient, conn) { Parameters = { new SqlParameter("@PatientID", PatientID) } }.ExecuteNonQuery();

                    conn.Close();
                    MessageBox.Show("Patient permanently deleted!");
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUnarchive_MouseEnter(object sender, EventArgs e)
        {
            btnUnarchive.BackgroundImage = Properties.Resources.outbox2;
        }

        private void btnUnarchive_MouseLeave(object sender, EventArgs e)
        {
            btnUnarchive.BackgroundImage = Properties.Resources.outbox;
        }

        private void btnView_MouseEnter(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view2;
        }

        private void btnView_MouseLeave(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view;
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.bin2;
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.bin;
        }

        private void btnView_Click_1(object sender, EventArgs e)
        {
            Form1 mainForm = (Form1)this.FindForm();
            patientDetailsControl detailsControl = new patientDetailsControl();
            detailsControl.IsArchived = true;  
            detailsControl.LoadPatientDetails(this.PatientID);
            mainForm.LoadControl(detailsControl);
        }
    }
}
