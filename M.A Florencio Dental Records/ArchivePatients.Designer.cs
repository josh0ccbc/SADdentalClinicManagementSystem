namespace M.A_Florencio_Dental_Records
{
    partial class ArchivePatients
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
            this.btnUnarchive = new MaterialSkin.Controls.MaterialButton();
            this.btnDelete = new MaterialSkin.Controls.MaterialButton();
            this.btnView = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(31, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(33, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "name";
            // 
            // lblGenderAge
            // 
            this.lblGenderAge.AutoSize = true;
            this.lblGenderAge.Location = new System.Drawing.Point(31, 47);
            this.lblGenderAge.Name = "lblGenderAge";
            this.lblGenderAge.Size = new System.Drawing.Size(63, 13);
            this.lblGenderAge.TabIndex = 1;
            this.lblGenderAge.Text = "Gender-age";
            // 
            // lblContact
            // 
            this.lblContact.AutoSize = true;
            this.lblContact.Location = new System.Drawing.Point(31, 78);
            this.lblContact.Name = "lblContact";
            this.lblContact.Size = new System.Drawing.Size(44, 13);
            this.lblContact.TabIndex = 2;
            this.lblContact.Text = "Contact";
            // 
            // btnUnarchive
            // 
            this.btnUnarchive.AutoSize = false;
            this.btnUnarchive.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUnarchive.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnUnarchive.Depth = 0;
            this.btnUnarchive.HighEmphasis = true;
            this.btnUnarchive.Icon = null;
            this.btnUnarchive.Location = new System.Drawing.Point(158, 14);
            this.btnUnarchive.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnUnarchive.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnUnarchive.Name = "btnUnarchive";
            this.btnUnarchive.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnUnarchive.Size = new System.Drawing.Size(53, 21);
            this.btnUnarchive.TabIndex = 4;
            this.btnUnarchive.Text = "unarchive";
            this.btnUnarchive.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnUnarchive.UseAccentColor = false;
            this.btnUnarchive.UseVisualStyleBackColor = true;
            this.btnUnarchive.Click += new System.EventHandler(this.btnUnarchive_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = false;
            this.btnDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDelete.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnDelete.Depth = 0;
            this.btnDelete.HighEmphasis = true;
            this.btnDelete.Icon = null;
            this.btnDelete.Location = new System.Drawing.Point(136, 47);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnDelete.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnDelete.Size = new System.Drawing.Size(75, 28);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnDelete.UseAccentColor = false;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.Color.White;
            this.btnView.BackgroundImage = global::M.A_Florencio_Dental_Records.Properties.Resources.view;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnView.Location = new System.Drawing.Point(212, 103);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(20, 20);
            this.btnView.TabIndex = 6;
            this.btnView.UseVisualStyleBackColor = false;
            // 
            // ArchivePatients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUnarchive);
            this.Controls.Add(this.lblContact);
            this.Controls.Add(this.lblGenderAge);
            this.Controls.Add(this.lblName);
            this.Name = "ArchivePatients";
            this.Size = new System.Drawing.Size(241, 131);
            this.Load += new System.EventHandler(this.ArchivePatients_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblGenderAge;
        private System.Windows.Forms.Label lblContact;
        private MaterialSkin.Controls.MaterialButton btnUnarchive;
        private MaterialSkin.Controls.MaterialButton btnDelete;
        private System.Windows.Forms.Button btnView;
    }
}
