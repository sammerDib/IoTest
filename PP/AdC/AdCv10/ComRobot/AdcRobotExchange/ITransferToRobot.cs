using System.Collections.Generic;
using System.ServiceModel;

namespace AdcRobotExchange
{
    public enum eDataBaseType
    {
        ConfigurationDataBase,
        ResultDataBase,
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: Interface WCF
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [ServiceContract]
    public interface ITransferToRobot
    {
        [OperationContract]
        void TransferVids(string Toolname, string UniqueID, List<VidBase> VidList);
        [OperationContract]
        void TransferInputList(string Toolname, string UniqueID, List<AdcInput> AdcInputList);
        [OperationContract]
        void TransferWaferReport(string Toolname, string UniqueID, WaferReport WaferReport);
        [OperationContract]
        void TransferDataBaseStatus(string Toolname, eDataBaseType DBType, bool Connected);
    }
}
