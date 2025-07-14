using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.ResultUI.Common;
using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Stats
{
    public abstract class MetroStatsVM : LotStatsVM
    {
        private readonly IMessenger _messenger;

        #region Properties

        public DataTableSource<WaferResultData> WaferResultDatas { get; } = new DataTableSource<WaferResultData>();

        public Func<int, ResultState> IntToResultStateStringFunc { get; } = i => (ResultState)i;

        public DataInToleranceLineChart MetroStatsChart { get; }

        public SortDefinition SortBySlotId { get; }

        public SortDefinition SortByState { get; }

        #endregion

        protected MetroStatsVM()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _messenger.Register<DisplaySelectedResultFullNameMessage>(this, (r, m) => OnChangeSelectedResultFullName(m));

            LotViews = StatsFactory.GetEnumsWithDescriptions<LotView>();
            LotSelectedView = new KeyValuePair<LotView, string>();

            MetroStatsChart = new DataInToleranceLineChart("Wafer Slot", true, Colors.MediumPurple, false);

            SortBySlotId = WaferResultDatas.Sort.AddSortDefinition(data => data.SlotId);
            SortByState = WaferResultDatas.Sort.AddSortDefinition(data => data.ResultItem.State);
        }

        #region EventHandlers

        private void OnChangeSelectedResultFullName(DisplaySelectedResultFullNameMessage msg)
        {
            SelectedResultFullName = msg.SelectedResultFullName;
        }

        #endregion EventHandlers

        #region Overrides of ObservableObject

        protected override void OnDeactivated()
        {
            _messenger.Unregister<DisplaySelectedResultFullNameMessage>(this);
            base.OnDeactivated();
        }

        #endregion

        #region Overrides of LotStatsVM

        public override void UpdateStats(object stats)
        {
            if (stats is WaferResultData[] statsArray)
            {
                var waferResultDatas = statsArray.Where(data => data?.ResultItem != null && data.ResultItem.ResultItemValues?.Count > 0).ToList();
                WaferResultDatas.Reset(waferResultDatas);

                MetroStatsChart.ClearAll();
                RedrawChart();
            }
        }

        public override void SelectLotView(object lotview)
        {
            var lv = (LotView)lotview;
            LotSelectedView = LotViews.FirstOrDefault(x => x.Key == lv);
        }

        #endregion

        #region Protected

        protected void SetData(IEnumerable<WaferResultData> datas, string measureName)
        {
            MetroStatsChart.SetData(datas,
                data => data.SlotId,
                data => data.ResultItem.ResultItemValues.SingleOrDefault(value => value.Name.Equals(measureName) && value.Type == (int)ResultValueType.Mean)?.Value,
                data => data.ResultItem.ResultItemValues.SingleOrDefault(value => value.Name.Equals(measureName) && value.Type == (int)ResultValueType.Min)?.Value,
                data => data.ResultItem.ResultItemValues.SingleOrDefault(value => value.Name.Equals(measureName) && value.Type == (int)ResultValueType.Max)?.Value,
                data =>
                {
                    double? state = data.ResultItem.ResultItemValues.SingleOrDefault(value => value.Name.Equals(measureName) && value.Type == (int)ResultValueType.State)?.Value;
                    var typedState = state.HasValue ? (MeasureState)state : MeasureState.NotMeasured;
                    return MetroHelper.GetSymbol(typedState);
                }, measureName);
        }


        #endregion

        #region Commands

        private AutoRelayCommand _exportCsvCommand;

        public AutoRelayCommand ExportCsvCommand => _exportCsvCommand ?? (_exportCsvCommand = new AutoRelayCommand(ExportToCsv));

        private void ExportToCsv()
        {
            var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
            var dialog = new SaveFileDialog
            {
                // Only csv file accepted ?
                Filter = @"Text Files (*.csv) | *.csv",
                FileName = "LotStats.csv"
            };

            if (dialog.ShowDialog() != true) return;

            string filePath = dialog.FileName;

            try
            {
                using (var sw = new StreamWriter(filePath, false))
                {
                    var csvLines = GetCsvLines();

                    string csvContent = string.Join(Environment.NewLine, csvLines);
                    byte[] bytes = Encoding.UTF8.GetBytes(csvContent);
                    byte[] fileBytes = Encoding.UTF8.GetPreamble().Concat(bytes).ToArray();

                    sw.BaseStream.Write(fileBytes, 0, fileBytes.Length);
                    sw.Close();

                    notifierVM.AddMessage(new Message(MessageLevel.Information, $"File: {filePath} was saved with success."));
                }
            }
            catch (IOException)
            {
                notifierVM.AddMessage(new Message(MessageLevel.Error, "Destination file is opened. Please close it and retry."));
            }
            catch (Exception ex)
            {
                notifierVM.AddMessage(new Message(MessageLevel.Error, ex.Message));
            }
        }

        protected abstract IEnumerable<string> GetCsvLines();

        #endregion

        protected abstract void RedrawChart();

        protected static double? GetValue(WaferResultData data, string name, ResultValueType type)
        {
            return data.ResultItem.ResultItemValues
                .FirstOrDefault(value => value.Name.Equals(name) && value.Type == (int)type)?.Value;
        }

        protected static string GetFormatedValue(WaferResultData data, string name, ResultValueType type, string nullValue = "")
        {
            double? value = GetValue(data, name, type);
            return value?.ToString("F5", CultureInfo.InvariantCulture) ?? nullValue;
        }

        protected static MeasureState? GetState(WaferResultData data, string name)
        {
            double? v = data.ResultItem.ResultItemValues.FirstOrDefault(value => value.Name.Equals(name) && value.Type == (int)ResultValueType.State)?.Value;
            if (v == null) return null;

            int index = Convert.ToInt32(v);
            return (MeasureState)index;
        }

        protected static int GetStateIndex(WaferResultData data, string name)
        {
            var MeasureState = GetState(data, name);
            return MeasureState.HasValue ? (int)MeasureState.Value : int.MaxValue;
        }
    }
}
