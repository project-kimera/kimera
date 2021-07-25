using Kimera.Data.Entities;
using System.Threading.Tasks;

namespace Kimera.Network.Services.Interfaces
{
    internal interface IMetadataService
    {
        public string[] ProductCodeRegexs { get; init; }

        public Task<bool> IsAvailableProductAsync(string productCode);

        public Task<GameMetadata> GetMetadataAsync(string productCode);
    }
}
