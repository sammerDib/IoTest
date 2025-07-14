using System;
using System.Collections.Generic;
using System.Linq;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    static public class MilTo
    {
        static public List<MIL_INT> StatList(params MIL_INT[] statTypes) { return statTypes.ToList(); }
    }

    public class MilImageResult : AMilId
    {
        protected MIL_ID StatContextId = MIL.M_NULL;

        public long ResultSize { get => MIL.MimInquire(MilId, MIL.M_RESULT_SIZE); }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MimFree(_milId);
                _milId = MIL.M_NULL;
            }

            if (StatContextId != MIL.M_NULL)
            {
                MIL.MimFree(StatContextId);
                StatContextId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        //
        //=================================================================
        public void AllocResult(MIL_ID sysId, long nbEntries, long resultType)
        {
            MIL.MimAlloc(sysId, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref StatContextId);
            MIL.MimAllocResult(sysId, nbEntries, resultType, ref _milId);
        }

        //=================================================================
        //
        //=================================================================
        public void GetResult(long resultType, out long[] data)
        {
            data = new long[ResultSize];
            MIL.MimGetResult(_milId, resultType, data);
        }

        public double GetResult(long resultType)
        {
            double val = double.NaN;
            MIL.MimGetResult(_milId, resultType, ref val);
            return val;
        }

        //=================================================================
        //
        //=================================================================
        public void Stat(MilImage milImage, List<MIL_INT> statTypes, MIL_INT? condition = null, double? lowCond = null, double? highCond = null)
        {

            foreach (var statType in statTypes)
            {
                MIL.MimControl(StatContextId, statType, MIL.M_ENABLE);
            }

            MIL.MimControl(StatContextId, MIL.M_CONDITION, condition.HasValue ? condition.Value : MIL.M_DISABLE);
            MIL.MimControl(StatContextId, MIL.M_COND_LOW, lowCond.HasValue ? lowCond.Value : MIL.M_DISABLE);
            MIL.MimControl(StatContextId, MIL.M_COND_HIGH, highCond.HasValue ? highCond.Value : MIL.M_DISABLE);

            MIL.MimStatCalculate(StatContextId, milImage, _milId, MIL.M_DEFAULT);

            //Reset for next stat
            foreach (var statType in statTypes)
            {
                MIL.MimControl(StatContextId, statType, MIL.M_DISABLE);
            }
        }

        public void Stat(MilImage milImage, params MIL_INT[] statTypes)
        {
            Stat(milImage, statTypes.ToList());
        }

        //=================================================================
        //
        //=================================================================
        public void Projection(MilImage srcImage, double projectionAxisAngle, Int64 operation)
        {
            Projection(srcImage, projectionAxisAngle, operation, MIL.M_NULL);
        }

        protected void Projection(MilImage srcImage, double projectionAxisAngle, Int64 operation, double operationValue)
        {
            MIL.MimProjection(srcImage, MilId, projectionAxisAngle, operation, operationValue);
        }

        //=================================================================
        //
        //=================================================================
        protected void Histogram(MilImage milImage)
        {
            MIL.MimHistogram(milImage, _milId);
        }

        public static long[] Histogram(MilImage milImage, long nbEntries, bool returnCumulativeValues = false)
        {
            using (MilImageResult milHisto = new MilImageResult())
            {
                milHisto.AllocResult(milImage.OwnerSystem, nbEntries, MIL.M_HIST_LIST);
                milHisto.Histogram(milImage);
                milHisto.GetResult(returnCumulativeValues ? MIL.M_CUMULATIVE_VALUE : MIL.M_VALUE, out var data);
                return data;
            }
        }
    }
}
