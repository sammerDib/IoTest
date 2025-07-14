using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance
{
    public class AxesSet
    {
        private readonly List<IAxis> _axes;

        public bool Empty => _axes.Count == 0;

        public AxesSet(List<IAxis> axes)
        {
            _axes = axes;
        }

        public bool ContainsAnyOf(params MovingDirection[] movingDirections) => movingDirections.Any(Contains);

        public bool ContainsAll(params MovingDirection[] movingDirections) => movingDirections.All(Contains);

        public bool Contains(MovingDirection movingDirection) => _axes.Any(
            axis => axis.AxisConfiguration.MovingDirection == movingDirection
        );

        public int IndexOf(MovingDirection movingDirection) => _axes.FindIndex(
            axis => axis.AxisConfiguration.MovingDirection == movingDirection
        );
    }
}
