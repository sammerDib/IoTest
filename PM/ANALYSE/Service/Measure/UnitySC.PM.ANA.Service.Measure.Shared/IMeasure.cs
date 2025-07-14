using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public delegate void MeasureProgressChangedEventHandler(MeasurePointProgress measurePointProgressBase);

    public delegate void MeasureDataChangedEventHandler(MeasurePointDataResultBase measurePointDataResultBase, MeasureContext measureContext);

    public delegate void MeasureResultChangedEventHandler(MeasurePointResult measurePointProgressBase, string resultFolderPath, DieIndex dieIndex = null);

    public interface IMeasure
    {
        MeasureType MeasureType { get; }

        bool WaferUnclampedMeasure { get; }

        MeasureToolsBase GetMeasureTools(MeasureSettingsBase measureSettings);

        TimeSpan GetEstimatedTime(MeasureSettingsBase measureSettings);

        bool PrepareExecution(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default);

        MeasurePointResult Execute(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default, bool useStaticRepeta = false);

        MeasurePointResult ExecuteSubMeasure(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default, bool useStaticRepeta = false);

        MeasurePointResult ComputeMeasureFromSubMeasures(MeasureSettingsBase measureSettings, MeasureContext measureContext, List<MeasurePointResult> subResults, CancellationToken cancelToken = default, bool useStaticRepeta = false);

        MeasurePointDataResultBase ExecutePostProcessing(MeasureSettingsBase measureSettings, MeasureContext measureContext, MeasurePointDataResultBase result);

        List<string> GetLightIds();

        MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings);

        MeasureDieResult CreateMetroDieResult();

        MeasurePointResult CreateNotMeasuredEmptyResult(string message);

        /// <summary>
        /// This function will be call during recipe execution, before next measure start
        /// or before recipe end. It is used, for example, to perform the final calibration
        /// of a dual thickness measurement.
        /// </summary>
        /// <param name="measureSettingsBase"></param>
        void MeasureTerminatedInRecipe(MeasureSettingsBase measureSettingsBase);

        /// <summary>
        /// Check if Z Axis should be frozen and dont be moved during the whole measurmement
        /// </summary>
        /// <param name="measureSettingsBase"></param>
        /// <returns>false if Z axis should be frozen and dont be moved during the whole measurmement, true otherwise</returns>
        bool CanZAxisMove(MeasureSettingsBase measureSettingsBase);

        void ApplyMeasureCorrection(MeasureResultBase measureResult, MeasureSettingsBase measureSettingsBase);

        void FinalizeMetroResult(MeasureResultBase measureResultBase, MeasureSettingsBase measureSettingsBase);

        event MeasureProgressChangedEventHandler MeasureProgressChangedEvent;
    }
}
