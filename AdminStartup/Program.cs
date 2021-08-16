using System;
using System.Security.Principal;
using Microsoft.Win32.TaskScheduler;

namespace AdminStartup
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.WriteLine("Admin Startup addition must be run as admin !");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("1 Register Service");
            Console.WriteLine("2 Remove Service");
            Console.WriteLine("Enter choice:");

            int input = (int.TryParse(Console.ReadLine(),out input)) ? input : 0;
            Console.Clear();
            if (input == 1)
            {
                using (TaskService ts = new TaskService())
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "Start Gateway Switcher program";
                    td.Triggers.Add(new LogonTrigger());
                    td.Actions.Add(new ExecAction(Environment.CurrentDirectory + "\\RunWCMD.exe", null, null));
                    td.Settings.AllowDemandStart = true;
                    td.Settings.Compatibility = TaskCompatibility.V2_3;
                    td.Settings.AllowHardTerminate = true;
                    td.Settings.MultipleInstances = TaskInstancesPolicy.StopExisting;
                    td.Settings.StopIfGoingOnBatteries = false;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    foreach (Task selTask in ts.RootFolder.AllTasks)
                    {
                        if (selTask.Name == "GatewaySwitcher")
                        {
                            ts.RootFolder.DeleteTask(selTask.Name);
                        }
                    }
                    ts.RootFolder.RegisterTaskDefinition(@"GatewaySwitcher", td);
                }
                Console.Clear();
                Console.WriteLine("Service created ...");
                Console.ReadKey();
            }
            else if (input == 2)
            {
                using (TaskService ts = new TaskService())
                {
                    foreach (Task selTask in ts.RootFolder.AllTasks)
                    {
                        if (selTask.Name == "GatewaySwitcher")
                        {
                            ts.RootFolder.DeleteTask(selTask.Name);
                        }
                    }
                }
                Console.Clear();
                Console.WriteLine("Service removed ...");
                Console.ReadKey();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Please enter valid input ...");
                Console.ReadKey();
                Console.Clear();
                Main( new string[0]);
            }
        }
    }
}
