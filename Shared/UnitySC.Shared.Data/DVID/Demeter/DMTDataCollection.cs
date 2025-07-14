using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.Data.DVID
{
    public class DMTDataCollection : ModuleDataCollection
    {
        private ILogger _appLogger;
        private LocalLogger _resultsLogger;

        [DataMember]
        public CWaferReport Report { get; set; }

        public DMTDataCollection(CWaferReport report, LocalLogger logger)
        {
            _appLogger = ClassLocator.Default.GetInstance<Shared.Logger.ILogger>();
            _resultsLogger = logger;
            
            Report = report;
            SlotID = report.iSlotID;
            LoadportID = report.iLoadPort;
            LotID = report.sLotID;
            ControlJobID = String.Empty;
            ProcessJobID = report.JobID;
            ProcessStartTime = ConvertStringToDateTime(report.sProcessStartTime);
            ProcessEndTime = ConvertStringToDateTime(report.sProcessStartTime);
            CarrierID = "";
            SubstrateID = report.sWaferID;
            AcquiredID = "";
            RecipeID = "";
        }       
        private DateTime ConvertStringToDateTime(string processStartTime)
        {
            // Eliminate unnecessary spaces (especially in the middle)
            processStartTime = Regex.Replace(processStartTime, @"\s+", " ").Trim();

            //  List of accepted formats
            string[] formats = { "MM-dd-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss" };

            foreach (string format in formats)
            {
                if (DateTime.TryParseExact(
                        processStartTime,
                        format,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime result))
                {
                    return result;
                }
            }

            // If no format matches, an exception is thrown
            throw new InvalidOperationException($"Failed to parse date: '{processStartTime}'. Expected formats: MM-dd-yyyy HH:mm:ss or dd-MM-yyyy HH:mm:ss.");
        }

    }        
}
