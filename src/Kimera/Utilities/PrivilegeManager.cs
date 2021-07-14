using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.Utilities
{
    public static class PrivilegeManager
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
                MessageBox.Show(ex.Message, "Kimera", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
