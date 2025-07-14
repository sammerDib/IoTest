using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core
{
    public class DataCollectionPlanChartsNode : Notifier, IDisposable
    {

        public DataCollectionPlanChartsNode(DataCollectionPlan dcp)
        {
            Charts.CollectionChanged += ChartsOnCollectionChanged;
            DataCollectionPlan = dcp;
            foreach (var dataWriter in dcp.DataWriters.Where(writer => writer is ChartDataWriter))
            {
                var chartDataWriter = (ChartDataWriter)dataWriter;
                Charts.Add(chartDataWriter);
            }
        }

        private void ChartsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContainCharts = Charts.Any();
        }

        public DataCollectionPlan DataCollectionPlan { get; set; }

        private bool _containCharts;

        public bool ContainCharts
        {
            get { return _containCharts; }
            private set
            {
                if (_containCharts == value) return;
                _containCharts = value;
                OnPropertyChanged(nameof(ContainCharts));
            }
        }

        public ObservableCollection<ChartDataWriter> Charts { get; set; } = new ObservableCollection<ChartDataWriter>();
        
        #region IDisposable

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Charts.CollectionChanged -= ChartsOnCollectionChanged;
                }

                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
