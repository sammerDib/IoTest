using System;
using System.Windows;
using System.Windows.Media;

using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.UIComponents.XamlResources.Shared
{
    public static class UnityScIcons
    {
        private static readonly ResourceDictionary Resources = new()
        {
            Source = new Uri("pack://application:,,,/UnitySC.Shared.UI;component/Styles/ImageGeometries.xaml", UriKind.Absolute)
        };

        private static PathGeometryIcon GetPathGeometry(string key)
        {
            var resource = Resources[key];
            if (resource is Geometry geometry)
            {
                return new PathGeometryIcon(PathGeometry.CreateFromGeometry(geometry));
            }

            return null;
        }

        public static PathGeometryIcon WaferResult => GetPathGeometry("WaferResultGeometry");

        public static PathGeometryIcon DataFlow => GetPathGeometry("DataflowGeometry");

        public static PathGeometryIcon Notifier => GetPathGeometry("BellSolidGeometry");
    }
}
