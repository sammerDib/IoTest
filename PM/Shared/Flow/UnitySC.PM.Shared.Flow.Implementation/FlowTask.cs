using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    
    public class FlowTask<TInput, TResult, TConfiguration> : IFlowTask where TResult : IFlowResult
        where TInput : IFlowInput
        where TConfiguration : FlowConfigurationBase
    {
        private readonly Task<TResult> _originalUnderlyingTask;
        protected readonly CancellationTokenSource CancellationSource;

        protected Task<TResult> UnderlyingTask;

        public delegate void BeforeFlow2Action(FlowTask<TInput, TResult> task);

        public delegate void BeforeFlow3Action(FlowTask<TInput, TResult, TConfiguration> task);

        public FlowTask(FlowComponent<TInput, TResult, TConfiguration> flowComponent, TimeSpan? timeout = null)
        {
            
            CancellationSource = timeout.HasValue
                ? new CancellationTokenSource(timeout.Value)
                : new CancellationTokenSource();
            Name = flowComponent.Name;
            flowComponent.CancellationToken = CancellationSource.Token;
            Input = flowComponent.Input;
            Configuration = flowComponent.Configuration;
            _originalUnderlyingTask = new Task<TResult>(() => FuncToExecute(flowComponent));
            UnderlyingTask = _originalUnderlyingTask;
        }

        public FlowTask(FlowComponent<TInput, TResult, TConfiguration> flowComponent,
            CancellationTokenSource cancellationTokenSource)
        {
            
            CancellationSource = cancellationTokenSource;
            Name = flowComponent.Name;
            flowComponent.CancellationToken = CancellationSource.Token;
            Input = flowComponent.Input;
            Configuration = flowComponent.Configuration;
            _originalUnderlyingTask = new Task<TResult>(() => FuncToExecute(flowComponent));
            UnderlyingTask = _originalUnderlyingTask;
        }

        public TResult Result => UnderlyingTask.Result;

        public Exception Exception => UnderlyingTask.Exception;

        public bool IsCanceled => UnderlyingTask.IsCanceled;

        public bool IsFaulted => UnderlyingTask.IsFaulted;

        public bool IsCompleted => UnderlyingTask.IsCompleted;

        public string Name { get; }
        
        public TInput Input { get; }
        
        public TConfiguration Configuration { get; }

        public TaskStatus Status => UnderlyingTask.Status;

        public void Start()
        {
            UnderlyingTask.Start();
        }


        public void Cancel()
        {
            ClassLocator.Default.GetInstance<ILogger>()
                .Information($"[Task for {Name}] cancellation is explicitly requested");
            CancellationSource.Cancel();
        }

        public Task ToTask()
        {
            return (Task)this;
        }

        public void Wait()
        {
            UnderlyingTask.Wait();
        }
        
        public Task CheckSuccessAndContinueWith(Action<Task> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return ContinueWith(task =>
            {
                CheckSuccess(this);
                continuationAction(task);
            }, continuationOptions);
        }

        public Task ContinueWith(Action<Task> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return UnderlyingTask.ContinueWith(continuationAction, continuationOptions);
        }
        
        public Task CheckSuccessAndContinueWith(Action<Task<TResult>> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return ContinueWith(task =>
            {
                CheckSuccess(this);
                continuationAction(task);
            }, continuationOptions);
        }

        public Task ContinueWith(Action<Task<TResult>> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return UnderlyingTask.ContinueWith(continuationAction, continuationOptions);
        }

        public Task<TTaskResult> CheckSuccessAndContinueWith<TTaskResult>(
            Func<Task, TTaskResult> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return ContinueWith(task =>
            {
                CheckSuccess(this);
                return continuationAction(task);
            }, continuationOptions);
        }

        public Task<TTaskResult> ContinueWith<TTaskResult>(Func<Task, TTaskResult> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return UnderlyingTask.ContinueWith(continuationAction, continuationOptions);
        }

        public Task<TTaskResult> CheckSuccessAndContinueWith<TTaskResult>(
            Func<Task<TResult>, TTaskResult> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return ContinueWith(task =>
            {
                CheckSuccess(this);
                return continuationAction(task);
            }, continuationOptions);
        }
        
        public Task<TTaskResult> ContinueWith<TTaskResult>(Func<Task<TResult>, TTaskResult> continuationAction,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
        {
            return UnderlyingTask.ContinueWith(continuationAction, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckSuccessAndContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
                BeforeFlow3Action beforeAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWithImpl(continuationFlowTask, null,
                beforeAction, null, true, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            ContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
                BeforeFlow3Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWithImpl(continuationFlowTask, null,
                beforeStartAction, null, false, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult>
            CheckSuccessAndContinueWith<TNewInput, TNewResult>(
                FlowTask<TNewInput, TNewResult> continuationFlowTask,
                BeforeFlow3Action beforeAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWithImpl<TNewInput, TNewResult, DefaultConfiguration>(null, continuationFlowTask,
                beforeAction, null, true, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult> ContinueWith<TNewInput, TNewResult>(
            FlowTask<TNewInput, TNewResult> continuationFlowTask,
            BeforeFlow3Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None)
            where TNewInput : IFlowInput where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWithImpl<TNewInput, TNewResult, DefaultConfiguration>(null,
                continuationFlowTask, beforeStartAction, null, false, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckSuccessAndContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
                BeforeFlow3Action beforeAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, CancellationSource);
            return CheckSuccessAndContinueWith(continuationFlowTask, beforeAction, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            ContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
                BeforeFlow3Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask =
                new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, CancellationSource);
            return ContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult>
            CheckSuccessAndContinueWith<TNewInput, TNewResult>(
                FlowComponent<TNewInput, TNewResult> continuationFlow,
                BeforeFlow3Action beforeAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, CancellationSource);
            return CheckSuccessAndContinueWith(continuationFlowTask, beforeAction, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult> ContinueWith<TNewInput, TNewResult>(
            FlowComponent<TNewInput, TNewResult> continuationFlow,
            BeforeFlow3Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, CancellationSource);
            return ContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }

        public TaskAwaiter<TResult> GetAwaiter()
        {
            return UnderlyingTask.GetAwaiter();
        }

        private static TResult FuncToExecute(FlowComponent<TInput, TResult, TConfiguration> flowComponent)
        {
            //No need for try-catch block here as all exception are handled in FlowComponent.Execute method
            flowComponent.Execute();

            return flowComponent.Result;
        }

        public static FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckPreviousFlowTasksSuccessAndContinueWhenAll<TNewInput, TNewResult, TNewConfiguration>(
                IFlowTask[] flowTasks, FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
                Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWhenAllImpl(flowTasks,
                continuationFlowTask, null, beforeStartAction, true);
        }

        public static FlowTask<TNewInput, TNewResult, TNewConfiguration> ContinueWhenAll<TNewInput, TNewResult,
            TNewConfiguration>(IFlowTask[] flowTasks,
            FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
            Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWhenAllImpl(flowTasks,
                continuationFlowTask, null, beforeStartAction, false);
        }
        
        public static FlowTask<TNewInput, TNewResult>
            CheckPreviousFlowTasksSuccessAndContinueWhenAll<TNewInput, TNewResult>(IFlowTask[] flowTasks,
                FlowTask<TNewInput, TNewResult> continuationFlowTask,
                Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWhenAllImpl<TNewInput, TNewResult, DefaultConfiguration>(
                flowTasks, null, continuationFlowTask, beforeStartAction, true);
        }

        public static FlowTask<TNewInput, TNewResult> ContinueWhenAll<TNewInput, TNewResult>(IFlowTask[] flowTasks,
            FlowTask<TNewInput, TNewResult> continuationFlowTask, Action<IFlowTask[]> beforeStartAction)
            where TNewInput : IFlowInput where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWhenAllImpl<TNewInput, TNewResult, DefaultConfiguration>(
                flowTasks, null, continuationFlowTask, beforeStartAction, false);
        }

        public static FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckPreviousFlowTasksSuccessAndContinueWhenAll<TNewInput, TNewResult, TNewConfiguration>(
                IFlowTask[] flowTasks, FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
                CancellationTokenSource cancellationTokenSource,
                Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask =
                new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, cancellationTokenSource);
            return CheckPreviousFlowTasksSuccessAndContinueWhenAll(flowTasks, continuationFlowTask, beforeStartAction);
        }
        
        public static FlowTask<TNewInput, TNewResult, TNewConfiguration> ContinueWhenAll<TNewInput, TNewResult,
            TNewConfiguration>(IFlowTask[] flowTasks,
            FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
            CancellationTokenSource cancellationTokenSource,
            Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, cancellationTokenSource);
            return ContinueWhenAll(flowTasks, continuationFlowTask, beforeStartAction);
        }
        
        public static FlowTask<TNewInput, TNewResult> CheckPreviousFlowTasksSuccessAndContinueWhenAll<TNewInput,
            TNewResult>(IFlowTask[] flowTasks,
            FlowComponent<TNewInput, TNewResult> continuationFlow,
            CancellationTokenSource cancellationTokenSource,
            Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, cancellationTokenSource);
            return CheckPreviousFlowTasksSuccessAndContinueWhenAll(flowTasks, continuationFlowTask, beforeStartAction);
        }

        public static FlowTask<TNewInput, TNewResult> ContinueWhenAll<TNewInput, TNewResult>(IFlowTask[] flowTasks,
            FlowComponent<TNewInput, TNewResult> continuationFlow,
            CancellationTokenSource cancellationTokenSource, Action<IFlowTask[]> beforeStartAction) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, cancellationTokenSource);
            return ContinueWhenAll(flowTasks, continuationFlowTask, beforeStartAction);
        }

        public static explicit operator Task(FlowTask<TInput, TResult, TConfiguration> flowTask)
        {
            return flowTask.UnderlyingTask;
        }

        public static explicit operator Task<TResult>(FlowTask<TInput, TResult, TConfiguration> flowTask)
        {
            return flowTask.UnderlyingTask;
        }

        protected IFlowTask ContinueWithImpl<TNewInput, TNewResult, TNewConfiguration>(
            FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask3 = null,
            FlowTask<TNewInput, TNewResult> continuationFlowTask2 = null,
            BeforeFlow3Action beforeStartAction3 = null,
            BeforeFlow2Action beforeStartAction2 = null, bool checkPreviousFlowSuccess = true,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var instance2 = this as FlowTask<TInput, TResult>;
            CheckExactlyOneContinuationFlowTaskIsNull(continuationFlowTask3, continuationFlowTask2);
            var continuationFlowTask = (IFlowTask)continuationFlowTask3 ?? continuationFlowTask2;
            string continuationFlowTaskName = continuationFlowTask3?.Name ?? continuationFlowTask2.Name;
            var originalTask = GetRelevantOriginalTask(continuationFlowTask3, continuationFlowTask2);
            var continuationTask = UnderlyingTask.ContinueWith(task =>
            {
                try
                {
                    if (checkPreviousFlowSuccess)
                    {  
                        CheckSuccess(this);
                    }
                    if (!(instance2 is null))
                    {
                        beforeStartAction2?.Invoke(instance2);
                    }
                    else
                    {
                        beforeStartAction3?.Invoke(this);
                    }

                    originalTask.RunSynchronously();
                    return originalTask.Result;
                }
                catch (TaskCanceledException)
                {
                    var canceledResult = Activator.CreateInstance<TNewResult>();
                    canceledResult.Status = new FlowStatus
                    {
                        Message = $"{continuationFlowTaskName} was cancelled because at least one of the previous flow tasks was cancelled or in error",
                        State = FlowState.Canceled
                    };
                    var cancelledTask = Task.FromResult(canceledResult);
                    return cancelledTask.Result;
                }
            }, continuationOptions);
            SetRelevantUnderlyingTask(continuationFlowTask3, continuationFlowTask2, continuationTask);
            return continuationFlowTask;
        }

        private static IFlowTask ContinueWhenAllImpl<TNewInput, TNewResult, TNewConfiguration>(IFlowTask[] flowTasks,
            FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask3,
            FlowTask<TNewInput, TNewResult> continuationFlowTask2, Action<IFlowTask[]> beforeStartAction,
            bool checkPreviousFlowTasksSuccess)
            where TNewInput : IFlowInput where TNewResult : IFlowResult where TNewConfiguration : FlowConfigurationBase
        {
            CheckExactlyOneContinuationFlowTaskIsNull(continuationFlowTask3, continuationFlowTask2);
            var continuationFlowTask = (IFlowTask)continuationFlowTask3 ?? continuationFlowTask2;
            string continuationFlowTaskName = continuationFlowTask3?.Name ?? continuationFlowTask2.Name;
            var originalTask = GetRelevantOriginalTask(continuationFlowTask3, continuationFlowTask2);
            var tasks = flowTasks.Select(flowTask => flowTask.ToTask()).ToArray();
            var continuationTask = Task.Factory.ContinueWhenAll(tasks, prevTasks =>
            {
                try
                {
                    if (checkPreviousFlowTasksSuccess)
                    {
                        CheckPreviousFlowTasksSuccess(flowTasks);
                    }

                    beforeStartAction?.Invoke(flowTasks);
                    originalTask.RunSynchronously();
                    return originalTask.Result;
                }
                catch (TaskCanceledException ignored)
                {
                    var canceledResult = Activator.CreateInstance<TNewResult>();
                    canceledResult.Status = new FlowStatus
                    {
                        Message = $"{continuationFlowTaskName} was cancelled because at least one of the previous flow tasks was cancelled or in error",
                        State = FlowState.Canceled
                    };
                    var cancelledTask = Task.FromResult(canceledResult);
                    return cancelledTask.Result;
                }
                
            });
            SetRelevantUnderlyingTask(continuationFlowTask3, continuationFlowTask2, continuationTask);
            return continuationFlowTask;
        }

        private static void CheckPreviousFlowTasksSuccess(IFlowTask[] flowTasks)
        {
            try
            {
                foreach (var flowTasksByType in flowTasks.GroupBy(task => task.GetType()))
                {
                    var checkMethodForType = flowTasksByType.Key
                        .GetMethod(nameof(CheckSuccess), BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(flowTasksByType.Key.GetGenericArguments());
                    foreach (var task in flowTasksByType)
                    {
                        checkMethodForType.Invoke(null, new object[] {task});
                    }
                }
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException is TaskCanceledException)
                {
                    throw e.InnerException;
                }

                throw;
            }
            
        }

        private static void CheckSuccess<TNewInput, TNewResult, TNewConfiguration>(
            FlowTask<TNewInput, TNewResult, TNewConfiguration> flowTask) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            if (flowTask.Status != TaskStatus.RanToCompletion ||
                flowTask.Result.Status.State != FlowState.Success)
            {
                throw new TaskCanceledException();
            }
        }

        private static Task<TNewResult> GetRelevantOriginalTask<TNewInput, TNewResult, TNewConfiguration>(
            FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask3,
            FlowTask<TNewInput, TNewResult> continuationFlowTask2) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return continuationFlowTask3 is null
                ? continuationFlowTask2._originalUnderlyingTask
                : continuationFlowTask3._originalUnderlyingTask;
        }

        private static void SetRelevantUnderlyingTask<TNewInput, TNewResult, TNewConfiguration>(
            FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask3,
            FlowTask<TNewInput, TNewResult> continuationFlowTask2, Task<TNewResult> continuationTask)
            where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            if (continuationFlowTask3 is null)
            {
                continuationFlowTask2.UnderlyingTask = continuationTask;
            }
            else
            {
                continuationFlowTask3.UnderlyingTask = continuationTask;
            }
        }

        private static void
            CheckExactlyOneContinuationFlowTaskIsNull<TNewInput, TNewResult, TNewConfiguration>(
                FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask3,
                FlowTask<TNewInput, TNewResult> continuationFlowTask2) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            if ((continuationFlowTask2 is null && continuationFlowTask3 is null) ||
                (!(continuationFlowTask2 is null) && !(continuationFlowTask3 is null)))
            {
                throw new ArgumentException(
                    $"Exactly one of {nameof(continuationFlowTask3)} and {nameof(continuationFlowTask2)} cannot be null, not both, nor none");
            }
        }
    }

    public class FlowTask<TInput, TResult> : FlowTask<TInput, TResult, DefaultConfiguration>
        where TResult : IFlowResult where TInput : IFlowInput
    {
        public FlowTask(FlowComponent<TInput, TResult, DefaultConfiguration> flowComponent, TimeSpan? timeout = null) :
            base(flowComponent, timeout)
        {
        }

        public FlowTask(FlowComponent<TInput, TResult, DefaultConfiguration> flowComponent,
            CancellationTokenSource cancellationTokenSource) : base(flowComponent, cancellationTokenSource)
        {
        }

        public FlowTask<TNewInput, TNewResult> CheckSuccessAndContinueWith<TNewInput, TNewResult>(
            FlowTask<TNewInput, TNewResult> continuationFlowTask,
            BeforeFlow2Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWithImpl(continuationFlowTask, null, null,
                beforeStartAction, true, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult> ContinueWith<TNewInput, TNewResult>(
            FlowTask<TNewInput, TNewResult> continuationFlowTask,
            BeforeFlow2Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            return (FlowTask<TNewInput, TNewResult>)ContinueWithImpl(continuationFlowTask, null, null,
                beforeStartAction, false, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult> CheckSuccessAndContinueWith<TNewInput, TNewResult>(
            FlowComponent<TNewInput, TNewResult> continuationFlow,
            BeforeFlow2Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, CancellationSource);
            return CheckSuccessAndContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult> ContinueWith<TNewInput, TNewResult>(
            FlowComponent<TNewInput, TNewResult> continuationFlow,
            BeforeFlow2Action beforeStartAction = null,
            TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
        {
            var continuationFlowTask = new FlowTask<TNewInput, TNewResult>(continuationFlow, CancellationSource);
            return ContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckSuccessAndContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
                BeforeFlow2Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWithImpl(continuationFlowTask, null,
                null, beforeStartAction, true, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            ContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowTask<TNewInput, TNewResult, TNewConfiguration> continuationFlowTask,
                BeforeFlow2Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            return (FlowTask<TNewInput, TNewResult, TNewConfiguration>)ContinueWithImpl(continuationFlowTask, null,
                null, beforeStartAction, false, continuationOptions);
        }

        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            CheckSuccessAndContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
                BeforeFlow2Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask =
                new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, CancellationSource);
            return CheckSuccessAndContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }
        
        public FlowTask<TNewInput, TNewResult, TNewConfiguration>
            ContinueWith<TNewInput, TNewResult, TNewConfiguration>(
                FlowComponent<TNewInput, TNewResult, TNewConfiguration> continuationFlow,
                BeforeFlow2Action beforeStartAction = null,
                TaskContinuationOptions continuationOptions = TaskContinuationOptions.None) where TNewInput : IFlowInput
            where TNewResult : IFlowResult
            where TNewConfiguration : FlowConfigurationBase
        {
            var continuationFlowTask =
                new FlowTask<TNewInput, TNewResult, TNewConfiguration>(continuationFlow, CancellationSource);
            return ContinueWith(continuationFlowTask, beforeStartAction, continuationOptions);
        }

        public static explicit operator Task(FlowTask<TInput, TResult> flowTask)
        {
            return flowTask.UnderlyingTask;
        }

        public static explicit operator Task<TResult>(FlowTask<TInput, TResult> flowTask)
        {
            return flowTask.UnderlyingTask;
        }
    }
}
