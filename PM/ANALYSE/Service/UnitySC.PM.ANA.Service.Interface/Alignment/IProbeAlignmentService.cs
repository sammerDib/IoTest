using System.ServiceModel;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Alignment
{
    [ServiceContract(CallbackContract = typeof(IProbeAlignmentServiceCallback))]
    public interface IProbeAlignmentService
    {
        #region LiseHF

        #region XY Analysis

        [OperationContract]
        Response<VoidResult> StartXYMeasurement(AlignmentLiseHFXYAnalysisInput input);

        [OperationContract]
        Response<VoidResult> InterruptXYMeasurement();

        #endregion

        #region Beam Profiler

        [OperationContract]
        Response<object> StartBeamProfiler();

        [OperationContract]
        Response<object> InterruptBeamProfiler();

        #endregion

        #region Spectrum Charact

        [OperationContract]
        Response<object> StartSpectrumCharact();

        [OperationContract]
        Response<object> InterruptSpectrumCharact();

        #endregion

        #endregion
    }
}
