using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using MyExtentions;

namespace GatewaySwticher
{
    public partial class Password : Form
    {
        private string passPhrase;
        private GatewaySwticher software;
        private bool firstUse = false;
        public Password()
        {
            InitializeComponent();
        }

        private void BtnSavePW_Click(object sender, EventArgs e)
        {
            if (firstUse)
            {
                Properties.Settings.Default.Phrase = Crypto.HashString(txtNewPass.Text, Crypto.Hash.SHA512, Crypto.TextEncodings.UTF8,
                  Crypto.ConversionTypes.Base64, "funny Hah");
                Properties.Settings.Default.Fast = chkFast.Checked;
                Properties.Settings.Default.Save();
                File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,Environment.CurrentDirectory + "\\user.config",true);
                loadPassword();
                firstUse = false;
                loadProgram();
            }
            else if (Crypto.VerifyHash(txtPass.Text, passPhrase, Crypto.Hash.SHA512, Crypto.TextEncodings.UTF8, "funny Hah"))
            {
                Properties.Settings.Default.Phrase = Crypto.HashString(txtNewPass.Text, Crypto.Hash.SHA512, Crypto.TextEncodings.UTF8,
                    Crypto.ConversionTypes.Base64, "funny Hah");
                Properties.Settings.Default.Save();
                File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,Environment.CurrentDirectory + "\\user.config",true);
                loadPassword();
            }
            else
            {
                MessageBox.Show("Old password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Height = 144;
            txtNewPass.Text = string.Empty;
        }

        private void loadProgram()
        {
            software = new GatewaySwticher();
            software.FormClosed += new FormClosedEventHandler(software_FormClosed);
            software.Show();
            this.Hide();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (Crypto.VerifyHash(txtPass.Text, passPhrase, Crypto.Hash.SHA512, Crypto.TextEncodings.UTF8, "funny Hah"))
            {
                loadProgram();
            }
            else
            {
                MessageBox.Show("Password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void software_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void BtnChangePW_Click(object sender, EventArgs e)
        {
            if (Crypto.VerifyHash(txtPass.Text, passPhrase, Crypto.Hash.SHA512, Crypto.TextEncodings.UTF8, "funny Hah"))
            {
                this.Height = 251;
                btnChangePW.Enabled = false;
            }
            else
            {
                this.Height = 144;
                MessageBox.Show("Old password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Password_Load(object sender, EventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\user.config"))
            {
                if (!ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).HasFile)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath));
                }
                File.Copy(Environment.CurrentDirectory + "\\user.config",ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,true);
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.Phrase))
            {
                firstUse = true;
                btnLogin.Enabled = false;
                btnChangePW.Enabled = false;
                txtPass.Enabled = false;
                lblPassword.Enabled = false;
                this.Height = 251;
                this.Opacity = 100;

            }
            else if (!firstUse && Properties.Settings.Default.Fast)
            {
                loadProgram();
            }
            else
            {
                loadPassword();
                firstUse = false;
                this.Opacity = 100;
            }
        }

        private void loadPassword()
        {
            passPhrase = Properties.Settings.Default.Phrase;
        }

        private void TxtPass_TextChanged(object sender, EventArgs e)
        {
            if (txtPass.Text != string.Empty)
            {
                btnLogin.Enabled = true;
                btnChangePW.Enabled = true;
            }
            else
            {
                btnLogin.Enabled = false;
                btnChangePW.Enabled = false;
            }
        }

        private void TxtNewPass_TextChanged(object sender, EventArgs e)
        {
            btnSavePW.Enabled = (txtNewPass.Text != string.Empty);
        }

        private void ChkFast_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Fast = chkFast.Checked;
            Properties.Settings.Default.Save();
            File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,Environment.CurrentDirectory + "\\user.config",true);
        }
    }
}
