// DatabaseInitializer.cs
using System;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Called after connection is confirmed working.
        /// Ensures all tables and default admin exist.
        /// </summary>
        public static void Initialize()
        {
            string connStr = ConnectionHelper.GetConnectionString();

            if (string.IsNullOrEmpty(connStr))
                throw new Exception("No connection string found. Run setup first.");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                EnsureTablesExist(conn);
                EnsureAdminExists(conn);
            }
        }

        private static void EnsureTablesExist(SqlConnection conn)
        {
            ServerDiscovery.CreateTables(conn);

            string sql = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
CREATE TABLE Users (
    UserID       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Role         NVARCHAR(20)  DEFAULT 'Staff',
    IsActive     BIT           DEFAULT 1,
    CreatedAt    DATETIME      DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Patients' AND xtype='U')
CREATE TABLE Patients (
    PatientID            INT IDENTITY(1,1) PRIMARY KEY,
    FullName             NVARCHAR(100) NOT NULL,
    Gender               NVARCHAR(100),
    BirthDate            DATE,
    Age                  INT,
    ContactNumber        NVARCHAR(255),
    Address              NVARCHAR(500),
    CivilStatus          NVARCHAR(255),
    Religion             NVARCHAR(255),
    GuardianName         NVARCHAR(255),
    GuardianContact      NVARCHAR(255),
    GuardianRelationship NVARCHAR(255),
    DateRegistered       DATETIME DEFAULT GETDATE(),
    IsArchived           BIT      DEFAULT 0
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PatientMedicalHistory' AND xtype='U')
CREATE TABLE PatientMedicalHistory (
    HistoryID               INT IDENTITY(1,1) PRIMARY KEY,
    PatientID               INT FOREIGN KEY REFERENCES Patients(PatientID),
    is_healthy              BIT,
    under_treatment         BIT,
    treatment_details       NVARCHAR(MAX),
    serious_illness         BIT,
    illness_details         NVARCHAR(MAX),
    recently_hospitalized   BIT,
    hospitalization_details NVARCHAR(MAX),
    LocalAestheticAllergy   BIT,
    PenicillinAllergy       BIT,
    SulfaAllergy            BIT,
    AspirinAllergy          BIT,
    LatexAllergy            BIT,
    OtherAllergies          NVARCHAR(MAX),
    TakingPrescriptionMeds  BIT,
    MedicationList          NVARCHAR(MAX),
    UsesTobacco             BIT,
    UsesAlcoholDrugs        BIT,
    HighBP                  BIT,
    LowBP                   BIT,
    HeartDisease            BIT,
    HeartMurmur             BIT,
    Diabetes                BIT,
    Thyroid                 BIT,
    Asthma                  BIT,
    RespiratoryProblems     BIT,
    Arthritis               BIT,
    KidneyDisease           BIT,
    IsPregnant              BIT,
    IsNursing               BIT,
    OnBirthControl          BIT,
    BloodType               NVARCHAR(255),
    BleedingTime            NVARCHAR(255),
    BloodPressure           NVARCHAR(255)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Appointments' AND xtype='U')
CREATE TABLE Appointments (
    AppointmentID   INT IDENTITY(1,1) PRIMARY KEY,
    PatientID       INT FOREIGN KEY REFERENCES Patients(PatientID),
    AppointmentDate DATETIME,
    ServiceType     NVARCHAR(100),
    Status          NVARCHAR(50)  DEFAULT 'Scheduled',
    Notes           NVARCHAR(MAX),
    CreatedAt       DATETIME      DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MedicalRecords' AND xtype='U')
CREATE TABLE MedicalRecords (
    record_id      INT IDENTITY(1,1) PRIMARY KEY,
    patient_id     INT FOREIGN KEY REFERENCES Patients(PatientID),
    appointment_id INT FOREIGN KEY REFERENCES Appointments(AppointmentID),
    visit_date     DATETIME      DEFAULT GETDATE(),
    diagnosis      NVARCHAR(MAX),
    [procedure]    NVARCHAR(MAX),
    notes          NVARCHAR(MAX)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Prescription' AND xtype='U')
CREATE TABLE Prescription (
    prescription_id   INT IDENTITY(1,1) PRIMARY KEY,
    record_id         INT FOREIGN KEY REFERENCES MedicalRecords(record_id),
    medication        NVARCHAR(MAX),
    prescription_date DATETIME DEFAULT GETDATE(),
    med_instructions  NVARCHAR(MAX)
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLog' AND xtype='U')
CREATE TABLE AuditLog (
    LogID     INT IDENTITY(1,1) PRIMARY KEY,
    UserID    INT FOREIGN KEY REFERENCES Users(UserID),
    Action    NVARCHAR(100),
    IPAddress NVARCHAR(50),
    LoggedAt  DATETIME DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PasswordResetTokens' AND xtype='U')
CREATE TABLE PasswordResetTokens (
    TokenID    INT IDENTITY(1,1) PRIMARY KEY,
    UserID     INT FOREIGN KEY REFERENCES Users(UserID),
    Token      NVARCHAR(100),
    ExpiryDate DATETIME,
    CreatedAt  DATETIME DEFAULT GETDATE()
);";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        private static void EnsureAdminExists(SqlConnection conn)
        {
            string check = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            int count;

            using (SqlCommand cmd = new SqlCommand(check, conn))
                count = (int)cmd.ExecuteScalar();

            if (count > 0) return;

            string hash = PasswordHelper.HashPassword("admin123");

            string insert = @"
INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, IsActive)
VALUES ('admin', 'admin@florenciodental.com', @Hash, 'Administrator', 'Admin', 1)";

            using (SqlCommand cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.ExecuteNonQuery();
            }
        }
        private static void SeedDentalServices(SqlConnection conn)
        {
            // Only seed if table is empty
            string checkQuery = "SELECT COUNT(*) FROM DentalServices";
            using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
            {
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) return; // already has data, skip
            }

            string insertQuery = @"
        INSERT INTO DentalServices (ServiceName, ServiceCode, Description) VALUES
        ('Oral Prophylaxis',  'LINIS',   'Professional teeth cleaning'),
        ('Restoration',       'PASTA',   'Tooth filling/restoration'),
        ('Extraction',        'BUNOT',   'Tooth extraction'),
        ('Dentures',          'PUSTISO', 'Full or partial dentures'),
        ('Dental Crowns',     'CROWN',   'Tooth crown'),
        ('Fixed Bridge',      'BRIDGE',  'Dental bridge'),
        ('Veneers',           'VENEER',  'Tooth veneers'),
        ('Ortho Braces',      'ORTHO',   'Orthodontic braces'),
        ('Teeth Whitening',   'WHITEN',  'Professional whitening');";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                cmd.ExecuteNonQuery();
        }
    }
}