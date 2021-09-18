using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public static class RegistryManager
    {
        private static readonly string _startupRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void AddStartupProgram(string programName, string executablePath)
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(_startupRegPath, true))
            {
                try
                {   
                    regKey.SetValue(programName, executablePath);

                    regKey.Close();
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void RemoveStartupProgram(string programName)
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(_startupRegPath, true))
            {
                try
                {
                    if (regKey.GetValue(programName) != null)
                        regKey.DeleteValue(programName, false);

                    regKey.Close();
                }
                catch
                {
                    throw;
                }
            }
        }

        public static bool IsStartupProgram(string programName)
        {
            using (var regKey = Registry.CurrentUser.OpenSubKey(_startupRegPath, true))
            {
                try
                {
                    if (regKey.GetValue(programName) != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
