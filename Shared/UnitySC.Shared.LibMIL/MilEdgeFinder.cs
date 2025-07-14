using System;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilEdgeFinder : AMilId
    {
        public string Name { get; set; }

        //=================================================================
        // Alloc Model
        //=================================================================
        public void Alloc(MIL_ID systemId, long edgeFinderType)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing edge finder");

            MIL.MedgeAlloc(systemId, edgeFinderType, MIL.M_DEFAULT, ref _milId);
            AMilId.checkMilError("Failed to allocate edge finder");
        }

        //=================================================================
        // Load a model from disk
        //=================================================================
        public void Restore(string fileName, MIL_ID systemId)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing edge finder");

            Name = System.IO.Path.GetFileNameWithoutExtension(fileName);
            MIL.MedgeRestore(fileName, systemId, MIL.M_DEFAULT, ref _milId);
            AMilId.checkMilError("Failed to load edge finder \"" + fileName + "\"");
        }

        //=================================================================
        // Load a model from memory
        //=================================================================
        public long Stream(byte[] memPtr, MIL_ID systemId, long operation, long streamType, double version = MIL.M_DEFAULT, long controlFlag = MIL.M_DEFAULT)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing edge finder");

            MIL_INT size = 0;
            MIL.MedgeStream(memPtr, systemId, operation, streamType, version, controlFlag, ref _milId, ref size);
            AMilId.checkMilError("Failed to stream edge finder");
            return size;
        }

        //=================================================================
        // Control
        //=================================================================
        public void Control(long controlType, double controlValue)
        {
            MIL.MedgeControl(_milId, controlType, controlValue);
            AMilId.checkMilError("Failed to control edge finder");
        }

        //=================================================================
        // Other
        //=================================================================
        public MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MedgeInquire(_milId, MIL.M_OWNER_SYSTEM); }
        }

        public double FilerSmoothness
        {
            get { double val = double.NaN; MIL.MedgeInquire(_milId, MIL.M_FILTER_SMOOTHNESS, ref val); return val; }
            set { Control(MIL.M_FILTER_SMOOTHNESS, value); }
        }

        public int FillGapDistance
        {
            get { int val = 0; MIL.MedgeInquire(_milId, MIL.M_FILL_GAP_DISTANCE, ref val); return val; }
            set { Control(MIL.M_FILL_GAP_DISTANCE, value); }
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MedgeFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }
    }
}