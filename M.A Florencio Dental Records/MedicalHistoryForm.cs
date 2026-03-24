using System;
using System.Windows.Forms;
using System.Drawing;
using System.Data.SqlClient;
using MaterialSkin;
using MaterialSkin.Controls;

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
        }

        private void MedicalHistoryForm_Load(object sender, EventArgs e)
        {
            CheckGender();
            LoadPatientMedicalHistory();
            UpdateNavigation();
            CustomizeTabColors();
            tabMedicalHistory.ItemSize = new System.Drawing.Size(100, 40);
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
                    radGoodHealth.Checked = Convert.ToBoolean(reader["GoodHealth"]);
                    radUnderTreatment.Checked = Convert.ToBoolean(reader["UnderMedicalTreatment"]);
                    radSeriousIllness.Checked = Convert.ToBoolean(reader["GoodHealth"]);
                    radHospitalized.Checked = Convert.ToBoolean(reader["UnderMedicalTreatment"]);
                    
                    // Allergies Tab
                    chkLocalAnesthetic.Checked = Convert.ToBoolean(reader["LocalAestheticAllergy"]);
                    chkPenicillin.Checked = Convert.ToBoolean(reader["PenicillinAllergy"]);
                    chkSulfa.Checked = Convert.ToBoolean(reader["SulfaAllergy"]);
                    chkLatex.Checked = Convert.ToBoolean(reader["AspirinAllergy"]);
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
                        txtBloodPressure.Text = reader["BloodPressure"].ToString() ?? "";
                        txtBleedingTime.Text = reader["BleedingTime"].ToString() ?? "";
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
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
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
                            GoodHealth = @GoodHealth,
                            UnderMedicalTreatment = @UnderTreatment,
                            SeriousIllnessOrSurgery = @SeriousIllness,
                            Hospitalized = @Hospitalized,
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
                            BloodPressure = @BloodPressure,
                            BleedingTime = @BleedingTime
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
                            (PatientID, GoodHealth, UnderMedicalTreatment, SeriousIllnessOrSurgery, Hospitalized,
                             LocalAestheticAllergy, PenicillinAllergy, SulfaAllergy, AspirinAllergy, LatexAllergy,
                             OtherAllergies, TakingPrescriptionMeds, MedicationList, UsesTobacco, UsesAlcoholDrugs,
                             HighBP, LowBP, HeartDisease, HeartMurmur, Diabetes, Thyroid, Asthma, RespiratoryProblems, Arthritis, KidneyDisease,
                             IsPregnant, IsNursing, OnBirthControl, BloodType, BloodPressure, BleedingTime)
                            VALUES
                            (@PatientID, @GoodHealth, @UnderTreatment, @SeriousIllness, @Hospitalized,
                             @LocalAnesthetic, @Penicillin, @Sulfa, @Aspirin, @Latex,
                             @OtherAllergies, @PrescriptionMeds, @MedicationList, @Tobacco, @AlcoholDrugs,
                             @HighBP, @LowBP, @HeartDisease, @HeartMurmur, @Diabetes, @Thyroid, @Asthma, @RespiratoryProblems, @Arthritis, @KidneyDisease,
                             @Pregnant, @Nursing, @BirthControl, @BloodType, @BloodPressure, @BleedingTime)";

                        SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                        AddMedicalHistoryParameters(insertCmd);
                        conn.Open();
                        insertCmd.ExecuteNonQuery();
                    }

                    conn.Close();
                    MessageBox.Show("Medical history saved successfully!");
                    this.Close();
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
            cmd.Parameters.AddWithValue("@GoodHealth", radGoodHealth.Checked);
            cmd.Parameters.AddWithValue("@UnderTreatment", radUnderTreatment.Checked);
            cmd.Parameters.AddWithValue("@SeriousIllness", radSeriousIllness.Checked);
            cmd.Parameters.AddWithValue("@Hospitalized", radHospitalized.Checked);
            cmd.Parameters.AddWithValue("@LocalAnesthetic", chkLocalAnesthetic.Checked);
            cmd.Parameters.AddWithValue("@Penicillin", chkPenicillin.Checked);
            cmd.Parameters.AddWithValue("@Sulfa", chkSulfa.Checked);
            cmd.Parameters.AddWithValue("@Aspirin", chkLatex.Checked);
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
            cmd.Parameters.AddWithValue("@BloodPressure", isFemale ? txtBloodPressure.Text ?? "" : "");
            cmd.Parameters.AddWithValue("@BleedingTime", isFemale ? txtBleedingTime.Text ?? "" : "");
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
    }
}