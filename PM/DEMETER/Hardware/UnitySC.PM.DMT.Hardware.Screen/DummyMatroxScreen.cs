using System.Threading.Tasks;
using System.Windows.Media;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;

namespace UnitySC.PM.DMT.Hardware.Screen
{
    public class DummyMatroxScreen : ScreenBase
    {
        public override void Init(DMTScreenConfig config, IGlobalStatusServer globalStatusServer, ScreenDensitronDM430GNControllerConfig screenDensitronConfig, ScreenController controller)
        {
            State = new DeviceState(DeviceStatus.Ready);
            ParseConfig(config);
        }

        public override void Shutdown()
        {
            // Method intentionally left empty.
        }

        public override void DisplayImage(USPImageMil procimg)
        {
            // Method intentionally left empty.
        }

        public override void Clear()
        {
            // Method intentionally left empty.
        }

        public override async Task DisplayImageAsync(USPImageMil procimg)
        {
            // Method intentionally left empty.
        }

        public override async Task ClearAsync(Color color)
        {
            // Method intentionally left empty.
        }
    }
}
