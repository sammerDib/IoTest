using System;
using System.Text;

using UnitySC.Equipment.Devices.Controller.JobDefinition;

namespace UnitySC.Equipment.Devices.Controller.EventArgs
{
    public class JobStatusChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// id of the job
        /// </summary>
        public Job Job { get; }


        public JobStatusChangedEventArgs(Job job)
        {
            Job = job;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder build = new StringBuilder();
            build.AppendLine(FormattableString.Invariant($"JobName = {Job}"));
            return build.ToString();
        }
    }
}
