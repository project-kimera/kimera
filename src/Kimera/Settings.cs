using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera
{
    public class Settings
    {
        public readonly static string DatabaseFilePath = @"data\database.sqlite";

        public readonly static string SettingsFilePath = @"data\settings.json";

        public readonly static Guid GUID_ALL_CATEGORY = Guid.Parse("B34641DF-A149-4670-BB27-D0A9696B3E3F");

        public readonly static Guid GUID_FAVORITE_CATEGORY = Guid.Parse("DD297C4A-3CC4-4E67-8E5F-512B02B5AF61");

        #region ::Application Settings::



        #endregion

        #region ::General::

        /// <summary>
        /// Gets or sets the value whether the application is started when the system is start up.
        /// </summary>
        public bool UseAutoStarter { get; set; } = false;

        #endregion

        #region ::Game::

        /// <summary>
        /// Gets or sets the work directory which is used by kimera.
        /// </summary>
        public string WorkDirectory { get; set; } = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows)), "Kimera");

        /// <summary>
        /// Gets or sets the master keys.
        /// </summary>
        public string MasterKeys { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value whether the auto remover is used or not.
        /// </summary>
        public bool UseAutoRemover { get; set; } = false;

        /// <summary>
        /// Gets or sets the interval(days) of auto removing task.
        /// </summary>
        public int AutoRemovingInterval { get; set; } = 7;

        #endregion

        #region ::Network::

        /// <summary>
        /// Gets or sets the value whether the Anti DPI Service is used or not.
        /// </summary>
        public bool UseAntiDPIService { get; set; } = true;

        #endregion

        public void Set(Settings settings)
        {
            UseAutoStarter = settings.UseAutoStarter;
            WorkDirectory = settings.WorkDirectory;
            MasterKeys = settings.MasterKeys;
            UseAutoRemover = settings.UseAutoRemover;
            AutoRemovingInterval = settings.AutoRemovingInterval;
            UseAntiDPIService = settings.UseAntiDPIService;
        }
    }
}
