using System;
using System.Configuration;
using System.Runtime.CompilerServices;

using Agileo.Common.Configuration;
using Agileo.Common.Logging;
using Agileo.Common.Tracing;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup
{
    public abstract class BaseNodeEditor<TConfig, TNode> : BaseEditor<TConfig>
        where TConfig : class, IConfiguration, new()
    {
        protected BaseNodeEditor(ILogger logger)
            : base(logger)
        {
        }

        #region Properties

        public TNode LoadedConfigNode => GetNode(LoadedConfig);

        public TNode CurrentConfigNode => GetNode(CurrentConfig);

        public TNode ModifiedConfigNode => GetNode(ModifiedConfig);

        #endregion Properties

        #region Overrides of BaseEditor<TConfig>

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

        public override bool ConfigurationEqualsCurrent()
            => SetupPanel.ObjectAreEquals(ModifiedConfigNode, CurrentConfigNode);

        #endregion

        #region Abstract Methods

        protected abstract TNode GetNode(TConfig rootConfig);

        #endregion
    }

    public abstract class BaseEditor : NotifyDataError
    {
        protected BaseEditor(ILogger logger)
        {
            Logger = logger;
        }

        #region Properties

        protected ILogger Logger { get; }

        protected IConfigManager ConfigManager { get; private set; }

        #endregion

        #region Methods

        protected abstract IConfigManager GetConfigManager();

        protected void Set<TY>(
            (Func<TY> get, Action<TY> set) property,
            TY newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (property.get().Equals(newValue))
            {
                return;
            }

            property.set(newValue);
            OnPropertyChanged(propertyName);
        }

        public abstract bool CurrentConfigurationEqualsLoaded();

        public abstract bool ConfigurationEqualsCurrent();

        public abstract void SaveConfig();

        public abstract void UndoChanges();

        public virtual void OnSetup() => ConfigManager = GetConfigManager();

        #endregion
    }

    public abstract class BaseEditor<T> : NotifyDataError
        where T : class, IConfiguration, new()
    {
        protected BaseEditor(ILogger logger)
        {
            Logger = logger;
        }

        #region Properties

        protected ILogger Logger { get; }

        protected IConfigManager ConfigManager { get; private set; }

        protected T LoadedConfig => ConfigManager?.Loaded as T;

        protected T CurrentConfig => ConfigManager?.Current as T;

        public T ModifiedConfig => ConfigManager?.Modified as T;

        #endregion

        #region Methods

        protected abstract IConfigManager GetConfigManager();

        protected void Set<TY>(
            (Func<TY> get, Action<TY> set) property,
            TY newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (property.get().Equals(newValue))
            {
                return;
            }

            property.set(newValue);
            OnPropertyChanged(propertyName);
        }

        public virtual bool ConfigurationEqualsCurrent() => SetupPanel.ObjectAreEquals(ModifiedConfig, CurrentConfig);

        public virtual void SaveConfig()
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

        public virtual void UndoChanges()
        {
            if (ConfigManager.IsApplyRequired)
            {
                ConfigManager.Undo();
            }

            OnPropertyChanged(null);
        }

        public virtual void OnSetup() => ConfigManager = GetConfigManager();

        #endregion
    }
}
