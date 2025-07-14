using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    public abstract class FlowComponent<TInput, TResult> : FlowComponent<TInput, TResult, DefaultConfiguration> where TInput : IFlowInput where TResult : IFlowResult
    {
        protected FlowComponent(TInput input, string name) : base(input, name)
        {
        }
    }

    /// <summary>
    /// Flow used to execute algo and measure on the tool
    /// </summary>
    /// <typeparam name="TInput">Flow input</typeparam>
    /// <typeparam name="TResult">Flow result</typeparam>
    /// <typeparam name="TConfiguration">Configuration of the flow : Default values can be overide by the content of IFlowsConfiguration</typeparam>
    public abstract class FlowComponent<TInput, TResult, TConfiguration> where TInput : IFlowInput where TResult : IFlowResult where TConfiguration : FlowConfigurationBase
    {
        private const string FlowsReportFolderName = "Flows";

        public delegate void StatusChangedEventHandler(FlowStatus status, TResult statusData);

        public ILogger Logger { get; private set; }
        public IGlobalStatusServer GlobalStatusServer { get; private set; }
        public CancellationToken CancellationToken { get; set; }

        protected FlowFDCProvider FdcProvider;

        private readonly ContextApplier<TInput> _contextApplier;
        private PMConfiguration _pmConfiguration;

        public FlowComponent(TInput input, string name)
        {
            Input = input;
            Name = name;
            Logger = ClassLocator.Default.GetInstance<ILogger>();
            GlobalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();
            _contextApplier = ClassLocator.Default.GetInstance<ContextApplier<TInput>>();
            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();

            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>();

            if (flowsConfiguration != null && flowsConfiguration.Flows != null)
            {
                _configuration = flowsConfiguration.Flows.OfType<TConfiguration>().FirstOrDefault();
            }
        }

        private void CreateReportFolder()
        {
            if (_configuration != null && _configuration.IsAnyReportEnabled())
            {
                if (string.IsNullOrEmpty(ReportFolder))
                {
                    string reportFolder = Path.Combine(Logger.LogDirectory, FlowsReportFolderName, Name.Replace(' ', '_'));
                    reportFolder = Path.Combine(reportFolder, DateTime.Now.ToString("yyyyMMdd-HHmmss.fff"));
                    ReportFolder = reportFolder;
                }
                try
                {
                    Directory.CreateDirectory(ReportFolder);
                }
                catch (IOException e)
                {
                    Logger.Error($"{LogHeader} Cannot create report folder: {e.Message}");
                    ReportFolder = "";
                }
                catch (UnauthorizedAccessException e)
                {
                    Logger.Error($"{LogHeader} Cannot create report folder: {e.Message}");
                    ReportFolder = "";
                }
                catch (ArgumentException e)
                {
                    Logger.Error($"{LogHeader} Cannot create report folder: {e.Message}");
                    ReportFolder = "";
                }
                catch (NotSupportedException e)
                {
                    Logger.Error($"{LogHeader} Cannot create report folder: {e.Message}");
                    ReportFolder = "";
                }
            }
        }

        private TConfiguration _configuration;

        public TConfiguration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = (TConfiguration)Activator.CreateInstance(typeof(TConfiguration));
                return _configuration;
            }
        }

        protected string LogHeader => $"[{Name}]";

        public string ReportFolder { get; set; }

        public string Name { get; }
        public TInput Input { get; set; }

        public event StatusChangedEventHandler StatusChanged;

        public virtual TResult Execute()
        {
            FdcProvider?.IncrementFlowTotalCounter();

            CreateReportFolder();

            Result = (TResult)Activator.CreateInstance(typeof(TResult));
            SetState(FlowState.InProgress);

            var validity = Input.CheckInputValidity();
            if (!validity.IsValid)
            {
                SetState(FlowState.Error, $"At least one flow input parameter is invalid :{Environment.NewLine}" +
                    $"{string.Join(Environment.NewLine, validity.Message.ToArray())}");
                return Result;
            }

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name);
            }

            try
            {
                ApplyContext();

                ProcessFlow();
                CheckCancellation();
                SetState(FlowState.Success);
                FdcProvider?.IncrementFlowSuccessCounter();
            }
            catch (PartialException e)
            {
                SetState(FlowState.Partial, $"{e.Message}");
            }
            catch (TaskCanceledException e)
            {
                SetState(FlowState.Canceled, $"{e.Message}");
            }
            catch (Exception e)
            {
                SetState(FlowState.Error, $"{e.Message}");
            }
            finally
            {
                FdcProvider?.CreateFDC();
            }

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name);
            }

            Report();
            return Result;
        }

        protected abstract void Process();

        private void ProcessFlow()
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name + "Execution");
            }

            Process();

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name + "Execution");
            }
        }

        private void ApplyContext()
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(GetType().Name + "Preparation");
            }

            _contextApplier.ApplyInitialContext(Input);

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(GetType().Name + "Preparation");
            }
        }

        public TResult Result { get; set; }

        private void OnStatusChange(FlowStatus status, TResult statusData = default)
        {
            StatusChanged?.Invoke(status, statusData);
            if (status.State == FlowState.Error)
                GlobalStatusServer.AddMessage(new UnitySC.Shared.Tools.Service.Message(UnitySC.Shared.Tools.Service.MessageLevel.Error, $"Error in flow {Name}: {status.Message}"));
        }

        protected void SetProgressMessage(string message = null, TResult statusData = default)
        {
            SetState(FlowState.InProgress, message, statusData);
        } 
        
        private void SetState(FlowState state, string message = null, TResult stateData = default)
        {
            var status = new FlowStatus(state, message);

            if (!EqualityComparer<TResult>.Default.Equals(stateData, default(TResult)))
                Result = stateData;
            Result.Status = status;

            Action<string, object[]> loggingMethod = Logger.Information;
            if (state == FlowState.Error)
            {
                loggingMethod = Logger.Error;
            }

            string logMessage = message is null
                ? $"{LogHeader} State changed to {state}"
                : $"{LogHeader} Status changed to {status.State}: {status.Message}";


            loggingMethod(logMessage, null);
            
            OnStatusChange(status, Result);
        }

        public void CheckCancellation()
        {
            if (CancellationToken != CancellationToken.None && CancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException($"{Name} is canceled");
            }
        }

        protected static bool IsResultStateError(IFlowResult result, string name)
        {
            return result.Status.State == FlowState.Error;
        }

        private void Report()
        {
            if (!Configuration.IsAnyReportEnabled())
            {
                return;
            }

            string foldersuffix = string.Empty;
            try
            {
                Input.Serialize(Path.Combine(ReportFolder, $"input.txt"));
                Result.Serialize(Path.Combine(ReportFolder, $"result_{ResultStatus()}.txt"));
                if (Result?.Status != null && Result.Status.State != FlowState.Success)
                    foldersuffix = "_Err";
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
                foldersuffix = "_Exp";
            }

            if (!string.IsNullOrEmpty(foldersuffix))
            {
                RenameReportFolderOnErrorOrException(foldersuffix);
            }
            else if (Configuration.WriteReportMode == FlowReportConfiguration.WriteOnError)
            {
                DeleteReportFolderOnSuccess();
            }
        }

        private void RenameReportFolderOnErrorOrException(string foldersuffix)
        {
            try
            {
                Directory.Move(ReportFolder, ReportFolder + foldersuffix);
                ReportFolder += foldersuffix; // update ReportFolder that has just been renamed
            }
            catch
            {
                // destination directory already exist so avoid to add suffix
            }
        }

        private void DeleteReportFolderOnSuccess()
        {
            Directory.Delete(ReportFolder, true);
        }

        private string ResultStatus()
        {
            return Result == null ? "null" : Result.Status == null ? "ukn" : Result.Status.State.ToString();
        }

        private void StartMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.Tag_Start(name);
        }

        private void EndMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.Tag_End(name);
        }
    }
}
