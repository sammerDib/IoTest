using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance
{
    public class PreviewItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Value { get; set; }
    }

    public class AppearancePanelViewModel : SetupNodePanel<UserInterfaceConfiguration>
    {
        #region Fields

        private readonly FileSystemWatcher _systemWatcher = new();

        #endregion

        #region Constructors

        static AppearancePanelViewModel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AppearancePanelResources)));
        }

        public AppearancePanelViewModel()
        {
        }

        public AppearancePanelViewModel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            PreviewDataTable = new DataTableSource<PreviewItem>();
            PreviewDataTable.Search.AddSearchDefinition(new InvariantText(string.Empty), item => item.Name);

            var previewItems = new List<PreviewItem>();
            var random = new Random();
            for (var i = 0; i < 20; i++)
            {
                previewItems.Add(new PreviewItem { Id = i, Name = $"Item {i}", Value = random.Next(0, 1000) });
            }

            PreviewDataTable.AddRange(previewItems);

            Rules.Add(
                new DelegateRule(
                    nameof(FontSizeRate),
                    () =>
                    {
                        if (FontSizeRate is < 0.7 or > 1.3)
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_FONT_SIZE_RATE_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(GlobalSizeRate),
                    () =>
                    {
                        if (GlobalSizeRate is < 0.7 or > 1.5)
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_GLOBAL_SIZE_SIZE_RATE_WARNING));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(Themes),
                    () =>
                    {
                        var path = UserInterfaceManager.GetThemePath(ModifiedConfigNode.Theme);
                        if (!File.Exists(path))
                        {
                            return LocalizationManager.GetString(
                                nameof(SetupPanelResources.SETUP_MESSAGE_THEME_WARNING));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        public DataTableSource<PreviewItem> PreviewDataTable { get; }

        public ObservableCollection<ThemeViewModel> Themes { get; } = new();

        public List<RateViewModel> FontSizeRates { get; } = new()
        {
            new RateViewModel(0.7, "70%"),
            new RateViewModel(0.85, "85%"),
            new RateViewModel(1, "100%"),
            new RateViewModel(1.15, "115%"),
            new RateViewModel(1.3, "130%")
        };

        public List<RateViewModel> GlobalSizeRates { get; } = new()
        {
            new RateViewModel(0.7, "70%"),
            new RateViewModel(0.9, "90%"),
            new RateViewModel(1, "100%"),
            new RateViewModel(1.10, "110%"),
            new RateViewModel(1.25, "125%"),
            new RateViewModel(1.5, "150%")
        };

        private Border _previewElement;

        public Border PreviewElement
        {
            get => _previewElement;
            set
            {
                _previewElement = value;
                ApplyPreviewTheme();
            }
        }

        public double FontSizeRate
        {
            get => ModifiedConfigNode.FontScale;
            set
            {
                if (ModifiedConfigNode.FontScale.Equals(value))
                {
                    return;
                }

                ModifiedConfigNode.FontScale = value;
                App.Instance.UserInterfaceManager.PreviewFontScale = value;

                foreach (var model in FontSizeRates)
                {
                    model.IsSelected = model.Value.Equals(ModifiedConfigNode.FontScale);
                }

                OnPropertyChanged();
            }
        }

        public double GlobalSizeRate
        {
            get => ModifiedConfigNode.GlobalScale;
            set
            {
                if (ModifiedConfigNode.GlobalScale.Equals(value))
                {
                    return;
                }

                ModifiedConfigNode.GlobalScale = value;
                App.Instance.UserInterfaceManager.PreviewGlobalScale = value;

                foreach (var rateViewModel in GlobalSizeRates)
                {
                    rateViewModel.IsSelected = rateViewModel.Value.Equals(ModifiedConfigNode.GlobalScale);
                }

                OnPropertyChanged();
            }
        }

        #endregion

        #region Overrides of SetupPanel

        protected override UserInterfaceConfiguration GetNode(ApplicationConfiguration applicationConfiguration)
            => applicationConfiguration?.UserInterfaceConfiguration;

        public override void OnSetup()
        {
            base.OnSetup();
            LoadThemes();
            RefreshConfiguration();

            _systemWatcher.Path = UserInterfaceManager.ThemeFolder;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            _systemWatcher.NotifyFilter = NotifyFilters.LastAccess
                                          | NotifyFilters.LastWrite
                                          | NotifyFilters.FileName
                                          | NotifyFilters.DirectoryName;

            // Only watch text files.
            _systemWatcher.Filter = "*.xaml";

            // Add event handlers.
            _systemWatcher.Changed += OnFolderChanged;
            _systemWatcher.Created += OnFolderChanged;
            _systemWatcher.Deleted += OnFolderChanged;
            _systemWatcher.Renamed += OnFolderChanged;

            // Begin watching.
            _systemWatcher.EnableRaisingEvents = true;
        }

        protected override void SaveConfig()
        {
            base.SaveConfig();

            App.Instance.UserInterfaceManager.Theme = CurrentConfigNode.Theme;
            App.Instance.UserInterfaceManager.FontScale = CurrentConfigNode.FontScale;
            App.Instance.UserInterfaceManager.GlobalScale = CurrentConfigNode.GlobalScale;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            RefreshConfiguration();
        }

        protected override void Dispose(bool disposing)
        {
            _systemWatcher.Changed -= OnFolderChanged;
            _systemWatcher.Created -= OnFolderChanged;
            _systemWatcher.Deleted -= OnFolderChanged;
            _systemWatcher.Renamed -= OnFolderChanged;
            _systemWatcher.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ApplyTheme(string theme)
        {
            var selectedTheme = Themes.Single(model => model.Theme == theme);
            foreach (var viewModel in Themes)
            {
                viewModel.IsSelected = false;
            }

            selectedTheme.IsSelected = true;
            ModifiedConfigNode.Theme = theme;

            ApplyPreviewTheme();
        }

        private void ApplyPreviewTheme()
        {
            if (PreviewElement == null)
            {
                return;
            }

            var appliedPreviewTheme = ModifiedConfigNode.Theme;
            var path = UserInterfaceManager.GetThemePath(appliedPreviewTheme);
            try
            {
                var dictionary = UserInterfaceManager.BuildThemeFromXaml(path);
                ThemeHelper.ApplyTo(dictionary, PreviewElement.Resources);
            }
            catch (Exception e)
            {
                PopupHelper.Error(e.Message);
            }
        }

        private void RefreshConfiguration()
        {
            var selectedTheme = Themes.SingleOrDefault(model => model.Theme == ModifiedConfigNode.Theme);
            foreach (var viewModel in Themes)
            {
                viewModel.IsSelected = false;
            }

            if (selectedTheme != null)
            {
                selectedTheme.IsSelected = true;
            }

            FontSizeRate = ModifiedConfigNode.FontScale;
            GlobalSizeRate = ModifiedConfigNode.GlobalScale;

            ApplyPreviewTheme();
        }

        private void OnFolderChanged(object source, FileSystemEventArgs e)
            => DispatcherHelper.DoInUiThreadAsynchronously(
                () =>
                {
                    LoadThemes();
                    RefreshConfiguration();
                });

        private void LoadThemes()
        {
            Themes.Clear();
            foreach (var file in Directory.EnumerateFiles(UserInterfaceManager.ThemeFolder, "*.xaml"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Themes.Add(new ThemeViewModel(name, false, ApplyTheme));
            }
        }

        #endregion
    }
}
