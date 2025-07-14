using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [ServiceContract(CallbackContract = typeof(IMeasureServiceCallback))]
    public interface IMeasureService
    {
        [OperationContract]
        Response<List<MeasureType>> GetAvailableMeasures();

        [OperationContract]
        Response<MeasureToolsBase> GetMeasureTools(MeasureSettingsBase measureSettings);

        [OperationContract]
        Response<MeasureConfigurationBase> GetMeasureConfiguration(MeasureType measureType);

        [OperationContract]
        Response<List<string>> GetMeasureLightIds(MeasureType measureType);

        [OperationContract]
        Response<VoidResult> SubscribeToMeasureChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToMeasureChanges();

        [OperationContract]
        Response<VoidResult> StartMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, DieIndex dieIndex);

        [OperationContract]
        Response<VoidResult> StartMeasureWithSubMeasures(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, List<MeasurePoint> subMeasurePoints, DieIndex dieIndex);

        [OperationContract]
        Response<VoidResult> StartStaticRepetaMeasure(MeasureSettingsBase measureSettings, MeasurePoint measurePoint, int nbOfStaticRepeta);

        [OperationContract]
        Response<VoidResult> CancelMeasure();

        [OperationContract]
        Response<VoidResult> StopStaticRepetaMeasure();
    }
}
