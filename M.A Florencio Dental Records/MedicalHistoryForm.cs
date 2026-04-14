using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class MedicalHistoryForm : MaterialForm
    {
        public int PatientID { get; set; }
        private int currentTab = 0;
        private int totalTabs = 4;
        private bool isFemale = false;

        public MedicalHistoryForm(int patientID)
        {
            InitializeComponent();
            PatientID = patientID;
            LoadBloodTypeCombo();
        }

        private void MedicalHistoryForm_Load(object sender, EventArgs e)
        {
            CheckGender();
            LoadPatientMedicalHistory();
            UpdateNavigation();
            CustomizeTabColors();

            txtIllnessDetails.Visible = false;
            lblIllnessDetails.Visible = false;
            txtMedicationDetails.Visible = false;
            lblMedicationDetails.Visible = false;
            txtHospitalizationDetails.Visible = false;
            lblHospitalizationDetails.Visible = false;
        }

        public void LoadBloodTypeCombo()
        {
            cmbBloodType.Items.Clear();
            cmbBloodType.Items.Add("-- Select Blood Type --");
            cmbBloodType.Items.Add("O+");
            cmbBloodType.Items.Add("O-");
            cmbBloodType.Items.Add("A+");
            cmbBloodType.Items.Add("A-");
            cmbBloodType.Items.Add("B+");
            cmbBloodType.Items.Add("B-");
            cmbBloodType.Items.Add("AB+");
            cmbBloodType.Items.Add("AB-");
            cmbBloodType.SelectedIndex = 0;
        }

        private void CheckGender()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT Gender FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    // ✅ Decrypt before comparing
                    string gender = SafeDecrypt(result);
                    isFemale = gender.Trim().ToLower() == "female";

                    if (!isFemale)
                    {
                        tabMedicalHistory.TabPages.Remove(tabWomen);
                        totalTabs = 4;
                    }
                    else
                    {
                        totalTabs = 5;
                    }
                }
            }
        }

        private string SafeDecrypt(object dbValue)
        {
            if (dbValue == DBNull.Value || dbValue == null) return "";
            string value = dbValue.ToString();
            if (string.IsNullOrEmpty(value)) return "";

            try { return CryptoHelper.Decrypt(value); }
            catch { return value; }
        }

        private void chkUnderMedication_CheckedChanged(object sender, EventArgs e)
        {
            txtMedicationDetails.Visible = chkUnderMedication.Checked;
            lblMedicationDetails.Visible = chkUnderMedication.Checked;
        }

        private void chkSeriousIllness_CheckedChanged(object sender, EventArgs e)
        {
            txtIllnessDetails.Visible = chkSeriousIllness.Checked;
            lblIllnessDetails.Visible = chkSeriousIllness.Checked;
        }

        private void chkHospitalized_CheckedChanged(object sender, EventArgs e)
        {
            txtHospitalizationDetails.Visible = chkHospitalized.Checked;
            lblHospitalizationDetails.Visible = chkHospitalized.Checked;
        }

        private void LoadPatientMedicalHistory()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    chkGoodHealth.Checked = Convert.ToBoolean(reader["is_healthy"]);
                    chkUnderMedication.Checked = Convert.ToBoolean(reader["under_treatment"]);
                    txtMedicationDetails.Text = SafeDecrypt(reader["treatment_details"]);

                    chkSeriousIllness.Checked = Convert.ToBoolean(reader["serious_illness"]);
                    txtIllnessDetails.Text = SafeDecrypt(reader["illness_details"]);

                    chkHospitalized.Checked = Convert.ToBoolean(reader["recently_hospitalized"]);
                    txtHospitalizationDetails.Text = SafeDecrypt(reader["hospitalization_details"]);

                    chkLocalAnesthetic.Checked = Convert.ToBoolean(reader["LocalAestheticAllergy"]);
                    chkPenicillin.Checked = Convert.ToBoolean(reader["PenicillinAllergy"]);
                    chkSulfa.Checked = Convert.ToBoolean(reader["SulfaAllergy"]);
                    chkAspirin.Checked = Convert.ToBoolean(reader["AspirinAllergy"]);
                    chkLatex.Checked = Convert.ToBoolean(reader["LatexAllergy"]);
                    txtOtherAllergies.Text = SafeDecrypt(reader["OtherAllergies"]);

                    chkPrescriptionMeds.Checked = Convert.ToBoolean(reader["TakingPrescriptionMeds"]);
                    txtMedicationList.Text = SafeDecrypt(reader["MedicationList"]);

                    chkUsesTobacco.Checked = Convert.ToBoolean(reader["UsesTobacco"]);
                    chkUsesAlcoholDrugs.Checked = Convert.ToBoolean(reader["UsesAlcoholDrugs"]);

                    chkHighBP.Checked = Convert.ToBoolean(reader["HighBP"]);
                    chkLowBP.Checked = Convert.ToBoolean(reader["LowBP"]);
                    chkHeartDisease.Checked = Convert.ToBoolean(reader["HeartDisease"]);
                    chkHeartMurmur.Checked = Convert.ToBoolean(reader["HeartMurmur"]);
                    chkDiabetes.Checked = Convert.ToBoolean(reader["Diabetes"]);
                    chkThyroid.Checked = Convert.ToBoolean(reader["Thyroid"]);
                    chkAsthma.Checked = Convert.ToBoolean(reader["Asthma"]);
                    chkRespiratoryProblems.Checked = Convert.ToBoolean(reader["RespiratoryProblems"]);
                    chkArthritis.Checked = Convert.ToBoolean(reader["Arthritis"]);
                    chkKidneyDisease.Checked = Convert.ToBoolean(reader["KidneyDisease"]);

                    string bloodType = SafeDecrypt(reader["BloodType"]);
                    if (cmbBloodType.Items.Contains(bloodType))
                        cmbBloodType.SelectedItem = bloodType;
                    else if (!string.IsNullOrEmpty(bloodType))
                        cmbBloodType.Text = bloodType;

                    if (isFemale)
                    {
                        chkPregnant.Checked = Convert.ToBoolean(reader["IsPregnant"]);
                        chkNursing.Checked = Convert.ToBoolean(reader["IsNursing"]);
                        chkBirthControl.Checked = Convert.ToBoolean(reader["OnBirthControl"]);
                    }
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentTab > 0)
            {
                currentTab--;
                tabMedicalHistory.SelectedIndex = currentTab;
                UpdateNavigation();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentTab < totalTabs - 1)
            {
                currentTab++;
                tabMedicalHistory.SelectedIndex = currentTab;
                UpdateNavigation();
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbBloodType.SelectedIndex <= 0 || cmbBloodType.SelectedItem == null)
            {
                MessageBox.Show("Please select a valid blood type before saving.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string checkQuery = "SELECT COUNT(*) FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@PatientID", PatientID);
                    conn.Open();
                    int exists = (int)checkCmd.ExecuteScalar();
                    conn.Close();

                    if (exists > 0)
                    {
                        string updateQuery = @"
                    UPDATE PatientMedicalHistory SET
                        is_healthy = @IsHealthy,
                        under_treatment = @UnderTreatment,
                        treatment_details = @TreatmentDetails,
                        serious_illness = @SeriousIllness,
                        illness_details = @IllnessDetails,
                        recently_hospitalized = @Hospitalized,
                        hospitalization_details = @HospitalizationDetails,
                        LocalAestheticAllergy = @LocalAnesthetic,
                        PenicillinAllergy = @Penicillin,
                        SulfaAllergy = @Sulfa,
                        AspirinAllergy = @Aspirin,
                        LatexAllergy = @Latex,
                        OtherAllergies = @OtherAllergies,
                        TakingPrescriptionMeds = @PrescriptionMeds,
                        MedicationList = @MedicationList,
                        UsesTobacco = @Tobacco,
                        UsesAlcoholDrugs = @AlcoholDrugs,
                        HighBP = @HighBP,
                        LowBP = @LowBP,
                        HeartDisease = @HeartDisease,
                        HeartMurmur = @HeartMurmur,
                        Diabetes = @Diabetes,
                        Thyroid = @Thyroid,
                        Asthma = @Asthma,
                        RespiratoryProblems = @RespiratoryProblems,
                        Arthritis = @Arthritis,
                        KidneyDisease = @KidneyDisease,
                        IsPregnant = @Pregnant,
                        IsNursing = @Nursing,
                        OnBirthControl = @BirthControl,
                        BloodType = @BloodType
                    WHERE PatientID = @PatientID";

                        SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                        AddMedicalHistoryParameters(updateCmd);
                        conn.Open();
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = @"
                    INSERT INTO PatientMedicalHistory
                        (PatientID, is_healthy, under_treatment, treatment_details,
                         serious_illness, illness_details, recently_hospitalized,
                         hospitalization_details, LocalAestheticAllergy, PenicillinAllergy,
                         SulfaAllergy, AspirinAllergy, LatexAllergy, OtherAllergies,
                         TakingPrescriptionMeds, MedicationList, UsesTobacco, UsesAlcoholDrugs,
                         HighBP, LowBP, HeartDisease, HeartMurmur, Diabetes, Thyroid,
                         Asthma, RespiratoryProblems, Arthritis, KidneyDisease,
                         IsPregnant, IsNursing, OnBirthControl, BloodType)
                    VALUES
                        (@PatientID, @IsHealthy, @UnderTreatment, @TreatmentDetails,
                         @SeriousIllness, @IllnessDetails, @Hospitalized,
                         @HospitalizationDetails, @LocalAnesthetic, @Penicillin,
                         @Sulfa, @Aspirin, @Latex, @OtherAllergies,
                         @PrescriptionMeds, @MedicationList, @Tobacco, @AlcoholDrugs,
                         @HighBP, @LowBP, @HeartDisease, @HeartMurmur, @Diabetes, @Thyroid,
                         @Asthma, @RespiratoryProblems, @Arthritis, @KidneyDisease,
                         @Pregnant, @Nursing, @BirthControl, @BloodType)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        AddMedicalHistoryParameters(insertCmd);
                        conn.Open();
                        insertCmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Patient medical history saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ Just close — Form1 navigation is handled by AddPatientInfo
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving medical history:\n\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMedicalHistoryParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@PatientID", PatientID);
            cmd.Parameters.AddWithValue("@IsHealthy", chkGoodHealth.Checked);
            cmd.Parameters.AddWithValue("@UnderTreatment", chkUnderMedication.Checked);

            cmd.Parameters.AddWithValue("@TreatmentDetails",
                string.IsNullOrEmpty(txtMedicationDetails.Text) ? "" : CryptoHelper.Encrypt(txtMedicationDetails.Text));

            cmd.Parameters.AddWithValue("@SeriousIllness", chkSeriousIllness.Checked);
            cmd.Parameters.AddWithValue("@IllnessDetails",
                string.IsNullOrEmpty(txtIllnessDetails.Text) ? "" : CryptoHelper.Encrypt(txtIllnessDetails.Text));

            cmd.Parameters.AddWithValue("@Hospitalized", chkHospitalized.Checked);
            cmd.Parameters.AddWithValue("@HospitalizationDetails",
                string.IsNullOrEmpty(txtHospitalizationDetails.Text) ? "" : CryptoHelper.Encrypt(txtHospitalizationDetails.Text));

            cmd.Parameters.AddWithValue("@LocalAnesthetic", chkLocalAnesthetic.Checked);
            cmd.Parameters.AddWithValue("@Penicillin", chkPenicillin.Checked);
            cmd.Parameters.AddWithValue("@Sulfa", chkSulfa.Checked);
            cmd.Parameters.AddWithValue("@Aspirin", chkAspirin.Checked);
            cmd.Parameters.AddWithValue("@Latex", chkLatex.Checked);

            cmd.Parameters.AddWithValue("@OtherAllergies",
                string.IsNullOrEmpty(txtOtherAllergies.Text) ? "" : CryptoHelper.Encrypt(txtOtherAllergies.Text));

            cmd.Parameters.AddWithValue("@PrescriptionMeds", chkPrescriptionMeds.Checked);
            cmd.Parameters.AddWithValue("@MedicationList",
                string.IsNullOrEmpty(txtMedicationList.Text) ? "" : CryptoHelper.Encrypt(txtMedicationList.Text));

            cmd.Parameters.AddWithValue("@Tobacco", chkUsesTobacco.Checked);
            cmd.Parameters.AddWithValue("@AlcoholDrugs", chkUsesAlcoholDrugs.Checked);

            cmd.Parameters.AddWithValue("@HighBP", chkHighBP.Checked);
            cmd.Parameters.AddWithValue("@LowBP", chkLowBP.Checked);
            cmd.Parameters.AddWithValue("@HeartDisease", chkHeartDisease.Checked);
            cmd.Parameters.AddWithValue("@HeartMurmur", chkHeartMurmur.Checked);
            cmd.Parameters.AddWithValue("@Diabetes", chkDiabetes.Checked);
            cmd.Parameters.AddWithValue("@Thyroid", chkThyroid.Checked);
            cmd.Parameters.AddWithValue("@Asthma", chkAsthma.Checked);
            cmd.Parameters.AddWithValue("@RespiratoryProblems", chkRespiratoryProblems.Checked);
            cmd.Parameters.AddWithValue("@Arthritis", chkArthritis.Checked);
            cmd.Parameters.AddWithValue("@KidneyDisease", chkKidneyDisease.Checked);

            cmd.Parameters.AddWithValue("@Pregnant", isFemale && chkPregnant.Checked);
            cmd.Parameters.AddWithValue("@Nursing", isFemale && chkNursing.Checked);
            cmd.Parameters.AddWithValue("@BirthControl", isFemale && chkBirthControl.Checked);

            string bloodType = cmbBloodType.SelectedItem?.ToString() ?? "";
            cmd.Parameters.AddWithValue("@BloodType",
                string.IsNullOrEmpty(bloodType) || bloodType == "-- Select Blood Type --"
                    ? ""
                    : CryptoHelper.Encrypt(bloodType));
        }

        private void UpdateNavigation()
        {
            btnCancel.Visible = (currentTab == 0);
            btnBack.Visible = (currentTab > 0);
            btnSave.Visible = (currentTab == totalTabs - 1);
            btnNext.Visible = (currentTab < totalTabs - 1);
        }

        private void CustomizeTabColors()
        {
            tabMedicalHistory.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabMedicalHistory.DrawItem += TabMedicalHistory_DrawItem;
        }

        private void TabMedicalHistory_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = tabMedicalHistory.TabPages[e.Index];
            var tabRect = tabMedicalHistory.GetTabRect(e.Index);

            if (e.Index == tabMedicalHistory.SelectedIndex)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.DodgerBlue), tabRect);
                e.Graphics.DrawString(tabPage.Text, new Font("Arial", 10, FontStyle.Bold),
                    Brushes.White, tabRect.Location);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), tabRect);
                e.Graphics.DrawString(tabPage.Text, new Font("Arial", 10),
                    Brushes.Black, tabRect.Location);
            }

            e.Graphics.DrawRectangle(Pens.Black, tabRect);
        }

        private void chkGoodHealth_CheckedChanged(object sender, EventArgs e) { }
        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cmbBloodType_SelectedIndexChanged(object sender, EventArgs e) { }
        private void tabGeneral_Click(object sender, EventArgs e) { }
    }
}