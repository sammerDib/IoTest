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
using System.Drawing;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base;

namespace ResultsRegisterSimulator
{
    public partial class MainRegisterVM : ObservableRecipient
    {

        #region Generate Common
        public static readonly string BaseGeneratePath = @".\GeneratedFiles";
        public static string GeneratePath;
        public static double Rad(double deg) { return (Math.PI / 180.0) * deg; }
        public static double Deg(double rad) { return (180.0 / Math.PI) * rad; }
        public static double NextGaussian(Random r, double mu = 0, double sigma = 1)
        {
            double u1 = 1.0 - r.NextDouble();
            double u2 = 1.0 - r.NextDouble();
            double rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double rand_normal = mu + sigma * rand_std_normal;
            return rand_normal;
        }

        private static void Gen5ptsSite(out double[] xs, out double[] ys)
        {
            double[] ro_mm = new double[] { 0, 40, 90, 75, 62 };
            double[] ang_dg = new double[] { 0, -45, 45, 135, 225 };
            xs = new double[ro_mm.Length];
            ys = new double[ro_mm.Length];
            for (int i = 0; i < ro_mm.Length; i++)
            {
                xs[i] = ro_mm[i] * Math.Cos(Rad(ang_dg[i]));
                ys[i] = ro_mm[i] * Math.Sin(Rad(ang_dg[i]));
            }
        }

        private static void Gen10ptsSite(out double[] xs, out double[] ys)
        {
            double[] ro_mm = new double[] { 75, 75, 75, 75, 78, 135, 135, 135, 135, 0 };
            double[] ang_dg = new double[] { 0, 90, 180, 270, 2, 45, 135, 225, 315, 0 };
            xs = new double[ro_mm.Length];
            ys = new double[ro_mm.Length];
            for (int i = 0; i < ro_mm.Length; i++)
            {
                xs[i] = ro_mm[i] * Math.Cos(Rad(ang_dg[i]));
                ys[i] = ro_mm[i] * Math.Sin(Rad(ang_dg[i]));
            }

        }

        private static void Gen40ptsSite(out double[] xs, out double[] ys)
        {
            xs = new double[40];
            ys = new double[40];

            int nk = 0;
            xs[nk] = 0.0;
            ys[nk++] = 0.0;

            for (int i = 0; i < 4; i++)
            {
                double rho_mm = 5.0;
                double ang_dg = 45.0;
                double ang_step_dg = 90.0;
                xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
            }

            for (int i = 0; i < 10; i++)
            {
                double rho_mm = 30.0;
                double ang_dg = 0.0;
                double ang_step_dg = 36.0;
                xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
            }

            for (int i = 0; i < 12; i++)
            {
                double rho_mm = 75.0;
                double ang_dg = 10.0;
                double ang_step_dg = 30.0;
                xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
            }

            for (int i = 0; i < 8; i++)
            {
                double rho_mm = 100.0;
                double ang_dg = 0.0;
                double ang_step_dg = 45.0;
                xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
            }

            for (int i = 0; i < 4; i++)
            {
                double rho_mm = 140.0;
                double ang_dg = 45.0;
                double ang_step_dg = 90.0;
                xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
            }

            xs[nk] = 150.0 * Math.Cos(Rad(-90.0));
            ys[nk++] = 150.0 * Math.Sin(Rad(-90.0));

        }

        private static void Gen20ptsSite(out double[] xs, out double[] ys)
        {
            xs = new double[20];
            ys = new double[20];

            int nk = 0;
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 4; i++)
                {
                    double rho_mm = 10.0 + j * 10.0;
                    double ang_dg = 0;
                    double ang_step_dg = 90.0;
                    xs[nk] = rho_mm * Math.Cos(Rad(ang_dg + i * ang_step_dg));
                    ys[nk++] = rho_mm * Math.Sin(Rad(ang_dg + i * ang_step_dg));
                }
            }
        }

        private static void GenRandomptsSite(out double[] xs, out double[] ys, int nbPts, double waferdiameter_mm, Random random = null)
        {
            Random rnd;
            if (random != null)
                rnd = random;
            else
                rnd = new Random();

            xs = new double[nbPts];
            ys = new double[nbPts];
            for (int i = 0; i < nbPts; i++)
            {
                double ro_mm = rnd.NextDouble() * waferdiameter_mm * 0.5;
                double ang_dg = rnd.NextDouble() * 360.0;
                xs[i] = ro_mm * Math.Cos(Rad(ang_dg));
                ys[i] = ro_mm * Math.Sin(Rad(ang_dg));
            }
        }

        #endregion //Generate Common

        #region Generate Execution
        private void BkgWorker_GenerateResFile(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            GenerateSampleFiles(worker);
        }

        public static void GenerateSampleFiles(BackgroundWorker worker)
        {
            GeneratePath = Path.Combine(BaseGeneratePath, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            Directory.CreateDirectory(GeneratePath);

            int total = 1;
            int complete = 0;
            worker.ReportProgress(0);

            GenerateTSVSampleFiles(ref worker, ref total, ref complete);

            GenerateNTPSampleFiles(ref worker, ref total, ref complete);

            GenerateThicknessSampleFiles(ref worker, ref total, ref complete);

            GenerateTOPOSampleFiles(ref worker, ref total, ref complete);

            GenerateSTEPSampleFiles(ref worker, ref total, ref complete);

            GenerateTRENCHSampleFiles(ref worker, ref total, ref complete);

            GeneratePillarSampleFiles(ref worker, ref total, ref complete);

            GeneratePeriodicStructSampleFiles(ref worker, ref total, ref complete);

            GenerateBOWSampleFiles(ref worker, ref total, ref complete);

            try
            { Directory.Delete(Path.Combine(GeneratePath, "LotThumbnail")); }
            catch
            { }

            complete++;
            worker.ReportProgress(CalculateProgress(total, complete));

            System.Diagnostics.Process.Start("explorer.exe", GeneratePath);
        }
        #endregion
    }
}
