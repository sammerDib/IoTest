using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
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
        #region Generate Periodic Structure files
        private static void GeneratePeriodicStructSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            Random rnd = new Random();

            int nbfile = 6;
            total += nbfile * 25;
            int slot = 0;

            double[] stateproba = new double[] { 0.8, 0.14, 0.05, 0.01 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);

            for (int n = 0; n < nbfile; n++)
            {
                int userawprofiles = 0;
                if (n % 3 == 0)
                    userawprofiles = 121;

                int nbPts = rnd.Next(10, 70);
                int repeta = rnd.Next(1, 4);
                total += nbPts;
                GenRandomptsSite(out double[] xs, out double[] ys, nbPts, 200-10, rnd);
                worker.ReportProgress(CalculateProgress(total, ++complete));

                double h = (400.0 - 20.0) * rnd.NextDouble() + 20.0;
                double w = (800.0 - 250.0) * rnd.NextDouble() + 250.0;
                double he = (10.0 - -10.0) * rnd.NextDouble() - 10.0;
                double we = (20.0 - -20.0) * rnd.NextDouble() - 20.0;

                // success , partial, error, notmeasure
                var ToleranceSet = new PeriodicStructResultSettings()
                {
                    HeightTolerance = new LengthTolerance(h*0.045, LengthToleranceUnit.Micrometer),
                    HeightTarget = new Length(h, LengthUnit.Micrometer),
                    WidthTolerance = new LengthTolerance(w*0.025, LengthToleranceUnit.Micrometer),
                    WidthTarget = new Length(w, LengthUnit.Micrometer),
                };
                var AcqSimSet = new PeriodicStructResultSettings()
                {

                    HeightTolerance = new LengthTolerance((h + he) * 0.05, LengthToleranceUnit.Micrometer),
                    HeightTarget = new Length((h+he), LengthUnit.Micrometer),
                    WidthTolerance = new LengthTolerance((w + we) * 0.03, LengthToleranceUnit.Micrometer),
                    WidthTarget = new Length((w + we), LengthUnit.Micrometer),
                };

                GeneratePeriodicStructSampleFile(ref worker, ref total, ref complete, slot++, 200, repeta, xs, ys, ToleranceSet, AcqSimSet,  stateproba, rnd, userawprofiles);
            }
        }

        private static RawProfile GeneratePeriodicStructRawProfile(int nCount , PeriodicStructResultSettings acqSettings, Length scanLength, Length stepHeight, int nbPoint, Random rnd, double rndStartPos = 0.01)
        {

            RawProfile rawscan = new RawProfile() { XUnit = scanLength.Unit, ZUnit = stepHeight.Unit, RawPoints = new List<RawProfilePoint>(nbPoint)};

            double Zlo_nm = rnd.NextDouble() * (1.0 - 0.5) + 0.5;
            Length Zlow = new Length(Zlo_nm, LengthUnit.Nanometer);

            double sigmaZ = acqSettings.HeightTolerance.GetAbsoluteTolerance(acqSettings.HeightTarget).GetValueAs(stepHeight.Unit);
            double stepstart = rnd.NextDouble() * rndStartPos;
            double stepX = 3.0 * scanLength.Value / (nbPoint - 1);
            double scanPosX = stepstart;

            int nbTotalPoint = nbPoint;
            int oneStartPeriod = nbTotalPoint / (3 * (nCount+1));
            int oneEndPeriod = 2 * oneStartPeriod;

            for (int iter = 0; iter < nCount; iter++)
            {
                for (int i = 0; i < 3 * oneStartPeriod; i++)
                {
                    bool isLo = (i < oneStartPeriod) || (i > oneEndPeriod);

                    double zc = isLo ? Zlow.GetValueAs(stepHeight.Unit) : stepHeight.GetValueAs(stepHeight.Unit);
                    double zalea = NextGaussian(rnd, zc, sigmaZ);

                    rawscan.RawPoints.Add(new RawProfilePoint() { X = scanPosX, Z = zalea });

                    scanPosX += stepX;
                }
            }
            return rawscan;
        }

        private static void GeneratePeriodicStructSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, PeriodicStructResultSettings toleranceSettings, PeriodicStructResultSettings acqSettings, double[] stateProba, Random rnd, int useScanRawProfilePts = 0)
        {
            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anaperiodic proba sum state is not equal to 1.0");
                return;
            }

            ResultType restype = ResultType.ANALYSE_PeriodicStructure;
            var res = new MetroResult(restype);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{restype.GetExt()}");

            var MeasuresSimulated = new PeriodicStructResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated PeriodicStruct result";
            MeasuresSimulated.Information = $"Simulated PeriodicStruct resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);

            double[] Angles = new double[] { 0.0, 90.0, -90.0, 137.5, 180.0, 255.3 };
            int indang = rnd.Next(0, Angles.Length - 1);
            var scanAngle = new Angle(Angles[indang], AngleUnit.Degree);

            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            int nStructCount = rnd.Next(4, 15);
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new PeriodicStructPointResult();
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];
                ptRes.ScanAngle = scanAngle;

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new PeriodicStructPointData();
                    acqdata.IndexRepeta = k;
                    acqdata.StructCount = nStructCount;

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
                        acqdata.Height = new Length(NextGaussian(rnd, acqSettings.HeightTarget.Value, 3.0 * acqSettings.HeightTolerance.Value), acqSettings.HeightTarget.Unit);
                        acqdata.Width = new Length(NextGaussian(rnd, acqSettings.WidthTarget.Value, 3.0 * acqSettings.WidthTolerance.Value), acqSettings.WidthTarget.Unit);
                        acqdata.Pitch = new Length(acqdata.Width.Micrometers + 5.0 , LengthUnit.Micrometer);
                        for (int nS = 0; nS < acqdata.StructCount; nS++)
                        { 
                            acqdata.StructHeights.Add(new Length(NextGaussian(rnd, acqSettings.HeightTarget.Value, 3.0 * acqSettings.HeightTolerance.Value), acqSettings.HeightTarget.Unit));
                            acqdata.StructWidths.Add(new Length(NextGaussian(rnd, acqSettings.WidthTarget.Value, 3.0 * acqSettings.WidthTolerance.Value), acqSettings.WidthTarget.Unit));
                        }
                        if (useScanRawProfilePts != 0)
                            acqdata.RawProfileScan = GeneratePeriodicStructRawProfile(acqdata.StructCount, acqSettings, acqdata.Width, acqdata.Height, useScanRawProfilePts, rnd);
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
                MessageBox.Show($"Cannot generate S{slot}.anaperiodicstric :\n{serrror}");
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
        #endregion //Generate PeriodicStruct files

    }
}
