using System;

using System.ServiceModel;



using UnitySC.PM.ANA.Service.Interface.Positionning;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{

    /// <summary>
    /// Allow to remotely convert a position from a Referential to another
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PositionningService : DuplexServiceBase<IPositionningServiceCallback>, IPositionningService
    {
        private IReferentialManager _referentialManager;
        public PositionningService(ILogger logger) : base(logger, ExceptionType.PositionningException)
        {
            _referentialManager = new ReferentialManagerImpl();

        }

        public Response<CompositePosition> StartConversion(CompositePosition position, Referential targetReferential)
        {
            var response = new Response<CompositePosition>
            {
                Result = _referentialManager.Convert(position).To(targetReferential)
            };
            return response;
        }


        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
