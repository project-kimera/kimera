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
        protected virtual string ApiUrl { get; } = string.Empty;

        public Type MetadataServiceType
        {
            get => typeof(DLSiteMetadataService);
        }

        public async Task<List<SearchResult>> GetSearchResultsAsync(string keyword)
        {
            try
            {
                List<SearchResult> results = new List<SearchResult>();

                string url = WebHelper.GetLocalizedApiUrl(ApiUrl);
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    SearchResult result = new SearchResult();
                    result.ProductCode = token["product_id"].ToString();
                    result.Name = token["product_name"].ToString();
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

        protected override string ApiUrl
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

        protected override string ApiUrl
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

        protected override string ApiUrl
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

        protected override string ApiUrl
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

        protected override string ApiUrl
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

        protected override string ApiUrl
        {
            get => "https://www.dlsite.com/pro/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/";
        }
    }
}
