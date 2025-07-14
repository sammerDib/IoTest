using System;
using System.Collections.Generic;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class FresnelCoefficients
    {
        public string Material;

        public List<FresnelCoefficient> Coefficients;
    }

    [Serializable]
    public class FresnelCoefficient
    {
        public int WaveLength;

        public Complex Coefficient;
    }

    [Serializable]
    public class Complex
    {
        public double Real;
        public double Imaginary;
    }
}
