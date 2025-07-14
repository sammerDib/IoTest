using System.Runtime.Serialization;
using System.ServiceModel;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Service.Interface.Alignment
{
    [DataContract]
    public enum ProgressType
    {
        [EnumMember] Information,

        [EnumMember] Error
    }

    [ServiceContract]
    public interface IProbeAlignmentServiceCallback
    {
        #region LiseHF

        #region XYAnalysis

        [OperationContract(IsOneWay = true)]
        void XYAlignmentProgress(string progress, ProgressType progressType);

        [OperationContract(IsOneWay = true)]
        void XYAlignmentChanged(AlignmentLiseHFXYAnalysisResult xyAnalysis);

        #endregion

        #region BeamProfiler

        [OperationContract(IsOneWay = true)]
        void BeamProfilerChanged(AlignmentLiseHFBeamProfileResult beamProfile);

        #endregion

        #region SpectrumCharact

        [OperationContract(IsOneWay = true)]
        void SpectrumCharactChanged(AlignmentLiseHFSpectrumCharacResult spectrumCharac);

        #endregion

        #endregion
    }
}
