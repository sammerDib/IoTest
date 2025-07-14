using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.AGS.Data.Enum;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.AGS.Service.Interface.Flow
{
    [DataContract]
    public class AcquisitionInput : IArgosInputFlow
    {
        public AcquisitionInput(uint frequency, Length waferRadius, Length pixelSize, int yResolution, double startAngle, double psoAccelerationAngle, Dictionary<SensorID, SensorRecipe> sensorRecipes)
        {
            Frequency = frequency;
            WaferRadius = waferRadius;
            PixelSize = pixelSize;
            YResolution = yResolution;
            StartAngle = startAngle;
            PsoAccelerationAngle = psoAccelerationAngle;
            SensorRecipes = sensorRecipes;
        }

        public AcquisitionInput()
        {
        }

        public InputValidity CheckInputValidity()
        {
            throw new System.NotImplementedException();
        }

        public ArgosContextBase InitialContext { get; }

        public uint Frequency;
        
        public Length WaferRadius;

        public Length PixelSize;

        public int YResolution;

        public int XResolution;

        public double StartAngle;
        
        public double PsoAccelerationAngle;

        public Dictionary<SensorID, SensorRecipe> SensorRecipes;
        
        public Dictionary<string, double> SensorCalibrationByAxesId;
    }
}
