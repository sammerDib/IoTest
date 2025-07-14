using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Tools;

namespace UnitySC.Result.CommonUI.ViewModel.LotWafer
{
    public class LotWaferSlotVM : ObservableRecipient
    {
        #region Properties

        private readonly IMessenger _messenger;

        protected override void OnDeactivated()
        {
            _messenger.Unregister<ResultStateNotificationMessage>(this);
  
            base.OnDeactivated();
        }

        public void Cleanup()
        {
            OnDeactivated();
        }

        public void UnRegisterMessenger()
        {
            _messenger.Unregister<ResultStateNotificationMessage>(this);
        }

        private bool _isResultNotNull;

        public bool IsResultExist
        {
            get => _isResultNotNull; set { if (_isResultNotNull != value) { _isResultNotNull = value; OnPropertyChanged(); } }
        }

        private IResultDataObject _dataObj;

        public IResultDataObject ResultDataObj
        {
            get => _dataObj; set { if (_dataObj != value) { _dataObj = value; OnPropertyChanged(); } }
        }

        public int SlotID { get; set; }

        public int SlotIndex { get; set; }

        public void UpdateSlotVM(LotWaferSlotVM slotvm)
        {
            ResultDataObj = slotvm.ResultDataObj;
            ImageThumbnail = slotvm.ImageThumbnail;
            Item = slotvm.Item;
            StopRotate = slotvm.StopRotate;

            //// RTI - for debug get image path
            //if ((ImageThumbnail as BitmapImage) != null)
            //{
            //    var bi = (ImageThumbnail as BitmapImage);
            //    string pathfs = (bi.StreamSource as FileStream)?.Name;
            //    string pathuri = bi.UriSource?.LocalPath;
            //}

        }

        // very specif set to assure coherence of slotvm for onaddresult event... use very carefully !!! -- need to call SetResult_Async after
        public void SetResult_ByPass(DataAccess.Dto.ResultItem dtoresult)
        {
            if (dtoresult == null)
                _dtoItem = null;
            else 
                _dtoItem = new LotItem(dtoresult);
            _isResultNotNull = _dtoItem != null;
        }

        private LotItem _dtoItem;

        public LotItem Item
        {
            get => _dtoItem;
            set
            {
                if (_dtoItem != value)
                {
                    _dtoItem = value;
                    OnPropertyChanged();

                    IsResultExist = _dtoItem != null;

                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(InternalState));
                    OnPropertyChanged(nameof(WaferName));

                    UpdateInfo();
                }
            }
        }

        private string _info = null;
        public string Info
        {
            get
            {
                return _info;
            }
            set
            {
                if (value != _info)
                {
                    _info = (string)value;
                    OnPropertyChanged();
                }  
            }
        }


        public ResultState State
        {
            get
            {
                return (_dtoItem == null) ? ResultState.NotProcess : (ResultState)_dtoItem.State;
            }
            set
            {
                if (_dtoItem != null)
                {
                    if (value != (ResultState)_dtoItem.State)
                    {
                        _dtoItem.State = (int)value;
                        OnPropertyChanged();

                        UpdateInfo();
                    }
                }
            }
        }

