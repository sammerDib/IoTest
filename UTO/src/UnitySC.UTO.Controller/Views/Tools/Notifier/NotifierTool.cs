using System;
using System.ComponentModel;
using System.Windows;

using Agileo.Common.Logging;
using Agileo.GUI.Components.Tools;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.GUI.Common.UIComponents.GuiExtended;
using UnitySC.GUI.Common.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.UTO.Controller.Views.Tools.Notifier
{
    public class NotifierTool : Tool
    {
        private NotifierVM _notifier;

        public NotifierVM Notifier
        {
            get => _notifier;
            set => SetAndRaiseIfChanged(ref _notifier, value);
        }

        private static readonly BadgedIcon BadgedIcon = new() { Icon = CustomPathIcon.EnabledNotifier };

        static NotifierTool()
        {
            DataTemplateGenerator.Create(typeof(NotifierTool), typeof(NotifierToolView));
        }
        public NotifierTool(string id) : base(id, BadgedIcon)
        {
            DisplayZone = VerticalAlignment.Bottom;
        }

        private void OnNotifierVmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Notifier.NbUnreadMessages))
            {
                BadgedIcon.Count = Notifier.NbUnreadMessages;
            }

            if (e.PropertyName == nameof(Notifier.NotificationEnabled))
            {
                UpdateIcon();
            }
        }

        private void UpdateIcon()
        {
            BadgedIcon.Icon = Notifier.NotificationEnabled
                ? CustomPathIcon.EnabledNotifier
                : CustomPathIcon.DisabledNotifier;
        }

        #region Overrides of Tool

        protected override void Dispose(bool disposing)
        {
            if (disposing && Notifier != null)
            {
                Notifier.PropertyChanged -= OnNotifierVmPropertyChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        public void Initialize()
        {
            Notifier = ClassLocator.Default.GetInstance<NotifierVM>();

            // Default OnSetup cannot be used due to ClassLocator constraints
            Notifier.PropertyChanged += OnNotifierVmPropertyChanged;
            var messenger= ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ClearAllMessages>(this, (r, m) => ClearAllMessages(m));
            messenger.Register<ClearMessage>(this, (r, m) => ClearMessage(m));

        }

        private void ClearMessage(ClearMessage m)
        {
            var dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            var sharedSupervisors = ClassLocator.Default.GetInstance<SharedSupervisors>();
            foreach (var actorType in dfClientConfiguration.AvailableModules)
            {
                try
                {
                    sharedSupervisors.GetGlobalStatusSupervisor(actorType).ClearMessage(m.MessageToClear);
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(nameof(NotifierTool)).Error(ex);
                }
            }
        }

        private void ClearAllMessages(ClearAllMessages m)
        {
            var dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            var sharedSupervisors = ClassLocator.Default.GetInstance<SharedSupervisors>();
            foreach (var actorType in dfClientConfiguration.AvailableModules)
            {
                try
                {
                    sharedSupervisors.GetGlobalStatusSupervisor(actorType).ClearAllMessages();
                }
                catch (Exception ex)
                {
                    Logger.GetLogger(nameof(NotifierTool)).Error(ex);
                }
            }
        }
    }
}
