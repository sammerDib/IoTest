using System;
using System.IO;

using UnitySC.Shared.Tools;

namespace UnitySC.PP.ADC.Service.Interface
{
    /// <summary>
    ///  Définition du fichier XML de configuration des process modules
    /// </summary>
    [Serializable]
    public class PPConfigurationADC
    {
        static public PPConfigurationADC Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("PPConfiguration file is missing");
            var PPConfig = XML.Deserialize<PPConfigurationADC>(path);
            return PPConfig;
        }

        //public ActorType Actor;

        public bool UseMatroxImagingLibrary;
        public int ChamberId;

        public string PathUIResourcesXml; // return @"C; //\Users\n.chaux\source\repos\UnityControl\PP\ADC\Output\Debug";
        public string PathModuleDll; // return @"C; //\Users\n.chaux\source\repos\UnityControl\PP\ADC\Output\Debug";

        // AppConfig
        public string Editor_RecipeFolder; // return @"C; //\Altasight\Data\Recipes";
        public string Editor_MetablockFolder; // return @"C; //\Altasight\Data\Metablocks";
        public string Editor_StartupMode; // return @"ExpertRecipeEdition";   // <!--ExpertRecipeEdition, SimplifiedRecipeEdition -->
        public string Editor_HelpFolder; // return @"C; //\Unitysc\ADCV9\Help";
        public string Editor_HideAdaToAdcWindowsServiceStatus; // return @"false";
        public string DatabaseConfig_UseExportedDatabase; // return @"false";
        public string DatabaseConfig_ExportedDatabaseFile; // return @"_\DataBase_adcdb";
        public string DatabaseConfig_UseDatabaseToGetRecipes; // return @"true";
        public string DatabaseConfig_RecipeCache; // return @"RecipeDataBaseCache";  //  <!-- Cache pour les recettes et fichiers associés-->
        public string DatabaseConfig_AdditionnalRecipeFiles_ServerDirectory; // return @"__\__\__\SqlRecipeFile" ;  //  <!-- Emplacement des fichier liées aux recettes dans la base de données-->
        public string DatabaseResults_ServerName; // return @"(local)";  //  <!--or IP = "172_123_123_123, or ServerName = "FMO-UNTITY-23"  or if in local computer = "(local)-->
        public string DatabaseResults_Use; // return @"True";   // <!--false => use Recipe_OutputDir as destination directory -->
        public string AdcEngine_ProductionMode; // return @"InAcquisition";  //  <!--InADCv9|InAcquisition-->
        public string AdcEngine_NbTasksPerPool; // return @"1";
        public string Debug_ImageViewer; // return @"C; //\Program Files\ImageJ\ImageJ_exe" ;
        public string AdaFolder; // return @"C; //\Altasight\Data\Ada";
        public string AdaToAdc_TestMode_PreloadImages; // return @"False";
        public string AdaToAdc_TestMode_AlwaysSendTheSameImage; // return @"";
        public string AdaToAdc_TransferToRobot_Enable; // return @"False";
        public string AdaToAdc_TransferToRobot_Embedded; // return @"True";
        public string Grading_Path; // return @"\\172_20_33_2\CIMConnectProjects\Equipment1\ADC\Sorting\" ;





    }
}

