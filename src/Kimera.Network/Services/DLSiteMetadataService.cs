using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Kimera.Network.Services.Interfaces;
using Kimera.Network.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Kimera.Network.Services
{
    internal class DLSiteMetadataService : IMetadataService
    {
        private const string URL_DLSITE_PRODUCT_HOMEPAGE = "https://www.dlsite.com/home/work/=/product_id/{0}.html";
        private const string URL_DLSITE_PRODUCT_INFO_API = "https://www.dlsite.com/home/api/=/product.json?workno={0}";

        public string[] ProductCodeRegexs { get; init; } = { "RJ[0-9]{6}", "VJ[0-9]{6}", "BJ[0-9]{6}", "[0-9]{6}" };

        public async Task<bool> IsAvailableProductAsync(string productCode)
        {
            try
            {
                string url = WebHelper.GetLocalizedApiUrl(URL_DLSITE_PRODUCT_INFO_API);
                string response = await WebHelper.GetResponseAsync(string.Format(url, productCode));

                JArray array = JArray.Parse(response);

                if (array.Count != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<GameMetadata> GetMetadataAsync(string productCode)
        {
            try
            {
                string url = WebHelper.GetLocalizedApiUrl(URL_DLSITE_PRODUCT_INFO_API);
                string response = await WebHelper.GetResponseAsync(string.Format(url, productCode));

                JArray array = JArray.Parse(response);

                if (array.Count == 1)
                {
                    GameMetadata metadata = new GameMetadata();
                    metadata.Name = array[0]["work_name"].ToString();
                    metadata.Description = array[0]["intro_s"].ToString();
                    metadata.Creator = array[0]["maker_name"].ToString();
                    metadata.Genres = array[0]["work_type_string"].ToString();
                    metadata.Memo = string.Empty;
                    metadata.PlayTime = 0;
                    metadata.IsFinished = false;
                    metadata.ThumbnailUri = string.Concat("https:", array[0]["image_main"]["url"].ToString());
                    metadata.IconUri = string.Concat("https:", array[0]["image_thum"]["url"].ToString());
                    metadata.HomepageUrl = string.Format(URL_DLSITE_PRODUCT_HOMEPAGE, productCode);

                    // Admitted Age
                    string ageCategoryString = array[0]["age_category_string"].ToString();

                    if (ageCategoryString == "general")
                    {
                        metadata.AdmittedAge = Age.All;
                    }
                    else if (ageCategoryString == "r15")
                    {
                        metadata.AdmittedAge = Age.Age15;
                    }
                    else if (ageCategoryString == "adult")
                    {
                        metadata.AdmittedAge = Age.Age18;
                    }
                    else
                    {
                        metadata.AdmittedAge = Age.All;
                    }

                    // Tags
                    List<string> tagsStrings = new List<string>();
                    JToken[] tagsArray = array[0]["genres"].ToArray();

                    foreach (JToken genre in tagsArray)
                    {
                        tagsStrings.Add(genre["name"].ToString());
                    }

                    metadata.Tags = string.Join(',', tagsStrings);

                    // Supported Languages
                    List<string> languageStrings = new List<string>();
                    string options = array[0]["options"].ToString();

                    if (options.Contains("JPN"))
                    {
                        languageStrings.Add("日本語");
                    }

                    if (options.Contains("ENG"))
                    {
                        languageStrings.Add("English");
                    }

                    if (options.Contains("CHI"))
                    {
                        languageStrings.Add("中國語");
                    }

                    if (options.Contains("CHI_HANS"))
                    {
                        languageStrings.Add("简体字");
                    }

                    if (options.Contains("CHI_HANT"))
                    {
                        languageStrings.Add("正體字");
                    }

                    if (options.Contains("KO_KR"))
                    {
                        languageStrings.Add("한국어");
                    }

                    metadata.SupportedLanguages = string.Join(',', languageStrings);

                    // Score
                    double sum = 0;
                    double count = 0;

                    for (int i = 1; i <= 5; i++)
                    {
                        int currentCount = int.Parse(array[0]["rate_count_detail"][$"{i}"].ToString());
                        sum += currentCount * i;
                        count += currentCount;
                    }

                    metadata.Score = Math.Round(sum / count, 2);

                    return metadata;
                }
                else
                {
                    return new GameMetadata();
                }
            }
            catch
            {
                throw;
            }         
        }
    }
}
