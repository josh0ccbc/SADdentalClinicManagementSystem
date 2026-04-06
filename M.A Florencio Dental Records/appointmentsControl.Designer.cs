namespace M.A_Florencio_Dental_Records
{
    partial class appointmentsControl
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
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.label2 = new System.Windows.Forms.Label();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.flowAppointments = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowAllAppointments = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.CalendarDimensions = new System.Drawing.Size(3, 1);
            this.monthCalendar1.Location = new System.Drawing.Point(87, 92);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 37);
            this.label2.TabIndex = 12;
            this.label2.Text = "Appointments";
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.CharacterCasing = MaterialSkin.Controls.MaterialButton.CharacterCasingEnum.Normal;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(700, 21);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(157, 36);
            this.materialButton1.TabIndex = 14;
            this.materialButton1.Text = "Add Appointment";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Divider;
            this.pictureBox3.Location = new System.Drawing.Point(17, 66);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(846, 14);
            this.pictureBox3.TabIndex = 11;
            this.pictureBox3.TabStop = false;
            // 
            // flowAppointments
            // 
            this.flowAppointments.AutoScroll = true;
            this.flowAppointments.Location = new System.Drawing.Point(16, 266);
            this.flowAppointments.Name = "flowAppointments";
            this.flowAppointments.Size = new System.Drawing.Size(845, 215);
            this.flowAppointments.TabIndex = 15;
            this.flowAppointments.Paint += new System.Windows.Forms.PaintEventHandler(this.flowAppointments_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 496);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 32);
            this.label1.TabIndex = 17;
            this.label1.Text = "All Appointments";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Divider;
            this.pictureBox1.Location = new System.Drawing.Point(15, 533);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(846, 14);
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // flowAllAppointments
            // 
            this.flowAllAppointments.Location = new System.Drawing.Point(16, 556);
            this.flowAllAppointments.Name = "flowAllAppointments";
            this.flowAllAppointments.Size = new System.Drawing.Size(844, 285);
            this.flowAllAppointments.TabIndex = 18;
            this.flowAllAppointments.Paint += new System.Windows.Forms.PaintEventHandler(this.FlowAllAppointments_Paint);
            // 
            // appointmentsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.Controls.Add(this.flowAllAppointments);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.flowAppointments);
            this.Controls.Add(this.materialButton1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.monthCalendar1);
            this.Name = "appointmentsControl";
            this.Size = new System.Drawing.Size(871, 860);
            this.Load += new System.EventHandler(this.appointmentsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private System.Windows.Forms.FlowLayoutPanel flowAppointments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowAllAppointments;
    }
}
