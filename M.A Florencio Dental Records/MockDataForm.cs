using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public partial class MockDataForm : Form
    {
        // ── Mock data pools ──────────────────────────────────────────
        private static readonly string[] FirstNamesFemale = {
            "Maria", "Ana", "Luisa", "Elena", "Sofia", "Cynthia", "Maricel", "Rosario",
            "Teresita", "Maribel", "Lourdes", "Cristina", "Angelica", "Florencia",
            "Josephine", "Perla", "Imelda", "Corazon", "Remedios", "Milagros",
            "Lilibeth", "Violeta", "Carmelita", "Concepcion", "Zenaida", "Esperanza",
            "Trinidad", "Pacita", "Felicidad", "Salvacion"
        };

        private static readonly string[] FirstNamesMale = {
            "Juan", "Carlos", "Roberto", "Miguel", "Antonio", "Eduardo", "Fernando",
            "Danilo", "Rodrigo", "Ernesto", "Armando", "Bernardo", "Victorino",
            "Gregorio", "Alfredo", "Emilio", "Renato", "Simplicio", "Marcelo",
            "Arturo", "Guillermo", "Leopoldo", "Conrado", "Aurelio", "Demetrio",
            "Metodio", "Saturnino", "Timoteo", "Hermenegildo", "Cornelio"
        };

        private static readonly string[] LastNames = {
            "Santos", "dela Cruz", "Reyes", "Mendoza", "Garcia", "Torres", "Flores",
            "Ramos", "Villanueva", "Cruz", "Lim", "Navarro", "Aquino", "Bautista",
            "Gutierrez", "Hernandez", "Jimenez", "Lopez", "Morales", "Pascual",
            "Rivera", "Salazar", "Tan", "Uy", "Velasco", "Vergara", "Virtucio",
            "Zulueta", "Aguilar", "Cabrera", "Castillo", "Chavez", "Corpuz",
            "Delos Reyes", "Diaz", "Domingo", "Espinosa", "Estrada", "Fernandez",
            "Francisco", "Galang", "Gonzales", "Guerrero", "Ibañez", "Iniguez",
            "Javier", "Lacson", "Lagman", "Lara", "Lazaro"
        };

        private static readonly string[] Streets = {
            "Rizal St", "Mabini Ave", "Luna St", "Aguinaldo Rd", "Bonifacio Blvd",
            "Kalayaan Ave", "Katipunan Ave", "Shaw Blvd", "Commonwealth Ave", "España Blvd",
            "Ortigas Ave", "Taft Ave", "EDSA", "Quirino Ave", "Gil Puyat Ave",
            "Ayala Ave", "Roxas Blvd", "Quezon Ave", "Macapagal Blvd", "Del Monte Ave",
            "Aurora Blvd", "Marcos Highway", "Sumulong Highway", "National Hwy", "Maharlika Hwy"
        };

        private static readonly string[] Cities = {
            "Manila", "Quezon City", "Pasig", "Makati", "Taguig", "Marikina",
            "Mandaluyong", "Paranaque", "Las Pinas", "Muntinlupa", "Caloocan",
            "Malabon", "Navotas", "Valenzuela", "Pasay", "Pateros"
        };

        private static readonly string[] CivilStatuses = {
            "Single", "Married", "Widowed", "Separated"
        };

        private static readonly string[] Religions = {
            "Catholic", "Iglesia ni Cristo", "Protestant", "Evangelical", "Buddhist", "Islam"
        };

        private static readonly string[] Diagnoses = {
            "Mild gingivitis",
            "Dental caries, lower left molar",
            "Periodontal disease, stage 2",
            "Tooth sensitivity, upper incisors",
            "Malocclusion, Class I",
            "Impacted wisdom tooth, lower right",
            "Bruxism, mild to moderate",
            "Dental abscess, upper left premolar"
        };

        private static readonly string[] Procedures = {
            "Scaling and root planing",
            "Composite resin filling",
            "Deep cleaning, antibiotic therapy",
            "Fluoride treatment",
            "Orthodontic assessment",
            "Surgical extraction",
            "Occlusal guard fabrication",
            "Incision and drainage"
        };

        private static readonly string[] Medications = {
            "Amoxicillin 500mg",
            "Ibuprofen 400mg",
            "Metronidazole 500mg",
            "Mefenamic Acid 500mg",
            "Clindamycin 300mg"
        };

        private static readonly string[] MedInstructions = {
            "Take 1 capsule 3x daily for 7 days after meals",
            "Take 1 tablet every 6 hours as needed for pain",
            "Take 1 tablet 2x daily for 5 days after meals",
            "Take 1 capsule every 6 hours for 3 days",
            "Take 1 capsule 4x daily for 10 days with food"
        };

        private static readonly string[] ServiceTypes = {
            "General Checkup", "Tooth Extraction", "Dental Cleaning",
            "Cavity Filling", "Root Canal", "Orthodontic Consultation"
        };

        private static readonly string[] AppointmentStatuses = {
            "Completed", "Scheduled", "Cancelled"
        };

        private static readonly string[] BloodTypes = {
            "A+", "O+", "B+", "AB+", "A-", "O-", "B-", "AB-"
        };

        private static readonly string[] GuardianRelationships = {
            "Mother", "Father", "Guardian"
        };

        // ── Form controls ─────────────────────────────────────────────
        private Button btnInsert;
        private Button btnClose;
        private ProgressBar progressBar;
        private Label lblStatus;
        private RichTextBox rtbLog;
        private Label lblTitle;
        private Label lblWarning;

        private readonly Random _rng = new Random(42);

        public MockDataForm()
        {
            InitializeComponent();
            BuildUI();
        }

        // ── Clean InitializeComponent — no Load event, no Designer.cs conflict ──
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(600, 520);
            this.Name = "MockDataForm";
            this.ResumeLayout(false);
        }

        private void BuildUI()
        {
            this.Text = "Insert Mock Patient Data";
            this.Size = new Size(600, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            lblTitle = new Label
            {
                Text = "Mock Data Generator",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 121, 107),
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblWarning = new Label
            {
                Text = "⚠  This will insert 50 test patients with fully encrypted fields.\n" +
                       "    Use only on a development/test database.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(180, 100, 0),
                Location = new Point(20, 50),
                Size = new Size(550, 40)
            };

            btnInsert = new Button
            {
                Text = "Insert 50 Mock Patients",
                Location = new Point(20, 100),
                Size = new Size(200, 38),
                BackColor = Color.FromArgb(0, 121, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnInsert.FlatAppearance.BorderSize = 0;
            btnInsert.Click += BtnInsert_Click;

            btnClose = new Button
            {
                Text = "Close",
                Location = new Point(235, 100),
                Size = new Size(100, 38),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            progressBar = new ProgressBar
            {
                Location = new Point(20, 150),
                Size = new Size(550, 20),
                Minimum = 0,
                Maximum = 50,
                Value = 0,
                Style = ProgressBarStyle.Continuous
            };

            lblStatus = new Label
            {
                Text = "Ready.",
                Location = new Point(20, 178),
                Size = new Size(550, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80)
            };

            rtbLog = new RichTextBox
            {
                Location = new Point(20, 205),
                Size = new Size(550, 260),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245),
                Font = new Font("Consolas", 8.5f),
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            this.Controls.AddRange(new Control[] {
                lblTitle, lblWarning, btnInsert, btnClose,
                progressBar, lblStatus, rtbLog
            });
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            btnInsert.Enabled = false;
            rtbLog.Clear();
            progressBar.Value = 0;
            lblStatus.Text = "Starting...";
            lblStatus.ForeColor = Color.FromArgb(80, 80, 80);

            try
            {
                InsertMockPatients();
                lblStatus.Text = "✅ Done! 50 mock patients inserted successfully.";
                lblStatus.ForeColor = Color.FromArgb(0, 121, 107);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error: " + ex.Message;
                lblStatus.ForeColor = Color.Red;
                Log("EXCEPTION: " + ex.Message, Color.Red);
            }
            finally
            {
                btnInsert.Enabled = true;
            }
        }

        private void InsertMockPatients()
        {
            string connStr = ConnectionSettings.Current.GetConnectionString();

            for (int i = 1; i <= 50; i++)
            {
                bool isMale = (i % 2 == 0);
                bool isMinor = (i % 8 == 0);

                string gender = isMale ? "Male" : "Female";
                string firstName = isMale
                    ? FirstNamesMale[_rng.Next(FirstNamesMale.Length)]
                    : FirstNamesFemale[_rng.Next(FirstNamesFemale.Length)];
                string lastName = LastNames[_rng.Next(LastNames.Length)];
                string fullName = firstName + " " + lastName;

                int age = isMinor ? _rng.Next(6, 16) : _rng.Next(18, 80);

                DateTime birthDate = DateTime.Today.AddYears(-age).AddDays(-_rng.Next(0, 365));

                string contact = "09" + _rng.Next(100000000, 999999999).ToString();
                string address = $"{_rng.Next(1, 200)} {Streets[_rng.Next(Streets.Length)]}, {Cities[_rng.Next(Cities.Length)]}";
                string civilStatus = isMinor ? "Single" : CivilStatuses[_rng.Next(CivilStatuses.Length)];
                string religion = Religions[_rng.Next(Religions.Length)];

                string guardianName = null;
                string guardianContact = null;
                string guardianRel = null;
                if (isMinor)
                {
                    guardianName = (isMale ? "Rosa" : "Pedro") + " " + lastName;
                    guardianContact = "09" + _rng.Next(100000000, 999999999).ToString();
                    guardianRel = GuardianRelationships[_rng.Next(GuardianRelationships.Length)];
                }

                // ✅ Encrypt all sensitive fields
                string encGender = CryptoHelper.Encrypt(gender);
                string encContact = CryptoHelper.Encrypt(contact);
                string encAddress = CryptoHelper.Encrypt(address);
                string encCivilStatus = CryptoHelper.Encrypt(civilStatus);
                string encReligion = CryptoHelper.Encrypt(religion);
                string encGuardianName = isMinor ? CryptoHelper.Encrypt(guardianName) : null;
                string encGuardianContact = isMinor ? CryptoHelper.Encrypt(guardianContact) : null;
                string encGuardianRel = isMinor ? CryptoHelper.Encrypt(guardianRel) : null;

                // ✅ All declared OUTSIDE the using block so Log() can read them
                int patientId = -1;
                int appointmentId = -1;
                int recordId = -1;
                string status = AppointmentStatuses[i % 3];

                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // ── INSERT PATIENT ────────────────────────────────────────
                    string sqlPatient = @"
                        INSERT INTO Patients
                            (FullName, Gender, BirthDate, Age, ContactNumber, Address,
                             DateRegistered, IsArchived, CivilStatus, Religion,
                             GuardianName, GuardianContact, GuardianRelationship)
                        OUTPUT INSERTED.PatientID
                        VALUES
                            (@FullName, @Gender, @BirthDate, @Age, @ContactNumber, @Address,
                             @DateRegistered, 0, @CivilStatus, @Religion,
                             @GuardianName, @GuardianContact, @GuardianRelationship)";

                    using (var cmd = new SqlCommand(sqlPatient, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Gender", encGender);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate.Date);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@ContactNumber", encContact);
                        cmd.Parameters.AddWithValue("@Address", encAddress);
                        cmd.Parameters.AddWithValue("@DateRegistered", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CivilStatus", encCivilStatus);
                        cmd.Parameters.AddWithValue("@Religion", encReligion);
                        cmd.Parameters.AddWithValue("@GuardianName", (object)encGuardianName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GuardianContact", (object)encGuardianContact ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@GuardianRelationship", (object)encGuardianRel ?? DBNull.Value);

                        patientId = (int)cmd.ExecuteScalar();
                    }

                    // ── INSERT MEDICAL HISTORY ────────────────────────────────
                    bool hasCondition = (i % 4 == 0);
                    bool seriousIllness = (i % 9 == 0);
                    bool hospitalized = (i % 20 == 0);
                    bool takingMeds = (i % 5 == 0);
                    bool isPregnant = (!isMale && !isMinor && i % 11 == 0);

                    string sqlHistory = @"
                        INSERT INTO PatientMedicalHistory
                            (PatientID, LocalAestheticAllergy, PenicillinAllergy, SulfaAllergy,
                             AspirinAllergy, LatexAllergy, OtherAllergies,
                             TakingPrescriptionMeds, MedicationList,
                             UsesTobacco, UsesAlcoholDrugs, HighBP, LowBP,
                             HeartDisease, HeartMurmur, Diabetes, Thyroid, Asthma,
                             RespiratoryProblems, Arthritis, KidneyDisease,
                             IsPregnant, IsNursing, OnBirthControl, BloodType,
                             is_healthy, under_treatment, treatment_details,
                             serious_illness, illness_details,
                             recently_hospitalized, hospitalization_details)
                        VALUES
                            (@PatientID, @LocalAn, @Pen, @Sulfa, @Aspirin, @Latex, @OtherAllergies,
                             @TakingMeds, @MedList,
                             @Tobacco, @Alcohol, @HighBP, @LowBP,
                             @HeartDisease, @HeartMurmur, @Diabetes, @Thyroid, @Asthma,
                             @Respiratory, @Arthritis, @Kidney,
                             @Pregnant, @Nursing, @BirthControl, @BloodType,
                             @IsHealthy, @UnderTreatment, @TreatmentDetails,
                             @SeriousIllness, @IllnessDetails,
                             @Hospitalized, @HospDetails)";

                    using (var cmd = new SqlCommand(sqlHistory, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        cmd.Parameters.AddWithValue("@LocalAn", i % 15 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Pen", i % 20 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Sulfa", i % 25 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Aspirin", i % 30 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Latex", i % 35 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@OtherAllergies", i % 18 == 0
                            ? (object)CryptoHelper.Encrypt("Ibuprofen allergy") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@TakingMeds", takingMeds ? 1 : 0);
                        cmd.Parameters.AddWithValue("@MedList", takingMeds
                            ? (object)CryptoHelper.Encrypt("Metformin 500mg daily") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Tobacco", i % 10 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Alcohol", i % 12 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@HighBP", i % 8 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@LowBP", i % 22 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@HeartDisease", i % 16 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@HeartMurmur", i % 40 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Diabetes", i % 7 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Thyroid", i % 32 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Asthma", i % 11 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Respiratory", i % 14 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Arthritis", i % 13 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Kidney", i % 28 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Pregnant", isPregnant ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Nursing", i % 17 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@BirthControl", i % 6 == 0 ? 1 : 0);
                        cmd.Parameters.AddWithValue("@BloodType", BloodTypes[i % BloodTypes.Length]);
                        cmd.Parameters.AddWithValue("@IsHealthy", hasCondition ? 0 : 1);
                        cmd.Parameters.AddWithValue("@UnderTreatment", hasCondition ? 1 : 0);
                        cmd.Parameters.AddWithValue("@TreatmentDetails", hasCondition
                            ? (object)CryptoHelper.Encrypt("Ongoing treatment for hypertension") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@SeriousIllness", seriousIllness ? 1 : 0);
                        cmd.Parameters.AddWithValue("@IllnessDetails", seriousIllness
                            ? (object)CryptoHelper.Encrypt("Type 2 Diabetes") : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Hospitalized", hospitalized ? 1 : 0);
                        cmd.Parameters.AddWithValue("@HospDetails", hospitalized
                            ? (object)CryptoHelper.Encrypt("Admitted for acute bronchitis, 3 days") : DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    // ── INSERT APPOINTMENT ────────────────────────────────────
                    DateTime apptDate = DateTime.Today.AddDays(-_rng.Next(1, 180));
                    TimeSpan apptTime = new TimeSpan(8 + _rng.Next(0, 8), _rng.Next(0, 2) * 30, 0);
                    TimeSpan apptEnd = apptTime.Add(new TimeSpan(0, 30, 0));
                    string service = ServiceTypes[_rng.Next(ServiceTypes.Length)];

                    string sqlAppt = @"
                        INSERT INTO Appointments
                            (PatientID, PatientName, AppointmentDate, AppointmentTime,
                             AppointmentEndTime, ServiceType, Status, Notes)
                        OUTPUT INSERTED.AppointmentID
                        VALUES
                            (@PatientID, @PatientName, @ApptDate, @ApptTime,
                             @ApptEnd, @ServiceType, @Status, @Notes)";

                    using (var cmd = new SqlCommand(sqlAppt, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                        cmd.Parameters.AddWithValue("@PatientName", fullName);
                        cmd.Parameters.AddWithValue("@ApptDate", apptDate.Date);
                        cmd.Parameters.AddWithValue("@ApptTime", apptTime);
                        cmd.Parameters.AddWithValue("@ApptEnd", apptEnd);
                        cmd.Parameters.AddWithValue("@ServiceType", service);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@Notes", "Mock appointment — testing only");

                        appointmentId = (int)cmd.ExecuteScalar();
                    }

                    // ── INSERT MEDICAL RECORD + PRESCRIPTION (Completed only) ─
                    if (status == "Completed")
                    {
                        string diagnosis = CryptoHelper.Encrypt(Diagnoses[_rng.Next(Diagnoses.Length)]);
                        string procedure = CryptoHelper.Encrypt(Procedures[_rng.Next(Procedures.Length)]);
                        string recNotes = CryptoHelper.Encrypt("Patient tolerated procedure well. Follow-up in 6 months.");

                        string sqlRecord = @"
                            INSERT INTO MedicalRecords
                                (patient_id, appointment_id, visit_date, diagnosis, [procedure], notes)
                            OUTPUT INSERTED.record_id
                            VALUES
                                (@PatientID, @AppointmentID, @VisitDate, @Diagnosis, @Procedure, @Notes)";

                        using (var cmd = new SqlCommand(sqlRecord, conn))
                        {
                            cmd.Parameters.AddWithValue("@PatientID", patientId);
                            cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                            cmd.Parameters.AddWithValue("@VisitDate", apptDate.Date);
                            cmd.Parameters.AddWithValue("@Diagnosis", diagnosis);
                            cmd.Parameters.AddWithValue("@Procedure", procedure);
                            cmd.Parameters.AddWithValue("@Notes", recNotes);

                            recordId = (int)cmd.ExecuteScalar();
                        }

                        string medication = CryptoHelper.Encrypt(Medications[_rng.Next(Medications.Length)]);
                        string instructions = CryptoHelper.Encrypt(MedInstructions[_rng.Next(MedInstructions.Length)]);

                        string sqlRx = @"
                            INSERT INTO Prescription
                                (record_id, medication, prescription_date, med_instructions)
                            VALUES
                                (@RecordID, @Medication, @RxDate, @Instructions)";

                        using (var cmd = new SqlCommand(sqlRx, conn))
                        {
                            cmd.Parameters.AddWithValue("@RecordID", recordId);
                            cmd.Parameters.AddWithValue("@Medication", medication);
                            cmd.Parameters.AddWithValue("@RxDate", apptDate.Date);
                            cmd.Parameters.AddWithValue("@Instructions", instructions);
                            cmd.ExecuteNonQuery();
                        }
                    }

                } // ── end using conn ────────────────────────────────────────

                // ✅ patientId, recordId, status all visible here
                progressBar.Value = i;
                lblStatus.Text = $"Inserting patient {i}/50: {fullName}";
                Application.DoEvents();

                Log(
                    $"[{i:D2}] ✅ {fullName} | {gender} | Age {age} | PatientID: {patientId}" +
                    (recordId > 0 ? $" | RecordID: {recordId}" : " | No record (not completed)"),
                    i % 2 == 0 ? Color.FromArgb(30, 30, 30) : Color.FromArgb(70, 70, 70)
                );
            }
        }

        private void Log(string message, Color color)
        {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(message + Environment.NewLine);
            rtbLog.SelectionColor = rtbLog.ForeColor;
            rtbLog.ScrollToCaret();
        }
    }
}