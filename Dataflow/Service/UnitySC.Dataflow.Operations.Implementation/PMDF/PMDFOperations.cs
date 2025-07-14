using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface.UTOOperations;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Serialize;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Operations.Implementation
{
    public class PMDFOperations : IPMDFOperations
    {
        private ILogger _logger;       
        private IDFManager _dfManager;
        private ICommonEventOperations _commonEventService;

        public PMDFOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<PMDFOperations>>();
        }
        public void Init()
        {            
            _dfManager = ClassLocator.Default.GetInstance<IDFManager>();
            _commonEventService = ClassLocator.Default.GetInstance<ICommonEventOperations>();
        }
        public bool CanStartPMRecipe(Identity identity, Material wafer)
        {
            return _dfManager.CanStartPMRecipe(identity, wafer);
        }
        public void RecipeExecutionComplete(Identity identity, Material wafer, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            _dfManager.RecipeExecutionComplete(identity, wafer, recipeKey, results, status);
        }

        public void RecipeStarted(Identity identity, Material wafer)
        {
            _dfManager.RecipeStarted(identity, wafer); 
        }        
        public DataflowRecipeInfo GetDataflowRecipeInfo(Identity identity, Material wafer)
        {
            return _dfManager.GetDataflowRecipeInfo(identity, wafer.ProcessJobID, wafer.GUIDWafer);
        }
        public Guid? RetrieveTheAssociatedPMRecipeKey(Identity identity, Material wafer)
        {
            return _dfManager.RetrieveTheAssociatedPMRecipeKey(identity, wafer);
        }

        public DataflowActorRecipe GetADCRecipeForSide(Identity identity, Material wafer, Side waferSide)
        {
            return _dfManager.GetPPRecipeByMaterialAndBySide(identity, wafer, waferSide);
        }

        public void NotifyDataCollectionChanged(Identity identity, ModuleDataCollection pmDataCollection)
        {
            if (pmDataCollection != null)
            {                
                UpdateDatacollectionLog(pmDataCollection);
                var dcConvert = ClassLocator.Default.GetInstance<IDataCollectionConvert>();
                SecsVariableList secsVariableList = dcConvert.ConvertToSecsVariableList(pmDataCollection);
                if (secsVariableList != null)
                {
                    _commonEventService.Fire_CE(CEName.WaferMeasurementResults, secsVariableList);
                }
                else
                {
                    _logger.Error("Failed to convert a data collection into a SecsVariableList");
                }                
            }
            else
                _logger.Error("PMDataCollection parameter is null in NotifyDataCollectionChanged method");
        }
        public void UpdateDatacollectionLog(ModuleDataCollection pmDataCollection)
        {
            var dfConfiguration = ClassLocator.Default.GetInstance<DFServerConfiguration>();  
            string dcTypeIdentified = String.Empty;
            try
            {                
                switch (pmDataCollection)
                {
                    case ANADataCollection anaDc:
                        dcTypeIdentified = "ANADataCollection";
                        SerializationTools.Serialize_DataContractObject(dfConfiguration.LogDataCollectionPathFile, anaDc);
                        break;
                    case DMTDataCollection dmtDc:
                        dcTypeIdentified = "DMTDataCollection";
                        SerializationTools.Serialize_DataContractObject(dfConfiguration.LogDataCollectionPathFile, dmtDc);
                        break;
                    default:
                        dcTypeIdentified = "ModuleDataCollection";
                        SerializationTools.Serialize_DataContractObject(dfConfiguration.LogDataCollectionPathFile, pmDataCollection);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Log of {dcTypeIdentified} Datacollection failed. Exception = {ex.Message} {ex.StackTrace}");
            }            
        }
    }
}
