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
    public partial class Patient : UserControl
    {
        public int PatientID;

        public Patient()
        {
            InitializeComponent();
            lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(btnView, "View Patient Full Information");
            toolTip.SetToolTip(BTNedit, "Edit Patient Information");
            toolTip.SetToolTip(btnDelete, "Archive Patient");
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

        private void Patient_Load(object sender, EventArgs e)
        {
        }

        public void SetPatient(DateTime BirthDate, string PatientID, string name, string gender, int age, string contact)
        {
            this.PatientID = int.Parse(PatientID);
            DPpatientid.Text = PatientID;
            lblName.Text = name;
            lblAge.Text = age + "";
            lblGenderAge.Text = SafeDecrypt(gender);    // ✅ Decrypt
            lblContact.Text = SafeDecrypt(contact);      // ✅ Decrypt
            BDAYage.Text = BirthDate.ToString("MMM dd, yyyy");
        }

        // FIXED: Correct way to access parent Form and LoadControl
        private void btnView_Click(object sender, EventArgs e)
        {
            // Get the main Form1
            Form1 mainForm = (Form1)this.FindForm();

            // Create the details control
            patientDetailsControl details = new patientDetailsControl();

            // Load patient details
            details.LoadPatientDetails(this.PatientID);

            // Load the control into Form1's panel
            mainForm.LoadControl(details);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // ✅ If Staff, require admin password before editing
            if (LoginForm.CurrentUserRole == "Staff")
            {
                AdminPasswordDialog adminPrompt = new AdminPasswordDialog();
                if (adminPrompt.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("Edit cancelled. Admin authorization required.",
                        "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // ✅ Proceed with edit
            EditPatientForm editForm = new EditPatientForm(PatientID);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                ReloadPatientCard();
                MessageBox.Show("Patient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ✅ NEW METHOD - RELOAD PATIENT DATA ON THIS CARD
        private void ReloadPatientCard()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // ✅ REFRESH ALL FIELDS ON THIS CARD
                        lblName.Text = reader["FullName"].ToString();
                        lblGenderAge.Text = SafeDecrypt(reader["Gender"].ToString()); // ✅ was missing SafeDecrypt
                        lblAge.Text = reader["Age"].ToString();
                        lblContact.Text = SafeDecrypt(reader["ContactNumber"]);
                        BDAYage.Text = Convert.ToDateTime(reader["BirthDate"]).ToString("MMM dd, yyyy");
                        DPpatientid.Text = reader["PatientID"].ToString();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reloading patient: " + ex.Message);
            }
        }

        private void btnView_MouseEnter(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view2;
        }

        private void btnView_MouseLeave(object sender, EventArgs e)
        {
            btnView.BackgroundImage = Properties.Resources.view;
        }

        private void ArchivePatient()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "UPDATE Patients SET IsArchived = 1 WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Patient archived successfully!");

                    Form1 mainForm = (Form1)this.FindForm();
                    mainForm.LoadControl(new patientControl());
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
                "Archive this patient record?",
                "Archive Patient",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ArchivePatient();
            }
        }

        private void btnDelete_MouseEnter(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.archive2;
        }

        private void btnDelete_MouseLeave(object sender, EventArgs e)
        {
            btnDelete.BackgroundImage = Properties.Resources.archive;
        }

        private void lblGenderAge_Click(object sender, EventArgs e)
        {
        }

        private void BTNedit_MouseEnter(object sender, EventArgs e)
        {
            BTNedit.BackgroundImage = Properties.Resources.edit2;
        }

        private void BTNedit_MouseLeave(object sender, EventArgs e)
        {
            BTNedit.BackgroundImage = Properties.Resources.edit;
        }
    }
}