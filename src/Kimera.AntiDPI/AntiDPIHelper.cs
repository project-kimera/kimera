using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.AntiDPI
{
    public class AntiDPIHelper : IDisposable
    {
        public void WriteResources(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string goodbyedpiPath = Path.Combine(directory, "goodbyedpi.exe");
            string winDivertDllPath = Path.Combine(directory, "WinDivert.dll");
            string winDivert32SysPath = Path.Combine(directory, "WinDivert32.sys");
            string winDivert64SysPath = Path.Combine(directory, "WinDivert64.sys");

            if (File.Exists(goodbyedpiPath))
            {
                File.Delete(goodbyedpiPath);
            }

            if (File.Exists(winDivertDllPath))
            {
                File.Delete(winDivertDllPath);
            }

            if (File.Exists(winDivert32SysPath))
            {
                File.Delete(winDivert32SysPath);
            }

            if (File.Exists(winDivert64SysPath))
            {
                File.Delete(winDivert64SysPath);
            }

            if (Environment.Is64BitOperatingSystem)
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                using (FileStream stream = new FileStream(goodbyedpiPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.goodbyedpi.exe").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(winDivertDllPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.WinDivert.dll").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(winDivert64SysPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x64.WinDivert64.sys").CopyTo(stream);
                }
            }
            else
            {
                Assembly currentAssembly = Assembly.GetExecutingAssembly();

                using (FileStream stream = new FileStream(goodbyedpiPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.goodbyedpi.exe").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(winDivertDllPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.WinDivert.dll").CopyTo(stream);
                }

                using (FileStream stream = new FileStream(winDivert32SysPath, FileMode.CreateNew))
                {
                    currentAssembly.GetManifestResourceStream("Kimera.AntiDPI.Resources.x86.WinDivert32.sys").CopyTo(stream);
                }
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
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.
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
