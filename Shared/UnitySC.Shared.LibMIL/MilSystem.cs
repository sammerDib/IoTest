using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilSystem : AMilId
    {
        private bool _ownsSystem = false;

        public void Attach(MIL_ID systemId, bool transferOwnership = false)
        {
            _milId = systemId;
            _ownsSystem = transferOwnership;
        }

        public void Allocate(string systemDescriptor, long systemNum = MIL.M_DEFAULT, long initFlag = MIL.M_DEFAULT)
        {
            _ownsSystem = true;
            MIL.MsysAlloc(Mil.Instance.Application, systemDescriptor, systemNum, initFlag, ref _milId);
        }

        public void Control(long controlType, MIL_INT controlValue)
        {
            MIL.MappControl(MilId, controlType, controlValue);
            AMilId.checkMilError("Could not control MIL application");
        }

        public MIL_INT SystemType => MIL.MsysInquire(MilId, MIL.M_SYSTEM_TYPE);
        public long DigitizerNum => MIL.MsysInquire(MilId, MIL.M_DIGITIZER_NUM);

        public void Free()
        {
            if (_milId != MIL.M_NULL)
                MIL.MsysFree(_milId);
            _milId = MIL.M_NULL;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_ownsSystem)
                return;

            Free();
        }
    }
}