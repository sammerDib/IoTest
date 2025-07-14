using System;
using System.Xml;

using CommunityToolkit.Mvvm.DependencyInjection;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Delegates;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Shared
{
    public static class ANARecipeHelper
    {

        public static void UpdateRecipeWithExternalFiles(ANARecipe recipe, RecipeLoadProgressChanged recipeLoadProgressChangedEvent)
        {
            var dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            var externalFilesWithoutData = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(recipe);
            int nbExternalFiles = externalFilesWithoutData.Count;
            int currentExternalFiles = 0;
            foreach (var externalFileWithOutData in externalFilesWithoutData)
            {
                currentExternalFiles++;
                var externalFile = dbRecipeService.Invoke(x => x.GetExternalFile(string.Concat(externalFileWithOutData.Key, externalFileWithOutData.Value.FileExtension), recipe.Key));
                externalFileWithOutData.Value.UpdateWith(externalFile);
                recipeLoadProgressChangedEvent?.Invoke($"Load extenal file {currentExternalFiles}/{nbExternalFiles}");
            }
        }

        public static string ConvertAnaRecipeIfNeeded(string anaRecipeString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(anaRecipeString);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("ANARecipe");
            foreach (XmlNode node in nodeList)
            {
                string fileVersionString = node.Attributes["FileVersion"].Value;
                Version fileVersion = new Version(fileVersionString);

                if (fileVersion <= new Version("1.0.1"))
                {
                    // Update the Strategy to "PerMeasurementType"
                    XmlNode strategyNode = node.SelectSingleNode("Execution/Strategy");
                    if (strategyNode != null)
                    {
                        strategyNode.InnerText = "PerMeasurementType";
                    }
                }
            }

            return xmlDoc.InnerXml;
        }

        //Code d'adaptation du integration time en intensity factor
        /*

            XmlNodeList probeNodes = xmlDoc.SelectNodes("//Probe[ProbeId='ProbeLiseHF' and IntegrationTimems]");
            foreach (XmlNode probeNode in probeNodes)
            {
                XmlNode integrationTimeNode = probeNode.SelectSingleNode("IntegrationTimems");
                if (integrationTimeNode != null && !string.IsNullOrWhiteSpace(integrationTimeNode.InnerText))
                {
                    //TODO We have to calculate Intensity Factor via measure intégration timed divide by calib persistant calibration time
                    // using the probeID and lowIllumination filter and ask to calibManager class
                    XmlNode intensityFactorNode = probeNode.SelectSingleNode("IntensityFactor");
                    if (intensityFactorNode == null)
                    {
                        intensityFactorNode = xmlDoc.CreateElement("IntensityFactor");
                        probeNode.AppendChild(intensityFactorNode);
                    }
                    intensityFactorNode.InnerText = "TODO Calculated Value";
                    probeNode.RemoveChild(integrationTimeNode);
                }
            }
        */

    }
}
