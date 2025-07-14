using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using AutoMapper.Internal;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format._001;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.ResultUI.Common.ViewModel.Defect;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Klarf.ViewModel
{
    public class KlarfDefectVM : DefectVMBase
    {
        private readonly IMessenger _messenger;
        private const ResultFormat FormatToken = ResultFormat.Klarf;

        #region Properties

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

        private int _clusterNumber;
        public int ClusterNumber
        {
            get => _clusterNumber;
            set
            {
                if (_clusterNumber != value)
                {
                    _clusterNumber = value;
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

        private bool _isDieKlarf;

        public bool IsDieKlarf
        {
            get => _isDieKlarf;
            set
            {
                if (_isDieKlarf != value)
                {
                    _isDieKlarf = value;
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

        private int _idCode;

        public int IDcode
        {
            get => _idCode; set { if (_idCode != value) { _idCode = value; OnPropertyChanged(); } }
        }

        private KlarfResultVM _parentVM;
        public KlarfResultVM ParentVM
        {
            get => _parentVM;
            set
            {
                if (_parentVM != value)
                {
                    _parentVM = value;
                    OnPropertyChanged();
                    //OnPropertyChanged(nameof(_parentVM.SearchClusterId));
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

        #region Constructor
        public KlarfDefectVM(KlarfResultVM parentVM) : base()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            DefectClusterImages = new ObservableCollection<Thumbnail>();

            ParentVM = parentVM;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Change le défaut courant.
        /// </summary>
        /// <param name="defectId"></param>
        public override void Change(int defectId)
        {
            _messenger.Send(new DefectActionMessage(DefectActionEnum.ChangeCurrentDefect, defectId), FormatToken.ToString());
        }

        /// <summary>
        /// Change le défaut courant.
        /// </summary>
        /// <param name="defectId"></param>
        public override void Change(object defectobj)
        {
            var klarfdefect = defectobj as KlarfDefect;
            if (klarfdefect != null)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    DefectClusterImages.Clear();
                });

                HasThumbnails = false;

                Init(klarfdefect.Defect, ParentVM.IsDisplayGridEnabled);
                Class = ParentVM.RoughBintoClass(IDcode);
                ColorCategory = ParentVM.RoughBintoColor(IDcode);
                ParentVM.IsSearchClusterDone = false;

                // Update list of images matching the defect Id
                // Add images with same defectId to defect detail view
                ParentVM.Defects.ActiveThumbnails.ForAll(_ => _.IsSelected = false);

                var activesImages = ParentVM.Defects.ActiveThumbnails.Where(_ => _.DefectId == Id);

                HasThumbnails = activesImages.Any();

                foreach (var item in activesImages)
                {
                    //  item.IsSelected = true;

                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        DefectClusterImages.Add(item);
                    });
                }

                foreach (var item in activesImages)
                {
                    item.IsSelected = true;
                }

                _messenger.Send(new UpdateRectHiliteMessage(true, klarfdefect), FormatToken.ToString());
            }
            else
            {
                _messenger.Send(new UpdateRectHiliteMessage(false), FormatToken.ToString());
            }
        }

        /// <summary>
        /// Init defect data.
        /// </summary>
        /// <param name="defect"></param>
        public void Init(PrmDefect defect, bool isDieGrieEnabled)
        {
            Id = (int)defect.Get("DEFECTID");
            PositionX = (double)defect.Get("XREL");
            PositionY = (double)defect.Get("YREL");
            SizeX = (double)defect.Get("XSIZE");
            SizeY = (double)defect.Get("YSIZE");
            ClusterNumber = (int)defect.Get("CLUSTERNUMBER");
            // Class = ((int)defect.Get("CLASSNUMBER")).ToString();// TO DO: Recupere le nom de la classe à partir de l'id.
            IDcode = (int)defect.Get("ROUGHBINNUMBER");
            Features = defect.GetFeaturesDico();

            IsDieKlarf = isDieGrieEnabled;
            if (IsDieKlarf)
            {
                DieIndexX = (int)defect.Get("XINDEX");
                DieIndexY = (int)defect.Get("YINDEX");
            }

            // Gestion de l'activation du bouton Previous et Next
            EnabledNextButton = ParentVM.Defects != null && Id < ParentVM.MaxIdDefect;
            EnablePreviousButton = ParentVM.Defects != null && Id > ParentVM.MinIdDefect;
        }

        public void InitDefectData()
        {
            Id = 0;
            PositionX = 0;
            PositionY = 0;
            SizeX = 0;
            SizeY = 0;
            ClusterNumber = 0;
            Class = string.Empty;
            IDcode = -1;
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
        public AutoRelayCommand GetPreviousDefect
        {
            get
            {
                return _getPreviousDefect ?? (_getPreviousDefect = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      NextButtonClicked = false;
                      _messenger.Send(new DefectActionMessage(DefectActionEnum.NavigateToDefect), FormatToken.ToString());
                  }
                  catch (Exception)
                  {
                  }
              },
              () => { return true; }));
            }
        }

        /// <summary>
        /// Command Next button click.
        /// </summary>
        private AutoRelayCommand _getNextDefect;
        public AutoRelayCommand GetNextDefect
        {
            get
            {
                return _getNextDefect ?? (_getNextDefect = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      NextButtonClicked = true;
                      _messenger.Send(new DefectActionMessage(DefectActionEnum.NavigateToDefect), FormatToken.ToString());
                  }
                  catch (Exception)
                  {
                  }
              },
              () => { return true; }));
            }
        }

        // Defects cluster (grey+binary) searched by its number in research
        private int _searchClusterNumber;
        public int SearchClusterNumber
        {
            get => _searchClusterNumber;
            set => SetProperty(ref _searchClusterNumber, value);
        }
        private AutoRelayCommand _searchDefectbyClusterNumber;
        public AutoRelayCommand SearchDefectByClusterNumber
        {
            get
            {
                return _searchDefectbyClusterNumber ?? (_searchDefectbyClusterNumber = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      _messenger.Send(new DefectActionMessage(DefectActionEnum.SearchDefectByClusterNumber, SearchClusterNumber), FormatToken.ToString());
                  }
                  catch (Exception)
                  {
                  }
              },
              () => { return true; }));
            }
        }

        #endregion Commands
    }
}
