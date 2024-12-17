using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public static class Logger
    {
        private static readonly string logFilePath = "debug.log";
        public static void Initialize()
        {
#if DEBUG
            if (File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, string.Empty);
                Log("Log initalized.");
                Log("Resetting previous log...");
            }
            else
            {
                File.WriteAllText(logFilePath, string.Empty);
                Log("Log initalized.");
            }


#else
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
#endif

        }

        public static void Log(string message)
        {
            // Tambahkan timestamp ke setiap log
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
#if DEBUG
            Debug.WriteLine(logMessage);
            File.AppendAllText(logFilePath, logMessage);
#endif
        }
    }

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            Logger.Initialize();
            Logger.Log("App launched in DEBUG mode");
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                Logger.Log($"Assembly Loaded: {args.LoadedAssembly.FullName}");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Logger.Log($"Custom Log: Unhandled Exception: {args.ExceptionObject}");
            };

            Application.ThreadException += (sender, args) =>
            {
                Logger.Log($"Custom Log: Thread Exception: {args.Exception}");
            };
#else
            Console.WriteLine("App started in RELEASE mode. Logs are disabled.");
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*Register regisForm = new Register();
            if (regisForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new Menu());
            }*/
            Logger.Log("Launching Main Instance");
            Application.Run(new Login());
            Logger.Log("App exited.");

        }
    }
}
