using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.DMTCalTransform;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared.Data.Ada;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.Shared.Proxy;
using UnitySC.PM.DMT.Service.Implementation.Extensions;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    internal abstract class MeasureExecutionBase : IDisposable
    {
        //=================================================================

        #region Variables Membres

        //=================================================================
        /// <summary> Recipe Execution </summary>
        public RecipeExecution Rexec { get; }

        public MeasureBase Measure { get; }

        public DMTRecipe Recipe => Rexec.Recipe;

        protected int ToolId;

        protected int ChamberId;

        protected Guid RecipeKey;

        protected int RecipeVersion;

        protected int ProductId;

        protected PMConfiguration PmConfiguration;
        
        protected MeasureConfigurationBase MeasureConfiguration { get; }

        public AdaWriter AdaWriter => Side == Side.Front ? Rexec.AdaWriterFS : Rexec.AdaWriterBS;

        public object AdaWriterLock => Side == Side.Front ? Rexec.AdaWriterFSLock : Rexec.AdaWriterBSLock;

        public bool IsLastMeasureExecution = false;
        protected Side Side => Measure.Side;
        protected double ActualExposureTimeMs; // Temps d'exposition effectivement utilisé

        protected ILogger Logger;

        protected DMTCameraManager CameraManager;

        protected FringeManager FringeManager = ClassLocator.Default.GetInstance<FringeManager>();
        protected DMTHardwareManager HardwareManager = ClassLocator.Default.GetInstance<DMTHardwareManager>();
        protected AlgorithmManager AlgorithmManager = ClassLocator.Default.GetInstance<AlgorithmManager>();
        protected CalibrationManager CalibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
        protected readonly DbRegisterAcquisitionServiceProxy DbRegisterAcqService;
        protected bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;

        protected Dictionary<DMTResult, DBPreRegisterAcquisitionResultFlow> PreRegisterFlowByResultType;

        protected DMTResult DmtResultType;

        #endregion Variables Membres

        //=================================================================

        #region Fonctions virtuelles ou abstraites

        //=================================================================
        public virtual void Dispose()
        {
        }

        /// <summary> Nombre total d'étapes dans le test </summary>
        public abstract int ComputeNumberOfAcquisitionSteps();

        public abstract int ComputeNumberOfComputationSteps();

        public virtual async Task ExecuteAsync()
        {
            await Task.Run(() => throw new NotImplementedException());
        }

        #endregion Fonctions virtuelles ou abstraites

        //=================================================================

        #region Fonctions

        //=================================================================

        /// <summary>
        ///     Crée une MeasureExecution du bon type en fonction de la Mesure
        /// </summary>
        public static MeasureExecutionBase Create(MeasureBase measure, ILogger logger, RecipeExecution rexec,
            bool productionMode, int toolId, int chamberId, Guid recipeKey, int recipeVersion, int productId,
            PMConfiguration pmConfiguration, IDMTScreenService screenService, DMTCameraManager cameraManager,
            CalibrationManager calibrationManager, DbRegisterAcquisitionServiceProxy dbRegisterAcqService,
            MeasuresConfiguration measuresConfiguration, FlowsConfiguration flowsConfiguration)
        {
            switch (measure)
            {
                case BrightFieldMeasure bfMeasure:
                    return new BrightFieldMeasureExecution(bfMeasure, logger, rexec, productionMode, toolId, chamberId,
                        recipeKey, recipeVersion, productId, pmConfiguration, cameraManager, dbRegisterAcqService,
                        measuresConfiguration.GetConfiguration<BrightFieldMeasureConfiguration>(),
                        flowsConfiguration.GetConfiguration<AutoExposureConfiguration>());

                case HighAngleDarkFieldMeasure hadMeasure:
                    return new HighAngleDarkFieldMeasureExecution(hadMeasure, logger, rexec, productionMode, toolId,
                        chamberId, recipeKey, recipeVersion, productId, pmConfiguration, screenService, cameraManager,
                        dbRegisterAcqService,
                        measuresConfiguration.GetConfiguration<HighAngleDarkFieldMeasureConfiguration>());

                case BackLightMeasure blMeasure:
                    return new BackLightMeasureExecution(blMeasure, logger, rexec, productionMode, toolId, chamberId,
                        recipeKey, recipeVersion, productId, pmConfiguration, cameraManager, dbRegisterAcqService,
                        measuresConfiguration.GetConfiguration<BackLightMeasureConfiguration>());

                case DeflectometryMeasure dfMeasure:
                    return new DeflectometryMeasureExecution(dfMeasure, logger, rexec, productionMode, toolId,
                        chamberId, recipeKey, recipeVersion, productId, pmConfiguration, cameraManager,
                        calibrationManager, dbRegisterAcqService,
                        measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>());

                default:
                    throw new ApplicationException("Unknown measure type: " + measure.GetType());
            }
        }

        /// <summary>
        ///     NB: Ce construteur est protected car il faut utiliser la méthode Create
        /// </summary>
        protected MeasureExecutionBase(
            MeasureBase measure, ILogger logger, RecipeExecution rexec, int toolId,
            int chamberId, Guid recipeKey, int recipeVersion, int productId, PMConfiguration pmConfiguration,
            DMTCameraManager cameraManager, DbRegisterAcquisitionServiceProxy dbRegisterAcqService, MeasureConfigurationBase measureConfiguration)
        {
            Measure = measure;
            MeasureConfiguration = measureConfiguration;
            Logger = logger;
            Rexec = rexec;
            ToolId = toolId;
            ChamberId = chamberId;
            RecipeKey = recipeKey;
            RecipeVersion = recipeVersion;
            ProductId = productId;
            PmConfiguration = pmConfiguration;
            DbRegisterAcqService = dbRegisterAcqService;
            CameraManager = cameraManager;
            PreRegisterFlowByResultType = new Dictionary<DMTResult, DBPreRegisterAcquisitionResultFlow>(1);
        }

        
        protected virtual void ReportProgress(string message)
        {
            var status = new RecipeStatus { Message = message };
            Rexec.ReportProgress(status);
        }

        public long PreRegisterFirstAcquisitionResult()
        {
            long previousInternalDbResid = -1;
            foreach (var preRegisterFlow in PreRegisterFlowByResultType.Values)
            {
                if (previousInternalDbResid == -1)
                {
                    preRegisterFlow.Input.Recipe = Recipe;
                    preRegisterFlow.Input.ToolKey = Rexec.PmChamber.Tool.ToolKey;
                    preRegisterFlow.Input.ChamberKey = Rexec.PmChamber.ChamberKey;
                    ExecutePreRegisterFlow(preRegisterFlow);
                    previousInternalDbResid = preRegisterFlow.Result.OutPreRegister.InternalDBResId;
                }
                else
                {
                    ExecutePreRegisterFlowWithPreviousInternalDbResId(preRegisterFlow, previousInternalDbResid);
                }
            }

            return previousInternalDbResid;
        }

        public void PreRegisterAcquisitionResultsWithParent(long previousInternalDbResid)
        {
            foreach (var preRegisterFlow in PreRegisterFlowByResultType.Values)
            {
                ExecutePreRegisterFlowWithPreviousInternalDbResId(preRegisterFlow, previousInternalDbResid);
            }
        }
        
        private static void ExecutePreRegisterFlowWithPreviousInternalDbResId(
            DBPreRegisterAcquisitionResultFlow preRegisterFlow, long previousInternalDbResid)
        {
            preRegisterFlow.Input.PreviousInternalDbResId = previousInternalDbResid;
            ExecutePreRegisterFlow(preRegisterFlow);
        }

        private static void ExecutePreRegisterFlow(DBPreRegisterAcquisitionResultFlow preRegisterFlow)
        {
            preRegisterFlow.Execute();
            if (preRegisterFlow.Result.Status.State != FlowState.Success)
            {
                throw new Exception($"PreRegistration failed for {preRegisterFlow.Input.DmtResultType}");
            }
        }

        protected void AddPreRegisterFlowForResult(DMTResult dmtResult, string acquisitionLabel, string imageLabel = null)
        {
            var preRegisterInput = new DBPreRegisterAcquisitionResultInput
            {
                AcquisitionPath = Rexec.OutputPath,
                AutomationInfo = Rexec.RemoteProductionInfo,
                DmtResultType = dmtResult,
                FileName = Rexec.GetOutputFileName(Measure.Side, (imageLabel ?? Measure.MeasureName.ToLower())),
                AcquisitionLabel = acquisitionLabel
            };
            PreRegisterFlowByResultType.Add(dmtResult, new DBPreRegisterAcquisitionResultFlow(preRegisterInput, DbRegisterAcqService));
        }
        
        #endregion Fonctions

        //=================================================================

        #region Sauvegarde des images

        //================================================================

      //  [Obsolete] ???
        public PathString GetFullPathName(string imageLabel, Side side)
        {
            return Rexec.OutputPath / Rexec.GetOutputFileName(side, imageLabel);
        }

        #endregion Sauvegarde des images
    }
}
