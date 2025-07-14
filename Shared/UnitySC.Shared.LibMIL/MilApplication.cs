using System;
using System.Text;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilApplication : AMilId
    {
        private bool ownsApplication = false;

        public void Attach(MIL_ID applicationId, bool transferOwnership = false)
        {
            Console.WriteLine("Initialize MIL from external Application/System ID...");
            _milId = applicationId;
            ownsApplication = transferOwnership;
        }

        public void Allocate(long InitFlag)
        {
            ownsApplication = true;

            Console.WriteLine("Allocating MIL application ...");
            MIL.MappAlloc("M_DEFAULT", InitFlag, ref _milId);
            AMilId.checkMilError("Could not allocate MIL application");
        }

        public void Control(long ControlType, MIL_INT ControlValue)
        {
            MIL.MappControl(MilId, ControlType, ControlValue);
            AMilId.checkMilError("Could not control MIL application");
        }

        public long InstalledSystemCount => MIL.MappInquire(MilId, MIL.M_INSTALLED_SYSTEM_COUNT);

        public string InstalledSystemPrintName(int n)
        {
            StringBuilder sb = new StringBuilder();
            MIL.MappInquire(MilId, MIL.M_INSTALLED_SYSTEM_PRINT_NAME + n, sb);
            return sb.ToString();
        }

        //=================================================================
        // Free
        // On ne peut appeler Free() que depuis le thread qui a appelé
        // Allocate(). Donc on ne peut pas libérer automatiquement dans le
        // destructeur :-(
        //=================================================================
        public void Free()
        {
            if (_milId != MIL.M_NULL)
                MIL.MappFree(_milId);
            _milId = MIL.M_NULL;
        }

        protected override void Dispose(bool disposing)
        {
            if (!ownsApplication)
                return;

            Free();
        }
    }
}
