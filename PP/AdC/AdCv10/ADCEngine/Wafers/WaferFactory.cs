using System;

namespace ADCEngine.Wafers
{
    public static class WaferFactory
    {
        public static WaferBase CreateWafer(string waferTypeName)
        {
            int diameter = ExtractDiameter(waferTypeName);

            switch (waferTypeName)
            {
                case var name when name.Contains("Notch"):
                    return new NotchWafer()
                    {
                        Diameter = diameter,
                        NotchSize = CalculateNotchSize(diameter)
                    };

                case var name when name.Contains("Flat"):
                    return new FlatWafer() { Diameter = diameter };

                case var name when name.Contains("Rectangular"):
                    throw new ArgumentException("Rectangular wafers are not supported.");

                default:
                    return new NotchWafer()
                    {
                        Diameter = 300000,
                        NotchSize = CalculateNotchSize(300000)
                    };
            }
        }

        private static int ExtractDiameter(string waferTypeName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(waferTypeName, "(\\d{3})");
            return match.Success ? int.Parse(match.Value) * 1000 : 0;
        }

        private static int CalculateNotchSize(int diameter)
        {
            return diameter / 100;
        }
    }
}
