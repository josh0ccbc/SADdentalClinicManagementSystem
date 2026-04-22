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
    public partial class AddPatientInfo : UserControl
    {
        public PatientData patientData;

        public AddPatientInfo()
        {
            InitializeComponent();
            patientData = new PatientData();
        }

        private void AddPatientInfo_Load(object sender, EventArgs e)
        {
            // ✅ Removed KeyDown — using ProcessCmdKey instead
        }

        // ✅ Intercepts Enter key at UserControl level before it bubbles up
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnNext_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ─── Safe encrypt with field-level error detection ────────────────────────
        private string SafeEncrypt(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            string encrypted = CryptoHelper.Encrypt(value);

            if (string.IsNullOrEmpty(encrypted))
            {
                throw new Exception(
                    $"Encryption failed for field '{fieldName}'.\n\n" +
                    "This usually means the encryption key is missing or inaccessible.\n\n" +
                    "Go to Admin Panel → Security → Import Key Backup and try again.");
            }

            return encrypted;
        }

        // ─── Safe decrypt helper ──────────────────────────────────────────────────
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
                return value;
            }
        }

        // ─── Age calculation ──────────────────────────────────────────────────────
        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        // ─── Birth date changed ───────────────────────────────────────────────────
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBirthDate.Value > DateTime.Today)
            {
                MessageBox.Show("Birth date cannot be in the future!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpBirthDate.Value = DateTime.Today.AddYears(-18);
                return;
            }

            int age = CalculateAge(dtpBirthDate.Value);
            txtAge.Text = age.ToString();

            bool isMinor = age < 18;
            label10.Visible = isMinor;
            Gname.Visible = isMinor;
            label11.Visible = isMinor;
            Gcontact.Visible = isMinor;
            label12.Visible = isMinor;
            Grelationship.Visible = isMinor;
            label13.Visible = isMinor;

            if (!isMinor)
            {
                Gname.Text = "";
                Gcontact.Text = "";
                Grelationship.Text = "";
            }
        }

        // ─── Form validation ──────────────────────────────────────────────────────
        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbGender.Text))
            {
                MessageBox.Show("Gender is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Contact Number is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (!long.TryParse(txtContact.Text, out _))
            {
                MessageBox.Show("Contact must be numbers only!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (txtContact.Text.Length < 10 || txtContact.Text.Length > 11)
            {
                MessageBox.Show("Contact must be 10-11 digits!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbCivilStatus.Text))
            {
                MessageBox.Show("Civil Status is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCivilStatus.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtReligion.Text))
            {
                MessageBox.Show("Religion is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReligion.Focus();
                return false;
            }

            if (int.TryParse(txtAge.Text, out int age) && age < 18)
            {
                if (string.IsNullOrWhiteSpace(Gname.Text))
                {
                    MessageBox.Show("Guardian Name is required for patients under 18!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gname.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Gcontact.Text))
                {
                    MessageBox.Show("Guardian Contact is required for patients under 18!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gcontact.Focus();
                    return false;
                }

                if (!long.TryParse(Gcontact.Text, out _))
                {
                    MessageBox.Show("Guardian Contact must be numbers only!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gcontact.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Grelationship.Text))
                {
                    MessageBox.Show("Guardian Relationship is required for patients under 18!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Grelationship.Focus();
                    return false;
                }
            }

            return true;
        }

        // ─── Back / Cancel button ─────────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = (Form2)this.FindForm();
            form2.returningToMain = true;

            Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.Show();
                mainForm.LoadControl(new DBcontrol());
            }

            form2.Close();
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button4;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button3;
        }

        // ─── Load existing patient info (for edit scenarios) ──────────────────────
        private void LoadPatientInfo(int patientID)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtFullName.Text = reader["FullName"].ToString();

                    string gender = SafeDecrypt(reader["Gender"]);
                    cmbGender.SelectedItem = gender;

                    dtpBirthDate.Value = Convert.ToDateTime(reader["BirthDate"]);
                    txtAge.Text = reader["Age"].ToString();

                    txtContact.Text = SafeDecrypt(reader["ContactNumber"]);
                    txtAddress.Text = SafeDecrypt(reader["Address"]);

                    string civilStatus = SafeDecrypt(reader["CivilStatus"]);
                    cmbCivilStatus.SelectedItem = civilStatus;

                    txtReligion.Text = SafeDecrypt(reader["Religion"]);

                    Gname.Text = SafeDecrypt(reader["GuardianName"]);
                    Gcontact.Text = SafeDecrypt(reader["GuardianContact"]);
                    Grelationship.Text = SafeDecrypt(reader["GuardianRelationship"]);
                }

                reader.Close();
            }

            dateTimePicker1_ValueChanged(null, null);
        }

        // ─── Next button — validate, save patient, open medical history ───────────
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            patientData.FullName = txtFullName.Text.Trim();
            patientData.Gender = cmbGender.Text.Trim();
            patientData.BirthDate = dtpBirthDate.Value;
            patientData.Age = int.Parse(txtAge.Text);
            patientData.ContactNumber = txtContact.Text.Trim();
            patientData.Address = txtAddress.Text.Trim();
            patientData.CivilStatus = cmbCivilStatus.Text.Trim();
            patientData.Religion = txtReligion.Text.Trim();
            patientData.GuardianName = Gname.Text.Trim();
            patientData.GuardianContact = Gcontact.Text.Trim();
            patientData.GuardianRelationship = Grelationship.Text.Trim();

            int newPatientID = SavePatientToDB();

            if (newPatientID <= 0)
                return;

            Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            Form2 form2 = (Form2)this.FindForm();

            form2.Hide();

            using (MedicalHistoryForm mhForm = new MedicalHistoryForm(newPatientID))
            {
                mhForm.ShowDialog(mainForm);
            }

            if (mainForm != null)
            {
                mainForm.LoadControl(new DBcontrol());
                mainForm.Show();
                mainForm.WindowState = FormWindowState.Normal;
                mainForm.BringToFront();
            }

            form2.returningToMain = true;
            form2.Close();
        }

        // ─── Save patient to DB with encryption guard ─────────────────────────────
        private int SavePatientToDB()
        {
            try
            {
                string encGender = SafeEncrypt(cmbGender.Text.Trim(), "Gender");
                string encContact = SafeEncrypt(txtContact.Text.Trim(), "Contact Number");
                string encAddress = SafeEncrypt(txtAddress.Text.Trim(), "Address");
                string encCivilStatus = SafeEncrypt(cmbCivilStatus.Text.Trim(), "Civil Status");
                string encReligion = SafeEncrypt(txtReligion.Text.Trim(), "Religion");
                string encGuardianName = SafeEncrypt(Gname.Text.Trim(), "Guardian Name");
                string encGuardianContact = SafeEncrypt(Gcontact.Text.Trim(), "Guardian Contact");
                string encGuardianRelationship = SafeEncrypt(Grelationship.Text.Trim(), "Guardian Relationship");

                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"
                        INSERT INTO Patients 
                            (FullName, Gender, BirthDate, Age, ContactNumber, Address,
                             CivilStatus, Religion, GuardianName, GuardianContact,
                             GuardianRelationship, DateRegistered, IsArchived)
                        VALUES
                            (@FullName, @Gender, @BirthDate, @Age, @ContactNumber, @Address,
                             @CivilStatus, @Religion, @GuardianName, @GuardianContact,
                             @GuardianRelationship, @DateRegistered, 0);

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gender", encGender);
                    cmd.Parameters.AddWithValue("@BirthDate", dtpBirthDate.Value.Date);
                    cmd.Parameters.AddWithValue("@Age", int.Parse(txtAge.Text));
                    cmd.Parameters.AddWithValue("@ContactNumber", encContact);
                    cmd.Parameters.AddWithValue("@Address", encAddress);
                    cmd.Parameters.AddWithValue("@CivilStatus", encCivilStatus);
                    cmd.Parameters.AddWithValue("@Religion", encReligion);
                    cmd.Parameters.AddWithValue("@GuardianName", encGuardianName);
                    cmd.Parameters.AddWithValue("@GuardianContact", encGuardianContact);
                    cmd.Parameters.AddWithValue("@GuardianRelationship", encGuardianRelationship);
                    cmd.Parameters.AddWithValue("@DateRegistered", DateTime.Now);

                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        MessageBox.Show(
                            "Patient was not saved — the database did not return a new ID.\n\n" +
                            "Check that the Patients table exists and the connection is valid.",
                            "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }

                    return (int)result;
                }
            }
            catch (Exception ex) when (ex.Message.Contains("Encryption failed"))
            {
                MessageBox.Show(ex.Message, "Encryption Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show(
                    "Database error while saving patient:\n\n" + sqlEx.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unexpected error saving patient:\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        // ─── Unused stub (keep for designer compatibility) ────────────────────────
        private void txtFullName_TextChanged(object sender, EventArgs e) { }
    }
}