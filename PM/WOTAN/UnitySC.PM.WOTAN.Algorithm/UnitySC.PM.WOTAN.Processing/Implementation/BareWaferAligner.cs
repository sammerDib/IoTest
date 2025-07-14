using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CV = Emgu.CV;

using UnitySC.PM.WOTAN.Common;

namespace UnitySC.PM.WOTAN.Processing
{
    public class BareWaferAligner : IBareWaferAligner
    {
        public AlignResult Align(Wafer wafer)
        {
            IList<LaneImage> laneImages = wafer.AlignImages;
            IList<AlignResult> alignResults = new List<AlignResult>();

            foreach (LaneImage laneImage in laneImages)
            {
                BareWaferLaneAligner bareWaferLaneAligner = new BareWaferLaneAligner(
                    wafer: wafer, 
                    laneNumber: laneImage.LaneNumber);
                alignResults.Add(bareWaferLaneAligner.Align());
            }

            AlignResult bestResult = null;
            double bestScore = double.MaxValue;
            foreach (AlignResult alignResult in alignResults)
            {
                if (alignResult.MarkTypeScore < bestScore)
                {
                    bestScore = alignResult.MarkTypeScore;
                    bestResult = alignResult;
                }
            }

            return bestResult;
        }
    }
}
