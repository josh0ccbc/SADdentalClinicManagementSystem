using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private List<ArchivedPatientInfo> allPatients = new List<ArchivedPatientInfo>();
        private int currentPage = 1;
        private int patientsPerPage = 9;
        private bool sortAtoZ = false;
        private string searchQuery = "";

        public Archive()
        {
            InitializeComponent();
        }

        private void Archive_Load(object sender, EventArgs e)
        {
            LoadPatients();
        }

        private void LoadAllPatients()
        {
            allPatients.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FullName, Gender, Age, ContactNumber FROM Patients WHERE IsArchived = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ArchivedPatientInfo patient = new ArchivedPatientInfo
                    {
                        PatientID = Convert.ToInt32(reader["PatientID"]),
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
            List<ArchivedPatientInfo> filteredPatients = FilterPatients();

            if (sortAtoZ)
            {
                filteredPatients = filteredPatients.OrderBy(p => p.FullName).ToList();
            }

            DisplayPage(filteredPatients);
        }

        private List<ArchivedPatientInfo> FilterPatients()
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return allPatients;

            return allPatients.Where(p =>
                p.FullName.ToLower().Contains(searchQuery.ToLower()) ||
                p.ContactNumber.Contains(searchQuery)
            ).ToList();
        }

        private void DisplayPage(List<ArchivedPatientInfo> patients)
        {
            flowArchive.Controls.Clear();

            if (patients.Count == 0)
            {
                Label emptyLabel = new Label();
                emptyLabel.Text = "No archived patients found";
                emptyLabel.Font = new Font("Segoe UI", 12);
                emptyLabel.AutoSize = true;
                flowArchive.Controls.Add(emptyLabel);
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

            List<ArchivedPatientInfo> pagePatients = patients
                .Skip((currentPage - 1) * patientsPerPage)
                .Take(patientsPerPage)
                .ToList();

            foreach (ArchivedPatientInfo patient in pagePatients)
            {
                ArchivePatients card = new ArchivePatients();
                card.PatientID = patient.PatientID;

                card.SetPatient(
                    patient.PatientID.ToString(),
                    patient.FullName,
                    patient.Gender,
                    patient.Age,
                    patient.ContactNumber
                );

                flowArchive.Controls.Add(card);
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

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadPatients();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadPatients();
        }

        private void flowArchive_Paint(object sender, PaintEventArgs e)
        {
        }
    }

    public class ArchivedPatientInfo
    {
        public int PatientID { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ContactNumber { get; set; }
    }
}