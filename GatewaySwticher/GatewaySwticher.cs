using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MyExtentions;

namespace GatewaySwticher
{
    public partial class GatewaySwticher : Form
    {
        [DllImport("user32.dll")]
        private static extern short RegisterHotKey(IntPtr hwnd, short id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("kernel32.dll")]
        private static extern short GlobalAddAtom(string lpString);

        [DllImport("kernel32.dll")]
        private static extern short GlobalDeleteAtom(short nAtom);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private static int WM_HOTKEY = 0x0312;

        public GatewaySwticher()
        {
            InitializeComponent();
        }

        private static Keys showStatusKey = Keys.F5;
        private static KeyModifier showStatusModifier = KeyModifier.Control;

        private string macAddress;
        private string message;
        private List<data> dataTuples = new List<data>();
        private List<short> regedHotkeys = new List<short>();

        enum KeyModifier
        {
            None = 0,
            Control = 0x2,
            Alt = 0x1,
            Shift = 0x4,
            Win = 0x8
        }

        private bool getSettings()
        {
            StringCollection hotkeys = new StringCollection();
            StringCollection gateWays = new StringCollection();
            StringCollection messages = new StringCollection();
            macAddress = cmbAdapter.SelectedItem.ToString().Split('#')[1].Trim();

            for (int i = 0; i < dgv.Rows.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty((string)dgv.Rows[i].Cells[0].Value) && !string.IsNullOrEmpty((string)dgv.Rows[i].Cells[1].Value))
                {
                    hotkeys.Add(dgv.Rows[i].Cells[0].Value.ToString());
                    gateWays.Add(dgv.Rows[i].Cells[1].Value.ToString());
                    messages.Add(string.IsNullOrEmpty((string) dgv.Rows[i].Cells[2].Value)
                        ? ""
                        : dgv.Rows[i].Cells[2].Value.ToString());
                }
            }

