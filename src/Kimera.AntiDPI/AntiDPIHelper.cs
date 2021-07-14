using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.AntiDPI
{
    public class AntiDPIHelper : IDisposable
    {
        private string _directory = string.Empty;

        private string _goodbyedpiExePath = string.Empty;
        private string _winDivertDllPath = string.Empty;
        private string _winDivert32SysPath = string.Empty;
        private string _winDivert64SysPath = string.Empty;

        private Process _process = new Process();

        public AntiDPIHelper(string directory)
        {
            _directory = directory;
            _goodbyedpiExePath = Path.Combine(_directory, "goodbyedpi.exe");
            _winDivertDllPath = Path.Combine(_directory, "WinDivert.dll");
            _winDivert32SysPath = Path.Combine(_directory, "WinDivert32.sys");
            _winDivert64SysPath = Path.Combine(_directory, "WinDivert64.sys");
        }

        public bool CheckResources()
        {
            if (!Directory.Exists(_directory))
            {
                return false;
            }

            if (Environment.Is64BitOperatingSystem)
            {
                if (File.Exists(_goodbyedpiExePath) && File.Exists(_winDivertDllPath) && File.Exists(_winDivert64SysPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (File.Exists(_goodbyedpiExePath) && File.Exists(_winDivertDllPath) && File.Exists(_winDivert32SysPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void EnsureResources()
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            if (File.Exists(_goodbyedpiExePath))
            {
                File.Delete(_goodbyedpiExePath);
            }

            if (File.Exists(_winDivertDllPath))
            {
                File.Delete(_winDivertDllPath);
            }

            if (File.Exists(_winDivert32SysPath))
            {
                File.Delete(_winDivert32SysPath);
            }

            if (File.Exists(_winDivert64SysPath))
            {
                File.Delete(_winDivert64SysPath);
            }

            if (Environment.Is64BitOperatingSystem)
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                using (FileStream stream = new FileStream(_goodbyedpiExePath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.goodbyedpi.exe").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(_winDivertDllPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.WinDivert.dll").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(_winDivert64SysPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.WinDivert64.sys").CopyTo(stream);
                }
            }
            else
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                using (FileStream stream = new FileStream(_goodbyedpiExePath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.goodbyedpi.exe").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(_winDivertDllPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.WinDivert.dll").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(_winDivert32SysPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.WinDivert32.sys").CopyTo(stream);
                }
            }
        }

        public bool Start()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = _goodbyedpiExePath;
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "-1";

                _process.StartInfo = startInfo;
                _process.Start();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                _process.Kill();
                _process.Close();
                _process.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region ::IDisposable Members::

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~AntiDPIHelper()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
