namespace M.A_Florencio_Dental_Records
{
    partial class patientDetailsControl
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
            this.DlblName = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.PlblPatientID = new System.Windows.Forms.Label();
            this.PlblGender = new System.Windows.Forms.Label();
            this.PlblBday = new System.Windows.Forms.Label();
            this.PlblAge = new System.Windows.Forms.Label();
            this.PlblContact = new System.Windows.Forms.Label();
            this.PlblAddress = new System.Windows.Forms.Label();
            this.PlblMH = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DlblName
            // 
            this.DlblName.AutoSize = true;
            this.DlblName.Location = new System.Drawing.Point(42, 116);
            this.DlblName.Name = "DlblName";
            this.DlblName.Size = new System.Drawing.Size(50, 17);
            this.DlblName.TabIndex = 0;
            this.DlblName.Text = "Name";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(17, 24);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // PlblPatientID
            // 
            this.PlblPatientID.AutoSize = true;
            this.PlblPatientID.Location = new System.Drawing.Point(42, 144);
            this.PlblPatientID.Name = "PlblPatientID";
            this.PlblPatientID.Size = new System.Drawing.Size(79, 17);
            this.PlblPatientID.TabIndex = 0;
            this.PlblPatientID.Text = "Patient ID";
            // 
            // PlblGender
            // 
            this.PlblGender.AutoSize = true;
            this.PlblGender.Location = new System.Drawing.Point(42, 173);
            this.PlblGender.Name = "PlblGender";
            this.PlblGender.Size = new System.Drawing.Size(63, 17);
            this.PlblGender.TabIndex = 2;
            this.PlblGender.Text = "Gender";
            // 
            // PlblBday
            // 
            this.PlblBday.AutoSize = true;
            this.PlblBday.Location = new System.Drawing.Point(42, 200);
            this.PlblBday.Name = "PlblBday";
            this.PlblBday.Size = new System.Drawing.Size(78, 17);
            this.PlblBday.TabIndex = 3;
            this.PlblBday.Text = "BirthDate";
            // 
            // PlblAge
            // 
            this.PlblAge.AutoSize = true;
            this.PlblAge.Location = new System.Drawing.Point(42, 229);
            this.PlblAge.Name = "PlblAge";
            this.PlblAge.Size = new System.Drawing.Size(37, 17);
            this.PlblAge.TabIndex = 4;
            this.PlblAge.Text = "Age";
            // 
            // PlblContact
            // 
            this.PlblContact.AutoSize = true;
            this.PlblContact.Location = new System.Drawing.Point(42, 257);
            this.PlblContact.Name = "PlblContact";
            this.PlblContact.Size = new System.Drawing.Size(65, 17);
            this.PlblContact.TabIndex = 5;
            this.PlblContact.Text = "Contact";
            // 
            // PlblAddress
            // 
            this.PlblAddress.AutoSize = true;
            this.PlblAddress.Location = new System.Drawing.Point(42, 285);
            this.PlblAddress.Name = "PlblAddress";
            this.PlblAddress.Size = new System.Drawing.Size(69, 17);
            this.PlblAddress.TabIndex = 6;
            this.PlblAddress.Text = "Address";
            // 
            // PlblMH
            // 
            this.PlblMH.AutoSize = true;
            this.PlblMH.Location = new System.Drawing.Point(42, 313);
            this.PlblMH.Name = "PlblMH";
            this.PlblMH.Size = new System.Drawing.Size(120, 17);
            this.PlblMH.TabIndex = 7;
            this.PlblMH.Text = "Medical History";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(687, 516);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 8;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(786, 516);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 9;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // patientDetailsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.PlblMH);
            this.Controls.Add(this.PlblAddress);
            this.Controls.Add(this.PlblContact);
            this.Controls.Add(this.PlblAge);
            this.Controls.Add(this.PlblBday);
            this.Controls.Add(this.PlblGender);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.PlblPatientID);
            this.Controls.Add(this.DlblName);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "patientDetailsControl";
            this.Size = new System.Drawing.Size(899, 559);
            this.Load += new System.EventHandler(this.patientDetailsControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DlblName;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label PlblPatientID;
        private System.Windows.Forms.Label PlblGender;
        private System.Windows.Forms.Label PlblBday;
        private System.Windows.Forms.Label PlblAge;
        private System.Windows.Forms.Label PlblContact;
        private System.Windows.Forms.Label PlblAddress;
        private System.Windows.Forms.Label PlblMH;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
    }
}
