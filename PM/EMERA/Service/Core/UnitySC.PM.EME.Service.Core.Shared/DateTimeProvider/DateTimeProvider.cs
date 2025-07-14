using System;

using UnitySC.PM.EME.Service.Core.Shared.DateTimeProvider;

namespace UnitySC.PM.EME.Service.Core.Shared.DateTimeHelper
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetDateTimeNow()
            => DateTime.Now;
    }
}
