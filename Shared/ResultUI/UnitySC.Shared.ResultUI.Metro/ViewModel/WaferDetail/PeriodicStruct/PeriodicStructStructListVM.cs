using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.ResultUI.Common.Converters;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct.StructDetails;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct
{
    public class PeriodicStructStructListVM : ObservableObject
    {
        private const string AllRepetaName = "All";

        private List<PeriodicStructPointData> _currentPointDatas;

        private List<PeriodicStructListStructure> _structList;

        #region Properties

        private ObservableCollection<PeriodicStructListStructure> _currentStructList = new ObservableCollection<PeriodicStructListStructure>();

        public ObservableCollection<PeriodicStructListStructure> CurrentStructList
        {
            get => _currentStructList;
            private set
            {
                SetProperty(ref _currentStructList, value);
            }
        }

        private ObservableCollection<string> _repetaSource = new ObservableCollection<string>();

        public ObservableCollection<string> RepetaSource
        {
            get => _repetaSource;
            private set
            {
                SetProperty(ref _repetaSource, value);
            }
        }

        private string _selectedRepeta = AllRepetaName;

        public string SelectedRepeta
        {
            get => _selectedRepeta;
            set
            {
                if (SetProperty(ref _selectedRepeta, value))
                {
                    UpdateStructuresValues();
                }
            }
        }

        private int _digits;

        public int Digits
        {
            get { return _digits; }
            set
            {
                if (SetProperty(ref _digits, value))
                {
                    UpdateStructList();
                }
            }
        }

        #region Struct stats properties

        #region Height

        private string _maxHeight = "-";

        public string MaxHeight
        {
            get { return _maxHeight; }
            private set { SetProperty(ref _maxHeight, value); }
        }

        private string _minHeight = "-";

        public string MinHeight
        {
            get { return _minHeight; }
            private set { SetProperty(ref _minHeight, value); }
        }

        private string _meanHeight = "-";

        public string MeanHeight
        {
            get => _meanHeight;
            private set => SetProperty(ref _meanHeight, value);
        }

        private string _threeSigmaHeight = "-";

        public string ThreeSigmaHeight
        {
            get => _threeSigmaHeight;
            private set => SetProperty(ref _threeSigmaHeight, value);
        }
        #endregion

        #region Width

        private string _maxWidth = "-";

        public string MaxWidth
        {
            get { return _maxWidth; }
            private set { SetProperty(ref _maxWidth, value); }
        }

        private string _minWidth = "-";

        public string MinWidth
        {
            get { return _minWidth; }
            private set { SetProperty(ref _minWidth, value); }
        }

        private string _meanWidth = "-";

        public string MeanWidth
        {
            get { return _meanWidth; }
            private set { SetProperty(ref _meanWidth, value); }
        }

        private string _threeSigmaWidth = "-";

        public string ThreeSigmaWidth
        {
            get => _threeSigmaWidth;
            private set => SetProperty(ref _threeSigmaWidth, value);
        }

        #endregion

        #endregion

        public bool HideRepeta { get; private set; }

        #endregion

        #region private Method

        private void UpdateStructList()
        {
            _structList = CreateStructureCollection(_currentPointDatas);
            UpdateStructuresValues();
        }

        private void UpdateStructuresValues()
        {
            var selectedRepeta = _structList;

            if (SelectedRepeta.IsNullOrEmpty())
            {
                SelectedRepeta = RepetaSource.FirstOrDefault();
            }

            if (SelectedRepeta != AllRepetaName && !SelectedRepeta.IsNullOrEmpty())
            {
                selectedRepeta = _structList.Where(x => x.RepetaId.ToString() == SelectedRepeta).ToList();
            }

            CurrentStructList = new ObservableCollection<PeriodicStructListStructure>(selectedRepeta);

            MaxHeight = LengthToStringConverter.ConvertToString(selectedRepeta.Max(x => x.Height.IsNullOrEmpty() ? (double?) null : Convert.ToDouble(x.Height)), Digits, true, "-", LengthUnit.Micrometer);
            MinHeight = LengthToStringConverter.ConvertToString(selectedRepeta.Min(x => x.Height.IsNullOrEmpty() ? (double?)null : Convert.ToDouble(x.Height)), Digits, true, "-", LengthUnit.Micrometer);
            var listHeight = selectedRepeta.Select(s => Convert.ToDouble(s.Height)).ToList();
            double? meanHeight = GetMean(listHeight);
            MeanHeight = LengthToStringConverter.ConvertToString(meanHeight, Digits, true, "-", LengthUnit.Micrometer);
            ThreeSigmaHeight = LengthToStringConverter.ConvertToString(GetStandardDeviation(listHeight), Digits, true, "-", LengthUnit.Micrometer);

            MaxWidth = LengthToStringConverter.ConvertToString(selectedRepeta.Max(x => x.Width.IsNullOrEmpty() ? (double?)null : Convert.ToDouble(x.Width)), Digits, true, "-", LengthUnit.Micrometer);
            MinWidth = LengthToStringConverter.ConvertToString(selectedRepeta.Min(x => x.Width.IsNullOrEmpty() ? (double?)null : Convert.ToDouble(x.Width)), Digits, true, "-", LengthUnit.Micrometer);
            var listWidth = selectedRepeta.Select(s => Convert.ToDouble(s.Width)).ToList();
            double? meanWidth = GetMean(listWidth);
            MeanWidth = LengthToStringConverter.ConvertToString(meanWidth, Digits, true, "-", LengthUnit.Micrometer);
            ThreeSigmaWidth = LengthToStringConverter.ConvertToString(GetStandardDeviation(listWidth), Digits, true, "-", LengthUnit.Micrometer);

        }

        private List<PeriodicStructListStructure> CreateStructureCollection(List<PeriodicStructPointData> datas)
        {
            var structs = new List<PeriodicStructListStructure>();
            var repetaList = new List<string>();

            if (datas == null) return structs;

            _currentPointDatas = datas;

            repetaList.Add(AllRepetaName);

            int repetaCount = 1;
            foreach (var data in datas)
            {
                if (data.StructCount != data.StructHeights.Count && data.StructCount != data.StructWidths.Count) continue;

                for (int index = 0; index < data.StructCount; index++)
                {
                    structs.Add(new PeriodicStructListStructure(index + 1, repetaCount,
                        LengthToStringConverter.ConvertToString(data.StructHeights[index], Digits, false, "-", LengthUnit.Micrometer),
                        LengthToStringConverter.ConvertToString(data.StructWidths[index], Digits, false, "-", LengthUnit.Micrometer)));
                }

                repetaList.Add(repetaCount.ToString());
                repetaCount++;
            }

            CreateRepeta(repetaList);

            return structs;
        }

        private void CreateRepeta(List<string> repetaList)
        {
            if (RepetaSource.Count == repetaList.Count) return;

            RepetaSource.Clear();
            foreach (string repeta in repetaList)
            {
                RepetaSource.Add(repeta);
            }
        }

        private static double? GetMean(List<double> doubleList)
        {
            if (doubleList.IsNullOrEmpty()) return null;

            double sum = 0;
            foreach (double value in doubleList)
            {
                sum += value;
            }

            return sum / doubleList.Count;
        }

        private static double? GetStandardDeviation(IReadOnlyCollection<double> sequence)
        {
            if (sequence.IsNullOrEmpty()) return null;

            double average = sequence.Average();
            double sum = sequence.Sum(d => Math.Pow(d - average, 2));
            return Math.Sqrt(sum / sequence.Count);
        }

        #endregion

        #region public method

        public void SetStructList(MeasurePointResult point)
        {
            _currentPointDatas = point?.Datas.OfType<PeriodicStructPointData>().ToList();

            UpdateStructList();
        }

        public void UpdateColumnVisibility(bool hasRepeta)
        {
            HideRepeta = !hasRepeta;
            OnPropertyChanged(nameof(HideRepeta));
        }

        #endregion
    }
}
