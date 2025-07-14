using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matrox.MatroxImagingLibrary;
using UnitySC.PM.PSD.Service.Implementation.Curvature;
using UnitySC.PM.Shared.Data.ProcessingImage;
using UnitySC.PM.Shared.LibMIL;
using static UnitySC.PM.PSD.Service.Implementation.Curvature.NativeMethods;

namespace CalculationTest
{
    class Program
    {

        private static long _instanceId = -1;

        private static List<ProcessingImage> images;

        static void Main(string[] args)
        {
            //_instanceId = NativeMethods.CreateNewInstance(Mil.Instance.HostSystem.MilId, _globalTopo.InstanceId);
            Mil.Instance.Allocate();
            _instanceId = NativeMethods.CreateNewInstance(Mil.Instance.HostSystem.MilId, -1L);

            FILTER_CONFIG filterConfig = new FILTER_CONFIG()
            {
                Contraste_min_amplitude = 15,
                Intensite_min_amplitude = 10,
                Contraste_min_curvature = 15,
                Intensite_min_curvature = 10
            };
            if (!NativeMethods.SetFilterConfig(_instanceId, filterConfig))
                throw new Exception("Unable to set Filter Config");

            REPORT_CONFIG reportConfig = new REPORT_CONFIG()
            {
                min_curvature = -0.03,
                max_curvature = 0.03,
                Use_Sigma = 0,
                Sigma = 1
            };
            if (!NativeMethods.SetReportConfig(_instanceId, reportConfig))
                throw new Exception("Unable to set Report Config");


            NativeMethods.TypeOfFrame typeOfFrame = 0;
            typeOfFrame |= (NativeMethods.TypeOfFrame.CurvatureX | NativeMethods.TypeOfFrame.CurvatureY);
    
            NativeMethods.INPUT_INFO ii = new NativeMethods.INPUT_INFO();
            ii.NbPeriods = 1;
            ii.NbImgX = ii.NbImgY = 15;
            ii.SizeX = 16384; // width;
            ii.SizeY = 9440; // height;

            ii.TypeOfFrame = typeOfFrame;

            if (!NativeMethods.SetInputInfo(_instanceId, ii, new List<int>() { 32 }.ToArray()))
                throw new Exception("Unable to set input info");



            // Load Images
            images = new List<ProcessingImage>();
            
            for (int i = 0; i < 30; i++)
            {
                var procImage = new ProcessingImage();
                procImage.Load($@"C:\temp\Bug Deflecto 15\WaferTest_S99_fringe-{i}.tif");
                //images.Add(procImage);
                NativeMethods.SetInputImage(_instanceId, procImage.GetMilImage(), i);
            }

            //IncrementalCalculate(images, 32, 'X');

            if (!NativeMethods.PerformIncrementalCalculation(_instanceId, 0, 'X'))
                throw new Exception("Unable to process images");

            var curvatureX= GetResultImage(8, TypeOfFrame.CurvatureX);

            curvatureX.Save(@"C:\temp\Bug Deflecto 15\ResultCalc\CurvatureX.tif");

            var curvatureY = GetResultImage(8, TypeOfFrame.CurvatureY);

            curvatureY.Save(@"C:\temp\Bug Deflecto 15\ResultCalc\CurvatureY.tif");
        }

        static void IncrementalCalculate(List<ProcessingImage> images, int period, char direction)
        {
            for (int k = 0; k <30; k++)
            {
                 if (k >= images.Count)
                    throw new Exception("Unable to set image " + k);

                if (!NativeMethods.SetInputImage(_instanceId, images[k].GetMilImage(), k))
                    throw new Exception("Unable to set image " + k);
            }

            // Calcul
            //.......
            if (!NativeMethods.PerformIncrementalCalculation(_instanceId, period, direction))
                throw new Exception("Unable to process images");
        }


        public static ProcessingImage GetResultImage(int depth, TypeOfFrame imageType, int periodIndex = 0)
        {
            var milid = NativeMethods.GetResultImage(_instanceId, imageType, periodIndex);
            using (MilImage milTemp = new MilImage(milid, transferOnwership: true))
            {
                ProcessingImage procimg = new ProcessingImage();
                if (milTemp.SizeBit == depth)
                {
                    procimg.SetMilImage(milTemp);
                }
                else
                {
                    MilImage milImage = procimg.GetMilImage();
                    milImage.Alloc2d(milTemp.SizeX, milTemp.SizeY, depth + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                    MilImage.Copy(milTemp, milImage);
                }
               
                return procimg;
            }
        }
    }


  
}
