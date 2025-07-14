using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.AutofocusV2
{
    public class AFCameraFlowFDCProvider : FlowFDCProvider
    {
        private const string ANA_AutofocusCameraSuccessRatio = "ANA_AutofocusCameraSuccessRatio";
        private readonly List<string> _providerFDCNames = new List<string>();

        public AFCameraFlowFDCProvider() : base(ANA_AutofocusCameraSuccessRatio)
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
            _providerFDCNames.Add(ANA_AutofocusCameraSuccessRatio);
        }
    }
}
