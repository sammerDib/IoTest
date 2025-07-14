namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    public class ACSLightConfig : LightConfig
    {
        /// <summary>Name of variable in ACS to control light intensitys</summary>
        public string Control { get; set; }

        /// <summary>Name of variable in ACS to control light power (On/Off)</summary>
        public string Power { get; set; }
    }
}
