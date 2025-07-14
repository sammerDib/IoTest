using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Referentials.Converters
{
    public class ObjectiveMotorConverter : IReferentialConverter
    {
        public ReferentialTag ReferentialTag1 => ReferentialTag.Objective;

        public ReferentialTag ReferentialTag2 => ReferentialTag.Motor;

        public bool IsEnabled { get; set; } = true;

        public PositionBase Convert(PositionBase positionBase, ReferentialBase referentialTo)
        {
            // Todo
            var position = positionBase.Clone() as PositionBase;
            position.Referential = referentialTo;
            return position;
        }
    }
}
