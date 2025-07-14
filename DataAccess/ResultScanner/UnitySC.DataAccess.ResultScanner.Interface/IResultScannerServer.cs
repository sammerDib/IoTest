using System;

namespace UnitySC.DataAccess.ResultScanner.Interface
{
    public interface IResultScannerServer
    {
        //
        // event CallBack for ResultService
        //
        event StateChangeEventHandler StateChanged;

        event StatisticsChangeEventHandler StatisticsChanged;
    }
}
