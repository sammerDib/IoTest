using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.FDC.PersistentData
{

    [Serializable]
    public class PersitentFDCCountdown : IPersistentFDCData
    {
        // Default constructor requested for Serialization
        public PersitentFDCCountdown() { }

        public PersitentFDCCountdown(string name) : this(name, DateTime.Now, TimeSpan.Zero) { }

        public PersitentFDCCountdown(string name, TimeSpan initialCount) : this(name, DateTime.Now, initialCount) { }

        public PersitentFDCCountdown(string name, DateTime resetdt, TimeSpan initialCountTime)
        {
            FDCName = name;
            InitialCountTime = initialCountTime;
            ResetDate = resetdt;
        }

        public string FDCName { get; set; }

        public DateTime ResetDate { get; set; }

        public TimeSpan InitialCountTime { get; set; }

        public TimeSpan Countdown 
        { 
            get 
            {
                var countdown = InitialCountTime - DateTime.Now.Subtract(ResetDate);
                if (countdown < TimeSpan.Zero)
                    countdown = TimeSpan.Zero;
                return countdown; 
            } 
        }
        public double CountdownHours { get { return Countdown.TotalHours; } }
        public double CountdownDays { get { return Countdown.TotalDays; } }
    }
}
