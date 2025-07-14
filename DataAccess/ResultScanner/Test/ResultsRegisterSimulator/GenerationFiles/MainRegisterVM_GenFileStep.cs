using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Step;
using System.Windows;
using UnitySC.Shared.Data.Enum;
using System.IO;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;
using System.Drawing;
using UnitySC.Shared.Data.FormatFile;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {
        #region Generate StepHeight files
        private static void GenerateSTEPSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            total += 3 + 5 * 1 + 25 * 3 + 10 * 3; // normal measures points

            Gen5ptsSite(out double[] xs_5pts, out double[] ys_5pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            GenRandomptsSite(out double[] xs_30pts, out double[] ys_30pts, 30, 300);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            GenRandomptsSite(out double[] xs_10pts, out double[] ys_10pts, 10, 200);
            worker.ReportProgress(CalculateProgress(total, ++complete));

         
            int slot = 0;
            // success , partial, error, notmeasure
            var ToleranceSet = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(5, LengthToleranceUnit.Micrometer),
                StepHeightTarget = new Length(110, LengthUnit.Micrometer),
                StepUp = true
            };
            var AcqSimSet = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(8, LengthToleranceUnit.Micrometer),
                StepHeightTarget = new Length(109, LengthUnit.Micrometer),
                StepUp = true
            };

            double[] stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            // slot 0 
            GenerateSTEPSampleFile(ref worker, ref total, ref complete, slot++, 200, 1, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba, null, 0);

            // slot 1 
            AcqSimSet.StepHeightTolerance.Value = 5;
            AcqSimSet.StepHeightTarget = ToleranceSet.StepHeightTarget;
            GenerateSTEPSampleFile(ref worker, ref total, ref complete, slot++, 200, 3, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba, new Length(100, LengthUnit.Micrometer), 50);

            // slot 2  
            ToleranceSet.StepUp = false; AcqSimSet.StepUp = false;
            AcqSimSet.StepHeightTolerance.Value = 12;
            stateproba = new double[] { 0.65, 0.19, 0.15, 0.01 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateSTEPSampleFile(ref worker, ref total, ref complete, slot++, 200, 5, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba, new Length(150, LengthUnit.Micrometer), 50);

            // slot 3 
            var ToleranceSet2 = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(90, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(850, LengthUnit.Nanometer),
                StepUp = true
            };

            var AcqSimSet2 = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(93, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(840, LengthUnit.Nanometer),
                StepUp = true
            };
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateSTEPSampleFile(ref worker, ref total, ref complete, slot++, 300, 1, xs_30pts, ys_30pts, ToleranceSet2, AcqSimSet2, stateproba, null, 0);

            // slot 4 
            stateproba = new double[] { 0.4, 0.35, 0.25, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            AcqSimSet2.StepHeightTolerance.Value = 200;
            GenerateSTEPSampleFile(ref worker, ref total, ref complete, slot++, 300, 4, xs_30pts, ys_30pts, ToleranceSet2, AcqSimSet2, stateproba, new Length(500, LengthUnit.Micrometer), 120);
    

            //Slot 5 - Die
            var ToleranceSet4 = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(3, LengthToleranceUnit.Micrometer),
                StepHeightTarget = new Length(55, LengthUnit.Micrometer),
                StepUp = true
            };
            var AcqSimSet4 = new StepResultSettings()
            {
                StepHeightTolerance = new LengthTolerance(3, LengthToleranceUnit.Micrometer),
                StepHeightTarget = new Length(56, LengthUnit.Micrometer),
                StepUp = true
            };
            stateproba = new double[] { 0.7, 0.2, 0.09, 0.01 };
            System.Diagnostics.Debug.Assert((stateproba.Sum() - 1.0) <= 0.00001);
            double DieSizW_mm = 0.88;
            double DieSizH_mm = 0.45;
            double[] xdies_4pts = new double[] { 0.2, 0.75, 1.2, 1.3 };
            double[] ydies_4pts = new double[] { 0.5, 0.75, 0.95, 0.15 };

            GenerateSTEPDieSampleFile(ref worker, ref total, ref complete, slot++, 200, 80, 3, DieSizW_mm, DieSizH_mm, 15.0,
                 xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, null, true, 0);

            //Slot 6 - Die - 
            xdies_4pts = new double[] { 0.5, 0.7, 0.65, 1.4 };
            ydies_4pts = new double[] { 0.2, 0.3, 0.5, 1.0 };
            GenerateSTEPDieSampleFile(ref worker, ref total, ref complete, slot++, 200, 60, 1, DieSizW_mm, DieSizH_mm, 20.0,
                 xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, new Length(275,LengthUnit.Micrometer), true, 37);

            //Slot 7 - Big Die
            DieSizW_mm = 32.5;
            DieSizH_mm = 32.5;
            double[] xdies_2pts = new double[] { 0.35 * DieSizW_mm, 0.35 * DieSizW_mm, };
            double[] ydies_2pts = new double[] { 0.825 * DieSizH_mm, 0.85 * DieSizH_mm, };
            GenerateSTEPDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 75, 5, DieSizW_mm, DieSizH_mm, 0.034,
                xdies_2pts, ydies_2pts, ToleranceSet4, AcqSimSet4, stateproba, null, true, 0);

        }

        private static RawProfile GenerateSTEPRawProfile(StepResultSettings acqSettings, Length scanLength, Length stepHeight, int nbPoint, double rndStartPos = 0.01)
        {
            var rnd = new Random();

            RawProfile rawscan = new RawProfile() { XUnit = scanLength.Unit, ZUnit = stepHeight.Unit, RawPoints = new List<RawProfilePoint>(nbPoint)};

            double Zlo_nm = rnd.NextDouble() * (200.0 - 50.0) + 50.0;
            Length Zlow = new Length(Zlo_nm, LengthUnit.Nanometer);

            double sigmaZ = acqSettings.StepHeightTolerance.GetAbsoluteTolerance(acqSettings.StepHeightTarget).GetValueAs(stepHeight.Unit);
            double stepstart = rnd.NextDouble() * rndStartPos;
            double stepX = scanLength.Value / (nbPoint - 1);
            double scanPosX = stepstart;
            int neg = acqSettings.StepUp ? 1 : -1;
            int half = nbPoint / 2;
            for (int i = 0; i < nbPoint; i++)
            {
                double zc = ((neg * (i - half) < 0.0)) ? Zlow.GetValueAs(stepHeight.Unit) : stepHeight.GetValueAs(stepHeight.Unit);
                double zalea = NextGaussian(rnd, zc, sigmaZ);

                rawscan.RawPoints.Add(new RawProfilePoint() { X = scanPosX, Z = zalea });

                scanPosX += stepX;
            }
            return rawscan;
        }

        private static void GenerateSTEPSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, StepResultSettings toleranceSettings, StepResultSettings acqSettings, double[] stateProba, Length scanLength, int useScanRawProfilePts = 0)
        {

            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anantp proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_Step);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_Step.GetExt()}");

            var MeasuresSimulated = new StepResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated STEP result";
            MeasuresSimulated.Information = $"Simulated STEP resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);

            double[] Angles = new double[] { 0.0, 90.0, -90.0, 137.5, 180.0, 255.3 };
            int indang = rnd.Next(0, Angles.Length - 1);
            var scanAngle = new Angle(Angles[indang], AngleUnit.Degree);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new StepPointResult();
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];
                ptRes.ScanAngle = scanAngle;

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new StepPointData();
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
                        acqdata.StepHeight = new Length(NextGaussian(rnd, acqSettings.StepHeightTarget.Value, 3.0 * acqSettings.StepHeightTolerance.Value), acqSettings.StepHeightTarget.Unit);
                        if (useScanRawProfilePts != 0)
                            acqdata.RawProfileScan = GenerateSTEPRawProfile(acqSettings, scanLength, acqdata.StepHeight, useScanRawProfilePts);
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
                MessageBox.Show($"Cannot generate S{slot}.anastp :\n{serrror}");
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

        private static void GenerateSTEPDieSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot,
            int waferdiameter_mm, int maxradius_mm, int nbRepeta,
            double dieSizeW_mm, double dieSizeH_mm, double testdiesigma,
            double[] xdies, double[] ydies,
            StepResultSettings toleranceSettings, StepResultSettings acqSettings, double[] stateProba, Length scanLength, bool computeDieGridXY = false, int useScanRawProfilePts = 0)
        {
            double probsum = stateProba.Sum();
            if ((probsum - 1.0) > 0.00001)
            {
                MessageBox.Show($"S{slot} anantp proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_Step);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_Step.GetExt()}");

            var MeasuresSimulated = new StepResult();
            MeasuresSimulated.Points = null;

            MeasuresSimulated.DiesMap = new WaferMap()
            {
                RotationAngle = new Angle(0.000001, AngleUnit.Degree),
                DieSizeWidth = new Length(dieSizeW_mm, LengthUnit.Millimeter),
                DieSizeHeight = new Length(dieSizeH_mm, LengthUnit.Millimeter),
                DiePitchWidth = new Length(dieSizeW_mm + 0.02, LengthUnit.Millimeter),
                DiePitchHeight = new Length(dieSizeH_mm + 0.02, LengthUnit.Millimeter),
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

            MeasuresSimulated.Name = $"S{slot} simulated STEP DIE result";
            MeasuresSimulated.Information = $"Simulated STEP DIE resuls, NbDies = {NbDiesPresent} NbSite = {xdies.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;

            MeasuresSimulated.Dies = new List<MeasureDieResult>(NbDiesPresent);

            double[] Angles = new double[] { 0.0, 90.0, -90.0, 137.5, 180.0, 255.3 };
            int indang = rnd.Next(0, Angles.Length - 1);
            var scanAngle = new Angle(Angles[indang], AngleUnit.Degree);

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

                            var ptRes = new StepPointResult();
                            ptRes.XPosition = xdies[nx];
                            ptRes.YPosition = ydies[nx];
                            ptRes.ScanAngle = scanAngle;

                            double qsum = 0.0;
                            int MaxStateSeen = -1;
                            for (int k = 0; k < nbRepeta; k++)
                            {
                                var acqdata = new StepPointData();
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
                                    acqdata.StepHeight = new Length(NextGaussian(rnd, acqSettings.StepHeightTarget.Value, 3.0 * acqSettings.StepHeightTolerance.Value), acqSettings.StepHeightTarget.Unit);
                                    if (useScanRawProfilePts != 0)
                                        acqdata.RawProfileScan = GenerateSTEPRawProfile(acqSettings, scanLength, acqdata.StepHeight, useScanRawProfilePts);
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

        #endregion //Generate STEP files

    }
}
