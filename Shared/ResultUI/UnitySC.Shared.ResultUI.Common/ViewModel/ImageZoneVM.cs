using System;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class ImageZoneVM : ObservableRecipient, IDisposable
    {
        #region Properties
        private ImageSource _waferimgsrc;

        public ImageSource WaferImageSource
        {
            get => _waferimgsrc;
            set
            {
                if (_waferimgsrc != value)
                {
                    _waferimgsrc = value;
                    OnPropertyChanged();
                    if (_waferimgsrc == null)
                        HiliteRect = new Int32Rect(0, 0, 1, 1);
                }
            }
        }

        // Bitmap


        private int _rcX;
        public int HiliteRcX
        {
            get => _rcX;
            set => SetProperty(ref _rcX, value);
        }

        private int _rcY;

        public int HiliteRcY
        {
            get => _rcY;
            set => SetProperty(ref _rcY, value);
        }

        private int _rcW;

        public int HiliteRcW
        {
            get => _rcW;
            set => SetProperty(ref _rcW, value);
        }

        private int _rcH;

        public int HiliteRcH
        {
            get => _rcH;
            set => SetProperty(ref _rcH, value);
        }

        private bool _showHilite;

        public bool ShowHilite
        {
            get => _showHilite;
            set => SetProperty(ref _showHilite, value);
        }

        public Int32Rect HiliteRect
        {
            get => new Int32Rect(HiliteRcX, HiliteRcY, HiliteRcW, HiliteRcH);
            set
            {
                HiliteRcX = value.X;
                HiliteRcY = value.Y;
                HiliteRcW = value.Width;
                HiliteRcH = value.Height;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event Action<System.Windows.Point> OnClick;

        public event Action<System.Windows.Point> OnDoubleClick;

        #endregion

        #region Commands

        private AutoRelayCommand<System.Windows.Point> _clickcommand;

        public AutoRelayCommand<System.Windows.Point> ClickCommand => _clickcommand ?? (_clickcommand = new AutoRelayCommand<System.Windows.Point>(mouseposition => OnClick?.Invoke(mouseposition), mouseposition => true));

        private AutoRelayCommand<System.Windows.Point> _doublclickcommand;

        public AutoRelayCommand<System.Windows.Point> DoubleClickCommand => _doublclickcommand ?? (_doublclickcommand = new AutoRelayCommand<System.Windows.Point>(mouseposition => OnDoubleClick?.Invoke(mouseposition), mouseposition => true));

        #endregion

        public ImageZoneVM()
        {
        }

        #region IDisposable

        public void Dispose()
        {
            _clickcommand = null;
            _doublclickcommand = null;
            WaferImageSource = null;
        }

        #endregion
    }
}
