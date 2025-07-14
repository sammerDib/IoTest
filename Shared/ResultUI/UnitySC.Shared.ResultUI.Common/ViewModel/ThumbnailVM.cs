using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

using Media = System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class ThumbnailVM : ObservableObject
    {
        #region Properties

        private Media.ImageSource _image;

        public Media.ImageSource Image
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                    ThumbnailDoubleClickCommand?.NotifyCanExecuteChanged();
                }
            }
        }

        private Media.Color _color;

        public Media.Color ColorCategory
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _label;

        public string LabelCategory
        {
            get => _label; set { if (_label != value) { _label = value; OnPropertyChanged(); } }
        }

        private string _title;

        public string Title
        {
            get => _title; set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }

        private int _defectId;

        public int DefectId
        {
            get => _defectId; set { if (_defectId != value) { _defectId = value; OnPropertyChanged(); } }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private bool _isActive; // Voir si c'est becessaire

        public bool IsActive
        {
            get => _isActive; set { if (_isActive != value) { _isActive = value; OnPropertyChanged(); } }
        }

        private int _index;

        public int Index
        {
            get => _index; set { if (_index != value) { _index = value; OnPropertyChanged(); } }
        }

        private string _imgfullpath;

        public string Imgfullpath
        {
            get => _imgfullpath; set { if (_imgfullpath != value) { _imgfullpath = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor 2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="color"></param>
        public ThumbnailVM(string title, int defectId, string catgName, System.Drawing.Color categColor, int index, string imgfullpath)
        {
            DefectId = defectId;
            Title = title;
            LabelCategory = catgName;
            ColorCategory = Media.Color.FromRgb(categColor.R, categColor.G, categColor.B);
            Index = index;
            Image = (Media.ImageSource)Application.Current?.FindResource("Image");

            Imgfullpath = imgfullpath;
        }

        #endregion Constructors

        #region Methods

        public void Update(Bitmap bmp)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Image = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                                              BitmapSizeOptions.FromEmptyOptions());
            });
        }

        public void UpdateImgSrc(ImageSource imgSrc)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Image = imgSrc;
            });
        }
        


        #endregion Methods

        #region Commands

        /// <summary>
        /// Navigation vers la pade de détail du wafer.
        /// </summary>
        private AutoRelayCommand _thumbnailDoubleClickCommand;

        public AutoRelayCommand ThumbnailDoubleClickCommand
        {
            get
            {
                return _thumbnailDoubleClickCommand ?? (_thumbnailDoubleClickCommand = new AutoRelayCommand(
              () =>
              {
                  //MessageBox.Show($"double click <{_title}>");
                  try
                  {
                      if (Extension.ImageJ_CheckValidity())
                          Extension.ImageJ((System.Windows.Media.Imaging.BitmapSource)Image);
                  }
                  catch (System.Exception ex)
                  {
                      //"Failed to export image to external viewer" -- viewer not set in app config ?
                      var notifierVM = ClassLocator.Default.GetInstance<NotifierVM>();
                      notifierVM.AddMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Warning, $"Failed to export image to external viewer\n Check if Debug.ImageViewer has been correctly set in appSettings config\n <{ex.Message}>"));
                  }
              },
              () => { return Image != null; }));
            }
        }
        #endregion Commands

    }
}
