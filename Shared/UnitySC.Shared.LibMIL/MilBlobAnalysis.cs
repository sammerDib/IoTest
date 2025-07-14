using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    // Feature List
    ///////////////////////////////////////////////////////////////////////
    public class MilBlobFeatureList : AMilId
    {
        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MblobFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        //
        //=================================================================
        public void Alloc()
        {

            MIL.MblobAlloc(Mil.Instance.HostSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref _milId);
            checkMilError("Failed to allocate blob feature list");
        }

        //=================================================================
        // 
        //=================================================================
        public void SelectFeature(MIL_INT Feature)
        {
            // MIL 10
            //MIL.MblobSelectFeature(_milId, Feature);

            // MIL X
            switch (Feature)
            {
                case MIL.M_AREA:
                    // MblobSelectFeature(M_AREA) has no MblobControl() since this feature is always calculated
                    // so skip it
                    return;

                case MIL.M_BOX_X_MIN :
                case MIL.M_BOX_X_MAX :
                case MIL.M_BOX_Y_MAX:
                case MIL.M_BOX_Y_MIN:
                case MIL.M_BLOB_TOUCHING_IMAGE_BORDERS:
                case MIL.M_BOX_AREA:
                case MIL.M_BOX_ASPECT_RATIO:
                case MIL.M_BOX_FILL_RATIO:
                case MIL.M_FIRST_POINT_X:
                case MIL.M_FIRST_POINT_Y:
                case MIL.M_FERET_X:
                case MIL.M_FERET_Y:
                    Feature = MIL.M_BOX;
                    break;

                case MIL.M_CHAIN_X:
                case MIL.M_CHAIN_Y:
                case MIL.M_CHAIN_INDEX:
                case MIL.M_NUMBER_OF_CHAINED_PIXELS:
                case MIL.M_TOTAL_NUMBER_OF_CHAINED_PIXELS:
                    Feature = MIL.M_CHAINS;
                    break;

                //Note de RTI : yen a d'autre à voir si on les fait tous 
                //M_FERETS
                //M_WORLD_BOX
                //M_RUNS
                //M_MIN_PERIMETER_BOX
                //M_MIN_AREA_BOX
                //M_INTERCEPT 
                case MIL.M_AXIS_PRINCIPAL_ANGLE:
                case MIL.M_AXIS_SECONDARY_ANGLE:
                    Feature = MIL.M_MOMENT_SECOND_ORDER;
                    break;
            }


            MIL.MblobControl(_milId, Feature, MIL.M_ENABLE);
         
            checkMilError("Failed to select blob feature");
        }

    }

    ///////////////////////////////////////////////////////////////////////
    // Result
    ///////////////////////////////////////////////////////////////////////
    public class MilBlobResult : AMilId
    {
        //=================================================================
        // Properties
        //=================================================================
        public int Number
        {
            // MIL10 
            //get { return (int) MIL.MblobGetNumber(_milId, MIL.M_NULL); }

            // MILX
            get
            {
                MIL_INT TotalBlobsNumber = 0;
                MIL.MblobGetResult(_milId, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref TotalBlobsNumber);;
                return (int)TotalBlobsNumber;
            }
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MblobFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // 
        //=================================================================
        public void Alloc()
        {
            MIL.MblobAllocResult(Mil.Instance.HostSystem, MIL.M_DEFAULT, MIL.M_DEFAULT, ref _milId);
            checkMilError("Failed to allocate blob result");
        }

        //=================================================================
        //
        //=================================================================
        public void Calculate(MilImage blobIdentImage, MilImage greyImage, MilBlobFeatureList featureListId)
        {
            MIL_ID GreyImageId = MIL.M_NULL;
            if (greyImage != null)
                GreyImageId = greyImage.MilId;

            MIL.MblobCalculate(featureListId, blobIdentImage, GreyImageId, _milId);
            checkMilError("Failed to calculate blobs");
        }

        //=================================================================
        //
        //=================================================================
        public double[] GetResult(MIL_INT feature)
        {
            double[] array = new double[Number];
            MIL.MblobGetResult(_milId, MIL.M_DEFAULT, feature, array);
            checkMilError("Failed to get blob result");
            return array;
        }

        //=================================================================
        //
        //=================================================================
        public void Select(MIL_INT operation, MIL_INT selectionCriterion, MIL_INT condition, double condLow, double condHigh)
        {
            MIL.MblobSelect(_milId, operation, selectionCriterion, condition, condLow, condHigh);
            checkMilError("Failed to select blobs");
        }

        //=================================================================
        //
        //=================================================================
        public void Control(MIL_INT controlType, MIL_INT controlValue)
        {
            MIL.MblobControl(_milId, controlType, controlValue);
            checkMilError("Failed to control blobs");
        }

        //=================================================================
        // 
        //=================================================================
        public void Fill(MilImage destImageBuf, MIL_INT criterion, MIL_INT value)
        {
            // MIL10
            //MIL.MblobFill(_milId, DestImageBuf, Criterion, Value);
            //MILX
            var oldcolor = MIL.MgraInquire(MIL.M_DEFAULT, MIL.M_COLOR);
            MIL.MgraColor(MIL.M_DEFAULT, value);
            MIL.MblobDraw(MIL.M_DEFAULT, _milId, destImageBuf, MIL.M_DRAW_BLOBS , criterion, MIL.M_DEFAULT);
            MIL.MgraColor(MIL.M_DEFAULT, oldcolor);

            checkMilError("Failed to fill blobs");
        }

        //=================================================================
        //
        //=================================================================
        public void Draw(MilGraphicsContext contextGra, MilImage destImageBuf, long operation, MIL_INT label)
        {
            MIL.MblobDraw(contextGra, MilId, destImageBuf, operation, label, ControlFlag: MIL.M_DEFAULT);
            checkMilError("Failed to draw blobs");
        }

        public void Draw(MilImage destImageBuf, long operation, MIL_INT label)
        {
            MIL.MblobDraw(MIL.M_DEFAULT, MilId, destImageBuf, operation, label, ControlFlag: MIL.M_DEFAULT);
            checkMilError("Failed to draw blobs");
        }
    }
}
