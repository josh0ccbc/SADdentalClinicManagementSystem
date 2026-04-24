using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SqlClient;

namespace M.A_Florencio_Dental_Records
{
    public partial class DentalChartForm : MaterialForm
    {
        private int _patientID;

        // Default constructor — needed by MaterialForm
        public DentalChartForm()
        {
            InitializeComponent();
        }

        public DentalChartForm(int patientID)
        {
            InitializeComponent();
            _patientID = patientID;
        }

        private void DentalChartForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT FullName FROM Patients WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", _patientID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    this.Text = result.ToString() + " - Dental Chart";
            }

            // Populate all comboboxes
            string[] conditionCodes = { "", "/", "D", "M", "MO", "Im", "Sp", "Rf", "Un", "X", "XO" };
            string[] restorationCodes = { "", "Am", "Co", "JC", "Ab", "Att", "P", "In", "Imp", "S", "Rm" };

            var conditionBoxes = new[] {
        comboBox1,  comboBox2,  comboBox3,  comboBox4,  comboBox5,
        comboBox6,  comboBox7,  comboBox10, comboBox17, comboBox19,
        comboBox21, comboBox22, comboBox25, comboBox27, comboBox29,
        comboBox31, comboBox32, comboBox35, comboBox37, comboBox38,
        comboBox39, comboBox40, comboBox45, comboBox46, comboBox49,
        comboBox51, comboBox53, comboBox54, comboBox55, comboBox61,
        comboBox62, comboBox65, comboBox67, comboBox69, comboBox70,
        comboBox71, comboBox72, comboBox77, comboBox78, comboBox81,
        comboBox83, comboBox85, comboBox86, comboBox88, comboBox90,
        comboBox91, comboBox95, comboBox97, comboBox99, comboBox101,
        comboBox103
    };

            var restorationBoxes = new[] {
        comboBox8,  comboBox9,  comboBox11, comboBox12, comboBox13,
        comboBox14, comboBox15, comboBox16, comboBox18, comboBox20,
        comboBox23, comboBox24, comboBox26, comboBox28, comboBox30,
        comboBox33, comboBox34, comboBox36, comboBox41, comboBox42,
        comboBox43, comboBox44, comboBox47, comboBox48, comboBox50,
        comboBox52, comboBox57, comboBox58, comboBox59, comboBox60,
        comboBox63, comboBox64, comboBox66, comboBox68, comboBox73,
        comboBox74, comboBox75, comboBox76, comboBox79, comboBox80,
        comboBox82, comboBox84, comboBox87, comboBox89, comboBox92,
        comboBox93, comboBox94, comboBox96, comboBox98, comboBox100,
        comboBox102, comboBox104
    };

            foreach (var cb in conditionBoxes)
                cb.Items.AddRange(conditionCodes);

            foreach (var cb in restorationBoxes)
                cb.Items.AddRange(restorationCodes);

