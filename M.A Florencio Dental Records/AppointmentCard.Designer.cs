namespace M.A_Florencio_Dental_Records
{
    partial class AppointmentCard
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
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblService = new System.Windows.Forms.Label();
            this.lblDateTime = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnMarkAsDone = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPatientName
            // 
            this.lblPatientName.AutoSize = true;
            this.lblPatientName.Location = new System.Drawing.Point(9, 8);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(78, 13);
            this.lblPatientName.TabIndex = 0;
            this.lblPatientName.Text = "lblPatientName";
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(217, 12);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(53, 13);
            this.lblService.TabIndex = 1;
            this.lblService.Text = "lblService";
            this.lblService.Click += new System.EventHandler(this.label2_Click);
            // 
            // lblDateTime
            // 
            this.lblDateTime.AutoSize = true;
            this.lblDateTime.Location = new System.Drawing.Point(381, 12);
            this.lblDateTime.Name = "lblDateTime";
            this.lblDateTime.Size = new System.Drawing.Size(30, 13);
            this.lblDateTime.TabIndex = 2;
            this.lblDateTime.Text = "Date";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(563, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "lblStatus";
            // 
            // btnMarkAsDone
            // 
            this.btnMarkAsDone.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.checkmark;
            this.btnMarkAsDone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMarkAsDone.FlatAppearance.BorderSize = 0;
            this.btnMarkAsDone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarkAsDone.Location = new System.Drawing.Point(761, 8);
            this.btnMarkAsDone.Name = "btnMarkAsDone";
            this.btnMarkAsDone.Size = new System.Drawing.Size(25, 25);
            this.btnMarkAsDone.TabIndex = 8;
            this.btnMarkAsDone.UseVisualStyleBackColor = true;
            this.btnMarkAsDone.Click += new System.EventHandler(this.btnMarkAsDone_Click);
            this.btnMarkAsDone.MouseEnter += new System.EventHandler(this.btnMarkAsDone_MouseEnter);
            this.btnMarkAsDone.MouseLeave += new System.EventHandler(this.btnMarkAsDone_MouseLeave);
            // 
            // btnDelete
            // 
            this.btnDelete.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.cancel;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(796, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(29, 29);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnDelete.MouseEnter += new System.EventHandler(this.btnDelete_MouseEnter_1);
            this.btnDelete.MouseLeave += new System.EventHandler(this.btnDelete_MouseLeave_1);
            // 
            // AppointmentCard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.btnMarkAsDone);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.lblService);
            this.Controls.Add(this.lblPatientName);
            this.Name = "AppointmentCard";
            this.Size = new System.Drawing.Size(836, 42);
            this.Load += new System.EventHandler(this.AppointmentCard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPatientName;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.Label lblDateTime;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnMarkAsDone;
    }
}
