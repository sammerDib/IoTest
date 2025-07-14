using System.Collections.ObjectModel;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Modules.Settings.View.Designer;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    [XmlInclude(typeof(EllipseDrawingItem))]
    [XmlInclude(typeof(PolygonDrawingItem))] 
    public class  HighAngleDarkFieldImageSettingData
    {
        public ObservableCollection<DrawingItem> DrawingImageItems;

    }
}
