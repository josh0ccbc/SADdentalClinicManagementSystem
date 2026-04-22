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
            this.label5 = new System.Windows.Forms.Label();
            this.lblPatientCount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTodayAppointments = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BTNAddPatient = new MaterialSkin.Controls.MaterialButton();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblUpcomingAppointments = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.CadetBlue;
            this.label5.Location = new System.Drawing.Point(44, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(216, 22);
            this.label5.TabIndex = 11;
            this.label5.Text = "Total Patients";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // lblPatientCount
            // 
            this.lblPatientCount.BackColor = System.Drawing.Color.White;
            this.lblPatientCount.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPatientCount.Location = new System.Drawing.Point(69, 166);
            this.lblPatientCount.Margin = new System.Windows.Forms.Padding(0);
            this.lblPatientCount.Name = "lblPatientCount";
            this.lblPatientCount.Padding = new System.Windows.Forms.Padding(45, 10, 45, 10);
            this.lblPatientCount.Size = new System.Drawing.Size(161, 42);
            this.lblPatientCount.TabIndex = 12;
            this.lblPatientCount.Text = "0";
            this.lblPatientCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPatientCount.Click += new System.EventHandler(this.lblPatientCount_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.CadetBlue;
            this.label6.Location = new System.Drawing.Point(329, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(227, 22);
            this.label6.TabIndex = 14;
            this.label6.Text = "Today\'s Appointments";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // lblTodayAppointments
            // 
            this.lblTodayAppointments.AutoSize = true;
            this.lblTodayAppointments.BackColor = System.Drawing.Color.White;
            this.lblTodayAppointments.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTodayAppointments.Location = new System.Drawing.Point(391, 166);
            this.lblTodayAppointments.Margin = new System.Windows.Forms.Padding(0);
            this.lblTodayAppointments.Name = "lblTodayAppointments";
            this.lblTodayAppointments.Padding = new System.Windows.Forms.Padding(45, 10, 45, 10);
            this.lblTodayAppointments.Size = new System.Drawing.Size(108, 38);
            this.lblTodayAppointments.TabIndex = 15;
            this.lblTodayAppointments.Text = "0";
            this.lblTodayAppointments.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTodayAppointments.Click += new System.EventHandler(this.lblTodayAppointments_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Location = new System.Drawing.Point(31, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(254, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Overview of today\'s operations";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(285, 37);
            this.label2.TabIndex = 5;
            this.label2.Text = "Clinic Dashboard";
            // 
            // BTNAddPatient
            // 
            this.BTNAddPatient.AutoSize = false;
            this.BTNAddPatient.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BTNAddPatient.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.BTNAddPatient.Depth = 0;
            this.BTNAddPatient.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNAddPatient.HighEmphasis = true;
            this.BTNAddPatient.Icon = null;
            this.BTNAddPatient.Location = new System.Drawing.Point(709, 32);
            this.BTNAddPatient.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.BTNAddPatient.MouseState = MaterialSkin.MouseState.HOVER;
            this.BTNAddPatient.Name = "BTNAddPatient";
            this.BTNAddPatient.NoAccentTextColor = System.Drawing.Color.Empty;
            this.BTNAddPatient.Size = new System.Drawing.Size(146, 36);
            this.BTNAddPatient.TabIndex = 16;
            this.BTNAddPatient.Text = "+ ADD NEW PATIENT";
            this.BTNAddPatient.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.BTNAddPatient.UseAccentColor = false;
            this.BTNAddPatient.UseVisualStyleBackColor = true;
            this.BTNAddPatient.Click += new System.EventHandler(this.BTNAddPatient_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.CadetBlue;
            this.label1.Location = new System.Drawing.Point(621, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 22);
            this.label1.TabIndex = 14;
            this.label1.Text = "Upcoming Appointments";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Container;
            this.pictureBox4.Location = new System.Drawing.Point(29, 116);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(242, 127);
            this.pictureBox4.TabIndex = 9;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Divider;
            this.pictureBox3.Location = new System.Drawing.Point(32, 86);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(823, 24);
            this.pictureBox3.TabIndex = 8;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Container;
            this.pictureBox5.Location = new System.Drawing.Point(320, 116);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(242, 127);
            this.pictureBox5.TabIndex = 13;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Container;
            this.pictureBox1.Location = new System.Drawing.Point(613, 116);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(242, 127);
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblUpcomingAppointments
            // 
            this.lblUpcomingAppointments.AutoSize = true;
            this.lblUpcomingAppointments.BackColor = System.Drawing.Color.White;
            this.lblUpcomingAppointments.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpcomingAppointments.Location = new System.Drawing.Point(682, 170);
            this.lblUpcomingAppointments.Margin = new System.Windows.Forms.Padding(0);
            this.lblUpcomingAppointments.Name = "lblUpcomingAppointments";
            this.lblUpcomingAppointments.Padding = new System.Windows.Forms.Padding(45, 10, 45, 10);
            this.lblUpcomingAppointments.Size = new System.Drawing.Size(108, 38);
            this.lblUpcomingAppointments.TabIndex = 15;
            this.lblUpcomingAppointments.Text = "0";
            this.lblUpcomingAppointments.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUpcomingAppointments.Click += new System.EventHandler(this.lblUpcomingAppointments_Click);
            // 
            // DBcontrol
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.BTNAddPatient);
            this.Controls.Add(this.lblPatientCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblUpcomingAppointments);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.lblTodayAppointments);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "DBcontrol";
            this.Size = new System.Drawing.Size(899, 559);
            this.Load += new System.EventHandler(this.DBcontrol_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblPatientCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTodayAppointments;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private MaterialSkin.Controls.MaterialButton BTNAddPatient;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblUpcomingAppointments;
    }
}
