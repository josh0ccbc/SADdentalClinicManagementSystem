namespace M.A_Florencio_Dental_Records
{
    partial class AddPrescriptionDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txtMedication = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpVisitDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInstructions = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProcedure = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 100);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Medication";
            // 
            // txtMedication
            // 
            this.txtMedication.Location = new System.Drawing.Point(31, 125);
            this.txtMedication.Margin = new System.Windows.Forms.Padding(4);
            this.txtMedication.Name = "txtMedication";
            this.txtMedication.Size = new System.Drawing.Size(200, 25);
            this.txtMedication.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 170);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Prescription Date";
            // 
            // dtpVisitDate
            // 
            this.dtpVisitDate.Location = new System.Drawing.Point(31, 199);
            this.dtpVisitDate.Name = "dtpVisitDate";
            this.dtpVisitDate.Size = new System.Drawing.Size(200, 25);
            this.dtpVisitDate.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 244);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "Instructions";
            // 
            // txtInstructions
            // 
            this.txtInstructions.Location = new System.Drawing.Point(31, 275);
            this.txtInstructions.Name = "txtInstructions";
            this.txtInstructions.Size = new System.Drawing.Size(200, 25);
            this.txtInstructions.TabIndex = 3;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(31, 321);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(156, 321);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(293, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "Diagnosis";
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(296, 125);
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(170, 25);
            this.txtDiagnosis.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(293, 170);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Procedure";
            // 
            // txtProcedure
            // 
            this.txtProcedure.Location = new System.Drawing.Point(296, 199);
            this.txtProcedure.Name = "txtProcedure";
            this.txtProcedure.Size = new System.Drawing.Size(170, 25);
            this.txtProcedure.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(293, 244);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Notes";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(296, 275);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(170, 69);
            this.txtNotes.TabIndex = 7;
            // 
            // AddPrescriptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 588);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.txtProcedure);
            this.Controls.Add(this.txtDiagnosis);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtInstructions);
            this.Controls.Add(this.dtpVisitDate);
            this.Controls.Add(this.txtMedication);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AddPrescriptionDialog";
            this.Padding = new System.Windows.Forms.Padding(4, 84, 4, 4);
            this.Load += new System.EventHandler(this.AddPrescriptionDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMedication;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpVisitDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInstructions;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProcedure;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNotes;
    }
}