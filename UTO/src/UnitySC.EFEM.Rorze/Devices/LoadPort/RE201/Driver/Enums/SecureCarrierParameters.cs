using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums
{
    public enum SecureCarrierOperationParameter
    {
        [Description("Opens the carrier.")] OpensTheCarrier = 0x0,

        [Description("Clamps the carrier.")] ClampsTheCarrier = 0x1
    }
}
