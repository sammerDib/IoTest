using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using UnitySC.Result.StandaloneClient.ViewModel.Common;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Result.StandaloneClient.Models
{
    public class FileEntry : ExplorerEntry
    {
        #region Fields

        private static readonly Regex s_slotIdRegex;

        #endregion

        #region Properties

        public string FileName { get; }

        public string TypeName { get; }
        
        public string Extension { get; }

        public int ExtensionId { get; }

        public ResultType ResultType { get; }

        public SolidColorBrush TypeColor { get; }

        private ImageSource _thumbnail;

        public ImageSource Thumbnail
        {
            get => _thumbnail;
            private set => SetProperty(ref _thumbnail, value);
        }

        private bool _thumbnailGenerating;

        public bool ThumbnailGenerating
        {
            get => _thumbnailGenerating;
            private set => SetProperty(ref _thumbnailGenerating, value);
        }

        public string SlotName { get; }

        #endregion

        static FileEntry()
        {
            s_slotIdRegex = new Regex("(?<=_S)[0-9]+(?=_)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public FileEntry(string path) : base(path)
        {
            try
            {
                FileName = path.Substring(path.LastIndexOf('\\') + 1);
                Extension = System.IO.Path.GetExtension(path).Substring(1).ToUpperInvariant();
                ExtensionId = ResultFormatExtension.GetExtIdFromExtensionOrDefault(Extension);

                ResultType = ResultFormatExtension.GetResultTypeFromExtension(Extension);
                TypeName = ResultType.GetDisplayName();
                TypeColor = ResultType.GetColor();

                var matches = s_slotIdRegex.Matches(FileName);
                foreach (Match match in matches)
                {
                    SlotName = $"S{match.Value}";
                    break;
                }

                Thumbnail = (ImageSource)Application.Current.FindResource("ResWaferNotProcess");
            }
            catch (Exception)
            {
                //Exception is catch when a file doesnt have any extension. 
                //ignored
            }
        }

        private bool _thumbnailLoaded;
        
        public void LoadThumbnail()
        {
            if (_thumbnailLoaded || ResultType == ResultType.NotDefined) return;
            Task.Run(async () => await LoadThumbnailAsync(false));
        }

        public void GenerateThumbnail()
        {
            if (ResultType == ResultType.NotDefined) return;
            Task.Run(async () => await LoadThumbnailAsync(true));
        }
        
        private async Task LoadThumbnailAsync(bool forceGeneration)
        {
            if (ThumbnailGenerating) return;

            await Task.Run(() =>
            {
                if (forceGeneration) _thumbnailLoaded = false;

                var resultDataFactory = App.Instance.ResultDataFactory;
                if (resultDataFactory == null) return;

                Application.Current?.Dispatcher?.Invoke(() => { Thumbnail = (ImageSource)Application.Current.FindResource("ResWaferWait"); });

                var resultDataObject = resultDataFactory.Create(ResultType, 1);
                resultDataObject.ResFilePath = Path;

                string thumbnailPath = FormatHelper.ThumbnailPathOf(resultDataObject);

                bool fileExists = File.Exists(thumbnailPath);
                if (!fileExists || forceGeneration)
                {
                    try
                    {
                        ThumbnailGenerating = true;

                        if (fileExists)
                        {
                            File.Delete(thumbnailPath);
                        }

                        if (resultDataObject.ReadFromFile(Path, out string _))
                        {
                            object[] parameters;

                            switch (ResultType.GetResultFormat())
                            {
                                case ResultFormat.Klarf:
                                    parameters = new object[] { App.Instance.Settings.KlarfSettings.RoughBins, App.Instance.Settings.KlarfSettings.SizeBins };
                                    break;
                                case ResultFormat.Haze:
                                    parameters = new object[] { ColorMapHelper.ColorMaps.FirstOrDefault(x => x.Name == App.Instance.Settings.HazeSettings.ColorMapName)?.Name };
                                    break;
                                default:
                                    parameters = null;
                                    break;
                            }

                            if (!resultDataFactory.GenerateThumbnailFile(resultDataObject, parameters))
                            {
                                SetThumbnailError();
                                return;
                            }
                        }
                        else
                        {
                            SetThumbnailError();
                            return;
                        }

                    }
                    catch (Exception)
                    {
                        SetThumbnailError();
                        return;
                    }
                    finally
                    {
                        ThumbnailGenerating = false;
                    }
                }

                Application.Current?.Dispatcher?.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() =>
                {
                    if (ThumbnailGenerating) return;
                    Thumbnail = BitmapFromUri(new Uri(thumbnailPath));
                }));
                _thumbnailLoaded = true;
            });
        }

        private void SetThumbnailError()
        {
            Application.Current?.Dispatcher?.Invoke(() => { Thumbnail = (ImageSource)Application.Current.FindResource("ResWaferErrorWarning"); });
        }

        private static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
