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
        private List<PatientInfo> allPatients = new List<PatientInfo>();
        private int currentPage = 1;
        private int patientsPerPage = 10;
        private bool sortAtoZ = false;
        private string searchQuery = "";

        public patientControl()
        {
            InitializeComponent();            
        }

        private void patientControl_Load(object sender, EventArgs e)
        {
            LoadPatients();
        }

        private void LoadAllPatients()
        {
            allPatients.Clear();

            using (SqlConnection conn = new SqlConnection(ConnectionSettings.Current.GetConnectionString()))
            {
                string query = "SELECT PatientID, FullName, Gender, BirthDate, Age, ContactNumber FROM Patients WHERE IsArchived = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PatientInfo patient = new PatientInfo
                    {
                        PatientID = Convert.ToInt32(reader["PatientID"]),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        FullName = reader["FullName"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        ContactNumber = reader["ContactNumber"].ToString()
                    };

                    allPatients.Add(patient);
                }

                conn.Close();
            }
        }

        private void LoadPatients()
        {
            LoadAllPatients();

            List<PatientInfo> filteredPatients = FilterPatients();

            if (sortAtoZ)
            {
                filteredPatients = filteredPatients.OrderBy(p => p.FullName).ToList();
            }

            DisplayPage(filteredPatients);
        }

        private List<PatientInfo> FilterPatients()
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return allPatients;

            return allPatients.Where(p =>
                p.FullName.ToLower().Contains(searchQuery.ToLower()) ||
                p.ContactNumber.Contains(searchQuery)
            ).ToList();
        }

        private void DisplayPage(List<PatientInfo> patients)
        {
            flowPatients.Controls.Clear();

            if (patients.Count == 0)
            {
                Label emptyLabel = new Label();
                emptyLabel.Text = "No patients found";
                emptyLabel.Font = new Font("Segoe UI", 12);
                emptyLabel.AutoSize = true;
                flowPatients.Controls.Add(emptyLabel);
                lblPageInfo.Text = "No results";
                btnPreviousPage.Enabled = false;
                btnNextPage.Enabled = false;
                return;
            }

            int totalPages = (int)Math.Ceiling((double)patients.Count / patientsPerPage);

            if (currentPage > totalPages)
                currentPage = totalPages;
            if (currentPage < 1)
                currentPage = 1;

            List<PatientInfo> pagePatients = patients
                .Skip((currentPage - 1) * patientsPerPage)
                .Take(patientsPerPage)
                .ToList();

            foreach (PatientInfo patient in pagePatients)
            {
                Patient card = new Patient();
                card.PatientID = patient.PatientID;

                card.SetPatient(
                    patient.BirthDate,
                    patient.PatientID.ToString(),
                    patient.FullName,
                    patient.Gender,
                    patient.Age,
                    patient.ContactNumber
                );

                flowPatients.Controls.Add(card);
            }

            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";

            btnPreviousPage.Enabled = (currentPage > 1);
            btnNextPage.Enabled = (currentPage < totalPages);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            searchQuery = txtSearch.Text.Trim();
            currentPage = 1;  
            LoadPatients();
        }

        private void btnAtoZ_Click(object sender, EventArgs e)
        {
            sortAtoZ = !sortAtoZ;
            currentPage = 1; 
            btnAtoZ.Text = sortAtoZ ? "A-Z" : "Z-A";
            LoadPatients();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPatients();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadPatients();
        }

        private void flowPatients_Paint(object sender, PaintEventArgs e)
        {
        }
    }

    public class PatientInfo
    {
        public int PatientID { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ContactNumber { get; set; }
    }
}