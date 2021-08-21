using Kimera.Data.Entities;

namespace Kimera.Entities
{
    public class GameRegistration
    {
        public bool IsRegistrable { get; set; }

        public PackageMetadata PackageMetadata { get; set; }

        public GameMetadata GameMetadata { get; set; }

        public Category Category { get; set; }
    }
}
