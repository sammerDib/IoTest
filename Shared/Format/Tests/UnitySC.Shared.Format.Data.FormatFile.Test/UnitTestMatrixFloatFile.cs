using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data.FormatFile;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace UnitySC.Shared.Format.Data.FormatFile
{
    [TestClass]
    public class UnitTestMatrixFloatFile
    {
        [TestMethod]
        public void _01_MonoChunk_NoCompress_NoParallel_3daFile()
        {
            string filename = "dummytestNC_1.3da";
            bool useCompression = false;
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(2.123, 2.564, "um", "rad", "ppm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            using (var format3daFile_W = new MatrixFloatFile())
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);
                // implict close file
            }

            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromFile(filename, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromFile should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromFile Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromFile Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromFile Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromFile Bad PixelSizeY");
            Assert.AreEqual(addData.UnitLabelX, headercheck.AdditionnalHeaderData.UnitLabelX, "GetHeaderFromFile Bad UnitLabelX");
            Assert.AreEqual(addData.UnitLabelY, headercheck.AdditionnalHeaderData.UnitLabelY, "GetHeaderFromFile Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromFile Bad UnitLabelZ");


            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromFile(filename, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _02_MonoChunk_Compression_NoParallel_3daFile()
        {
            string filenameNC = "dummytestNC_2.3da";
            string filename = "dummytest_2.3da";
            bool useCompression = true;
            int height = 64;
            int width = 95;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(2.123, 2.564, "um", "rad", "ppm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // direct write
            using (var format3daFile_W = new MatrixFloatFile(filenameNC, headerdata))
            {
                format3daFile_W.WriteInFile(filenameNC, headerdata, fmatrix, false);
                // implict close file
            }

            // direct write with compression
            using (var format3daFile_W = new MatrixFloatFile(filename, headerdata))
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);
                // implict close file
            }

            FileInfo fi = new FileInfo(filenameNC);
            long fileSizeNoCompress = fi.Length;
            fi = new FileInfo(filename);
            long fileSize = fi.Length;
            Assert.IsTrue(fileSizeNoCompress > fileSize, "Compressed Data should smaller that no compressed");

            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");

            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.ReadChunksFromFile(filename, -1), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _03_MonoChunk_Compression_Parallel_3daFile()
        {
            string filenameNC = "dummytestNC_3.3da";
            string filename = "dummytest_3.3da";
            bool useCompression = true;
            int height = 64;
            int width = 95;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(5.123, 5.264, "um", "um", "mm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // direct write
            using (var format3daFile_W = new MatrixFloatFile(filenameNC, headerdata))
            {
                format3daFile_W.WriteInFile(filenameNC, headerdata, fmatrix, false);
                // implict close file
            }

            // direct write with compression
            using (var format3daFile_W = new MatrixFloatFile(filename, headerdata))
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);
                // implict close file
            }

            FileInfo fi = new FileInfo(filenameNC);
            long fileSizeNoCompress = fi.Length;
            fi = new FileInfo(filename);
            long fileSize = fi.Length;
            Assert.IsTrue(fileSizeNoCompress > fileSize, "Compressed Data should smaller that no compressed");

            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");

            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.ReadChunksFromFile(filename, 0), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _04_MonoChunk_NoCompress_Parallel_3daFile()
        {
            string filename = "dummytestNC_4.3da";
            bool useCompression = false;
            int height = 82;
            int width = 46;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;


            var addData = new MatrixFloatFile.AdditionnalHeaderData(1.123, 0.6, "um", "um", "mm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            using (var format3daFile_W = new MatrixFloatFile())
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);
                // implict close file
            }

            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromFile(filename, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromFile should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromFile Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromFile Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromFile Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromFile Bad PixelSizeY");
            Assert.AreEqual(addData.UnitLabelX, headercheck.AdditionnalHeaderData.UnitLabelX, "GetHeaderFromFile Bad UnitLabelX");
            Assert.AreEqual(addData.UnitLabelY, headercheck.AdditionnalHeaderData.UnitLabelY, "GetHeaderFromFile Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromFile Bad UnitLabelZ");

            // direct read
            using (var format3daFile_R = new MatrixFloatFile(filename, 0))
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.GetChunkStatus(), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _05_MultiChunk_3daFile()
        {
            // this case is for huge file for obvious memory and time reason we are going to simulate this huge chunk

            string filename = "dummytest_5.3da";
            int height = 325;
            int width = 320;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(4.123, 5.564, "um", "mm", "-");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            using (var format3daFile_W = new MatrixFloatFile(filename, headerdata))
            {
                for (int i = 0; i < (height / 10); i++)
                {
                    float[] pTempData = fmatrix.Skip(i * 10 * width).Take(10 * width).ToArray();
                    format3daFile_W.FeedDataToWrite(pTempData);
                }
                if (height % 10 != 0)
                {
                    float[] pTempData = fmatrix.Skip((height / 10) * 10 * width).Take((height % 10) * width).ToArray();
                    format3daFile_W.FeedDataToWrite(pTempData);
                }
                format3daFile_W.CloseFile();
            }

            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromFile(filename, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromFile should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromFile Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromFile Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromFile Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromFile Bad PixelSizeY");
            Assert.AreEqual(addData.UnitLabelX, headercheck.AdditionnalHeaderData.UnitLabelX, "GetHeaderFromFile Bad UnitLabelX");
            Assert.AreEqual(addData.UnitLabelY, headercheck.AdditionnalHeaderData.UnitLabelY, "GetHeaderFromFile Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromFile Bad UnitLabelZ");

            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {

                var status = format3daFile_R.ReadChunksFromFile(filename, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MultiChunks, status, "read bad multi chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;

                // dans ce cas trés specific nous pouvons reformer un buffer car nous avons simuler des chunks de petite tailles
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file

                format3daFile_R.CloseFile();
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _06_MultiChunk_Parallel_3daFile()
        {
            // this case is normally designed for huge file for obvious memory and time reason we are going to simulate this
            // huge chunk and replace it with smaller ones

            string filename = "dummytest_6.3da";
            int height = 542;
            int width = 412;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(4.123, 5.564, "mm", "deg", "-");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            using (var format3daFile_W = new MatrixFloatFile(filename, headerdata))
            {
                for (int i = 0; i < (height / 10); i++)
                {
                    float[] pTempData = fmatrix.Skip(i * 10 * width).Take(10 * width).ToArray();
                    format3daFile_W.FeedDataToWrite(pTempData);
                }
                if (height % 10 != 0)
                {
                    float[] pTempData = fmatrix.Skip((height / 10) * 10 * width).Take((height % 10) * width).ToArray();
                    format3daFile_W.FeedDataToWrite(pTempData);
                }
                format3daFile_W.CloseFile();
            }


            string Badfilename = "FileThatDoesNotExist.3da";
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromFile(Badfilename, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromFile(filename, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromFile should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromFile Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromFile Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromFile Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromFile Bad PixelSizeY");
            Assert.AreEqual(addData.UnitLabelX, headercheck.AdditionnalHeaderData.UnitLabelX, "GetHeaderFromFile Bad UnitLabelX");
            Assert.AreEqual(addData.UnitLabelY, headercheck.AdditionnalHeaderData.UnitLabelY, "GetHeaderFromFile Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromFile Bad UnitLabelZ");

            // direct reading
            using (var format3daFile_R = new MatrixFloatFile(filename, 0))
            {
                var chunks = format3daFile_R.Chunks;

                // dans ce cas trés specific nous pouvons reformer un buffer car nous avons simuler des chunks de petite tailles
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.GetChunkStatus(), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file

            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _07_RetroCompatibility_3daFile_ReadV2()
        {
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;
            fmatrix[3 * width + 24] = 40.0f;

            String filename = "dummytest_7_v2.3da";
            int nRetroVersion2 = 2;
            using (var FStream = new FileStream(filename, FileMode.Create))
            {
                using (var FBinaryWriter = new BinaryWriter(FStream))
                {
                    FBinaryWriter.Write(nRetroVersion2);
                    FBinaryWriter.Write(height);
                    FBinaryWriter.Write(width);
                    FBinaryWriter.Write(sizeof(float));

                    uint originalsize = (uint)(fmatrix.Length * sizeof(float));
                    uint compressedsize = 0; // Not compressed
                    FBinaryWriter.Write(originalsize);
                    FBinaryWriter.Write(compressedsize);
                    var lDataConverted = new Byte[fmatrix.Length * sizeof(float)];
                    Buffer.BlockCopy(fmatrix, 0, lDataConverted, 0, lDataConverted.Length);
                    FBinaryWriter.Write(lDataConverted);
                }
            }

            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");

            // direct read no parallelization
            using (var format3daFile_CHECK = new MatrixFloatFile(filename, -1))
            {
                var readData1 = MatrixFloatFile.AggregateChunks(format3daFile_CHECK.GetChunkStatus(), format3daFile_CHECK);

                Assert.AreEqual(width, format3daFile_CHECK.Header.Width, "Read Bad Width");
                Assert.AreEqual(height, format3daFile_CHECK.Header.Height, "Read Bad Height");

                Assert.AreEqual(nRetroVersion2, format3daFile_CHECK.Header.Version, "Read Bad Earlier Version ");

                Assert.AreEqual(1.0, format3daFile_CHECK.Header.AdditionnalHeaderData.PixelSizeX, "Read Bad PixelSizeX");
                Assert.AreEqual(1.0, format3daFile_CHECK.Header.AdditionnalHeaderData.PixelSizeY, "Read Bad PixelSizeY");
                Assert.AreEqual(String.Empty, format3daFile_CHECK.Header.AdditionnalHeaderData.UnitLabelX, "Read Bad UnitLabelX");
                Assert.AreEqual(String.Empty, format3daFile_CHECK.Header.AdditionnalHeaderData.UnitLabelY, "Read Bad UnitLabelY");
                Assert.AreEqual(String.Empty, format3daFile_CHECK.Header.AdditionnalHeaderData.UnitLabelZ, "Read Bad UnitLabelZ");

                CollectionAssert.AreEqual(fmatrix, readData1);
            }

            //Delete file
            File.Delete(filename);
        }

        [TestMethod]
        public void _08_Write3daWithArray2D()
        {
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;
            fmatrix[3 * width + 24] = 40.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(3.3, 0.364, "mm", "deg", "-");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            String filename = "dummytest_8.3da";
            using (var format3daFile = new MatrixFloatFile())
            {
                format3daFile.WriteInFile(filename, headerdata, fmatrix, true);
            }

            var data1_2D = new float[height, width];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    data1_2D[y, x] = fmatrix[y * width + x];
                }
            }

            String filename2D = "dummytest_8_2D.3da";
            using (var format3daFile2D = new MatrixFloatFile())
            {
                format3daFile2D.WriteInFile(filename2D, addData, data1_2D, true);
                format3daFile2D.CloseFile();
            }

            AssertFileEx.AreContentEqual(filename, filename2D);

            using (var format3daFile_CHECK = new MatrixFloatFile(filename2D, -1))
            {
                // No Parallelization
                var readData1 = MatrixFloatFile.AggregateChunks(format3daFile_CHECK.GetChunkStatus(), format3daFile_CHECK);

                Assert.AreEqual(width, format3daFile_CHECK.Header.Width, "Read Bad Width");
                Assert.AreEqual(height, format3daFile_CHECK.Header.Height, "Read Bad Height");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_CHECK.Header.Version, "Read Bad Version ");

                CollectionAssert.AreEqual(fmatrix, readData1);
            }

            //Delete file
            File.Delete(filename2D);

            using (var format3daFile2D = new MatrixFloatFile(filename2D, headerdata))
            {
                format3daFile2D.WriteInFile(filename2D, headerdata.AdditionnalHeaderData, data1_2D, true);
                format3daFile2D.CloseFile();
            }
            using (var format3daFile_CHECK2 = new MatrixFloatFile(filename2D, 0))
            {
                //  Parallelization
                var readData2 = MatrixFloatFile.AggregateChunks(format3daFile_CHECK2.GetChunkStatus(), format3daFile_CHECK2);

                Assert.AreEqual(width, format3daFile_CHECK2.Header.Width, "Read Bad Width 2");
                Assert.AreEqual(height, format3daFile_CHECK2.Header.Height, "Read Bad Height 2");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_CHECK2.Header.Version, "Read Bad Version 2");

                CollectionAssert.AreEqual(fmatrix, readData2);
            }

            //Delete file
            File.Delete(filename2D);
            File.Delete(filename);
        }

        [TestMethod]
        public void _09_Memory_MonoChunk_NoCompress_NoParallel_3daFile()
        {
            bool useCompression = false;
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(2.123, 2.564, "um", "rad", "ppm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            byte[] MemBuffer = null;
            using (var format3daFile_W = new MatrixFloatFile())
            {
                MemBuffer = format3daFile_W.WriteInMemory(headerdata, fmatrix, useCompression);
                // implict close file
            }

            byte[] BadMemBuffer = new byte[] { 0, 1, 0, 1, 1 };
            AssertEx.Throws<Exception>(() => MatrixFloatFile.GetSizeFromMemory(BadMemBuffer, out int Wbad, out int Hbad));
            Assert.IsTrue(MatrixFloatFile.GetSizeFromMemory(MemBuffer, out int W, out int H), "GetSizeFromMemory should return true");
            Assert.AreEqual(width, W, "GetSizeFromMemory Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromMemory Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromMemory(MemBuffer, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromMemory should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromMemory Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromMemory Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromMemory Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromMemory Bad PixelSizeY");
            Assert.AreEqual(addData.UnitLabelX, headercheck.AdditionnalHeaderData.UnitLabelX, "GetHeaderFromMemory Bad UnitLabelX");
            Assert.AreEqual(addData.UnitLabelY, headercheck.AdditionnalHeaderData.UnitLabelY, "GetHeaderFromMemory Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromMemory Bad UnitLabelZ");


            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromMemory(MemBuffer, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }
        }

        [TestMethod]
        public void _10_Memory_MonoChunk_Compression_Parallel_3daFile()
        {
            byte[] MemBufferNC = null;
            byte[] MemBuffer = null;
            bool useCompression = true;
            int height = 64;
            int width = 95;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(5.123, 5.264, "um", "um", "mm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // direct write
            using (var format3daFile_W = new MatrixFloatFile(headerdata))
            {
                MemBufferNC = format3daFile_W.WriteInMemory(headerdata, fmatrix, false);
                // implict close file
            }

            //write with compression
            using (var format3daFile_W = new MatrixFloatFile())
            {
                MemBuffer = format3daFile_W.WriteInMemory(headerdata, fmatrix, useCompression);
                // implict close file
            }

            Assert.IsTrue(MemBufferNC.Length > MemBuffer.Length, "Compressed Data should smaller that no compressed");

            Assert.IsTrue(MatrixFloatFile.GetSizeFromMemory(MemBuffer, out int W, out int H), "GetSizeFromMemory should return true");
            Assert.AreEqual(width, W, "GetSizeFromMemory Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromMemory Bad Height");

            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.ReadChunksFromMemory(MemBuffer, 0), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

        }

        [TestMethod]
        public void _11_FiletoMemory()
        {
            string filename = "dummytest_11.3da";
            bool useCompression = true;
            int height = 64;
            int width = 95;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(5.123, 5.264, "um", "um", "mm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // direct write with compression
            using (var format3daFile_W = new MatrixFloatFile(filename, headerdata))
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);
                // implict close file
            }

            Assert.IsTrue(MatrixFloatFile.GetSizeFromFile(filename, out int W, out int H), "GetSizeFromFile should return true");
            Assert.AreEqual(width, W, "GetSizeFromFile Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromFile Bad Height");

            byte[] memoryBuffer = null;
            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.ReadChunksFromFile(filename, 0), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");

                format3daFile_R.CloseFile();
                memoryBuffer = format3daFile_R.WriteInMemory(useCompression);
                // implict close file
            }
            File.Delete(filename);

            // check if Memory buffer is OK 
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var rmatrix = MatrixFloatFile.AggregateChunks(format3daFile_R.ReadChunksFromMemory(memoryBuffer, 0), format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, format3daFile_R.GetChunkStatus(), "read bad chunks status");

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");

            }
        }

        [TestMethod]
        public void _12_MemoryToFile()
        {
            string filename = "Dummy12.3da";
            bool useCompression = false;
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(2.123, 2.564, "um", "rad", "ppm");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            byte[] MemBuffer = null;
            using (var format3daFile_W = new MatrixFloatFile())
            {
                MemBuffer = format3daFile_W.WriteInMemory(headerdata, fmatrix, useCompression);
                // implict close file
            }

            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromMemory(MemBuffer, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");

                format3daFile_R.CloseFile();

                format3daFile_R.WriteInFile(filename, useCompression);
                // implict close file
            }

            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromFile(filename);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "pixelsizeX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
            }

            File.Delete(filename);
        }

        [TestMethod]
        public void _13_HeaderData_3daFile()
        {
            bool useCompression = false;
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(0.0, 1.2, null, String.Empty, " ");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            // write
            byte[] MemBuffer = null;
            using (var format3daFile_W = new MatrixFloatFile())
            {
                MemBuffer = format3daFile_W.WriteInMemory(headerdata, fmatrix, useCompression);
                // implict close file
            }

            Assert.IsTrue(MatrixFloatFile.GetSizeFromMemory(MemBuffer, out int W, out int H), "GetSizeFromMemory should return true");
            Assert.AreEqual(width, W, "GetSizeFromMemory Bad Width");
            Assert.AreEqual(height, H, "GetSizeFromMemory Bad Height");
            Assert.IsTrue(MatrixFloatFile.GetHeaderFromMemory(MemBuffer, out MatrixFloatFile.HeaderData headercheck), "GetHeaderFromMemory should return true");
            Assert.AreEqual(width, headercheck.Width, "GetHeaderFromMemory Bad Width");
            Assert.AreEqual(height, headercheck.Height, "GetHeaderFromMemory Bad Height");
            Assert.AreEqual(addData.PixelSizeX, headercheck.AdditionnalHeaderData.PixelSizeX, "GetHeaderFromMemory Bad PixelSizeX");
            Assert.AreEqual(addData.PixelSizeY, headercheck.AdditionnalHeaderData.PixelSizeY, "GetHeaderFromMemory Bad PixelSizeY");
            Assert.IsTrue(String.IsNullOrEmpty(headercheck.AdditionnalHeaderData.UnitLabelX), "GetHeaderFromMemory Bad UnitLabelX");
            Assert.IsTrue(String.IsNullOrEmpty(headercheck.AdditionnalHeaderData.UnitLabelY), "GetHeaderFromMemory Bad UnitLabelY");
            Assert.AreEqual(addData.UnitLabelZ, headercheck.AdditionnalHeaderData.UnitLabelZ, "GetHeaderFromMemory Bad UnitLabelZ");


            // read
            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromMemory(MemBuffer, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.IsTrue(String.IsNullOrEmpty(format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX), "UnitLabelX");
                Assert.IsTrue(String.IsNullOrEmpty(format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY), "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

            var headerdata2 = new MatrixFloatFile.HeaderData(height, width, null);
            using (var format3daFile_W = new MatrixFloatFile())
            {
                MemBuffer = format3daFile_W.WriteInMemory(headerdata2, fmatrix, useCompression);
                // implict close file
            }

            using (var format3daFile_R = new MatrixFloatFile())
            {
                var status = format3daFile_R.ReadChunksFromMemory(MemBuffer, -1);
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(1.0, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(1.0, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.IsTrue(String.IsNullOrEmpty(format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX), "UnitLabelX");
                Assert.IsTrue(String.IsNullOrEmpty(format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY), "UnitLabelY");
                Assert.IsTrue(String.IsNullOrEmpty(format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ), "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
                // implict close file
            }

        }

        [TestMethod]
        public void _14_3da_BCRF_Converters()
        {
            string filename = "dummytest14.3da";
            bool useCompression = false;
            int height = 32;
            int width = 32;
            var fmatrix = new float[width * height];
            fmatrix[9 * width + 9] = 10.0f;
            fmatrix[19 * width + 19] = 20.0f;
            fmatrix[24 * width + 3] = 30.0f;

            var addData = new MatrixFloatFile.AdditionnalHeaderData(1.2, 1.35, "px", "px", "um");
            var headerdata = new MatrixFloatFile.HeaderData(height, width, addData);

            string bcrffilename = Path.ChangeExtension(filename, "bcrf");
            byte[] MemBcrf = null;
            // write
            using (var format3daFile_W = new MatrixFloatFile())
            {
                format3daFile_W.WriteInFile(filename, headerdata, fmatrix, useCompression);

                format3daFile_W.ToBCRF_File(bcrffilename);

                format3daFile_W.ToBCRF_Buffer(out MemBcrf);

                var hash = new System.Security.Cryptography.SHA1Managed();
                var hashedBytes = hash.ComputeHash(MemBcrf);
                Assert.AreEqual(Helper.AssertFileEx.ConvertBytesToHex(hashedBytes), Helper.AssertFileEx.GetFileHash(bcrffilename), "Bcrf files content dfiffers");
            }
            File.Delete(filename);

            using (var format3daFile_R = new MatrixFloatFile())
            {
                format3daFile_R.FromBCRF_File(bcrffilename);

                var status = format3daFile_R.GetChunkStatus();
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R.Header.Width;
                int rheight = format3daFile_R.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
            }
            File.Delete(bcrffilename);

            using (var format3daFile_R2 = new MatrixFloatFile())
            {
                format3daFile_R2.FromBCRF_Buffer(MemBcrf);

                var status = format3daFile_R2.GetChunkStatus();
                Assert.AreEqual(MatrixFloatFile.ChunkStatus.MonoChunk, status, "read bad chunks status");
                int rwidth = format3daFile_R2.Header.Width;
                int rheight = format3daFile_R2.Header.Height;
                var rmatrix = MatrixFloatFile.AggregateChunks(status, format3daFile_R2);

                Assert.AreEqual(MatrixFloatFile.FORMAT_VERSION, format3daFile_R2.Header.Version, "Write Bad Version");

                Assert.AreEqual(width, format3daFile_R2.Header.Width, "W");
                Assert.AreEqual(height, format3daFile_R2.Header.Height, "H");
                Assert.AreEqual(width, rwidth, "rW");
                Assert.AreEqual(height, rheight, "rH");

                Assert.AreEqual(addData.PixelSizeX, format3daFile_R2.Header.AdditionnalHeaderData.PixelSizeX, 0.00001, "PixelSizeX");
                Assert.AreEqual(addData.PixelSizeY, format3daFile_R2.Header.AdditionnalHeaderData.PixelSizeY, 0.00001, "PixelSizeY");
                Assert.AreEqual(addData.UnitLabelX, format3daFile_R2.Header.AdditionnalHeaderData.UnitLabelX, "UnitLabelX");
                Assert.AreEqual(addData.UnitLabelY, format3daFile_R2.Header.AdditionnalHeaderData.UnitLabelY, "UnitLabelY");
                Assert.AreEqual(addData.UnitLabelZ, format3daFile_R2.Header.AdditionnalHeaderData.UnitLabelZ, "UnitLabelZ");

                Assert.AreEqual(fmatrix[9 * width + 9], rmatrix[9 * width + 9], "[9,9]=10");
                Assert.AreEqual(fmatrix[19 * width + 19], rmatrix[19 * width + 19], "[19,19]=20");
                Assert.AreEqual(fmatrix[24 * width + 3], rmatrix[24 * width + 3], "[3,24]=30");
            }

         

        }

    }
}
