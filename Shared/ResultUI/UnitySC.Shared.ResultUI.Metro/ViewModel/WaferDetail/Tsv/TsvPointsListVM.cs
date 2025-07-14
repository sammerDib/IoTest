using System;
using System.Collections.Generic;

using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.ResultUI.Metro.PointSelector;
using UnitySC.Shared.ResultUI.Metro.Utilities;
using UnitySC.Shared.UI.ViewModel.AdvancedGridView;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv
{
    public class TsvPointsListVM : MetroPointsListVM
    {
        public List<GeneratedColumn> GeneratedColumns { get; } = new List<GeneratedColumn>();

        private bool _generateColumnsFlag;

        public bool GenerateColumnsFlag
        {
            get => _generateColumnsFlag;
            set => SetProperty(ref _generateColumnsFlag, value);
        }
        
        #region Properties
        
        public Func<MeasurePointResult, Length> MeasurePointResultToCopla { get; }
        
        #endregion

        #region Sort Definitions
        
        public SortDefinition SortByLength { get; }
        public SortDefinition SortByWidth { get; }
        public SortDefinition SortByDepth { get; }
        
        public SortDefinition SortByMinLength { get; }
        public SortDefinition SortByMaxLength { get; }
        public SortDefinition SortBy3SigmaLength { get; }

        public SortDefinition SortByMinWidth { get; }
        public SortDefinition SortByMaxWidth { get; }
        public SortDefinition SortBy3SigmaWidth { get; }

        public SortDefinition SortByMinDepth { get; }
        public SortDefinition SortByMaxDepth { get; }
        public SortDefinition SortBy3SigmaDepth { get; }

        public SortDefinition SortByCopla { get; }

        #endregion

        public TsvPointsListVM(PointSelectorBase pointSelector) : base(pointSelector)
        {
            MeasurePointResultToCopla = result =>
            {
                if (result is TSVPointResult tsvResult)
                {
                    return tsvResult.CoplaInWaferValue == null ? tsvResult.CoplaInDieValue : tsvResult.CoplaInWaferValue;
                }

                return null;
            };

            SortByLength = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.LengthTsvStat.Mean?.Micrometers : double.MinValue);
            SortByMinLength = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.LengthTsvStat.Min?.Micrometers : double.MinValue);
            SortByMaxLength = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.LengthTsvStat.Max?.Micrometers : double.MinValue);
            SortBy3SigmaLength = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.LengthTsvStat.Sigma3?.Micrometers : double.MinValue);

            SortByWidth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.WidthTsvStat.Mean?.Micrometers : double.MinValue);
            SortByMinWidth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.WidthTsvStat.Min?.Micrometers : double.MinValue);
            SortByMaxWidth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.WidthTsvStat.Max?.Micrometers : double.MinValue);
            SortBy3SigmaWidth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.WidthTsvStat.Sigma3?.Micrometers : double.MinValue);

            SortByDepth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.DepthTsvStat.Mean?.Micrometers : double.MinValue);
            SortByMinDepth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.DepthTsvStat.Min?.Micrometers : double.MinValue);
            SortByMaxDepth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.DepthTsvStat.Max?.Micrometers : double.MinValue);
            SortBy3SigmaDepth = SortedPoints.Sort.AddSortDefinition(result => result is TSVPointResult tsvResult ? tsvResult.DepthTsvStat.Sigma3?.Micrometers : double.MinValue);

            SortByCopla = SortedPoints.Sort.AddSortDefinition(result =>
            {
                if (result is TSVPointResult tsvResult)
                {
                    if (tsvResult.CoplaInWaferValue != null)
                    {
                        return tsvResult.CoplaInWaferValue.Micrometers;
                    }

                    if (tsvResult.CoplaInDieValue != null)
                    {
                        return tsvResult.CoplaInDieValue.Micrometers;
                    }
                }

                return double.MinValue;
            });
        }

        #region Overrides of MetroPointsListVM

        public override void UpdatePointsSource(ICollection<MeasurePointResult> sourcePoint, bool showRepetaColumns, bool showDieIndex, bool showQualityScore, bool showSiteID)
        {
            base.UpdatePointsSource(sourcePoint, showRepetaColumns, showDieIndex, showQualityScore, showSiteID);

            // Notify view that generated columns changed
            GenerateColumnsFlag = !GenerateColumnsFlag;
        }

        #endregion
    }
}
