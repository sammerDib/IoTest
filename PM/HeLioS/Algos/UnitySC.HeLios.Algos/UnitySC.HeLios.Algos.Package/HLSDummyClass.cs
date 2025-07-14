using System;
using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.HeLios.Algos.Package
{
    // Dummy class use for example 
    // use shared cpp class
    public class HLSDummyClass : IDisposable
    {
        public HLSDummyClass()
        {
            Lut = new LutInterpolation();
        }

        public LutInterpolation Lut { set; get; }

        public double X( double x) { return (double)(Lut?.X(x));  }

        public void Dispose()
        {
            if (Lut != null)
            {
                Lut.Dispose();
                Lut = null;
            }
        }
    }
}
