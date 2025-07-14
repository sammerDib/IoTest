using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Format.Metro
{
    [DataContract]
    public class MeasureScanLine : MeasurePointDataResultBase
    {
        [DataMember]
        public RawProfile RawProfileScan { get; set; }
    }
}
