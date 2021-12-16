using Caliburn.Micro;
using Kimera.Data.Entities;
using Kimera.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kimera.Utilities
{
    public class VariableBuilder
    {
        private Game _game = new Game();

        private Dictionary<string, string> _environmentVariables = new Dictionary<string, string>();
        private Dictionary<string, string> _dynamicVariables = new Dictionary<string, string>();

        public VariableBuilder(Game game)
        {
            if (game != null)
            {
                _game = game;

                InitializeVariables();
            }
        }

        private void InitializeVariables()
        {
            var variables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            foreach (DictionaryEntry entry in variables)
            {
                _environmentVariables.Add((string)entry.Key, (string)entry.Value);
            }

            // BaseDirectory
            _dynamicVariables.Add("BaseDirectory", Directory.GetCurrentDirectory());

            // WorkDirectory
            _dynamicVariables.Add("WorkDirectory", GetWorkDirectory());

            // WorkDirectory
            _dynamicVariables.Add("EntryPointPath", _game.PackageMetadataNavigation.EntryPointFilePath);

            // WorkDirectory
            _dynamicVariables.Add("ActualEntryPointPath", GetActualEntryPointPath());

            // WorkDirectory
            _dynamicVariables.Add("PackageType", _game.PackageMetadataNavigation.Type.ToString());

            // WorkDirectory
            _dynamicVariables.Add("PackageStatus", _game.PackageStatus.ToString());

            // WorkDirectory
            _dynamicVariables.Add("GameGuid", _game.SystemId.ToString());
        }

        public string GetWorkDirectory()
        {
            return IoC.Get<Settings>().WorkDirectory;
        }

        public string GetGameDirectory()
        {
            return Path.Combine(GetWorkDirectory(), _game.SystemId.ToString());
        }

        public string GetActualEntryPointPath()
        {
            return Path.Combine(GetGameDirectory(), _game.PackageMetadataNavigation.EntryPointFilePath);
        }

        public string FillVariables(string text)
        {
            string result = text;

            List<string> requiredVariables = TextManager.ParseTexts(text, "%", "%").ToList();

            foreach (var variable in _environmentVariables)
            {
                string targetVariable = requiredVariables.Find(s => string.Compare(s, variable.Key, true) == 0);

                if (!string.IsNullOrEmpty(targetVariable))
                {
                    result = Regex.Replace(result, $"%{targetVariable}%", variable.Value, RegexOptions.IgnoreCase);
                }
            }

            foreach (var variable in _dynamicVariables)
            {
                string targetVariable = requiredVariables.Find(s => string.Compare(s, variable.Key, true) == 0);

                if (!string.IsNullOrEmpty(targetVariable))
                {
                    result = Regex.Replace(result, $"%{targetVariable}%", variable.Value, RegexOptions.IgnoreCase);
                }
            }

            return result;
        }
    }
}
