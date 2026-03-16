namespace M.A_Florencio_Dental_Records
{
    partial class Archive
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
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.flowArchive = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 37);
            this.label2.TabIndex = 12;
            this.label2.Text = "Archives";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::M.A_Florencio_Dental_Records.Properties.Resources.Divider;
            this.pictureBox3.Location = new System.Drawing.Point(20, 57);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(745, 10);
            this.pictureBox3.TabIndex = 11;
            this.pictureBox3.TabStop = false;
            // 
            // flowArchive
            // 
            this.flowArchive.Location = new System.Drawing.Point(20, 73);
            this.flowArchive.Name = "flowArchive";
            this.flowArchive.Size = new System.Drawing.Size(745, 470);
            this.flowArchive.TabIndex = 13;
            // 
            // Archive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowArchive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox3);
            this.Name = "Archive";
            this.Size = new System.Drawing.Size(785, 559);
            this.Load += new System.EventHandler(this.Archive_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.FlowLayoutPanel flowArchive;
    }
}
