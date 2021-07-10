using System;

namespace Kimera.Data.Entities
{
    public class GameMetadata
    {
        public int Id { get; set; }

        public Guid SystemId { get; set; }

        public Guid Game { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string AdmittedAge { get; set; }

        public string Genres { get; set; } // It should be separated by comma.

        public string Tags { get; set; } // It should be separated by comma.

        public string SupportedLanguages { get; set; }  // It should be separated by comma.

        public Version Version { get; set; }

        public string ThumbnailUrl { get; set; }

        public string HomepageUrl { get; set; }

        public virtual Game GameNavigation { get; set; }
    }
}
