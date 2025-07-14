using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing the result of the MIL PatternMatching/ModelFinder
    //
    ///////////////////////////////////////////////////////////////////////
    public abstract class AMilResult : AMilId
    {
        //=================================================================
        // Properties
        //=================================================================
        public abstract int Number { get; protected set; }

        public abstract double PositionX { get; protected set; }
        public abstract double PositionY { get; protected set; }
        public abstract double Angle { get; protected set; }
        public abstract double Scale { get; protected set; }
        public abstract double Score { get; protected set; }
        public abstract double[] PositionsX { get; protected set; }
        public abstract double[] PositionsY { get; protected set; }
        public abstract double[] Angles { get; protected set; }
        public abstract double[] Scales { get; protected set; }
        public abstract double[] Scores { get; protected set; }
        public abstract AMilModel parentModel { get; protected set; }

        //=================================================================
        // Methods
        //=================================================================
        public abstract void FindModel(MilImage milImage);

        public abstract void Draw(MilImage milDestImage,
                         int Operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION,
                         int Index = MIL.M_DEFAULT,
                         int ControlFlag = MIL.M_DEFAULT
                        );

        public abstract void Draw(MilGraphicsContext milGC,
                         int Operation = MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION,
                         int Index = MIL.M_DEFAULT,
                         int ControlFlag = MIL.M_DEFAULT
                        );

        public abstract void AllocResult();
    }
}