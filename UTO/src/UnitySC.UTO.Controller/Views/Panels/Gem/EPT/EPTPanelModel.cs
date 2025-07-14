using System;
using System.Linq;
using Agileo.Common.Localization;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.Semi.Gem300.Abstractions.E116;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.EPT
{
    public class EptPanelModel : BusinessPanel
    {

        #region Constructors

        static EptPanelModel()
        {
            DataTemplateGenerator.Create(typeof(EptPanelModel), typeof(EPTView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(EPTRessources)));
        }

        public EptPanelModel() : this($"{nameof(EptPanelModel)} DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public EptPanelModel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            #region Substrates

            EptSource = new DataTableSource<IEPTTracker>();

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_ELEMENT_NAME),
                ept => ept.EPTElementName,
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_ELEMENT_TYPE),
                ept => ept.EPTElementType.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_STATE),
                ept => ept.EPTState.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_STATE_TIME),
                ept => ept.EPTStateTime.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_TASK_NAME),
                ept => ept.TaskName,
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_TASK_TYPE),
                ept => ept.TaskType.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_PREVIOUS_STATE),
                ept => ept.PreviousEPTState.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_PREVIOUS_TASK_NAME),
                ept => ept.PreviousTaskName,
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_PREVIOUS_TASK_TYPE),
                ept => ept.PreviousTaskType.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_BLOCKED_REASON),
                ept => ept.BlockedReason.ToString(),
                true);

            _ = EptSource.Search.AddSearchDefinition(
                nameof(EPTRessources.EPT_BLOCKED_REASON_TEXT),
                ept => ept.BlockedReasonText,
                true);
            #endregion

            ResetEptTrackers();
        }

        #endregion

        #region Properties

        public static IE116Standard E116Standard => App.ControllerInstance.GemController.E116Std;

        public DataTableSource<IEPTTracker> EptSource { get; }

        #endregion

        #region other methods

        private void ResetEptTrackers()
        {
            if (App.ControllerInstance.GemController.IsSetupDone)
            {
                var trackers = E116Standard.Trackers.ToList();
                DispatcherHelper.DoInUiThreadAsynchronously(() => EptSource.Reset(trackers));
            }
        }

        #endregion

        #region Event handlers

        private void E116Standard_TrackerChanged(object sender, EPTTrackerStateChangedEventArgs e)
        {
            ResetEptTrackers();
        }

        #endregion

        #region Overrides of BusinessPanel

        public override void OnSetup()
        {
            base.OnSetup();

            if (E116Standard == null) return;

            E116Standard.TrackerChanged += E116Standard_TrackerChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (E116Standard != null && disposing)
            {
                E116Standard.TrackerChanged -= E116Standard_TrackerChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
