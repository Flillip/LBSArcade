using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OSHelper
{
    internal class Protector
    {
        private Process arcade;

        public Protector()
        {
            string? path = Environment.ProcessPath;

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            AddStartup(Process.GetCurrentProcess().ProcessName, path);

            if (File.Exists("./close"))
                File.Delete("./close");
        }

        /// <summary>
        /// Protects the program by reopening it if it is closed (either due to an error or being closed manually).
        /// </summary>
        public void Protect()
        {
            Logger.Debug("Starting Arcade");

            this.arcade = new Process();
            this.arcade.StartInfo.FileName = "./LBSArcade";
            this.arcade.Start();

            new Thread(() =>
            {
                DLLImports.SetWindowFocus(this.arcade, true);

                Logger.Debug("Waiting until Arcade crashes or is closed");

                while (this.arcade.HasExited == false)
                    Thread.Sleep(100);

                Logger.Debug("Aracde closed");

                this.arcade.Dispose();

                if (File.Exists("./close"))
                {
                    Logger.Debug("close file exists. Exiting");
                    File.Delete("./close");
                    Environment.Exit(0);
                }

                else
                {
                    Logger.Debug("Starting new thread to keep it alive");
                    Protect(); // Incase it's closed we need to reopen it
                }

            }).Start();
        }

        internal static void AddStartup(string appName, string path)
        {
            // Windows only code, so return when not on windows
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return;

            using RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key != null)
                key.SetValue(appName, "\"" + path + "\"");
        }
    }
}
