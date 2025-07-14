using System;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing a result of the MIL model finder
    //
    ///////////////////////////////////////////////////////////////////////
    public class MilModelFinderResult : AMilResult
    {
        //=================================================================
        // Properties
        //=================================================================
        public override int Number { get; protected set; }

        public override double PositionX { get; protected set; }
        public override double PositionY { get; protected set; }
        public override double Angle { get; protected set; }
        public override double Scale { get; protected set; }
        public override double Score { get; protected set; }
        public override double[] PositionsX { get; protected set; }
        public override double[] PositionsY { get; protected set; }
        public override double[] Angles { get; protected set; }
        public override double[] Scales { get; protected set; }
        public override double[] Scores { get; protected set; }

        public override AMilModel parentModel { get; protected set; }

        public MilModelFinder ParentModelFinder { get { return (MilModelFinder)parentModel; } }

        //=================================================================
        // Constructor
        //=================================================================
        public MilModelFinderResult(MilModelFinder parentModel)
        {
            this.parentModel = parentModel;
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MmodFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Allocate result buffer
        //=================================================================
        public override void AllocResult()
        {
            switch (ParentModelFinder.ContextType)
            {
                case MIL.M_GEOMETRIC_CONTROLLED:
                    AllocResult(MIL.M_DEFAULT);
                    break;
#if MIL_10
				case MIL.M_SHAPE_CIRCLE:
                    AllocResult(MIL.M_SHAPE_CIRCLE);
                    break;
#endif
                default:
                    throw new ApplicationException("unknown model type: " + ParentModelFinder.ContextType);
            }
        }

        public void AllocResult(long controlFlag)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing model result");

            MIL.MmodAllocResult(parentModel.OwnerSystem, controlFlag, ref _milId);
            AMilId.checkMilError("Failed to allocate result");
        }

        //=================================================================
        // Find the associated model in the target buffer
        //=================================================================
        public override void FindModel(MilImage milImage)
        {
            // ModelFinder can't find anything if the image is smaller than 16x16 pixels
            if (milImage.SizeX < 16 || milImage.SizeY < 16)
                return;

            // Find model
            MIL.MmodFind(parentModel.MilId, milImage.MilId, _milId);
            checkMilError("Failed to find model");

            // Read results
            MIL_INT number = 0;
            MIL.MmodGetResult(_milId, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref number);
            Number = (int)number;
            if (Number > 0)
            {
                MIL.MmodControl(MilId, MIL.M_GENERAL, MIL.M_RESULT_OUTPUT_UNITS, MIL.M_PIXEL);
                checkMilError("Failed to set output units");

                PositionsX = new double[Number];
                PositionsY = new double[Number];
                Angles = new double[Number];
                Scales = new double[Number];
                Scores = new double[Number];

                MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_POSITION_X, PositionsX);
                MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_POSITION_Y, PositionsY);
                MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_ANGLE, Angles);
                MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_SCALE, Scales);
                MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_SCORE, Scores);

                PositionX = PositionsX[0];
                PositionY = PositionsY[0];
                Angle = Angles[0];
                Scale = Scales[0];
                Score = Scores[0];
            }
        }

        //=================================================================
        //
        //=================================================================
        private double[] _indexes;

        public double[] Indexes
        {
            get
            {
                if (_indexes == null)
                {
                    _indexes = new double[Number];
                    MIL.MmodGetResult(_milId, MIL.M_ALL, MIL.M_INDEX, _indexes);
                }
                return _indexes;
            }
        }

        //=================================================================
        // Draw the result(s) position in an image
        //=================================================================
        public override void Draw(MilImage milDestImage,
                         int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                         int index = MIL.M_DEFAULT,
                         int controlFlag = MIL.M_DEFAULT
                        )
        {
            MIL.MmodDraw(MIL.M_DEFAULT, _milId, milDestImage.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw result");
        }

        public override void Draw(MilGraphicsContext milGC,
                         int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                         int index = MIL.M_DEFAULT,
                         int controlFlag = MIL.M_DEFAULT
                        )
        {
            MIL.MmodDraw(milGC.MilId, _milId, milGC.Image.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw result");
        }
    }
}