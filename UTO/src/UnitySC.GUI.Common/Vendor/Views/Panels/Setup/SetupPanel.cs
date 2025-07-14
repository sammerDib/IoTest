using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;
using Agileo.GUI.Collections;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.Saliences;
using Agileo.GUI.Services.UserMessages;

using Newtonsoft.Json;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup
{
    public interface ISetupPanel
    {
        void UndoCommandExecute();

        bool UndoCommandCanExecute();

        void SaveCommandExecute();

        bool SaveCommandCanExecute();

        void RefreshSaveRequired();

        bool HasError { get; }

        GraphicalElementCollection<BusinessPanelCommandElement> Commands { get; }
    }

    public abstract class SetupPanel<T> : NotifyDataErrorPanel, ISetupPanel
        where T : class, IConfiguration, new()
    {
        #region Fields

        private readonly T _designTimeConfiguration;
        protected IConfigManager ConfigManager { get; private set; }

        private readonly UserMessage _restartMessage = new(
            MessageLevel.Warning,
            new LocalizableText(nameof(SetupPanelResources.SETUP_MESSAGE_RESTART)));

        private readonly UserMessage _unsavedMessage = new(
            MessageLevel.Info,
            new LocalizableText(nameof(SetupPanelResources.SETUP_MESSAGE_CHANGES_UNSAVED)));

        private UserMessage _errorMessage = new(
            MessageLevel.Error,
            new LocalizableText(nameof(SetupPanelResources.SETUP_MESSAGE_ERRORS)));

        #endregion Fields

        #region Constructors

        static SetupPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        protected SetupPanel()
            : this($"DesignTime{typeof(T).Name}Ctor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameters) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        protected SetupPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            if (IsInDesignMode)
            {
                _designTimeConfiguration = new T();
                return;
            }

            Logger = App.Instance.GetLogger(GetType().Name);
            ErrorsChanged += OnErrorsChanged;
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            HasError = HasErrors;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether applying changes need an application restart.
        /// </summary>
        protected virtual bool ChangesNeedRestart => false;

        protected ILogger Logger { get; }

        private bool _hasChanges;

        /// <summary>
        /// Useful to know if save is required. Save is required if at least one accessLevel was changed.
        /// When property is set, salience activation and userMessage are updated
        /// </summary>
        protected bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (!SetAndRaiseIfChanged(ref _hasChanges, value))
                {
                    return;
                }

                if (_hasChanges)
                {
                    Messages.Show(_unsavedMessage);
                    Saliences.Add(SalienceType.UnfinishedTask);
                }
                else
                {
                    Saliences.Remove(SalienceType.UnfinishedTask);
                    Messages.Hide(_unsavedMessage);
                }
            }
        }

        private bool _hasError;

        public bool HasError
        {
            get => _hasError;
            protected set
            {
                if (!SetAndRaiseIfChanged(ref _hasError, value))
                {
                    return;
                }

                if (value)
                {
                    var errors = GetAllErrors().ToList();
                    if (errors.Any())
                    {
                        var sb = new StringBuilder();
                        var lastItem = errors.Last();
                        foreach (var error in errors)
                        {
                            if (error.Equals(lastItem))
                            {
                                sb.Append(error);
                            }
                            else
                            {
                                sb.AppendLine(error);
                            }
                        }

                        _errorMessage = new UserMessage(MessageLevel.Error, sb.ToString());
                    }
                    else
                    {
                        _errorMessage = new(
                            MessageLevel.Error,
                            new LocalizableText(nameof(SetupPanelResources.SETUP_MESSAGE_ERRORS)));
                    }
                    Messages.Show(_errorMessage);
                    Saliences.Add(SalienceType.Alarm);
                }
                else
                {
                    Saliences.Remove(SalienceType.Alarm);
                    Messages.Hide(_errorMessage);
                }
            }
        }

        protected T LoadedConfig => ConfigManager?.Loaded as T;

        protected T CurrentConfig => ConfigManager?.Current as T;

        public T ModifiedConfig => IsInDesignMode ? _designTimeConfiguration : ConfigManager?.Modified as T;

        #endregion Properties

        #region Commands

        #region UndoCommand

        public virtual bool UndoCommandCanExecute() => IsInDesignMode || HasChanges;

        public void UndoCommandExecute()
        {
            if (!UndoCommandCanExecute())
            {
                return;
            }

            Popups.ShowDuring(new BusyIndicator(nameof(SetupPanelResources.SETUP_UNDO_CHANGES)), TryUndoChanges);
        }

        private void TryUndoChanges()
        {
            try
            {
                UndoChanges();
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error occurred while undoing configuration changes");

                Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(nameof(SetupPanelResources.SETUP_UNDO_CHANGES_FAILED), e.Message)));
            }
        }

        protected virtual void UndoChanges()
        {
            if (ConfigManager.IsApplyRequired)
            {
                ConfigManager.Undo();
            }

            OnPropertyChanged(string.Empty);
        }

        #endregion UndoCommand

        #region SaveCommand

        public void SaveCommandExecute()
        {
            if (!SaveCommandCanExecute())
            {
                return;
            }

            Popups.ShowDuring(new BusyIndicator(nameof(SetupPanelResources.SETUP_SAVE_CONFIG)), TrySaveConfig);
        }

        private void TrySaveConfig()
        {
            try
            {
                SaveConfig();
                Messages.Show(
                    new UserMessage(MessageLevel.Success, nameof(SetupPanelResources.SETUP_MESSAGE_SUCCESS_SAVED))
                    {
                        SecondsDuration = 5
                    });

                if (!ChangesNeedRestart)
                {
                    return;
                }

                App.Instance.UserInterface.Messages.Hide(_restartMessage);
                if (!ConfigurationEqualsLoaded())
                {
                    App.Instance.UserInterface.Messages.Show(_restartMessage);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error occurred while saving configuration");
                Messages.Show(
                    new UserMessage(
                        MessageLevel.Error,
                        new LocalizableText(nameof(SetupPanelResources.SETUP_SAVE_CONFIG_FAILED), e.Message)));
            }
        }

        protected virtual void SaveConfig()
        {
            if (!ConfigManager.IsApplyRequired && !ConfigManager.IsSaveRequired)
            {
                return;
            }

            ConfigManager.Apply(true);

            // traces
            Logger.Info(
                CurrentConfig.AsAttachment(),
                "Configuration has been successfully saved and will take effect after the application restart.");
        }

        public virtual bool SaveCommandCanExecute() => IsInDesignMode || (HasChanges && !HasError);

        #endregion SaveCommand

        #endregion Commands

        #region EventHandlers

        public override void OnSetup()
        {
            ConfigManager = GetConfigManager();

            if (ConfigManager == null)
            {
                throw new ConfigurationErrorsException("ConfigurationManager instance is null.");
            }

            if (ConfigManager.Loaded == null)
            {
                ConfigManager.Load();
            }

            // Check specif configurations now that Config variable has been set in base class
            if (LoadedConfig == null)
            {
                throw new ConfigurationErrorsException($"{nameof(LoadedConfig)} is null or is not a {typeof(T)}");
            }

            if (CurrentConfig == null)
            {
                throw new ConfigurationErrorsException($"{nameof(CurrentConfig)} is null or is not a {typeof(T)}");
            }

            if (ModifiedConfig == null)
            {
                throw new ConfigurationErrorsException($"{nameof(ModifiedConfig)} is null or is not a {typeof(T)}");
            }

            base.OnSetup();
        }

        public virtual void RefreshSaveRequired()
        {
            if (IsInDesignMode)
            {
                return;
            }

            HasError = HasErrors || !ValidateConfiguration();
            HasChanges = !ConfigurationEqualsCurrent();
        }

        #endregion EventHandlers

        #region Methods

        /// <summary>
        /// Override this method in case the user want to validate other configuration fields.
        /// </summary>
        /// <returns><c>true</c> if the configuration is valid; otherwise <c>false</c>.</returns>
        protected virtual bool ValidateConfiguration()
        {
            var errors = ModifiedConfig?.ValidatedParameters();
            return string.IsNullOrWhiteSpace(errors);
        }

        protected virtual IConfigManager GetConfigManager() => App.Instance.ConfigurationManager;

        protected virtual bool ConfigurationEqualsLoaded() => ObjectAreEquals(ModifiedConfig, LoadedConfig);

        protected virtual bool ConfigurationEqualsCurrent() => ObjectAreEquals(ModifiedConfig, CurrentConfig);

        protected bool ShowOpenFileDialog<TPanel>(Expression<Func<TPanel, string>> memberLambda, string filter)
            where TPanel : ISetupPanel
        {
            var property = GetProperty(memberLambda);
            if (property is null)
            {
                return false;
            }

            var actualFilePath = (string)property.GetValue(this);

            var openFileDialog = new OpenFileDialog { Filter = filter, ReadOnlyChecked = true, ShowReadOnly = true };
            if (!string.IsNullOrEmpty(actualFilePath) && File.Exists(actualFilePath))
            {
                var fullPath = Path.GetFullPath(actualFilePath);
                openFileDialog.FileName = fullPath;
                openFileDialog.InitialDirectory = Path.GetDirectoryName(fullPath) ?? string.Empty;
            }
            else
            {
                openFileDialog.InitialDirectory =
                    !string.IsNullOrWhiteSpace(actualFilePath) && Directory.Exists(actualFilePath)
                        ? Path.GetFullPath(actualFilePath)
                        : AppContext.BaseDirectory;
            }

            if (openFileDialog.ShowDialog() != true
                || string.IsNullOrEmpty(openFileDialog.FileName)
                || !File.Exists(openFileDialog.FileName))
            {
                return false;
            }

            property.SetValue(this, GetRelativePath(openFileDialog.FileName));
            return true;
        }

        protected bool ShowOpenFolderDialog<TPanel>(Expression<Func<TPanel, string>> memberLambda)
            where TPanel : ISetupPanel
        {
            var property = GetProperty(memberLambda);
            if (property is null)
            {
                return false;
            }

            var initialFolderPath = (string)property.GetValue(this);

            var currentFullPath = !string.IsNullOrWhiteSpace(initialFolderPath) && Directory.Exists(initialFolderPath)
                ? Path.GetFullPath(initialFolderPath)
                : AppContext.BaseDirectory;

            var openFolderDialog =
                new FolderBrowserDialog { SelectedPath = currentFullPath, ShowNewFolderButton = false };

            if (openFolderDialog.ShowDialog() != DialogResult.OK
                || string.IsNullOrEmpty(openFolderDialog.SelectedPath)
                || !Directory.Exists(openFolderDialog.SelectedPath))
            {
                return false;
            }

            property.SetValue(this, GetRelativePath(openFolderDialog.SelectedPath));
            return true;
        }

        public static bool ObjectAreEquals<TObj>(TObj x, TObj y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xType = x.GetType();
            var yType = y.GetType();

            if (xType != yType)
            {
                return false;
            }

            using var xStream = GetJsonStream(x);
            using var yStream = GetJsonStream(y);

            if (xStream.Length != yStream.Length)
            {
                return false;
            }

            xStream.Position = yStream.Position = 0L;
            for (var index = 0; index < xStream.Length; ++index)
            {
                if (xStream.ReadByte() != yStream.ReadByte())
                {
                    return false;
                }
            }

            return true;
        }

        private static Stream GetJsonStream<TObj>(TObj value)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(writer);
            var serializer = new JsonSerializer();
            serializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
            return stream;
        }

        private static PropertyInfo GetProperty<TPanel>(Expression<Func<TPanel, string>> memberLambda)
        {
            if (memberLambda.Body is not MemberExpression memberSelectorExpression)
            {
                return null;
            }

            return memberSelectorExpression.Member is not PropertyInfo property ? null : property;
        }

        private static string GetRelativePath(string fullPath)
        {
            var relativePath = fullPath;
            if (!relativePath.StartsWith(AppContext.BaseDirectory))
            {
                return relativePath;
            }

            relativePath = relativePath.Replace(AppContext.BaseDirectory, string.Empty);
            return Path.Combine(".", relativePath);
        }

        #endregion Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _restartMessage?.Dispose();
                _unsavedMessage?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }

    public abstract class SetupPanel : SetupPanel<ApplicationConfiguration>
    {
        protected SetupPanel()
        {
        }

        protected SetupPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        protected override IConfigManager GetConfigManager() => App.Instance.ConfigurationManager;
    }

    public abstract class SetupNodePanel<TConfig, TNode> : SetupPanel<TConfig>
        where TConfig : class, IConfiguration, new()
    {
        #region Constructors

        protected SetupNodePanel()
        {
        }

        protected SetupNodePanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        #endregion Constructors

        #region Properties

        public TNode LoadedConfigNode => GetNode(LoadedConfig);

        public TNode CurrentConfigNode => GetNode(CurrentConfig);

        public TNode ModifiedConfigNode => GetNode(ModifiedConfig);

        #endregion Properties

        #region Override

        public override void OnSetup()
        {
            base.OnSetup();

            // Check specif configurations now that Config variable has been set in base class
            if (LoadedConfigNode == null)
            {
                throw new ConfigurationErrorsException(
                    $"{nameof(LoadedConfigNode)} is null or is not a {typeof(TNode)} in App.Instance");
            }

            if (CurrentConfigNode == null)
            {
                throw new ConfigurationErrorsException(
                    $"{nameof(CurrentConfigNode)} is null or is not a {typeof(TNode)} in App.Instance");
            }

            if (ModifiedConfigNode == null)
            {
                throw new ConfigurationErrorsException(
                    $"{nameof(ModifiedConfigNode)} is null or is not a {typeof(TNode)} in App.Instance");
            }
        }

        protected abstract TNode GetNode(TConfig rootConfig);

        protected override bool ConfigurationEqualsCurrent()
            => ConfigurationNodeAreEquals(ModifiedConfigNode, CurrentConfigNode);

        protected override bool ConfigurationEqualsLoaded()
            => ConfigurationNodeAreEquals(ModifiedConfigNode, LoadedConfigNode);

        protected virtual bool ConfigurationNodeAreEquals(TNode configNode1, TNode configNode2)
            => ObjectAreEquals(configNode1, configNode2);

        protected override bool ValidateConfiguration()
        {
            if (ModifiedConfigNode is not IConfiguration configNode)
            {
                return true;
            }

            var errors = configNode.ValidatedParameters();
            return string.IsNullOrWhiteSpace(errors);
        }

        #endregion Override
    }

    public abstract class SetupNodePanel<TNode> : SetupNodePanel<ApplicationConfiguration, TNode>
    {
        #region Constructors

        protected SetupNodePanel()
        {
        }

        protected SetupNodePanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        #endregion Constructors
    }
}
