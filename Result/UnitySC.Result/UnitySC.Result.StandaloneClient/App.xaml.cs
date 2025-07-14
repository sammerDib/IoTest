using System.Collections.Generic;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Result.StandaloneClient.ViewModel.Common;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.StandaloneClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Properties

        public MainWindowVM MainWindowViewModel { get; private set; }
        
        public IResultDataFactory ResultDataFactory { get; private set; }
        
        public IMessenger Messenger { get; private set; }

        public Dictionary<ResultType, ResultWaferVM> ResultWaferDictionary { get; private set; }

        public NotifierVM NotifierVM => ClassLocator.Default.GetInstance<NotifierVM>();

        public static App Instance => Current as App;

        public readonly Settings Settings = new Settings();

        #endregion

        public App()
        {
            Bootstrapper.Register();
        }

        #region Overrides of Application

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowViewModel = new MainWindowVM();
            MainWindow = new MainWindow { DataContext = MainWindowViewModel };

            ResultDataFactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            ResultWaferDictionary = ViewerVMBuilder.Instance.BuildDicoResultVM(ResultDataFactory); //call builder for Wafer result

            ResultDataFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(Settings.KlarfSettings.RoughBins, Settings.KlarfSettings.SizeBins);
            ResultDataFactory.GetDisplayFormat(ResultType.ADC_Haze).UpdateInternalDisplaySettingsPrm(Settings.HazeSettings.ColorMapName);

            Settings.LoadThumbnailColorMapSettings(Settings.ThumbnailConfigPath);

            Messenger = ClassLocator.Default.GetInstance<IMessenger>();

            MainWindow.Show();

            if (e.Args.Length > 0)
            {
                string fileParams = string.Join(" ", e.Args);
                string path = Path.GetFullPath(fileParams);

                while (path.EndsWith(@"\"))
                {
                    path = path.Remove(path.Length - 1, 1);
                }

                MainWindowViewModel.Init(path);
            }
            else
            {
                MainWindowViewModel.Init();
            }

        }

        #endregion
    }
}
