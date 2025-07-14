using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Data.Enum;
using System.IO;
using System.Drawing;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {
        #region Generate TSV files
        private static void GenerateTSVSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            total += 3 + 10 * 3 + 40 * 3 + 20 * 2; // normal measures points

            Gen10ptsSite(out double[] xs_10pts, out double[] ys_10pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            Gen40ptsSite(out double[] xs_40pts, out double[] ys_40pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            Gen20ptsSite(out double[] xs_20pts, out double[] ys_20pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));


            int slot = 0;
            // success , partial, error, notmeasure
            var ToleranceSet = new TSVResultSettings()
            {
                DepthTarget = new Length(100.0, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(250.0, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(15.0, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(250.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(20.0, LengthToleranceUnit.Micrometer)
            };
            var AcqSimSet = new TSVResultSettings()
            {
                DepthTarget = new Length(105.0, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(2.0, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(252.0, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(1.0, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(248.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(0.5, LengthToleranceUnit.Micrometer)
            };
            double[] stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            // slot 0
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 1, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 1
            AcqSimSet.DepthTarget = new Length(80.0, LengthUnit.Micrometer);
            AcqSimSet.DepthTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Micrometer);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 3, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 2
            AcqSimSet.DepthTarget = new Length(100.0, LengthUnit.Micrometer);
            AcqSimSet.DepthTolerance = new LengthTolerance(5.0, LengthToleranceUnit.Micrometer);
            AcqSimSet.WidthTarget = new Length(240.0, LengthUnit.Micrometer);
            AcqSimSet.WidthTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Micrometer);
            stateproba = new double[] { 0.65, 0.19, 0.15, 0.01 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 5, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 3
            var ToleranceSet2 = new TSVResultSettings()
            {
                DepthTarget = new Length(350, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(30.0, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(500.0, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(20.0, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(450.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(20.0, LengthToleranceUnit.Micrometer)
            };

            var AcqSimSet2 = new TSVResultSettings()
            {
                DepthTarget = new Length(345.0, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(2.0, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(505.0, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(2.0, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(448.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(2.0, LengthToleranceUnit.Micrometer)
            };
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 1, xs_40pts, ys_40pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 4
            stateproba = new double[] { 0.4, 0.35, 0.25, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 4, xs_40pts, ys_40pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 5
            AcqSimSet2.DepthTarget = new Length(100.0, LengthUnit.Micrometer);
            AcqSimSet2.DepthTolerance = new LengthTolerance(25.0, LengthToleranceUnit.Micrometer);
            AcqSimSet2.WidthTarget = new Length(300.0, LengthUnit.Micrometer);
            AcqSimSet2.WidthTolerance = new LengthTolerance(50.0, LengthToleranceUnit.Micrometer);
            AcqSimSet2.LengthTarget = new Length(450.0, LengthUnit.Micrometer);
            AcqSimSet2.LengthTolerance = new LengthTolerance(20.0, LengthToleranceUnit.Micrometer);
            stateproba = new double[] { 0.05, 0.0, 0.2, 0.75 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 300, 2, xs_40pts, ys_40pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 6
            var ToleranceSet3 = new TSVResultSettings()
            {
                DepthTarget = new Length(875, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(30.0, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(365, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(15.0, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(550.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(20.0, LengthToleranceUnit.Micrometer)
            };
            var AcqSimSet3 = new TSVResultSettings()
            {
                DepthTarget = new Length(875, LengthUnit.Micrometer),
                DepthTolerance = new LengthTolerance(30.0 * 0.5, LengthToleranceUnit.Micrometer),
                WidthTarget = new Length(365, LengthUnit.Micrometer),
                WidthTolerance = new LengthTolerance(15.0 * 0.5, LengthToleranceUnit.Micrometer),
                LengthTarget = new Length(550.0, LengthUnit.Micrometer),
                LengthTolerance = new LengthTolerance(20 * 0.5, LengthToleranceUnit.Micrometer)
            };
            stateproba = new double[] { 0.75, 0.2, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 150, 3, xs_20pts, ys_20pts, ToleranceSet3, AcqSimSet3, stateproba);

            // slot 7
            stateproba = new double[] { 0.65, 0.2, 0.15, 0.0 };
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 150, 10, xs_20pts, ys_20pts, ToleranceSet3, AcqSimSet3, stateproba);

            // Slot 8
            double[] xdies_3pts = new double[] { 1.5, 5.0, 9.0 };
            double[] ydies_3pts = new double[] { 7.5, 5.0, 2.0 };
            GenerateTSVDieSampleFile(ref worker, ref total, ref complete, slot++, 300, 130, 1, xdies_3pts, ydies_3pts, ToleranceSet3, AcqSimSet3, stateproba);

            // Slot 9
            GenerateTSVDieSampleFile(ref worker, ref total, ref complete, slot++, 300, 95, 5, xdies_3pts, ydies_3pts, ToleranceSet3, AcqSimSet3, stateproba);

            // Slot 10
            double[] xdies_5pts = new double[] { 0.8, 9.2, 5.0, 0.8, 9.2 };
            double[] ydies_5pts = new double[] { 0.8, 0.8, 5.0, 7.2, 7.2 };
            stateproba = new double[] { 0.6, 0.05, 0.1, 0.25 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateTSVDieSampleFile(ref worker, ref total, ref complete, slot++, 300, 75, 2, xdies_5pts, ydies_5pts, ToleranceSet3, AcqSimSet3, stateproba);

            // Slot 11
            GenRandomptsSite(out double[] xs_Randpts, out double[] ys_Randpts, 4, 150);
            stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            GenerateTSVSampleFile(ref worker, ref total, ref complete, slot++, 150, 2, xs_Randpts, ys_Randpts, ToleranceSet, AcqSimSet, stateproba, true);

        }

        private static void GenerateTSVSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, TSVResultSettings toleranceSettings, TSVResultSettings acqSettings, double[] stateProba, bool genThumbnails = false)
        {

            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anatsv proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_TSV);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_TSV.GetExt()}");

            string TsvThumbPath = string.Empty;
            if (genThumbnails)
            {
                TsvThumbPath = Path.Combine(GeneratePath, $"S{slot}_TSV_Thumbs");
                Directory.CreateDirectory(TsvThumbPath);
            }

            var MeasuresSimulated = new TSVResult();
            MeasuresSimulated.DiesMap = null;
            MeasuresSimulated.Dies = null;

            MeasuresSimulated.Wafer = new UnitySC.Shared.Data.WaferDimensionalCharacteristic()
            {
                WaferShape = WaferShape.Notch,
                Diameter = new Length(waferdiameter_mm, LengthUnit.Millimeter),
                Category = "1.15sim",
                DiameterTolerance = new Length(500, LengthUnit.Micrometer),
                Notch = new UnitySC.Shared.Data.NotchDimentionalCharacteristic()
                {
                    Depth = new Length(1.2, LengthUnit.Millimeter),
                    Angle = new Angle(0, AngleUnit.Degree),
                    DepthPositiveTolerance = new Length(0.250, LengthUnit.Millimeter),
                    AngleNegativeTolerance = new Angle(1, AngleUnit.Degree),
                    AnglePositiveTolerance = new Angle(5, AngleUnit.Degree),
                },
                SampleHeight = new Length(0, LengthUnit.Millimeter),
                SampleWidth = new Length(0, LengthUnit.Millimeter)
            };

            MeasuresSimulated.Name = $"S{slot} simulated TSV result";
            MeasuresSimulated.Information = $"Simulated TSV resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.BestFitPlan = new BestFitPlan()
            {
                CoeffA = new Length(1.1, LengthUnit.Micrometer),
                CoeffB = new Length(2.02, LengthUnit.Micrometer),
                CoeffC = new Length(0.03, LengthUnit.Micrometer)
            };

            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new TSVPointResult();
                ptRes.CoplaInWaferValue = new Length(2.0 * rnd.NextDouble(), LengthUnit.Micrometer);
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new TSVPointData();
                    acqdata.IndexRepeta = k;

                    double prnd = rnd.NextDouble();
                    double probCum = 0.0;
                    for (int j = 0; j < 4; j++)
                    {
                        probCum += stateProba[j];
                        if (prnd <= probCum)
                        {
                            MaxStateSeen = Math.Max(MaxStateSeen, j);
                            acqdata.State = (MeasureState)j;
                            acqdata.QualityScore = rnd.NextDouble() * (Qmax[j] - Qmin[j]) + Qmin[j];
                            qsum += acqdata.QualityScore;
                            break;
                        }
                    }

                    if (acqdata.State != MeasureState.NotMeasured)
                    {
                        bAllNotMeasured = false;
                        acqdata.Depth = new Length(NextGaussian(rnd, acqSettings.DepthTarget.Micrometers, 3.0 * acqSettings.DepthTolerance.Value), LengthUnit.Micrometer);
                        acqdata.Width = new Length(NextGaussian(rnd, acqSettings.WidthTarget.Micrometers, 3.0 * acqSettings.WidthTolerance.Value), LengthUnit.Micrometer);
                        acqdata.Length = new Length(NextGaussian(rnd, acqSettings.LengthTarget.Micrometers, 3.0 * acqSettings.LengthTolerance.Value), LengthUnit.Micrometer);

                        if (genThumbnails)
                        {

                            using (var whitpen = new Pen(Color.White))
                            using (var Graypen = new Pen(Color.DarkGray))
                            using (var zefont = new Font("Arial", 10.0f))
                            using (var ViewBmp = new Bitmap(100, 100))
                            {
                                double dPixelSzW = 40.0 / toleranceSettings.WidthTarget.Micrometers;
                                double dPixelSzL = 40.0 / toleranceSettings.LengthTarget.Micrometers;
                                var gImage = Graphics.FromImage(ViewBmp);
                                gImage.Clear(Color.Black);
                                gImage.FillEllipse(Graypen.Brush, 25, 25, 50, 50);
                                gImage.FillEllipse(whitpen.Brush,
                                    (float)(50.0 - (0.5 * acqdata.Width.Micrometers * dPixelSzW)),
                                    (float)(50.0 - (0.5 * acqdata.Length.Micrometers * dPixelSzL)),
                                    (float)(acqdata.Width.Micrometers * dPixelSzW),
                                    (float)(acqdata.Length.Micrometers * dPixelSzL));


                                var Labelrect = new Rectangle(2, 2, 96, 18);
                                var stringFormat = new StringFormat();
                                stringFormat.Alignment = StringAlignment.Center;
                                stringFormat.LineAlignment = StringAlignment.Center;
                                gImage.DrawString($"{i}_{k}", zefont, Brushes.Cyan, Labelrect, stringFormat);

                                ViewBmp.Save(Path.Combine(TsvThumbPath, $"{i}_{k}.png"));
                            }
                            acqdata.ResultImageFileName = Path.Combine($".\\S{slot}_TSV_Thumbs", $"{i}_{k}.png");
                        }

                    }
                    else
                        bHasSomeNotMeasured = true;

                    ptRes.Datas.Add(acqdata);
                }

                qsum /= nbRepeta;

                bool bUseMaxStateSee_Strategy = true;
                if (bUseMaxStateSee_Strategy)
                {
                    ptRes.State = (MeasureState)MaxStateSeen;
                }
                else
                {
                    // point  result state is alaborate from average 
                    if (bAllNotMeasured || bHasSomeNotMeasured)
                        ptRes.State = MeasureState.NotMeasured;
                    else
                    {
                        if (qsum <= 0.40)
                            ptRes.State = MeasureState.Error;
                        else if (qsum <= 0.75)
                            ptRes.State = MeasureState.Partial;
                        else
                            ptRes.State = MeasureState.Success;
                    }
                }

                MeasuresSimulated.Points.Add(ptRes);
                worker.ReportProgress(CalculateProgress(total, ++complete));
            }

            res.MeasureResult = MeasuresSimulated;
            if (!res.WriteInFile(res.ResFilePath, out string serrror))
                MessageBox.Show($"Cannot generate S{slot}.anares :\n{serrror}");
            else
            {
                //generate a thumbnail
                var display = new MetroDisplay();
                display.GenerateThumbnailFile(res);

                string thumbPath = FormatHelper.ThumbnailPathOf(res.ResFilePath);
                string genThumbPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(thumbPath)), Path.GetFileName(thumbPath));
                if (File.Exists(genThumbPath))
                    File.Delete(genThumbPath);
                File.Move(thumbPath, genThumbPath);
            }

        }

        private static void GenerateTSVDieSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int maxradius_mm, int nbRepeta, double[] xdies, double[] ydies, TSVResultSettings toleranceSettings, TSVResultSettings acqSettings, double[] stateProba)
        {


            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anatsv proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_TSV);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_TSV.GetExt()}");

            var MeasuresSimulated = new TSVResult();
            MeasuresSimulated.Points = null;

            MeasuresSimulated.DiesMap = new WaferMap()
            {
                RotationAngle = new Angle(0.000012, AngleUnit.Degree),
                DieSizeWidth = new Length(10.0, LengthUnit.Millimeter),
                DieSizeHeight = new Length(8.0, LengthUnit.Millimeter),
                DiePitchWidth = new Length(14.0, LengthUnit.Millimeter),
                DiePitchHeight = new Length(16.0, LengthUnit.Millimeter),
                DieGridTopLeftXPosition = new Length(3.0, LengthUnit.Millimeter), //a voir au pif pour l'instant
                DieGridTopLeftYPosition = new Length(6.0, LengthUnit.Millimeter), //a voir au pif pour l'instant
                DieReferenceColumnIndex = 6,
                DieReferenceRowIndex = 4
            };

            //pour du 300mm on va faire du 21 x 18 (21 col, 18 rows of dies) 
            double HalfWaferSize__mm = waferdiameter_mm / 2.0;
            double Maxradiusinside = maxradius_mm;
            double Maxradiusinside_sqr = Maxradiusinside * Maxradiusinside;

            var DM = MeasuresSimulated.DiesMap;
            int nDieColMax = (int)Math.Floor((double)waferdiameter_mm / DM.DiePitchWidth.Millimeters); //21;
            int nDieRowMax = (int)Math.Floor((double)waferdiameter_mm / DM.DiePitchHeight.Millimeters); //18;

            DM.DieGridTopLeftXPosition = new Length(-HalfWaferSize__mm + ((double)waferdiameter_mm - (double)nDieColMax * DM.DiePitchWidth.Millimeters) * 0.5, LengthUnit.Millimeter);
            DM.DieGridTopLeftYPosition = new Length(HalfWaferSize__mm - ((double)waferdiameter_mm - (double)nDieRowMax * DM.DiePitchHeight.Millimeters) * 0.5, LengthUnit.Millimeter);

            bool[][] bdiepresencearray = new bool[nDieColMax][];
            int NbDiesPresent = 0;
            for (int i = 0; i < nDieColMax; i++)
            {
                bdiepresencearray[i] = new bool[nDieRowMax];
                for (int j = 0; j < nDieRowMax; j++)
                {
                    double dx = (DM.DieGridTopLeftXPosition.Millimeters + DM.DieSizeWidth.Millimeters * 0.5 + i * DM.DiePitchWidth.Millimeters);
                    double dy = (DM.DieGridTopLeftYPosition.Millimeters - (DM.DieSizeHeight.Millimeters * 0.5 + j * DM.DiePitchHeight.Millimeters));
                    double dist_sqr = dx * dx + dy * dy;
                    bdiepresencearray[i][j] = (dist_sqr <= Maxradiusinside_sqr);
                    if (bdiepresencearray[i][j])
                    {
                        NbDiesPresent++;
                    }
                }
            }
            MeasuresSimulated.DiesMap.SetDiesPresences(bdiepresencearray);

            total += NbDiesPresent * xdies.Length;

            MeasuresSimulated.Wafer = new UnitySC.Shared.Data.WaferDimensionalCharacteristic()
            {
                WaferShape = WaferShape.Notch,
                Diameter = new Length(waferdiameter_mm, LengthUnit.Millimeter),
                Category = "1.15sim",
                DiameterTolerance = new Length(500, LengthUnit.Micrometer),
                Notch = new UnitySC.Shared.Data.NotchDimentionalCharacteristic()
                {
                    Depth = new Length(1.2, LengthUnit.Millimeter),
                    Angle = new Angle(0, AngleUnit.Degree),
                    DepthPositiveTolerance = new Length(0.250, LengthUnit.Millimeter),
                    AngleNegativeTolerance = new Angle(1, AngleUnit.Degree),
                    AnglePositiveTolerance = new Angle(5, AngleUnit.Degree),
                },
                SampleHeight = new Length(0, LengthUnit.Millimeter),
                SampleWidth = new Length(0, LengthUnit.Millimeter)
            };

            MeasuresSimulated.Name = $"S{slot} simulated TSV DIE result";
            MeasuresSimulated.Information = $"Simulated TSV DIE resuls, NbDies = {NbDiesPresent} NbSite = {xdies.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.BestFitPlan = new BestFitPlan()
            {
                CoeffA = new Length(1.8, LengthUnit.Micrometer),
                CoeffB = new Length(2.02, LengthUnit.Micrometer),
                CoeffC = new Length(0.03, LengthUnit.Micrometer)
            };

            MeasuresSimulated.Dies = new List<MeasureDieResult>(NbDiesPresent);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };

            for (int j = 0; j < nDieRowMax; j++)
            {
                for (int i = 0; i < nDieColMax; i++)
                {
                    if (bdiepresencearray[i][j])
                    {
                        var resDie = new TSVDieResult()
                        {
                            BestFitPlan = new BestFitPlan()
                            {
                                CoeffA = new Length(rnd.NextDouble(), LengthUnit.Micrometer),
                                CoeffB = new Length(rnd.NextDouble(), LengthUnit.Micrometer),
                                CoeffC = new Length(rnd.NextDouble() - 0.5, LengthUnit.Micrometer)
                            },
                            ColumnIndex = i - DM.DieReferenceColumnIndex,
                            RowIndex = -(j - DM.DieReferenceRowIndex),
                            State = GlobalState.Success
                        };

                        for (int nx = 0; nx < xdies.Length; nx++)
                        {
                            bool bAllNotMeasured = true;

                            var ptRes = new TSVPointResult();
                            ptRes.CoplaInDieValue = new Length(2.0 * rnd.NextDouble(), LengthUnit.Micrometer);
                            ptRes.XPosition = xdies[nx];
                            ptRes.YPosition = ydies[nx];

                            double qsum = 0.0;
                            int MaxStateSeen = -1;
                            for (int k = 0; k < nbRepeta; k++)
                            {
                                var acqdata = new TSVPointData();
                                acqdata.IndexRepeta = k;

                                double prnd = rnd.NextDouble();
                                double probCum = 0.0;
                                for (int m = 0; m < 4; m++)
                                {
                                    probCum += stateProba[m];
                                    if (prnd <= probCum)
                                    {
                                        MaxStateSeen = Math.Max(MaxStateSeen, m);

                                        acqdata.State = (MeasureState)m;
                                        acqdata.QualityScore = rnd.NextDouble() * (Qmax[m] - Qmin[m]) + Qmin[m];
                                        qsum += acqdata.QualityScore;
                                        break;
                                    }
                                }

                                if (acqdata.State != MeasureState.NotMeasured)
                                {
                                    bAllNotMeasured = false;
                                    acqdata.Depth = new Length(NextGaussian(rnd, acqSettings.DepthTarget.Micrometers, 3.0 * acqSettings.DepthTolerance.Value), LengthUnit.Micrometer);
                                    acqdata.Width = new Length(NextGaussian(rnd, acqSettings.WidthTarget.Micrometers, 3.0 * acqSettings.WidthTolerance.Value), LengthUnit.Micrometer);
                                    acqdata.Length = new Length(NextGaussian(rnd, acqSettings.LengthTarget.Micrometers, 3.0 * acqSettings.LengthTolerance.Value), LengthUnit.Micrometer);
                                }
                                ptRes.Datas.Add(acqdata);
                            }

                            qsum /= nbRepeta;
                            bool bUseMaxStateSee_Strategy = true;
                            if (bUseMaxStateSee_Strategy)
                            {
                                ptRes.State = (MeasureState)MaxStateSeen;
                            }
                            else
                            {
                                if (bAllNotMeasured)
                                    ptRes.State = MeasureState.NotMeasured;
                                else
                                {
                                    if (qsum <= 0.40)
                                        ptRes.State = MeasureState.Error;
                                    else if (qsum <= 0.75)
                                        ptRes.State = MeasureState.Partial;
                                    else
                                        ptRes.State = MeasureState.Success;
                                }
                            }
                            resDie.Points.Add(ptRes);
                            worker.ReportProgress(CalculateProgress(total, ++complete));
                        }

                        MeasuresSimulated.Dies.Add(resDie);
                    }

                }
            }

            res.MeasureResult = MeasuresSimulated;
            if (!res.WriteInFile(res.ResFilePath, out string serrror))
                MessageBox.Show($"Cannot generate S{slot}.anatsv :\n{serrror}");
            else
            {
                //generate a thumbnail
                var display = new MetroDisplay();
                display.GenerateThumbnailFile(res);
                string thumbPath = FormatHelper.ThumbnailPathOf(res.ResFilePath);
                string destPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(thumbPath)), Path.GetFileName(thumbPath));
                if (File.Exists(destPath))
                    File.Delete(destPath);
                File.Move(thumbPath, destPath);
            }

        }
        #endregion //Generate TSV files

    }
}
