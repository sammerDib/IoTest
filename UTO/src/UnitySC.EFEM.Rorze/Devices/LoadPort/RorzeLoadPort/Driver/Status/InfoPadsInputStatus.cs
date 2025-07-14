using System;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status
{
    public class InfoPadsInputStatus: Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoPadsInputStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="InfoPadsInputStatus"/> instance</param>
        /// </summary>
        public InfoPadsInputStatus(InfoPadsInputStatus other)
        {
            Set(other);
        }

        public InfoPadsInputStatus(string messageStatusData)
        {
            var status = messageStatusData.Replace(":", string.Empty);

            if (status.Length != 4)
                throw new InvalidOperationException("Expected input string must have exactly 4 characters among these: '-', 'A-D'. "
                                                    + $"Input={status}");

            InfoPadAPresence = status[0] == 'A';
            InfoPadBPresence = status[1] == 'B';
            InfoPadCPresence = status[2] == 'C';
            InfoPadDPresence = status[3] == 'D';
        }

        #endregion Constructors

        #region Properties

        public bool InfoPadAPresence { get; internal set; }

        public bool InfoPadBPresence { get; internal set; }

        public bool InfoPadCPresence { get; internal set; }

        public bool InfoPadDPresence { get; internal set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(InfoPadsInputStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                    return;

                InfoPadAPresence = other.InfoPadAPresence;
                InfoPadBPresence = other.InfoPadBPresence;
                InfoPadCPresence = other.InfoPadCPresence;
                InfoPadDPresence = other.InfoPadDPresence;
            }
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new InfoPadsInputStatus(this);
        }

        #endregion Status Override
    }
}
