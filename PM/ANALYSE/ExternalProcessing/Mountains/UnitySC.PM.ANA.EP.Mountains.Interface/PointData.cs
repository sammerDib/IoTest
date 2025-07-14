using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.EP.Mountains.Interface
{
    [DataContract]
    public class PointData 
    {
        [DataMember]
        public string StudiableFile { get; set; }

        [DataMember]
        public int PointNumber { get; set; }

        [DataMember]
        public double XCoordinate { get; set; }

        [DataMember]
        public double YCoordinate { get; set; }

        #region Contructor

        public PointData() { }

        public PointData(string studiableFile, int pointNumber)
        {
            StudiableFile = studiableFile;
            PointNumber = pointNumber;
        }

        #endregion
    }
}
