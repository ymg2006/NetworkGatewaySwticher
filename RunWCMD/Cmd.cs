using System.Diagnostics;

namespace RunWCMD
{
    class Cmd
    {
        public static string ExecuteCommand(string command,string args = "/c", bool _RedirectStandardError = true,bool _RedirectStandardOutput = true,bool _UseShellExecute = false,bool _CreateNoWindow = true) 
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", string.Format("{0} {1}",args , command))
            {
                RedirectStandardError = _RedirectStandardError,
                RedirectStandardOutput = _RedirectStandardOutput,
                UseShellExecute = _UseShellExecute,
                CreateNoWindow = _CreateNoWindow
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = procStartInfo;
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();

                if (string.IsNullOrEmpty(output))
                    output = proc.StandardError.ReadToEnd();

                return output;
            }
        }
    }
}