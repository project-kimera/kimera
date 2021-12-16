using Kimera.IO;
using Kimera.Network.Entities;
using Kimera.Network.Services.Interfaces;
using Kimera.Network.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kimera.Network.Services
{
    public class DLSiteSharedClass
    {
        protected virtual string SearchApiUrl { get; } = string.Empty;

        public Type MetadataServiceType
        {
            get => typeof(DLSiteMetadataService);
        }

        public async Task<List<SearchResult>> GetSearchResultsAsync(string keyword)
        {
            try
            {
                DLSiteMetadataService metadataService = new DLSiteMetadataService();

                List<SearchResult> results = new List<SearchResult>();

                string searchApiUrl = WebHelper.GetLocalizedApiUrl(SearchApiUrl);
                string encodedKeyword = HttpUtility.UrlEncode(keyword);
                string searchApiResponse = await WebHelper.GetResponseAsync(string.Format(searchApiUrl, encodedKeyword));
                searchApiResponse = TextManager.RemoveBOM(searchApiResponse);
                TextManager.WriteTextFile(@"E:\test.txt", searchApiResponse, Encoding.UTF8);

                JArray array = JArray.Parse(searchApiResponse);

                foreach (JObject item in array.Children<JObject>())
                {
                    SearchResult result = new SearchResult();
                    result.ProductCode = item["product_id"].ToString();
                    result.Name = item["product_name"].ToString();
                    result.ThumbnailUrl = await metadataService.GetThumbnailUrlAsync(result.ProductCode, true);

                    results.Add(result);
                }

                return results;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteHomeSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Home";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/home/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }

    internal class DLSiteManiaxSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Maniax";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/maniax/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }
    internal class DLSiteGirlsSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Girls";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/girls/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }


    internal class DLSiteBLSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite BL";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/bl/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }


    internal class DLSiteSoftwareSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Software";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/soft/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }


    internal class DLSiteProSearchService : DLSiteSharedClass, ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Pro";
        }

        protected override string SearchApiUrl
        {
            get => "https://www.dlsite.com/pro/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }
}
