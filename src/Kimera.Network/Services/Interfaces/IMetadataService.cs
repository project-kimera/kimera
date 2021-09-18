using Kimera.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kimera.Network.Services.Interfaces
{
    public interface IMetadataService
    {
        public string[] ProductCodeRegexs { get; init; }

        public Task<bool> IsAvailableProductAsync(string productCode);

        public Task<GameMetadata> GetMetadataAsync(string productCode);
    }
}
