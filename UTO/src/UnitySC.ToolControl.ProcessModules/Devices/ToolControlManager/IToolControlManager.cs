using System;
using System.Collections.Generic;
using System.IO;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager
{
    [Device]
    public interface IToolControlManager : IAbstractDataFlowManager
    {
        #region Methods

        void SendDataSetAck(TableDataResponse response);

        void DataSetRequest(TableDataRequest request);

        RecipeDownloadResult DownloadRecipe(string recipeName);

        RecipeUploadResult UploadRecipe(string recipeName, Stream recipe);

        DeleteRecipeResult DeleteRecipe(IEnumerable<string> recipeNames);

        #endregion

        #region Events

        event EventHandler<S13F13EventArgs> S13F13Raised;

        event EventHandler<S13F16EventArgs> S13F16Raised;

        event EventHandler<CollectionEventEventArgs> ToolControlCollectionEventRaised;

        #endregion
    }
}
