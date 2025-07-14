using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.PM.EME.Service.Core.Recipe.AcquisitionPath;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class RecipeSaving
    {
        private readonly ILogger<RecipeOrchestrator> _logger;
        private readonly IImageFileSaver _fileSaver;
        private readonly IAdaFileSaver _adaFileSaver;
        private readonly IRecipeAcquisitionTemplateComposer _composer;
        private readonly DbRegisterAcquisitionServiceProxy _dbRegisterAcquisitionServiceProxy;
        private readonly int _toolKey;
        private readonly int _chamberKey;

        public RecipeSaving(ILogger<RecipeOrchestrator> logger, IImageFileSaver fileSaver, IAdaFileSaver adaFileSaver,
            IRecipeAcquisitionTemplateComposer composer, PMConfiguration pmConfiguration,
            DbRegisterAcquisitionServiceProxy dbRegisterAcquisitionServiceProxy)
        {
            _logger = logger;
            _fileSaver = fileSaver;
            _adaFileSaver = adaFileSaver;
            _composer = composer;
            _dbRegisterAcquisitionServiceProxy = dbRegisterAcquisitionServiceProxy;
            _toolKey = pmConfiguration.ToolKey;
            _chamberKey = pmConfiguration.ChamberKey;
        }

        public void SaveInDatabase(RecipeAdapter recipe, List<IAcquisitionImageResult> results)
        {
            Parallel.ForEach(results, result =>
            {
                try
                {
                    SaveResultInDataBase(recipe, result);
                }
                catch (Exception ex)
                {
                    string label = result.AcquisitionLabel;
                    _logger.Error(ex,
                        $"Failed to save acquisition result for label '{label}', ToolKey: {_toolKey}', ChamberKey: {_chamberKey}");
                }
            });
        }

        private void SaveResultInDataBase(RecipeAdapter recipe, IAcquisitionImageResult result)
        {
            var remoteProductionInfo = recipe.RemoteProductionInfo ?? new RemoteProductionInfo();
            _dbRegisterAcquisitionServiceProxy.PreRegisterAcquisition(_toolKey, _chamberKey, recipe.Info,
                remoteProductionInfo, result.BaseName, result.FolderName, result.ResultType, 0, result.AcquisitionLabel,
                ResultFilterTag.Engineering);
        }

        public string SaveImage(RecipeAdapter recipe, AcquisitionSettings acquisition, ServiceImage image,
            AcquisitionPosition position = null)
        {
            return _fileSaver.Save(recipe, acquisition, image, position?.PositionInGrid);
        }

        public ImageFolderAndBaseName ImageFolderAndBaseName(RecipeAdapter recipe, AcquisitionSettings acquisition)
        {
            return _fileSaver.GetFolderAndBaseName(recipe, acquisition);
        }

        public void DeletePreviousImages(RecipeAdapter recipe)
        {
            if (!string.IsNullOrEmpty(recipe.CustomSavePath))
            {
                return;
            }

            string directory = _composer.GetImageDirectory(recipe, new AcquisitionSettings());
            if (string.IsNullOrEmpty(directory))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(directory);
            if (!directoryInfo.Exists)
            {
                return;
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                    _logger.Warning($"File not deleted: {file.FullName}");
                }
            }
        }

        public void SaveAdaFile(RecipeAdapter recipe, List<IAcquisitionImageResult> acquisitionResults)
        {
            _logger.Information("Generating ada file");
            _adaFileSaver.GenerateFile(recipe, acquisitionResults);
        }
    }
}
