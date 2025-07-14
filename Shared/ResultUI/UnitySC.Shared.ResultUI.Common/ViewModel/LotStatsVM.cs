using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public class LotStatsVM : LotViewHeaderVM
    {
        private readonly IMessenger _messenger = ClassLocator.Default.GetInstance<IMessenger>();


        #region Overrides of LotViewHeaderVM

        protected override void OnSelectedResultFullNameChanged(string name)
        {
            // Nothing to do
        }

        protected override void OnLotSelectedViewChanged(KeyValuePair<LotView, string> selectedView)
        {
            _messenger.Send(new DisplayManageLotViewMessage { SelectedStatsLotview = selectedView });
        }

        protected override void Cleanup()
        {
            // Nothing to do
        }

        #endregion

        protected LotStatsVM()
        {
        }

        public virtual void UpdateKlarfSettings(object settingsdata)
        {
        }

        public virtual void UpdateStats(object stats) // to define more accuretly
        {
        }

        public virtual void SelectLotView(object lotview)
        {
        }
    }
}
