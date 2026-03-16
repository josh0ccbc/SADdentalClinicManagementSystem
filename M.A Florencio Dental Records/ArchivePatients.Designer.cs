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
            this.btnView = new MaterialSkin.Controls.MaterialButton();
            this.btnUnarchive = new MaterialSkin.Controls.MaterialButton();
            this.btnDelete = new MaterialSkin.Controls.MaterialButton();
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
            this.lblContact.Size = new System.Drawing.Size(35, 13);
            this.lblContact.TabIndex = 2;
            this.lblContact.Text = "label3";
            // 
            // btnView
            // 
            this.btnView.AutoSize = false;
            this.btnView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnView.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnView.Depth = 0;
            this.btnView.HighEmphasis = true;
            this.btnView.Icon = null;
            this.btnView.Location = new System.Drawing.Point(181, 103);
            this.btnView.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnView.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnView.Name = "btnView";
            this.btnView.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnView.Size = new System.Drawing.Size(56, 22);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "view";
            this.btnView.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnView.UseAccentColor = false;
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnUnarchive
            // 
            this.btnUnarchive.AutoSize = false;
            this.btnUnarchive.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUnarchive.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnUnarchive.Depth = 0;
            this.btnUnarchive.HighEmphasis = true;
            this.btnUnarchive.Icon = null;
            this.btnUnarchive.Location = new System.Drawing.Point(120, 103);
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
            this.btnDelete.Location = new System.Drawing.Point(162, 47);
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
            // ArchivePatients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUnarchive);
            this.Controls.Add(this.btnView);
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
        private MaterialSkin.Controls.MaterialButton btnView;
        private MaterialSkin.Controls.MaterialButton btnUnarchive;
        private MaterialSkin.Controls.MaterialButton btnDelete;
    }
}
