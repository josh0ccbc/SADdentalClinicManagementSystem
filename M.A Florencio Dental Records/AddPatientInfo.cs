using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    public partial class AddPatientInfo : UserControl
    {
        public PatientData patientData;

        public AddPatientInfo()
        {
            InitializeComponent();
            patientData = new PatientData();
        }

        private void AddPatientInfo_Load(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dtpBirthDate.Value > DateTime.Today)
            {
                MessageBox.Show("Birth date cannot be in the future!");
                dtpBirthDate.Value = DateTime.Today.AddYears(-18);
                return;
            }

            int age = CalculateAge(dtpBirthDate.Value);
            txtAge.Text = age.ToString();

            if (age < 18)
            {
                label10.Visible = true;
                Gname.Visible = true;
                label11.Visible = true;
                Gcontact.Visible = true;
                label12.Visible = true;
                Grelationship.Visible = true;
                label13.Visible = true;
            }
            else
            {
                label10.Visible = false;
                Gname.Visible = false;
                label11.Visible = false;
                Gcontact.Visible = false;
                label12.Visible = false;
                Grelationship.Visible = false;
                label13.Visible = false;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Full Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbGender.Text))
            {
                MessageBox.Show("Gender is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGender.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContact.Text))
            {
                MessageBox.Show("Contact Number is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (!long.TryParse(txtContact.Text, out long contact))
            {
                MessageBox.Show("Contact must be numbers only!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (txtContact.Text.Length < 10 || txtContact.Text.Length > 11)
            {
                MessageBox.Show("Contact must be 10-11 digits!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContact.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Address is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAddress.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbCivilStatus.Text))
            {
                MessageBox.Show("Civil Status is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCivilStatus.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtReligion.Text))
            {
                MessageBox.Show("Religion is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReligion.Focus();
                return false;
            }

            // Guardian validation - only if age < 18
            int age = int.Parse(txtAge.Text);
            if (age < 18)
            {
                if (string.IsNullOrWhiteSpace(Gname.Text))
                {
                    MessageBox.Show("Guardian Name is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gname.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Gcontact.Text))
                {
                    MessageBox.Show("Guardian Contact is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gcontact.Focus();
                    return false;
                }

                if (!long.TryParse(Gcontact.Text, out long gcontact))
                {
                    MessageBox.Show("Guardian Contact must be numbers only!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Gcontact.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Grelationship.Text))
                {
                    MessageBox.Show("Guardian Relationship is required!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Grelationship.Focus();
                    return false;
                }
            }

            return true;
        }

        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = (Form2)this.FindForm();
            form2.returningToMain = true; 

            Form1 mainForm = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (mainForm != null)
            {
                mainForm.Show();
                mainForm.LoadControl(new DBcontrol());
            }
            form2.Close();
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button4;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackgroundImage = Properties.Resources.Button3;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            patientData.FullName = txtFullName.Text;
            patientData.Gender = cmbGender.Text;
            patientData.BirthDate = dtpBirthDate.Value;
            patientData.Age = int.Parse(txtAge.Text);
            patientData.ContactNumber = txtContact.Text;
            patientData.Address = txtAddress.Text;
            patientData.CivilStatus = cmbCivilStatus.Text;
            patientData.Religion = txtReligion.Text;
            patientData.GuardianName = Gname.Text;
            patientData.GuardianContact = Gcontact.Text;
            patientData.GuardianRelationship = Grelationship.Text;

            Form2 form2 = (Form2)this.FindForm();
            MedicalInfo medicalInfo = new MedicalInfo();
            medicalInfo.patientData = patientData; 
            form2.LoadControl(medicalInfo);
        }

        private void txtFullName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
