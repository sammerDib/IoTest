using System.Collections.Generic;

using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    public abstract class FlowFDCProvider : IFDCProvider
    {
        protected readonly FDCManager FdcManager;
        protected readonly IGlobalStatusServer GlobalStatusServer;

        private readonly string _fdcSuccesRatioName;
        private PersistentFDCRate<double> _fdcFlowSuccessRate;

        protected FlowFDCProvider(string fdcSuccesRatioName)
        {
            FdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            GlobalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            _fdcSuccesRatioName = fdcSuccesRatioName;
            _fdcFlowSuccessRate = new PersistentFDCRate<double>(_fdcSuccesRatioName);
        }

        public virtual FDCData GetFDC(string fdcName)
        {
            if (fdcName.Equals(_fdcSuccesRatioName))
            {
                var flowSuccessPercent = _fdcFlowSuccessRate.GetPercentageRate();
                return FDCData.MakeNew(_fdcSuccesRatioName, flowSuccessPercent, "%");
            }

            return null;
        }

        public virtual void ResetFDC(string fdcName)
        {
            if (fdcName.Equals(_fdcSuccesRatioName))
            {
                _fdcFlowSuccessRate.Clear();
                FdcManager.SetPersistentFDCData(_fdcFlowSuccessRate);
            }
        }
        public virtual void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            // Nothing to do
        }

        public virtual void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            if (fdcName.Equals(_fdcSuccesRatioName))
            {
                if (persistentFDCData is PersistentFDCRate<double> pfdcRatio)
                {
                    _fdcFlowSuccessRate = pfdcRatio;
                }
            }
        }

        public abstract void StartFDCMonitor();

        public abstract void Register();

        public void CreateFDC()
        {
            if (GlobalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                var FlowSuccessPercent = _fdcFlowSuccessRate.GetPercentageRate();
                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(_fdcSuccesRatioName, FlowSuccessPercent, "%")
                };

                FdcManager.SendFDCs(fdcList);
            }
        }

        public void IncrementFlowSuccessCounter()
        {
            if (GlobalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcFlowSuccessRate.Amount++;
                FdcManager.SetPersistentFDCData(_fdcFlowSuccessRate);
            }
        }

        public void IncrementFlowTotalCounter()
        {
            if (GlobalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcFlowSuccessRate.Total++;
                FdcManager.SetPersistentFDCData(_fdcFlowSuccessRate);
            }
        }
    }
}
