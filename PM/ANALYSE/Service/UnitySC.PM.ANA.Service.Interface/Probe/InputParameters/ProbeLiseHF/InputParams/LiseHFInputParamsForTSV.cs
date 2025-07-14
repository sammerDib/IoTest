using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class LiseHFInputParamsForTSV : LiseHFInputParams
    {
        public LiseHFInputParamsForTSV()
        {
        }

        #region Properties

        [DataMember]
        public Length TSVDiameter { get; set; }

        [DataMember]
        public Length TSVDepth { get; set; }

        [DataMember]
        public Length TSVDepthTolerance { get; set; }

        #endregion Properties
    }
}
