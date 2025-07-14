using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.UTO.Controller.Views.Panels.DataFlow.Integration;
using UnitySC.UTO.Controller.Views.Tools.Notifier;

namespace UnitySC.UTO.Controller.Views.Panels.Integration
{
    public abstract class BaseUnityIntegrationPanel : BusinessPanel
    {
        private static readonly List<BaseUnityIntegrationPanel> Instances = new();

        #region Fields

        private static readonly MessengerHandler MessengerHandler = new();
        private static bool _registered;
        private static bool _firstShow;

        #endregion

        protected BaseUnityIntegrationPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Instances.Add(this);
        }

        #region Virtual methods

        protected virtual void Register()
        {
        }

        #endregion

        #region Private methdos

        private static void InternalRegister()
        {
            if (_registered)
            {
                return;
            }

            SerilogInit.InitWithCurrentAppConfig();

            // Message
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            Result.CommonUI.Bootstrapper.Register();

            _registered = true;
        }

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();

            InternalRegister();
            Register();

            MessengerHandler.SubscribePanel(this);
        }

        #region Overrides of BusinessPanel

        public override void OnShow()
        {
            base.OnShow();

            if (!_firstShow)
            {
                OnFirstShow();
                _firstShow = true;
            }
        }

        protected virtual void OnFirstShow()
        {
        }

        #endregion

        #endregion

        #region Protected

        protected static void RegisterUserSupervisor()
        {
            TryRegister<IUserSupervisor>(typeof(UserSupervisorIntegration), true);
        }

        protected static void RegisterIDbRecipeService()
        {
            TryRegister<ServiceInvoker<IDbRecipeService>>(
                () => new ServiceInvoker<IDbRecipeService>(
                    "RecipeService",
                    ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(),
                    ClassLocator.Default.GetInstance<IMessenger>(),
                    ClientConfiguration.GetDataAccessAddress()));
        }

        protected static void RegisterExternalUserControls()
        {
            var dfClientConfiguration = GetDfClientConfiguration();

            if (Registered<ExternalUserControls>())
            {
                return;
            }

            var externalControls = new ExternalUserControls(new SerilogLogger<object>());
            TryRegister(() => externalControls);
            externalControls.Init(dfClientConfiguration.ExternalUserControlsDir);

            try
            {
                foreach (var pmInit in externalControls.PmInits)
                {
                        pmInit.Value.BootStrap();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        protected static void RegisterSharedSupervisors()
        {
            // PMs Shared supervisors
            TryRegister<SharedSupervisors>(true);
        }

        private static DFClientConfiguration _currentDfClientConfiguration;

        protected static DFClientConfiguration GetDfClientConfiguration()
        {
            if (_currentDfClientConfiguration != null)
            {
                return _currentDfClientConfiguration;
            }

            var dfClientConfigurationPath = App.ControllerInstance.ControllerConfig.ApplicationPath
                .DfClientConfigurationFolderPath;

            if (string.IsNullOrEmpty(dfClientConfigurationPath))
            {
                dfClientConfigurationPath =
                    new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
                dfClientConfigurationPath = Path.Combine(
                    dfClientConfigurationPath,
                    "Configuration\\XML");
            }

            var dfClientConfigurationFilePath = Path.Combine(
                dfClientConfigurationPath,
                "DFClientConfiguration.xml");

            if (!File.Exists(dfClientConfigurationFilePath))
            {
                throw new Exception(
                    $"Missing dataflow config file <{dfClientConfigurationFilePath}>");
            }

            _currentDfClientConfiguration =
                DFClientConfiguration.Init(dfClientConfigurationFilePath);
            ClassLocator.Default.Register<IDFClientConfiguration>(
                () => _currentDfClientConfiguration,
                true);

            return _currentDfClientConfiguration;
        }

        #endregion

        private static readonly Dictionary<Type, object> Registrations = new();

        private static bool TryRegister<T>(Type implementationType, bool singleton)
            where T : class
        {
            if (Registrations.ContainsKey(implementationType))
            {
                return false;
            }

            Registrations.Add(implementationType, null);
            ClassLocator.Default.Register(typeof(T), implementationType, singleton);
            return true;
        }

        private static bool TryRegister<T>(Func<T> createInstance)
            where T : class
        {
            if (Registrations.ContainsKey(typeof(T)))
            {
                return false;
            }

            Registrations.Add(typeof(T), null);
            ClassLocator.Default.Register(createInstance);
            return true;
        }

        private static bool TryRegister<T>(bool singleton)
            where T : class
        {
            if (Registrations.ContainsKey(typeof(T)))
            {
                return false;
            }

            Registrations.Add(typeof(T), null);
            ClassLocator.Default.Register<T>(singleton);
            return true;
        }

        private static bool Registered<T>()
        {
            return Registrations.ContainsKey(typeof(T));
        }

        public static void FinalizeSetup()
        {
            // ClassLocator initialization must be done to allow GetInstance of IMessenger.
            MessengerHandler.Initialize();

            var notifierTool = GUI.Common.App.Instance.UserInterface.ToolManager.Tools
                .OfType<NotifierTool>()
                .SingleOrDefault();
            notifierTool?.Initialize();

            foreach (var instance in Instances)
            {
                instance.OnFinalizeSetup();
            }
        }

        public virtual void OnFinalizeSetup()
        {
        }
    }
}
