using System.IO;
using System.Text;

using UnitySC.Shared.Format.Helper;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoFocus
{
    internal class GetZFocusReport
    {
        private readonly StringBuilder _report = new StringBuilder();

        public void AddToReport(double currentZPosition, double currentDistance)
        {
            _report.AppendLine(MeasurementReport(currentZPosition, currentDistance));
        }

        private string MeasurementReport(double position, double distance)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();
            return $"{position}{separator}{distance}";
        }

        public void Save(string filename)
        {
            File.WriteAllText(filename, _report.ToString());
        }
    }
}
