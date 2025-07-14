using System;
using System.IO;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

using Media = System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class Thumbnail : ObservableObject
    {
        public Thumbnail(string path, int defectId, string categoryName, System.Drawing.Color categoryColor, int index)
        {
            DefectId = defectId;
            LabelCategory = categoryName;
            ColorCategory = Media.Color.FromRgb(categoryColor.R, categoryColor.G, categoryColor.B);
            Index = index;
            Imgfullpath = path;

            Title = Path.GetFileNameWithoutExtension(path);

            _source = new BitmapImage();
            _source.BeginInit();
            _source.UriSource = new Uri(path);
            _source.CacheOption = BitmapCacheOption.OnLoad;
            _source.EndInit();
            _source.Freeze();
        }

        public Thumbnail(string title, string path, int defectId, string categoryName, System.Drawing.Color categoryColor, int index)
        {
            DefectId = defectId;
            LabelCategory = categoryName;
            ColorCategory = Media.Color.FromRgb(categoryColor.R, categoryColor.G, categoryColor.B);
            Index = index;
            Imgfullpath = path;

            Title = title;

            try
            {
                // Read TIF file and update thumbnails
                using (var imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    BitmapSource bmpSource = decoder.Frames[index];
                    encoder.Frames.Add(BitmapFrame.Create(bmpSource));

                    using (var memoryStream = new MemoryStream())
                    {
                        encoder.Save(memoryStream);
                        memoryStream.Position = 0;

                        _source = new BitmapImage();
                        _source.BeginInit();
                        _source.CacheOption = BitmapCacheOption.OnLoad;
                        _source.StreamSource = memoryStream;
                        _source.EndInit();
                        _source.Freeze();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

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
                    GoToWaferDetailPageCommand?.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isActive; // Voir si c'est becessaire
        public bool IsActive
        {
            get => _isActive; set { if (_isActive != value) { _isActive = value; OnPropertyChanged(); } }
        }

        private string _imgfullpath;
        public string Imgfullpath
        {
            get => _imgfullpath; set { if (_imgfullpath != value) { _imgfullpath = value; OnPropertyChanged(); } }
        }

        private BitmapImage _source;
        public BitmapImage Source
        {
            get { return _source; }
            set { _source = value; }
        }

        private int _defectId;
        public int DefectId
        {
            get { return _defectId; }
            set { _defectId = value; }
        }

        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _labelCategory;
        public string LabelCategory
        {
            get { return _labelCategory; }
            set { _labelCategory = value; }
        }

        private Media.Color _colorCategory;
        public Media.Color ColorCategory
        {
            get => _colorCategory;
            set
            {
                if (_colorCategory != value)
                {
                    _colorCategory = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }



        #region Commands
        // Navigation vers la pade de détail du wafer.
        private AutoRelayCommand _goToWaferDetailPageCommand;

        public AutoRelayCommand GoToWaferDetailPageCommand
        {
            get
            {
                return _goToWaferDetailPageCommand ?? (_goToWaferDetailPageCommand = new AutoRelayCommand(
              () =>
              {
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
