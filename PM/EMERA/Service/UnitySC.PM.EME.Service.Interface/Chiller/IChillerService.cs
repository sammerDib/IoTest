using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Chiller
{
    [ServiceContract(CallbackContract = typeof(IChillerServiceCallback))]
    public interface IChillerService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();
        
        [OperationContract]
        Response<VoidResult> Reset();
                
        [OperationContract]
        Response<VoidResult> SetTemperature(double value);
        
        [OperationContract]
        Response<VoidResult> SetConstFanSpeedMode(ConstFanSpeedMode mode);
                
        [OperationContract]
        Response<VoidResult> SetChillerMode(ChillerMode mode);
        
        [OperationContract]
        Response<VoidResult> SetFanSpeed(double value);
        
        [OperationContract]
        Response<VoidResult> SetMaxCompressionSpeed(double value);
    }
}
