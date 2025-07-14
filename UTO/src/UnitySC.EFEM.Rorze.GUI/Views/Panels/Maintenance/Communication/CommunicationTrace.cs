using System;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Maintenance.Communication
{
    public class CommunicationTrace : IEquatable<CommunicationTrace>
    {
        public enum MessageDirection
        {
            Received,
            Sent
        }

        #region Constructor

        public CommunicationTrace(DateTime date, string correspondent, MessageDirection direction, string content)
        {
            Date = date;
            Correspondent = correspondent ?? throw new ArgumentNullException(nameof(correspondent));
            Direction = direction;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        #endregion Constructor

        #region Properties

        public DateTime Date { get; }

        public string Correspondent { get; }

        public MessageDirection Direction { get; }

        public string Content { get; }

        #endregion Properties

        #region IEquatable

        public bool Equals(CommunicationTrace other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Date.Equals(other.Date)
                   && Correspondent == other.Correspondent
                   && Direction == other.Direction
                   && Content == other.Content;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CommunicationTrace)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Correspondent.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Direction;
                hashCode = (hashCode * 397) ^ Content.GetHashCode();
                return hashCode;
            }
        }

        #endregion IEquatable
    }
}
