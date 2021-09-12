using Kimera.Data.Entities;

namespace Kimera.Entities
{
    public class GameRegistration
    {
        public bool IsRegistrable { get; set; } = false;

        public PackageMetadata PackageMetadata { get; set; } = new PackageMetadata();

        public GameMetadata GameMetadata { get; set; } = new GameMetadata();

        public Category Category { get; set; } = new Category();
    }
}
