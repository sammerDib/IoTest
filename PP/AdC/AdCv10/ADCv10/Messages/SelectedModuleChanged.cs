using ADC.ViewModel;

namespace ADC.Messages
{
    public class SelectedModuleChanged
    {
        public SelectedModuleChanged(object sender)
        {
            Sender = sender;
        }

        public ModuleNodeViewModel OldModule { get; set; }

        public ModuleNodeViewModel NewModule { get; set; }

        public object Sender { get; set; }
    }
}
