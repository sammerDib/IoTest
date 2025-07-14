using System.Runtime.Serialization;
using System.ServiceModel;

using UnitySC.PM.ANA.Service.Core.Calibration;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public enum ProgressType
    {
        [EnumMember]
        Information,

        [EnumMember]
        Error
    }

    [ServiceContract]
    public interface ICalibrationServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ObjectiveCalibrationChanged(ObjectiveCalibrationResult objCalibResult);

        [OperationContract(IsOneWay = true)]
        void XYCalibrationChanged(XYCalibrationData xyCalibrationData);

        [OperationContract(IsOneWay = true)]
        void XYCalibrationTestChanged(XYCalibrationTest xyCalibrationTest);

        [OperationContract(IsOneWay = true)]
        void XYCalibrationProgress(string progress, ProgressType progressType);

        [OperationContract(IsOneWay = true)]
        void XYCalibrationTestProgress(string progress, ProgressType progressType);

        [OperationContract(IsOneWay = true)]
        void LiseHFSpotCalibrationChanged(LiseHFSpotCalibrationResults liseHFSpotCalibResults);

        [OperationContract(IsOneWay = true)]
        void LiseHFRefCalibrationChanged(LiseHFIntegrationTimeCalibrationResults liseHFRefCalibResults);
    }
}
