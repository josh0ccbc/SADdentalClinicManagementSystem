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
using MaterialSkin;
using MaterialSkin.Controls;

namespace M.A_Florencio_Dental_Records
{
    public partial class Form2 : MaterialForm
    {
        private Form1 mainForm;
        public bool returningToMain = false;

        public Form2(Form1 parent)
        {
            InitializeComponent();
            mainForm = parent;
        }

        public Form2()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;


        }

        public void LoadControl(UserControl control)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(control);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.LoadControl(new AddPatientInfo());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            returningToMain = true;

            mainForm.LoadControl(new DBcontrol());
            mainForm.Show();
            this.Close();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.HoverArrowBack;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.ArrowBack;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            returningToMain = true;

            mainForm.LoadControl(new DBcontrol());
            mainForm.Show();
            this.Close();
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {

        }

        private void Form2_MouseEnter(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtFullName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAge_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtContact_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContact_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void Form2_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (!returningToMain && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Cancel the close
                Application.Exit();
            }
        }
    }
}
