using Kimera.Abstractions.Bases;
using Kimera.Data.Entities;
using System.Threading.Tasks;

namespace Kimera.Abstractions
{
    public interface IMetadataManager : IPluginBase
    {
        public bool IsAsync { get; }

        public bool GetGameMetadata(object parameter, out GameMetadata metadata);

        public Task<bool> GetGameMetadataAsync(object parameter, out GameMetadata metadata);

        public bool GetPackageMetadata(object parameter, out PackageMetadata metadata);

        public Task<bool> GetPackageMetadataAsync(object parameter, out PackageMetadata metadata);
    }
}
