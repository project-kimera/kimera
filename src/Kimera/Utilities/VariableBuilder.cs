using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public static class VariableBuilder
    {
        public static string GetWorkDirectory()
        {
            return IoC.Get<Settings>().WorkDirectory;
        }

        public static string GetGameDirectory(Guid gameGuid)
        {
            string path = Path.Combine(GetWorkDirectory(), gameGuid.ToString());

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
