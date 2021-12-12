using Newtonsoft.Json;
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
        public readonly static string ApplicationName = "Kimera";

        public readonly static string DatabaseFilePath = @"data\database.sqlite";

        public readonly static string SettingsFilePath = @"data\settings.json";

        #region ::System Settings::

        [JsonIgnore]
        public static Dictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        #endregion

        #region ::Application Settings::



        #endregion

        #region ::General::

        /// <summary>
        /// Gets or sets the value whether the dark theme is used or not.
        /// </summary>
        public bool UseDarkTheme { get; set; } = true;

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
        /// Gets or sets the value whether the auto remover is used or not.
        /// </summary>
        public bool UseAutoRemovingReminder { get; set; } = true;

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
            UseDarkTheme = settings.UseDarkTheme;
            WorkDirectory = settings.WorkDirectory;
            MasterKeys = settings.MasterKeys;
            UseAutoRemover = settings.UseAutoRemover;
            UseAutoRemovingReminder = settings.UseAutoRemovingReminder;
            AutoRemovingInterval = settings.AutoRemovingInterval;
            UseAntiDPIService = settings.UseAntiDPIService;
        }
    }
}
