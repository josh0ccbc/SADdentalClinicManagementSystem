using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M.A_Florencio_Dental_Records
{
    // Add this as a new class at the bottom of AppointmentCard.cs, outside the AppointmentCard class
    public partial class CancelReasonDialog : Form
    {
        public string Reason { get; private set; }

        private TextBox txtReason;
        private Button btnConfirm;
        private Button btnCancel;
        private Label lblPrompt;

        public CancelReasonDialog()
        {
            this.Text = "Cancellation Reason";
            this.Size = new Size(380, 220);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblPrompt = new Label();
            lblPrompt.Text = "Please enter the reason for cancellation:";
            lblPrompt.Location = new Point(15, 15);
            lblPrompt.Size = new Size(340, 20);

            txtReason = new TextBox();
            txtReason.Location = new Point(15, 45);
            txtReason.Size = new Size(340, 80);
            txtReason.Multiline = true;
            txtReason.ScrollBars = ScrollBars.Vertical;

            btnConfirm = new Button();
            btnConfirm.Text = "Confirm Cancellation";
            btnConfirm.Location = new Point(15, 140);
            btnConfirm.Size = new Size(160, 35);
            btnConfirm.BackColor = Color.Crimson;
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    MessageBox.Show("Please enter a reason for cancellation.", "Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Reason = txtReason.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancel = new Button();
            btnCancel.Text = "Go Back";
            btnCancel.Location = new Point(195, 140);
            btnCancel.Size = new Size(160, 35);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(lblPrompt);
            this.Controls.Add(txtReason);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }
    }
}