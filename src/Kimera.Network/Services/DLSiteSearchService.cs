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
    internal class DLSiteHomeSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Home";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/home/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteManiaxSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Maniax";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/maniax/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteGirlsSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Girls";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/girls/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteBLSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite BL";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/bl/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteSoftwareSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Software";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/soft/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }

    internal class DLSiteProSearchService : ISearchService
    {
        public string ServiceName
        {
            get => "DLSite Pro";
        }

        public async Task<Dictionary<string, string>> GetSearchResult(string keyword)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                string url = WebHelper.GetLocalizedApiUrl("https://www.dlsite.com/pro/sapi/=/language/jp/keyword/{0}/per_page/30/format/json/");
                string encodedKeyword = HttpUtility.UrlEncodeUnicode(keyword);
                string response = await WebHelper.GetResponseAsync(string.Format(url, encodedKeyword));

                JArray array = JArray.Parse(response);

                foreach (JObject token in array.Children<JObject>())
                {
                    result.Add(token["product_id"].ToString(), token["product_name"].ToString());
                }

                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}
