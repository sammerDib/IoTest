using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class CColorManager
    {
        public static int COLOR_RED = 79;
        public static int COLOR_GREEN = 139;
        public static int COLOR_BLUE = 251;
    }
    public class CIOManagerMessage
    {
        private bool m_bFFUAlarm;
        private bool m_bVacuumOK;
        private bool m_bAirOK;
        private bool m_bDoorOpened;
		private bool m_bPM1DoorOpened;
		private bool m_bPM2DoorOpened;
		private bool m_bPM3DoorOpened;
		private bool m_bPM4DoorOpened;
		private bool m_bDoorOpen;
        private bool m_bDoorClose;
        private bool m_bInterlockState;
        private IOControllerStatus m_IOGeneralStatus;
        private bool m_bConnected;

        public CIOManagerMessage(IOControllerStatus pIOGeneralStatus, bool pbConnected, 
           bool bFFUAlarm,           
           bool bVacuumOK,
           bool bAirOK,
           bool bDoorOpened,
           bool bDoorOpen,
           bool bDoorClose,
           bool bInterlockState)
        {
            m_IOGeneralStatus = pIOGeneralStatus;
            m_bConnected = pbConnected;
            m_bFFUAlarm = bFFUAlarm;
            m_bVacuumOK = bVacuumOK;
            m_bAirOK = bAirOK;
            m_bDoorOpened = bDoorOpened;
            m_bDoorOpen = bDoorOpen;
            m_bDoorClose = bDoorClose;
            m_bInterlockState = bInterlockState;
        }

        public IOControllerStatus IOGeneralStatus
        {
            get { return m_IOGeneralStatus; }
            set { m_IOGeneralStatus = value; }
        }

        public bool bConnected
        {
            get { return m_bConnected; }
            set { m_bConnected = value; }
        }

        public bool bFFUAlarm
        {
            get { return m_bFFUAlarm; }
            set { m_bFFUAlarm = value; }
        }
        public bool bVacuumOK
        {
            get { return m_bVacuumOK; }
            set { m_bVacuumOK = value; }
        }
        public bool bAirOK
        {
            get { return m_bAirOK; }
            set { m_bAirOK = value; }
        }
        public bool bDoorOpened
        {
            get { return m_bDoorOpened; }
            set { m_bDoorOpened = value; }
		}
		public bool bPM1DoorOpened
		{
			get { return m_bPM1DoorOpened; }
			set { m_bPM1DoorOpened = value; }
		}
		public bool bPM2DoorOpened
		{
			get { return m_bPM2DoorOpened; }
			set { m_bPM2DoorOpened = value; }
		}
		public bool bPM3DoorOpened
		{
			get { return m_bPM3DoorOpened; }
			set { m_bPM3DoorOpened = value; }
		}
		public bool bPM4DoorOpened
		{
			get { return m_bPM4DoorOpened; }
			set { m_bPM4DoorOpened = value; }
		}
		public bool bDoorOpen
        {
            get { return m_bDoorOpen; }
            set { m_bDoorOpen = value; }
        }
        public bool bDoorClose
        {
            get { return m_bDoorClose; }
            set { m_bDoorClose = value; }
        }
        public bool bInterlockState
        {
            get { return m_bInterlockState; }
            set { m_bInterlockState = value; }
        }
    }
}
