using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public partial class DBcontrol : UserControl
    {
        public DBcontrol()
        {
            InitializeComponent();
        }

        // ✅ LOAD EVENT (SAFE PLACE)
        private void DBcontrol_Load(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                string connStr = ConnectionHelper.GetConnectionString();

                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("Database not configured.");
                    return;
                }

                lblPatientCount.Text = GetPatientCount(connStr).ToString();
                lblTodayAppointments.Text = GetTodayAppointments(connStr).ToString();
                lblUpcomingAppointments.Text = GetUpcomingAppointments(connStr).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dashboard error: " + ex.Message);
            }
        }

        // ==========================
        // PATIENT COUNT
        // ==========================
        public int GetPatientCount(string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Patients";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // ==========================
        // TODAY APPOINTMENTS
        // ==========================
        public int GetTodayAppointments(string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = @today";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@today", DateTime.Today);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // ==========================
        // UPCOMING APPOINTMENTS
        // ==========================
        public int GetUpcomingAppointments(string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"SELECT COUNT(*) 
                                 FROM Appointments
                                 WHERE AppointmentDate > CAST(GETDATE() AS DATE)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // ==========================
        // BUTTON NAVIGATION
        // ==========================
        private void BTNAddPatient_Click(object sender, EventArgs e)
        {
            Form1 mainForm = (Form1)this.FindForm();
            Form2 frm = new Form2(mainForm);

            mainForm.Hide();
            frm.Show();
            this.Hide();
        }

        private void BTNAddPatient_MouseEnter(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.HoverButton;
        }

        private void BTNAddPatient_MouseLeave(object sender, EventArgs e)
        {
            BTNAddPatient.BackgroundImage = Properties.Resources.Button;
        }
    }
}