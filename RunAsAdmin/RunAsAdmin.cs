using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RunAsAdmin
{
    public partial class RunAsAdmin : Form
    {
        public RunAsAdmin()
        {
            InitializeComponent();
        }

        private void RunAsAdmin_Load(object sender, EventArgs e)
        {
            this.Hide();
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = AppDomain.CurrentDomain.BaseDirectory + "GatewaySwticher.exe";//AppDomain.CurrentDomain.BaseDirectory
            proc.Verb = "runas";
            try
            {
                Process.Start(proc);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            Application.Exit();
        }
    }
}
