using System;
using System.Collections.Generic;
using System.Text;

using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.DataCollectionConverter;
using UnitySC.Shared.Logger;

namespace UnitySC.Shared.DMT.DataCollection
{
    public class DMTDataCollectionConverter : IDataCollectionConverter
    {
        private ILogger _appLogger;
        private LocalLogger _resultsLogger;
        private StringBuilder _logResult;

        public SecsVariableList ConvertToSecsVariableList(ModuleDataCollection moduleDataCollection)
        {
            if (moduleDataCollection is DMTDataCollection dmtDataCollection)
                return ConvertDMTDataCollectionToSecsVariableList(dmtDataCollection);
            return null;
        }

    
        private SecsVariableList ConvertDMTDataCollectionToSecsVariableList(DMTDataCollection dmtDataCollection)
        {

            _appLogger.Information("------------------------------------------------");
            _appLogger.Information("Convert ADC results into datacollection START");
            _appLogger.Information($"Wafer results from : {dmtDataCollection.SubstrateID}");
            _appLogger.Information($"PJob:{dmtDataCollection.ProcessJobID} Lotid:{dmtDataCollection.LotID}  LP:{dmtDataCollection.LoadportID} slot:{dmtDataCollection.SlotID} recipe:{dmtDataCollection.RecipeID}");


            // Add wafer parameters
            var dataVariables = ConvertModuleDataCollectionToSecsVariableList(dmtDataCollection);

            String waferParameters = "\r\n\r\n";
            waferParameters = waferParameters + $"SlotID = {dmtDataCollection.SlotID}\r\n";
            waferParameters = waferParameters + $"LoadportID = {dmtDataCollection.LoadportID}\r\n";
            waferParameters = waferParameters + $"LotID = {dmtDataCollection.SlotID}\r\n";
            waferParameters = waferParameters + $"ControlJobID = \r\n";
            waferParameters = waferParameters + $"ProcessJobID = {dmtDataCollection.ProcessJobID}\r\n";
            waferParameters = waferParameters + $"ProcessStartTime = {dmtDataCollection.ProcessStartTime.ToString("MM-dd-yyyy HH:mm:ss")}\r\n";
            waferParameters = waferParameters + $"ProcessEndTime = {dmtDataCollection.ProcessStartTime.ToString("MM-dd-yyyy HH:mm:ss")}\r\n";
            waferParameters = waferParameters + $"CarrierID = \r\n";
            waferParameters = waferParameters + $"SubstrateID = {dmtDataCollection.SubstrateID}\r\n";
            waferParameters = waferParameters + $"AcquiredID = \r\n";
            waferParameters = waferParameters + $"RecipeID = \r\n";
            _resultsLogger.Information(waferParameters);

            // Add Defects
            try
            {
                _logResult = new StringBuilder();
                List<SecsVariable> defectSVs = ConversionDefects.GetConvertSVsFromDefects(dmtDataCollection.Report.DefectList, ref _logResult);
                _resultsLogger.Information(_logResult.ToString());
                dataVariables.List.AddRange(defectSVs);                
            }
            catch (Exception ex)
            {
                if (ex.Message == ConversionDefects.NoVIDProcessDefect_Msg)
                    _appLogger.Information("No defects in Report");
                else
                    _appLogger.Error($"Convert defects into data collection failed - Exception: {ex.Message + ex.StackTrace}");

                _resultsLogger.Information(_logResult.ToString());
            }
            // Add Measurements
            try
            {
                _logResult = new StringBuilder();
                List<SecsVariable> measurementSVs = ConversionMeasurements.GetConvertSVFromMeasurements(dmtDataCollection.Report.MeasurementList, ref _logResult);
                _resultsLogger.Information(_logResult.ToString());
                dataVariables.List.AddRange(measurementSVs);
            }
            catch (Exception ex)
            {
                if (ex.Message == ConversionMeasurements.NoVIDProcessMeasurement_Msg)
                    _appLogger.Information("No measurements in Report");
                else
                    _appLogger.Error($"Convert measurements into data collection failed - Exception: {ex.Message + ex.StackTrace}");
            }
            // Add APC
            try
            {
                _logResult = new StringBuilder();
                List<SecsVariable> apcSVs = ConversionAPCs.GetConvertSVFromAPCs(dmtDataCollection.Report.APCList, ref _logResult);
                _resultsLogger.Information(_logResult.ToString());
                dataVariables.List.AddRange(apcSVs);
            }
            catch (Exception ex)
            {
                if (ex.Message == ConversionMeasurements.NoVIDProcessMeasurement_Msg)
                    _appLogger.Information("No APC in Report");
                else
                    _appLogger.Error($"Convert APC into data collection failed - Exception: {ex.Message + ex.StackTrace}");
            }

            _appLogger.Information("Convert ADC results into datacollection COMPLETE");
            _appLogger.Information("------------------------------------------------");
            return dataVariables;

        }


        private SecsVariableList ConvertModuleDataCollectionToSecsVariableList(ModuleDataCollection anaDataCollection)
        {
            var dataVariables = new SecsVariableList();
            dataVariables.Add(new SecsVariable("PW_LoadPortID", new SecsItem(SecsFormat.UInt4, (uint)anaDataCollection.LoadportID)));
            dataVariables.Add(new SecsVariable("PW_SlotID", new SecsItem(SecsFormat.UInt4, (uint)anaDataCollection.SlotID)));
            dataVariables.Add(new SecsVariable("PW_CarrierID", new SecsItem(SecsFormat.Ascii, anaDataCollection.CarrierID)));
            dataVariables.Add(new SecsVariable("PW_LotID", new SecsItem(SecsFormat.Ascii, anaDataCollection.LotID)));
            dataVariables.Add(new SecsVariable("PW_SubstrateID", new SecsItem(SecsFormat.Ascii, anaDataCollection.SubstrateID)));
            dataVariables.Add(new SecsVariable("PW_AcquiredID", new SecsItem(SecsFormat.Ascii, anaDataCollection.AcquiredID)));
            dataVariables.Add(new SecsVariable("PW_ControlJobID", new SecsItem(SecsFormat.Ascii, anaDataCollection.ControlJobID)));
            dataVariables.Add(new SecsVariable("PW_ProcessJobID", new SecsItem(SecsFormat.Ascii, anaDataCollection.ProcessJobID)));
            dataVariables.Add(new SecsVariable("PW_RecipeID", new SecsItem(SecsFormat.Ascii, anaDataCollection.RecipeID)));
            dataVariables.Add(new SecsVariable("PW_StartTime", new SecsItem(SecsFormat.Ascii, anaDataCollection.ProcessStartTime)));
            dataVariables.Add(new SecsVariable("PW_EndTime", new SecsItem(SecsFormat.Ascii, anaDataCollection.ProcessEndTime)));

            return dataVariables;
        }
   

    }
}
