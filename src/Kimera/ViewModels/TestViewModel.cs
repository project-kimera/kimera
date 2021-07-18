using Kimera.Commands;
using Kimera.Data.Entities;
using Kimera.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera.ViewModels
{
    public class TestViewModel : ViewModelBase
    {
        public DelegateCommand DLH_IVPCCommand { get; }

        public DelegateCommand DLH_GPCFPCommand { get; }

        public DelegateCommand DLH_GGMACommand { get; }

        public DelegateCommand HTTP_GRACommand { get; }

        public TestViewModel()
        {
            DLH_IVPCCommand = new DelegateCommand(DLH_IVPC);
            DLH_GPCFPCommand = new DelegateCommand(DLH_GPCFP);
            DLH_GGMACommand = new DelegateCommand(DLH_GGMA);
            HTTP_GRACommand = new DelegateCommand(HTTP_GRA);
        }

        private void DLH_IVPC()
        {
            List<string> testStrings = new List<string>() { "RJ109283", "VJ024593", "BJ124052", "RJ204" ,"2505", "RJ", "204550" };

            string result = string.Empty;

            foreach (string testString in testStrings)
            {
                result += $"{testString} - {DLSiteHelper.IsValidProductCode(testString)}\r\n";
            }

            MessageBox.Show(result, "DLH-IPVC Test Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DLH_GPCFP()
        {
            List<string> testStrings = new List<string>();
            testStrings.Add(@"C:\[RJ204353] TEST GAME\TEST GAME\www\game.exe");
            testStrings.Add(@"E:\games\test\RJ203454\VJ024503\game.exe");
            testStrings.Add(@"D:\game.exe");

            string result = string.Empty;

            foreach (string testString in testStrings)
            {
                result += $"{testString} - '{DLSiteHelper.GetProductCodeFromPath(testString)}'\r\n";
            }

            MessageBox.Show(result, "DLH-GPCFP Test Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void DLH_GGMA()
        {
            GameMetadata metadata = await DLSiteHelper.GetGameMetadataAsync("RJ311136"); // SHERYL

            string result = $"Name : {metadata.Name}\r\n" +
                $"Description : {metadata.Description}\r\n" +
                $"Creator : {metadata.Creator}\r\n" +
                $"AdmittedAge : {metadata.AdmittedAge}\r\n" +
                $"Genres : {metadata.Genres}\r\n" +
                $"Tags : {metadata.Tags}\r\n" +
                $"Languages : {metadata.SupportedLanguages}\r\n" +
                $"Score : {metadata.Score}\r\n" +
                $"Thumbnail URL : {metadata.ThumbnailUri}\r\n" +
                $"Icon URL : {metadata.IconUri}\r\n" +
                $"Homepage URL : {metadata.HomepageUrl}\r\n";

            MessageBox.Show(result, "DLH-GGMA Test Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void HTTP_GRA()
        {
            string testURL = "https://www.dlsite.com/maniax/work/=/product_id/RJ311136.html";

            string result = await HttpHelper.GetResponseAsync(testURL);

            MessageBox.Show(result, "HTTP-GRA Test Result", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
