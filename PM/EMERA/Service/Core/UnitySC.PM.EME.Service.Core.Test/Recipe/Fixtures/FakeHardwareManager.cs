using System.Collections.Generic;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Core.Test.Recipe.Fixtures
{
    public class FakeHardwareManager : EmeHardwareManager
    {
        public FakeHardwareManager(EMERecipe emeRecipe) : base(null, null,new FakeConfigurationManager(), null, null)
        {
            EMELights = new Dictionary<string, EMELightBase>();

            foreach (var acquisition in emeRecipe.Acquisitions)
            {
                var config = new EMELightConfig
                {
                    DeviceID = acquisition.LightDeviceId, Type = ConvertToType(acquisition.LightDeviceId)
                };
                EMELights.Add(acquisition.LightDeviceId,
                    new EMELight(config, null, null, new SerilogLogger<EMELight>()));
            }
        }

        private static EMELightType ConvertToType(string acquisitionLightDeviceId)
        {
            switch (acquisitionLightDeviceId)
            {
                case "3":
                    return EMELightType.DirectionalDarkField0Degree;
                case "4":
                    return EMELightType.DirectionalDarkField90Degree;
            }

            return EMELightType.Unknown;
        }
    }
}
