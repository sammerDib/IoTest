using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Defect
{
    /// <summary>
    /// Base class for Klarf and Aso defects.
    /// </summary>
    public abstract class DefectVMBase : ObservableRecipient
    {
        #region Properties

        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Characteristics of the defect
        /// </summary>
        private Dictionary<string, string> _features = new Dictionary<string, string>();
        public Dictionary<string, string> Features
        {
            get => _features;
            set => SetProperty(ref _features, value);
        }

        // Cluster of both images (binary and grey) of selected defect
        private ObservableCollection<Thumbnail> _defectClusterImages;
        public ObservableCollection<Thumbnail> DefectClusterImages
        {
            get { return _defectClusterImages; }
            set { SetProperty(ref _defectClusterImages, value); }
        }


        /// <summary>
        /// Get if the defect has images.
        /// </summary>
        private bool _hasThumbnails;
        public bool HasThumbnails
        {
            get => _hasThumbnails;
            set
            {
                if (SetProperty(ref _hasThumbnails, value))
                {
                    ColSpan1 = _hasThumbnails ? 1 : 3;
                }
            }
        }

        private int _colspan1 = 3;

        public int ColSpan1
        {
            get => _colspan1;
            set => SetProperty(ref _colspan1, value);
        }

        #endregion

        public abstract void Change(int defectId);
        public abstract void Change(object defectobj);

    }
}
