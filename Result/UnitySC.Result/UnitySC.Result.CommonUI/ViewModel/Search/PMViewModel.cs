using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class PMViewModel : ObservableRecipient, IDisposable
    {
        #region Properties

        private string _labelName;

        public string LabelName
        {
            get => _labelName;
            set
            {
                if (_labelName == value) return;
                _labelName = value;
                OnPropertyChanged();
            }
        }

        private ActorType _actorType;

        public ActorType ActorType
        {
            get => _actorType;
            set
            {
                if (_actorType == value) return;
                _actorType = value;
                OnPropertyChanged();
            }
        }

        private int _chamberId;

        public int ChamberId
        {
            get => _chamberId;
            set
            {
                if (_chamberId == value) return;
                _chamberId = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<PostProcessViewModel> _postprocessList;

        public ObservableCollection<PostProcessViewModel> PostProcessList
        {
            get => _postprocessList;
            set
            {
                if (_postprocessList == value) return;
                _postprocessList = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AcquisitionDataViewModel> _acquisitionList;

        public ObservableCollection<AcquisitionDataViewModel> AcquisitionList
        {
            get => _acquisitionList;
            set
            {
                if (_acquisitionList == value) return;
                _acquisitionList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public PMViewModel()
        {
            PostProcessList = new ObservableCollection<PostProcessViewModel>();
            AcquisitionList = new ObservableCollection<AcquisitionDataViewModel>();
        }

        public PMViewModel(ProcessModuleResult pmres)
        {
            ChamberId = pmres.ChamberId;
            ActorType = pmres.ActorType;
            LabelName = pmres.LabelPMName;

            // Post Process
            var postProcessList = new List<PostProcessViewModel>();
            foreach (string reslbl in pmres.PostProcessingResultLabels)
            {
                var waferResdataArray = pmres.PostProcessingResults[reslbl];
                var resType =  waferResdataArray.Where(x => x?.ResultItem != null)
                              .Select(x => x.ResultItem.ResultTypeEnum)
                              .DefaultIfEmpty(ResultType.Empty)
                              .First();
                postProcessList.Add(new PostProcessViewModel(reslbl, resType, waferResdataArray));
            }

            PostProcessList = new ObservableCollection<PostProcessViewModel>(postProcessList);

            // Acquisition
            var acquisitionList = new List<AcquisitionDataViewModel>();
            foreach (string acqlbl in pmres.AcquisitionResultsLabels)
            {
                var waferAcqDataArray = pmres.AcquisitionResults[acqlbl];
                var resType = waferAcqDataArray.Where(x => x?.AcqItem != null)
                              .Select(x => x.AcqItem.ResultTypeEnum)
                              .DefaultIfEmpty(ResultType.Empty)
                              .First();
                acquisitionList.Add(new AcquisitionDataViewModel(acqlbl, resType, waferAcqDataArray));
            }

            AcquisitionList = new ObservableCollection<AcquisitionDataViewModel>(acquisitionList);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the element corresponding to the previous selected result or returns the first element of the same type otherwise, returns the first present element.
        /// </summary>
        public PPViewModel GetFirstMatchingPostProcessViewModel(string resultLabelName, Type type)
        {
            // Get from post process list
            if (type == typeof(PostProcessViewModel) && PostProcessList.Any())
            {
                return PostProcessList.FirstOrDefault(x => x.ResultLabelName == resultLabelName) ?? PostProcessList.FirstOrDefault();
            }

            // Get from acquisition list
            if (type == typeof(AcquisitionDataViewModel) && AcquisitionList.Any())
            {
                return AcquisitionList.FirstOrDefault(x => x.ResultLabelName == resultLabelName) ?? AcquisitionList.FirstOrDefault();
            }

            // Returns the first element
            if (PostProcessList.Any()) return PostProcessList.FirstOrDefault();
            return AcquisitionList.FirstOrDefault();
        }

        public void Dispose()
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (AcquisitionList != null)
                    AcquisitionList.Clear();
                if (PostProcessList != null)
                    PostProcessList.Clear();
            });
        }

        #endregion
    }
}