            // Load existing chart if patient already has one
            LoadDentalChartFromDB();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDentalChartToDB();
            MessageBox.Show("Dental chart saved!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoadDentalChartFromDB()
        {
            var teeth = new Dictionary<string, (ComboBox condition, ComboBox restoration)>
    {
        { "55", (comboBox5,  comboBox15) },
        { "54", (comboBox1,  comboBox11) },
        { "53", (comboBox2,  comboBox12) },
        { "52", (comboBox3,  comboBox13) },
        { "51", (comboBox4,  comboBox14) },
        { "61", (comboBox19, comboBox20) },
        { "62", (comboBox6,  comboBox8)  },
        { "63", (comboBox7,  comboBox9)  },
        { "64", (comboBox10, comboBox16) },
        { "65", (comboBox17, comboBox18) },
        { "18", (comboBox31, comboBox33) },
        { "17", (comboBox32, comboBox34) },
        { "16", (comboBox35, comboBox36) },
        { "15", (comboBox29, comboBox30) },
        { "14", (comboBox21, comboBox23) },
        { "13", (comboBox22, comboBox24) },
        { "12", (comboBox25, comboBox26) },
        { "11", (comboBox27, comboBox28) },
        { "21", (comboBox38, comboBox42) },
        { "22", (comboBox40, comboBox44) },
        { "23", (comboBox46, comboBox48) },
        { "24", (comboBox51, comboBox52) },
        { "25", (comboBox37, comboBox41) },
        { "26", (comboBox39, comboBox43) },
        { "27", (comboBox45, comboBox47) },
        { "28", (comboBox49, comboBox50) },
        { "48", (comboBox54, comboBox58) },
        { "47", (comboBox56, comboBox60) },
        { "46", (comboBox62, comboBox64) },
        { "45", (comboBox67, comboBox68) },
        { "44", (comboBox53, comboBox57) },
        { "43", (comboBox55, comboBox59) },
        { "42", (comboBox61, comboBox63) },
        { "41", (comboBox65, comboBox66) },
        { "31", (comboBox70, comboBox74) },
        { "32", (comboBox72, comboBox76) },
        { "33", (comboBox78, comboBox80) },
        { "34", (comboBox83, comboBox84) },
        { "35", (comboBox69, comboBox73) },
        { "36", (comboBox71, comboBox75) },
        { "37", (comboBox77, comboBox79) },
        { "38", (comboBox81, comboBox82) },
        { "85", (comboBox99,  comboBox100) },
        { "84", (comboBox85,  comboBox87)  },
        { "83", (comboBox86,  comboBox89)  },
        { "82", (comboBox91,  comboBox93)  },
        { "81", (comboBox95,  comboBox96)  },
        { "71", (comboBox103, comboBox104) },
        { "72", (comboBox88,  comboBox92)  },
        { "73", (comboBox90,  comboBox94)  },
        { "74", (comboBox97,  comboBox98)  },
        { "75", (comboBox101, comboBox102) },
    };

            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                string query = "SELECT ToothNumber, Condition, Restoration FROM DentalChart WHERE PatientID = @PatientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientID", _patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string tooth = reader["ToothNumber"].ToString();
                    string condition = reader["Condition"].ToString();
                    string restoration = reader["Restoration"].ToString();

                    if (teeth.ContainsKey(tooth))
                    {
                        teeth[tooth].condition.Text = condition;
                        teeth[tooth].restoration.Text = restoration;
                    }
                }
            }
        }

        private void SaveDentalChartToDB()
        {
            // Map each tooth number to its condition and restoration comboboxes
            var teeth = new Dictionary<string, (ComboBox condition, ComboBox restoration)>
            {
                { "55", (comboBox5,  comboBox15) },
                { "54", (comboBox1,  comboBox11) },
                { "53", (comboBox2,  comboBox12) },
                { "52", (comboBox3,  comboBox13) },
                { "51", (comboBox4,  comboBox14) },
                { "61", (comboBox19, comboBox20) },
                { "62", (comboBox6,  comboBox8)  },
                { "63", (comboBox7,  comboBox9)  },
                { "64", (comboBox10, comboBox16) },
                { "65", (comboBox17, comboBox18) },
                // permanent upper
                { "18", (comboBox31, comboBox33) },
                { "17", (comboBox32, comboBox34) },
                { "16", (comboBox35, comboBox36) },
                { "15", (comboBox29, comboBox30) },
                { "14", (comboBox21, comboBox23) },
                { "13", (comboBox22, comboBox24) },
                { "12", (comboBox25, comboBox26) },
                { "11", (comboBox27, comboBox28) },
                { "21", (comboBox38, comboBox42) },
                { "22", (comboBox40, comboBox44) },
                { "23", (comboBox46, comboBox48) },
                { "24", (comboBox51, comboBox52) },
                { "25", (comboBox37, comboBox41) },
                { "26", (comboBox39, comboBox43) },
                { "27", (comboBox45, comboBox47) },
                { "28", (comboBox49, comboBox50) },
                // permanent lower
                { "48", (comboBox54, comboBox58) },
                { "47", (comboBox56, comboBox60) },
                { "46", (comboBox62, comboBox64) },
                { "45", (comboBox67, comboBox68) },
                { "44", (comboBox53, comboBox57) },
                { "43", (comboBox55, comboBox59) },
                { "42", (comboBox61, comboBox63) },
                { "41", (comboBox65, comboBox66) },
                { "31", (comboBox70, comboBox74) },
                { "32", (comboBox72, comboBox76) },
                { "33", (comboBox78, comboBox80) },
                { "34", (comboBox83, comboBox84) },
                { "35", (comboBox69, comboBox73) },
                { "36", (comboBox71, comboBox75) },
                { "37", (comboBox77, comboBox79) },
                { "38", (comboBox81, comboBox82) },
                // temporary lower
                { "85", (comboBox99,  comboBox100) },
                { "84", (comboBox85,  comboBox87)  },
                { "83", (comboBox86,  comboBox89)  },
                { "82", (comboBox91,  comboBox93)  },
                { "81", (comboBox95,  comboBox96)  },
                { "71", (comboBox103, comboBox104) },
                { "72", (comboBox88,  comboBox92)  },
                { "73", (comboBox90,  comboBox94)  },
                { "74", (comboBox97,  comboBox98)  },
                { "75", (comboBox101, comboBox102) },
            };

            using (SqlConnection conn = new SqlConnection(ConnectionHelper.GetConnectionString()))
            {
                conn.Open();

                // Delete existing chart for this patient first
                string delete = "DELETE FROM DentalChart WHERE PatientID = @PatientID";
                SqlCommand delCmd = new SqlCommand(delete, conn);
                delCmd.Parameters.AddWithValue("@PatientID", _patientID);
                delCmd.ExecuteNonQuery();

                // Insert one row per tooth
                string insert = @"INSERT INTO DentalChart 
            (PatientID, ToothNumber, Condition, Restoration, DateRecorded)
            VALUES (@PatientID, @ToothNumber, @Condition, @Restoration, @DateRecorded)";

                foreach (var tooth in teeth)
                {
                    string condition = tooth.Value.condition.Text.Trim();
                    string restoration = tooth.Value.restoration.Text.Trim();

                    if (string.IsNullOrEmpty(condition) && string.IsNullOrEmpty(restoration))
                        continue; // skip empty teeth

                    SqlCommand cmd = new SqlCommand(insert, conn);
                    cmd.Parameters.AddWithValue("@PatientID", _patientID);
                    cmd.Parameters.AddWithValue("@ToothNumber", tooth.Key);
                    cmd.Parameters.AddWithValue("@Condition", condition);
                    cmd.Parameters.AddWithValue("@Restoration", restoration);
                    cmd.Parameters.AddWithValue("@DateRecorded", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox104_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox20_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox103_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox102_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox19_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox101_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox52_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox84_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox68_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox30_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox100_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox15_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox51_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox83_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox63_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox29_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox99_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox98_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox18_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox97_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox50_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox82_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox66_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox28_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox81_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox65_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox49_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox27_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox96_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox95_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox14_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox80_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox48_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox79_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void comboBox47_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox26_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox46_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox78_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox94_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox45_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox77_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox93_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox61_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox13_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox25_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox44_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox76_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox92_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox43_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox75_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox91_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox59_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox24_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox42_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox74_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox90_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox89_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox41_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox12_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox73_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox57_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox40_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox72_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox88_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox39_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox71_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox55_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox38_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox70_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox22_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox69_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox37_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox87_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox53_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox86_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox21_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox85_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox67_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox60_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox34_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox58_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox33_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox56_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox32_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox54_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox31_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox35_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox36_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox62_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox64_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}