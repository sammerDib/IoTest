using System;
using System.Collections.Generic;

using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.DataMonitoring.DataWriter.File;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataWriterEditor
{
    public class DataWriterTypeSelectionPopup : Notifier
    {
        public DataWriterTypeSelectionPopup()
        {
            //For now, only FileDataWriter and ChartDataWriter is creatable at runtime
            var type = typeof(FileDataWriter);
            WriterTypes.Add(type);
            WriterTypes.Add(typeof(ChartDataWriter));
        }

        public List<Type> WriterTypes { get; } = new List<Type>();

        private Type _selectedWriterType;

        public Type SelectedWriterType
        {
            get { return _selectedWriterType; }
            set
            {
                if (_selectedWriterType == value) return;
                _selectedWriterType = value;
                OnPropertyChanged(nameof(SelectedWriterType));
            }
        }
    }
}
