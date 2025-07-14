using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class Profile1DFlowDummy : Profile1DFlow
    {
        public Profile1DFlowDummy(Profile1DInput input) : base(input)
        { }

        protected override void Process()
        {
            Result.Profile = new Profile2d();
            int nbPoints = (int) (20 / Input.Speed.MillimetersPerSecond);
            double lengthX = Input.EndPosition.X - Input.StartPosition.X;
            double lengthY = Input.EndPosition.Y - Input.StartPosition.Y;
            double length = Math.Sqrt((lengthX * lengthX) + (lengthY * lengthY));
            double increment = length / (nbPoints - 1);
            for (double xy = 0; xy <= length; xy+= increment)
            {
                Result.Profile.Add(new Point2d(
                    xy,
                    xy / increment < (double) nbPoints / 2.0 ? 0.0 : 50.0
                ));
            }
            Result.Status = new FlowStatus(FlowState.Success);
        }
    }
}
