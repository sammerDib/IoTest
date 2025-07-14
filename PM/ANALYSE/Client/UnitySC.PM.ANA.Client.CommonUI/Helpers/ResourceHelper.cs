using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;

namespace UnitySC.PM.ANA.Client.CommonUI.Helpers
{
    public static class ResourceHelper
    {
        public static string GetMeasureName(MeasureType measureType)
        {
            var resourceName = "MeasureName" + measureType.ToString();
            var measureName = Properties.Resources.ResourceManager.GetString(resourceName);
            if (measureName is null)
                throw new Exception("The name for the measure could not be found in resources: " + resourceName);
            return measureName;
        }

        public static string GetMeasureDescription(MeasureType measureType)
        {
            var resourceName = "MeasureDescription" + measureType.ToString();
            var measureName = Properties.Resources.ResourceManager.GetString(resourceName);
            if (measureName is null)
                throw new Exception("The desription for the measure could not be found in resources: " + resourceName);
            return measureName;
        }

        public static BitmapImage GetMeasureImage(MeasureType measureType)
        {
            var uriPack = new Uri(@"pack://application:,,,/UnitySC.PM.ANA.Client.CommonUI;component/Styles/MeasureIcons/Measure" + measureType.ToString()+".png");
            return new BitmapImage(uriPack);
        }
    }
}
