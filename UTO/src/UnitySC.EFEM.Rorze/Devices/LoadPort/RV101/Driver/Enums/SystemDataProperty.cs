namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    public enum SystemDataProperty
    {
        SerialNoOfSystem                        = 0,
        HostIpAddress                           = 1,
        Rs232ConnectionHost                     = 2,
        BodyNumber                              = 6,
        EventEnablingFlag                       = 7,
        SoftwareSwitch                          = 8,
        EmergencyStopSignal                     = 10,
        TemporarilyStopSignal                   = 11,
        VacuumSourcePressureSignal              = 12,
        AirSourcePressureSignal                 = 13,
        OperatingTimeCarrierClamp               = 14,
        ChatteringPreventingTimeForPlacementJudgement = 15,
        CarrierDetectingTime                    = 16,
        OperatingTimeShutter                    = 17,
        PresenceSensor                          = 19,
        LineNoOfController                      = 39,
        PortNoHostPc                            = 68,
        IpAddressLogServer                      = 69
    }
}
