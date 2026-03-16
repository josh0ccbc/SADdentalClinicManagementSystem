namespace M.A_Florencio_Dental_Records
{
    partial class Patient
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
            this.lblName = new System.Windows.Forms.Label();
            this.lblGenderAge = new System.Windows.Forms.Label();
            this.lblContact = new System.Windows.Forms.Label();
            this.DPpatientid = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(93, 8);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(47, 15);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "label1";
            // 
            // lblGenderAge
            // 
            this.lblGenderAge.AutoSize = true;
            this.lblGenderAge.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenderAge.Location = new System.Drawing.Point(286, 9);
            this.lblGenderAge.Name = "lblGenderAge";
            this.lblGenderAge.Size = new System.Drawing.Size(47, 15);
            this.lblGenderAge.TabIndex = 6;
            this.lblGenderAge.Text = "label1";
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContact.Location = new System.Drawing.Point(467, 9);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(47, 15);
            this.lblContact.TabIndex = 7;
            this.lblContact.Text = "label2";
            // 
            // DPpatientid
            // 
            this.DPpatientid.AutoSize = true;
            this.DPpatientid.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DPpatientid.Location = new System.Drawing.Point(16, 14);
            this.DPpatientid.Name = "DPpatientid";
            this.DPpatientid.Size = new System.Drawing.Size(47, 15);
            this.DPpatientid.TabIndex = 4;
            this.DPpatientid.Text = "label1";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.White;
            this.btnDelete.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.archive;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Location = new System.Drawing.Point(642, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(20, 20);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnDelete.MouseEnter += new System.EventHandler(this.btnDelete_MouseEnter);
            this.btnDelete.MouseLeave += new System.EventHandler(this.btnDelete_MouseLeave);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.White;
            this.btnView.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.view;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Location = new System.Drawing.Point(609, 6);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(20, 20);
            this.btnView.TabIndex = 5;
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            this.btnView.MouseEnter += new System.EventHandler(this.btnView_MouseEnter);
            this.btnView.MouseLeave += new System.EventHandler(this.btnView_MouseLeave);
            // 
            // Patient
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.lblContact);
            this.Controls.Add(this.lblGenderAge);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.DPpatientid);
            this.Controls.Add(this.lblName);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Patient";
            this.Size = new System.Drawing.Size(704, 51);
            this.Load += new System.EventHandler(this.Patient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Label lblGenderAge;
        private System.Windows.Forms.Label lblContact;
        private System.Windows.Forms.Label DPpatientid;
        private System.Windows.Forms.Button btnDelete;
    }
}
