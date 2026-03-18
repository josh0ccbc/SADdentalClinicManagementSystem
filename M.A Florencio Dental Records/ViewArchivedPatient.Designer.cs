namespace M.A_Florencio_Dental_Records
{
    partial class ViewArchivedPatient
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblMedicalHistory = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblContact = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblBirthDate = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblPatientID = new System.Windows.Forms.Label();
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblCivilStatus = new System.Windows.Forms.Label();
            this.lblReligion = new System.Windows.Forms.Label();
            this.panelGuardian = new System.Windows.Forms.Panel();
            this.lblGuardianName = new System.Windows.Forms.Label();
            this.lblGuardianContact = new System.Windows.Forms.Label();
            this.lblGuardianRelationship = new System.Windows.Forms.Label();
            this.panelGuardian.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(767, 380);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 20;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(668, 380);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 19;
            this.btnDelete.Text = "Unarchive";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnUnarchive_Click);
            // 
            // lblMedicalHistory
            // 
            this.lblMedicalHistory.AutoSize = true;
            this.lblMedicalHistory.Location = new System.Drawing.Point(67, 288);
            this.lblMedicalHistory.Name = "lblMedicalHistory";
            this.lblMedicalHistory.Size = new System.Drawing.Size(79, 13);
            this.lblMedicalHistory.TabIndex = 18;
            this.lblMedicalHistory.Text = "Medical History";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(67, 260);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 17;
            this.lblAddress.Text = "Address";
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(67, 232);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(44, 13);
            this.lblContact.TabIndex = 16;
            this.lblContact.Text = "Contact";
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Location = new System.Drawing.Point(67, 204);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(26, 13);
            this.lblAge.TabIndex = 15;
            this.lblAge.Text = "Age";
            // 
            // lblBirthDate
            // 
            this.lblBirthDate.AutoSize = true;
            this.lblBirthDate.Location = new System.Drawing.Point(67, 175);
            this.lblBirthDate.Name = "lblBirthDate";
            this.lblBirthDate.Size = new System.Drawing.Size(51, 13);
            this.lblBirthDate.TabIndex = 14;
            this.lblBirthDate.Text = "BirthDate";
            // 
            // lblGender
            // 
            this.lblGender.AutoSize = true;
            this.lblGender.Location = new System.Drawing.Point(67, 148);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(42, 13);
            this.lblGender.TabIndex = 13;
            this.lblGender.Text = "Gender";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(16, 13);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 12;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblPatientID
            // 
            this.lblPatientID.AutoSize = true;
            this.lblPatientID.Location = new System.Drawing.Point(67, 119);
            this.lblPatientID.Name = "lblPatientID";
            this.lblPatientID.Size = new System.Drawing.Size(54, 13);
            this.lblPatientID.TabIndex = 10;
            this.lblPatientID.Text = "Patient ID";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Location = new System.Drawing.Point(67, 91);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(35, 13);
            this.lblFullName.TabIndex = 11;
            this.lblFullName.Text = "Name";
            // 
            // lblCivilStatus
            // 
            this.lblCivilStatus.AutoSize = true;
            this.lblCivilStatus.Location = new System.Drawing.Point(67, 315);
            this.lblCivilStatus.Name = "lblCivilStatus";
            this.lblCivilStatus.Size = new System.Drawing.Size(59, 13);
            this.lblCivilStatus.TabIndex = 18;
            this.lblCivilStatus.Text = "Civil Status";
            // 
            // lblReligion
            // 
            this.lblReligion.AutoSize = true;
            this.lblReligion.Location = new System.Drawing.Point(67, 340);
            this.lblReligion.Name = "lblReligion";
            this.lblReligion.Size = new System.Drawing.Size(45, 13);
            this.lblReligion.TabIndex = 18;
            this.lblReligion.Text = "Religion";
            // 
            // panelGuardian
            // 
            this.panelGuardian.Controls.Add(this.lblGuardianRelationship);
            this.panelGuardian.Controls.Add(this.lblGuardianContact);
            this.panelGuardian.Controls.Add(this.lblGuardianName);
            this.panelGuardian.Location = new System.Drawing.Point(488, 4);
            this.panelGuardian.Name = "panelGuardian";
            this.panelGuardian.Size = new System.Drawing.Size(354, 241);
            this.panelGuardian.TabIndex = 21;
            // 
            // lblGuardianName
            // 
            this.lblGuardianName.AutoSize = true;
            this.lblGuardianName.Location = new System.Drawing.Point(17, 18);
            this.lblGuardianName.Name = "lblGuardianName";
            this.lblGuardianName.Size = new System.Drawing.Size(41, 13);
            this.lblGuardianName.TabIndex = 0;
            this.lblGuardianName.Text = "Gname";
            // 
            // lblGuardianContact
            // 
            this.lblGuardianContact.AutoSize = true;
            this.lblGuardianContact.Location = new System.Drawing.Point(20, 51);
            this.lblGuardianContact.Name = "lblGuardianContact";
            this.lblGuardianContact.Size = new System.Drawing.Size(51, 13);
            this.lblGuardianContact.TabIndex = 1;
            this.lblGuardianContact.Text = "Gcontact";
            // 
            // lblGuardianRelationship
            // 
            this.lblGuardianRelationship.AutoSize = true;
            this.lblGuardianRelationship.Location = new System.Drawing.Point(23, 86);
            this.lblGuardianRelationship.Name = "lblGuardianRelationship";
            this.lblGuardianRelationship.Size = new System.Drawing.Size(68, 13);
            this.lblGuardianRelationship.TabIndex = 2;
            this.lblGuardianRelationship.Text = "Grelationship";
            // 
            // ViewArchivedPatient
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panelGuardian);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblReligion);
            this.Controls.Add(this.lblCivilStatus);
            this.Controls.Add(this.lblMedicalHistory);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblContact);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.lblBirthDate);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.lblPatientID);
            this.Controls.Add(this.lblFullName);
            this.Name = "ViewArchivedPatient";
            this.Size = new System.Drawing.Size(857, 470);
            this.Load += new System.EventHandler(this.ViewArchivedPatient_Load);
            this.panelGuardian.ResumeLayout(false);
            this.panelGuardian.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblMedicalHistory;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.Label lblBirthDate;
        private System.Windows.Forms.Label lblGender;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblPatientID;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblCivilStatus;
        private System.Windows.Forms.Label lblReligion;
        private System.Windows.Forms.Panel panelGuardian;
        private System.Windows.Forms.Label lblGuardianRelationship;
        private System.Windows.Forms.Label lblGuardianContact;
        private System.Windows.Forms.Label lblGuardianName;
    }
}
