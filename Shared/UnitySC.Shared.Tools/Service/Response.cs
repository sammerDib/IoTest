using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Service
{
    [DataContract]
    public class Response<T>
    {
        /// <summary>
        /// Request result.
        /// </summary>
        [DataMember]
        public T Result { get; set; }

        /// <summary>
        /// Messages from the processed operation.
        /// </summary>
        [DataMember]
        public List<Message> Messages { get; set; }

        /// <summary>
        /// Exceptions raised by the server
        /// </summary>
        [DataMember]
        public ExceptionService Exception { get; set; }
    }

    [DataContract]
    public class VoidResult
    {
    }
}
