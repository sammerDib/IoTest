using System;
using System.IO;
using System.Linq;

using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.Composer;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class RecipeAcquisitionTemplateComposer : IRecipeAcquisitionTemplateComposer
    {
        private readonly PMConfiguration _pmConfiguration;
        private readonly string _toolName;

        public RecipeAcquisitionTemplateComposer(PMConfiguration pmConfiguration, DbToolServiceProxy dbToolServiceProxy)
        {
            _pmConfiguration = pmConfiguration;
            if (dbToolServiceProxy != null)
                _toolName = dbToolServiceProxy.GetChamber(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey).Tool.Name;
        }

        public string GetVignetteImageFileName(RecipeAdapter recipe, AcquisitionSettings acquisition, (int, int) positionInGrid)
        {
            var composer = new TemplateComposer(_pmConfiguration.OutputAcqFileNameTemplate, EmeraVignetteResultPathParams.Empty);
            var fileNameParams = GetParamsForVignetteImageNames(recipe, acquisition, positionInGrid);
            return composer.ComposeWith(fileNameParams);
        }

        public string GetFullImageFileName(RecipeAdapter recipe, AcquisitionSettings acquisition)
        {
            string fileExtension = Path.GetExtension(_pmConfiguration.OutputAcqFileNameTemplate);
            string pmConfigurationOutputAcqFileNameTemplate = _pmConfiguration.OutputAcqFileNameTemplate.Split(new []{"_C"}, StringSplitOptions.None)[0];
            
            var composer = new TemplateComposer(pmConfigurationOutputAcqFileNameTemplate + fileExtension, EmeraFullImageResultPathParams.Empty);
            var fileNameParams = GetParamsForFullImageNames(recipe, acquisition);
            return composer.ComposeWith(fileNameParams);
        }

        public string GetImageBaseName(RecipeAdapter recipe, AcquisitionSettings acquisition)
        {
            string fileName = GetVignetteImageFileName(recipe, acquisition, (0, 0));
            return fileName.Split(new string[]{"_C"}, StringSplitOptions.None)[0];
        }

        public string GetImageDirectory(RecipeAdapter recipe, AcquisitionSettings acquisition)
        {
            var composer = new TemplateComposer(_pmConfiguration.OutputAcqPathTemplate, EmeraVignetteResultPathParams.Empty);
            var fileNameParams = GetParamsForVignetteImageNames(recipe, acquisition, (0, 0));
            string path = composer.ComposeWith(fileNameParams);
            return Path.Combine(_pmConfiguration.OutputAcqServer, path);
        }

        public string GetAdaFileName(RecipeAdapter recipe)
        {
            var fileNameParams = GetParamsForAda(recipe, (0, 0));

            var pathComposer = new TemplateComposer(_pmConfiguration.OutputAdaFolder, EmeraVignetteResultPathParams.Empty);            
            string path = string.IsNullOrEmpty(recipe.CustomSavePath)
                ? GetImageDirectory(recipe, recipe.Acquisitions.First())
                : recipe.CustomSavePath;


            var fileComposer = new TemplateComposer(_pmConfiguration.OutputAdaFileNameTemplate, EmeraVignetteResultPathParams.Empty);
            string file = fileComposer.ComposeWith(fileNameParams);
            
            return Path.Combine(path, file);
        }

        private EmeraVignetteResultPathParams GetParamsForVignetteImageNames(RecipeAdapter recipe, AcquisitionSettings acquisition,
            (int, int) position)
            => new EmeraVignetteResultPathParams
            {
                ActorType = recipe.ActorType,
                Tool = _toolName,
                WaferCategory = recipe.WaferCategory,
                Recipe = recipe.Name,
                Product = recipe.Product,
                Step = recipe.Step,
                Filter = acquisition.Filter.Type.ToString(),
                Light = acquisition.Light.DeviceID,
                Column = position.Item1,
                Line = position.Item2,
                StartProcessDate = recipe.RecipeExecutionDateTime
            };
        
        private EmeraFullImageResultPathParams GetParamsForFullImageNames(RecipeAdapter recipe, AcquisitionSettings acquisition)
            => new EmeraFullImageResultPathParams
            {
                ActorType = recipe.ActorType,
                Tool = _toolName,
                WaferCategory = recipe.WaferCategory,
                Recipe = recipe.Name,
                Product = recipe.Product,
                Step = recipe.Step,
                Filter = acquisition.Filter.Type.ToString(),
                Light = acquisition.Light.DeviceID,
                StartProcessDate = recipe.RecipeExecutionDateTime
            };

        private EmeraVignetteResultPathParams GetParamsForAda(RecipeAdapter recipe, (int, int) position)
            => new EmeraVignetteResultPathParams()
            {
                ActorType = recipe.ActorType,
                Tool = _toolName,
                WaferCategory = recipe.WaferCategory,
                Recipe = recipe.Name,
                Product = recipe.Product,
                Step = recipe.Step,
                Column = position.Item1,
                Line = position.Item2,
                StartProcessDate = recipe.RecipeExecutionDateTime
            };
    }
}
