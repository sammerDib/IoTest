using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.PatternRec
{
    public class PatternRecFlowFDCProvider : FlowFDCProvider
    {
        private const string ANA_PatternRecSuccessRatio = "ANA_PatternRecSuccessRatio";
        private readonly List<string> _providerFDCNames = new List<string>();

        public PatternRecFlowFDCProvider() : base(ANA_PatternRecSuccessRatio)
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
            _providerFDCNames.Add(ANA_PatternRecSuccessRatio);
        }
    }
}
