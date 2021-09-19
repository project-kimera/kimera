using Kimera.Network.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kimera.Network.Services.Interfaces
{
    public interface ISearchService
    {
        public string ServiceName { get; }

        public Type MetadataServiceType { get; }

        public Task<List<SearchResult>> GetSearchResultsAsync(string keyword);
    }
}
