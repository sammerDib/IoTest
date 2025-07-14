using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
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
        #region Generate NTP files
        private static void GenerateNTPSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            total += 3 + 5 * 1 + 20 * 3 + 10 * 3; // normal measures points

            Gen5ptsSite(out double[] xs_5pts, out double[] ys_5pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            Gen20ptsSite(out double[] xs_20pts, out double[] ys_20pts);
            worker.ReportProgress(CalculateProgress(total, ++complete));
            GenRandomptsSite(out double[] xs_10pts, out double[] ys_10pts, 10, 200);
            worker.ReportProgress(CalculateProgress(total, ++complete));

            var otpA = new ExternalProcessingOutput()
            { Name = "OutA", OutputTarget = 100.0, OutputTolerance = new Tolerance(10.0, ToleranceUnit.AbsoluteValue) };
            var otpB = new ExternalProcessingOutput()
            { Name = "Output B", OutputTarget = 0.25, OutputTolerance = new Tolerance(0.01, ToleranceUnit.AbsoluteValue) };
            var otpC = new ExternalProcessingOutput()
            { Name = "Mesure Style C", OutputTarget = 156.56, OutputTolerance = new Tolerance(20.0, ToleranceUnit.Percentage) };
            var otpD = new ExternalProcessingOutput()
            { Name = "Super Mesure D", OutputTarget = 100.0, OutputTolerance = new Tolerance(5.0, ToleranceUnit.Percentage) };


            int slot = 0;
            // success , partial, error, notmeasure
            var ToleranceSet = new NanoTopoResultSettings()
            {
                RoughnessTarget = new Length(50.0, LengthUnit.Nanometer),
                RoughnessTolerance = new LengthTolerance(5.0, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(250.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(15.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB, otpC, otpD }
            };
            var AcqSimSet = new NanoTopoResultSettings()
            {
                RoughnessTarget = new Length(52.0, LengthUnit.Nanometer),
                RoughnessTolerance = new LengthTolerance(3.0, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(240.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB, otpC, otpD }
            };
            double[] stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            // slot 0 // r, sh, o4
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 200, 1, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 1 // r, sh, o2
            AcqSimSet.RoughnessTarget = new Length(48.0, LengthUnit.Nanometer);
            AcqSimSet.RoughnessTolerance = new LengthTolerance(2.0, LengthToleranceUnit.Nanometer);
            ToleranceSet.ExternalProcessingOutputs.Remove(otpA); AcqSimSet.ExternalProcessingOutputs.Remove(otpA);
            ToleranceSet.ExternalProcessingOutputs.Remove(otpD); AcqSimSet.ExternalProcessingOutputs.Remove(otpD);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 200, 3, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba, true);

            // slot 2  // r, - , o3
            AcqSimSet.RoughnessTolerance = new LengthTolerance(3.0, LengthToleranceUnit.Nanometer);
            ToleranceSet.StepHeightTarget = null; AcqSimSet.StepHeightTarget = null;
            ToleranceSet.StepHeightTolerance = null; AcqSimSet.StepHeightTolerance = null;
            ToleranceSet.ExternalProcessingOutputs.Add(otpA); AcqSimSet.ExternalProcessingOutputs.Add(otpA);
            stateproba = new double[] { 0.65, 0.19, 0.15, 0.01 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 200, 5, xs_10pts, ys_10pts, ToleranceSet, AcqSimSet, stateproba);

            // slot 3 // -, sh , o1
            var ToleranceSet2 = new NanoTopoResultSettings()
            {
                RoughnessTarget = null,
                RoughnessTolerance = null,
                StepHeightTarget = new Length(570.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(30.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpD }
            };

            var AcqSimSet2 = new NanoTopoResultSettings()
            {
                RoughnessTarget = null,
                RoughnessTolerance = null,
                StepHeightTarget = new Length(570.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(30.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpD }
            };
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 150, 1, xs_20pts, ys_20pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 4 // -, sh , o1
            stateproba = new double[] { 0.4, 0.35, 0.25, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 150, 4, xs_20pts, ys_20pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 5 // -, sh , -
            AcqSimSet2.StepHeightTarget = new Length(556.0, LengthUnit.Nanometer);
            AcqSimSet2.StepHeightTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Nanometer);
            AcqSimSet2.ExternalProcessingOutputs = null;
            stateproba = new double[] { 0.1, 0.0, 0.5, 0.4 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 150, 2, xs_20pts, ys_20pts, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 6 // -, -, o2
            var ToleranceSet3 = new NanoTopoResultSettings()
            {
                RoughnessTarget = null,
                RoughnessTolerance = null,
                StepHeightTarget = null,
                StepHeightTolerance = null,
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB }
            };
            var AcqSimSet3 = new NanoTopoResultSettings()
            {
                RoughnessTarget = null,
                RoughnessTolerance = null,
                StepHeightTarget = null,
                StepHeightTolerance = null,
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB }
            };
            stateproba = new double[] { 0.75, 0.2, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateNTPSampleFile(ref worker, ref total, ref complete, slot++, 200, 3, xs_5pts, ys_5pts, ToleranceSet3, AcqSimSet3, stateproba);

            //Slot 7 - Die
            var ToleranceSet4 = new NanoTopoResultSettings()
            {
                RoughnessTarget = new Length(12.0, LengthUnit.Nanometer),
                RoughnessTolerance = new LengthTolerance(5.0, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(175.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(15.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB }
            };
            var AcqSimSet4 = new NanoTopoResultSettings()
            {
                RoughnessTarget = new Length(13.0, LengthUnit.Nanometer),
                RoughnessTolerance = new LengthTolerance(3.0, LengthToleranceUnit.Nanometer),
                StepHeightTarget = new Length(165.0, LengthUnit.Nanometer),
                StepHeightTolerance = new LengthTolerance(10.0, LengthToleranceUnit.Nanometer),
                ExternalProcessingOutputs = new List<ExternalProcessingOutput>() { otpA, otpB }
            };
            stateproba = new double[] { 0.7, 0.2, 0.09, 0.01 };
            System.Diagnostics.Debug.Assert((stateproba.Sum() - 1.0) <= 0.00001);

            //Slot 7 - Die - r sp 02
            double DieSizW_mm = 1.5;
            double DieSizH_mm = 1.0;
            double[] xdies_4pts = new double[] { 0.1, 0.75, 1.2, 1.3 };
            double[] ydies_4pts = new double[] { 0.9, 0.75, 0.95, 0.05 };

            GenerateNTPDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 60, 3, DieSizW_mm, DieSizH_mm, 15.0,
                 xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, false, true);

            //Slot 8 - Die - r sp o2
            xdies_4pts = new double[] { 0.2, 0.7, 0.5, 1.4 };
            ydies_4pts = new double[] { 0.8, 0.3, 0.5, 1.0 };
            GenerateNTPDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 60, 1, DieSizW_mm, DieSizH_mm, 20.0,
                 xdies_4pts, ydies_4pts, ToleranceSet4, AcqSimSet4, stateproba, false, true);

            //Slot 9 - Big Die - r sp o2 - thumbnails 3D
            DieSizW_mm = 32.5;
            DieSizH_mm = 32.5;
            double[] xdies_2pts = new double[] { 0.75 * DieSizW_mm, 0.25 * DieSizW_mm, };
            double[] ydies_2pts = new double[] { 0.75 * DieSizH_mm, 0.25 * DieSizH_mm, };
            GenerateNTPDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 75, 5, DieSizW_mm, DieSizH_mm, 0.034,
                xdies_2pts, ydies_2pts, ToleranceSet4, AcqSimSet4, stateproba, true, true);

            //Slot 10 - Big Die - r sp o2 - numerous repeta No thumbs
            GenerateNTPDieSampleFile(ref worker, ref total, ref complete, slot++, 150, 75, 20, DieSizW_mm, DieSizH_mm, 0.034,
              xdies_2pts, ydies_2pts, ToleranceSet4, AcqSimSet4, stateproba, false, true);

        }

        private static void GenerateNTPSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, NanoTopoResultSettings toleranceSettings, NanoTopoResultSettings acqSettings, double[] stateProba, bool genreport = false)
        {

            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anantp proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_NanoTopo);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_NanoTopo.GetExt()}");

            string NtpReportPath = string.Empty;
            if (genreport)
            {
                NtpReportPath = Path.Combine(GeneratePath, $"S{slot}_NTP");
                Directory.CreateDirectory(NtpReportPath);
            }

            var MeasuresSimulated = new NanoTopoResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated NTP result";
            MeasuresSimulated.Information = $"Simulated NTP resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new NanoTopoPointResult();
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new NanoTopoPointData();
                    acqdata.IndexRepeta = k;

                    if (genreport)
                    {
                        int num = k % 3 + 1;
                        acqdata.ReportFileName = Path.Combine($".\\S{slot}_NTP", $"REPORT{num}.pdf");
                    }

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

                        if (acqSettings.RoughnessTarget != null && acqSettings.RoughnessTolerance != null)
                            acqdata.Roughness = new Length(NextGaussian(rnd, acqSettings.RoughnessTarget.Nanometers, 3.0 * acqSettings.RoughnessTolerance.Value), LengthUnit.Nanometer);

                        if (acqSettings.StepHeightTarget != null && acqSettings.StepHeightTolerance != null)
                            acqdata.StepHeight = new Length(NextGaussian(rnd, acqSettings.StepHeightTarget.Nanometers, 3.0 * acqSettings.StepHeightTolerance.Value), LengthUnit.Nanometer);

                        if (acqSettings.ExternalProcessingOutputs != null)
                        {
                            acqdata.ExternalProcessingResults = new List<ExternalProcessingResult>(acqSettings.ExternalProcessingOutputs.Count);
                            foreach (var output in acqSettings.ExternalProcessingOutputs)
                            {
                                acqdata.ExternalProcessingResults.Add(new ExternalProcessingResult()
                                {
                                    Name = output.Name,
                                    Value = NextGaussian(rnd, output.OutputTarget, 3.0 * output.OutputTolerance.Value)
                                });
                            }
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
                MessageBox.Show($"Cannot generate S{slot}.anantp :\n{serrror}");
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

        private static void GenerateNTPDieSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot,
            int waferdiameter_mm, int maxradius_mm, int nbRepeta,
            double dieSizeW_mm, double dieSizeH_mm, double testdiesigma,
            double[] xdies, double[] ydies,
            NanoTopoResultSettings toleranceSettings, NanoTopoResultSettings acqSettings, double[] stateProba, bool genThumbnails = false, bool computeDieGridXY = false)
        {
            double probsum = stateProba.Sum();
            if ((probsum - 1.0) > 0.00001)
            {
                MessageBox.Show($"S{slot} anantp proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_NanoTopo);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_NanoTopo.GetExt()}");

            string NtpThumb3DPath = string.Empty;
            if (genThumbnails)
            {
                NtpThumb3DPath = Path.Combine(GeneratePath, $"S{slot}_NTP_Thumbs");
                Directory.CreateDirectory(NtpThumb3DPath);
            }

            var MeasuresSimulated = new NanoTopoResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated NTP DIE result";
            MeasuresSimulated.Information = $"Simulated NTP DIE resuls, NbDies = {NbDiesPresent} NbSite = {xdies.Length}, NbRepeta = {nbRepeta}";
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

                            var ptRes = new NanoTopoPointResult();
                            ptRes.XPosition = xdies[nx];
                            ptRes.YPosition = ydies[nx];

                            double qsum = 0.0;
                            int MaxStateSeen = -1;
                            for (int k = 0; k < nbRepeta; k++)
                            {
                                var acqdata = new NanoTopoPointData();
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

                                    if (acqSettings.RoughnessTarget != null && acqSettings.RoughnessTolerance != null)
                                        acqdata.Roughness = new Length(NextGaussian(rnd, acqSettings.RoughnessTarget.Nanometers, 3.0 * acqSettings.RoughnessTolerance.Value), LengthUnit.Nanometer);

                                    if (acqSettings.StepHeightTarget != null && acqSettings.StepHeightTolerance != null)
                                        acqdata.StepHeight = new Length(NextGaussian(rnd, acqSettings.StepHeightTarget.Nanometers, 3.0 * acqSettings.StepHeightTolerance.Value), LengthUnit.Nanometer);

                                    if (acqSettings.ExternalProcessingOutputs != null)
                                    {
                                        acqdata.ExternalProcessingResults = new List<ExternalProcessingResult>(acqSettings.ExternalProcessingOutputs.Count);
                                        foreach (var output in acqSettings.ExternalProcessingOutputs)
                                        {
                                            acqdata.ExternalProcessingResults.Add(new ExternalProcessingResult()
                                            {
                                                Name = output.Name,
                                                Value = NextGaussian(rnd, output.OutputTarget, 3.0 * output.OutputTolerance.Value)
                                            });
                                        }
                                    }

                                    if (genThumbnails)
                                    {
                                        int thumbW_px = 150; int thumbH_px = 98;
                                        var rc = new Rectangle(thumbW_px / 6, thumbH_px / 7, thumbW_px * 4 / 6, thumbH_px * 5 / 7);
                                        float bngbase = (float)NextGaussian(rnd, 184.0, 4.0);
                                        float[] thumbdata3d = new float[thumbW_px * thumbH_px];

                                        float a = (float)rnd.Next(4, 8);
                                        float b = (float)rnd.Next(4, 8);

                                        int ndx = rnd.Next(-10, 10) - (rc.X + rc.Width / 2);
                                        int ndy = rnd.Next(-10, 10) - (rc.Y + rc.Height / 2);

                                        for (int pidx = 0; pidx < thumbdata3d.Length; pidx++)
                                        {
                                            int x = pidx % thumbW_px;
                                            int y = pidx / thumbW_px;

                                            if (rc.Contains(x, y))
                                            {
                                                float xp = (float)(x + ndx) / 8.0f;
                                                float yp = (float)(y + ndy) / 8.0f;

                                                thumbdata3d[pidx] = bngbase * (1.5f + (float)((rnd.NextDouble() - 0.5) * 0.075)) - ((xp * xp / a * a) + (yp * yp / b * b));
                                            }
                                            else
                                            {
                                                thumbdata3d[pidx] = bngbase * (1.0f + (float)(rnd.NextDouble() * 0.01));
                                            }
                                        }

                                        using (var format3daFile = new MatrixFloatFile())
                                        {
                                            var headerdata = new MatrixFloatFile.HeaderData(thumbH_px, thumbW_px, new MatrixFloatFile.AdditionnalHeaderData(1.0, 1.0, "px", "px", "nm"));
                                            format3daFile.WriteInFile(Path.Combine(NtpThumb3DPath, $"{i}_{j}_{nx}_{k}.3da"), headerdata, thumbdata3d, true);
                                        }

                                        acqdata.ResultImageFileName = Path.Combine($".\\S{slot}_NTP_Thumbs", $"{i}_{j}_{nx}_{k}.3da");
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

        #endregion //Generate NTP files

    }
}
