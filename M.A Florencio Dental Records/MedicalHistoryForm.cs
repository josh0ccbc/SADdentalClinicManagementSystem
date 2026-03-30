using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Linq;

namespace M.A_Florencio_Dental_Records
{
    public partial class MedicalHistoryForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
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
            tabMedicalHistory.ItemSize = new System.Drawing.Size(100, 40);

            txtIllnessDetails.Visible = false;
            lblIllnessDetails.Visible = false;
            txtMedicationDetails.Visible = false;
            lblMedicationDetails.Visible = false;
            txtHospitalizationDetails.Visible = false;
            lblHospitalizationDetails.Visible = false;
        }

        public void LoadBloodTypeCombo()
        {
            cmbBloodType.Items.Add("O+");
            cmbBloodType.Items.Add("O-");
            cmbBloodType.Items.Add("A+");
            cmbBloodType.Items.Add("A-");
            cmbBloodType.Items.Add("B+");
            cmbBloodType.Items.Add("B-");
            cmbBloodType.Items.Add("AB+");
            cmbBloodType.Items.Add("AB-");
        }

        private void CheckGender()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Gender FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string gender = result.ToString();
                    isFemale = (gender.ToLower() == "female");

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

                conn.Close();
            }
        }

        private void chkGoodHealth_CheckedChanged(object sender, EventArgs e)
        {
                
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
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", PatientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // General Tab
                    chkGoodHealth.Checked = Convert.ToBoolean(reader["is_healthy"]);
                    chkUnderMedication.Checked = Convert.ToBoolean(reader["under_treatment"]);
                    txtMedicationDetails.Text = reader["treatment_details"].ToString() ?? "";
                    chkSeriousIllness.Checked = Convert.ToBoolean(reader["serious_illness"]);
                    txtIllnessDetails.Text = reader["illness_details"].ToString() ?? "";
                    chkHospitalized.Checked = Convert.ToBoolean(reader["recently_hospitalized"]);
                    txtHospitalizationDetails.Text = reader["hospitalization_details"].ToString() ?? "";

                    txtIllnessDetails.Visible = chkSeriousIllness.Checked;
                    lblIllnessDetails.Visible = chkSeriousIllness.Checked;
                    txtMedicationDetails.Visible = chkUnderMedication.Checked;
                    lblMedicationDetails.Visible = chkUnderMedication.Checked;
                    txtHospitalizationDetails.Visible = chkHospitalized.Checked;
                    lblHospitalizationDetails.Visible = chkHospitalized.Checked;

                    // Allergies Tab
                    chkLocalAnesthetic.Checked = Convert.ToBoolean(reader["LocalAestheticAllergy"]);
                    chkPenicillin.Checked = Convert.ToBoolean(reader["PenicillinAllergy"]);
                    chkSulfa.Checked = Convert.ToBoolean(reader["SulfaAllergy"]);
                    chkAspirin.Checked = Convert.ToBoolean(reader["AspirinAllergy"]);
                    chkLatex.Checked = Convert.ToBoolean(reader["LatexAllergy"]);
                    txtOtherAllergies.Text = reader["OtherAllergies"].ToString() ?? "";

                    // Medications Tab
                    chkPrescriptionMeds.Checked = Convert.ToBoolean(reader["TakingPrescriptionMeds"]);
                    txtMedicationList.Text = reader["MedicationList"].ToString() ?? "";
                    chkUsesTobacco.Checked = Convert.ToBoolean(reader["UsesTobacco"]);
                    chkUsesAlcoholDrugs.Checked = Convert.ToBoolean(reader["UsesAlcoholDrugs"]);

                    // Conditions Tab
                    chkHighBP.Checked = Convert.ToBoolean(reader["HighBP"] ?? false);
                    chkLowBP.Checked = Convert.ToBoolean(reader["LowBP"] ?? false);
                    chkHeartDisease.Checked = Convert.ToBoolean(reader["HeartDisease"] ?? false);
                    chkHeartMurmur.Checked = Convert.ToBoolean(reader["HeartMurmur"] ?? false);
                    chkDiabetes.Checked = Convert.ToBoolean(reader["Diabetes"] ?? false);
                    chkThyroid.Checked = Convert.ToBoolean(reader["Thyroid"] ?? false);
                    chkAsthma.Checked = Convert.ToBoolean(reader["Asthma"] ?? false);
                    chkRespiratoryProblems.Checked = Convert.ToBoolean(reader["RespiratoryProblems"] ?? false);
                    chkArthritis.Checked = Convert.ToBoolean(reader["Arthritis"] ?? false);
                    chkKidneyDisease.Checked = Convert.ToBoolean(reader["KidneyDisease"] ?? false);

                    // Women Tab (if female)
                    if (isFemale)
                    {
                        chkPregnant.Checked = Convert.ToBoolean(reader["IsPregnant"]);
                        chkNursing.Checked = Convert.ToBoolean(reader["IsNursing"]);
                        chkBirthControl.Checked = Convert.ToBoolean(reader["OnBirthControl"]);
                        cmbBloodType.Text = reader["BloodType"].ToString() ?? "";
                    }
                }

                conn.Close();
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
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                    BloodType = @BloodType,
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
                    (PatientID, is_healthy, under_treatment, treatment_details, serious_illness, illness_details, 
                     recently_hospitalized, hospitalization_details,
                     LocalAestheticAllergy, PenicillinAllergy, SulfaAllergy, AspirinAllergy, LatexAllergy,
                     OtherAllergies, TakingPrescriptionMeds, MedicationList, UsesTobacco, UsesAlcoholDrugs,
                     HighBP, LowBP, HeartDisease, HeartMurmur, Diabetes, Thyroid, Asthma, RespiratoryProblems, Arthritis, KidneyDisease,
                     IsPregnant, IsNursing, OnBirthControl, BloodType)
                    VALUES
                    (@PatientID, @IsHealthy, @UnderTreatment, @TreatmentDetails, @SeriousIllness, @IllnessDetails,
                     @Hospitalized, @HospitalizationDetails,
                     @LocalAnesthetic, @Penicillin, @Sulfa, @Aspirin, @Latex,
                     @OtherAllergies, @PrescriptionMeds, @MedicationList, @Tobacco, @AlcoholDrugs,
                     @HighBP, @LowBP, @HeartDisease, @HeartMurmur, @Diabetes, @Thyroid, @Asthma, @RespiratoryProblems, @Arthritis, @KidneyDisease,
                     @Pregnant, @Nursing, @BirthControl, @BloodType)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        AddMedicalHistoryParameters(insertCmd);
                        conn.Open();
                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Patient Recorded Successfully");
                    conn.Close();

                    Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();

                    if (mainForm != null)
                    {
                        mainForm.Show();
                        mainForm.WindowState = FormWindowState.Normal;
                        mainForm.BringToFront();
                        mainForm.LoadControl(new DBcontrol());
                    }
                    else
                    {
                        mainForm = new Form1();
                        mainForm.Show();
                        mainForm.LoadControl(new DBcontrol());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void AddMedicalHistoryParameters(SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@PatientID", PatientID);
            cmd.Parameters.AddWithValue("@IsHealthy", chkGoodHealth.Checked);
            cmd.Parameters.AddWithValue("@UnderTreatment", chkUnderMedication.Checked);
            cmd.Parameters.AddWithValue("@TreatmentDetails", txtMedicationDetails.Text ?? "");
            cmd.Parameters.AddWithValue("@SeriousIllness", chkSeriousIllness.Checked);
            cmd.Parameters.AddWithValue("@IllnessDetails", txtIllnessDetails.Text ?? "");
            cmd.Parameters.AddWithValue("@Hospitalized", chkHospitalized.Checked);
            cmd.Parameters.AddWithValue("@HospitalizationDetails", txtHospitalizationDetails.Text ?? "");
            cmd.Parameters.AddWithValue("@LocalAnesthetic", chkLocalAnesthetic.Checked);
            cmd.Parameters.AddWithValue("@Penicillin", chkPenicillin.Checked);
            cmd.Parameters.AddWithValue("@Sulfa", chkSulfa.Checked);
            cmd.Parameters.AddWithValue("@Aspirin", chkAspirin.Checked);
            cmd.Parameters.AddWithValue("@Latex", chkLatex.Checked);
            cmd.Parameters.AddWithValue("@OtherAllergies", txtOtherAllergies.Text ?? "");
            cmd.Parameters.AddWithValue("@PrescriptionMeds", chkPrescriptionMeds.Checked);
            cmd.Parameters.AddWithValue("@MedicationList", txtMedicationList.Text ?? "");
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
            cmd.Parameters.AddWithValue("@Pregnant", isFemale ? chkPregnant.Checked : false);
            cmd.Parameters.AddWithValue("@Nursing", isFemale ? chkNursing.Checked : false);
            cmd.Parameters.AddWithValue("@BirthControl", isFemale ? chkBirthControl.Checked : false);
            cmd.Parameters.AddWithValue("@BloodType", cmbBloodType.Text ?? "");
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

        private void materialTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbBloodType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabGeneral_Click(object sender, EventArgs e)
        {

        }
    }
}