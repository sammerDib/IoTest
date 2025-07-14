using System;
using System.Runtime.Serialization;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion
{
    [Serializable]
    public class TaskCompletionException : Exception
    {
        public TaskCompletionInfos CompletionInfo { get; set; }

        public TaskCompletionException() : base()
        {
        }

        public TaskCompletionException(string message) : base(message)
        {
        }

        public TaskCompletionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskCompletionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
