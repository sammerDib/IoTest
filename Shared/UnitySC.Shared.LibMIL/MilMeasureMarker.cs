using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilMeasureMarker : AMilId
    {
        public MIL_ID Context = MIL.M_DEFAULT;

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MmeasFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Functions
        //=================================================================
        public void Alloc(MIL_ID systemId, long markerType, long controlFlag = MIL.M_DEFAULT)
        {
            MIL.MmeasAllocMarker(systemId, markerType, controlFlag, ref _milId);
            checkMilError("MmeasAllocMarker");
        }

        public void SetMarker(long characteristicToSet, double firstValue, double secondValue)
        {
            MIL.MmeasSetMarker(MilId, characteristicToSet, firstValue, secondValue);
            checkMilError("MmeasSetMarker");
        }

        public void SetMarker(long characteristicToSet, double firstValue)
        {
            SetMarker(characteristicToSet, firstValue, MIL.M_NULL);
        }

        public void Draw(MilGraphicsContext graphicsContext, long operation, long index = MIL.M_DEFAULT, long controlFlag = MIL.M_DEFAULT)
        {
            MIL.MmeasDraw(graphicsContext, MilId, graphicsContext.Image, operation, index, controlFlag);
            checkMilError("MmeasDraw");
        }

        public void Find(MIL_ID imageBufId, long measurementList)
        {
            MIL.MmeasFindMarker(Context, imageBufId, MilId, measurementList);
            checkMilError("MmeasFindMarker");
        }

        public MIL_INT Inquire(long inquireType)
        {
            MIL_INT value = MIL.MmeasInquire(MilId, inquireType, MIL.M_NULL, MIL.M_NULL);
            checkMilError("MmeasInquire");
            return value;
        }

        public double GetDoubleResult(long resultType)
        {
            double value = 0;
            MIL.MmeasGetResult(MilId, resultType, ref value, MIL.M_NULL);
            checkMilError("MmeasGetResult");
            return value;
        }

        public PointF GetPointFResult(long resultType)
        {
            double x = double.NaN, y = double.NaN;
            MIL.MmeasGetResult(MilId, resultType, ref x, ref y);
            checkMilError("MmeasGetResult");
            return new PointF((float)x, (float)y);
        }

        //=================================================================
        // Résultats (MmeasGetResult)
        //=================================================================
        public bool ValidFlag
        {
            get
            {
                MIL_INT ValidFlag = MIL.M_FALSE;
                MIL.MmeasGetResult(MilId, MIL.M_VALID_FLAG + MIL.M_TYPE_MIL_INT, ref ValidFlag, MIL.M_NULL);
                return (ValidFlag == MIL.M_TRUE);
            }
        }

        //public double Angle { get { return GetDoubleResult(MIL.M_BOX_ANGLE_FOUND); } }
        public PointF TopLeftCorner { get { return GetPointFResult(MIL.M_BOX_CORNER_TOP_LEFT); } }

        public PointF TopRightCorner { get { return GetPointFResult(MIL.M_BOX_CORNER_TOP_RIGHT); } }
        public PointF BottomLeftCorner { get { return GetPointFResult(MIL.M_BOX_CORNER_BOTTOM_LEFT); } }
        public PointF BottomRightCorner { get { return GetPointFResult(MIL.M_BOX_CORNER_BOTTOM_RIGHT); } }

        //public QuadF QuadF
        //{
        //    get
        //    {
        //        QuadF q = new QuadF();
        //        q.corners[0] = TopLeftCorner;
        //        q.corners[1] = BottomLeftCorner;
        //        q.corners[2] = BottomRightCorner;
        //        q.corners[3] = TopRightCorner;
        //        return q;
        //    }
        //}

        //  public double StripWidth { get { return GetDoubleResult(MIL.M_STRIPE_WIDTH); } }

        //=================================================================
        // Propriétées (MmeasInquire)
        //=================================================================
        public int MarkerType { get { return (int)Inquire(MIL.M_MARKER_TYPE); } }
    }
}