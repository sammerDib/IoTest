using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Interface.AxesService
{
    public interface IArgosAxesService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToAxesChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToAxesChanges();

        [OperationContract]
        Response<AxesConfig> GetAxesConfiguration();

        [OperationContract]
        Response<bool> Move(params PMAxisMove[] moves);
        
        [OperationContract]
        Response<bool> RelativeMove(params PMAxisMove[] moves);

        [OperationContract]
        Response<bool> GoToHome(AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToLoadUnload(AxisSpeed speed);

        [OperationContract]
        Response<bool> WaitMotionEnd(int timeout);

        [OperationContract]
        Response<PositionBase> GetCurrentPosition();

        [OperationContract]
        Response<bool> StopAllMoves();

    }
}
