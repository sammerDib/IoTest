using System;

using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.TestUtils
{
    public class FakeReferentialSupervisor : IReferentialService
    {
        private ReferentialSettingsBase _currentReferentialSettings;

        public Response<PositionBase> ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return new Response<PositionBase> { Result = positionToConvert };
        }

        public Response<VoidResult> DeleteSettings(ReferentialTag referentialTag)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            throw new NotImplementedException();
        }

        public Response<ReferentialSettingsBase> GetSettings(ReferentialTag referentialTag)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SetSettings(ReferentialSettingsBase settings)
        {
            _currentReferentialSettings = settings;
            return new Response<VoidResult>();
        }
    }
}
