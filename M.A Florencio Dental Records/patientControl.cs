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
    public partial class patientControl : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public patientControl()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void patientControl_Load(object sender, EventArgs e)
        {

        }

        private void LoadPatients()
        {
            flowPatients.Controls.Clear();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FullName, Gender, BirthDate, Age, ContactNumber FROM Patients WHERE IsArchived = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Patient card = new Patient();
                    card.PatientID = Convert.ToInt32(reader["PatientID"]);
                    
                    card.SetPatient(
                        Convert.ToDateTime(reader["BirthDate"]),     
                        reader["PatientID"].ToString(),              
                        reader["FullName"].ToString(),               
                        reader["Gender"].ToString(),                 
                        Convert.ToInt32(reader["Age"]),            
                        reader["ContactNumber"].ToString()            
                    );

                    flowPatients.Controls.Add(card);
                }
            }
        }

        private void flowPatients_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
