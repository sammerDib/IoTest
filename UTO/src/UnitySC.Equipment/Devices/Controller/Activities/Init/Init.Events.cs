using Agileo.StateMachine;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    internal partial class Init
    {
        private class AnyConnectionStateChanged : Event
        {
        }

        private class AllDevicesConnected : Event
        {
        }

        private class ConnectAllStarted : Event
        {
        }

        private class LightTowerDone : Event
        {
        }

        private class RobotDone : Event
        {
        }

        private class RobotFailed : Event
        {
        }

        private class AlignerStarted : Event
        {
        }

        private class AllLoadPortsStarted : Event
        {
        }

        private class AllProcessModulesStarted : Event
        {
        }

        private class AllSubstrateIdReaders : Event
        {
        }

        private class FfuInitStarted : Event
        {
        }

        private class AnyCommandDone : Event
        {
        }

        private class AllDevicesInitialized : Event
        {
        }

        private class DataFlowManagerStarted : Event
        {
        }

        private class DiosInitialized : Event
        {
        }
    }
}
