using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    public class MachineActivity : Activity
    {
        #region Properties

        /// <summary>
        /// Gets the Controller
        /// </summary>
        protected Controller Controller { get; }

        /// <summary>
        /// Gets the Efem
        /// </summary>
        protected Abstractions.Devices.Efem.Efem Efem { get; }

        /// <summary>
        /// Gets the Number of Wafers on Tool
        /// </summary>
        /// <returns>Number of substrates on tool</returns>
        protected int NbSubstratesOnTool
        {
            get
            {
                return SubstratesOnTool.Count();
            }
        }

        protected IEnumerable<Substrate> SubstratesOnTool
        {
            get
            {
                return Controller.GetSubstrates();
            }
        }

        public bool Paused { get; protected set; }

        public bool Pausing { get; protected set; }

        #endregion Properties
        
        protected readonly EventWaitHandle _pauseWaitHandle = new AutoResetEvent(true);

        /// <inheritdoc />
        public override ErrorDescription Check(ActivityManager context)
        {
            if (context == null)
            {
                return new ErrorDescription(
                    ErrorCode.ParameterImproperlySpecified,
                    "The activity manager is null. Activity cannot start");
            }

            return new ErrorDescription();
        }

        /// <inheritdoc />
        protected override ErrorDescription StartFinalizer()
        {
            return new ErrorDescription();
        }

        protected void PerformAction(Action action, bool pausable = true)
        {
            if (pausable)
            {
                //Check if activity pause was requested
                WaitForResume();
            }
            
            //Try to perform action
            TryCatch(action);
        }

        protected void PerformAction(Action action, Event nextEventToSend, bool pausable = true)
        {
            if (pausable)
            {
                //Check if activity pause was requested
                WaitForResume();
            }

            //Try to perform action
            TryCatch(action, nextEventToSend);
        }
        
        protected virtual void WaitForResume()
        {
            //If pause was requested
            if (Pausing)
            {
                //Switch to paused state
                Pausing = false;
                Paused = true;

                //Wait for the activity to be resumed or stopped or aborted
                _pauseWaitHandle.WaitOne();

                //Switch off the paused state
                Paused = false;
            }
        }

        public virtual ErrorDescription Pause()
        {
            if (!IsStarted)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity not started.");
            }

            //If paused already
            if (Paused || Pausing)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity already pausing or paused.");
            }

            Pausing = true;
            //Reset the handle to start the pause once the current action is done
            _pauseWaitHandle.Reset();

            return new ErrorDescription();
        }

        public virtual ErrorDescription Resume()
        {
            if (!IsStarted)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity not started.");
            }
            
            if (!Paused)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity not paused.");
            }

            //Set the handle to enable WaitForResume to finish
            _pauseWaitHandle.Set();

            return new ErrorDescription();
        }

        public override ErrorDescription Abort()
        {
            //Exit from pause state
            Paused = false;
            Pausing = false;
            _pauseWaitHandle.Set();

            return base.Abort();
        }

        /// <inheritdoc />
        protected override void ActivityExit(Event ev)
        {
            //If activity already ended before it was possible to pause it, set the pause handler
            if (Pausing || Paused)
            {
                Pausing = false;
                _pauseWaitHandle.Set();
                Paused = false;
            }

            base.ActivityExit(ev);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineActivity"/> class.
        /// Default constructor is required by auto-generated codes from State Machines, even if the default State Machine constructor is never used in the project.
        /// </summary>
        protected MachineActivity()
        {
            // For State Machine compliance. Not used
        }

        public MachineActivity(string id, Controller controller) : base(id)
        {
            Controller = controller;
            Efem = Controller.TryGetDevice<Abstractions.Devices.Efem.Efem>();
            Paused = false;
            Pausing = false;
        }
    }
}
