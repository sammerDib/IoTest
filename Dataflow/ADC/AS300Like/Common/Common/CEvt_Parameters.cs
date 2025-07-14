    using Common;
using System;
using System.Collections.Generic;

namespace Common
{
    public class CEvt_Parameters
    {
        public OnDisplayChange Evt;
        public List<Object> List;
        public String m_CreatedFrom;
		public int ID;
		public CEvt_Parameters(OnDisplayChange pEvt, List<Object> pList, int pID)
        {
            Evt = pEvt;
            List = pList;;
			ID = pID;
		}


		public String CreatedFrom
		{
			set
			{
				m_CreatedFrom = value + " #" + ID.ToString();

			}
			get { return m_CreatedFrom; }
		}
	}
}
