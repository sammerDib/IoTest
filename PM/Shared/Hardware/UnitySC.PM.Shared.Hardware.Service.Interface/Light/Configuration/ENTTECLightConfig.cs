using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    public class ENTTECLightConfig : LightConfig
    {
        /// This should contains an output
        /// If present the light will write the output on the given controller to the specific value
        /// If the light is not linked to an io don't include <LinkedIo> in the config even if empty
        public DigitalIoConsumerConfig LinkedIo { get; set; }

        // The value of the output
        // Unused if there is no LinkedIo
        public bool OutputActivationValue { get; set; }

        // Id of the light to turn off when turning this light
        // If there is no antagonist light don't include <AntagonistLightId> in the config even if empty
        public string AntagonistLightId { get; set; }
    }
}
