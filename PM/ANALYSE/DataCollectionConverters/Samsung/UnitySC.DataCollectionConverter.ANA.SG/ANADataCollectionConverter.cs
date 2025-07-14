using System.Collections.Generic;

using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.DataCollectionConverter;

namespace UnitySC.DataCollectionConverter.ANA.SG
{
    public class ANADataCollectionConverter : IDataCollectionConverter
    {
        public SecsVariableList ConvertToSecsVariableList(ModuleDataCollection moduleDataCollection)
        {
            if (moduleDataCollection is ANADataCollection anaDataCollection)
                return ConvertANADataCollectionToSecsVariableList(anaDataCollection);
            return null;
        }

        private SecsVariableList ConvertANADataCollectionToSecsVariableList(ANADataCollection anaDataCollection)
        {
            var dataVariables = ConvertModuleDataCollectionToSecsVariableList(anaDataCollection);
            dataVariables.Add(ConvertWaferStatisticsToSecsVariable(anaDataCollection.WaferStatistics));
            dataVariables.Add(ConvertDiesStatisticsToSecsVariable(anaDataCollection.AllDiesStatistics));
            dataVariables.Add(ConvertWaferMeasureDataToSecsVariable(anaDataCollection.WaferMeasureData));

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

        private SecsVariable ConvertWaferStatisticsToSecsVariable(DVIDWaferStatistics waferStatistics)
        {
            var body = new List<SecsItem>();
            body.Add(new SecsItem(SecsFormat.Ascii, waferStatistics.Name));
            var waferStatisticsItemList = new List<SecsItem>();

            foreach (var waferStatistic in waferStatistics.WaferStatistics)
            {
                var measureItemList = new List<SecsItem>();
                measureItemList.Add(new SecsItem(SecsFormat.Ascii, waferStatistic.MeasureName));
                var waferStatisticsForMeasure = new List<SecsItem>();
                foreach (var data in waferStatistic.WaferStatisticsForMeasure)
                {
                    var statisticList = new List<SecsItem>();
                    statisticList.Add(new SecsItem(SecsFormat.Ascii, data.Name));
                    statisticList.Add(new SecsItem(SecsFormat.Ascii, data.Unit));

                    switch (data)
                    {
                        case DCDataInt statisticInt:
                            statisticList.Add(new SecsItem(SecsFormat.Int4, statisticInt.Value));
                            break;

                        case DCDataDouble statisticDouble:
                            statisticList.Add(new SecsItem(SecsFormat.Float8, statisticDouble.Value));
                            break;
                    }
                    waferStatisticsForMeasure.Add(new SecsItem(SecsFormat.List, new SecsItemList(statisticList)));
                }
                measureItemList.Add(new SecsItem(SecsFormat.List, new SecsItemList(waferStatisticsForMeasure)));
                waferStatisticsItemList.Add(new SecsItem(SecsFormat.List, new SecsItemList(measureItemList)));
            }

            body.Add(new SecsItem(SecsFormat.List, new SecsItemList(waferStatisticsItemList)));
            var secsItem = new SecsItem(SecsFormat.List, new SecsItemList(body));

            return new SecsVariable("PW_GlobalWaferStatistics", secsItem);
        }

        private SecsVariable ConvertDiesStatisticsToSecsVariable(DVIDAllDiesStatistics allDiesStatistics)
        {
            var body = new List<SecsItem>();
            body.Add(new SecsItem(SecsFormat.Ascii, allDiesStatistics.Name));
            var measuresCountList = new List<SecsItem>();

            foreach (var diesStatistic in allDiesStatistics.DiesStatistics)
            {
                var measureList = new List<SecsItem>();
                measureList.Add(new SecsItem(SecsFormat.Ascii, diesStatistic.MeasureName));
                var diesCountList = new List<SecsItem>();
                if (diesStatistic.DiesStatisticsForMeasure != null)
                {
                    foreach (var die in diesStatistic.DiesStatisticsForMeasure)
                    {
                        var dieList = new List<SecsItem>();
                        dieList.Add(new SecsItem(SecsFormat.Int4, die.ColumnIndex));
                        dieList.Add(new SecsItem(SecsFormat.Int4, die.RowIndex));
                        var statisticCountList = new List<SecsItem>();
                        foreach (var statistic in die
                                     .DieStatistics)
                        {
                            var statisticList = new List<SecsItem>();
                            statisticList.Add(new SecsItem(SecsFormat.Boolean, statistic.IsMeasured));
                            statisticList.Add(new SecsItem(SecsFormat.Ascii, statistic.Name));
                            statisticList.Add(new SecsItem(SecsFormat.Ascii, statistic.Unit));
                            switch (statistic)
                            {
                                case DCDataInt statisticInt:
                                    statisticList.Add(new SecsItem(SecsFormat.Int4, statisticInt.Value));
                                    break;

                                case DCDataDouble statisticDouble:
                                    statisticList.Add(new SecsItem(SecsFormat.Float8, statisticDouble.Value));
                                    break;
                            }

                            statisticCountList.Add(new SecsItem(SecsFormat.List, new SecsItemList(statisticList)));
                        }
                        dieList.Add(new SecsItem(SecsFormat.List, new SecsItemList(statisticCountList)));
                        diesCountList.Add(new SecsItem(SecsFormat.List, new SecsItemList(dieList)));
                    }
                }

                measureList.Add(new SecsItem(SecsFormat.List, new SecsItemList(diesCountList)));
                measuresCountList.Add(new SecsItem(SecsFormat.List, new SecsItemList(measureList)));
            }
            body.Add(new SecsItem(SecsFormat.List, new SecsItemList(measuresCountList)));

            var secsItem = new SecsItem(SecsFormat.List, new SecsItemList(body));
            return new SecsVariable("PW_DiesStatistics", secsItem);
        }

        private SecsVariable ConvertWaferMeasureDataToSecsVariable(DVIDWaferMeasureData pointsMeasureData)
        {
            var body = new List<SecsItem>();
            body.Add(new SecsItem(SecsFormat.Ascii, pointsMeasureData.Name));
            var measureDataList = new List<SecsItem>();

            foreach (var waferMeasureData in pointsMeasureData.WaferMeasuresData)
            {
                var measureList = new List<SecsItem>();
                measureList.Add(new SecsItem(SecsFormat.Ascii, waferMeasureData.MeasureName));
                var pointsCountList = new List<SecsItem>();
                foreach (var measureData in waferMeasureData.WaferMeasuresDataForMeasure)
                {
                    var pointList = new List<SecsItem>();
                    pointList.Add(new SecsItem(SecsFormat.Float8, measureData.CoordinateX));
                    pointList.Add(new SecsItem(SecsFormat.Float8, measureData.CoordinateY));
                    pointList.Add(new SecsItem(SecsFormat.Int4, measureData.DieColumnIndex));
                    pointList.Add(new SecsItem(SecsFormat.Int4, measureData.DieRowIndex));
                    pointList.Add(new SecsItem(SecsFormat.Int4, measureData.SiteId));
                    var pointMeasuresCountList = new List<SecsItem>();
                    foreach (var pointMeasureData in measureData.PointMeasuresData)
                    {
                        var pointMeasureList = new List<SecsItem>();
                        pointMeasureList.Add(new SecsItem(SecsFormat.Boolean, pointMeasureData.IsMeasured));
                        pointMeasureList.Add(new SecsItem(SecsFormat.Ascii, pointMeasureData.Name));
                        pointMeasureList.Add(new SecsItem(SecsFormat.Ascii, pointMeasureData.Unit));
                        switch (pointMeasureData)
                        {

                            case DCDataIntWithDescription
                                pointMeasureDataIntDescript:
                                pointMeasureList.Add(new SecsItem(SecsFormat.Int4, pointMeasureDataIntDescript.Value));
                                pointMeasureList.Add(new SecsItem(SecsFormat.Ascii, pointMeasureDataIntDescript.Description ?? string.Empty));
                                break;

                            case DCDataDoubleWithDescription
                                pointMeasureDataDoubleDescript:
                                pointMeasureList.Add(new SecsItem(SecsFormat.Float8, pointMeasureDataDoubleDescript.Value));
                                pointMeasureList.Add(new SecsItem(SecsFormat.Ascii, pointMeasureDataDoubleDescript.Description ?? string.Empty));
                                break;

                            case DCDataInt
                                pointMeasureDataInt:
                                pointMeasureList.Add(new SecsItem(SecsFormat.Int4, pointMeasureDataInt.Value));
                                break;

                            case DCDataDouble
                                pointMeasureDataDouble:
                                pointMeasureList.Add(new SecsItem(SecsFormat.Float8, pointMeasureDataDouble.Value));
                                break;
                        }

                        pointMeasuresCountList.Add(new SecsItem(SecsFormat.List, new SecsItemList(pointMeasureList)));
                    }
                    pointList.Add(new SecsItem(SecsFormat.List, new SecsItemList(pointMeasuresCountList)));
                    pointsCountList.Add(new SecsItem(SecsFormat.List, new SecsItemList(pointList)));
                }
                measureList.Add(new SecsItem(SecsFormat.List, new SecsItemList(pointsCountList)));
                measureDataList.Add(new SecsItem(SecsFormat.List, new SecsItemList(measureList)));
            }

            body.Add(new SecsItem(SecsFormat.List, new SecsItemList(measureDataList)));

            var secsItem = new SecsItem(SecsFormat.List, new SecsItemList(body));
            return new SecsVariable("PW_PointMeasure", secsItem);
        }
    }
}
