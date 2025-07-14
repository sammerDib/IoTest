using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Proxy;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

using UnitySCSharedAlgosOpenCVWrapper;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.Shared.Proxy;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;

namespace UnitySC.PM.DMT.Service.Implementation.Execution.Measure
{
    /// <summary>
    ///     Classe abstraite pour les measurs qui font de l'auto-exposition.
    ///     Elle contient la gestion de l'auto-exposition:
    ///     - en mode édition de recette (fontion TestAutoExposure) ou lors de l'exécution d'une mesure (fonction
    ///     ApplyExposureTimeForMeasure),
    ///     - le choix de l'exposition manuelle ou automatique,
    ///     - l'algorithme d'auto-exposition à proprement parler.
    /// </summary>
    internal abstract class MeasureExecutionWithAutoExposure : MeasureExecutionBase
    {
        //=================================================================

        #region Fonctions Abstraites

        //=================================================================
        protected abstract void SetScreenForAutoExposure();

        #endregion Fonctions Abstraites

        /// <summary>
        ///     Clé pour le cache des temps d'exposition
        /// </summary>
        protected struct Key
        {
            public string MeasureName;
            public Side Side;

            public override string ToString()
            {
                return MeasureName + "/" + Enum.GetName(typeof(Side), Side);
            }
        }

        //=================================================================

        #region Variables

        //=================================================================
        /// <summary>
        ///     Le cache des temps d'expositions déjà mesurés
        /// </summary>
        protected static readonly Dictionary<Key, double> s_cache = new Dictionary<Key, double>();

        /// <summary> Temps d'exposition sans la correction écran, résultat de l'auto-exposition ou valeur fixe selon le cas </summary>
        public double CalibratedExposureTime;

        private const int NB_LOOPS = 10;

        //private AutoExposureSettings _configs;
        private readonly USPImageMil _maskImage;

        private CameraBase _camera;
        private ScreenBase _screen;

        #endregion Variables

        //=================================================================

        #region Fonctions

        //=================================================================

        public MeasureExecutionWithAutoExposure(
            MeasureBase measure, ILogger logger, RecipeExecution rexec, int toolId,
            int chamberId, Guid recipeKey, int recipeVersion, int productId, PMConfiguration pmConfiguration,
            DMTCameraManager cameraManager, DbRegisterAcquisitionServiceProxy dbRegisterAcqService, MeasureConfigurationBase measureConfiguration) :
            base(measure, logger, rexec, toolId, chamberId, recipeKey, recipeVersion, productId, pmConfiguration,
                 cameraManager, dbRegisterAcqService, measureConfiguration)
        {
        }

        public override int ComputeNumberOfAcquisitionSteps()
        {
            if (Measure.AutoExposureTimeTrigger == AutoExposureTimeTrigger.Never)
                return 0;
            return AutoExposureFlow.MaximumNumberOfSteps;
        }

        public int ComputeNumberOfStepsForAutoExposure()
        {
            return NB_LOOPS;
        }

        /// <summary>
        ///     Reset la liste des ExposureTime mesurées
        /// </summary>
        public static void ResetCache()
        {
            s_cache.Clear();
        }

