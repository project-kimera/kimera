using System;
using System.Collections.Generic;
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

        public readonly static Guid GUID_UNCATEGORIZED_CATEGORY = Guid.Parse("C528DFF5-A584-4D26-A386-B3C16D8274B9");

        public readonly static Guid GUID_FAVORITE_CATEGORY = Guid.Parse("DD297C4A-3CC4-4E67-8E5F-512B02B5AF61");
    }
}
