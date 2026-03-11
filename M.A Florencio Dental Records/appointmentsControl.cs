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
    public partial class appointmentsControl : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public appointmentsControl()
        {
            InitializeComponent();
            LoadAppointmentDates();
        }

        private void appointmentsControl_Load(object sender, EventArgs e)
        {

        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            LoadAppointments(e.Start);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddAppointmentForm form = new AddAppointmentForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadAppointmentDates();
            }
        }

        private void LoadAppointmentDates()
        {
            List<DateTime> appointmentDates = new List<DateTime>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT AppointmentDate FROM Appointments";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    appointmentDates.Add(Convert.ToDateTime(reader["AppointmentDate"]));
                }
            }

            monthCalendar1.BoldedDates = appointmentDates.ToArray();
            }

        private void LoadAppointments(DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                        AppointmentTime AS Time,
                        PatientName AS Patient,
                        Notes
                        FROM Appointments
                        WHERE AppointmentDate = @date
                        ORDER BY AppointmentTime";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date.Date);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                DGVAppointments.DataSource = table;
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }
    }
    }
