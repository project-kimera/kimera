using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Network.Utilities
{
    public static class WebHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<string> GetResponseAsync(string url)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        return body;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                string text = $"{ex.Message}\r\n{ex.InnerException.Message}";
                return text;
            }
        }

        public static async Task<bool> IsImageUrlAsync(string url)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string body = await response.Content.ReadAsStringAsync();

                        bool result = response.ToString().Contains("Content-Type: image");
                        return result;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
