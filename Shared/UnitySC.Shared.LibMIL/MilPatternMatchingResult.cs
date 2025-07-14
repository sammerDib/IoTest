using System;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing a result of the MIL pattern module
    //
    ///////////////////////////////////////////////////////////////////////
#pragma warning disable CS0618 // An obsolete member replaces a non-obsolete member
#pragma warning disable CS0612 // An obsolete member replaces a non-obsolete member

    public class MilPatternMatchingResult : AMilResult
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

        //=================================================================
        // Constructor
        //=================================================================
        public MilPatternMatchingResult(MilPatternMatching parentModel)
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
                MIL.MpatFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Allocate result buffer
        //=================================================================
        public override void AllocResult()
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing pattern result");

            MIL.MpatAllocResult(parentModel.OwnerSystem, parentModel.Number, ref _milId);
            AMilId.checkMilError("Failed to allocate pattern result");
        }

        //=================================================================
        // Find the associated model in the target buffer
        //=================================================================
        public override void FindModel(MilImage milImage)
        {
            MIL.MpatSetSearchParameter(MilId, MIL.M_RESULT_OUTPUT_UNITS, MIL.M_PIXEL);

            // Find model
            MIL.MpatFindModel(milImage.MilId, parentModel.MilId, MilId);
            AMilId.checkMilError("Failed to find pattern model");

            // Read results
            Number = (int)MIL.MpatGetNumber(MilId);
            if (Number > 0)
            {
                PositionsX = new double[Number];
                PositionsY = new double[Number];
                Angles = new double[Number];
                Scales = new double[Number];
                Scores = new double[Number];

                //MIL.MpatSetAngle(parentModel.MilId, MIL.M_SEARCH_ANGLE_MODE, MIL.M_DISABLE);
                //MIL.MpatSetAngle(parentModel.MilId, MIL.M_SEARCH_ANGLE, 0.0);

                MIL.MpatGetResult(MilId, MIL.M_POSITION_X, PositionsX);
                MIL.MpatGetResult(MilId, MIL.M_POSITION_Y, PositionsY);
                //MIL.MpatGetResult(MilId, MIL.M_ANGLE, Angles);
                MIL.MpatGetResult(MilId, MIL.M_SCORE, Scores);

                PositionX = PositionsX[0];
                PositionY = PositionsY[0];
                Angle = Angles[0];
                Scale = Scales[0];
                Score = Scores[0];
            }
        }

        //=================================================================
        // Draw the result(s) position in an image
        //=================================================================
        public override void Draw(MilImage milDestImage,
                         int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION,
                         int index = MIL.M_DEFAULT,
                         int controlFlag = MIL.M_DEFAULT
                        )
        {
            MIL.MpatDraw(MIL.M_DEFAULT, MilId, milDestImage.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw pattern result");
        }

        public override void Draw(MilGraphicsContext milGC,
                         int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION,
                         int index = MIL.M_DEFAULT,
                         int controlFlag = MIL.M_DEFAULT
                        )
        {
            MIL.MpatDraw(milGC.MilId, MilId, milGC.Image.MilId, operation, index, controlFlag);
            AMilId.checkMilError("Failed to draw pattern result");
        }
    }
}
