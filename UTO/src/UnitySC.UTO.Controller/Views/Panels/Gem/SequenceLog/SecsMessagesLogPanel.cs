using Agileo.GUI.Services.Icons;
using Agileo.Semi.Communication.Abstractions;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SequenceLog
{
    public class SecsMessagesLogPanel : BaseSequenceLogPanel
    {
        private bool _isShown;

        public SecsMessagesLogPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        private void OnMessageExchanged(object sender, MessageEventArgs e)
        {
            SequenceTableSource.Add(ToSecsMessage(e.Message));

            if (_isShown)
            {
                // Updating filters requires browsing the collection.
                // To optimize performance, this call is only made when the panel is visible.
                SequenceTableSource.UpdateFilterPossibleValues();
            }
        }

        #region Overrides of BusinessPanel

        public override void OnShow()
        {
            base.OnShow();
            _isShown = true;

            SequenceTableSource.UpdateFilterPossibleValues();
        }

        public override void OnHide()
        {
            _isShown = false;
            base.OnHide();
        }

        #endregion

        #region Overrides of IdentifiableElement

        public override void OnSetup()
        {
            base.OnSetup();

            App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageReceived += OnMessageExchanged;
            App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageSent += OnMessageExchanged;
            App.ControllerInstance.GemController.E30Std.Connection.SecondaryMessageReceived += OnMessageExchanged;
            App.ControllerInstance.GemController.E30Std.Connection.SecondaryMessageSent += OnMessageExchanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageReceived -= OnMessageExchanged;
                App.ControllerInstance.GemController.E30Std.Connection.PrimaryMessageSent -= OnMessageExchanged;
                App.ControllerInstance.GemController.E30Std.Connection.SecondaryMessageReceived -= OnMessageExchanged;
                App.ControllerInstance.GemController.E30Std.Connection.SecondaryMessageSent -= OnMessageExchanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
