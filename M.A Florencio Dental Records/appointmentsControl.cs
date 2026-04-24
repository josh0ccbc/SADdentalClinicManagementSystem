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
        private bool isLoading = false;

        private int currentPage = 1;
        private int appointmentsPerPage = 6;
        private int totalAppointmentsCount = 0;
        private int totalPages = 0;

        public appointmentsControl()
        {
            InitializeComponent();
            monthCalendar1.SelectionStart = DateTime.Today;
            monthCalendar1.SelectionEnd = DateTime.Today;
        }

        private async void AppointmentsControl_Load(object sender, EventArgs e)
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                await LoadAppointmentDatesAsync();
                await LoadAppointmentsAsync(DateTime.Today);

                currentPage = 1;
                await LoadAllAppointmentsPagedAsync(currentPage);
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task<int> GetTotalAppointmentsCountAsync()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Appointments WHERE Status != 'Cancelled' AND Status != 'Done'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;

                    await conn.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();

                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return 0;
            }
        }

        private async Task LoadAllAppointmentsPagedAsync(int pageNumber)
        {
            try
            {
                totalAppointmentsCount = await GetTotalAppointmentsCountAsync();
                totalPages = (int)Math.Ceiling((double)totalAppointmentsCount / appointmentsPerPage);

                if (pageNumber < 1) pageNumber = 1;
                if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

                currentPage = pageNumber;

                foreach (Control ctrl in panelPagination.Controls)
                {
                    ctrl.Dispose();
                }
                panelPagination.Controls.Clear();

                panelPagination.SuspendLayout();

                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"
                        SELECT a.AppointmentID, a.PatientName, a.AppointmentDate, a.AppointmentTime, 
                               s.ServiceName, a.Status, a.Notes
                        FROM Appointments a
                        LEFT JOIN DentalServices s ON a.ServiceID = s.ServiceID
                        WHERE a.Status != 'Cancelled' AND a.Status != 'Done'
                        ORDER BY a.AppointmentDate DESC, a.AppointmentTime ASC
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;

                    int offset = (pageNumber - 1) * appointmentsPerPage;
                    cmd.Parameters.AddWithValue("@Offset", offset);
                    cmd.Parameters.AddWithValue("@PageSize", appointmentsPerPage);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            Label emptyLabel = new Label
                            {
                                Text = "No appointments",
                                Font = new Font("Segoe UI", 12),
                                ForeColor = Color.Gray,
                                AutoSize = true
                            };
                            panelPagination.Controls.Add(emptyLabel);
                        }
                        else
                        {
                            while (await reader.ReadAsync())
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

                                card.OnMarkAsDone += Card_OnMarkAsDone;
                                panelPagination.Controls.Add(card);
                            }
                        }
                    }
                }

                panelPagination.ResumeLayout(true);
                UpdatePaginationInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointments: " + ex.Message);
            }
        }

        private void UpdatePaginationInfo()
        {
            if (totalPages > 0)
            {
                lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
                btnPrevious.Enabled = currentPage > 1;
                btnNext.Enabled = currentPage < totalPages;
            }
            else
            {
                lblPageInfo.Text = "Page 1 of 1";
                btnPrevious.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        private async void btnPrevious_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await LoadAllAppointmentsPagedAsync(currentPage);
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                await LoadAllAppointmentsPagedAsync(currentPage);
            }
        }

        private async Task LoadAppointmentDatesAsync()
        {
            List<DateTime> appointmentDates = new List<DateTime>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT DISTINCT CAST(AppointmentDate AS DATE) FROM Appointments WHERE Status != 'Cancelled' AND Status != 'Done'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            appointmentDates.Add(Convert.ToDateTime(reader[0]));
                        }
                    }
                }

                monthCalendar1.BoldedDates = appointmentDates.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading calendar: " + ex.Message);
            }
        }

        private async Task LoadAppointmentsAsync(DateTime date)
        {
            try
            {
                foreach (Control ctrl in flowAppointments.Controls)
                {
                    ctrl.Dispose();
                }
                flowAppointments.Controls.Clear();

                flowAppointments.SuspendLayout();

                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT a.AppointmentID, a.PatientName, a.AppointmentDate, a.AppointmentTime, 
                                           s.ServiceName, a.Status, a.Notes
                                    FROM Appointments a
                                    LEFT JOIN DentalServices s ON a.ServiceID = s.ServiceID
                                    WHERE CAST(a.AppointmentDate AS DATE) = @SelectedDate
                                    AND a.Status != 'Done'
                                    ORDER BY a.AppointmentTime ASC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@SelectedDate", date.Date);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                        {
                            Label emptyLabel = new Label
                            {
                                Text = "No appointments for this date",
                                Font = new Font("Segoe UI", 12),
                                ForeColor = Color.Gray,
                                AutoSize = true
                            };
                            flowAppointments.Controls.Add(emptyLabel);
                        }
                        else
                        {
                            while (await reader.ReadAsync())
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

                                card.OnMarkAsDone += Card_OnMarkAsDone;
                                flowAppointments.Controls.Add(card);
                            }
                        }
                    }
                }

                flowAppointments.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading appointments: " + ex.Message);
            }
        }

        private async void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (isLoading) return;
            await LoadAppointmentsAsync(e.Start);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AppointmentForm apptForm = new AppointmentForm();
            apptForm.FormClosed += async (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("=== AppointmentForm closed: " + apptForm.DialogResult);

                if (apptForm.DialogResult == DialogResult.OK && !isLoading)
                {
                    isLoading = true;
                    try
                    {
                        await LoadAppointmentDatesAsync();
                        await LoadAppointmentsAsync(monthCalendar1.SelectionStart);
                        currentPage = 1;
                        await LoadAllAppointmentsPagedAsync(currentPage);
                        monthCalendar1.SelectionStart = DateTime.Today;
                    }
                    finally
                    {
                        isLoading = false;
                    }
                }
            };

            System.Diagnostics.Debug.WriteLine("=== Showing AppointmentForm ===");
            apptForm.Show();
        }

        private async void Card_OnMarkAsDone(int appointmentID)
        {
            if (isLoading) return;

            var appt = await GetAppointmentDetailsAsync(appointmentID);

            if (appt != null)
            {
                CompleteAppointmentForm completeForm = new CompleteAppointmentForm(appointmentID, appt, appt.PatientID);

                if (completeForm.ShowDialog() == DialogResult.OK)
                {
                    // ✅ No delete — appointment is already marked Done inside CompleteAppointmentForm
                    // Queries filter by Status != 'Done' so it won't show up anymore
                    isLoading = true;
                    try
                    {
                        await LoadAppointmentDatesAsync();
                        await LoadAppointmentsAsync(monthCalendar1.SelectionStart);
                        currentPage = 1;
                        await LoadAllAppointmentsPagedAsync(currentPage);
                    }
                    finally
                    {
                        isLoading = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("Appointment not found");
            }
        }

        private async Task<AppointmentDetail> GetAppointmentDetailsAsync(int appointmentID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = @"SELECT a.AppointmentID, a.PatientID, a.PatientName, a.AppointmentDate, 
                                           a.AppointmentTime, s.ServiceName, a.Status
                                    FROM Appointments a
                                    LEFT JOIN DentalServices s ON a.ServiceID = s.ServiceID
                                    WHERE a.AppointmentID = @AppointmentID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentID);

                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new AppointmentDetail
                            {
                                AppointmentID = Convert.ToInt32(reader["AppointmentID"]),
                                PatientID = Convert.ToInt32(reader["PatientID"]),
                                PatientName = reader["PatientName"].ToString(),
                                AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]),
                                AppointmentTime = TimeSpan.Parse(reader["AppointmentTime"].ToString()),
                                ServiceName = reader["ServiceName"]?.ToString() ?? "N/A",
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }

        public async Task<int> GetTodayAppointmentsCountAsync()
        {
            try
            {
                int count = 0;
                using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
                {
                    string query = "SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDate AS DATE) = CAST(GETDATE() AS DATE) AND Status != 'Cancelled' AND Status != 'Done'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 30;

                    await conn.OpenAsync();
                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                        count = (int)result;
                }
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return 0;
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e) { }
        private void flowAppointments_Paint(object sender, PaintEventArgs e) { }
        private void FlowAllAppointments_Paint(object sender, PaintEventArgs e) { }
    }

    public class AppointmentDetail
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
    }
}