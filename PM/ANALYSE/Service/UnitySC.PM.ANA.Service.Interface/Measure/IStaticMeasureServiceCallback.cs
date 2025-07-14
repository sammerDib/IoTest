using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface.Measure
{
    [ServiceContract]
    public interface IStaticMeasureServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StaticMeasureProgressChanged(MeasurePointProgress measurePoint);

        [OperationContract(IsOneWay = true)]
        void StaticMeasureResultChanged(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex, MeasurePoint measurePoint);
    }
}
