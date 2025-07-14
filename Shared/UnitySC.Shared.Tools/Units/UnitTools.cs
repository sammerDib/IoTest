using System;

namespace UnitySC.Shared.Tools.Units
{
    public class UnitTools
    {
        public static double MultiplyDoubleDecimal(double valueDouble, decimal coefDecimal)
        {
            double res;

            try
            {
                decimal valueDecimal = Convert.ToDecimal(valueDouble);
                res = (double)(valueDecimal * coefDecimal);
            }
            catch (OverflowException)
            {
                res = valueDouble * (double)coefDecimal;
            }

            return res;
        }

        public static double DivideDoubleDecimal(double valDouble, decimal valDecimal)
        {
            double res;

            try
            {
                decimal valueDecimal = Convert.ToDecimal(valDouble);
                res = (double)(valueDecimal / valDecimal);
            }
            catch (OverflowException)
            {
                res = valDouble / (double)valDecimal;
            }

            return res;
        }
    }
}
