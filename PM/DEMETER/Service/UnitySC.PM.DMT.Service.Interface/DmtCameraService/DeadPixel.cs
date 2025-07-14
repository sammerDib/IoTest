using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySC.PM.DMT.Service.Interface
{
    [Flags]
    public enum DeadPixelTypes { WhitePixel = 1, BlackPixel = 2 };

    /// <summary>
    /// Represents a dead pixel on a camera
    /// </summary>
    public class DeadPixel
    {
        private int m_iX;
        /// <summary>
        /// the X coordinate of the dead pixel
        /// </summary>
        public int X
        {
            get { return m_iX; }
            set { m_iX = value; }
        }
        private int m_iY;
        /// <summary>
        /// the Y coordinate of the dead pixel
        /// </summary>
        public int Y
        {
            get { return m_iY; }
            set { m_iY = value; }
        }

        private DeadPixelTypes m_type;
        /// <summary>
        /// The dead pixel type
        /// </summary>
        public DeadPixelTypes Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        /// <summary>
        /// returns a summary of the dead pixels with its coordinates
        /// </summary>
        /// <returns>A string with the dead pixel's coordinates</returns>
        public override string ToString()
        {
            return "X : " + X + " Y : " + Y;
        }
    }
}
