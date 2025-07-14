using System;
using System.Windows.Media;

using Agileo.DataMonitoring;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    public class DataSourceConfiguration : Notifier
    {
        public DataSourceConfiguration(IDataSource dataSource, Color sourceLineColor)
        {
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            SourceName = dataSource.Information.SourceName;
            SourceType = dataSource.Information.SourceUnitName;
            SourceUnit = dataSource.Information.SourceUnitAbbreviation;
            SourceLineColor = sourceLineColor;
        }

        public string SourceName { get; set; }

        public string SourceType { get; set; }

        public string SourceUnit { get; set; }

        private Color _sourceLineColor;

        public Color SourceLineColor
        {
            get { return _sourceLineColor; }
            set
            {
                if (_sourceLineColor.Equals(value)) return;
                _sourceLineColor = value;
                OnPropertyChanged(nameof(SourceLineColor));
            }
        }

        public override string ToString()
        {
            return string.Concat(nameof(SourceName), ": ", SourceName, " | ", nameof(SourceType), ": ", SourceType, " | ", nameof(SourceUnit), ": ", SourceUnit, " | ", nameof(SourceLineColor), ": ", SourceLineColor);
        }
    }
}
