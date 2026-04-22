namespace M.A_Florencio_Dental_Records
{
    partial class EditMedicalRecordForm
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
            this.dtpVisitDate = new System.Windows.Forms.DateTimePicker();
            this.txtDiagnosis = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.materialButton1 = new MaterialSkin.Controls.MaterialButton();
            this.materialButton2 = new MaterialSkin.Controls.MaterialButton();
            this.cmbProcedure = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMedication = new System.Windows.Forms.TextBox();
            this.txtMedInstructions = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpPrescDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.lstPrescriptions = new System.Windows.Forms.ListBox();
            this.btnAddPrescription = new MaterialSkin.Controls.MaterialButton();
            this.btnRemovePrescription = new MaterialSkin.Controls.MaterialButton();
            this.label8 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpVisitDate
            // 
            this.dtpVisitDate.Location = new System.Drawing.Point(368, 97);
            this.dtpVisitDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpVisitDate.Name = "dtpVisitDate";
            this.dtpVisitDate.Size = new System.Drawing.Size(298, 25);
            this.dtpVisitDate.TabIndex = 0;
            // 
            // txtDiagnosis
            // 
            this.txtDiagnosis.Location = new System.Drawing.Point(368, 160);
            this.txtDiagnosis.Margin = new System.Windows.Forms.Padding(4);
            this.txtDiagnosis.Name = "txtDiagnosis";
            this.txtDiagnosis.Size = new System.Drawing.Size(298, 25);
            this.txtDiagnosis.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(365, 134);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Diagnosis";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(365, 196);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Procedure";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(365, 253);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Notes";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(368, 277);
            this.txtNotes.Margin = new System.Windows.Forms.Padding(4);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(298, 72);
            this.txtNotes.TabIndex = 1;
            this.txtNotes.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // materialButton1
            // 
            this.materialButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton1.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton1.Depth = 0;
            this.materialButton1.HighEmphasis = true;
            this.materialButton1.Icon = null;
            this.materialButton1.Location = new System.Drawing.Point(602, 368);
            this.materialButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton1.Name = "materialButton1";
            this.materialButton1.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton1.Size = new System.Drawing.Size(64, 36);
            this.materialButton1.TabIndex = 3;
            this.materialButton1.Text = "Save";
            this.materialButton1.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton1.UseAccentColor = false;
            this.materialButton1.UseVisualStyleBackColor = true;
            this.materialButton1.Click += new System.EventHandler(this.materialButton1_Click);
            // 
            // materialButton2
            // 
            this.materialButton2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialButton2.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.materialButton2.Depth = 0;
            this.materialButton2.HighEmphasis = false;
            this.materialButton2.Icon = null;
            this.materialButton2.Location = new System.Drawing.Point(517, 368);
            this.materialButton2.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialButton2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialButton2.Name = "materialButton2";
            this.materialButton2.NoAccentTextColor = System.Drawing.Color.Empty;
            this.materialButton2.Size = new System.Drawing.Size(77, 36);
            this.materialButton2.TabIndex = 3;
            this.materialButton2.Text = "Cancel";
            this.materialButton2.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.materialButton2.UseAccentColor = false;
            this.materialButton2.UseVisualStyleBackColor = true;
            this.materialButton2.Click += new System.EventHandler(this.materialButton2_Click);
            // 
            // cmbProcedure
            // 
            this.cmbProcedure.FormattingEnabled = true;
            this.cmbProcedure.Location = new System.Drawing.Point(368, 220);
            this.cmbProcedure.Name = "cmbProcedure";
            this.cmbProcedure.Size = new System.Drawing.Size(298, 25);
            this.cmbProcedure.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(365, 73);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Visit Date";
            // 
            // txtMedication
            // 
            this.txtMedication.Location = new System.Drawing.Point(23, 160);
            this.txtMedication.Name = "txtMedication";
            this.txtMedication.Size = new System.Drawing.Size(298, 25);
            this.txtMedication.TabIndex = 5;
            this.txtMedication.TextChanged += new System.EventHandler(this.txtMedication_TextChanged);
            // 
            // txtMedInstructions
            // 
            this.txtMedInstructions.Location = new System.Drawing.Point(23, 221);
            this.txtMedInstructions.Name = "txtMedInstructions";
            this.txtMedInstructions.Size = new System.Drawing.Size(298, 25);
            this.txtMedInstructions.TabIndex = 5;
            this.txtMedInstructions.TextChanged += new System.EventHandler(this.txtMedInstructions_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 137);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Medication";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(840, 162);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(182, 25);
            this.textBox3.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 196);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(178, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "Medication Instructions";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // dtpPrescDate
            // 
            this.dtpPrescDate.Location = new System.Drawing.Point(23, 97);
            this.dtpPrescDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpPrescDate.Name = "dtpPrescDate";
            this.dtpPrescDate.Size = new System.Drawing.Size(298, 25);
            this.dtpPrescDate.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 73);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "Prescription Date";
            // 
            // lstPrescriptions
            // 
            this.lstPrescriptions.FormattingEnabled = true;
            this.lstPrescriptions.ItemHeight = 17;
            this.lstPrescriptions.Location = new System.Drawing.Point(22, 279);
            this.lstPrescriptions.Name = "lstPrescriptions";
            this.lstPrescriptions.Size = new System.Drawing.Size(299, 72);
            this.lstPrescriptions.TabIndex = 6;
            // 
            // btnAddPrescription
            // 
            this.btnAddPrescription.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddPrescription.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnAddPrescription.Depth = 0;
            this.btnAddPrescription.HighEmphasis = true;
            this.btnAddPrescription.Icon = null;
            this.btnAddPrescription.Location = new System.Drawing.Point(257, 364);
            this.btnAddPrescription.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnAddPrescription.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnAddPrescription.Name = "btnAddPrescription";
            this.btnAddPrescription.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnAddPrescription.Size = new System.Drawing.Size(64, 36);
            this.btnAddPrescription.TabIndex = 3;
            this.btnAddPrescription.Text = "Add";
            this.btnAddPrescription.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnAddPrescription.UseAccentColor = false;
            this.btnAddPrescription.UseVisualStyleBackColor = true;
            this.btnAddPrescription.Click += new System.EventHandler(this.btnAddPrescription_Click);
            // 
            // btnRemovePrescription
            // 
            this.btnRemovePrescription.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemovePrescription.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnRemovePrescription.Depth = 0;
            this.btnRemovePrescription.HighEmphasis = false;
            this.btnRemovePrescription.Icon = null;
            this.btnRemovePrescription.Location = new System.Drawing.Point(159, 364);
            this.btnRemovePrescription.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnRemovePrescription.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRemovePrescription.Name = "btnRemovePrescription";
            this.btnRemovePrescription.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnRemovePrescription.Size = new System.Drawing.Size(80, 36);
            this.btnRemovePrescription.TabIndex = 3;
            this.btnRemovePrescription.Text = "Remove";
            this.btnRemovePrescription.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnRemovePrescription.UseAccentColor = false;
            this.btnRemovePrescription.UseVisualStyleBackColor = true;
            this.btnRemovePrescription.Click += new System.EventHandler(this.btnRemovePrescription_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 253);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 17);
            this.label8.TabIndex = 2;
            this.label8.Text = "Prescription List";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources._2Divider;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(340, 63);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(5, 368);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // EditMedicalRecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 419);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lstPrescriptions);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.txtMedInstructions);
            this.Controls.Add(this.txtMedication);
            this.Controls.Add(this.cmbProcedure);
            this.Controls.Add(this.btnRemovePrescription);
            this.Controls.Add(this.materialButton2);
            this.Controls.Add(this.btnAddPrescription);
            this.Controls.Add(this.materialButton1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.txtDiagnosis);
            this.Controls.Add(this.dtpPrescDate);
            this.Controls.Add(this.dtpVisitDate);
            this.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "EditMedicalRecordForm";
            this.Padding = new System.Windows.Forms.Padding(4, 84, 4, 4);
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditMedicalRecordForm";
            this.Load += new System.EventHandler(this.EditMedicalRecordForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpVisitDate;
        private System.Windows.Forms.TextBox txtDiagnosis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNotes;
        private MaterialSkin.Controls.MaterialButton materialButton1;
        private MaterialSkin.Controls.MaterialButton materialButton2;
        private System.Windows.Forms.ComboBox cmbProcedure;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMedication;
        private System.Windows.Forms.TextBox txtMedInstructions;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpPrescDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstPrescriptions;
        private MaterialSkin.Controls.MaterialButton btnAddPrescription;
        private MaterialSkin.Controls.MaterialButton btnRemovePrescription;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}