using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySC.PM.Shared.ADCConfiguration
{
    public class WaferType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Diameter { get; set; }
        public double SizeX { get; set; }
        public double SizeY { get; set; }
        public string Comment { get; set; }
        public string SurfaceName { get; set; }
        public double FlatVerticalX { get; set; }
        public double FlatHorizontalY { get; set; }
        public int Shape { get; set; }
        public WaferShape ShapeType
        {
            get { return (WaferShape)Shape; }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum WaferShape : int
    {
        Notch = 0,
        Flat = 1,
        DoubleFlat = 2,
        Rectangular = 3
    }
}
