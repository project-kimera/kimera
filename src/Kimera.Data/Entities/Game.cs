using Kimera.Data.Enums;
using System;

namespace Kimera.Data.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public GameMetadata GameMetadata { get; set; }

        public PackageMetadata PackageMetadata { get; set; }

        public PackageStatus PackageStatus { get; set; }

        public double StarRate { get; set; }

        public string Memo { get; set; }

        public int PlayTime { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime FirstTime { get; set; }

        public DateTime LastTime { get; set; }

        public Game(Guid id, PackageStatus packageStatus)
        {
            Id = id;
            PackageStatus = packageStatus;
        }
    }
}
