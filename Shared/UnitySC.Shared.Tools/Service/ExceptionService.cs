using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Service
{
    [DataContract]
    public class ExceptionService
    {
        public ExceptionService(string message, ExceptionType type, string stackTrace = null)
        {
            this.Message = message;
            this.Type = type;
            this.StackTrace = stackTrace;
        }

        /// <summary>
        /// Message level
        /// </summary>
        [DataMember]
        public ExceptionType Type { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Stack trace de l'exception.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string InnerException { get; set; }

        public override string ToString()
        {
            return "Message exception : " + Message;
        }
    }
}