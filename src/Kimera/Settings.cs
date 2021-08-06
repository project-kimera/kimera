using Kimera.Entities;
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
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public readonly static Guid GUID_ALL_CATEGORY = Guid.Parse("B34641DF-A149-4670-BB27-D0A9696B3E3F");

        public readonly static Guid GUID_FAVORITE_CATEGORY = Guid.Parse("DD297C4A-3CC4-4E67-8E5F-512B02B5AF61");

        #region ::Application Settings::

        /// <summary>
        /// Gets or sets the latest search category.
        /// </summary>
        public SearchCategory LatestSearchCategory { get; set; } = SearchCategory.All;

        #endregion

        #region ::General::

        /// <summary>
        /// Gets or sets the work directory which is used by kimera.
        /// </summary>
        public string WorkDirectory { get; set; } = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows)), "Kimera");

        #endregion

        #region ::Network::

        /// <summary>
        /// Gets or sets the value whether the Anti DPI Service is used or not.
        /// </summary>
        public bool UseAntiDPIService { get; set; } = true;

        #endregion

        #region ::Game Service::

        /// <summary>
        /// Gets or sets the value whether the auto remover is used or not.
        /// </summary>
        public bool UseAutoRemover { get; set; } = false;

        /// <summary>
        /// Gets or sets the interval(days) of auto removing task.
        /// </summary>
        public int AutoRemovingInterval { get; set; } = 14;

        /// <summary>
        /// Gets or sets the list of master keys.
        /// </summary>
        public List<string> MasterKeyCollection { get; set; } = new List<string>();

        #endregion
    }
}
