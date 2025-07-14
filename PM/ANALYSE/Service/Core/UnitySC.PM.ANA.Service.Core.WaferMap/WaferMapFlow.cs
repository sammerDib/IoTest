using System;
using System.Globalization;
using System.IO;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Tools;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.WaferMap
{
    public class WaferMapFlow : FlowComponent<WaferMapInput, WaferMapResult, WaferMapConfiguration>
    {

        private struct DieGridData
        {
            public DieGridData(bool presence, XYPosition topLeftDieCorner, XYPosition topRightDieCorner, XYPosition bottomLeftDieCorner, XYPosition bottomRightDieCorner)
            {
                DiePresence = presence;
                TopLeftDieCornerPosition = topLeftDieCorner;
                TopRightDieCornerPosition = topRightDieCorner;
                BottomLeftDieCornerPosition = bottomLeftDieCorner;
                BottomRightDieCornerPosition = bottomRightDieCorner;
            }

            public bool DiePresence;
            public XYPosition TopLeftDieCornerPosition;
            public XYPosition TopRightDieCornerPosition;
            public XYPosition BottomLeftDieCornerPosition;
            public XYPosition BottomRightDieCornerPosition;
        }

        public WaferMapFlow(WaferMapInput input) : base(input, "WaferMapFlow")
        {
        }

        protected override void Process()
        {
            var wafer = WaferUtils.CreateWaferShapeFromWaferCharacteristics(Input.WaferCharacteristics, Input.EdgeExclusion);
            CornersAreInsideUsableWafer(wafer, Input.TopLeftCorner.Position.ToXYPosition(), Input.BottomRightCorner.Position.ToXYPosition());

            Result.DieDimensions = Input.DieDimensions;
            Result.RotationAngle = Input.DieDimensions.DieAngle;
            FillResultDieGridInformation(wafer);
        }

        private static void CornersAreInsideUsableWafer(IWaferShape wafer, XYPosition topLeftCorner, XYPosition bottomRightCorner)
        {
            bool knowTopLeftCornerIsInsideUsableWafer = wafer.IsInside(new PointUnits(topLeftCorner.X.Millimeters(), topLeftCorner.Y.Millimeters()));
            bool knowBottomRightCornerIsInsideUsableWafer = wafer.IsInside(new PointUnits(bottomRightCorner.X.Millimeters(), bottomRightCorner.Y.Millimeters()));

            if (!knowTopLeftCornerIsInsideUsableWafer || !knowBottomRightCornerIsInsideUsableWafer)
            {
                throw new Exception("The top left and bottom right die corner provided must not be inside the edge exclusion.");
            }
        }

        private void FillResultDieGridInformation(IWaferShape wafer)
        {
            var pitchWidth = Input.DieDimensions.DieWidth + Input.DieDimensions.StreetWidth;
            var pitchHeight = Input.DieDimensions.DieHeight + Input.DieDimensions.StreetHeight;
            var angle = Input.DieDimensions.DieAngle;

            // We cancel the die angle of the top left corner to compute the maximum dies on top/left as if it was 0°.
            // (It's equivalent to projecting the top left point coordinates to XY-axes rotated of "angle")
            var topLeftCornerWithoutRotation = (XYZTopZBottomPosition)Input.TopLeftCorner.Position.Clone();
            MathTools.ApplyAntiClockwiseRotation(-angle, topLeftCornerWithoutRotation, new XYPosition(topLeftCornerWithoutRotation.Referential, 0, 0));
            var distanceAtLeftWaferEdge = Input.WaferCharacteristics.Diameter.Millimeters / 2 + topLeftCornerWithoutRotation.X;
            var distanceAtTopWaferEdge = Input.WaferCharacteristics.Diameter.Millimeters / 2 - topLeftCornerWithoutRotation.Y;
            var maxDiesAtLeft = Math.Ceiling(distanceAtLeftWaferEdge / pitchWidth.Millimeters);
            var maxDiesAbove = Math.Ceiling(distanceAtTopWaferEdge / pitchHeight.Millimeters);

            var distanceAtRightWaferEdge = Input.WaferCharacteristics.Diameter.Millimeters / 2 - topLeftCornerWithoutRotation.X;
            var distanceAtBottomWaferEdge = Input.WaferCharacteristics.Diameter.Millimeters / 2 + topLeftCornerWithoutRotation.Y;
            var maxDiesAtRightWithCurrent = Math.Ceiling(distanceAtRightWaferEdge / pitchWidth.Millimeters);
            var maxDiesNbBelowWithCurrent = Math.Ceiling(distanceAtBottomWaferEdge / pitchHeight.Millimeters);

            var dieGridTopLeft = new XYPosition(
                new WaferReferential(),
                Input.TopLeftCorner.Position.X - maxDiesAtLeft * pitchWidth.Millimeters,
                Input.TopLeftCorner.Position.Y + maxDiesAbove * pitchHeight.Millimeters);
            // We rotate back the die grid top left to get its actual position with the die angle taken into account
            MathTools.ApplyAntiClockwiseRotation(angle, dieGridTopLeft, Input.TopLeftCorner.Position.ToXYPosition());

            var maxDiesNumberOnXAxis = (int)(maxDiesAtLeft + maxDiesAtRightWithCurrent);
            var maxDiesNumberOnYAxis = (int)(maxDiesAbove + maxDiesNbBelowWithCurrent);

            var diesPresence = new Matrix<bool>(maxDiesNumberOnYAxis, maxDiesNumberOnXAxis);
            var dieGridData = new Matrix<DieGridData>(maxDiesNumberOnYAxis, maxDiesNumberOnXAxis);

            for (int col = 0; col < maxDiesNumberOnXAxis; col++)
            {
                for (int row = 0; row < maxDiesNumberOnYAxis; row++)
                {
                    var topLeftDieCorner = new XYPosition(
                        new WaferReferential(),
                        dieGridTopLeft.X + col * pitchWidth.Millimeters,
                        dieGridTopLeft.Y - row * pitchHeight.Millimeters);

                    var bottomLeftDieCorner = new XYPosition(
                        new WaferReferential(),
                        topLeftDieCorner.X,
                        topLeftDieCorner.Y - pitchHeight.Millimeters);

                    var topRightDieCorner = new XYPosition(
                        new WaferReferential(),
                        topLeftDieCorner.X + pitchWidth.Millimeters,
                        topLeftDieCorner.Y);

                    var bottomRightDieCorner = new XYPosition(
                        new WaferReferential(),
                        topRightDieCorner.X,
                        topRightDieCorner.Y - pitchHeight.Millimeters);

                    MathTools.ApplyAntiClockwiseRotation(angle, topLeftDieCorner, dieGridTopLeft);
                    MathTools.ApplyAntiClockwiseRotation(angle, bottomLeftDieCorner, dieGridTopLeft);
                    MathTools.ApplyAntiClockwiseRotation(angle, topRightDieCorner, dieGridTopLeft);
                    MathTools.ApplyAntiClockwiseRotation(angle, bottomRightDieCorner, dieGridTopLeft);

                    bool dieIsInsideUsableWafer = wafer.IsInside(new PointUnits(topLeftDieCorner.X.Millimeters(), topLeftDieCorner.Y.Millimeters())) &&
                                                  wafer.IsInside(new PointUnits(topRightDieCorner.X.Millimeters(), topRightDieCorner.Y.Millimeters())) &&
                                                  wafer.IsInside(new PointUnits(bottomLeftDieCorner.X.Millimeters(), bottomLeftDieCorner.Y.Millimeters())) &&
                                                  wafer.IsInside(new PointUnits(bottomRightDieCorner.X.Millimeters(), bottomRightDieCorner.Y.Millimeters()));

                    diesPresence.SetValue(row, col, dieIsInsideUsableWafer);
                    dieGridData.SetValue(row, col, new DieGridData(presence: dieIsInsideUsableWafer, topLeftDieCorner: topLeftDieCorner, topRightDieCorner: topRightDieCorner, bottomLeftDieCorner: bottomLeftDieCorner, bottomRightDieCorner: bottomRightDieCorner));
                }
            }

            if (Configuration.IsAnyReportEnabled())
            {
                WriteDiesPresenceInCSVFormat(dieGridData, Path.Combine(ReportFolder, $"diesPresenceArray.csv"));
            }

            Logger.Debug($"{LogHeader} Die grid columns : {diesPresence.Columns}.");
            Logger.Debug($"{LogHeader} Die grid rows : {diesPresence.Rows}.");
            Logger.Debug($"{LogHeader} Top left corner position : (x: {dieGridTopLeft.X} ; y: {dieGridTopLeft.X}) ");

            Result.DiesPresence = diesPresence;
            Result.DieGridTopLeft = dieGridTopLeft;
        }

        private void WriteDiesPresenceInCSVFormat(Matrix<DieGridData> dieGridData, string filepath)
        {
            try
            {
                var sbCSV = new CSVStringBuilder();
                sbCSV.AppendLine(
                    $"Row",
                    $"Column",
                    $"Die presence",
                    $"Top left corner position",
                    $"Top right corner position", 
                    $"Bottom left corner position",
                    $"Bottom right corner position");

                for (int r = 0; r < dieGridData.Rows; r++)
                {
                    for (int c = 0; c < dieGridData.Columns; c++)
                    {
                        var currentData = dieGridData.GetValue(r, c);
                        sbCSV.AppendLine(
                            $"{r}",
                            $"{c}",
                            $"{currentData.DiePresence}",
                            $"(x: {Math.Round(currentData.TopLeftDieCornerPosition.X, 3).ToString(CultureInfo.InvariantCulture)} | y: {Math.Round(currentData.TopLeftDieCornerPosition.Y, 3).ToString(CultureInfo.InvariantCulture)})",
                            $"(x: {Math.Round(currentData.TopRightDieCornerPosition.X, 3).ToString(CultureInfo.InvariantCulture)} | y: {Math.Round(currentData.TopRightDieCornerPosition.Y, 3).ToString(CultureInfo.InvariantCulture)})",
                            $"(x: {Math.Round(currentData.BottomLeftDieCornerPosition.X, 3).ToString(CultureInfo.InvariantCulture)} | y: {Math.Round(currentData.BottomLeftDieCornerPosition.Y, 3).ToString(CultureInfo.InvariantCulture)})",
                            $"(x: {Math.Round(currentData.BottomRightDieCornerPosition.X, 3).ToString(CultureInfo.InvariantCulture)} | y: {Math.Round(currentData.BottomRightDieCornerPosition.Y, 3).ToString(CultureInfo.InvariantCulture)})");
                     }
                }
                File.WriteAllText(filepath, sbCSV.ToString()); ;
            }
            catch (Exception e)
            {
                Logger.Error($"{LogHeader} Reporting failed : {e.Message}");
            }
        }
    }
}
