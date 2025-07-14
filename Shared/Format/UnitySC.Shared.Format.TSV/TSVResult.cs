using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.MetroBase;

namespace UnitySC.Shared.Format.TSV
{
    [Serializable]
    public class TSVResult : MeasureResultDataBase
    {
        public override ResultFormat ResFormat { get; set; } = ResultFormat.Metrology; // Todo : En attente modification romain.

        [XmlElement("TestTSV")]
        public string Test { get; set; }
    }
}
