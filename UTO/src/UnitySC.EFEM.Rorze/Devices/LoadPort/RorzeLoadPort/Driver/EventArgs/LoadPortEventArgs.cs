using System.Text;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.EventArgs
{
    /// <summary>
    /// Base class for LoadPort event args.
    /// </summary>
    public class LoadPortEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPortEventArgs"/> class.
        /// </summary>
        public LoadPortEventArgs(byte loadPortNumber)
        {
            LoadPortNumber = loadPortNumber;
        }

        /// <summary>
        /// Port Number
        /// </summary>
        public byte LoadPortNumber { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder build = new StringBuilder();
            build.AppendLine($"Port = {LoadPortNumber}");
            return build.ToString();
        }
    }
}
