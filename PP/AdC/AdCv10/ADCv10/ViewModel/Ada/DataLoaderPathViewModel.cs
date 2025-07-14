using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using CommunityToolkit.Mvvm.ComponentModel;

using LibProcessing;

using Serilog;

namespace ADC.ViewModel.Ada
{
    /// <summary>
    /// View model pour la selection des images d'entrée lors de la création d'un ada
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DataLoaderPathViewModel : ObservableRecipient
    {
        private ProcessingClassMil3D _processingClassMil3D = new ProcessingClassMil3D();
        private ProcessingClassMil _processingClassMil = new ProcessingClassMil();

        private DataloaderViewModel _dataloaderViewLodel;
        public DataLoaderPathViewModel(DataloaderViewModel dataloaderViewLodel)
        {
            _dataloaderViewLodel = dataloaderViewLodel;
        }

        private string _name = "Full image";
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private string _path;
        public string Path
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    _dataloaderViewLodel.IsValid = true;
                    OnPropertyChanged();

                    if (_path != null)
                    {
                        try
                        {
                            UpdateWaferCenter();
                        }
                        catch (Exception ex)
                        {
                            Log.Debug("Update wafer center error " + ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Mise a jour du centre du wafer en focntion des informations contenues dans les images
        /// </summary>
        private void UpdateWaferCenter()
        {
            int width;
            int height;
            int depth; //unused

            switch (_dataloaderViewLodel.ImageSelection)
            {
                case DataloaderViewModel.ImageSelectionType.MultiFullImage:
                case DataloaderViewModel.ImageSelectionType.OneFullImage:
                    if (File.Exists(_path))
                    {
                        if (_path.ToUpper().EndsWith(".3DA"))
                            _processingClassMil3D.ImageDiskInquire(_path, out width, out height, out depth);
                        else
                            _processingClassMil.ImageDiskInquire(_path, out width, out height, out depth);

                        if (width != -1)
                            _dataloaderViewLodel.WaferCenterX = width / 2.0;
                        if (height != -1)
                            _dataloaderViewLodel.WaferCenterY = height / 2.0;
                    }
                    break;
                case DataloaderViewModel.ImageSelectionType.Mosaic:
                    if (Directory.Exists(_path))
                    {
                        string firstLineFileName = string.Empty;
                        if (_dataloaderViewLodel.Is3D)
                        {
                            firstLineFileName = Directory.GetFiles(_path, "*.3da").First();
                            _processingClassMil3D.ImageDiskInquire(firstLineFileName, out width, out height, out depth);
                            if (width != -1)
                                _dataloaderViewLodel.WaferCenterX = (width * _dataloaderViewLodel.NbColumnsMosaic3D()) / 2.0;

                            if (height != -1)
                                _dataloaderViewLodel.WaferCenterY = height / 2.0;
                        }
                        else
                        {
                            IEnumerable<string> columnFolderNames = Directory.GetDirectories(_path, NewAdaViewModel.MosaicColumnFolderName + "*");
                            IEnumerable<string> lineFileNames = Directory.GetFiles(columnFolderNames.First());
                            firstLineFileName = lineFileNames.First();
                            _processingClassMil.ImageDiskInquire(firstLineFileName, out width, out height, out depth);
                            if (width != -1)
                                _dataloaderViewLodel.WaferCenterX = (width * columnFolderNames.Count()) / 2.0;
                            if (height != -1)
                                _dataloaderViewLodel.WaferCenterY = (height * lineFileNames.Count()) / 2.0;
                        }
                    }
                    break;
                case DataloaderViewModel.ImageSelectionType.Die:
                    if (File.Exists(_path))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(_path);
                        XmlNode root = doc.DocumentElement;

                        // pixelSizeX
                        XmlNode pixelSizeX = root.SelectSingleNode("/root/PixelSizeX/@value");
                        if (pixelSizeX != null)
                            _dataloaderViewLodel.PixelSizeX = Convert.ToDouble(pixelSizeX.Value);

                        // pixelSizeZ
                        XmlNode pixelSizeY = root.SelectSingleNode("/root/PixelSizeY/@value");
                        if (pixelSizeY != null)
                            _dataloaderViewLodel.PixelSizeY = Convert.ToDouble(pixelSizeY.Value);

                        // wafer center X
                        XmlNode waferCenterX = root.SelectSingleNode("/root/WaferCenterX/@value");
                        if (waferCenterX != null)
                            _dataloaderViewLodel.WaferCenterX = Convert.ToDouble(waferCenterX.Value);

                        // wafer center Y
                        XmlNode waferCenterY = root.SelectSingleNode("/root/WaferCenterY/@value");
                        if (waferCenterY != null)
                            _dataloaderViewLodel.WaferCenterY = Convert.ToDouble(waferCenterY.Value);
                    }
                    break;
            }
        }
    }
}
