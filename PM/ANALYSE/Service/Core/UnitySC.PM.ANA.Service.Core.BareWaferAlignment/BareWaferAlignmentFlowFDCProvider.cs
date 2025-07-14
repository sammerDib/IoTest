using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.BareWaferAlignment
{
    public class BareWaferAlignmentFlowFDCProvider : FlowFDCProvider
    {
        private Length _fdcWaferAlignmentShiftX;
        private Length _fdcWaferAlignmentShiftY;
        private Angle _fdcWaferAlignmentAngle;

        private const string ANA_WaferAlignmentShiftX = "ANA_WaferAlignmentShiftX";
        private const string ANA_WaferAlignmentShiftY = "ANA_WaferAlignmentShiftY";
        private const string ANA_WaferAlignmentAngle = "ANA_WaferAlignmentAngle";
        private const string ANA_WaferAlignmentSuccessRatio = "ANA_WaferAlignmentSuccessRatio";
        private readonly List<string> _providerFDCNames = new List<string>();

        public BareWaferAlignmentFlowFDCProvider() : base(ANA_WaferAlignmentSuccessRatio)
        {
        }

        public override FDCData GetFDC(string fdcName)
        {
            var fdc = base.GetFDC(fdcName);

            switch (fdcName)
            {
                case ANA_WaferAlignmentShiftX:
                    if (_fdcWaferAlignmentShiftX != null)
                    {
                        fdc = FDCData.MakeNew(ANA_WaferAlignmentShiftX, _fdcWaferAlignmentShiftX.Micrometers, "µm");
                    }
                    break;

                case ANA_WaferAlignmentShiftY:
                    if (_fdcWaferAlignmentShiftY != null)
                    {
                        fdc = FDCData.MakeNew(ANA_WaferAlignmentShiftY, _fdcWaferAlignmentShiftY.Micrometers, "µm");
                    }
                    break;

                case ANA_WaferAlignmentAngle:
                    if (_fdcWaferAlignmentAngle != null)
                    {
                        fdc = FDCData.MakeNew(ANA_WaferAlignmentAngle, _fdcWaferAlignmentAngle.Degrees, "°");
                    }
                    break;
            }

            return fdc;
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

        public void CreateFDC(Interface.Algo.BareWaferAlignmentResult result)
        {
            if (GlobalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcWaferAlignmentShiftX = result.ShiftX;
                _fdcWaferAlignmentShiftY = result.ShiftY;
                _fdcWaferAlignmentAngle = result.Angle;

                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(ANA_WaferAlignmentShiftX, _fdcWaferAlignmentShiftX.Micrometers, "µm"),
                    FDCData.MakeNew(ANA_WaferAlignmentShiftY, _fdcWaferAlignmentShiftY.Micrometers, "µm"),
                    FDCData.MakeNew(ANA_WaferAlignmentAngle, _fdcWaferAlignmentAngle.Degrees, "°"),
                };

                FdcManager.SendFDCs(fdcList);
            }
        }

        private void InitProviderFDCNames()
        {
            _providerFDCNames.Add(ANA_WaferAlignmentShiftX);
            _providerFDCNames.Add(ANA_WaferAlignmentShiftY);
            _providerFDCNames.Add(ANA_WaferAlignmentAngle);
            _providerFDCNames.Add(ANA_WaferAlignmentSuccessRatio);
        }
    }
}
