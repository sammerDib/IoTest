using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

using MergeContext;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.Ada
{
    public class EnterProductionInfoViewModel: ClosableViewModel
    {
        public EnterProductionInfoViewModel()
        {
            ProductionInfo=new ADCRemoteProductionInfo();
        }
        public ADCRemoteProductionInfo ProductionInfo { get; set; }

        public bool IsValidated { get; internal set; } = false;

        private AutoRelayCommand _okCommand;

        private bool AreProductionInfoValid()
        {
            if (!string.IsNullOrEmpty(ProductionInfo.WaferID) &&
                !string.IsNullOrEmpty(ProductionInfo.LotID) &&
                !string.IsNullOrEmpty(ProductionInfo.JobID) &&
                ProductionInfo.SlotID>=0)
                return true;
            else
                return false;



        }

        public AutoRelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        CloseSignal = true;
                    },
                    () => { return AreProductionInfoValid(); }
                ));
            }
        }

        
    }
}
