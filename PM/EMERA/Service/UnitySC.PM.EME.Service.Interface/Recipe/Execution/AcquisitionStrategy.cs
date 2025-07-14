using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.EME.Service.Interface.Recipe.Execution
{
    [DataContract]
    public enum AcquisitionStrategy
    {                
        [EnumMember]
        RasterScan,//Traverses the wafer line by line, like reading or writing a paragraph.
        [EnumMember]
        Serpentine,//Moves in continuous motion, eliminating the need for line resets and maximizing efficiency.
    }
}
