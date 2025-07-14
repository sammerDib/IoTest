using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.Autofocus
{
    public class AFLiseFlowFDCProvider : FlowFDCProvider
    {
        private const string ANA_AutofocusLiseSuccessRatio = "ANA_AutofocusLiseSuccessRatio";
        private readonly List<string> _providerFDCNames = new List<string>();

        public AFLiseFlowFDCProvider() : base(ANA_AutofocusLiseSuccessRatio)
        {
        }

        public override void Register()
        {
            InitProviderFDCNames();
            FdcManager.RegisterFDCProvider(this, _providerFDCNames);
        }

        public override void StartFDCMonitor()
        {
            // Nothing to do
        }

        private void InitProviderFDCNames()
        {
            _providerFDCNames.Add(ANA_AutofocusLiseSuccessRatio);
        }
    }
}
