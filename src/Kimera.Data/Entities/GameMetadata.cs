using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kimera.Data.Entities
{
    public class GameMetadata
    {
        public Game Game { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string AdmittedAge { get; set; }

        public string Genres { get; set; } // It should be separated by comma.

        public string Tags { get; set; } // It should be separated by comma.

        public virtual ICollection<CultureInfo> SupportedLanguages { get; set; } = new List<CultureInfo>();

        public virtual Version Version { get; set; }

        public string ThumbnailUrl { get; set; }

        public string HomepageUrl { get; set; }
    }
}
