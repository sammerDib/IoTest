using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace AdcRobotExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: Structure décrivant les inputs de l'ADC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class AdcInput
    {
        //[Obsolete][DataMember] public int ActorTypeId;
        //[Obsolete][DataMember] public int ChannelID;

        [DataMember] public int InputResultType; // check UnitySC.Shared.Data.Enum.ResultType
        [DataMember] public string InputPictureDirectory;
    }


}
