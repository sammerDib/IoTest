using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Microsoft.Win32;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace AdcBasicObjects.Rendering
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ImageRenderingViewModel : RenderingViewModelBase
    {
        private ProcessingClassMil3D _processingClassMil3D = new ProcessingClassMil3D();

        private class ImageIndexChangedMessage
        {
            public int ImageIndex;
        }

        public ImageRenderingViewModel(ModuleBase module) :
            base(module)
        {
            Module.Recipe.recipeExecutedEvent += (s, e) => { OnPropertyChanged(nameof(CurrentBitmap)); };
            // ClassLocator.Default.GetInstance<IMessenger>().Register<ImageIndexChangedMessage>(this, (r, m) =>
            Messenger.Register<ImageIndexChangedMessage>(this, (r, m) =>
            {
                _isUpdating = true;
                ImageIndex = m.ImageIndex;
                _isUpdating = false;
            });
        }

        //=================================================================
        // Propriétés
        //=================================================================
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        private int _imageWidth = 0;
        public int ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                _imageWidth = value;
                OnPropertyChanged();
            }
        }

        private int _imageHeight = 0;
        public int ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged();
            }
        }

        private static bool _isUpdating = false;
        private int _imageIndex = int.MinValue;
        private static int _defaultImageIndex = 0;
        public int ImageIndex
        {
            get
            {
                if (_imageIndex == int.MinValue)
                    ImageIndex = _defaultImageIndex;
                return _imageIndex;
            }
            set
            {
                if (value >= RenderingObjects.Count)
                    value = RenderingObjects.Count - 1;
                if (value < 0)
                    value = 0;

                if (value != _imageIndex || value != _defaultImageIndex)
                {
                    _defaultImageIndex = _imageIndex = value;
                    OnPropertyChanged();
                    if (!_isUpdating)
                        ClassLocator.Default.GetInstance<IMessenger>().Send(new ImageIndexChangedMessage() { ImageIndex = _imageIndex });
                    OnPropertyChanged(nameof(CurrentBitmap));
                }
            }
        }

        private MilImage _milImage = null;
        private BitmapSource _currentBitmap = null;
        public BitmapSource CurrentBitmap
        {
            get
            {
                SetCurrentBitmap();
                return _currentBitmap;
            }
            set
            {
                _currentBitmap = value;
            }
        }

        //=================================================================
        // Commandes
        //=================================================================
        private AutoRelayCommand _nextImageCommand = null;
        public AutoRelayCommand NextImageCommand
        {
            get
            {
                return _nextImageCommand ?? (_nextImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (_imageIndex < RenderingObjects.Count - 1)
                            ImageIndex++;
                    }));
            }
        }

        private AutoRelayCommand _previousImageCommand = null;
        public AutoRelayCommand PreviousImageCommand
        {
            get
            {
                return _previousImageCommand ?? (_previousImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (_imageIndex > 0)
                            ImageIndex--;
                    }));
            }
        }

        private AutoRelayCommand _firstImageCommand = null;
        public AutoRelayCommand FirstImageCommand
        {
            get
            {
                return _firstImageCommand ?? (_firstImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        ImageIndex = 0;
                    }));
            }
        }

        private AutoRelayCommand _lastImageCommand = null;
        public AutoRelayCommand LastImageCommand
        {
            get
            {
                return _lastImageCommand ?? (_lastImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        ImageIndex = RenderingObjects.Count - 1;
                    }));
            }
        }

        private AutoRelayCommand _saveAsCommand;
        public AutoRelayCommand SaveAsCommand
        {
            get
            {
                return _saveAsCommand ?? (_saveAsCommand = new AutoRelayCommand(
                    () => SaveData(),
                    () => CurrentBitmap != null));
            }
        }

        private AutoRelayCommand _openExternallyCommand;
        public AutoRelayCommand OpenExternallyCommand
        {
            get
            {
                return _openExternallyCommand ?? (_openExternallyCommand = new AutoRelayCommand(
                    () => OpenExternally(),
                    () => CurrentBitmap != null));
            }
        }

        private AutoRelayCommand _copyToClipboardCommand;
        public AutoRelayCommand CopyToClipboardCommand
        {
            get
            {
                return _copyToClipboardCommand ?? (_copyToClipboardCommand = new AutoRelayCommand(
                    () => Clipboard.SetImage(CurrentBitmap),
                    () => CurrentBitmap != null));
            }
        }

        //=================================================================
        // Conversion image MIL => image WPF
        //=================================================================
        private void SetCurrentBitmap()
        {
            if (RenderingObjects.Count == 0)
            {
                _milImage = null;
                _currentBitmap = null;
                return;
            }

            if (ImageIndex >= RenderingObjects.Count)
                ImageIndex = RenderingObjects.Count - 1;
            if (ImageIndex < 0)
                ImageIndex = 0;

            ImageBase image = (ImageBase)RenderingObjects[ImageIndex];


            MilImage milImage = image.CurrentProcessingImage.GetMilImage();
            if (milImage == null)
            {
                _milImage = null;
                _currentBitmap = null;
                return;
            }

            if ((_milImage == milImage) && (_currentBitmap != null))    // Déjà la bonne bitmap
                return;

            _milImage = milImage;

            ImageWidth = milImage.SizeX;
            ImageHeight = milImage.SizeY;
            DisplayImage(ImageIndex);
        }

        protected virtual void DisplayImage(int index)
        {
            ImageBase image = (ImageBase)RenderingObjects[ImageIndex];
            using (var image8Bit = _processingClassMil3D.ConvertTo8bit(image.CurrentProcessingImage))
            {
                _currentBitmap = image8Bit.ConvertToWpfBitmapSource();
            }
            FileName = !string.IsNullOrEmpty(image.Filename) ? Path.GetFileName(image.Filename) : string.Empty;
        }

        public void SaveData()
        {
            if (_milImage == null)
                return;

            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "BMP file (*.bmp)|*.bmp|Tiff file (*.tiff)|*.tiff;*.tif|Png file (*.png)|*.png|Jpeg file (*.jpg)|*.jpg|All files (*.*)|*.*";
            saveFileDlg.AddExtension = true;

            if (!string.IsNullOrEmpty(FileName))
            {
                PathString pathString = new PathString(FileName);
                saveFileDlg.InitialDirectory = pathString.Directory;
                saveFileDlg.FileName = Path.GetFileNameWithoutExtension(pathString.Filename);
            }

            try
            {
                if (saveFileDlg.ShowDialog() == true)
                    CurrentBitmap.Save(saveFileDlg.FileName);
            }
            catch (Exception ex)
            {
                string msg = "Failed to save \"" + saveFileDlg.FileName + "\"";
                ExceptionMessageBox.Show(msg, ex);
            }
        }

        private void OpenExternally()
        {
            try
            {
                if (_milImage == null)
                    return;

                string viewer = ConfigurationManager.AppSettings.Get("Debug.ImageViewer");
                if (viewer == null)
                    viewer = @"C:\Program Files\ImageJ\ImageJ.exe";

                PathString filename = FileName;
                filename.ChangeExtension(".tif");
                filename = filename.RemoveInvalidFilePathCharacters();
                filename = PathString.GetTempPath() / filename;

                _milImage.Save(filename);
                System.Diagnostics.Process.Start(viewer, filename);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show("Failed to export image to external viewer", ex);
            }

        }
    }
}
