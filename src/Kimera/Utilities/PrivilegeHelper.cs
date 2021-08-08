using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

namespace Kimera.Utilities
{
    internal static class PrivilegeHelper
    {
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            if (identity != null)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        public static void RunAsAdiministrator()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.FileName = Path.Combine(Environment.CurrentDirectory, "Kimera.exe");
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.Verb = "runas";
                Process.Start(startInfo);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The privilege helper couldn't run the process as administrator privilege.");
                return;
            }
        }
    }
}
