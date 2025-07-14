using Agileo.GUI.Services.Icons;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.Helpers;

// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace UnitySC.GUI.Common.UIComponents.XamlResources.Shared
{
    public static class CustomPathIcon
    {
        private static PathGeometryIcon GetPathGeometryIcon(string pathGeometryKey)
        {
            return new PathGeometryIcon(ResourcesHelper.Get<PathGeometry>(pathGeometryKey));
        }

        public static PathGeometryIcon Connected => GetPathGeometryIcon("ConnectedIcon");
        public static PathGeometryIcon ControlAndProcessJob => GetPathGeometryIcon("ControlAndProcessJobIcon");
        public static PathGeometryIcon ControlJobHOQ => GetPathGeometryIcon("ControlJobHOQIcon");
        public static PathGeometryIcon Deselection => GetPathGeometryIcon("DeselectionIcon");
        public static PathGeometryIcon DisabledNotifier => GetPathGeometryIcon("DisabledNotifierIcon");
        public static PathGeometryIcon Disconnected => GetPathGeometryIcon("DisconnectedIcon");
        public static PathGeometryIcon EnabledNotifier => GetPathGeometryIcon("EnabledNotifierIcon");
        public static PathGeometryIcon GoTo => GetPathGeometryIcon("GoToIcon");
        public static PathGeometryIcon Reservation => GetPathGeometryIcon("ReservationIcon");
        public static PathGeometryIcon Wafer => GetPathGeometryIcon("WaferIcon");
        public static PathGeometryIcon FourInch => GetPathGeometryIcon("FourInch");
        public static PathGeometryIcon SixInch => GetPathGeometryIcon("SixInch");
        public static PathGeometryIcon EightInch => GetPathGeometryIcon("EightInch");
    }
}
