namespace M.A_Florencio_Dental_Records
{
    partial class MedicalRecordCard
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
            this.lblDate = new System.Windows.Forms.Label();
            this.lblDiagnosis = new System.Windows.Forms.Label();
            this.lblProcedure = new System.Windows.Forms.Label();
            this.btnViewDC = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(19, 12);
            this.lblDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(54, 15);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "lblDate";
            // 
            // lblDiagnosis
            // 
            this.lblDiagnosis.AutoSize = true;
            this.lblDiagnosis.Location = new System.Drawing.Point(246, 12);
            this.lblDiagnosis.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDiagnosis.Name = "lblDiagnosis";
            this.lblDiagnosis.Size = new System.Drawing.Size(87, 15);
            this.lblDiagnosis.TabIndex = 0;
            this.lblDiagnosis.Text = "lblDiagnosis";
            // 
            // lblProcedure
            // 
            this.lblProcedure.AutoSize = true;
            this.lblProcedure.Location = new System.Drawing.Point(428, 12);
            this.lblProcedure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcedure.Name = "lblProcedure";
            this.lblProcedure.Size = new System.Drawing.Size(92, 15);
            this.lblProcedure.TabIndex = 0;
            this.lblProcedure.Text = "lblProcedure";
            // 
            // btnViewDC
            // 
            this.btnViewDC.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.tooth;
            this.btnViewDC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnViewDC.FlatAppearance.BorderSize = 0;
            this.btnViewDC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewDC.Location = new System.Drawing.Point(776, 6);
            this.btnViewDC.Name = "btnViewDC";
            this.btnViewDC.Size = new System.Drawing.Size(25, 25);
            this.btnViewDC.TabIndex = 1;
            this.btnViewDC.UseVisualStyleBackColor = true;
            this.btnViewDC.Click += new System.EventHandler(this.button1_Click);
            this.btnViewDC.MouseEnter += new System.EventHandler(this.btnViewDC_MouseEnter);
            this.btnViewDC.MouseLeave += new System.EventHandler(this.btnViewDC_MouseLeave);
            // 
            // btnEdit
            // 
            this.btnEdit.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.edit;
            this.btnEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Location = new System.Drawing.Point(745, 6);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(23, 23);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            this.btnEdit.MouseEnter += new System.EventHandler(this.btnEdit_MouseEnter);
            this.btnEdit.MouseLeave += new System.EventHandler(this.btnEdit_MouseLeave);
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.view;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(707, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 25);
            this.button1.TabIndex = 1;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.MouseEnter += new System.EventHandler(this.button1_MouseEnter);
            this.button1.MouseLeave += new System.EventHandler(this.button1_MouseLeave);
            // 
            // MedicalRecordCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnViewDC);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblProcedure);
            this.Controls.Add(this.lblDiagnosis);
            this.Controls.Add(this.lblDate);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MedicalRecordCard";
            this.Size = new System.Drawing.Size(799, 40);
            this.Load += new System.EventHandler(this.MedicalRecordCard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblDiagnosis;
        private System.Windows.Forms.Label lblProcedure;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnViewDC;
    }
}
