using System;

using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Controller.Configuration;
using UnitySC.EFEM.Controller.HostInterface.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.EFEM.Controller.Views.Panels.Setup.HostInterface
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel
    /// </summary>
    public class HostInterfacePanel : SetupNodePanel<EfemControllerConfiguration, HostConfiguration>
    {
        #region Constructors

        static HostInterfacePanel()
        {
            DataTemplateGenerator.Create(typeof(HostInterfacePanel), typeof(HostInterfacePanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(HostInterfacePanelResources)));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SetupPanelResources)));
        }

        public HostInterfacePanel() : base("DesignTime constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostInterfacePanel"/> class.
        /// </summary>
        /// <param name="relativeId">Graphical identifier of the View Model. Can be either a <seealso cref="string"/> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public HostInterfacePanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
        }

        #endregion Constructors

        #region Properties

        public string IpAddress
        {
            get => ModifiedConfigNode?.IpAddress ?? string.Empty;
            set => ModifiedConfigNode.IpAddress = value;
        }

        public uint TcpPort
        {
            get => ModifiedConfigNode?.TcpPort ?? 0;
            set => ModifiedConfigNode.TcpPort = value;
        }

        #endregion Properties

        #region Overrides

        protected override bool ChangesNeedRestart => true;

        protected override HostConfiguration GetNode(EfemControllerConfiguration rootConfig) =>
            rootConfig?.HostConfiguration;

        #endregion Overrides
    }
}
