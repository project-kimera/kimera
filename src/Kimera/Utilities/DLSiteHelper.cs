using Kimera.Data.Entities;
using Kimera.Data.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public static class DLSiteHelper
    {
        private static readonly string[] ARRAY_PRODUCT_CODE_REGEX = { "RJ[0-9]{1,6}", "VJ[0-9]{1,6}", "BJ[0-9]{1,6}" };

        private const string URL_DLSITE_PRODUCT_HOMEPAGE = "https://www.dlsite.com/home/work/=/product_id/{0}.html";
        private const string URL_DLSITE_PRODUCT_INFO_API = "https://www.dlsite.com/home/api/=/product.json?workno={0}";

        public static bool IsValidProductCode(string productCode)
        {
            foreach (string regex in ARRAY_PRODUCT_CODE_REGEX)
            {
                if (Regex.IsMatch(productCode, regex))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetProductCodeFromPath(string path)
        {
            string[] pathChunks = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = pathChunks.Length - 1; i>=0; i--)
            {
                foreach (string regex in ARRAY_PRODUCT_CODE_REGEX)
                {
                    Match match = Regex.Match(pathChunks[i], regex);

                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
            }

            return string.Empty;
        }

        public static async Task<GameMetadata> GetGameMetadataAsync(string productCode)
        {
            string response = await HttpHelper.GetResponseAsync(string.Format(URL_DLSITE_PRODUCT_INFO_API, productCode));

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
                metadata.IsCompleted = false;
                metadata.ThumbnailUrl = array[0]["image_thum"]["url"].ToString();
                metadata.HomepageUrl = string.Format(URL_DLSITE_PRODUCT_HOMEPAGE, productCode);

                // Admitted Age
                string ageCategoryString = array[0]["age_category_string"].ToString();

                if (ageCategoryString == "general")
                {
                    metadata.AdmittedAge = Age.All;
                }
                else if (ageCategoryString == "r15")
                {
                    metadata.AdmittedAge = Age.R15;
                }
                else if (ageCategoryString == "adult")
                {
                    metadata.AdmittedAge = Age.R18;
                }
                else
                {
                    metadata.AdmittedAge = Age.All;
                }

                // Tags
                List<string> genreStrings = new List<string>();
                JToken[] genresArray = array["genres"].ToArray();
                
                foreach (JToken genre in genresArray)
                {
                    genreStrings.Add(genre["name"].ToString());
                }

                metadata.Genres = string.Join(',', genreStrings);

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

                // Score
                int sum = 0;
                int count = 0;

                for (int i = 1; i <= 5; i++)
                {
                    int currentCount = int.Parse(array[0]["rate_count_detail"][i].ToString());
                    sum += currentCount * i;
                    count += currentCount;
                }

                metadata.Score = sum / count;

                return metadata;
            }
            else
            {
                return new GameMetadata();
            }
        }
    }
}
