using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger
{
    public interface LoggerObservable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anObserver"></param>
        /// <returns></returns>
        void RegisterLogger(LoggerObserver anObserver);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anObserver"></param>
        /// <returns></returns>
        void UnRegisterLogger(LoggerObserver anObserver);
    }
}
