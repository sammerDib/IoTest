using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740
{
    /// <summary>
    /// Class responsible to handle recipe storage (read, write, list... recipes).
    /// </summary>
    internal class RecipeLibrarian
    {
        internal static IEnumerable<RecipeModel> GetRecipes(string path)
        {
            foreach (var file in Directory.GetFiles(path, "OCRRec*.Ini"))
            {
                if (!TryGetIndexFromFileName(file, out int index))
                {
                    continue;
                }

                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddIniFile(file);

                // Run through all the providers and build up the sources.
                // This will actually read the INI files and parse them.
                var configRoot = configBuilder.Build();

                // Initialize needed recipe data
                var name     = string.Empty;
                var isFrame  = false;
                var isStored = false;
                var angle = 0;

                // Grab the OCR Recipe section so we can look at it.
                var ocrRecipeSection = configRoot.GetSection("OCR Recipe");
                foreach (var child in ocrRecipeSection.GetChildren())
                {
                    switch (child.Key)
                    {
                        case "Name":
                            name = child.Value;
                            break;

                        case "Frame":
                            if (int.TryParse(child.Value, out int frameParsed))
                            {
                                isFrame = frameParsed > 0;
                            }

                            break;

                        case "Stored":
                            if (int.TryParse(child.Value, out int storedParsed))
                            {
                                isStored = storedParsed > 0;
                            }

                            break;

                        case "Angle":
                            if (int.TryParse(child.Value, out int angleParsed))
                            {
                                angle = angleParsed;
                            }

                            break;
                    }
                }

                yield return new RecipeModel(index + 1, name, isFrame, isStored, angle);
            }
        }

        private static bool TryGetIndexFromFileName(string fileName, out int index)
        {
            // Initialize to incorrect value (default in case of failure)
            index = -1;

            // Check that number can be extracted with regex
            const string pattern = @".*OCRRec(\d{2})[.]Ini";
            if (!Regex.IsMatch(fileName, pattern))
            {
                return false;
            }

            // Extract number string and check it can be parsed into integer
            var numberString = Regex.Match(fileName, pattern).Groups[1].Value;
            if (!int.TryParse(numberString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numberParsed))
            {
                return false;
            }

            // Success
            index = numberParsed;
            return true;
        }
    }
}
