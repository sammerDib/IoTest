using System;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing a model of the MIL pattern module
    //
    ///////////////////////////////////////////////////////////////////////

#pragma warning disable CS0612 // Obsolete member
#pragma warning disable CS0618 // Obsolete member

    public class MilPatternMatching : AMilModel
    {
        //=================================================================
        // Properties
        //=================================================================

        //-----------------------------------------------------------------
        // Model's reference position relative to the top-left corner of the model's source image
        //-----------------------------------------------------------------
        public override int OriginalX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ORIGINAL_X); }
        }

        public override int OriginalY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ORIGINAL_Y); }
        }

        //-----------------------------------------------------------------
        // Position in Model's source image
        //-----------------------------------------------------------------
        public override int AllocOffsetX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ALLOC_OFFSET_X); }
            set
            {
                MIL.MpatSetSearchParameter(MilId, MIL.M_ALLOC_OFFSET_X, value);
                checkMilError("Failed to set AllocOffsetX on model");
            }
        }

        public override int AllocOffsetY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ALLOC_OFFSET_Y); }
            set
            {
                MIL.MpatSetSearchParameter(MilId, MIL.M_ALLOC_OFFSET_Y, value);
                checkMilError("Failed to set AllocOffsetY on model");
            }
        }

        public override int AllocSizeX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ALLOC_SIZE_X); }
        }

        public override int AllocSizeY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_ALLOC_SIZE_Y); }
        }

        public override int ReferenceX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_CENTER_X + MIL.M_DEFAULT); }
            set { MIL.MpatSetCenter(_milId, value, ReferenceY); }
        }

        public override int ReferenceY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_CENTER_Y + MIL.M_DEFAULT); }
            set { MIL.MpatSetCenter(_milId, ReferenceX, value); }
        }

        public override Rectangle AllocRectangle
        {
            get { return new Rectangle(AllocOffsetX, AllocOffsetY, AllocSizeX, AllocSizeY); }
        }

        //-----------------------------------------------------------------
        // Model's search region
        //-----------------------------------------------------------------
        public int PositionStartX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_POSITION_START_X); }
        }

        public int PositionStartY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_POSITION_START_Y); }
        }

        public int PositionUncertaintyX
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_POSITION_UNCERTAINTY_X); }
        }

        public int PositionUncertaintyY
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_POSITION_UNCERTAINTY_Y); }
        }

        public override Rectangle Position
        {
            get
            {
                return new Rectangle(PositionStartX, PositionStartY, PositionUncertaintyX, PositionUncertaintyY);
            }
            set
            {
                SetPosition(value.X, value.Y, value.Width, value.Height);
            }
        }

        //-----------------------------------------------------------------
        // Position in Model's source image
        //-----------------------------------------------------------------
        public double MinSpacingX
        {
            get { return MIL.MpatInquire(MilId, MIL.M_MIN_SPACING_X); }
            set
            {
                MIL.MpatSetSearchParameter(MilId, MIL.M_MIN_SPACING_X, value);
                checkMilError("Failed to set MinSpacingX on model");
            }
        }

        public double MinSpacingY
        {
            get { return MIL.MpatInquire(MilId, MIL.M_MIN_SPACING_Y); }
            set
            {
                MIL.MpatSetSearchParameter(MilId, MIL.M_MIN_SPACING_Y, value);
                checkMilError("Failed to set MinSpacingY on model");
            }
        }

        //-----------------------------------------------------------------
        // Advanced
        //-----------------------------------------------------------------
        public override double Acceptance
        {
            get
            {
                double threshold = 0;
                MIL.MpatInquire(MilId, MIL.M_ACCEPTANCE_THRESHOLD, ref threshold);
                return threshold;
            }
            set
            {
                MIL.MpatSetAcceptance(MilId, value);
                checkPropertyMilError("Acceptance", "Advanced");
            }
        }

        public override double Certainty
        {
            get
            {
                double threshold = 0;
                MIL.MpatInquire(MilId, MIL.M_CERTAINTY_THRESHOLD, ref threshold);
                return threshold;
            }
            set
            {
                MIL.MpatSetCertainty(MilId, value);
                checkPropertyMilError("Certainty", "Advanced");
            }
        }

        public override int Number
        {
            get { return (int)MIL.MpatInquire(MilId, MIL.M_NUMBER_OF_OCCURRENCES); }
            set
            {
                MIL.MpatSetNumber(MilId, value);
                checkMilError("Failed to set number of occurences on model");
            }
        }

        //-----------------------------------------------------------------
        // Other
        //-----------------------------------------------------------------
        public override bool IsPreprocessed
        {
            get { return MIL.MpatInquire(MilId, MIL.M_PREPROCESSED) == 1; }
        }

        public override MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MpatInquire(MilId, MIL.M_OWNER_SYSTEM); }
        }

        //=================================================================
        // Constructor
        //=================================================================
        public MilPatternMatching()
        {
            Usage = EUsage.Stiching_Centering;
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
        // Load a model from disk
        //=================================================================
        public override void Restore(MIL_ID systemId, string filename)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing pattern");

            Name = System.IO.Path.GetFileNameWithoutExtension(filename);
            MIL.MpatRestore(systemId, filename, ref _milId);
            checkMilError("Failed to load pattern model \"" + filename + "\"");
        }

        //=================================================================
        // Save a model to disk
        //=================================================================
        public override void Save(string filename)
        {
            MIL.MpatSave(filename, MilId);
            checkMilError("Failed to save pattern model \"" + filename + "\"");
        }

        //=================================================================
        // Preprocess the model
        //=================================================================
        public override void Preprocess(MilImage milTypicalImage)
        {
            MIL_ID milTypicalImageId;
            if (milTypicalImage == null)
                milTypicalImageId = MIL.M_NULL;
            else
                milTypicalImageId = milTypicalImage.MilId;

            MIL.MpatPreprocModel(milTypicalImageId, MilId, MIL.M_DEFAULT);
            checkMilError("Failed to preprocess pattern model");
        }

        //=================================================================
        // Set search rectangle
        //=================================================================
        public void SetPosition(int offX, int offY, int sizeX, int sizeY)
        {
            MIL.MpatSetPosition(MilId, offX, offY, sizeX, sizeY);
            checkMilError("Failed to set search position of pattern model");
        }

        //=================================================================
        // Draw the model position in an image
        //=================================================================
        public override void Draw(MilImage milDestImage,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT)
        {
            MIL.MpatDraw(MIL.M_DEFAULT, MilId, milDestImage.MilId, operation, index, controlFlag);
            checkMilError("Failed to draw pattern model");
        }

        public override void Draw(MilGraphicsContext milGC,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT)
        {
            MIL.MpatDraw(milGC.MilId, MilId, milGC.Image.MilId, operation, index, controlFlag);
            checkMilError("Failed to draw pattern model");
        }

        //=================================================================
        //  Alloc Model
        //=================================================================
        public void AllocModel(MIL_ID systemId, MilImage milSrcImage, int offX, int offY, int sizeX, int sizeY, int modelType)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing pattern");

            MIL.MpatAllocModel(systemId, milSrcImage.MilId, offX, offY, sizeX, sizeY, modelType, ref _milId);
            checkMilError("Failed to allocate pattern model");
        }

        public void AllocModel(MIL_ID systemId, MilImage milSrcImage, Rectangle rect, int modelType)
        {
            AllocModel(systemId, milSrcImage, rect.X, rect.Y, rect.Width, rect.Height, modelType);
        }

        //=================================================================
        // Copy model's data
        //=================================================================
        public void Copy(MilImage destImage, MIL_INT copyMode)
        {
            MIL.MpatCopy(MilId, destImage.MilId, copyMode);
            checkMilError("Failed to copy pattern model's data");
        }

        //=================================================================
        // Create a result object
        //=================================================================
        public override AMilResult NewResult()
        {
            return new MilPatternMatchingResult(this);
        }
    }
}
