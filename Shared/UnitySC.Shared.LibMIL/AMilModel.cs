using System;
using System.ComponentModel;
using System.Drawing;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing the model of the MIL PatternMatching/ModelFinder
    //
    ///////////////////////////////////////////////////////////////////////
    public abstract class AMilModel : AMilId
    {
        //=================================================================
        // Enum
        //=================================================================
        public enum EUsage : int
        {
            [Description("Stitching / Vertical Centering")]
            Stitching = 0,

            [Description("Horizontal Centering")]
            Centering = 1,

            [Description("Stitching / Centering")]
            Stiching_Centering = 2,

            [Description("Stiching / Vertical Centering, search on the whole line")]
            Stitching_Line = 3,

            [Description("Horizontal Centering, search on the whole column")]
            Centering_Column = 4,

            [Description("Whole die")]
            WholeDie = 5,

            [Description("Corner")]
            Corner = 6
        };

        //=================================================================
        // Properties
        //=================================================================
        public string Name { get; set; } = "";

        public EUsage Usage { get; set; } = EUsage.Centering;

        //-----------------------------------------------------------------
        // Reference
        //-----------------------------------------------------------------
        public abstract int OriginalX { get; }

        public abstract int OriginalY { get; }

        //-----------------------------------------------------------------
        // Position
        //-----------------------------------------------------------------
        public abstract Rectangle Position { get; set; }

        //-----------------------------------------------------------------
        // Reference point
        //-----------------------------------------------------------------
        public abstract int ReferenceX { get; set; }

        public abstract int ReferenceY { get; set; }

        //-----------------------------------------------------------------
        // Advanced
        //-----------------------------------------------------------------
        public abstract double Acceptance { get; set; }

        public abstract double Certainty { get; set; }
        public abstract int Number { get; set; }

        //-----------------------------------------------------------------
        // Model size
        //-----------------------------------------------------------------
        public abstract int AllocOffsetX { get; set; }

        public abstract int AllocOffsetY { get; set; }
        public abstract int AllocSizeX { get; }
        public abstract int AllocSizeY { get; }
        public abstract Rectangle AllocRectangle { get; }

        //-----------------------------------------------------------------
        // Other
        //-----------------------------------------------------------------
        public abstract bool IsPreprocessed { get; }

        public abstract MIL_ID OwnerSystem { get; }

        //=================================================================
        // Abstract methods
        //=================================================================
        public abstract void Save(string filename);

        public abstract void Restore(MIL_ID systemId, string filename);

        public abstract void Preprocess(MilImage milTypicalImage);

        public abstract void Draw(MilImage milDestImage,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT);

        public abstract void Draw(MilGraphicsContext milGC,
                                  int operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION + MIL.M_DRAW_EDGES,
                                  int index = MIL.M_DEFAULT,
                                  int controlFlag = MIL.M_DEFAULT);

        public abstract AMilResult NewResult();

        //=================================================================
        // Functions
        //=================================================================

        //-----------------------------------------------------------------
        // Set search margin.
        // The function calls SetPosition with parameters such that the
        // model is searched at a fixed position +- the margin.
        //-----------------------------------------------------------------
        public void SetMargin(int marginX, int marginY)
        {
            int x = AllocOffsetX - marginX;
            int y = AllocOffsetY - marginY;
            int w = 2 * marginX;
            int h = 2 * marginY;
            Position = new Rectangle(x, y, w, h);
        }

        //-----------------------------------------------------------------
        // Compute Number of Occurencies
        //-----------------------------------------------------------------
        public void ComputeNbOccurencies(bool searchAll)
        {
            if (searchAll)
            {
                Number = 1000;
            }
            else
            {
                switch (Usage)
                {
                    case EUsage.Stitching:
                    case EUsage.Centering:
                    case EUsage.Stiching_Centering:
                    case EUsage.WholeDie:
                    case EUsage.Corner:
                        Number = 1;
                        break;

                    case EUsage.Stitching_Line:
                    case EUsage.Centering_Column:
                        Number = 5;
                        break;

                    default:
                        throw new ApplicationException("Internal Error: model usage not handled !");
                }
            }
        }

        //-----------------------------------------------------------------
        // Tell if model can be used for stitching or centering
        //-----------------------------------------------------------------
        public bool UseForCentering()
        {
            switch (Usage)
            {
                case EUsage.Centering:
                case EUsage.Stiching_Centering:
                case EUsage.Centering_Column:
                    return true;

                default:
                    return false;
            }
        }

        public bool UseForStitching()
        {
            switch (Usage)
            {
                case EUsage.Stitching:
                case EUsage.Stiching_Centering:
                case EUsage.Stitching_Line:
                    return true;

                default:
                    return false;
            }
        }

        //-----------------------------------------------------------------
        // ToString
        //-----------------------------------------------------------------
        public override string ToString()
        {
            return Name;
        }
    }
}