        /// <summary>
        ///     Exécute l'auto-(ou manuelle-)exposition en mode exécution de la recette
        /// </summary>
        protected void ApplyExposureTimeForMeasure(bool isFlowReportingNeeded = true)
        {
            _camera = HardwareManager.CamerasBySide[Measure.Side];
            _screen = HardwareManager.ScreensBySide[Measure.Side];

            var key = new Key { MeasureName = Measure.MeasureName, Side = Side };

            switch (Measure.AutoExposureTimeTrigger)
            {
                case AutoExposureTimeTrigger.Never:
                    CalibratedExposureTime = Measure.ExposureTimeMs;
                    SetFixedExposureTime(Measure.ExposureTimeMs);
                    break;

                case AutoExposureTimeTrigger.OnFirstWaferOfLot:
                case AutoExposureTimeTrigger.OnAllWafer:
                    bool found = s_cache.TryGetValue(key, out CalibratedExposureTime);
                    if (found)
                        SetFixedExposureTime(CalibratedExposureTime);
                    else
                    {
                        USPImageMil displayImage = null;
                        switch (Measure)
                        {
                            case DeflectometryMeasure dfMeasure:
                                var fringesDict = FringeManager.GetFringeImageDict(dfMeasure.Side, dfMeasure.Fringe);
                                int largestPeriod = dfMeasure.Fringe.Periods.Max();
                                displayImage = fringesDict[FringesDisplacement.X][largestPeriod][0];
                                break;

                            case HighAngleDarkFieldMeasure highAngleDarkFieldMeasure:
                                displayImage = CalibrationManager.GetHighAngleDarkFieldMaskForSide(highAngleDarkFieldMeasure.Side);
                                break;
                        }

                        var autoExposureInput = new AutoExposureInput(Measure, displayImage);
                        var cameraManger = ClassLocator.Default.GetInstance<IDMTInternalCameraMethods>();
                        var autoExposureFlow = FlowsAreSimulated ? new AutoExposureFlowDummy(autoExposureInput, HardwareManager, cameraManger)
                                                                 : new AutoExposureFlow(autoExposureInput, HardwareManager, cameraManger);

                        if (isFlowReportingNeeded)
                        {
                            autoExposureFlow.StatusChanged += AutoExposureFlowOnStatusChanged;
                        }

                        var result = autoExposureFlow.Execute();
                        bool success = result.Status.State == FlowState.Success;
                        CalibratedExposureTime = result.ExposureTimeMs;
                        if (result.Status.State == FlowState.Error)
                        {
                            if (isFlowReportingNeeded)
                            {
                                autoExposureFlow.StatusChanged -= AutoExposureFlowOnStatusChanged;
                            }

                            throw new Exception(
                                $"AutoExposure failed. Last known status message: {result.Status.Message}");
                        }

                        if (success && Measure.AutoExposureTimeTrigger == AutoExposureTimeTrigger.OnFirstWaferOfLot)
                        {
                            s_cache[key] = CalibratedExposureTime;
                            SetFixedExposureTime(CalibratedExposureTime);
                        }

                        if (isFlowReportingNeeded)
                        {
                            autoExposureFlow.StatusChanged -= AutoExposureFlowOnStatusChanged;
                        }
                    }

                    break;

                default:
                    throw new ApplicationException(
                        "Unknown AutoExposureTimeTrigger: " + Measure.AutoExposureTimeTrigger);
            }
        }

        private void AutoExposureFlowOnStatusChanged(FlowStatus status, AutoExposureResult statusdata)
        {
            var statusReport = new AutoExposureStatus();
            statusReport.Side = Measure.Side;
            statusReport.ExposureTimeMs = statusdata.ExposureTimeMs;
            statusReport.CurrentStep = statusdata.CurrentStep;
            statusReport.TotalSteps = statusdata.TotalSteps;
            statusReport.Message = status.Message;
            switch (status.State)
            {
                case FlowState.InProgress:
                    statusReport.State =
                        DMTRecipeState
                            .Executing; // TODO Must check if it is Acquiring or AcquiringAndComputing
                    break;

                case FlowState.Canceled:
                    statusReport.State = DMTRecipeState.Aborted;
                    break;

                case FlowState.Error:
                    statusReport.State = DMTRecipeState.Failed;
                    break;

                case FlowState.Success:
                    statusReport.State = DMTRecipeState.Executing;
                    break;
            }

            Rexec.ReportProgress(statusReport);
        }

        /// <summary>
        ///     Set le temps d'exposition avec les corrections nécessaires
        /// </summary>
        protected virtual void SetFixedExposureTime(double expo)
        {
            ActualExposureTimeMs = CameraManager.SetExposureTime(_camera, expo);
            Logger.Information($"calibrated={CalibratedExposureTime:0.000}ms actual={ActualExposureTimeMs:0.000}ms");
        }

        /// <summary>
        ///     Reporte un message d'avancement
        /// </summary>
        protected override void ReportProgress(string message)
        {
            var status = new AutoExposureStatus { Message = message, ExposureTimeMs = CalibratedExposureTime };
            Rexec.ReportProgress(status);
        }

        #endregion Fonctions
    }
}
