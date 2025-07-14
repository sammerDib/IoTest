using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Data.Extensions;

using UnitySCPMSharedAlgosVSIWrapper;

namespace VSIWrapperTests
{

    [TestClass]
    public class UnitVSIWrapperTest
    {
        private static byte[] LoadImgFile(string imgPath, string extension, ref int width, ref int height)
        {
            Stream imageStreamSource = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = null;
            switch (extension)
            {
                case ".png":
                    decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".bmp":
                    decoder = new BmpBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;

                case ".tif":
                case ".tiff":
                    decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    break;
                default:
                    throw new ApplicationException("unkwown file extension: " + extension);
            }

            BitmapSource img = decoder.Frames[0];
            byte[] data = img.ConvertToByteArray();
            imageStreamSource.Close();

            return data;
        }

        private string GetVSIDataFolder()
        {
            string workingDirectory = AppContext.BaseDirectory;
            string testDirectory;
            if (workingDirectory.Contains("x64"))
                testDirectory = Directory.GetParent(workingDirectory).Parent?.Parent?.Parent?.FullName;
            else
                testDirectory = Directory.GetParent(workingDirectory).Parent?.Parent?.FullName;
            string dataDirectory = testDirectory + "\\Data\\";

            return dataDirectory;
        }

        private List<byte[]> GetImageStack(string folderName, int nbOfImagesToGet, ref int width, ref int height)
        {
            List<byte[]> imageStackList = new List<byte[]>();

            DirectoryInfo d = new DirectoryInfo(GetVSIDataFolder() + folderName);
            FileInfo[] filePaths = d.GetFiles().OrderBy(file => Regex.Replace(file.Name, @"\d+", match => match.Value.PadLeft(4, '0'))).ToArray();

            if (nbOfImagesToGet == -1) nbOfImagesToGet = filePaths.Length;

            for (int i = 0; i < nbOfImagesToGet; i++)
            {
                if (filePaths[i].Extension != ".png")
                    continue;
                byte[] byteArray = LoadImgFile(filePaths[i].FullName, filePaths[i].Extension, ref width, ref height);
                imageStackList.Add(byteArray);
            }

            return imageStackList;
        }

        [TestMethod]
        public void Expect_VSI_to_work_with_good_parameters()
        {
            //Take all images
            int numberOfImagesToTake = -1;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count > 0);

            byte[][] imageStack = imageStackList.ToArray();

            double ruleStep = 50.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.6;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            Assert.AreEqual(vsiOutput.Status, StatusCode.OK);

        }

        [TestMethod]
        public void Expect_VSI_to_fail_with_the_wrong_parameters()
        {
            //Take all images
            int numberOfImagesToTake = -1;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count > 0);

            byte[][] imageStack = imageStackList.ToArray();

            //WRONG RULE STEP HERE ON PURPOSE
            double ruleStep = 90.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.6;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            Assert.AreEqual(vsiOutput.Status, StatusCode.DF_SRCWAVELENGTH_ERROR);

        }

        [TestMethod]
        public void Expect_VSI_to_fail_with_less_than_5_images()
        {
            //Only take 4 images
            int numberOfImagesToTake = 4;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count == numberOfImagesToTake);

            byte[][] imageStack = imageStackList.ToArray();

            double ruleStep = 50.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.6;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            Assert.AreEqual(vsiOutput.Status, StatusCode.NOT_ENOUGHIMAGES);

        }

        [TestMethod]
        public void Expect_a_high_mask_to_return_a_high_percentage_of_NaNs()
        {
            //Take all images
            int numberOfImagesToTake = -1;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count > 0);

            byte[][] imageStack = imageStackList.ToArray();

            double ruleStep = 50.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.99;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            int nanCount = 0;

            foreach (var value in vsiOutput.ResultArray)
            {
                if (float.IsNaN(value))
                {
                    nanCount++;
                }
            }

            double actualNaNPercentage = (double)nanCount / vsiOutput.ResultArray.Length;
            double expectedNaNPercentage = 0.99;
            double percentageTolerance = 0.01;

            Assert.AreEqual(expectedNaNPercentage, actualNaNPercentage, percentageTolerance);
        }

        [TestMethod]
        public void Expect_a_low_mask_to_return_a_high_low_of_NaNs()
        {
            //Take all images
            int numberOfImagesToTake = -1;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count > 0);

            byte[][] imageStack = imageStackList.ToArray();

            double ruleStep = 50.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.20;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            int nanCount = 0;

            foreach (var value in vsiOutput.ResultArray)
            {
                if (float.IsNaN(value))
                {
                    nanCount++;
                }
            }

            double actualNaNPercentage = (double)nanCount / vsiOutput.ResultArray.Length;
            double expectedNaNPercentage = 0.01;
            double percentageTolerance = 0.01;

            Assert.AreEqual(expectedNaNPercentage, actualNaNPercentage, percentageTolerance);
        }

        [TestMethod]
        public void Expect_no_mask_to_return_no_NaNs()
        {
            //Take all images
            int numberOfImagesToTake = -1;

            int width = 0;
            int height = 0;

            string folderName = "VSI_crop";

            List<byte[]> imageStackList = GetImageStack(folderName, numberOfImagesToTake, ref width, ref height);

            Assert.IsTrue(imageStackList.Count > 0);

            byte[][] imageStack = imageStackList.ToArray();

            double ruleStep = 50.0e-9;
            double lambdaCenter = 630e-9;     // wavelength [m]
            double fwhmLambda = 124e-09;    // spectral bandwidth [m]
            double noiseLevel = 0.5;          // noise [LSB]

            double maskThreshold = 0.0;

            VSIOutput vsiOutput = VSI.ComputeTopography(imageStack, width, height, ruleStep, lambdaCenter, fwhmLambda, noiseLevel, maskThreshold);

            int nanCount = 0;

            foreach (var value in vsiOutput.ResultArray)
            {
                if (float.IsNaN(value))
                {
                    nanCount++;
                }
            }

            Assert.AreEqual(0, nanCount);
        }
    }
}