        public ResultInternalState InternalState
        {
            get
            {
                return (_dtoItem == null) ? ResultInternalState.NotProcess : (ResultInternalState)_dtoItem.InternalState;
            }
            set
            {
                if (_dtoItem != null)
                {
                    if (value != (ResultInternalState)_dtoItem.InternalState)
                    {
                        _dtoItem.InternalState = (int)value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public string WaferName
        {
            get
            {
                return (_dtoItem == null) ? SlotID.ToString() : string.Format("{0} - {1}", SlotID, _dtoItem.WaferName);
            }
        }

        private ImageSource _imgThumbnail;

        public ImageSource ImageThumbnail
        {
            get => _imgThumbnail;
            set
            {
                if (_imgThumbnail != value)
                {
                    _imgThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _stopRotate;

        public bool StopRotate
        {
            get => _stopRotate; set { if (_stopRotate != value) { _stopRotate = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="slotID"></param>
        public LotWaferSlotVM(int slotID = 0)
            : base()
        {
            SlotID = slotID;
            SlotIndex = SlotID - 1;
            _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferNotProcess");
            _isResultNotNull = false;

            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _messenger.Register<ResultStateNotificationMessage>(this, (r, m) => OnResultStateChanged(m));
        }

        #endregion Constructor

        #region Methods

        private async void OnResultStateChanged(ResultStateNotificationMessage msg)
        {
            if (_dtoItem != null)
            {
                if (_dtoItem.Id == msg.ResultID)
                {
                    _dtoItem.State = msg.State;
                    _dtoItem.InternalState = msg.InternalState;

                    /*System.Diagnostics.Debug.WriteLine($"*** {System.DateTime.Now.ToString("HH: mm:ss.fff")} Notif reschange <{_dtoItem.FileName}> :" +
                        $"\n   State : {_dtoItem.State} => {msg.State}" +
                        $"\n   InternalState : {_dtoItem.State} => {msg.InternalState}");*/

                    await Task.Run(() =>
                    {
                        if (_isResultNotNull)
                        {
                            UpdateThumbnail_async();
                        }

                        UpdateInfo();

                        // Notify UI
                        //..........
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            OnPropertyChanged(nameof(State));
                            OnPropertyChanged(nameof(InternalState));
                            OnPropertyChanged(nameof(Info));
                        });
                    });
                }
            }
        }

        private void UpdateThumbnail_async(bool dirNotAccessible = false)
        {
            bool bUseWaitingImageForLoadingwait = true;

            bool skipfinalupdate = false;
            _stopRotate = false;
            if (_dtoItem == null)
            {
                _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferNotProcess");
            }
            else
            {
                if (_dtoItem.State >= 0)
                {
                    if (bUseWaitingImageForLoadingwait)
                    { // Load wait image while searching image
                        _stopRotate = true;
                        _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferWait");

                        // Notify UI
                        //..........
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            OnPropertyChanged(nameof(StopRotate));
                            OnPropertyChanged(nameof(ImageThumbnail));
                        });
                    }

                    if (dirNotAccessible)
                    {
                        _stopRotate = false;
                        _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferErrorWarning");
                    }
                    else if (string.IsNullOrEmpty(_dtoItem.ResPath) || !File.Exists(_dtoItem.ResPath))
                    {
                        _stopRotate = false;
                        _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferErrorWarning");
                    }
                    else
                    {
                        switch (InternalState)
                        {
                            case ResultInternalState.Ok:
                                {
                                    try
                                    {

                                        if (!string.IsNullOrEmpty(_dtoItem.ResThumbnailPath))
                                        {

                                            if (bUseWaitingImageForLoadingwait)
                                            {
                                                skipfinalupdate = true;
                                            }

                                            Task.Run(() =>
                                            {
                                                //Task.Delay(2000).Wait(); 
                                                try
                                                {
                                                    var bi = new BitmapImage();
                                                    bi.BeginInit();
                                                    bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                                                    bi.CacheOption = BitmapCacheOption.OnLoad;
                                                    bi.UriSource = new Uri(_dtoItem.ResThumbnailPath);
                                                    bi.EndInit();

                                                    bi.Freeze();
                                                    StopRotate = false;
                                                    ImageThumbnail = bi;
                                                }
                                                catch 
                                                {
                                                    StopRotate = false;
                                                    ImageThumbnail = (ImageSource)Application.Current.FindResource("ResWaferWarning");
                                                }
                                            });
                                        }
                                        else
                                        {
                                            _stopRotate = false;
                                            _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferWarning");
                                        }
                                    }
                                    catch
                                    {
                                        skipfinalupdate = false;
                                        _stopRotate = false;
                                        _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferWarning");
                                    }
                                }
                                break;

                            case ResultInternalState.NotProcess:
                                if (bUseWaitingImageForLoadingwait)
                                {
                                    skipfinalupdate = true;
                                    // already loaded we can skip raise property
                                }
                                else
                                {
                                    _stopRotate = true;
                                    _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferWait");
                                }
                                break;

                            case ResultInternalState.Error:
                                _stopRotate = false;
                                _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferErrorWarning");
                                break;
                        }
                    }
                }
                else if (State <= ResultState.Error)
                {
                    // all ResultState Errors
                    _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferError");
                }
                else if (State == ResultState.NotProcess)
                {
                    _imgThumbnail = (ImageSource)Application.Current.FindResource("ResWaferProcessing");
                }
            }

            if (!skipfinalupdate)
            {
                // Notify UI
                //..........
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(StopRotate));
                    OnPropertyChanged(nameof(ImageThumbnail));
                });
            }
        }

        public void SetResult_Async(LotItem item, bool dirNotAccessible = false)
        {
            _dtoItem = item;
            _isResultNotNull = _dtoItem != null;
            if (_isResultNotNull)
            {
                 Task.Run(() => UpdateThumbnail_async(dirNotAccessible));
            }

            UpdateInfo();

            // Notify UI
            //..........
            Application.Current?.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(Item));
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(InternalState));
                OnPropertyChanged(nameof(WaferName));

                OnPropertyChanged(nameof(IsResultExist));
                OnPropertyChanged(nameof(Info));
            });
        }

        private void UpdateInfo()
        {
            string infomessage = string.Empty;
            if (_isResultNotNull && State != ResultState.Ok)
            {
                infomessage = State.ToHumanizedString();
            }
            Info = infomessage;
        }

        #endregion Methods
    }
}
