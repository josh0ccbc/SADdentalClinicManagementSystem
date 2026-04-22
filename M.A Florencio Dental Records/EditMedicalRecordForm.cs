using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class EditMedicalRecordForm : MaterialForm
    {
        private int _recordID;

        public EditMedicalRecordForm(int recordID)
        {
            InitializeComponent();
            _recordID = recordID;
        }

        private void EditMedicalRecordForm_Load(object sender, EventArgs e)
        {
            LoadServices(); 
            LoadRecord();
            LoadExistingPrescriptions();
            SetFormTitle();
        }

        private void SetFormTitle()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT p.FullName 
                             FROM MedicalRecords mr
                             JOIN Patients p ON mr.patient_id = p.PatientID
                             WHERE mr.record_id = @RecordID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RecordID", _recordID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        this.Text = "Edit Medical Record — " + result.ToString();
                }
            }
            catch { /* silently ignore, title is non-critical */ }
        }

        private void LoadServices()
        {
            cmbProcedure.Items.Clear();

            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT ServiceName FROM DentalServices ORDER BY ServiceName";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    cmbProcedure.Items.Add(reader["ServiceName"].ToString());
            }

            cmbProcedure.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbProcedure.Items.Count > 0)
                cmbProcedure.SelectedIndex = 0;
        }

        private void LoadRecord()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT diagnosis, [procedure], notes, visit_date FROM MedicalRecords WHERE record_id = @RecordID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RecordID", _recordID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtDiagnosis.Text = SafeDecrypt(reader["diagnosis"]);
                        txtNotes.Text = SafeDecrypt(reader["notes"]);
                        dtpVisitDate.Value = reader["visit_date"] != DBNull.Value
                            ? Convert.ToDateTime(reader["visit_date"])
                            : DateTime.Today;

                        string currentProcedure = SafeDecrypt(reader["procedure"]);
                        if (!string.IsNullOrEmpty(currentProcedure) &&
                            cmbProcedure.Items.Contains(currentProcedure))
                            cmbProcedure.SelectedItem = currentProcedure;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading record: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiagnosis.Text) && cmbProcedure.SelectedItem == null)
            {
                MessageBox.Show("Please enter at least a diagnosis or select a procedure.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // ✅ Update medical record
                            string updateQuery = @"
                            UPDATE MedicalRecords 
                            SET diagnosis   = @Diagnosis,
                                [procedure] = @Procedure,
                                notes       = @Notes,
                                visit_date  = @VisitDate
                            WHERE record_id = @RecordID";

                            SqlCommand updateCmd = new SqlCommand(updateQuery, conn, trans);
                            updateCmd.Parameters.AddWithValue("@Diagnosis", CryptoHelper.Encrypt(txtDiagnosis.Text));
                            updateCmd.Parameters.AddWithValue("@Procedure", CryptoHelper.Encrypt(cmbProcedure.SelectedItem?.ToString() ?? ""));
                            updateCmd.Parameters.AddWithValue("@Notes", CryptoHelper.Encrypt(txtNotes.Text));
                            updateCmd.Parameters.AddWithValue("@VisitDate", dtpVisitDate.Value.Date);
                            updateCmd.Parameters.AddWithValue("@RecordID", _recordID);
                            updateCmd.ExecuteNonQuery();

                            // ✅ Insert only NEW prescriptions (IsExisting = false)
                            foreach (PrescriptionData presc in lstPrescriptions.Items)
                            {
                                if (presc.IsExisting) continue; // skip already saved ones

                                string prescQuery = @"
                                INSERT INTO Prescription 
                                (record_id, medication, prescription_date, med_instructions)
                                VALUES 
                                (@RecordID, @Medication, @PrescDate, @Instructions)";

                                SqlCommand prescCmd = new SqlCommand(prescQuery, conn, trans);
                                prescCmd.Parameters.AddWithValue("@RecordID", _recordID);
                                prescCmd.Parameters.AddWithValue("@Medication", CryptoHelper.Encrypt(presc.Medication));
                                prescCmd.Parameters.AddWithValue("@PrescDate", presc.PrescriptionDate);
                                prescCmd.Parameters.AddWithValue("@Instructions", CryptoHelper.Encrypt(presc.Instructions));
                                prescCmd.ExecuteNonQuery();
                            }

                            trans.Commit();

                            MessageBox.Show("Medical record updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show("Error saving: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExistingPrescriptions()
        {
            lstPrescriptions.Items.Clear();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT prescription_id, medication, prescription_date, med_instructions 
                                 FROM Prescription 
                                 WHERE record_id = @RecordID 
                                 ORDER BY prescription_date DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RecordID", _recordID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lstPrescriptions.Items.Add(new PrescriptionData
                        {
                            PrescriptionID = Convert.ToInt32(reader["prescription_id"]),
                            Medication = SafeDecrypt(reader["medication"]),
                            PrescriptionDate = Convert.ToDateTime(reader["prescription_date"]),
                            Instructions = SafeDecrypt(reader["med_instructions"]),
                            IsExisting = true // ✅ flag so we know it's already in DB
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading prescriptions: " + ex.Message);
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null) return "";
            string value = dbValue.ToString();
            if (string.IsNullOrEmpty(value)) return "";
            try { return CryptoHelper.Decrypt(value); }
            catch { return value; }
        }

        private void btnAddPrescription_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMedication.Text))
            {
                MessageBox.Show("Enter medication name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMedInstructions.Text))
            {
                MessageBox.Show("Enter medication instructions.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lstPrescriptions.Items.Add(new PrescriptionData
            {
                Medication = txtMedication.Text.Trim(),
                PrescriptionDate = dtpPrescDate.Value,
                Instructions = txtMedInstructions.Text.Trim()
            });

            txtMedication.Text = "";
            txtMedInstructions.Text = "";
            dtpPrescDate.Value = DateTime.Today;
        }

        private void btnRemovePrescription_Click(object sender, EventArgs e)
        {
            if (lstPrescriptions.SelectedItem == null)
            {
                MessageBox.Show("Select a prescription to remove.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrescriptionData selected = (PrescriptionData)lstPrescriptions.SelectedItem;

            DialogResult confirm = MessageBox.Show(
                $"Remove prescription: {selected.Medication}?",
                "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            // ✅ If it exists in DB, delete it
            if (selected.IsExisting && selected.PrescriptionID > 0)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                    {
                        string query = "DELETE FROM Prescription WHERE prescription_id = @PrescID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PrescID", selected.PrescriptionID);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error removing prescription: " + ex.Message);
                    return;
                }
            }

            lstPrescriptions.Items.Remove(selected);
        }

        private void txtMedInstructions_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtMedication_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
