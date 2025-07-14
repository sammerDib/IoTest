using System;
using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance
{
    public enum MotorStatus
    {
        Running = 0,
        Overflow = 1,
        Overspeed = 2,
        Overload = 4,
        InPosition = 8,
        Disabled = 16,
        PushModeTorqueLimitReached = 32,
        OverTemperature = 128,
        PushModeTimeoutNotReached = 256,
        EmergencyStop = 512
    }

    public enum SpeedUnit
    {
        One = 1,
        Ten = 10,
        Hundred = 100,
    }

    // TODO: add comments to explain how it work
    public static class MotorResolution
    {
        public static List<int> ValidValues = new List<int>() { 200, 300, 400, 500, 600, 800, 1000, 1200, 1500, 2000, 2500, 3000, 4000, 5000, 6000, 8000, 10000, 12000, 25000, 50000 };

        public static Dictionary<int, int> ValuesForSpeed100 = new Dictionary<int, int>()
        {
            [200] = 0,
            [300] = 40,
            [400] = 1,
            [500] = 2,
            [600] = 42,
            [800] = 43,
            [1000] = 3,
            [1200] = 44,
            [1500] = 45,
            [2000] = 4,
            [2500] = 5,
            [3000] = 46,
            [4000] = 47,
            [5000] = 6,
            [6000] = 48,
            [8000] = 49,
            [10000] = 7,
            [12000] = 50,
            [25000] = 8,
            [50000] = 10,
        };

        public static Dictionary<int, int> ValuesForSpeed10 = new Dictionary<int, int>()
        {
            [200] = 20,
            [300] = 60,
            [400] = 21,
            [500] = 22,
            [600] = 62,
            [800] = 63,
            [1000] = 23,
            [1200] = 64,
            [1500] = 65,
            [2000] = 24,
            [2500] = 25,
            [3000] = 66,
            [4000] = 67,
            [5000] = 26,
            [6000] = 68,
            [8000] = 69,
            [10000] = 27,
            [12000] = 70,
            [25000] = 28,
            [50000] = 30,
        };

        public static Dictionary<int, int> ValuesForSpeed1 = new Dictionary<int, int>()
        {
            [50000] = 100,
        };

        public static int GetParameterValue(int motorResolution, SpeedUnit speedUnit)
        {
            switch (speedUnit)
            {
                case SpeedUnit.Hundred: return ValuesForSpeed100[motorResolution];
                case SpeedUnit.Ten: return ValuesForSpeed10[motorResolution];
                case SpeedUnit.One: return ValuesForSpeed1[motorResolution];
                default: throw new Exception($"SpeedUnit '{speedUnit}' not supported.");
            }
        }
    }
}
