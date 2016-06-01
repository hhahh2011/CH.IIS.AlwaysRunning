using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.ServiceProcess;
using System.Text;


namespace CH.IIS.AlwaysRunning
{
    static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            if (!Environment.UserInteractive)
            {
                RunAsService();
                return;
            }

            string exeArg = string.Empty;

            if (args == null || args.Length < 1)
            {
                Console.WriteLine("Welcome to CH.IIS.AlwaysRunning!");

                Console.WriteLine("Please press a key to continue...");
                Console.WriteLine("-[i]: Install this application as a Windows Service;");
                Console.WriteLine("-[u]: Uninstall this Windows Service application;");

                while (true)
                {
                    exeArg = Console.ReadKey().KeyChar.ToString();
                    Console.WriteLine();

                    if (Run(exeArg, null))
                        break;
                }
            }
            else
            {
                exeArg = args[0];

                if (!string.IsNullOrEmpty(exeArg))
                    exeArg = exeArg.TrimStart('-');

                Run(exeArg, args);
            }
        }
        private static bool Run(string exeArg, string[] startArgs)
        {
            switch (exeArg.ToLower())
            {
                case ("i"):
                    SelfInstaller.InstallMe();
                    return true;

                case ("u"):
                    SelfInstaller.UninstallMe();
                    return true;
                default:
                    Console.WriteLine("Invalid argument!");
                    return false;
            }
        }
        
        static void RunAsService()
        {
            ServiceBase[] servicesToRun;

            servicesToRun = new ServiceBase[] { new MainService() };

            ServiceBase.Run(servicesToRun);
        }
    }
}