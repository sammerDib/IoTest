using System.ServiceModel;

namespace UnitySC.PM.EME.Service.Interface.Chiller
{
    [ServiceContract]
    public interface IChillerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateFanSpeedCallback(double value);
        
        [OperationContract(IsOneWay = true)]
        void UpdateMaxCompressionSpeedCallback(double value);
        
        [OperationContract(IsOneWay = true)]
        void UpdateTemperatureCallback(double value);
        
        [OperationContract(IsOneWay = true)]
        void UpdateConstantFanSpeedModeCallback(ConstFanSpeedMode value); 
        
        [OperationContract(IsOneWay = true)]
        void UpdateAlarms(AlarmDetection value);
        
        [OperationContract(IsOneWay = true)]
        void UpdateLeaks(LeakDetection value);

        [OperationContract(IsOneWay = true)]
        void UpdateMode(ChillerMode mode);
    }
}
