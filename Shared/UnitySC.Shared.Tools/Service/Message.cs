using System;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Tools.Service
{
    [DataContract]
    public class Message:IEquatable<Message>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Message(MessageLevel level, string userContent, string advancedContent = null, string source = null, DateTime? date = null)
        {
            Level = level;
            UserContent = userContent;
            AdvancedContent = advancedContent;
            Source = source;
            if (!date.HasValue)
                Date = DateTime.Now;
            Error = ErrorID.Undefined;
        }
        public Message(ErrorID error, MessageLevel level, string userContent, string advancedContent = null, string source = null, DateTime? date = null)
            : this(level, userContent, advancedContent, source, date)
        {
            Error = error;
        }

        /// <summary>
        /// Message level
        /// </summary>
        [DataMember]
        public MessageLevel Level { get; set; }

        [DataMember]
        public ErrorID Error { get; set; }
        /// <summary>
        /// User content : To display in UI
        /// </summary>
        [DataMember]
        public string UserContent { get; set; }

        /// <summary>
        /// Advanced Content : For debug
        /// </summary>
        [DataMember]
        public string AdvancedContent { get; set; }

        /// <summary>
        /// Message source
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Message date
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        public bool Equals(Message other)
        {
            return Source==other.Source && Date==other.Date && Level==other.Level && Error==other.Error && UserContent==other.UserContent && AdvancedContent==other.AdvancedContent;
        }
    }
}
