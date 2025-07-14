using System.Runtime.Serialization;

namespace UnitySC.Shared.Data.Enum
{
    // [ActorType(10)]
    // Sub section of actor type [Actor Category (4 bit)] [0-15] cf ActorCategory
    // Sub section of actor Specific Actor Type (6 bits)] [0-63] per ActorCategory
    public enum ActorType
    {
        [EnumMember(Value = "Unknown")]
        Unknown = 0 << PMEnumHelper.SpecificActorShift | ActorCategory.Unknown,
        [EnumMember(Value = "DEMETER")]
        DEMETER = 1 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "BrightField2D")]
        BrightField2D = 2 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "Darkfield")]
        Darkfield = 3 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "BrightFieldPattern")]
        BrightFieldPattern = 4 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "Edge")]
        Edge = 5 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "NanoTopography")]
        NanoTopography = 6 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "LIGHTSPEED")]
        LIGHTSPEED = 7 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "BrightField3D")]
        BrightField3D = 8 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "EdgeInspect")]
        EdgeInspect = 9 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "ANALYSE")]
        ANALYSE = 10 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "HardwareControl")]
        HardwareControl = 11 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "HeLioS")]
        HeLioS = 12 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "Argos")]
        Argos = 13 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "EMERA")]
        EMERA = 14 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "Wotan")]
        Wotan = 15 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,
        [EnumMember(Value = "Thor")]
        Thor = 16 << PMEnumHelper.SpecificActorShift | ActorCategory.ProcessModule,

        // Post Processing
        [EnumMember(Value = "ADC")]
        ADC = 1 << PMEnumHelper.SpecificActorShift | ActorCategory.PostProcessing,

        // Manager
        [EnumMember(Value = "DataflowManager")]
        DataflowManager = 1 << PMEnumHelper.SpecificActorShift | ActorCategory.Manager,
        [EnumMember(Value = "DataAccess")]
        DataAccess = 2 << PMEnumHelper.SpecificActorShift | ActorCategory.Manager,
    }


}
