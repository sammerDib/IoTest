using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    internal class MilMeasureResult : AMilId
    {
        public MilMeasureMarker ParentMarker { get; private set; }

        public MilMeasureResult(MilMeasureMarker marker)
        {
            ParentMarker = marker;
        }

        public void Alloc(MIL_ID systemId, long resultType = MIL.M_CALCULATE)
        {
            MIL.MmeasAllocResult(systemId, resultType, ref _milId);
            checkMilError("MmeasAllocResult");
        }

        //public void Calculate(MIL_ID ContextId, MIL_ID Marker1Id, MIL_ID Marker2Id, long MeasurementList)
        //{
        //    MIL.MmeasCalculate(MIL_ID ContextId, MIL_ID Marker1Id, MIL_ID Marker2Id, MIL_ID MeasResultId, long MeasurementList);
        //}

        public void Free()
        {
            if (_milId != 0)
            {
                MIL.MmeasFree(_milId);
                _milId = 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Free();
        }
    }
}