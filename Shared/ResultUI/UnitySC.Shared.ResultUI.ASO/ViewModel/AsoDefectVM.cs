using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using AutoMapper.Internal;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.ASO;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.ASO.ViewModel
{
    public class AsoDefectVM : DefectVMBase
    {
        private readonly IMessenger _messenger;
        private const ResultFormat FormatToken = ResultFormat.ASO;

        #region Constructor

        public AsoDefectVM(AsoResultVM parentVM)
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            DefectClusterImages = new ObservableCollection<Thumbnail>();

            ParentVM = parentVM;

            var data = ParentVM.ResultDataObj as DataAso;
            IsDieMode = data.UseDieGridDisplay;
        }

        #endregion Constructor

        #region Properties

        private AsoDefect _asoDefectObj;
        public AsoDefect AsoDefectObj
        {
            get => _asoDefectObj;
            set
            {
                if (_asoDefectObj != value)
                {
                    _asoDefectObj = value;
                    OnPropertyChanged();

                    if (_asoDefectObj == null)
                    {
                        InitDefectData();
                    }
                    else
                    {
                        Id = _asoDefectObj.DefectId;

                        PositionX = _asoDefectObj.Defect.MicronPositionX;
                        PositionY = _asoDefectObj.Defect.MicronPositionY;

                        SizeX = _asoDefectObj.Defect.MicronSizeX;
                        SizeY = _asoDefectObj.Defect.MicronSizeY;

                        ClusterNumber = _asoDefectObj.Defect.ClusterNumber;
                        Class = _asoDefectObj.Defect.CustomerReportLabel;
                        ColorCategory = _asoDefectObj.Defect.Color;
                        DefCount = _asoDefectObj.Defect.NumberOfDefect;
                        ReportType = _asoDefectObj.Defect.ReportTypeForSize;
                        UnitUsed = _asoDefectObj.Defect.UnitUsed;

                        ClusterMaxSize = _asoDefectObj.Defect.MaxClusterSize;
                        TotalClusterSize = _asoDefectObj.Defect.TotalclusterSize;

                        MosaicColumn = _asoDefectObj.Defect.SrcImageMosaic_Column;
                        MosaicLine = _asoDefectObj.Defect.SrcImageMosaic_Line;
                        BlocNumber = _asoDefectObj.Defect.BlocNumber;


                        if (IsDieMode)
                        {
                            // Die mode specific
                            DieIndexX = _asoDefectObj.Defect.SrcImageMosaic_DieX;
                            DieIndexY = _asoDefectObj.Defect.SrcImageMosaic_DieY;

                            var data = ParentVM.ResultDataObj as DataAso;
                            PositioninDieX = PositionX - (data.DieOriginX + DieIndexX * data.DiePitchX + data.WaferSizeX_mm * 500.0);
                            PositioninDieY = PositionY - (-data.DieOriginY + DieIndexY * (-1.0) * data.DiePitchY + data.WaferSizeY_mm * 500.0); // a valider avec des data connu (sens des aces et point origin a confirmer)
                        }
                    }

                }
            }
        }

        private double _positionX;

        public double PositionX
        {
            get => _positionX;
            set
            {
                if (_positionX != value)
                {
                    _positionX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _positionY;

        public double PositionY
        {
            get => _positionY;
            set
            {
                if (_positionY != value)
                {
                    _positionY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _sizeX;

        public double SizeX
        {
            get => _sizeX;
            set
            {
                if (_sizeX != value)
                {
                    _sizeX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _sizeY;

        public double SizeY
        {
            get => _sizeY;

            set
            {
                if (_sizeY != value)
                {
                    _sizeY = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _custerNumber;
        public int ClusterNumber
        {
            get => _custerNumber;
            set
            {
                if (_custerNumber != value)
                {
                    _custerNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _class;

        public string Class
        {
            get => _class;
            set
            {
                if (_class != value)
                {
                    _class = value;
                    OnPropertyChanged();
                }
            }
        }

        private System.Drawing.Color _colorcategory;

        public System.Drawing.Color ColorCategory
        {
            get => _colorcategory;
            set
            {
                if (_colorcategory != value)
                {
                    _colorcategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isDieMode;

        public bool IsDieMode
        {
            get => _isDieMode;
            set
            {
                if (_isDieMode != value)
                {
                    _isDieMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _dieIndexX;

        public int DieIndexX
        {
            get => _dieIndexX;
            set
            {
                if (_dieIndexX != value)
                {
                    _dieIndexX = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _dieIndexY;

        public int DieIndexY
        {
            get => _dieIndexY;
            set
            {
                if (_dieIndexY != value)
                {
                    _dieIndexY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _positioninDieX;

        public double PositioninDieX
        {
            get => _positioninDieX;
            set
            {
                if (_positioninDieX != value)
                {
                    _positioninDieX = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _positioninDieY;

        public double PositioninDieY
        {
            get => _positioninDieY;
            set
            {
                if (_positioninDieY != value)
                {
                    _positioninDieY = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _clusterMaxSize;

        public double ClusterMaxSize
        {
            get => _clusterMaxSize;
            set
            {
                if (_clusterMaxSize != value)
                {
                    _clusterMaxSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _totalClusterSize;

        public double TotalClusterSize
        {
            get => _totalClusterSize;
            set
            {
                if (_totalClusterSize != value)
                {
                    _totalClusterSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _defCount;

        public int DefCount
        {
            get => _defCount;
            set
            {
                if (_defCount != value)
                {
                    _defCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _unitUsed;

        public string UnitUsed
        {
            get => _unitUsed;
            set
            {
                if (_unitUsed != value)
                {
                    _unitUsed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _reportType;

        public string ReportType
        {
            get => _reportType;
            set
            {
                if (_reportType != value)
                {
                    _reportType = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _mosaicColumn;

        public int MosaicColumn
        {
            get => _mosaicColumn;
            set
            {
                if (_mosaicColumn != value)
                {
                    _mosaicColumn = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _mosaicLine;

        public int MosaicLine
        {
            get => _mosaicLine;
            set
            {
                if (_mosaicLine != value)
                {
                    _mosaicLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _blocNumber;
        public int BlocNumber
        {
            get => _blocNumber;
            set
            {
                if (_blocNumber != value)
                {
                    _blocNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private AsoResultVM _parentVM;
        public AsoResultVM ParentVM
        {
            get => _parentVM;
            set
            {
                if (_parentVM != value)
                {
                    _parentVM = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _nextButtonClicked;
        public bool NextButtonClicked
        {
            get => _nextButtonClicked;
            set
            {
                if (_nextButtonClicked != value)
                {
                    _nextButtonClicked = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _enabledNextButton;
        public bool EnabledNextButton
        {
            get => _enabledNextButton;
            set
            {
                if (_enabledNextButton != value)
                {
                    _enabledNextButton = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _enablePreviousButton;
        public bool EnablePreviousButton
        {
            get => _enablePreviousButton;
            set
            {
                if (_enablePreviousButton != value)
                {
                    _enablePreviousButton = value;
                    OnPropertyChanged();
                }
            }
        }



        #endregion Properties

        #region Methods

        /// <summary>
        /// Change current defect.
        /// </summary>
        /// <param name="defectId"></param>
        public override void Change(int defectId)
        {
            _messenger.Send(new DefectActionMessage(DefectActionEnum.ChangeCurrentDefect, defectId), FormatToken.ToString());
        }

        /// <summary>
        /// Change current defect.
        /// </summary>
        /// <param name="defectId"></param>
        public override void Change(object defectobj)
        {
            var asodefect = defectobj as AsoDefect;
            if (asodefect != null)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    DefectClusterImages.Clear();
                });

                HasThumbnails = false;

                AsoDefectObj = asodefect;

                Features = AsoDefectObj.Defect.CharacFeatures;

                EnabledNextButton = ParentVM.Defects != null && Id < ParentVM.MaxIdDefect;
                EnablePreviousButton = ParentVM.Defects != null && Id > ParentVM.MinIdDefect;

                // Update list of images matching the defect Id
                // Add images with same defectId to defect detail view
                ParentVM.Defects.ActiveThumbnails.ForAll(_ => _.IsSelected = false);

                var activesImages = ParentVM.Defects.ActiveThumbnails.Where(_ => _.DefectId == Id);
                HasThumbnails = activesImages.Any();

                foreach (var item in activesImages)
                {
                    //item.IsSelected = true;

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        DefectClusterImages.Add(item);
                    });
                }

                foreach (var item in activesImages)
                {
                    item.IsSelected = true;
                }

                _messenger.Send(new UpdateRectHiliteMessage(true, asodefect), FormatToken.ToString());
            }
            else
            {
                _messenger.Send(new UpdateRectHiliteMessage(false), FormatToken.ToString());
            }
        }

        public void InitDefectData()
        {
            Id = 0;
            PositionX = 0.0;
            PositionY = 0.0;
            SizeX = 0.0;
            SizeY = 0.0;
            DefCount = 0;
            ClusterNumber = 0;
            ClusterMaxSize = 0.0;
            TotalClusterSize = 0.0;

            Class = string.Empty;
            UnitUsed = string.Empty;
            ReportType = string.Empty;

            PositioninDieX = 0;
            PositioninDieY = 0;
            DieIndexX = 0;
            DieIndexY = 0;
            MosaicColumn = 0;
            MosaicLine = 0;
            BlocNumber = 0;

            Features = new Dictionary<string, string>();

            Application.Current?.Dispatcher.Invoke(() =>
            {
                DefectClusterImages.Clear();
            });

            _messenger.Send(new UpdateRectHiliteMessage(false));
        }

        #endregion Methods

        #region Commands

        /// <summary>
        /// Command Previous button click.
        /// </summary>
        private AutoRelayCommand _getPreviousDefect;
        public AutoRelayCommand GetPreviousDefect => _getPreviousDefect ?? (_getPreviousDefect = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        NextButtonClicked = false;
                        _messenger.Send(new DefectActionMessage(DefectActionEnum.NavigateToDefect), FormatToken.ToString());
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                },
                () => true));

        /// <summary>
        /// Command Next button click.
        /// </summary>
        private AutoRelayCommand _getNextDefect;
        public AutoRelayCommand GetNextDefect => _getNextDefect ?? (_getNextDefect = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        NextButtonClicked = true;
                        _messenger.Send(new DefectActionMessage(DefectActionEnum.NavigateToDefect), FormatToken.ToString());
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                },
                () => true));

        // Defects cluster (grey+binary) searched by its number in research
        private int _searchClusterNumber;
        public int SearchClusterNumber
        {
            get => _searchClusterNumber;
            set => SetProperty(ref _searchClusterNumber, value);
        }

        private AutoRelayCommand _searchDefectbyClusterNumber;
        public AutoRelayCommand SearchDefectByClusterNumber => _searchDefectbyClusterNumber ?? (_searchDefectbyClusterNumber = new AutoRelayCommand(
                () =>
                {
                    try
                    {
                        _messenger.Send(new DefectActionMessage(DefectActionEnum.SearchDefectByClusterNumber, SearchClusterNumber), FormatToken.ToString());
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                },
                () => true));

        #endregion Commands
    }
}
