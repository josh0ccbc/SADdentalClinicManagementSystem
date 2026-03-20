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
    public partial class appointmentsControl : UserControl
    {
        string connectionString = @"Data Source=DESKTOP-ASL74A6;Initial Catalog=DentalClinicDB;Integrated Security=True";

        public appointmentsControl()
        {
            InitializeComponent();
            LoadAppointmentDates();

            monthCalendar1.SelectionStart = DateTime.Today;
            monthCalendar1.SelectionEnd = DateTime.Today;
            LoadAppointments(DateTime.Today);
        }

        private void appointmentsControl_Load(object sender, EventArgs e)
        {
            LoadAppointments(DateTime.Today);
        }

        // Calendar date selected
        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            LoadAppointments(e.Start);
        }

        // Add appointment button
        private void button1_Click(object sender, EventArgs e)
        {
            AppointmentForm appointmentForm = new AppointmentForm();
            appointmentForm.ShowDialog();

            // Reload everything after adding
            LoadAppointmentDates();
            LoadAppointments(DateTime.Today);
            monthCalendar1.SelectionStart = DateTime.Today;
        }

        // Load all appointment dates to bold on calendar
        private void LoadAppointmentDates()
        {
            List<DateTime> appointmentDates = new List<DateTime>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT CAST(AppointmentDate AS DATE) FROM Appointments WHERE Status != 'Cancelled'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    appointmentDates.Add(Convert.ToDateTime(reader[0]));
                }
                conn.Close();
            }

            monthCalendar1.BoldedDates = appointmentDates.ToArray();
        }

        // Load appointments for selected date as CARDS
        private void LoadAppointments(DateTime date)
        {
            // Clear existing cards
            flowAppointments.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.AppointmentID, a.PatientName, a.AppointmentDate, a.AppointmentTime, 
                                       s.ServiceName, a.Status, a.Notes
                                FROM Appointments a
                                LEFT JOIN DentalServices s ON a.ServiceID = s.ServiceID
                                WHERE CAST(a.AppointmentDate AS DATE) = @SelectedDate
                                ORDER BY a.AppointmentTime ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SelectedDate", date.Date);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    Label emptyLabel = new Label();
                    emptyLabel.Text = "No appointments for this date";
                    emptyLabel.Font = new Font("Segoe UI", 12);
                    emptyLabel.ForeColor = Color.Gray;
                    emptyLabel.AutoSize = true;
                    flowAppointments.Controls.Add(emptyLabel);
                }
                else
                {
                    while (reader.Read())
                    {
                        AppointmentCard card = new AppointmentCard();

                        card.SetAppointment(
                            Convert.ToInt32(reader["AppointmentID"]),
                            reader["PatientName"].ToString(),
                            Convert.ToDateTime(reader["AppointmentDate"]),
                            TimeSpan.Parse(reader["AppointmentTime"].ToString()),
                            reader["ServiceName"].ToString() ?? "N/A",
                            reader["Status"].ToString(),
                            reader["Notes"].ToString()
                        );

                        flowAppointments.Controls.Add(card);
                    }
                }

                conn.Close();
            }
        }

        public int GetTodayAppointmentsCount()
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE) AND Status != 'Cancelled'";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                count = (int)cmd.ExecuteScalar();
                conn.Close();
            }
            return count;
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
        }
    }
}