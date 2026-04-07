using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class EditPatientForm : MaterialForm
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";
        public int PatientID { get; set; }

        private int currentTab = 0;
        private int totalTabs = 4;
        private bool isFemale = false;

        public EditPatientForm(int patientID)
        {
            InitializeComponent();
            PatientID = patientID;
        }

        // ─── STEP 1: Form loads ───────────────────────────────────────────────
        private void EditPatientForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            SetupTabControl();
            LoadPatientData();
            UpdateNavigation();
        }

        // ─── STEP 2: Tab control styling ─────────────────────────────────────
        private void SetupTabControl()
        {
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += TabControl_DrawItem;
            tabControl.ItemSize = new Size(100, 40);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = tabControl.TabPages[e.Index];
            var tabRect = tabControl.GetTabRect(e.Index);

            if (e.Index == tabControl.SelectedIndex)
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

        // ─── STEP 3: Load patient + medical data from DB ──────────────────────
        private void LoadPatientData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        MessageBox.Show("Patient not found!");
                        this.Close();
                        return;
                    }

                    txtFullName.Text = reader["FullName"].ToString();

                    cmbGender.Items.Clear();
                    cmbGender.Items.Add("Male");
                    cmbGender.Items.Add("Female");
                    cmbGender.SelectedItem = reader["Gender"].ToString();

                    string gender = reader["Gender"].ToString();
                    isFemale = (gender.ToLower() == "female");

                    dtpBirthDate.Value = Convert.ToDateTime(reader["BirthDate"]);
                    txtContactNumber.Text = reader["ContactNumber"].ToString();
                    txtAddress.Text = reader["Address"].ToString();

                    cmbCivilStatus.Items.Clear();
                    cmbCivilStatus.Items.Add("Single");
                    cmbCivilStatus.Items.Add("Married");
                    cmbCivilStatus.Items.Add("Divorced");
                    cmbCivilStatus.Items.Add("Widowed");
                    cmbCivilStatus.SelectedItem = reader["CivilStatus"].ToString();

                    txtReligion.Text = reader["Religion"].ToString();
                    txtGuardianName.Text = reader["GuardianName"].ToString();
                    txtGuardianContact.Text = reader["GuardianContact"].ToString();

                    cmbGuardianRelationship.Items.Clear();
                    cmbGuardianRelationship.Items.Add("Parent");
                    cmbGuardianRelationship.Items.Add("Sibling");
                    cmbGuardianRelationship.Items.Add("Spouse");
                    cmbGuardianRelationship.Items.Add("Other");

                    string rel = reader["GuardianRelationship"].ToString();
                    if (!string.IsNullOrEmpty(rel))
                        cmbGuardianRelationship.SelectedItem = rel;

                    reader.Close();
                    conn.Close();

                    // Women tab visibility based on gender
                    if (!isFemale)
                    {
                        tabControl.TabPages.Remove(tabWomen);
                        totalTabs = 4;
                    }
                    else
                    {
                        totalTabs = 5;
                    }

                    LoadMedicalHistory();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading patient: " + ex.Message);
            }
        }

        private void LoadMedicalHistory()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PatientID", PatientID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        conn.Close();
                        return;
                    }

                    // General
                    chkGoodHealth.Checked = Convert.ToBoolean(reader["is_healthy"]);
                    chkUnderMedication.Checked = Convert.ToBoolean(reader["under_treatment"]);
                    txtMedicationDetails.Text = reader["treatment_details"].ToString();
                    chkSeriousIllness.Checked = Convert.ToBoolean(reader["serious_illness"]);
                    txtIllnessDetails.Text = reader["illness_details"].ToString();
                    chkHospitalized.Checked = Convert.ToBoolean(reader["recently_hospitalized"]);
                    txtHospitalizationDetails.Text = reader["hospitalization_details"].ToString();

                    txtMedicationDetails.Visible = chkUnderMedication.Checked;
                    lblMedicationDetails.Visible = chkUnderMedication.Checked;
                    txtIllnessDetails.Visible = chkSeriousIllness.Checked;
                    lblIllnessDetails.Visible = chkSeriousIllness.Checked;
                    txtHospitalizationDetails.Visible = chkHospitalized.Checked;
                    lblHospitalizationDetails.Visible = chkHospitalized.Checked;

                    // Allergies
                    chkLocalAnesthetic.Checked = Convert.ToBoolean(reader["LocalAestheticAllergy"]);
                    chkPenicillin.Checked = Convert.ToBoolean(reader["PenicillinAllergy"]);
                    chkSulfa.Checked = Convert.ToBoolean(reader["SulfaAllergy"]);
                    chkAspirin.Checked = Convert.ToBoolean(reader["AspirinAllergy"]);
                    chkLatex.Checked = Convert.ToBoolean(reader["LatexAllergy"]);
                    txtOtherAllergies.Text = reader["OtherAllergies"].ToString();

                    // Medications
                    chkPrescriptionMeds.Checked = Convert.ToBoolean(reader["TakingPrescriptionMeds"]);
                    txtMedicationList.Text = reader["MedicationList"].ToString();
                    chkUsesTobacco.Checked = Convert.ToBoolean(reader["UsesTobacco"]);
                    chkUsesAlcoholDrugs.Checked = Convert.ToBoolean(reader["UsesAlcoholDrugs"]);

                    // Conditions
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

                    // Women (only if tab exists)
                    if (isFemale)
                    {
                        chkPregnant.Checked = Convert.ToBoolean(reader["IsPregnant"]);
                        chkNursing.Checked = Convert.ToBoolean(reader["IsNursing"]);
                        chkBirthControl.Checked = Convert.ToBoolean(reader["OnBirthControl"]);
                        cmbBloodType.Text = reader["BloodType"].ToString();
                    }

                    reader.Close();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading medical history: " + ex.Message);
            }
        }

        // ─── STEP 4: Navigation button visibility ─────────────────────────────
        private void UpdateNavigation()
        {
            btnCancel.Visible = (currentTab == 0);
            btnBack.Visible = (currentTab > 0);
            btnSave.Visible = (currentTab == totalTabs - 1);
            btnNext.Visible = (currentTab < totalTabs - 1);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentTab > 0)
            {
                currentTab--;
                tabControl.SelectedIndex = currentTab;
                UpdateNavigation();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentTab < totalTabs - 1)
            {
                currentTab++;
                tabControl.SelectedIndex = currentTab;
                UpdateNavigation();
            }
        }

        // ─── STEP 5: Cancel ───────────────────────────────────────────────────
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ─── STEP 6: Validate ─────────────────────────────────────────────────
        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full name is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (cmbGender.SelectedIndex == -1)
            {
                MessageBox.Show("Select a gender!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContactNumber.Text))
            {
                MessageBox.Show("Contact number is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNumber.Focus();
                return false;
            }

            if (!long.TryParse(txtContactNumber.Text, out _))
            {
                MessageBox.Show("Contact must be numbers only!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNumber.Focus();
                return false;
            }

            if (txtContactNumber.Text.Length < 10 || txtContactNumber.Text.Length > 11)
            {
                MessageBox.Show("Contact must be 10-11 digits!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContactNumber.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            if (cmbCivilStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Select a civil status!", "Validation Error",
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

            // Guardian required only for minors
            int age = DateTime.Today.Year - dtpBirthDate.Value.Year;
            if (dtpBirthDate.Value.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                if (string.IsNullOrWhiteSpace(txtGuardianName.Text))
                {
                    MessageBox.Show("Guardian name is required for minors!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGuardianName.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtGuardianContact.Text))
                {
                    MessageBox.Show("Guardian contact is required for minors!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGuardianContact.Focus();
                    return false;
                }

                if (!long.TryParse(txtGuardianContact.Text, out _))
                {
                    MessageBox.Show("Guardian contact must be numbers only!", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGuardianContact.Focus();
                    return false;
                }
            }

            return true;
        }

        // ─── STEP 7: Save — UPDATE both tables ────────────────────────────────
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields())
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int age = DateTime.Today.Year - dtpBirthDate.Value.Year;
                    if (dtpBirthDate.Value.Date > DateTime.Today.AddYears(-age)) age--;

                    // UPDATE Patients
                    string updatePatient = @"
                        UPDATE Patients SET
                            FullName             = @FullName,
                            Gender               = @Gender,
                            BirthDate            = @BirthDate,
                            Age                  = @Age,
                            ContactNumber        = @ContactNumber,
                            Address              = @Address,
                            CivilStatus          = @CivilStatus,
                            Religion             = @Religion,
                            GuardianName         = @GuardianName,
                            GuardianContact      = @GuardianContact,
                            GuardianRelationship = @GuardianRelationship
                        WHERE PatientID = @PatientID";

                    SqlCommand cmdPatient = new SqlCommand(updatePatient, conn);
                    cmdPatient.Parameters.AddWithValue("@PatientID", PatientID);
                    cmdPatient.Parameters.AddWithValue("@FullName", txtFullName.Text);
                    cmdPatient.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem.ToString());
                    cmdPatient.Parameters.AddWithValue("@BirthDate", dtpBirthDate.Value.Date);
                    cmdPatient.Parameters.AddWithValue("@Age", age);
                    cmdPatient.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text);
                    cmdPatient.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmdPatient.Parameters.AddWithValue("@CivilStatus", cmbCivilStatus.SelectedItem.ToString());
                    cmdPatient.Parameters.AddWithValue("@Religion", txtReligion.Text);
                    cmdPatient.Parameters.AddWithValue("@GuardianName", txtGuardianName.Text ?? "");
                    cmdPatient.Parameters.AddWithValue("@GuardianContact", txtGuardianContact.Text ?? "");
                    cmdPatient.Parameters.AddWithValue("@GuardianRelationship", cmbGuardianRelationship.SelectedItem?.ToString() ?? "");
                    cmdPatient.ExecuteNonQuery();

                    // UPSERT PatientMedicalHistory
                    string checkQuery = "SELECT COUNT(*) FROM PatientMedicalHistory WHERE PatientID = @PatientID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@PatientID", PatientID);
                    int exists = (int)checkCmd.ExecuteScalar();

                    string medQuery = exists > 0
                        ? @"UPDATE PatientMedicalHistory SET
                                is_healthy              = @IsHealthy,
                                under_treatment         = @UnderTreatment,
                                treatment_details       = @TreatmentDetails,
                                serious_illness         = @SeriousIllness,
                                illness_details         = @IllnessDetails,
                                recently_hospitalized   = @Hospitalized,
                                hospitalization_details = @HospitalizationDetails,
                                LocalAestheticAllergy   = @LocalAnesthetic,
                                PenicillinAllergy       = @Penicillin,
                                SulfaAllergy            = @Sulfa,
                                AspirinAllergy          = @Aspirin,
                                LatexAllergy            = @Latex,
                                OtherAllergies          = @OtherAllergies,
                                TakingPrescriptionMeds  = @PrescriptionMeds,
                                MedicationList          = @MedicationList,
                                UsesTobacco             = @Tobacco,
                                UsesAlcoholDrugs        = @AlcoholDrugs,
                                HighBP                  = @HighBP,
                                LowBP                   = @LowBP,
                                HeartDisease            = @HeartDisease,
                                HeartMurmur             = @HeartMurmur,
                                Diabetes                = @Diabetes,
                                Thyroid                 = @Thyroid,
                                Asthma                  = @Asthma,
                                RespiratoryProblems     = @RespiratoryProblems,
                                Arthritis               = @Arthritis,
                                KidneyDisease           = @KidneyDisease,
                                IsPregnant              = @Pregnant,
                                IsNursing               = @Nursing,
                                OnBirthControl          = @BirthControl,
                                BloodType               = @BloodType
                            WHERE PatientID = @PatientID"
                        : @"INSERT INTO PatientMedicalHistory
                            (PatientID, is_healthy, under_treatment, treatment_details,
                             serious_illness, illness_details, recently_hospitalized, hospitalization_details,
                             LocalAestheticAllergy, PenicillinAllergy, SulfaAllergy, AspirinAllergy, LatexAllergy,
                             OtherAllergies, TakingPrescriptionMeds, MedicationList, UsesTobacco, UsesAlcoholDrugs,
                             HighBP, LowBP, HeartDisease, HeartMurmur, Diabetes, Thyroid, Asthma,
                             RespiratoryProblems, Arthritis, KidneyDisease,
                             IsPregnant, IsNursing, OnBirthControl, BloodType)
                           VALUES
                            (@PatientID, @IsHealthy, @UnderTreatment, @TreatmentDetails,
                             @SeriousIllness, @IllnessDetails, @Hospitalized, @HospitalizationDetails,
                             @LocalAnesthetic, @Penicillin, @Sulfa, @Aspirin, @Latex,
                             @OtherAllergies, @PrescriptionMeds, @MedicationList, @Tobacco, @AlcoholDrugs,
                             @HighBP, @LowBP, @HeartDisease, @HeartMurmur, @Diabetes, @Thyroid, @Asthma,
                             @RespiratoryProblems, @Arthritis, @KidneyDisease,
                             @Pregnant, @Nursing, @BirthControl, @BloodType)";

                    SqlCommand medCmd = new SqlCommand(medQuery, conn);
                    AddMedicalParameters(medCmd);
                    medCmd.ExecuteNonQuery();

                    conn.Close();
                }

                MessageBox.Show("Patient updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating patient: " + ex.Message);
            }
        }

        // ─── Shared parameter helper ──────────────────────────────────────────
        private void AddMedicalParameters(SqlCommand cmd)
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

        // ─── Checkbox toggle handlers ─────────────────────────────────────────
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
    }
}