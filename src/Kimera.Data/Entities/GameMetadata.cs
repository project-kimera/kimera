using Kimera.Data.Enums;
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

        public Age AdmittedAge { get; set; }

        public string Genres { get; set; } // It should be separated by comma.

        public string Tags { get; set; } // It should be separated by comma.

        public string SupportedLanguages { get; set; }  // It should be separated by comma.

        public double Score { get; set; }

        public string Memo { get; set; }

        public int PlayTime { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime FirstTime { get; set; }

        public DateTime LastTime { get; set; }

        public string IconUri { get; set; }

        public string ThumbnailUri { get; set; }

        public string HomepageUrl { get; set; }

        public virtual Game GameNavigation { get; set; }

        public GameMetadata Copy()
        {
            return (GameMetadata)this.MemberwiseClone();
        }
    }
}
