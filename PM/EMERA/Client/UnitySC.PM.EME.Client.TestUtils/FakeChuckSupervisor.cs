using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeChuckSupervisor : IEMEChuckService
    {
        private ChuckState _state = new ChuckState()
        {
            WaferClampStates = new Dictionary<Length, bool>(),
            WaferPresences = new Dictionary<Length, MaterialPresence>()
        };

        private readonly IMessenger _messenger;

        public FakeChuckSupervisor(IMessenger messenger, List<WaferCategory> waferCategories)
        {
            _messenger = messenger;
            waferCategories.ForEach(cat =>
                                            {
                                                _state.WaferClampStates.Add(cat.DimentionalCharacteristic.Diameter, false);
                                                _state.WaferPresences.Add(cat.DimentionalCharacteristic.Diameter, MaterialPresence.Present);
                                            });
        }

        public Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            _state.WaferClampStates[wafer.Diameter] = true;
            _messenger.Send(_state);
            return new Response<bool> { Result = true };
        }

        public Response<ChuckState> GetCurrentState()
        {
            return new Response<ChuckState> { Result = _state };
        }

        public Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            _state.WaferClampStates[wafer.Diameter] = false;
            _messenger.Send(_state);
            return new Response<bool> { Result = true };
        }

        public Task<Response<VoidResult>> ResetAirbearing()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new System.NotImplementedException();
        }

        public Response<VoidResult> RefreshAllValues()
        {
            throw new System.NotImplementedException();
        }
    }
}
