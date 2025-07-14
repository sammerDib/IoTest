using System.ServiceModel;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [ServiceContract]
    public interface IMeasureServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void MeasureProgressChanged(MeasurePointProgress measurePoint);

        [OperationContract(IsOneWay = true)]
        void MeasureResultChanged(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex);

        [OperationContract(IsOneWay = true)]
        void StaticMeasureResultStarted(int repeatIndex);

        [OperationContract(IsOneWay = true)]
        void StaticMeasureResultChanged(MeasurePointResult res, int repeatIndex);
    }
}
