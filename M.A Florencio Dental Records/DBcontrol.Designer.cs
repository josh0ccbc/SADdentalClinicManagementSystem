namespace M.A_Florencio_Dental_Records
{
    partial class DBcontrol
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BTNAddPatient = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPatientCount = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAppointmentCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // BTNAddPatient
            // 
            this.BTNAddPatient.BackColor = System.Drawing.Color.Transparent;
            this.BTNAddPatient.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.Button;
            this.BTNAddPatient.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BTNAddPatient.FlatAppearance.BorderSize = 0;
            this.BTNAddPatient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BTNAddPatient.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNAddPatient.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BTNAddPatient.Location = new System.Drawing.Point(572, 34);
            this.BTNAddPatient.Name = "BTNAddPatient";
            this.BTNAddPatient.Size = new System.Drawing.Size(131, 30);
            this.BTNAddPatient.TabIndex = 7;
            this.BTNAddPatient.Text = "+ New Patient";
            this.BTNAddPatient.UseVisualStyleBackColor = false;
            this.BTNAddPatient.Click += new System.EventHandler(this.BTNAddPatient_Click);
            this.BTNAddPatient.MouseEnter += new System.EventHandler(this.BTNAddPatient_MouseEnter);
            this.BTNAddPatient.MouseLeave += new System.EventHandler(this.BTNAddPatient_MouseLeave);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Divider;
            this.pictureBox3.Location = new System.Drawing.Point(23, 86);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(678, 10);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Container;
            this.pictureBox4.Location = new System.Drawing.Point(23, 116);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(242, 127);
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.CadetBlue;
            this.label5.Location = new System.Drawing.Point(38, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 22);
            this.label5.TabIndex = 11;
            this.label5.Text = "TOTAL PATIENTS";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // lblPatientCount
            // 
            this.lblPatientCount.BackColor = System.Drawing.Color.White;
            this.lblPatientCount.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientCount.Location = new System.Drawing.Point(63, 166);
            this.lblPatientCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblPatientCount.Name = "lblPatientCount";
            this.lblPatientCount.Padding = new System.Windows.Forms.Padding(45, 10, 45, 10);
            this.lblPatientCount.Size = new System.Drawing.Size(161, 42);
            this.lblPatientCount.TabIndex = 12;
            this.lblPatientCount.Text = "0";
            this.lblPatientCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Container;
            this.pictureBox5.Location = new System.Drawing.Point(271, 116);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(242, 127);
            this.pictureBox5.TabIndex = 13;
            this.pictureBox5.TabStop = false;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.CadetBlue;
            this.label6.Location = new System.Drawing.Point(280, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(227, 22);
            this.label6.TabIndex = 14;
            this.label6.Text = "TODAY\'S APPOINTMENTS";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAppointmentCount
            // 
            this.lblAppointmentCount.AutoSize = true;
            this.lblAppointmentCount.BackColor = System.Drawing.Color.White;
            this.lblAppointmentCount.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAppointmentCount.Location = new System.Drawing.Point(342, 166);
            this.lblAppointmentCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblAppointmentCount.Name = "lblAppointmentCount";
            this.lblAppointmentCount.Padding = new System.Windows.Forms.Padding(45, 10, 45, 10);
            this.lblAppointmentCount.Size = new System.Drawing.Size(108, 38);
            this.lblAppointmentCount.TabIndex = 15;
            this.lblAppointmentCount.Text = "0";
            this.lblAppointmentCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(23, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(254, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Overview of today\'s operations";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 37);
            this.label2.TabIndex = 5;
            this.label2.Text = "Clinic Dashboard";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // DBcontrol
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblPatientCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.lblAppointmentCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.BTNAddPatient);
            this.Name = "DBcontrol";
            this.Size = new System.Drawing.Size(737, 458);
            this.Load += new System.EventHandler(this.DBcontrol_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BTNAddPatient;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPatientCount;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAppointmentCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}
