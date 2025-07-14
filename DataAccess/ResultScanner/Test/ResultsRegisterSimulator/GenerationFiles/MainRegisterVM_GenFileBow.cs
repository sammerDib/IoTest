using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Tools.Units;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {
        #region Generate Bow files
        private static void GenerateBOWSampleFiles(ref BackgroundWorker worker, ref int total, ref int complete)
        {
            total += 3 + 5 * 2; // normal measures points

            // le centre, puisque le bow n'a qu'un seul point de repere (au lieu de 5) vs. Generation Trench
            double[] xs_site = { 0.0 }; 
            double[] ys_site = { 0.0 };

            worker.ReportProgress(CalculateProgress(total, ++complete));
           
            int slot = 0;
            // success , partial, error, notmeasure
            var ToleranceSet = new BowResultSettings()
            {
                BowTargetMax = new Length(110, LengthUnit.Micrometer),
                BowTargetMin = new Length(25, LengthUnit.Micrometer)
            };
            var AcqSimSet = new BowResultSettings()
            {
                BowTargetMax = new Length(111, LengthUnit.Micrometer),
                BowTargetMin = new Length(10, LengthUnit.Micrometer)
            };

            double[] stateproba = new double[] { 0.8, 0.15, 0.05, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            // slot 0 
            GenerateBOWSampleFile(ref worker, ref total, ref complete, slot++, 200, 1, xs_site, ys_site, ToleranceSet, AcqSimSet, stateproba);

            // slot 1 
            AcqSimSet.BowTargetMax = ToleranceSet.BowTargetMax;
            AcqSimSet.BowTargetMin = ToleranceSet.BowTargetMin;
            GenerateBOWSampleFile(ref worker, ref total, ref complete, slot++, 200, 3, xs_site, ys_site, ToleranceSet, AcqSimSet, stateproba);

            // slot 2  
            stateproba = new double[] { 0.65, 0.19, 0.15, 0.01 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateBOWSampleFile(ref worker, ref total, ref complete, slot++, 200, 5, xs_site, ys_site, ToleranceSet, AcqSimSet, stateproba);

            // slot 3 
            var ToleranceSet2 = new BowResultSettings()
            {
                BowTargetMax = new Length(160, LengthUnit.Micrometer),
                BowTargetMin = new Length(80, LengthUnit.Micrometer)
            };

            var AcqSimSet2 = new BowResultSettings()
            {
                BowTargetMax = new Length(240, LengthUnit.Micrometer),
                BowTargetMin = new Length(45, LengthUnit.Micrometer)
            };
            stateproba = new double[] { 0.9, 0.09, 0.01, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateBOWSampleFile(ref worker, ref total, ref complete, slot++, 300, 1, xs_site, ys_site, ToleranceSet2, AcqSimSet2, stateproba);

            // slot 4 
            stateproba = new double[] { 0.4, 0.35, 0.25, 0.0 };
            System.Diagnostics.Debug.Assert(stateproba.Sum() == 1.0);
            GenerateBOWSampleFile(ref worker, ref total, ref complete, slot++, 300, 4, xs_site, ys_site, ToleranceSet2, AcqSimSet2, stateproba);
        }

        private static void GenerateBOWSampleFile(ref BackgroundWorker worker, ref int total, ref int complete, int slot, int waferdiameter_mm, int nbRepeta, double[] xs, double[] ys, BowResultSettings toleranceSettings, BowResultSettings acqSettings, double[] stateProba)
        {
            double probsum = stateProba.Sum();
            if (probsum != 1.0)
            {
                MessageBox.Show($"S{slot} anabow proba sum state is not equal to 1.0");
                return;
            }

            var rnd = new Random();

            var res = new MetroResult(ResultType.ANALYSE_Bow);
            res.ResFilePath = Path.Combine(GeneratePath, $"S{slot}.{ResultType.ANALYSE_Bow.GetExt()}");

            var MeasuresSimulated = new BowResult();
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

            MeasuresSimulated.Name = $"S{slot} simulated BOW result";
            MeasuresSimulated.Information = $"Simulated BOW resuls, NbSite = {xs.Length}, NbRepeta = {nbRepeta}";
            MeasuresSimulated.Settings = toleranceSettings;
            MeasuresSimulated.Points = new List<MeasurePointResult>(xs.Length);
 
            double[] Qmax = new double[] { 1.0, 0.8, 0.7, 0.2 };
            double[] Qmin = new double[] { 0.85, 0.5, 0.0, 0.0 };
            bool bAllNotMeasured = true;
            for (int i = 0; i < xs.Length; i++)
            {
                var ptRes = new BowPointResult();
                ptRes.XPosition = xs[i];
                ptRes.YPosition = ys[i];

                double qsum = 0.0;
                bool bHasSomeNotMeasured = false;
                int MaxStateSeen = -1;
                
                for (int k = 0; k < nbRepeta; k++)
                {
                    var acqdata = new BowTotalPointData();
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
                        acqdata.Bow = new Length(NextGaussian(rnd, ((acqSettings.BowTargetMax.Micrometers + acqSettings.BowTargetMin.Micrometers)/2), ((acqSettings.BowTargetMax.Micrometers - acqSettings.BowTargetMin.Micrometers) / 2)), acqSettings.BowTargetMax.Unit);
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
                MessageBox.Show($"Cannot generate S{slot}.anabow :\n{serrror}");
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

        #endregion //Generate BOW files

    }
}
