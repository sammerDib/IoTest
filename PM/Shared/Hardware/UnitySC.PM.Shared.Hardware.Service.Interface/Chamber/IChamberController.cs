using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public interface IChamberController
    {
        void Init(List<Message> initErrors);

        bool ResetController();

        void Connect();

        void Connect(string deviceId);

        void Disconnect();

        void Disconnect(string deviceID);

    }
}
