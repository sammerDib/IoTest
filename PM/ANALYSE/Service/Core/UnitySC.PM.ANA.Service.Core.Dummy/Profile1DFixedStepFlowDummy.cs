using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Interface.Profile1D;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class Profile1DFixedStepFlowDummy : Profile1DFixedStepFlow
    {
        private Random _rnd = new Random();
        public Profile1DFixedStepFlowDummy(Profile1DFixedStepInput input) : base(input)
        { }

        private double MakeNoise(double lvl)
        { 
            return (_rnd.NextDouble() - 0.5) * lvl;
        }
        protected override void Process()
        {
            double noiselvel = 3.0;
            double noiseX = 0.05;
            Result.Profile = new Profile2d
            {
                new Point2d(0.0, 50.0 + MakeNoise(noiselvel)),
                new Point2d(0.1, 50.0 + MakeNoise(noiselvel)),
                new Point2d(0.2, 50.0 + MakeNoise(noiselvel)),
                new Point2d(0.3, 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.4, 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.5, 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.5, 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.6, 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.7 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.8 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(0.9 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.0 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.1 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.2 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.3 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.4 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.5 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.5 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.6 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.7 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                new Point2d(1.8 + MakeNoise(noiseX), 10.0 + MakeNoise(noiselvel)),
                                 
            };
        }
    }
}
