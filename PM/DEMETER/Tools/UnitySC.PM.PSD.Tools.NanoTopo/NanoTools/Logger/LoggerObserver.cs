using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger
{
    /**
    *****************************************************************************************
    * 
    * \class LoggerObserver
    * \brief Interface implémentée tout objet amené à observer des logs
    * 
    *****************************************************************************************/
    public interface LoggerObserver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Source"></param>
        /// <param name="EventType"></param>
        /// <param name="Entry"></param>
        /// <returns></returns>
        void WriteLogEntry(DateTime Time,int Source, TypeOfEvent EventType, String Entry);
    }//LoggerObserver
}
  