            if (macAddress == String.Empty || hotkeys.Count != gateWays.Count)
            {
                MessageBox.Show("MAC Address is empty", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (hotkeys.Count == gateWays.Count)
            {
                dataTuples.Clear();
                for (int i = 0; i < hotkeys.Count; i++)
                {
                    dataTuples.Add(new data(hotkeys[i], gateWays[i],messages[i]));
                }
                return true;
            }
            else
            {
                MessageBox.Show("Hotkeys and Gatways count is not Euqal", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool initSetting()
        {
            StringCollection hotkeys = Properties.Settings.Default.HotKeys;
            StringCollection gateWays = Properties.Settings.Default.GateWays;
            StringCollection messages = Properties.Settings.Default.Messages;
            macAddress = Properties.Settings.Default.Adapter;
            if (macAddress == String.Empty || hotkeys.Count != gateWays.Count)
            {
                return false;
            }
            else if (hotkeys.Count == gateWays.Count)
            {
                dataTuples.Clear();
                for (int i = 0; i < hotkeys.Count; i++)
                {
                    dataTuples.Add(new data(hotkeys[i], gateWays[i],messages[i]));
                }
                return true;
            }
            else
            {
                MessageBox.Show("Hotkeys and Gatways count is not Euqal", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            dgv.Columns[0].Width = (int)(this.Width * 0.33) - 36;
            dgv.Columns[1].Width = (int)(this.Width * 0.33) - 47;
            dgv.Columns[1].Width = (int)(this.Width * 0.33) - 47;
            loadComboData();
            if (initSetting())
            {
                this.WindowState = FormWindowState.Minimized;
                dgv.Rows.Clear();
                foreach (data dataItem in dataTuples)
                {
                    dgv.Rows.Add(dataItem.hotKey, dataItem.gateWay,dataItem.message);
                }

                btnSave.Enabled = false;
                btnApply.Enabled = false;
                foreach (var item in cmbAdapter.Items)
                {
                    if (item.ToString().Contains(macAddress))
                    {
                        cmbAdapter.SelectedItem = item;
                    }
                }
                addGlobalHotkeys();
            }
            else
            {
                if (cmbAdapter.Items.Count != 0)
                {
                    cmbAdapter.SelectedIndex = 0;
                }
            }
        }

        private void loadComboData()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up
                    && !nic.Name.Contains("loopback")
                    && nic.GetPhysicalAddress().ToString() != string.Empty)
                {
                    cmbAdapter.Items.Add(nic.Name + " # " + BytesToHex(nic.GetPhysicalAddress().GetAddressBytes()));
                }
            }

            //var output = Cmd.ExecuteCommand("route print -4");
        }

        private string BytesToHex(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                hex.Append(bytes[i].ToString("X2"));
                if (i != bytes.Length - 1)
                {
                    hex.Append(":");
                }
            }
            return hex.ToString();
        }

        private static int getSum<T>(List<T> inputList)
        {
            int output = 0;
            if (typeof(T) == typeof(KeyModifier))
            {
                List<KeyModifier> inputList2 = inputList as List<KeyModifier>;
                foreach (KeyModifier input in inputList2)
                {
                    output += (int)input;
                }
            }
            else if (typeof(T) == typeof(Keys))
            {
                List<Keys> inputList2 = inputList as List<Keys>;
                foreach (Keys input in inputList2)
                {
                    output += (int)input;
                }
            }
            else
            {
                MessageBox.Show("Some modifiers or keys are not supported, contact author", "Support", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return output;
        }

        private void addGlobalHotkeys()
        {
            string atomName = Process.GetCurrentProcess().Id.ToString("X8");
            short hotkeyID = GlobalAddAtom(atomName);

            regedHotkeys.Add(RegisterHotKey(this.Handle, hotkeyID, (int)showStatusModifier, (int)showStatusKey));
            foreach (data dataTuple in dataTuples)
            {
                regedHotkeys.Add(RegisterHotKey(this.Handle, hotkeyID, dataTuple.keyModifiersValue, dataTuple.keysValue));
            }

            //if (Marshal.GetLastWin32Error() != 0)
            //{ throw new Win32Exception(); }
        }

        private void removeGlobalHotkeys()
        {
            foreach (short regedHotkey in regedHotkeys)
            {
                UnregisterHotKey(this.Handle, regedHotkey);
                GlobalDeleteAtom(regedHotkey);
            }
            regedHotkeys.Clear();
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int key = ((int)m.LParam >> 16) & 0xFFFF;
                int modifier = (int)m.LParam & 0xFFFF;

                if (modifier == (int)showStatusModifier && key == (int)showStatusKey)
                {
                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (nic.OperationalStatus == OperationalStatus.Up
                            && !nic.Name.Contains("loopback")
                            && nic.Name.Contains(cmbAdapter.SelectedItem.ToString().Split('#')[0].Trim()))
                        {
                            foreach (var data in dataTuples)
                            {
                                if (data.gateWay == nic.GetIPProperties().GatewayAddresses[0].Address.ToString())
                                {
                                    message = data.message;
                                    break;
                                }
                                else
                                {
                                    message = "";
                                }
                            }
                            IntPtr handle = GetForegroundWindow();
                            MessageBox.Show(new WindowWrapper(handle),nic.GetIPProperties().GatewayAddresses[0].Address.ToString() + message,
                                "Current Gateway", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //if (this.WindowState == FormWindowState.Minimized)
                            //{
                            //    notifyIcon.ShowBalloonTip(100, "Current Gateway",
                            //        nic.GetIPProperties().GatewayAddresses[0].Address.ToString(), ToolTipIcon.Info);
                            //}
                            //else
                            //{
                            //    MessageBox.Show(nic.GetIPProperties().GatewayAddresses[0].Address.ToString(),
                            //        "Current Gateway", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //}
                        }
                    }
                }
                else
                {
                    foreach (data item in dataTuples)
                    {
                        if (item.keyModifiersValue == modifier && item.keysValue == key)
                        {
                            changeGateway(item.gateWay, macAddress);
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        private void changeGateway(string ipAddress, string macAddress)
        {
            if (IsAddress(ipAddress) && IsMac(macAddress))
            {
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True");
                    ManagementObjectCollection adapters = searcher.Get();

                    foreach (ManagementObject adapter in adapters)
                    {
                        if (adapter.Properties["MACAddress"].Value.ToString() == macAddress)
                        {
                            ManagementBaseObject GatewaySetter = adapter.GetMethodParameters("SetGateways");
                            GatewaySetter["DefaultIPGateway"] = new string[] { ipAddress };
                            //GatewaySetter["GatewayCostMetric"] = new int[] { 1 };
                            adapter.InvokeMethod("setGateways", GatewaySetter, null);
                            break;
                        }
                    }
                    foreach (var data in dataTuples)
                    {
                        if (data.gateWay == ipAddress)
                        {
                            message = data.message;
                            break;
                        }
                        else
                        {
                            message = "";
                        }
                    }
                    IntPtr handle = GetForegroundWindow();
                    MessageBox.Show(new WindowWrapper(handle), ipAddress.ToString() + message,
                        "Current Gateway", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            else
            {
                Console.WriteLine("Please input ip and mac addresses in correct format");
            }
        }
        private static bool IsAddress(string arginput)
        {
            if (!Regex.IsMatch(arginput.Trim(), @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.?){4}$"))
            {
                return false;
            }
            return true;
        }
        private static bool IsMac(string arginput)
        {
            if (!Regex.IsMatch(arginput.Trim(), @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$"))
            {
                return false;
            }
            return true;
        }

        private void GatewaySwticher_FormClosing(object sender, FormClosingEventArgs e)
        {
            InputBoxResult test = InputBox.Show("Enter Passwrod :"
                , "Close confirmation", "");

            if (test.ReturnCode == DialogResult.OK && !string.IsNullOrEmpty(test.Text))
            {
                if (Crypto.VerifyHash(test.Text, Properties.Settings.Default.Phrase, Crypto.Hash.SHA512,
                    Crypto.TextEncodings.UTF8, "funny Hah"))
                {
                    removeGlobalHotkeys();
                    if (btnSave.Enabled == true && MessageBox.Show("Save settings ?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        SaveSettings();
                    }
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        notifyIcon.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show("Password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            else
            {
                MessageBox.Show("Please enter password to close", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void Dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message == "DataGridViewComboBoxCell value is not valid.")
            {
                object value = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (!((DataGridViewComboBoxColumn)dgv.Columns[e.ColumnIndex]).Items.Contains(value))
                {
                    ((DataGridViewComboBoxColumn)dgv.Columns[e.ColumnIndex]).Items.Add(value);
                    e.ThrowException = false;
                }
            }
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox txtbox = e.Control as TextBox;
            if (this.dgv.CurrentCell.ColumnIndex == 0)
            {
                if (txtbox != null)
                {
                    txtbox.KeyPress -= txtGateway_KeyPress;
                    txtbox.KeyDown += new KeyEventHandler(txtHotkey_KeyDown);
                }
            }
            else if (this.dgv.CurrentCell.ColumnIndex == 1)
            {
                if (txtbox != null)
                {
                    txtbox.KeyDown -= txtHotkey_KeyDown;
                    txtbox.KeyPress += new KeyPressEventHandler(txtGateway_KeyPress);
                }
            }
            else
            {
                if (txtbox != null)
                {
                    txtbox.KeyDown -= txtHotkey_KeyDown;
                    txtbox.KeyPress -= txtGateway_KeyPress;
                }
            }
        }

        private void txtHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox senderTextbox = sender as TextBox;
            if (e.KeyCode != Keys.ControlKey
                && e.KeyCode != Keys.Alt
                && e.KeyCode != Keys.Menu
                && e.KeyCode != Keys.ShiftKey
                && e.KeyCode != Keys.LWin
                && e.KeyCode != Keys.RWin
                && e.KeyCode != Keys.Delete
                && e.KeyCode != Keys.Back
                && e.KeyCode != showStatusKey)
            {
                senderTextbox.Text = e.Modifiers.ToString() + " + " + e.KeyCode.ToString();
                e.SuppressKeyPress = true;
            }
        }

        private void txtGateway_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox senderTextbox = sender as TextBox;
            string[] splittedValues = senderTextbox.Text.Split('.');
            //int firstDot = senderTextbox.Text.IndexOf('.');
            //int secondDot = senderTextbox.Text.IndexOf('.', firstDot + 1);
            //int lastDot = senderTextbox.Text.LastIndexOf('.');
            int result;

            if (!char.IsControl(e.KeyChar)
                && !char.IsDigit(e.KeyChar)
                && e.KeyChar != '.'
                && e.KeyChar != (char)Keys.Delete
                && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
            else if ((e.KeyChar == '.') && (senderTextbox.Text.EndsWith(".") || (splittedValues.Length > 3)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar != '.'
                     && e.KeyChar != (char)Keys.Delete
                     && e.KeyChar != (char)Keys.Back
                     && e.KeyChar != (char)Keys.Escape)
            {
                if (senderTextbox.SelectionStart == senderTextbox.TextLength
                    && int.TryParse(splittedValues[splittedValues.Length - 1], out result)
                    && (result * 10 + int.Parse(e.KeyChar.ToString()) > 255))
                {
                    e.Handled = true;
                }
                //else if (senderTextbox.SelectionStart < senderTextbox.TextLength)
                //{
                //    if ((senderTextbox.SelectionStart < firstDot) || (firstDot == -1))
                //    {
                //        senderTextbox.SelectionStart = 0;
                //        senderTextbox.SelectionLength = (firstDot != -1) ? firstDot : senderTextbox.TextLength;
                //    }
                //    else if ((senderTextbox.SelectionStart < secondDot) || (secondDot == -1))
                //    {
                //        senderTextbox.SelectionStart = firstDot + 1;
                //        senderTextbox.SelectionLength = (secondDot != -1) ? secondDot - firstDot - 1 : senderTextbox.TextLength - firstDot;
                //    }
                //    else if ((senderTextbox.SelectionStart < lastDot) || (lastDot == -1))
                //    {
                //        senderTextbox.SelectionStart = secondDot + 1;
                //        senderTextbox.SelectionLength = (lastDot != -1) ? lastDot - secondDot - 1 : senderTextbox.TextLength - lastDot;
                //    }
                //    else if (senderTextbox.SelectionStart >= lastDot)
                //    {
                //        senderTextbox.SelectionStart = lastDot + 1;
                //        senderTextbox.SelectionLength = senderTextbox.TextLength - lastDot;
                //    }
                //}
            }
        }

        private void GatewaySwticher_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
                removeGlobalHotkeys();
                addGlobalHotkeys();
            }
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                InputBoxResult test = InputBox.Show("Enter Passwrod :"
                    , "Open confirmation", "");

                if (test.ReturnCode == DialogResult.OK && !string.IsNullOrEmpty(test.Text))
                {
                    if (Crypto.VerifyHash(test.Text, Properties.Settings.Default.Phrase, Crypto.Hash.SHA512,
                        Crypto.TextEncodings.UTF8, "funny Hah"))
                    {
                        this.Show();
                        this.ShowInTaskbar = true;
                        notifyIcon.Visible = false;
                        this.WindowState = FormWindowState.Normal;
                        removeGlobalHotkeys();
                        addGlobalHotkeys();
                    }
                    else
                    {
                        MessageBox.Show("Password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter password to open", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (data dataTuple in dataTuples)
                {
                    sb.AppendFormat("{0} = {1}", dataTuple.hotKey, dataTuple.gateWay).AppendLine();
                }

                sb.AppendFormat("{0} + {1} = Current GateWay", showStatusModifier.ToString(), showStatusKey.ToString());
                notifyIcon.ShowBalloonTip(100, "Gateway Swticher By Strik3rZ", sb.ToString(), ToolTipIcon.Info);
            }
        }

        private void SaveSettings()
        {
            StringCollection hotkeys = new StringCollection();
            StringCollection gateWays = new StringCollection();
            StringCollection messages = new StringCollection();
            for (int i = 0; i < dgv.Rows.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty((string)dgv.Rows[i].Cells[0].Value) && !string.IsNullOrEmpty((string)dgv.Rows[i].Cells[1].Value))
                {
                    hotkeys.Add(dgv.Rows[i].Cells[0].Value.ToString());
                    gateWays.Add(dgv.Rows[i].Cells[1].Value.ToString());
                    messages.Add(string.IsNullOrEmpty((string) dgv.Rows[i].Cells[2].Value)
                        ? ""
                        : dgv.Rows[i].Cells[2].Value.ToString());
                }
            }
            Properties.Settings.Default.HotKeys = hotkeys;
            Properties.Settings.Default.GateWays = gateWays;
            Properties.Settings.Default.Messages = messages;
            Properties.Settings.Default.Adapter = cmbAdapter.SelectedItem.ToString().Split('#')[1].Trim();
            Properties.Settings.Default.Save();
            File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,Environment.CurrentDirectory + "\\user.config",true);
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            InputBoxResult test = InputBox.Show("Enter Passwrod :"
                , "Open confirmation", "");

            if (test.ReturnCode == DialogResult.OK && !string.IsNullOrEmpty(test.Text))
            {
                if (Crypto.VerifyHash(test.Text, Properties.Settings.Default.Phrase, Crypto.Hash.SHA512,
                    Crypto.TextEncodings.UTF8, "funny Hah"))
                {
                    SaveSettings();
                    if (btnApply.Enabled && MessageBox.Show("Apply settings ?", "Apply", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        applySettings();
                        btnApply.Enabled = false;
                    }
                    btnSave.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter password to open", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CmbAdapter_SelectedIndexChanged(object sender, EventArgs e)
        {
            macAddress = cmbAdapter.SelectedItem.ToString().Split('#')[1].Trim();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Adapter))
            {
                if (cmbAdapter.SelectedItem.ToString().Split('#')[1].Trim() != Properties.Settings.Default.Adapter)
                {
                    btnSave.Enabled = true;
                }
            }
        }

        private void applySettings()
        {
            loadComboData();

            if (getSettings())
            {
                removeGlobalHotkeys();
                addGlobalHotkeys();
            }
        }
        private void BtnApply_Click(object sender, EventArgs e)
        {
            InputBoxResult test = InputBox.Show("Enter Passwrod :"
                , "Open confirmation", "");

            if (test.ReturnCode == DialogResult.OK && !string.IsNullOrEmpty(test.Text))
            {
                if (Crypto.VerifyHash(test.Text, Properties.Settings.Default.Phrase, Crypto.Hash.SHA512,
                    Crypto.TextEncodings.UTF8, "funny Hah"))
                {
                    applySettings();
                    if (btnSave.Enabled && MessageBox.Show("Save settings ?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        SaveSettings();
                        btnSave.Enabled = false;
                    }

                    btnApply.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Password is wrong", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter password to open", "Empty field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void Dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            btnApply.Enabled = true;
            btnSave.Enabled = true;
        }

        private void Dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            btnApply.Enabled = true;
            btnSave.Enabled = true;
        }
        public class data
        {
            public data(string _hotkey, string _gateWay,string _message)
            {
                this.hotKey = _hotkey;
                this.gateWay = _gateWay;
                this.message = _message;
                List<KeyModifier> _hotkey_KeyModifiers = new List<KeyModifier>();

                List<Keys> _hotkey_Keys = new List<Keys>();

                string[] _backingList = _hotkey.Split('+');
                foreach (string item in _backingList)
                {
                    string trimmedItem = item.Trim();
                    switch (trimmedItem)
                    {
                        case "Shift":
                            _hotkey_KeyModifiers.Add(KeyModifier.Shift);
                            break;
                        case "Alt":
                            _hotkey_KeyModifiers.Add(KeyModifier.Alt);
                            break;
                        case "Control":
                            _hotkey_KeyModifiers.Add(KeyModifier.Control);
                            break;
                        case "Win":
                            _hotkey_KeyModifiers.Add(KeyModifier.Win);
                            break;
                        case "None":
                            _hotkey_KeyModifiers.Add(KeyModifier.None);
                            break;
                        default:
                            _hotkey_Keys.Add((Keys)Enum.Parse(typeof(Keys), trimmedItem));
                            break;
                    }
                }
                this.keyModifiersValue = getSum<KeyModifier>(_hotkey_KeyModifiers);
                this.keysValue = getSum<Keys>(_hotkey_Keys);
            }
            public string hotKey { get; set; }
            public string gateWay { get; set; }

            public string message { get; set; }

            public int keyModifiersValue { get; set; }
            public int keysValue { get; set; }
        }

        private void ChkFast_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Fast = chkFast.Checked;
            Properties.Settings.Default.Save();
            File.Copy(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath,Environment.CurrentDirectory + "\\user.config",true);
        }
    }
    public class WindowWrapper : IWin32Window
    {
        private IntPtr _handle;

        public WindowWrapper(IntPtr handle)
        {
            this._handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
    }
}
