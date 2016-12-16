namespace BulkUserCreator
{
    partial class Form1
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
            this.txtUsersConnStr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtProfilesConnStr = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProfileId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNameFormat = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.nudNoUsers = new System.Windows.Forms.NumericUpDown();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudNoUsers)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Users connection string";
            // 
            // txtUsesrConnStr
            // 
            this.txtUsersConnStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsersConnStr.Location = new System.Drawing.Point(180, 12);
            this.txtUsersConnStr.Name = "txtUsersConnStr";
            this.txtUsersConnStr.Size = new System.Drawing.Size(407, 20);
            this.txtUsersConnStr.TabIndex = 1;
            this.txtUsersConnStr.Text = "server=.;database=EmployerUsers;trusted_connection=true;";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Profiles connection string";
            // 
            // txtProfilesConnStr
            // 
            this.txtProfilesConnStr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProfilesConnStr.Location = new System.Drawing.Point(180, 38);
            this.txtProfilesConnStr.Name = "txtProfilesConnStr";
            this.txtProfilesConnStr.Size = new System.Drawing.Size(407, 20);
            this.txtProfilesConnStr.TabIndex = 3;
            this.txtProfilesConnStr.Text = "server=.;database=EmployerUsers;trusted_connection=true;";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Profile Id";
            // 
            // txtProfileId
            // 
            this.txtProfileId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProfileId.Location = new System.Drawing.Point(180, 64);
            this.txtProfileId.Name = "txtProfileId";
            this.txtProfileId.Size = new System.Drawing.Size(407, 20);
            this.txtProfileId.TabIndex = 5;
            this.txtProfileId.Text = "b1fae38b-2325-4aa9-b0c3-3a31ef367210";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Name format";
            // 
            // txtNameFormat
            // 
            this.txtNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNameFormat.Location = new System.Drawing.Point(180, 90);
            this.txtNameFormat.Name = "txtNameFormat";
            this.txtNameFormat.Size = new System.Drawing.Size(407, 20);
            this.txtNameFormat.TabIndex = 7;
            this.txtNameFormat.Text = "User{0}";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Number of users";
            // 
            // nudNoUsers
            // 
            this.nudNoUsers.Location = new System.Drawing.Point(180, 116);
            this.nudNoUsers.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nudNoUsers.Name = "nudNoUsers";
            this.nudNoUsers.Size = new System.Drawing.Size(120, 20);
            this.nudNoUsers.TabIndex = 9;
            this.nudNoUsers.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStop.Location = new System.Drawing.Point(15, 142);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(572, 23);
            this.btnStartStop.TabIndex = 10;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(15, 187);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(572, 23);
            this.pbProgress.TabIndex = 11;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 213);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(38, 13);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "Ready";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 239);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.nudNoUsers);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtNameFormat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtProfileId);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtProfilesConnStr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsersConnStr);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Bulk User Creator";
            ((System.ComponentModel.ISupportInitialize)(this.nudNoUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsersConnStr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtProfilesConnStr;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProfileId;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNameFormat;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudNoUsers;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lblStatus;
    }
}

