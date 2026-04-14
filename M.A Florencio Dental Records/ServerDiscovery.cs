using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public static class ServerDiscovery
    {
        private static readonly string[] CommonServerNames = new[]
        {
            ".\\SQLEXPRESS",
            "localhost\\SQLEXPRESS",
            "(local)\\SQLEXPRESS",
            ".\\MSSQLSERVER",
            "localhost\\MSSQLSERVER",
            "localhost",
            ".",
            "(local)",
            ".\\SQLEXPRESS19",
            ".\\SQLEXPRESS17",
            ".\\SQLEXPRESS16",
            ".\\SQL2019",
            ".\\SQL2017",
            ".\\SQL2016",
            Environment.MachineName + "\\SQLEXPRESS",
            Environment.MachineName,
        };

        public static string FindWorkingServer()
        {
            try
            {
                var discovered = FindAvailableServers();
                foreach (string server in discovered)
                {
                    var (ok, _) = TestConnectionDetailed(server, "master");
                    if (ok) return server;
                }
            }
            catch { }

            foreach (string server in CommonServerNames)
            {
                try
                {
                    var (ok, _) = TestConnectionDetailed(server, "master");
                    if (ok) return server;
                }
                catch { }
            }

            return null;
        }

        public static List<string> FindAvailableServers()
        {
            var servers = new List<string>();

            try
            {
                var table = SqlDataSourceEnumerator.Instance.GetDataSources();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();

                    string fullName = string.IsNullOrEmpty(instanceName)
                        ? serverName
                        : $"{serverName}\\{instanceName}";

                    if (!servers.Contains(fullName))
                        servers.Add(fullName);
                }
            }
            catch { }

            foreach (string common in CommonServerNames)
            {
                if (!servers.Contains(common))
                    servers.Add(common);
            }

            return servers;
        }

        public static (bool success, string error) TestConnectionDetailed(
            string serverName, string databaseName = "master")
        {
            try
            {
                string connStr =
                    $"Server={serverName};Database={databaseName};" +
                    $"Integrated Security=True;Connection Timeout=5;";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    return (true, null);
                }
            }
            catch (SqlException ex)
            {
                string msg;
                switch (ex.Number)
                {
                    case 26: msg = $"Error 26: Server '{serverName}' not found."; break;
                    case 18456: msg = $"Error 18456: Login failed for '{serverName}'."; break;
                    case 53: msg = $"Error 53: Network path not found for '{serverName}'."; break;
                    case 40: msg = $"Error 40: Could not open connection to '{serverName}'."; break;
                    default: msg = $"SQL Error {ex.Number}: {ex.Message}"; break;
                }
                return (false, msg);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static (bool success, string error) CreateDatabaseIfNotExists(
            string serverName, string databaseName)
        {
            try
            {
                string masterConn =
                    $"Server={serverName};Database=master;" +
                    $"Integrated Security=True;Connection Timeout=10;";

                using (SqlConnection conn = new SqlConnection(masterConn))
                {
                    conn.Open();

                    string createDb = $@"
                        IF NOT EXISTS (
                            SELECT name FROM sys.databases WHERE name = '{databaseName}'
                        )
                        CREATE DATABASE [{databaseName}]";

                    using (SqlCommand cmd = new SqlCommand(createDb, conn))
                        cmd.ExecuteNonQuery();
                }

                string dbConn =
                    $"Server={serverName};Database={databaseName};" +
                    $"Integrated Security=True;Connection Timeout=10;";

                using (SqlConnection conn = new SqlConnection(dbConn))
                {
                    conn.Open();
                    CreateTables(conn);
                    CreateDefaultAdmin(conn);
                    SeedDentalServices(conn); // ✅ Seeds default services on fresh install
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static void CreateTables(SqlConnection conn)
        {
            string sql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
            CREATE TABLE Users (
                UserID       INT IDENTITY(1,1) PRIMARY KEY,
                Username     NVARCHAR(50)  NOT NULL UNIQUE,
                Email        NVARCHAR(100) NOT NULL UNIQUE,
                PasswordHash NVARCHAR(255) NOT NULL,
                FullName     NVARCHAR(100) NULL,
                Role         NVARCHAR(50)  NULL DEFAULT 'Dentist',
                CreatedDate  DATETIME      NULL DEFAULT GETDATE(),
                IsActive     BIT           NULL DEFAULT 1
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DentalServices' AND xtype='U')
            CREATE TABLE DentalServices (
                ServiceID    INT IDENTITY(1,1) PRIMARY KEY,
                ServiceName  VARCHAR(100) NOT NULL,
                ServiceCode  VARCHAR(20)  NULL,
                Description  NVARCHAR(500) NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Patients' AND xtype='U')
            CREATE TABLE Patients (
                PatientID            INT IDENTITY(1,1) PRIMARY KEY,
                FullName             VARCHAR(MAX) NULL,
                Gender               VARCHAR(MAX) NULL,
                BirthDate            DATE         NOT NULL,
                Age                  INT          NULL,
                ContactNumber        VARCHAR(MAX) NULL,
                Address              VARCHAR(MAX) NULL,
                MedicalHistory       TEXT         NULL,
                DateRegistered       DATETIME     NULL DEFAULT GETDATE(),
                IsArchived           BIT          NULL DEFAULT 0,
                CivilStatus          VARCHAR(MAX) NULL,
                Religion             VARCHAR(MAX) NULL,
                GuardianName         VARCHAR(MAX) NULL,
                GuardianContact      VARCHAR(MAX) NULL,
                GuardianRelationship VARCHAR(MAX) NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Appointments' AND xtype='U')
            CREATE TABLE Appointments (
                AppointmentID      INT IDENTITY(1,1) PRIMARY KEY,
                PatientID          INT          NULL REFERENCES Patients(PatientID),
                AppointmentDate    DATE         NOT NULL,
                AppointmentTime    TIME         NOT NULL,
                PatientName        VARCHAR(100) NULL,
                ServiceID          INT          NULL REFERENCES DentalServices(ServiceID),
                ServiceType        VARCHAR(100) NULL,
                Status             VARCHAR(50)  NULL,
                Notes              NVARCHAR(500) NULL,
                AppointmentEndTime TIME         NULL,
                EndTime            TIME         NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PatientMedicalHistory' AND xtype='U')
            CREATE TABLE PatientMedicalHistory (
                MedicalHistoryID        INT IDENTITY(1,1) PRIMARY KEY,
                PatientID               INT          NULL REFERENCES Patients(PatientID),
                LocalAestheticAllergy   BIT          NULL,
                PenicillinAllergy       BIT          NULL,
                SulfaAllergy            BIT          NULL,
                AspirinAllergy          BIT          NULL,
                LatexAllergy            BIT          NULL,
                OtherAllergies          VARCHAR(MAX) NULL,
                TakingPrescriptionMeds  BIT          NULL,
                MedicationList          VARCHAR(MAX) NULL,
                UsesTobacco             BIT          NULL,
                UsesAlcoholDrugs        BIT          NULL,
                HighBP                  BIT          NULL,
                LowBP                   BIT          NULL,
                HeartDisease            BIT          NULL,
                HeartMurmur             BIT          NULL,
                Diabetes                BIT          NULL,
                Thyroid                 BIT          NULL,
                Asthma                  BIT          NULL,
                RespiratoryProblems     BIT          NULL,
                Arthritis               BIT          NULL,
                KidneyDisease           BIT          NULL,
                IsPregnant              BIT          NULL,
                IsNursing               BIT          NULL,
                OnBirthControl          BIT          NULL,
                BloodType               VARCHAR(MAX) NULL,
                LastUpdated             DATETIME     NULL DEFAULT GETDATE(),
                is_healthy              BIT          NULL,
                under_treatment         BIT          NULL,
                treatment_details       VARCHAR(MAX) NULL,
                serious_illness         BIT          NULL,
                illness_details         VARCHAR(MAX) NULL,
                recently_hospitalized   BIT          NULL,
                hospitalization_details VARCHAR(MAX) NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MedicalRecords' AND xtype='U')
            CREATE TABLE MedicalRecords (
                record_id      INT IDENTITY(1,1) PRIMARY KEY,
                patient_id     INT          NOT NULL REFERENCES Patients(PatientID),
                appointment_id INT          NULL     REFERENCES Appointments(AppointmentID),
                visit_date     DATE         NULL,
                diagnosis      VARCHAR(MAX) NULL,
                [procedure]    VARCHAR(MAX) NULL,
                notes          VARCHAR(MAX) NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Prescription' AND xtype='U')
            CREATE TABLE Prescription (
                prescription_id   INT IDENTITY(1,1) PRIMARY KEY,
                record_id         INT          NOT NULL REFERENCES MedicalRecords(record_id),
                medication        VARCHAR(MAX) NULL,
                prescription_date DATE         NULL,
                med_instructions  VARCHAR(MAX) NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AuditLog' AND xtype='U')
            CREATE TABLE AuditLog (
                LogID      INT IDENTITY(1,1) PRIMARY KEY,
                UserID     INT           NULL REFERENCES Users(UserID),
                Action     NVARCHAR(100) NULL,
                ActionDate DATETIME      NULL DEFAULT GETDATE(),
                IPAddress  NVARCHAR(50)  NULL
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PasswordResetTokens' AND xtype='U')
            CREATE TABLE PasswordResetTokens (
                TokenID     INT IDENTITY(1,1) PRIMARY KEY,
                UserID      INT           NOT NULL REFERENCES Users(UserID),
                Token       NVARCHAR(255) NOT NULL UNIQUE,
                CreatedDate DATETIME      NULL DEFAULT GETDATE(),
                ExpiryDate  DATETIME      NOT NULL,
                IsUsed      BIT           NULL DEFAULT 0
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EncryptionKeys' AND xtype='U')
            CREATE TABLE EncryptionKeys (
                KeyID                INT IDENTITY(1,1) PRIMARY KEY,
                KeyName              NVARCHAR(100)  NOT NULL UNIQUE,
                ProtectedKeyData     VARBINARY(MAX) NOT NULL,
                DataProtectionScope  NVARCHAR(20)   NOT NULL,
                CreatedDate          DATETIME       NOT NULL DEFAULT GETDATE(),
                CreatedByUser        NVARCHAR(100)  NOT NULL,
                IsActive             BIT            NOT NULL DEFAULT 1
            );

            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EncryptionAuditLog' AND xtype='U')
            CREATE TABLE EncryptionAuditLog (
                LogID           INT IDENTITY(1,1) PRIMARY KEY,
                Action          NVARCHAR(50)   NOT NULL,
                KeyName         NVARCHAR(100)  NOT NULL,
                PatientID       INT            NULL,
                PerformedByUser NVARCHAR(100)  NOT NULL,
                Timestamp       DATETIME       NOT NULL DEFAULT GETDATE(),
                Details         NVARCHAR(MAX)  NULL
            );";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
                cmd.ExecuteNonQuery();
        }

        // ✅ Seeds default dental services — skips if data already exists
        private static void SeedDentalServices(SqlConnection conn)
        {
            string check = "SELECT COUNT(*) FROM DentalServices";
            using (SqlCommand cmd = new SqlCommand(check, conn))
            {
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) return; // already has data, don't seed again
            }

            string insert = @"
                INSERT INTO DentalServices (ServiceName, ServiceCode, Description) VALUES
                ('Oral Prophylaxis', 'LINIS',   'Professional teeth cleaning'),
                ('Restoration',      'PASTA',   'Tooth filling/restoration'),
                ('Extraction',       'BUNOT',   'Tooth extraction'),
                ('Dentures',         'PUSTISO', 'Full or partial dentures'),
                ('Dental Crowns',    'CROWN',   'Tooth crown'),
                ('Fixed Bridge',     'BRIDGE',  'Dental bridge'),
                ('Veneers',          'VENEER',  'Tooth veneers'),
                ('Ortho Braces',     'ORTHO',   'Orthodontic braces'),
                ('Teeth Whitening',  'WHITEN',  'Professional whitening');";

            using (SqlCommand cmd = new SqlCommand(insert, conn))
                cmd.ExecuteNonQuery();
        }

        private static void CreateDefaultAdmin(SqlConnection conn)
        {
            string check = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            using (SqlCommand cmd = new SqlCommand(check, conn))
            {
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) return;
            }

            string passwordHash = PasswordHelper.HashPassword("admin123");

            string insert = @"
                INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, IsActive)
                VALUES ('admin', 'admin@florenciodental.com', @Hash, 'Administrator', 'Admin', 1)";

            using (SqlCommand cmd = new SqlCommand(insert, conn))
            {
                cmd.Parameters.AddWithValue("@Hash", passwordHash);
                cmd.ExecuteNonQuery();
            }
        }
    }
}