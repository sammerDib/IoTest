using System;

using UnitySC.Equipment.Abstractions.Vendor.JobDefinition;
using UnitySC.Equipment.Devices.Controller.EventArgs;
using UnitySC.Equipment.Devices.Controller.JobDefinition;

namespace UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.WaferMachineActivity
{
    public abstract class WaferMachineActivity : MachineActivity
    {
        #region Events

        public event EventHandler<SubstrateIdReadingHasBeenFinishedEventArgs>
            SubstrateIdReadingHasBeenFinished;

        public event EventHandler WaferAlignStart;

        public event EventHandler WaferAlignEnd;

        public event EventHandler LastWaferEntry;

        public event EventHandler<JobStatusChangedEventArgs> JobStatusChanged;
        #endregion

        #region Constructor

        protected WaferMachineActivity()
        {

        }

        protected WaferMachineActivity(string id, Controller controller)
            : base(id, controller)
        {

        }

        #endregion

        #region protected

        protected void OnSubstrateIdReadingHasBeenFinished(bool isSuccess, string substrateId, string acquiredId)
        {
            if (SubstrateIdReadingHasBeenFinished != null)
            {
                SubstrateIdReadingHasBeenFinishedEventArgs args =
                    new SubstrateIdReadingHasBeenFinishedEventArgs(
                        isSuccess,
                        substrateId,
                        acquiredId);

                SubstrateIdReadingHasBeenFinished.Invoke(this, args);
            }
        }

        protected void OnWaferAlignStart()
        {
            try
            {
                WaferAlignStart?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        protected void OnWaferAlignEnd()
        {
            try
            {
                WaferAlignEnd?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        protected void OnLastWaferEntry()
        {
            try
            {
                LastWaferEntry?.Invoke(this, System.EventArgs.Empty);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        protected void OnJobStatusChanged(Job job, JobStatus newStatus)
        {
            try
            {
                Logger.Info($"Job {job.ControlJobId} => JobStatus changed to {newStatus}");
                job.Status = newStatus;
                JobStatusChanged?.Invoke(this, new JobStatusChangedEventArgs(job));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}
