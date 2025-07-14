using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcTools;

using BasicModules.DataLoader.Rendering;

using CommunityToolkit.Mvvm.ComponentModel;

using LibProcessing;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace BasicModules.DataLoader
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class SelectRenderingImageViewModel : ObservableRecipient
    {
        /// <summary> Le DataLoader associé </summary>
        public DataLoaderBase Module;

        /// <summary> Liste des "images" d'une couche de l'acquisition </summary>
        public List<AcquisitionDataViewModel> InputDataList { get; private set; }

        /// <summary>
        /// Wafer Width mm
        /// </summary>
        public float WaferWidth { get; private set; }

        /// <summary>
        /// Wafer Height mm
        /// </summary>
        public float WaferHeight { get; private set; }

        /// <summary>
        /// All dataloader image Height mm
        /// </summary>
        public float AllImageHeight { get; private set; }

        /// <summary>
        /// All dataloader image width mm 
        /// </summary>
        public float AllImageWidth { get; private set; }

        /// <summary>
        /// Wafer position mm canvas
        /// </summary>
        public float WaferPositionX { get; private set; }

        /// <summary>
        /// Wafer postion mm canvas
        /// </summary>
        public float WaferPositionY { get; private set; }

        /// <summary>
        /// The wafer is rectangular
        /// </summary>
        public bool IsRectangularWafer { get; private set; }

        /// <summary>
        /// The dataloader is full image
        /// </summary>
        public bool IsFullImage { get; private set; }

        /// <summary>
        /// Wafer aligner angle
        /// </summary>
        public double AlignerAngle { get; private set; }

        /// <summary>
        /// Opposite aligner angle for wpf display
        /// </summary>
        public double OppositeAlignerAngle { get; private set; }

        /// <summary>
        /// Background task in progress
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private bool _loadImageisBusy;
        public bool LoadImageIsBusy
        {
            get => _loadImageisBusy; set { if (_loadImageisBusy != value) { _loadImageisBusy = value; OnPropertyChanged(); } }
        }

        private ProcessingClassMil3D _processingClassMil3D = new ProcessingClassMil3D();
        private ProcessingClassMil _processingClassMil = new ProcessingClassMil();

        public SelectRenderingImageViewModel(DataLoaderBase module)
        {
            Module = module;
        }

        public void Init()
        {
            InputDataList = new List<AcquisitionDataViewModel>();

            // Recupération des info du wafer
            InspectionInputInfoBase inputInfo = (InspectionInputInfoBase)Module.GetInputInfo();
            WaferHeight = Module.Recipe.Wafer.SurroundingRectangle.Height / 1000.0f; // Size in mm
            WaferWidth = Module.Recipe.Wafer.SurroundingRectangle.Width / 1000.0f; // Size in mm
            IsRectangularWafer = Module.Recipe.Wafer is RectangularWafer;
            IsFullImage = inputInfo is FullImageInputInfo;
            AlignerAngle = inputInfo.MatrixInfo.AlignerAngleDegree;
            OppositeAlignerAngle = -AlignerAngle;
            MatrixBase matrix = Module.CreateMatrix(inputInfo.MatrixInfo);

            // Converti les images en pixel dans un répére wafer au centre en mm
            InputInfoToAcquisitionDataVM(inputInfo, matrix);

            // Dertermine la taille du canvas à afficher
            float xMinmm = InputDataList.Min(i => i.X);
            float xMaxmm = InputDataList.Max(i => i.X);
            float yMinmm = InputDataList.Min(i => i.Y);
            float yMaxmm = InputDataList.Max(i => i.Y);
            xMinmm = Math.Min(xMinmm - InputDataList.First().Width, -(WaferWidth / 2.0f));
            yMaxmm = Math.Max(yMaxmm + InputDataList.First().Height, WaferWidth / 2.0f);
            AllImageWidth = Math.Max(-xMinmm + xMaxmm + InputDataList.First().Width, WaferWidth);
            AllImageHeight = Math.Max(-yMinmm + yMaxmm + InputDataList.First().Height, WaferHeight);

            //Repasse dans un repére image pour un affichage wpf
            foreach (var acquisitionData in InputDataList)
            {
                acquisitionData.X = acquisitionData.X - xMinmm;
                // Changement le repére implique un changment de coin
                acquisitionData.Y = yMaxmm - acquisitionData.Y - acquisitionData.Height;
            }

            //Défini la position du wafer dans le repére image 
            WaferPositionX = (-(WaferWidth / 2.0f) - xMinmm);
            WaferPositionY = (yMaxmm - (WaferHeight / 2.0f));
        }

        /// <summary>
        /// Converti les images dans un repére wafer au centre en mm
        /// </summary>
        /// <param name="inputInfo"></param>
        /// <param name="matrix"></param>
        public void InputInfoToAcquisitionDataVM(InputInfoBase inputInfo, MatrixBase matrix)
        {
            if (inputInfo is DieInputInfo) // Die
            {
                foreach (var inputData in ((InspectionInputInfoBase)inputInfo).InputDataList.OfType<AcquisitionDieImage>().OrderBy(x => x.Y).OrderBy(y => y.X))
                {
                    Rectangle pixelImageSize = new Rectangle(inputData.X, inputData.Y, inputData.Width, inputData.Height);
                    RectangleF micronImageSize = matrix.pixelToMicron(pixelImageSize).SurroundingRectangle;

                    // Acquistion data in mm for the view
                    AcquisitionDataViewModel acquisitionData = new AcquisitionDataViewModel(inputData, this)
                    {
                        Height = micronImageSize.Height / 1000.0f,
                        Width = micronImageSize.Width / 1000.0f,
                        X = micronImageSize.X / 1000.0f,
                        Y = micronImageSize.Y / 1000.0f
                    };

                    InputDataList.Add(acquisitionData);
                }
            }
            else if (inputInfo is MosaicInputInfo) // Mosaic
            {
                MosaicInputInfo mosaicInputInfo = ((MosaicInputInfo)inputInfo);
                int mosaicImageWidth = mosaicInputInfo.MosaicImageWidth;
                int mosaicImageHeight = mosaicInputInfo.MosaicImageHeight;

                foreach (var inputData in ((InspectionInputInfoBase)inputInfo).InputDataList.OfType<AcquisitionMosaicImage>().OrderBy(x => x.Line).ThenBy(y => y.Column))
                {
                    Rectangle pixelImageSize = new Rectangle(inputData.Column * mosaicImageWidth, inputData.Line * mosaicImageHeight, mosaicImageWidth, mosaicImageHeight);
                    RectangleF micronImageSize = matrix.pixelToMicron(pixelImageSize).SurroundingRectangle;

                    // acquistion data in mm for the view
                    AcquisitionDataViewModel acquisitionData = new AcquisitionDataViewModel(inputData, this)
                    {
                        Height = micronImageSize.Height / 1000.0f,
                        Width = micronImageSize.Width / 1000.0f,
                        X = micronImageSize.X / 1000.0f,
                        Y = micronImageSize.Y / 1000.0f
                    };
                    InputDataList.Add(acquisitionData);
                }
            }

            else if (inputInfo is InspectionInputInfoBase) // Other
            {
                foreach (var inputData in ((InspectionInputInfoBase)inputInfo).InputDataList)
                    InputDataList.Add(new AcquisitionDataViewModel(inputData, this));
            }
        }

        /// <summary>
        /// Include all image
        /// </summary>
        public bool IncludeAll
        {
            get => InputDataList != null ? InputDataList.All(x => x.IsEnabled) : false;
            set
            {
                if (InputDataList != null && value != IncludeAll)
                {
                    InputDataList.ForEach(x => x.IsEnabled = value);
                }
                OnPropertyChanged();
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Creation asynchrone des éléments à afficher.
        /// </summary>
        private AutoRelayCommand _loadCommand;
        public AutoRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new AutoRelayCommand(
              () =>
              {
                  IsBusy = true;
                  Task.Factory.StartNew(() =>
                  {
                      try
                      {
                          Init();
                          System.Windows.Application.Current.Dispatcher.Invoke(() =>
                          {
                              // OnPropertyChanged for all properties
                              OnPropertyChanged(string.Empty);
                          });
                      }
                      catch (Exception ex)
                      {
                          System.Windows.Application.Current.Dispatcher.Invoke(() =>
                          {
                              ExceptionMessageBox.Show("Error in init select rendering file", ex);
                              Module.logDebug("Error in init select rendering file" + ex.ToString());
                          });
                      }
                      finally
                      {
                          System.Windows.Application.Current.Dispatcher.Invoke(() => { IsBusy = false; });
                      }
                  });
              },
              () => { return true; }));
            }
        }

        /// <summary>
        /// Selected image
        /// </summary>
        private AcquisitionDataViewModel _selectedInputData;
        public AcquisitionDataViewModel SelectedInputData
        {
            get => _selectedInputData;
            set
            {
                if (_selectedInputData != value)
                {
                    _selectedInputData = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedPath));
                    UpdatePreviewImage(_selectedInputData?.Filename);
                }
            }
        }

        #region preview 

        public string SelectedPath
        {
            get => SelectedInputData?.Filename;
        }

        private void UpdatePreviewImage(string uri)
        {
            LoadImageIsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (ProcessingImage previewMilImage = new ProcessingImage())
                    {
                        if (uri.ToUpper().EndsWith(".3DA"))
                            _processingClassMil3D.Load(uri, previewMilImage);
                        else
                            _processingClassMil.Load(uri, previewMilImage);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (_processingClassMil3D.CopyToWriteableBitmap(ref _previewImage, previewMilImage))
                                OnPropertyChanged(nameof(PreviewImage));
                        });
                    }
                }
                finally
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        LoadImageIsBusy = false;
                    });
                }
            });
        }





        private WriteableBitmap _previewImage;
        public WriteableBitmap PreviewImage => _previewImage;


        private AutoRelayCommand _openImageCommand;
        public AutoRelayCommand OpenImageCommand
        {
            get
            {
                return _openImageCommand ?? (_openImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (File.Exists(SelectedPath))
                            Process.Start(SelectedPath);
                    },
                    () => { return SelectedPath != null; }));
            }
        }

        #endregion


        /// <summary>
        /// Enable image for rendering
        /// </summary>
        private AutoRelayCommand _enableImageCommand;
        public AutoRelayCommand EnableImageCommand
        {
            get
            {
                return _enableImageCommand ?? (_enableImageCommand = new AutoRelayCommand(
              () =>
              {
                  SelectedInputData.IsEnabled = true;
              },
              () => { return SelectedInputData != null && !SelectedInputData.IsEnabled; }));
            }
        }

        /// <summary>
        /// Disable image for rendering
        /// </summary>
        private AutoRelayCommand _disableImageCommand;
        public AutoRelayCommand DisableImageCommand
        {
            get
            {
                return _disableImageCommand ?? (_disableImageCommand = new AutoRelayCommand(
              () =>
              {
                  SelectedInputData.IsEnabled = false;
              },
              () => { return SelectedInputData != null && SelectedInputData.IsEnabled; }));
            }
        }
    }
}
