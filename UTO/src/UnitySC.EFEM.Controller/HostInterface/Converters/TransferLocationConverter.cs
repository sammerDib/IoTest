using System;

using Agileo.SemiDefinitions;

namespace UnitySC.EFEM.Controller.HostInterface.Converters
{
    public static class TransferLocationConverter
    {
        public static TransferLocation ToTransferLocation(Constants.Stage stage)
        {
            switch (stage)
            {
                case Constants.Stage.LP1:
                    return TransferLocation.LoadPort1;

                case Constants.Stage.LP2:
                    return TransferLocation.LoadPort2;

                case Constants.Stage.LP3:
                    return TransferLocation.LoadPort3;

                case Constants.Stage.LP4:
                    return TransferLocation.LoadPort4;

                case Constants.Stage.Tilt:
                    return TransferLocation.ProcessModuleA;

                case Constants.Stage.Aligner:
                    return TransferLocation.PreAlignerA;

                case Constants.Stage.TiltPort2:
                    return TransferLocation.ProcessModuleB;

                case Constants.Stage.TiltPort3:
                    return TransferLocation.ProcessModuleC;

                case Constants.Stage.TiltPort4:
                    return TransferLocation.ProcessModuleD;

                default:
                    throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
            }
        }

        public static Constants.Stage ToStage(TransferLocation transferLocation)
        {
            switch (transferLocation)
            {
                case TransferLocation.LoadPort1:
                    return Constants.Stage.LP1;

                case TransferLocation.LoadPort2:
                    return Constants.Stage.LP2;

                case TransferLocation.LoadPort3:
                    return Constants.Stage.LP3;

                case TransferLocation.LoadPort4:
                    return Constants.Stage.LP4;

                case TransferLocation.PreAlignerA:
                    return Constants.Stage.Aligner;

                case TransferLocation.ProcessModuleA:
                    return Constants.Stage.Tilt;

                case TransferLocation.ProcessModuleB:
                    return Constants.Stage.TiltPort2;

                case TransferLocation.ProcessModuleC:
                    return Constants.Stage.TiltPort3;

                case TransferLocation.ProcessModuleD:
                    return Constants.Stage.TiltPort4;

                case TransferLocation.LoadPort5:
                case TransferLocation.LoadPort6:
                case TransferLocation.LoadPort7:
                case TransferLocation.LoadPort8:
                case TransferLocation.LoadPort9:
                case TransferLocation.LoadLockA:
                case TransferLocation.LoadLockB:
                case TransferLocation.LoadLockC:
                case TransferLocation.LoadLockD:
                case TransferLocation.PreAlignerB:
                case TransferLocation.PreAlignerC:
                case TransferLocation.PreAlignerD:
                case TransferLocation.DummyPortA:
                case TransferLocation.DummyPortB:
                case TransferLocation.DummyPortC:
                case TransferLocation.DummyPortD:
                case TransferLocation.Robot:
                    throw new NotImplementedException(
                        $"{nameof(TransferLocation)} \"{transferLocation}\" is not yet recognized by the system.");

                default:
                    throw new ArgumentOutOfRangeException(nameof(transferLocation), transferLocation, null);
            }
        }
    }
}
