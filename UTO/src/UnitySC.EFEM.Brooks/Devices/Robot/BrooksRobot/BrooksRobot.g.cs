using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot
{
    public partial class BrooksRobot : UnitySC.Equipment.Abstractions.Devices.Robot.Robot, IBrooksRobot
    {
        public static new readonly DeviceType Type;

        static BrooksRobot()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.BrooksRobot.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.BrooksRobot");
            }
        }

        public BrooksRobot(string name)
            : this(name, Type)
        {
        }

        protected BrooksRobot(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GetMotionProfiles":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetMotionProfiles();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetMotionProfiles(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetMotionProfiles interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetMotionProfile":
                    {
                        System.String motionprofile = (System.String)execution.Context.GetArgument("motionProfile");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetMotionProfile(motionprofile);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetMotionProfile(motionprofile, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetMotionProfile interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public void GetMotionProfiles()
        {
            CommandExecution execution = new CommandExecution(this, "GetMotionProfiles");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetMotionProfilesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GetMotionProfiles");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetMotionProfile(string motionProfile)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("motionProfile", motionProfile));
            CommandExecution execution = new CommandExecution(this, "SetMotionProfile", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetMotionProfileAsync(string motionProfile)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("motionProfile", motionProfile));
            CommandExecution execution = new CommandExecution(this, "SetMotionProfile", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public string MotionProfile
        {
            get { return (string)GetStatusValue("MotionProfile"); }
            protected set { SetStatusValue("MotionProfile", value); }
        }
    }
}
