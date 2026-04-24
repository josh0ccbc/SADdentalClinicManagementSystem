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
    public partial class MedicalRecordCard : UserControl
    {
        public int RecordID { get; private set; }
        public event EventHandler<int> OnViewClicked;
        public event EventHandler<int> OnEditClicked;

        public MedicalRecordCard()
        {
            InitializeComponent();

            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(button1, "View Patient Medical Record");
            toolTip.SetToolTip(btnEdit, "Edit Patient Medical Record");
            toolTip.SetToolTip(btnViewDC, "View Dental Chart");
        }

        private void MedicalRecordCard_Load(object sender, EventArgs e)
        {

        }

        public void SetRecord(int recordID, DateTime visitDate, string appointmentTime,
                          string diagnosis, string procedure, string notes)
        {
            RecordID = recordID;

            lblDate.Text = visitDate.ToString("MMM dd, yyyy") +
                           (string.IsNullOrEmpty(appointmentTime) ? "" : "  at  " + appointmentTime);

            lblDiagnosis.Text = (string.IsNullOrEmpty(diagnosis) ? "N/A" : diagnosis);
            lblProcedure.Text = (string.IsNullOrEmpty(procedure) ? "N/A" : procedure);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.view2;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackgroundImage = Properties.Resources.view;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnViewClicked?.Invoke(this, RecordID);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            OnEditClicked?.Invoke(this, RecordID);
        }

        private void btnEdit_MouseLeave(object sender, EventArgs e)
        {
            btnEdit.BackgroundImage = Properties.Resources.edit;
        }

        private void btnEdit_MouseEnter(object sender, EventArgs e)
        {
            btnEdit.BackgroundImage = Properties.Resources.edit2;
        }

        private void btnViewDC_MouseEnter(object sender, EventArgs e)
        {
            btnViewDC.BackgroundImage = Properties.Resources.tooth2;
        }

        private void btnViewDC_MouseLeave(object sender, EventArgs e)
        {
            btnViewDC.BackgroundImage = Properties.Resources.tooth;
        }
    }
}
