using Kimera.Data.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public static class DLSiteBroker
    {
        private const string URL_DLSITE_PRODUCT_INFO_API = "https://www.dlsite.com/maniax/api/=/product.json?workno={0}&locale=ko-KR";

        public static async Task<GameMetadata> GetGameMetadataAsync(string productCode)
        {
            string response = await HttpHelper.GetResponseAsync(string.Format(URL_DLSITE_PRODUCT_INFO_API, productCode));

            JArray array = JArray.Parse(response);

            if (array.Count == 1)
            {
                
            }
        }
    }
}
