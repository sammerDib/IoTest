using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Referentials
{
    public abstract class ReferentialConverterBase : IReferentialConverter
    {
        public abstract ReferentialTag ReferentialTagLeft { get; }
        public abstract ReferentialTag ReferentialTagRight { get; }

        protected abstract PositionBase ConvertFromLeftToRight();
        protected abstract PositionBase ConvertFromRightoLeft();
        protected List<ReferentialSettingsBase>  Settings { get; private set; }
        protected PositionBase PositionBaseFrom { get; private set; }
        protected ReferentialBase ReferentialTo { get; private set; }

        public PositionBase Convert(PositionBase positionBase, ReferentialBase referentialTo, List<ReferentialSettingsBase> settings)
        {
            Settings = settings;
            if (positionBase.Referential.Tag == ReferentialTagLeft && referentialTo.Tag == ReferentialTagRight)
            {
                return ConvertFromLeftToRight();
            }
            else if (positionBase.Referential.Tag == ReferentialTagRight && referentialTo.Tag == ReferentialTagLeft)
            {
                return ConvertFromRightoLeft();
            }
            else
            {
                throw new InvalidOperationException($"Bad referential in converter Left {0} Right {1}");
            }
        }

        public bool IsEnabled { get; set; }
    }
}
