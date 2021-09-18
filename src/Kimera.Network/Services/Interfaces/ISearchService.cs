using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kimera.Network.Services.Interfaces
{
    public interface ISearchService
    {
        public string ServiceName { get; }

        public Task<Dictionary<string, string>> GetSearchResult(string keyword);
    }
}
