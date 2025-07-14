using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Data.Enum;
using System.IO;

using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;
using System.Windows.Media;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {


        #region Generate Thickness files
        private static void GenerateThicknessSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            total += 3 + 40 * 1 + 20 * 1 + 10 * 1; // normal measures points

            Gen40ptsSite(out double[] xs_40pts, out double[] ys_40pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            Gen20ptsSite(out double[] xs_20pts, out double[] ys_20pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            GenRandomptsSite(out double[] xs_10pts, out double[] ys_10pts, 10, 200);
            worker.ReportProgress(CalculateProgress(total, ++complete));

            var layerA = new ThicknessLengthSettings() { Name = $"Layer A", Target = new Length(750, LengthUnit.Micrometer), Tolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer),
                LayerColor = (Color)ColorConverter.ConvertFromString("#FF88B25E"),  IsMeasured = true };
            var layerB = new ThicknessLengthSettings() { Name = $"Layer Thin B", Target = new Length(500, LengthUnit.Nanometer), Tolerance = new LengthTolerance(100, LengthToleranceUnit.Nanometer),
                LayerColor = (Color)ColorConverter.ConvertFromString("#FF8FA3C6"), IsMeasured = true};
            var layerC = new ThicknessLengthSettings() { Name = $"Layer C", Target = new Length(125, LengthUnit.Micrometer), Tolerance = new LengthTolerance(8, LengthToleranceUnit.Micrometer),
                LayerColor = (Color)ColorConverter.ConvertFromString("#FFB2725E"), IsMeasured = true};
            var layerD = new ThicknessLengthSettings() { Name = $"Layer D", Target = new Length(300, LengthUnit.Micrometer), Tolerance = new LengthTolerance(35, LengthToleranceUnit.Micrometer),
                LayerColor = (Color)ColorConverter.ConvertFromString("#FFE3B238"),  IsMeasured = true};

            int slot = 0;
            // success , partial, error, notmeasure
            var ToleranceSet = new ThicknessResultSettings()
            {
                TotalTarget = new Length(1.1755, LengthUnit.Millimeter),
                TotalTolerance = new LengthTolerance(93.1, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerB, layerC, layerD },
                HasWaferThicknesss = true
            };
            var AcqSimSet = new ThicknessResultSettings()
            {
                TotalTarget = new Length(1.1755, LengthUnit.Millimeter),
                TotalTolerance = new LengthTolerance(93.1, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerB, layerC, layerD },
                HasWaferThicknesss = true
            };
            layerA.IsMeasured = layerB.IsMeasured = layerC.IsMeasured = layerD.IsMeasured = true;
            double[] stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            // slot 0 // 4 layer random - no repeta
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 200, 1, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 1 // 4 layer random - no repeta
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 200, 1, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 2 // 4 layer random - repeta x5
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 200, 5, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 3 // 3 Layer x40 -- Layer D is not measured - wafer thickness is not measured
            var ToleranceSet2 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(1170, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(95, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC, layerD },
                HasWaferThicknesss = false
            };
            var AcqSimSet2 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(1170, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(93, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC, layerD },
                HasWaferThicknesss = false
            };
            layerA.IsMeasured = layerB.IsMeasured = layerC.IsMeasured = true;
            layerD.IsMeasured = false;
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 300, 1, xs_40pts, ys_40pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 4 -- Layer D is not measured - wafer thickness is not measured
            stateproba = new double[] { 0.4, 0.35, 0.25, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 300, 3, xs_40pts, ys_40pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 5 // 1 Layer x20 - rep 3
            var ToleranceSet3 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(875, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(58, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC },
                HasWaferThicknesss = true
            };
            var AcqSimSet3 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(875, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(58, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC },
                HasWaferThicknesss = true
            };
            layerA.IsMeasured = true;
            layerB.IsMeasured = layerC.IsMeasured = layerD.IsMeasured = false;
            stateproba = new double[] { 0.6, 0.35, 0.05, 0.0 };
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 150, 3, xs_20pts, ys_20pts, ToleranceSet3, AcqSimSet3, stateproba);

            // slot 6 // 1 Layer x20 - rep 3 - with several not measured
            stateproba = new double[] { 0.1, 0.0, 0.5, 0.4 };
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 150, 3, xs_20pts, ys_20pts, ToleranceSet3, AcqSimSet3, stateproba);

            //Slot 7 - Die
            var ToleranceSet4 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(875, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(58, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC},
                HasWaferThicknesss = false
            };
            var AcqSimSet4 = new ThicknessResultSettings()
            {
                TotalTarget = new Length(875, LengthUnit.Micrometer),
                TotalTolerance = new LengthTolerance(58.0, LengthToleranceUnit.Micrometer),
                ThicknessLayers = new List<ThicknessLengthSettings>() { layerA, layerC },
                HasWaferThicknesss = false
            };
            layerA.IsMeasured = layerC.IsMeasured = true;
            layerB.IsMeasured = layerD.IsMeasured = false;
            stateproba = new double[] { 0.7, 0.2, 0.09, 0.01 };
            System.Diagnostics.Debug.Assert((stateproba.Sum() - 1.0) <= 0.00001);

            //Slot 7 - Die x4 - // 2 Layer repx3 
            double DieSizW_mm = 1.5;
            double DieSizH_mm = 1.0;
            double[] xdies_4pts = new double[] { 0.1, 0.75, 1.2, 1.3 };
            double[] ydies_4pts = new double[] { 0.9, 0.75, 0.95, 0.05 };

            GenerateThicknessDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 60, 3, DieSizW_mm, DieSizH_mm, 13.0,
                 xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, true);

            //Slot 8 - Die x4 - // 2 Layer rep x5 
            xdies_4pts = new double[] { 0.2, 0.7, 0.5, 1.4 };
            ydies_4pts = new double[] { 0.8, 0.3, 0.5, 1.0 };
            GenerateThicknessDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 30, 5, DieSizW_mm, DieSizH_mm, 20.0,
                xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, true);

            //Slot 9 - Big Die - // 2 Layer rep x1  - thumbnails 3D
            DieSizW_mm = 32.5;
            DieSizH_mm = 32.5;
            double[] xdies_2pts = new double[] { 0.75 * DieSizW_mm, 0.25 * DieSizW_mm, };
            double[] ydies_2pts = new double[] { 0.75 * DieSizH_mm, 0.25 * DieSizH_mm, };
            GenerateThicknessDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 75, 1, DieSizW_mm, DieSizH_mm, 0.034,
                xdies_2pts, ydies_2pts, ToleranceSet4, AcqSimSet4, stateproba, true);

            //Slot 10 - 3 layers -  numerous sites and repeta+
            layerA.IsMeasured = layerB.IsMeasured = layerC.IsMeasured = true;
            layerD.IsMeasured = false;
            GenRandomptsSite(out double[] xs_200pts, out double[] ys_200pts, 200, 300);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateThicknessSampleFile(ref worker, ref total, ref complete, slot++, 300, 3, xs_200pts, ys_200pts, ToleranceSet2, AcqSimSet2, stateproba);
        }

        private static void GenerateThicknessSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, ThicknessResultSettings toleranceSettings, ThicknessResultSettings acqSettings, double[] stateProba)
        {
            double probsum = stateProba.Sum();
            if ((probsum - 1.0) > 0.00001)
            {
                MessageBox.Show($"S{slot} anathick proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_Thickness);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_Thickness.GetExt()}");

            var MeasuresSimulated = new ThicknessResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated Thickness result";
            MeasuresSimulated.Information = $"Simulated Thickness resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new ThicknessPointResult();
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new ThicknessPointData();
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
                        if (acqSettings.ThicknessLayers != null)
                        {
                            acqdata.ThicknessLayerResults = new List<ThicknessLengthResult>(acqSettings.ThicknessLayers.Count);
                            foreach (var layer in acqSettings.ThicknessLayers)
                            {
                                if (layer.IsMeasured)
                                {
                                    acqdata.ThicknessLayerResults.Add(new ThicknessLengthResult()
                                    {
                                        Name = layer.Name,
                                        Length = new Length(NextGaussian(rnd, layer.Target.Value, 3.0 * layer.Tolerance.Value), layer.Target.Unit)
                                    });
                                }
                            }
                            if (acqSettings.HasWaferThicknesss)
                            {
                                double waferThickMeasure_um = NextGaussian(rnd, acqSettings.TotalTarget.Micrometers, 3.0 * acqSettings.TotalTolerance.GetAbsoluteTolerance(acqSettings.TotalTarget).Micrometers);
                                acqdata.WaferThicknessResult = new ThicknessLengthResult()
                                {
                                    Name = ThicknessResultSettings.WaferThicknessLayerName,
                                    Length = new Length(waferThickMeasure_um, LengthUnit.Micrometer).ToUnit(acqSettings.TotalTarget.Unit)
                                };
                            }
                            else
                                acqdata.WaferThicknessResult = null;
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
                MessageBox.Show($"Cannot generate S{slot}.anathick :\n{serrror}");
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

        private static void GenerateThicknessDieSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot,
        int waferdiameter_mm, int maxradius_mm, int nbRepeta,
        double dieSizeW_mm, double dieSizeH_mm, double testdiesigma,
        double[] xdies, double[] ydies,
        ThicknessResultSettings toleranceSettings, ThicknessResultSettings acqSettings, double[] stateProba, bool computeDieGridXY = false)
        {
            double probsum = stateProba.Sum();
            if ((probsum - 1.0) > 0.00001)
            {
                MessageBox.Show($"S{slot} anathick proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_Thickness);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_Thickness.GetExt()}");

            var MeasuresSimulated = new ThicknessResult();
            MeasuresSimulated.Points = null;

            MeasuresSimulated.DiesMap = new WaferMap()
            {
                RotationAngle = new Angle(0.000001, AngleUnit.Degree),
                DieSizeWidth = new Length(dieSizeW_mm, LengthUnit.Millimeter),
                DieSizeHeight = new Length(dieSizeH_mm, LengthUnit.Millimeter),
                DiePitchWidth = new Length(dieSizeW_mm + 0.02, LengthUnit.Millimeter),
                DiePitchHeight = new Length(dieSizeH_mm + 0.01, LengthUnit.Millimeter),
                DieGridTopLeftXPosition = new Length(3.0, LengthUnit.Millimeter), //a voir au pif pour l'instant
                DieGridTopLeftYPosition = new Length(0.5, LengthUnit.Millimeter), //a voir au pif pour l'instant
                DieReferenceColumnIndex = 0,
                DieReferenceRowIndex = 0
            };

            //pour du 150mm on va faire du 91(98) x 124() (91 col, 124 rows of dies) 
            double HalfWaferSize__mm = waferdiameter_mm / 2.0;
            double Maxradiusinside = maxradius_mm;
            double Maxradiusinside_sqr = Maxradiusinside * Maxradiusinside;

            var DM = MeasuresSimulated.DiesMap;
            int nDieColMax = (int)Math.Floor((double)(waferdiameter_mm - DM.DieGridTopLeftXPosition.Millimeters) / DM.DiePitchWidth.Millimeters);
            int nDieRowMax = (int)Math.Floor((double)(waferdiameter_mm - DM.DieGridTopLeftYPosition.Millimeters) / DM.DiePitchHeight.Millimeters);

            if (computeDieGridXY)
            {
                DM.DieGridTopLeftXPosition = new Length(-HalfWaferSize__mm + ((double)waferdiameter_mm - (double)nDieColMax * DM.DiePitchWidth.Millimeters) * 0.5, LengthUnit.Millimeter);
                DM.DieGridTopLeftYPosition = new Length(HalfWaferSize__mm - ((double)waferdiameter_mm - (double)nDieRowMax * DM.DiePitchHeight.Millimeters) * 0.5, LengthUnit.Millimeter);
                DM.DieReferenceColumnIndex = nDieColMax / 2;
                DM.DieReferenceRowIndex = nDieRowMax / 2;
            }

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
                        double val = NextGaussian(rnd, 0, testdiesigma);
                        if (Math.Abs(val) < 0.34)
                            NbDiesPresent++;
                        else
                            bdiepresencearray[i][j] = false;
                    }
                }
            }
            MeasuresSimulated.DiesMap.SetDiesPresences(bdiepresencearray);

            total += NbDiesPresent * xdies.Length;

            MeasuresSimulated.Wafer = new UnitySC.Shared.Data.WaferDimensionalCharacteristic()
            {
                WaferShape = WaferShape.Flat,
                Diameter = new Length(waferdiameter_mm, LengthUnit.Millimeter),
                Category = "1.5/1.6",
                DiameterTolerance = new Length(0.2, LengthUnit.Millimeter),
                Flats = new List<UnitySC.Shared.Data.FlatDimentionalCharacteristic>()
                {
                    new UnitySC.Shared.Data.FlatDimentionalCharacteristic()
                    {
                        Angle = new Angle(0, AngleUnit.Degree),
                        AngleTolerance = null,
                        ChordLength = new Length(57.5, LengthUnit.Millimeter),
                        ChordLengthTolerance = new Length(2.5, LengthUnit.Millimeter)
                    },
                    new UnitySC.Shared.Data.FlatDimentionalCharacteristic()
                    {
                        Angle = new Angle(-90, AngleUnit.Degree),
                        AngleTolerance = null,
                        ChordLength = new Length(37.5, LengthUnit.Micrometer),
                        ChordLengthTolerance = new Length(2.5, LengthUnit.Millimeter)
                    }
                },
                SampleHeight = new Length(0, LengthUnit.Millimeter),
                SampleWidth = new Length(0, LengthUnit.Millimeter)
            };

            MeasuresSimulated.Name = $"S{slot} simulated Thickness DIE result";
            MeasuresSimulated.Information = $"Simulated Thickness DIE resuls, NbDies = {NbDiesPresent} NbSite = {xdies.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;

            MeasuresSimulated.Dies = new List<MeasureDieResult>(NbDiesPresent);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };

            for (int j = 0; j < nDieRowMax; j++)
            {
                for (int i = 0; i < nDieColMax; i++)
                {
                    if (bdiepresencearray[i][j])
                    {
                        var resDie = new MeasureDieResult()
                        {
                            ColumnIndex = i - DM.DieReferenceColumnIndex,
                            RowIndex = -(j - DM.DieReferenceRowIndex),
                            State = GlobalState.Success
                        };

                        for (int nx = 0; nx < xdies.Length; nx++)
                        {
                            bool bAllNotMeasured = true;

                            var ptRes = new ThicknessPointResult();
                            ptRes.XPosition = xdies[nx];
                            ptRes.YPosition = ydies[nx];

                            double qsum = 0.0;
                            int MaxStateSeen = -1;
                            for (int k = 0; k < nbRepeta; k++)
                            {
                                var acqdata = new ThicknessPointData();
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

                                    if (acqSettings.ThicknessLayers != null)
                                    {
                                        acqdata.ThicknessLayerResults = new List<ThicknessLengthResult>(acqSettings.ThicknessLayers.Count);
                                        foreach (var layer in acqSettings.ThicknessLayers)
                                        {
                                            if (layer.IsMeasured)
                                            {
                                                acqdata.ThicknessLayerResults.Add(new ThicknessLengthResult()
                                                {
                                                    Name = layer.Name,
                                                    Length = new Length(NextGaussian(rnd, layer.Target.Value, 3.0 * layer.Tolerance.Value), layer.Target.Unit)
                                                });
                                            }
                                        }
                                        if (acqSettings.HasWaferThicknesss)
                                        {
                                            double waferThickMeasure_um = NextGaussian(rnd, acqSettings.TotalTarget.Micrometers, 3.0 * acqSettings.TotalTolerance.GetAbsoluteTolerance(acqSettings.TotalTarget).Micrometers);
                                            acqdata.ThicknessLayerResults.Add(new ThicknessLengthResult()
                                            {
                                                Name = ThicknessResultSettings.WaferThicknessLayerName,
                                                Length = new Length(waferThickMeasure_um, LengthUnit.Micrometer).ToUnit(acqSettings.TotalTarget.Unit)
                                            });
                                        }
                                    }
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
                MessageBox.Show($"Cannot generate S{slot}.anares :\n{serrror}");
            else
            {
                //generate a thumbnail
                var display = new MetroDisplay();
                display.GenerateThumbnailFile(res);
                string thumbPath = FormatHelper.ThumbnailPathOf(res.ResFilePath);
                File.Move(thumbPath, Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(thumbPath)), Path.GetFileName(thumbPath)));
            }
        }

        #endregion
    }
}
