using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReferentialService : BaseService, IReferentialService
    {
        private IReferentialManager _refManager;

        public ReferentialService(ILogger logger) : base(logger, ExceptionType.ReferentialException)
        {
            _refManager = ClassLocator.Default.GetInstance<IReferentialManager>();
        }

        public Response<PositionBase> ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return InvokeDataResponse(messageContainer =>
            {
                return _refManager.ConvertTo(positionToConvert, referentialTo);
            });
        }

        public Response<ReferentialSettingsBase> GetSettings(ReferentialTag referentialTag)
        {
            return InvokeDataResponse<ReferentialSettingsBase>(messageContainer =>
                _refManager.GetSettings(referentialTag)
            );
        }

        public Response<VoidResult> DeleteSettings(ReferentialTag referentialTag)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _refManager.DeleteSettings(referentialTag);
            });
        }

        public Response<VoidResult> DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _refManager.DisableReferentialConverter(referentialTag1, referentialTag2);
            });
        }

        public Response<VoidResult> EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _refManager.EnableReferentialConverter(referentialTag1, referentialTag2);
            });
        }

        public Response<VoidResult> SetSettings(ReferentialSettingsBase settings)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _refManager.SetSettings(settings);
            });
        }
    }
}
