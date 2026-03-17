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

namespace M.A_Florencio_Dental_Records
{
    public partial class Form1 : MaterialForm
    {
        Button NavButton;

        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.AddFormToManage(this);

            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal500,
                Primary.Teal700,
                Primary.Teal200,
                Accent.Teal200,
                TextShade.WHITE
            );        
        }

        void UseButton(Button btn)
        {
            if (NavButton != null)
            {
                NavButton.BackColor = Color.FromArgb(255, 255, 255); 
                NavButton.ForeColor = Color.Black;
            }

            NavButton = btn;
            NavButton.BackColor = Color.FromArgb(95, 158, 160); 
            NavButton.ForeColor = Color.White;
        }

        void ActivateButton(Button btn)
        {
            if (NavButton != null)
            {
                NavButton.BackColor = Color.FromArgb(255, 255, 255);
                NavButton.ForeColor = Color.Black;
            }

            button1.BackColor = Color.White;
            button1.ForeColor = Color.Black;

            NavButton = btn;
            NavButton.BackColor = Color.FromArgb(95, 158, 160);
            NavButton.ForeColor = Color.White;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadControl(new DBcontrol());
            FormPnl.Visible = true;
            ActivateButton(button1);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormPnl.Visible = true;
            LoadControl(new DBcontrol());
            ActivateButton((Button)sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActivateButton((Button)sender);
            LoadControl(new patientControl());
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void BTNAddPatient_MouseEnter_1(object sender, EventArgs e)
        {
            
        }

        private void BTNAddPatient_MouseLeave(object sender, EventArgs e)
        {
            
        }

        private void BTNAddPatient_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        public void LoadControl(UserControl control)
        {
            FormPnl.Controls.Clear();
            FormPnl.Controls.Add(control);
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadControl(new appointmentsControl());
            ActivateButton((Button)sender);
        }

        private void NAVArchive_Click(object sender, EventArgs e)
        {
            LoadControl(new Archive());
            ActivateButton((Button)sender);
        }
    }
}
