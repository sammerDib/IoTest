using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Defect
{
    public abstract class DefectVM : LotStatsVM
    {
        #region Properties

        // Private
        private GridStatsVM _gridStatsVM;

        private HistogramVMBase _histogramVM;
        private Dictionary<HistogramType, HistogramVMBase> _histogramDictionary;
        private Dictionary<ResultValueType, List<WaferStatsData>> _lotStatsData;
        private bool _isExportCsvButtonEnabled;

        // Public
        public List<KeyValuePair<HistogramType, string>> HistogramTypes { get; set; }

        public List<ResultValueType> ResultValueTypes { get; private set; }

        public GridStatsVM GridStatsVM
        {
            get => _gridStatsVM;
            set
            {
                if (_gridStatsVM != value)
                {
                    _gridStatsVM = value;
                    OnPropertyChanged();
                }
            }
        }

        public HistogramVMBase HistogramVM
        {
            get => _histogramVM;
            private set
            {
                if (_histogramVM == value) return;
                _histogramVM = value;
                OnPropertyChanged();
            }
        }

        public bool IsExportCsvButtonEnabled
        {
            get => _isExportCsvButtonEnabled;

            set
            {
                if (_isExportCsvButtonEnabled != value)
                {
                    _isExportCsvButtonEnabled = value; OnPropertyChanged();
                }
            }
        }

        protected Dictionary<string, DefectBin> DefectBinDictionary { get; private set; }

        private void OnChangeSelectedResultFullName(DisplaySelectedResultFullNameMessage msg)
        {
            SelectedResultFullName = msg.SelectedResultFullName;
        }

        public abstract AutoRelayCommand CommandExportCSV { get; }

        #endregion Properties

        #region Selections

        private ResultValueType _selectedResValueType;

        public ResultValueType SelectedResValueType
        {
            get => _selectedResValueType;
            set
            {
                if (_selectedResValueType == value) return;

                _selectedResValueType = value;
                StatsFactory.LastResultValueType = value;
                UpdateStatsVM();
                OnPropertyChanged();
            }
        }

        private KeyValuePair<HistogramType, string> _selectedHistogram;

        public KeyValuePair<HistogramType, string> SelectedHistogram
        {
            get => _selectedHistogram;
            set
            {
                if (_selectedHistogram.Key == value.Key) return;

                _selectedHistogram = value;
                StatsFactory.LastSelectedHistogram = value;

                HistogramVM = _histogramDictionary[_selectedHistogram.Key];
                HistogramVM.Generate(_lotStatsData[SelectedResValueType], DefectBinDictionary);
                HistogramVM.UpdateUnits(SelectedResValueType);
                OnPropertyChanged();
            }
        }

        #endregion Selections

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DefectVM()
        {
            HistogramTypes = StatsFactory.GetEnumsWithDescriptions<HistogramType>();
            _gridStatsVM = new GridStatsVM();
            ResultValueTypes = new List<ResultValueType>
            {
                ResultValueType.Count,
                ResultValueType.AreaSize
            };
            DefectBinDictionary = new Dictionary<string, DefectBin>();
            BuildDictionaries();

            LotViews = StatsFactory.GetEnumsWithDescriptions<LotView>();
            LotSelectedView = new KeyValuePair<LotView, string>();

            Messenger.Register<DisplaySelectedResultFullNameMessage>(this, (r, m) => OnChangeSelectedResultFullName(m));
        }

        protected override void OnDeactivated()
        {
            Messenger.Unregister<DisplaySelectedResultFullNameMessage>(this);
            base.OnDeactivated();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Met à jour les stats en fonction du type de résultat selectionné.
        /// </summary>
        /// <param name="stats"></param>
        public override void UpdateStats(object stats)
        {
            ExtractStatsDataFromLot((WaferResultData[])stats);
            UpdateStatsVM();
        }

        public override void SelectLotView(object lotview)
        {
            var lv = (LotView)lotview;
            LotSelectedView = LotViews.FirstOrDefault(x => x.Key == lv);
        }

        /// <summary>
        /// Génère les données servant pour les statistiques.
        /// </summary>
        /// <param name="slotsdata"></param>
        private void ExtractStatsDataFromLot(WaferResultData[] slotsdata)
        {
            var tempDico = new Dictionary<string, DefectBin>();
            ResetDictionaries();
            // Get stats data
            if (slotsdata != null && slotsdata.Length > 0)
            {
                var resultItems = slotsdata.Where(x => x != null && x.ResultItem != null && x.ResultItem.ResultItemValues.Count > 0)
                                       .Select(x => x.ResultItem).ToList();
                if (resultItems.Count > 0)
                {
                    bool isKlarfResult = resultItems.FirstOrDefault()?.ResultTypeEnum.GetResultFormat() == ResultFormat.Klarf;
                    if (!isKlarfResult) DefectBinDictionary.Clear();

                    foreach (var resItem in resultItems)
                    {
                        foreach (var resultItemValue in resItem.ResultItemValues)
                        {
                            var resultValueType = (ResultValueType)resultItemValue.Type;

                            // Fill legend
                            if (!isKlarfResult)
                            {
                                if (!DefectBinDictionary.Keys.Contains(resultItemValue.Name))
                                {
                                    var defectBin = new DefectBin
                                    {
                                        Label = resultItemValue.Name,
                                        Color = Color.Transparent.ToArgb()
                                    };
                                    DefectBinDictionary.Add(defectBin.Label, defectBin);
                                }

                                if (resultValueType == ResultValueType.Color)
                                {
                                    if (DefectBinDictionary[resultItemValue.Name].Color == Color.Transparent.ToArgb())
                                    {
                                        DefectBinDictionary[resultItemValue.Name].Color = (int)resultItemValue.Value;
                                    }
                                }
                            }
                            else
                            {
                                if (!tempDico.Keys.Contains(resultItemValue.Name))
                                {
                                    DefectBin defectBin;
                                    if (DefectBinDictionary.Keys.Contains(resultItemValue.Name))
                                        defectBin = DefectBinDictionary[resultItemValue.Name];
                                    else
                                        defectBin = new DefectBin
                                        {
                                            RoughBin = Convert.ToInt16(resultItemValue.Name),
                                            Label = "None _" + resultItemValue.Name,
                                            Color = 16777215
                                        };
                                    tempDico.Add(defectBin.RoughBin.ToString(), defectBin);
                                }
                            }

                            // fill value stats
                            var waferStatsData = new WaferStatsData
                            {
                                SlotId = resItem.Result.WaferResult.SlotId,
                                State = (ResultState)resItem.State,
                                ResultValue = resultItemValue
                            };
                            _lotStatsData[resultValueType].Add(waferStatsData);
                        }
                    }
                    if (isKlarfResult)
                        DefectBinDictionary = tempDico;

                    // TODO The dictionary is not sorted since the OrderBy method returns a new collection.
                    DefectBinDictionary.OrderBy(x => x.Key);
                }
            }
        }

        /// <summary>
        /// Met à jour la vue statistique en fonction  du pm selctionné ou du type de resultValue selectionné.
        /// </summary>
        private void UpdateStatsVM()
        {
            // Generate grid stats
            var statsData = _lotStatsData[StatsFactory.LastResultValueType];
            _gridStatsVM.Generate(statsData, DefectBinDictionary);
            // Generate histogram
            _selectedHistogram = HistogramTypes.FirstOrDefault(x => x.Key == StatsFactory.LastSelectedHistogram.Key);
            HistogramVM = _histogramDictionary[_selectedHistogram.Key];
            HistogramVM.Generate(statsData, DefectBinDictionary);
            SelectedResValueType = StatsFactory.LastResultValueType;
            HistogramVM.UpdateUnits(SelectedResValueType);
            // CSV button
            IsExportCsvButtonEnabled = statsData.Count > 0;
        }

        /// <summary>
        /// Initialize histogram and resultValue type dictionnaries.
        /// </summary>
        private void BuildDictionaries()
        {
            // Buid dico for histograms
            _histogramDictionary = new Dictionary<HistogramType, HistogramVMBase>();
            foreach (var histoTYpe in (HistogramType[])Enum.GetValues(typeof(HistogramType)))
            {
                _histogramDictionary.Add(histoTYpe, StatsFactory.GetCurrentHistogram(histoTYpe));
            }
            // Buid dico for ResultValue type
            _lotStatsData = new Dictionary<ResultValueType, List<WaferStatsData>>();
            foreach (var resValue in (ResultValueType[])Enum.GetValues(typeof(ResultValueType)))
            {
                _lotStatsData.Add(resValue, new List<WaferStatsData>());
            }
        }

        /// <summary>
        /// Reset dictionnaries values
        /// </summary>
        private void ResetDictionaries()
        {
            foreach (var val in _lotStatsData.Values)
            {
                val.Clear();
            }
        }

        /// <summary>
        /// Export statistics data in CSV format.
        /// </summary>
        public void ExportCSV()
        {
            var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
            var dialog = new SaveFileDialog
            {
                Filter = @"Text Files (*.csv) | *.csv"  // Only csv file accepted ?
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;

                try
                {
                    using (var sw = new StreamWriter(filePath, false))
                    {
                        var dt = GridStatsVM.DataView.Table;
                        var stringBulder = new StringBuilder();

                        foreach (DataColumn col in dt.Columns)
                            stringBulder.Append($";{col.ColumnName}");
                        // Write CSV file header content
                        sw.WriteLine(stringBulder.ToString().Trim(';'));

                        // Write datable rows content in csv file
                        foreach (DataRow row in dt.Rows)
                        {
                            stringBulder = new StringBuilder(row[0].ToString());
                            for (int j = 1; j < dt.Columns.Count; j++)
                                stringBulder.Append($";{row[j]}");
                            // Write row data in csv file
                            sw.WriteLine(stringBulder);
                        }
                        sw.Close();
                        notifierVM.AddMessage(new Shared.Tools.Service.Message(MessageLevel.Information, "File: " + filePath + " was saved with success"));
                    }
                }
                catch (IOException)
                {
                    notifierVM.AddMessage(new Shared.Tools.Service.Message(MessageLevel.Error, "Destination file is opened. Please close it and retry !"));
                }
                catch (Exception ex)
                {
                    notifierVM.AddMessage(new Shared.Tools.Service.Message(MessageLevel.Error, ex.Message));
                }
            }
        }

        #endregion Methods
    }
}
