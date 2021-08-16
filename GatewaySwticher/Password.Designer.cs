namespace GatewaySwticher
{
    partial class Password
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Password));
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnChangePW = new System.Windows.Forms.Button();
            this.lblNewPass = new System.Windows.Forms.Label();
            this.txtNewPass = new System.Windows.Forms.TextBox();
            this.btnSavePW = new System.Windows.Forms.Button();
            this.chkFast = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtPass
            // 
            this.txtPass.Location = new System.Drawing.Point(133, 12);
            this.txtPass.Name = "txtPass";
            this.txtPass.PasswordChar = '*';
            this.txtPass.Size = new System.Drawing.Size(231, 22);
            this.txtPass.TabIndex = 0;
            this.txtPass.TextChanged += new System.EventHandler(this.TxtPass_TextChanged);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 15);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(115, 17);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Enter Password :";
            // 
            // btnLogin
            // 
            this.btnLogin.Enabled = false;
            this.btnLogin.Location = new System.Drawing.Point(236, 45);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(218, 41);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.BtnLogin_Click);
            // 
            // btnChangePW
            // 
            this.btnChangePW.Enabled = false;
            this.btnChangePW.Location = new System.Drawing.Point(12, 45);
            this.btnChangePW.Name = "btnChangePW";
            this.btnChangePW.Size = new System.Drawing.Size(218, 41);
            this.btnChangePW.TabIndex = 3;
            this.btnChangePW.Text = "Change Password";
            this.btnChangePW.UseVisualStyleBackColor = true;
            this.btnChangePW.Click += new System.EventHandler(this.BtnChangePW_Click);
            // 
            // lblNewPass
            // 
            this.lblNewPass.AutoSize = true;
            this.lblNewPass.Location = new System.Drawing.Point(12, 125);
            this.lblNewPass.Name = "lblNewPass";
            this.lblNewPass.Size = new System.Drawing.Size(108, 17);
            this.lblNewPass.TabIndex = 5;
            this.lblNewPass.Text = "New Password :";
            // 
            // txtNewPass
            // 
            this.txtNewPass.Location = new System.Drawing.Point(133, 122);
            this.txtNewPass.Name = "txtNewPass";
            this.txtNewPass.PasswordChar = '*';
            this.txtNewPass.Size = new System.Drawing.Size(321, 22);
            this.txtNewPass.TabIndex = 4;
            this.txtNewPass.TextChanged += new System.EventHandler(this.TxtNewPass_TextChanged);
            // 
            // btnSavePW
            // 
            this.btnSavePW.Enabled = false;
            this.btnSavePW.Location = new System.Drawing.Point(236, 151);
            this.btnSavePW.Name = "btnSavePW";
            this.btnSavePW.Size = new System.Drawing.Size(218, 41);
            this.btnSavePW.TabIndex = 6;
            this.btnSavePW.Text = "Save";
            this.btnSavePW.UseVisualStyleBackColor = true;
            this.btnSavePW.Click += new System.EventHandler(this.BtnSavePW_Click);
            // 
            // chkFast
            // 
            this.chkFast.AutoSize = true;
            this.chkFast.Checked = true;
            this.chkFast.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFast.Location = new System.Drawing.Point(375, 13);
            this.chkFast.Name = "chkFast";
            this.chkFast.Size = new System.Drawing.Size(84, 21);
            this.chkFast.TabIndex = 8;
            this.chkFast.Text = "Fastload";
            this.chkFast.UseVisualStyleBackColor = true;
            this.chkFast.CheckedChanged += new System.EventHandler(this.ChkFast_CheckedChanged);
            // 
            // Password
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(466, 97);
            this.Controls.Add(this.chkFast);
            this.Controls.Add(this.btnSavePW);
            this.Controls.Add(this.lblNewPass);
            this.Controls.Add(this.txtNewPass);
            this.Controls.Add(this.btnChangePW);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPass);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Password";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Password";
            this.Load += new System.EventHandler(this.Password_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnChangePW;
        private System.Windows.Forms.Label lblNewPass;
        private System.Windows.Forms.TextBox txtNewPass;
        private System.Windows.Forms.Button btnSavePW;
        private System.Windows.Forms.CheckBox chkFast;
    }
}