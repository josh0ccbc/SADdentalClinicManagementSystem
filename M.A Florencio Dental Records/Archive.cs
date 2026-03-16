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
    public partial class Archive : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public int PatientID { get; set; }

        public Archive()
        {
            InitializeComponent();
            LoadArchivedPatients();
        }

        private void LoadArchivedPatients()
        {
            flowArchive.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Load ONLY archived patients (IsArchived = 1)
                string query = "SELECT PatientID, FullName, Gender, Age, ContactNumber FROM Patients WHERE IsArchived = 1";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ArchivePatients card = new ArchivePatients();

                    card.PatientID = Convert.ToInt32(reader["PatientID"]);

                    card.SetPatient(
                        reader["PatientID"].ToString(),
                        reader["FullName"].ToString(),
                        reader["Gender"].ToString(),
                        Convert.ToInt32(reader["Age"]),
                        reader["ContactNumber"].ToString()
                    );

                    flowArchive.Controls.Add(card);
                }
            }
        }

        private void Archive_Load(object sender, EventArgs e)
        {

        }
    }
}
