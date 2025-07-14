using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common2017.PostProcess
{
    public class PostProcessSatus : IEquatable<PostProcessSatus>
    {
        private enumPPStatus m_Status = enumPPStatus.ppsNotStarted;
        private String m_Message = String.Empty;

        public PostProcessSatus(enumPPStatus status, String message)
        {
            m_Status = status;
            m_Message = message;
        }

        public enumPPStatus Status { get => m_Status; }
        public string Message { get => m_Message; }

        public override bool Equals(object other)
        {
            return (other is PostProcessSatus otherPostProcesSatus) ? Equals(otherPostProcesSatus) : false;
        }

        public bool Equals(PostProcessSatus otherPostProcesSatus)
        {
            if (otherPostProcesSatus is null) return false;

            bool hasSameStatus = Status == otherPostProcesSatus.Status;
            bool hasSameMessage = Message == otherPostProcesSatus.Message;


            return hasSameStatus && hasSameMessage;
        }
        public static bool operator ==(PostProcessSatus lstatus, PostProcessSatus rstatus)
        {
            if (lstatus is null && rstatus is null)
            {
                return true;
            }

            if (lstatus is null || rstatus is null)
            {
                return false;
            }

            return lstatus.Equals(rstatus);
        }

        public static bool operator !=(PostProcessSatus lstatus, PostProcessSatus rstatus)
        {
            return !(lstatus == rstatus);
        }
    }
}
