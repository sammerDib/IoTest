

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using System.ServiceModel;
using UnitySC.PM.ANA.Service.Interface.Positionning;
using UnitySC.Shared.Tools.Service;
using System;

namespace UnitySC.PM.ANA.Service.Test
{

    public class PositionningServiceTestClient : ServiceTestClient<IPositionningService>, IPositionningService
    {

        public PositionningServiceTestClient(InstanceContext instanceContext) :
            base("PositionningService", instanceContext)
        { }

        public Response<CompositePosition> StartConversion(CompositePosition position, Referential targetReferential)
        {
            var resp = ServiceInvoker.InvokeAndGetMessages(s => s.StartConversion(position, targetReferential));
            return resp;
        }
    }


    [TestClass]
    public class PositionningServiceTest : IPositionningServiceCallback
    {
        // IPositionningServiceCallback
        public void ConversionDone(CompositePosition position)
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Expect_Service_to_convert_positions()
        {

            var instanceContext = new InstanceContext(this);
            var testClient = new PositionningServiceTestClient(instanceContext);

            IPositionningService service = new PositionningService(new SerilogLogger<PositionningService>());
            testClient.SetUpService(service);
            var positionBuilder = new CompositePosition.Builder(Referential.Wafer);
            var position = positionBuilder
                .Add(Axes.X, 0)
                .Add(Axes.Y, 0)
                .Build();


            testClient.StartConversion(position, Referential.Stage);

            testClient.TearDownService();
        }
    }
}
