using System;

using System.Windows.Forms;

namespace RunWCMD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            Cmd.ExecuteCommand(string.Format("start " + "GatewaySwticher " + Environment.CurrentDirectory + "\\GatewaySwticher.exe")); //AppDomain.CurrentDomain.BaseDirectory
            Application.Exit();
        }
    }
}
