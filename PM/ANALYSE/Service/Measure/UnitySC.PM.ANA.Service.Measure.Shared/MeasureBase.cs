using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.AutofocusTracker;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public abstract class MeasureBase<TMeasureSettings, TMeasureResult> : IMeasure
        where TMeasureSettings : MeasureSettingsBase
        where TMeasureResult : MeasurePointResult, new()
    {
        protected readonly AnaHardwareManager HardwareManager;
      
        protected readonly MeasuresConfiguration MeasuresConfiguration;

        protected readonly MeasureAutofocusTracker MeasureAutofocusTracker;

        protected readonly ILogger Logger;

        protected const string FormatSaveDecimal = "0.000";

        private PMConfiguration _pmConfiguration;

        public virtual bool WaferUnclampedMeasure { get; } = false;

        protected abstract MeasurePointDataResultBase Process(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken);

        protected virtual MeasurePointDataResultBase SubProcess(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            return null;
        }

        protected virtual MeasurePointDataResultBase ComputeMeasureFromSubMeasures(TMeasureSettings measureSettings, MeasureContext measureContext, List<MeasurePointResult> subResults, CancellationToken cancelToken)
        {
            return null;
        }

        protected abstract MeasureToolsBase GetMeasureToolsInternal(TMeasureSettings measureSettings);

        protected abstract MeasurePointDataResultBase CreateNotMeasuredPointData(TMeasureSettings measureSettings, Exception exception);

        public MeasurePointResult CreateNotMeasuredEmptyResult(string message) 
        {
            var ptres = new TMeasureResult() { State = MeasureState.NotMeasured, Message = message};
            ptres.Datas.Add(CreateNotMeasuredPointData(null, new Exception(message)));
            return ptres;
        }

        public abstract MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings);

        public virtual MeasureSettingsBase UpdateSetting(MeasureSettingsBase measureSettings)
        { 
            return measureSettings; 
        }

        public virtual MeasureDieResult CreateMetroDieResult()
        {
            return new MeasureDieResult();
        }

        public virtual void MeasureTerminatedInRecipe(MeasureSettingsBase measureSettingsBase)
        {
        }

        public virtual bool CanZAxisMove(MeasureSettingsBase measureSettingsBase)
        {
            return true;
        }

        public virtual void ApplyMeasureCorrection(MeasureResultBase measureResult, MeasureSettingsBase measureSettingsBase)
        {
            // By default nothing to do specifically.
        }

        public virtual void FinalizeMetroResult(MeasureResultBase measureResultBase, MeasureSettingsBase measureSettingsBase)
        {
            measureResultBase.FillMeasureDiesStateFromData();
        }

        protected virtual void FinalizeSpecificResult(TMeasureResult measureResult)
        {
            // By default nothing to do specifically.
        }
        public virtual bool PrepareExecution(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default)
        {
            // by default preparation succeeded
            return true;
        }

        public abstract MeasureType MeasureType { get; }

        public event MeasureProgressChangedEventHandler MeasureProgressChangedEvent;

        public event MeasureDataChangedEventHandler MeasureDataChangedEvent;

        public event MeasureProgressChangedEventHandler StaticMeasureProgressChangedEvent;

        public event MeasureDataChangedEventHandler StaticMeasureDataChangedEvent;

        public MeasureBase(ILogger logger)
        {
            Logger = logger;
            HardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            MeasuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();
            MeasureAutofocusTracker = ClassLocator.Default.GetInstance<MeasureAutofocusTracker>();
            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
        }

        private TMeasureSettings ConvertMeasureSettings(MeasureSettingsBase measureSettings)
        {
            var converted = measureSettings as TMeasureSettings;
            if (converted == null)
                throw new InvalidCastException($"{MeasureType} Measure : Bad measure settings");
            return converted;
        }

        public MeasureToolsBase GetMeasureTools(MeasureSettingsBase measureSettings)
        {
            var convertedSettings = ConvertMeasureSettings(measureSettings);
            return GetMeasureToolsInternal(convertedSettings);
        }

        public MeasurePointResult Execute(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default, bool useStaticRepeta = false)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name);
            }

            try
            {
                var convertedSettings = ConvertMeasureSettings(measureSettings);
                var datas = new List<MeasurePointDataResultBase>();

                try
                {
                    cancelToken.ThrowIfCancellationRequested();
                    if (!useStaticRepeta)
                    {
                        ProcessCommonSettings(convertedSettings, measureContext, cancelToken);
                    }

                    var data = ProcessMeasure(convertedSettings, measureContext, cancelToken);
                    if (measureSettings.PostProcessingSettingsIsEnabled)
                    {
                        data = ExecutePostProcessing(convertedSettings, measureContext, data);
                    }

                    NotifyMeasureDataChanged(data, measureContext);
                    datas.Add(data);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var data = CreateNotMeasuredPointData(convertedSettings, ex);
                    datas.Add(data);
                    var repeatIndex = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
                    Logger.Error(ex, $"Error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y} Repeat:{repeatIndex}");
                }

                var result = CreateResult(measureContext.MeasurePoint, convertedSettings, datas);
                return result;
            }
            catch (OperationCanceledException)
            {
                Logger.Information($"Measure cancelled {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Global error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                };
                return result;
            }
            finally
            {
                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(GetType().Name);
                }
            }
        }

        public MeasurePointResult ExecuteSubMeasure(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default, bool useStaticRepeta = false)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name);
            }

            try
            {
                var convertedSettings = ConvertMeasureSettings(measureSettings);
                var datas = new List<MeasurePointDataResultBase>();

                try
                {
                    cancelToken.ThrowIfCancellationRequested();
                    if (!useStaticRepeta)
                    {
                        ProcessCommonSettings(convertedSettings, measureContext, cancelToken);
                    }

                    var data = ProcessSubMeasure(convertedSettings, measureContext, cancelToken);

                    // TODO Warp : for the moment, PostProcessing doesn't exists for SubMeasure
                    // To be reavaluated when others measures with submeasures will be implemented
                    //if (measureSettings.PostProcessingSettingsIsEnabled)
                    //{
                    //    data = ExecutePostProcessing(convertedSettings, measureContext, data);
                    //}

                    NotifyMeasureDataChanged(data, measureContext);
                    datas.Add(data);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var data = CreateNotMeasuredPointData(convertedSettings, ex);
                    datas.Add(data);
                    var repeatIndex = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
                    Logger.Error(ex, $"Error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y} Repeat:{repeatIndex}");
                }

                var result = CreateResult(measureContext.MeasurePoint, convertedSettings, datas, true);
                return result;
            }
            catch (OperationCanceledException)
            {
                Logger.Information($"Measure cancelled {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                    IsSubMeasurePoint = true
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Global error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                    IsSubMeasurePoint = true
                };
                return result;
            }
            finally
            {
                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(GetType().Name);
                }
            }
        }

        public MeasurePointResult ComputeMeasureFromSubMeasures(MeasureSettingsBase measureSettings, MeasureContext measureContext, List<MeasurePointResult> subResults, CancellationToken cancelToken = default, bool useStaticRepeta = false)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name);
            }

            try
            {
                var convertedSettings = ConvertMeasureSettings(measureSettings);
                var datas = new List<MeasurePointDataResultBase>();

                try
                {
                    cancelToken.ThrowIfCancellationRequested();
                    if (!useStaticRepeta)
                    {
                        ProcessCommonSettings(convertedSettings, measureContext, cancelToken);
                    }

                    var data = ComputeMeasureFromSubMeasures(convertedSettings, measureContext, subResults, cancelToken);
                    if (measureSettings.PostProcessingSettingsIsEnabled)
                    {
                        data = ExecutePostProcessing(convertedSettings, measureContext, data);
                    }

                    NotifyMeasureDataChanged(data, measureContext);
                    datas.Add(data);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var data = CreateNotMeasuredPointData(convertedSettings, ex);
                    datas.Add(data);
                    var repeatIndex = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
                    Logger.Error(ex, $"Error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y} Repeat:{repeatIndex}");
                }

                var result = CreateResult(measureContext.MeasurePoint, convertedSettings, datas);
                return result;
            }
            catch (OperationCanceledException)
            {
                Logger.Information($"Measure cancelled {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                };
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Global error during measure {measureSettings.Name}[{measureSettings.MeasureType}] at point x:{measureContext.MeasurePoint.Position?.X}  y:{measureContext.MeasurePoint.Position.Y}");
                var result = new TMeasureResult()
                {
                    XPosition = measureContext.MeasurePoint.Position.X,
                    YPosition = measureContext.MeasurePoint.Position.Y,
                    State = MeasureState.NotMeasured,
                };
                return result;
            }
            finally
            {
                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(GetType().Name);
                }
            }
        }

        // This function works well if the recipe deal with measurePosition (i.e. if the Alignment parameters are set)
        private void ProcessCommonSettings(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name + "Preparation");
            }

            ReferentialBase referential = ReferentialHelper.CreateDieOrWaferReferential(measureContext.DieIndex);
            var measurePosition = measureContext.MeasurePoint.Position.ToXYZTopZBottomPosition(referential);

            // If measure need Autofocus, try to correct the Z position to reduce (avoid?) scan range
            var afSettings = (measureSettings as IAutoFocusMeasureSettings)?.AutoFocusSettings;
            if (afSettings != null)
            {
                // TODO : GetCorrectedAutofocusPosition use the closest focus position, even if the closest position
                // is really far. Should we research the closest focus position in a restriced area?
                // TODO : Uncomment when we can use quality score to save only the correct autofocus
                //var correctedPosition = MeasureAutofocusTracker.GetCorrectedAutofocusPosition(measurePosition, afSettings);
                //if (correctedPosition != null)
                //    measurePosition = correctedPosition;
            }

            if (measureSettings.MeasureStartAtMeasurePoint)
            {
                HardwareUtils.MoveAxesTo(HardwareManager.Axes, measurePosition);
                cancelToken.ThrowIfCancellationRequested();
            }

            if (afSettings != null)
            {
                // The Autofocus will, according AutoFocusType :
                //  - Run AFLise and its context : Objective
                //  - Run AFCamera and its context : Objective, Lights
                //  - Go to focus position
                measurePosition.ZTop = DoAutoFocus(afSettings, cancelToken);
                cancelToken.ThrowIfCancellationRequested();

                // Save focus position found
                MeasureAutofocusTracker.SaveAutofocusResult(afSettings, measurePosition);
            }

            if (!(measureContext.MeasurePoint.PatternRec is null))
            {
                // The patternRec will :
                // - Apply context : Lights + Objective
                // - Run PatternRec
                PatternRecResult patternRecRes = DoPatternRec(measureContext.MeasurePoint.PatternRec, cancelToken);
                measurePosition.X += patternRecRes.ShiftX.Millimeters;
                measurePosition.Y += patternRecRes.ShiftY.Millimeters;

                cancelToken.ThrowIfCancellationRequested();

                // Apply patternRec shift found
                var patternRecShift = new XYZTopZBottomMove(patternRecRes.ShiftX.Millimeters, patternRecRes.ShiftY.Millimeters, 0, 0);
                HardwareUtils.MoveIncremental(HardwareManager.Axes, patternRecShift);
                cancelToken.ThrowIfCancellationRequested();
            }

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name + "Preparation");
            }
        }

        private MeasurePointDataResultBase ProcessMeasure(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name + "Execution");
            }

            var result = Process(measureSettings, measureContext, cancelToken);

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name + "Execution");
            }
            return result;
        }

        private MeasurePointDataResultBase ProcessSubMeasure(TMeasureSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name + "Execution");
            }

            var result = SubProcess(measureSettings, measureContext, cancelToken);

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name + "Execution");
            }

            return result;
        }

        public TMeasureResult CreateResult(MeasurePoint measurePoint, MeasureSettingsBase measureSettings, List<MeasurePointDataResultBase> datas, bool isSubMeasurePoint = false)
        {
            StringBuilder messages = new StringBuilder();
            if (datas.Count == 1)
            {
                // No repeta case
                if (!string.IsNullOrEmpty(datas[0]?.Message))
                {
                    messages.Append($"{datas[0].Message}");
                }
            }
            else
            {
                // Multiple repeat datas (or None)
                int nRepetaId = 0;
                foreach (var data in datas)
                {
                    if (!string.IsNullOrEmpty(data?.Message))
                    {
                        if (messages.Length != 0)
                        {
                            messages.AppendLine();
                        }
                        messages.Append($"rep {nRepetaId}: {data.Message}");
                    }
                    nRepetaId++;
                }
            }

            var result = new TMeasureResult()
            {
                SiteId = measurePoint.Id,
                XPosition = measurePoint.Position.X,
                YPosition = measurePoint.Position.Y,
                Datas = datas,
                Message = (messages.Length != 0) ? messages.ToString() : null,
                IsSubMeasurePoint = isSubMeasurePoint
            };

            result.ComputeQualityScoreFromDatas();
            result.FillMeasurePointStateFromData(measureSettings);
            FinalizeSpecificResult(result);
            return result;
        }

        protected void NotifyMeasureProgressChanged(MeasurePointProgress measurePoint)
        {
            MeasureProgressChangedEvent?.Invoke(measurePoint);
        }

        protected void NotifyMeasureDataChanged(MeasurePointDataResultBase data, MeasureContext measureContext)
        {
            MeasureDataChangedEvent?.Invoke(data, measureContext);
        }

        private PatternRecResult DoPatternRec(PatternRecognitionDataWithContext patternRec, CancellationToken cancellationToken)
        {
            Logger.Information(MeasureType.ToString() + " Measure : Start Pattern rec");

            PatternRecInput patternRecInput = new PatternRecInput(patternRec);

            // InitialContext will contains Lights + Objective
            patternRecInput.InitialContext = patternRec.Context;

            PatternRecFlow patternRecFlow = FlowsAreSimulated ? new PatternRecFlowDummy(patternRecInput) : new PatternRecFlow(patternRecInput);
            patternRecFlow.CancellationToken = cancellationToken;
            var patternRecResult = patternRecFlow.Execute();

            if (patternRecResult.Status.State == FlowState.Canceled)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (patternRecResult.Status.State != FlowState.Success)
            {
                Logger.Error(MeasureType.ToString() + " Measure : The pattern rec failed");
                throw new Exception(MeasureType.ToString() + " Measure : The pattern rec failed");
            }
            return patternRecResult;
        }

        private double DoAutoFocus(AutoFocusSettings autofocusSettings, CancellationToken cancellationToken)
        {
            var autoFocusInput = new AutofocusInput(autofocusSettings);

            Logger.Information(MeasureType.ToString() + " Measure : Start AutoFocus");
            var autoFocusFlow = FlowsAreSimulated ? new AutofocusFlowDummy(autoFocusInput) : new AutofocusFlow(autoFocusInput);
            autoFocusFlow.CancellationToken = cancellationToken;
            var afResult = autoFocusFlow.Execute();

            if (afResult.Status.State == FlowState.Canceled)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (afResult.Status.State != FlowState.Success)
            {
                Logger.Error(MeasureType.ToString() + " Measure : AutoFocus failed");
                throw new Exception(MeasureType.ToString() + " Measure : AutoFocus failed");
            }

            return afResult.ZPosition;
        }

        protected bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;

        public virtual List<string> GetLightIds()
        {
            return HardwareManager.Lights.Keys.ToList();
        }

        /// <returns>The image file name.</returns>
        protected static string SaveServiceImage(ServiceImage serviceImage, string fileNameSuffixAndExtension, MeasureContext measureContext)
        {
            string imageFileName = string.Empty;

            if (!String.IsNullOrEmpty(measureContext.ResultFoldersPath.ExternalFilePrefix))
                imageFileName = measureContext.ResultFoldersPath.ExternalFilePrefix;

            if (measureContext.DieIndex != null)
                imageFileName += $"C{measureContext.DieIndex.Column}R{measureContext.DieIndex.Row}-";

            string repeatText = string.Empty;
            if (measureContext is MeasureContextRepeat)
                repeatText = $"i{(measureContext as MeasureContextRepeat).RepeatIndex}-";

            imageFileName = $"{imageFileName}Id{measureContext.MeasurePoint.Id}-X{measureContext.MeasurePoint.Position.X.ToString(FormatSaveDecimal)}-Y{measureContext.MeasurePoint.Position.Y.ToString(FormatSaveDecimal)}-{repeatText}{fileNameSuffixAndExtension}";

            string relativeImageFilePath = Path.Combine(measureContext.ResultFoldersPath.ExternalFileFolderName, imageFileName);

            string resultFilePath = Path.Combine(measureContext.ResultFoldersPath.RecipeFolderPath, relativeImageFilePath);
            serviceImage.SaveToFile(resultFilePath);

            return relativeImageFilePath;
        }

        public virtual TimeSpan GetEstimatedTime(MeasureSettingsBase measureSettings)
        {
            return TimeSpan.FromSeconds(5);
        }

        public virtual MeasurePointDataResultBase ExecutePostProcessing(MeasureSettingsBase measureSettings, MeasureContext measureContext, MeasurePointDataResultBase result)
        {
            return result;
        }

        private void StartMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.Tag_Start(name);
        }

        private void EndMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.Tag_End(name);
        }
    }
}
