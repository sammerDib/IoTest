using System;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.DMT.Service.Flows.SaveImage
{
    
    public class SaveImageFlowTask : FlowTask<SaveImageInput, SaveImageResult, SaveImageConfiguration>
    {
        public SaveImageFlowTask(SaveImageFlow flowComponent, TimeSpan? timeout = null) : base(flowComponent, timeout)
        {
        }

        public SaveImageFlowTask(SaveImageFlow flowComponent, CancellationTokenSource cancellationTokenSource) : base(flowComponent, cancellationTokenSource)
        {
        }

        public Task ContinueWithResultGeneratedHandlerInvocationIfNeeded(Action<DMTResultGeneratedEventArgs> onResultGeneratedEventHandler)
        {
            return UnderlyingTask.ContinueWith(task =>
            {
                if (Status == TaskStatus.RanToCompletion && Result.Status.State == FlowState.Success)
                {
                    onResultGeneratedEventHandler(new DMTResultGeneratedEventArgs(Result.ImageName, Result.ImageSide,
                        Result.SavePath));
                }
            });
        }
    }
}
