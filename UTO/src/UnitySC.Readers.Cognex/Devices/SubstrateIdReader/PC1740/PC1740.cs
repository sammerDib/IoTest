using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Configuration;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740
{
    public partial class PC1740 : IConfigurableDevice<PC1740Configuration>
    {
        #region Fields

        private List<string> _fileNamesList;
        private CognexSubstrateIdReaderDriver Driver { get; set; }

        private DriverWrapper DriverWrapper { get; set; }

        #endregion Fields

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        var endReplyIndicators = new List<string>
                        {
                            CognexConstants.CognexEndReplyIndicatorLogin,
                            CognexConstants.CognexEndReplyIndicatorRead
                        };
                        Driver = new CognexSubstrateIdReaderDriver(
                            Logger,
                            Configuration.CommunicationConfig.ConnectionMode,
                            (byte)InstanceId,
                            aliveBitPeriod: Configuration.CommunicationConfig.AliveBitPeriod);
                        Driver.Setup(
                            Configuration.CommunicationConfig.IpAddress,
                            Configuration.CommunicationConfig.TcpPort,
                            Configuration.CommunicationConfig.AnswerTimeout,
                            Configuration.CommunicationConfig.ConfirmationTimeout,
                            Configuration.CommunicationConfig.InitTimeout,
                            "\r\n",
                            Configuration.CommunicationConfig.MaxNbRetry,
                            Configuration.CommunicationConfig.ConnectionRetryDelay,
                            endReplyIndicators);
                        Driver.SubstrateIdReceived += Driver_SubstrateIdReceived;
                        Driver.FileNamesListReceived += Driver_FileNamesListReceived;
                        Driver.CommunicationEstablished += Driver_CommunicationEstablished;
                        Driver.CommunicationClosed += Driver_CommunicationClosed;
                        Driver.CommunicationStarted += Driver_CommunicationStarted;
                        Driver.CommunicationStopped += Driver_CommunicationStopped;
                        DriverWrapper = new DriverWrapper(Driver, Logger);
                    }
                    else if (ExecutionMode == ExecutionMode.Simulated)
                    {
                        SetUpSimulatedMode();
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region ICommunicatingDevice Commands

        protected override void InternalStartCommunication()
        {
            Driver.EnableCommunications();
        }

        protected override void InternalStopCommunication()
        {
            Driver.Disconnect();
        }

        #endregion ICommunicatingDevice Commands

        #region ISubstrateIdReader Commands

        protected override void InternalRequestRecipes()
        {
            try
            {
                DriverWrapper.RunCommand(
                    delegate { Driver.GetFileList(); },
                    SubstrateIdReaderCommand.GetFileList);
                var recipes = new List<RecipeModel>();
                if (Configuration.UseOnlyOneT7)
                {
                    if (Configuration.T7Recipe == null)
                    {
                        throw new InvalidOperationException(
                            $"{nameof(Configuration.T7Recipe)} is null.");
                    }

                    recipes.Add(
                        new RecipeModel(
                            1,
                            Configuration.T7Recipe.Name,
                            false,
                            true,
                            Configuration.T7Recipe.Angle));
                }
                else
                {
                    recipes.AddRange(RecipeLibrarian.GetRecipes(Configuration.RecipeFolderPath));
                }

                if (recipes.Count == 0)
                {
                    throw new InvalidOperationException("No recipe stored in the recipe folder.");
                }

                // Check the two list
                var errors = recipes.FindAll(
                    r => r.IsStored && !_fileNamesList.Contains(r.Name + ".job"));
                if (errors.Count == 0)
                {
                    Recipes.Clear();
                    Recipes.AddRange(recipes);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Some Recipe are stored in the config file but not present on the camera ({errors})");
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalRead(string recipeName)
        {
            try
            {
                if (!string.IsNullOrEmpty(recipeName))
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.LoadJob(recipeName); },
                        SubstrateIdReaderCommand.LoadJob);
                }

                DriverWrapper.RunCommand(
                    delegate { Driver.Read(); },
                    SubstrateIdReaderCommand.Read);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        protected override void InternalGetImage(string imagePath)
        {
            try
            {
                var pathFolder = Path.GetDirectoryName(imagePath);
                if (Directory.Exists(pathFolder))
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.GetImage(imagePath); },
                        SubstrateIdReaderCommand.GetImage);
                }
                else
                {
                    var userMessage = $"The path folder '{pathFolder}' does not exist.";
                    OnUserErrorRaised(userMessage);
                    Logger.Error(userMessage);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion ISubstrateIdReader Commands

        #region Configuration

        public new PC1740Configuration Configuration
            => ConfigurationExtension.Cast<PC1740Configuration>(base.Configuration);

        public PC1740Configuration CreateDefaultConfiguration()
        {
            return new PC1740Configuration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader)}/{nameof(PC1740)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region Event Handlers

        private void Driver_SubstrateIdReceived(object sender, SubstrateIdReceivedEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            SubstrateId = e.SubstrateId;
            if (!e.IsSucceed)
            {
                var task = new Task(() => InternalGetImage(Configuration.ImagePath));
                task.Start();
            }
        }

        private void Driver_FileNamesListReceived(object sender, FileNamesReceivedEventArgs e)
        {
            if (e != null)
            {
                _fileNamesList = e.Recipes;
            }
        }

        private void Driver_CommunicationEstablished(object sender, EventArgs e)
        {
            IsCommunicationStarted = IsCommunicating = true;
            SetState(OperatingModes.Idle);
        }

        private void Driver_CommunicationClosed(object sender, EventArgs e)
        {
            IsCommunicating = false;
            SetState(OperatingModes.Maintenance);
        }

        private void Driver_CommunicationStopped(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        private void Driver_CommunicationStarted(object sender, EventArgs e)
        {
            IsCommunicationStarted = Driver.IsCommunicationStarted;
        }

        #endregion Event Handlers

        #region Other Methods

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            base.InternalInterrupt(interruption, interruptedExecution);
            if (ExecutionMode == ExecutionMode.Real)
            {
                DriverWrapper?.InterruptTask();
                Driver.ClearCommandsQueue();
            }
        }

        #endregion Other Methods

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Driver == null)
                {
                    return;
                }

                Driver.SubstrateIdReceived -= Driver_SubstrateIdReceived;
                Driver.FileNamesListReceived -= Driver_FileNamesListReceived;
                Driver.CommunicationEstablished -= Driver_CommunicationEstablished;
                Driver.CommunicationClosed -= Driver_CommunicationClosed;
                Driver.CommunicationStarted -= Driver_CommunicationStarted;
                Driver.CommunicationStopped -= Driver_CommunicationStopped;
                Driver = null;
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
