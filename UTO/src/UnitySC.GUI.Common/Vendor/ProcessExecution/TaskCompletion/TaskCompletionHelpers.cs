using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Agileo.Common.Tracing;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion
{
    public static class TaskCompletionHelpers
    {
        public static string DefineTaskReplyInfo(TaskCompletionSource<TaskCompletionInfos> tcs, string activity)
        {
            string message = string.Empty;
            switch (tcs.Task.Result)
            {
                case TaskCompletionInfos.Executed:
                    break;
                case TaskCompletionInfos.Failed:
                    message = activity + " Failed event received from device";
                    break;
                case TaskCompletionInfos.Aborted:
                    message = activity + " Aborted event received from device";
                    break;
                case TaskCompletionInfos.Stopped:
                    message = activity + "Stopped event received";
                    break;
                case TaskCompletionInfos.ErrorDetected:
                    message = activity + " Error event received from device";
                    break;
                case TaskCompletionInfos.TimeOutDetected:
                    message = activity + " TimeOut detected";
                    break;
                case TaskCompletionInfos.CommunicationClosed:
                    message = activity + " Communication closed detected";
                    break;
                case TaskCompletionInfos.AlarmDetected:
                    message = activity + " Alarm detected";
                    break;
                case TaskCompletionInfos.ExceptionDetected:
                    message = activity + " Exception detected";
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(tcs.Task.Result),
                        (int)tcs.Task.Result,
                        typeof(TaskCompletionInfos));
            }

            return message;
        }
    }

    public class WaitEndOfContractHelper
    {
        public void WaitEndOfContract(string activityName, Duration timeOut, TaskCompletionSource<TaskCompletionInfos> tcs)
        {
            if (tcs == null)
            {
                throw new ArgumentNullException(nameof(tcs), "TaskCompletionSource can't be null");
            }
            //Set TaskCompletionSource and wait with timeOut as timeout
            int tOut = System.Threading.Timeout.Infinite;
            if (timeOut.Value > 0)
            {
                tOut = (int)timeOut.As(UnitsNet.Units.DurationUnit.Millisecond);
            }

            bool wasSignaled = tcs.Task.Wait(tOut);
            //control if TaskCompletionSource released by finished event or any else.
            if (!wasSignaled)
            {
                throw new TimeoutException($"{activityName} - TimeOut {timeOut.Seconds} detected before reply from device");
            }

            string message = TaskCompletionHelpers.DefineTaskReplyInfo(tcs, activityName);
            if (!string.IsNullOrEmpty(message)) //Fault detected
            {
                TaskCompletionException ex = new TaskCompletionException(message)
                { CompletionInfo = tcs.Task.Result };
                TraceManager.Instance().Trace("WaitEndOfActivityHelper",
                    TraceLevelType.Error, ex.Message);
            }
        }
    }
}
