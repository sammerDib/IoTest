using System;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Jobs
{
    internal class JobDetailsChangedEventArgs : EventArgs
    {
        public JobDetailsViewModel Job { get; }

        public bool CloseExpander { get; }

        public JobDetailsChangedEventArgs(JobDetailsViewModel job, bool closeExpander)
        {
            Job = job;
            CloseExpander = closeExpander;
        }
    }
}
