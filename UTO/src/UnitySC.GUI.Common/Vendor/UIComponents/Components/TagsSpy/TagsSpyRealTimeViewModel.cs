using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataSource.MessageDataBus;
using Agileo.DataMonitoring.DataWriter.Chart.TagChart;
using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.DataWriter.File.Csv;
using Agileo.DataMonitoring.DataWriter.File.StorageStrategy;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.MessageDataBus;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.TagsSpy
{
    public class TagsSpyRealTimeViewModel : Notifier
    {
        #region Fields

        private DataCollectionPlan _currentDataCollectionPlan;

        private FileDataWriter _currentFileWriter;

        #endregion Fields

        #region Properties

        private TagChartDataWriter _tagChartDataWriter;

        public TagChartDataWriter TagChartDataWriter
        {
            get => _tagChartDataWriter;
            private set => SetAndRaiseIfChanged(ref _tagChartDataWriter, value);
        }

        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set => SetAndRaiseIfChanged(ref _isPaused, value);
        }

        private bool _wantFileWriter;

        public bool WantFileWriter
        {
            get => _wantFileWriter;
            set => SetAndRaiseIfChanged(ref _wantFileWriter, value);
        }

        private double _defaultDcpFrequency = 10;

        public double DefaultDcpFrequency
        {
            get => _defaultDcpFrequency;
            set
            {
                if (SetAndRaiseIfChanged(ref _defaultDcpFrequency, value))
                {
                    _currentDataCollectionPlan.Frequency = Frequency.FromHertz(_defaultDcpFrequency);
                }
            }
        }

        private double _defaultWriterFrequency = 10;

        public double DefaultWriterFrequency
        {
            get { return _defaultWriterFrequency; }
            set
            {
                if (SetAndRaiseIfChanged(ref _defaultWriterFrequency, value))
                {
                    TagChartDataWriter.WriteFrequency = Frequency.FromHertz(_defaultWriterFrequency);
                }
            }
        }

        private int _range;

        public int Range
        {
            get => _range;
            set => SetAndRaiseIfChanged(ref _range, value);
        }

        private bool _dcpExist;

        public bool DcpExist
        {
            get => _dcpExist;
            set => SetAndRaiseIfChanged(ref _dcpExist, value);
        }

        #endregion

        #region Dcp edition

        private static string GetDefaultDcpName() => $"TagsSpy_{DateTime.Now:yyyyMMdd_HHmmss}";

        private void SetDefaultDcpWithWriter()
        {
            DcpExist = true;
            _currentDataCollectionPlan = new DataCollectionPlan(GetDefaultDcpName(), Frequency.FromHertz(_defaultDcpFrequency)) { IsDynamic = false };
            TagChartDataWriter = new TagChartDataWriter("Tag spy writer", Frequency.FromHertz(_defaultWriterFrequency));
            Range = TagChartDataWriter.Chart.SlidingRange;
            UpdateFileWriter();
            _currentDataCollectionPlan.AddDataWriter(TagChartDataWriter);
            App.Instance.DataCollectionPlanLibrarian.Add(_currentDataCollectionPlan);
        }

        private void UpdateFileWriter()
        {
            if (_wantFileWriter)
            {
                _currentFileWriter = new CsvFileDataWriter(new TimestampedFileStorageStrategy(GetDefaultDcpName(),
                        Path.GetFullPath(App.Instance.Config.ApplicationPath.DcpStoragePath)));

                _currentDataCollectionPlan.AddDataWriter(_currentFileWriter);
            }
            else
            {
                _currentDataCollectionPlan.RemoveDataWriter(_currentFileWriter);
            }
        }

        #endregion

        #region Remove

        public void RemoveTag(BaseTag tag)
        {
            var currentMdbTagsSource = _currentDataCollectionPlan.DataSources;
            _currentDataCollectionPlan.RemoveDataSource(currentMdbTagsSource.First(source => source.Information.SourceName.Equals(tag.Name)));

            if (currentMdbTagsSource.Count == 0)
            {
                DcpExist = false;
                _currentDataCollectionPlan.Stop();
                App.Instance.DataCollectionPlanLibrarian.Remove(_currentDataCollectionPlan);
                DisposeDcp();
            }
        }

        private void DisposeDcp()
        {
            TagChartDataWriter.Dispose();
            TagChartDataWriter = null;
            _currentDataCollectionPlan.Dispose();
            _currentDataCollectionPlan = null;
        }

        public void RemoveAllTags(List<BaseTag> tags)
        {
            tags.ForEach(RemoveTag);
        }

        #endregion

        #region Add

        public void AddTag(BaseTag tag)
        {
            if (_currentDataCollectionPlan == null)
            {
                SetDefaultDcpWithWriter();
                _currentDataCollectionPlan.Start();
            }

            if (TagChartDataWriter.IsMaxDisplaying())
            {
                MessageBox.Show("Tag limit reached, tag added in the sources but not visible on the chart");
            }

            _currentDataCollectionPlan.AddDataSource(new MdbTagDataSource(tag));
        }

        #endregion

        #region Commands

        private ICommand _applyRangeCommand;

        public ICommand ApplyRangeCommand => _applyRangeCommand ?? (_applyRangeCommand = new DelegateCommand(CanApplyRangeCommandExecute));

        private void CanApplyRangeCommandExecute()
        {
            if (TagChartDataWriter != null)
            {
                TagChartDataWriter.Chart.SlidingRange = Range;
            }
        }

        private ICommand _paused;

        public ICommand Paused => _paused ?? (_paused = new DelegateCommand(StartStopCommandExecute));

        private void StartStopCommandExecute()
        {
            if (IsPaused)
                TagChartDataWriter?.Chart.UnPause();
            else
                TagChartDataWriter?.Chart.Pause();
            IsPaused = !IsPaused;
        }

        private ICommand _recenter;

        public ICommand Recenter => _recenter ?? (_recenter = new DelegateCommand(RecenterCommandExecute));

        private void RecenterCommandExecute()
        {
            IsPaused = false;
            TagChartDataWriter?.Chart.Recenter();
        }

        private ICommand _makeCaptureCommand;

        public ICommand MakeCaptureCommand =>
            _makeCaptureCommand ?? (_makeCaptureCommand = new DelegateCommand<FrameworkElement>(MakeCaptureCommandExecute));


        private readonly Dictionary<string, int> _fileNames = new Dictionary<string, int>();

        private void MakeCaptureCommandExecute(FrameworkElement arg)
        {
            const string path = @".\DataCollectionPlan\ScreenShotVisualization\";

            var basePath = "TagChartView";
            var fileName = basePath + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");

            string fullPath;
            if (!_fileNames.ContainsKey(fileName))
            {
                _fileNames.Add(fileName, 0);
                fullPath = path + fileName;
            }
            else
            {
                _fileNames[fileName]++;
                fullPath = path + fileName + "(" + _fileNames[fileName] + ")";
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(fullPath);
            }

            TakeScreenShot((int)arg.ActualHeight, (int)arg.ActualWidth, arg, fullPath, ".png");
        }

        private static void TakeScreenShot(
            int height,
            int width,
            FrameworkElement visual,
            string file,
            string extension)
        {
            // Set another DPI if necessary
            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);
            BitmapEncoder encoder;

            switch (extension)
            {
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                default:
                    return;
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));
            var path = file + extension;
            using (Stream stm = File.Create(path))
            {
                encoder.Save(stm);
            }
        }

        #endregion
    }
}
