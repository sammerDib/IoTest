using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace FormatHAZE
{
    public class LSHazeResults
    {
        private static int FORMAT_VERSION = 1;

        public List<LSHazeData> Data = new List<LSHazeData>(3);

        #region Read/Write
        /// <summary>
        /// Read data from file
        /// </summary>
        /// <param name="pFilePathName">Path File name </param>
        /// <param name="sError">Error message if exception is thrown</param>
        /// <returns>true if success</returns
        public bool ReadFromFile(string pFilePathName, out String sError)
        {
            bool bSuccess = false;
            sError = "";
            FileStream lStream = null;
            BinaryReader lBinaryReader = null;

            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.Read);
                lBinaryReader = new BinaryReader(lStream);
                int lVersion = lBinaryReader.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. LSHazeResults Reading failed.");

                Data = new List<LSHazeData>(3);
                int nbData = lBinaryReader.ReadInt32();
                for (int i = 0; i < nbData; i++)
                {
                    LSHazeData dt = new LSHazeData();
                    dt.Read(lBinaryReader);
                    Data.Add(dt);
                }
                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryReader != null)
                    lBinaryReader.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }

        /// <summary>
        /// Write data to file
        /// </summary>
        /// <param name="pFilePathName">Path File name</param>
        /// <param name="sError">Error message if exception is thrown</param>
        /// <returns>true if success</returns>
        public bool WriteInFile(string pFilePathName, out String sError)
        {
            sError = "";
            FileStream lStream = null;
            BinaryWriter lBinaryWriter = null;
            bool bSuccess = false;
            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Create);
                lBinaryWriter = new BinaryWriter(lStream);
                lBinaryWriter.Write(LSHazeResults.FORMAT_VERSION);
                lBinaryWriter.Write(Data.Count);
                foreach (LSHazeData dt in Data)
                {
                    dt.Write(lBinaryWriter);
                }

                bSuccess = true;
            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
                bSuccess = false;
            }
            finally
            {
                if (lBinaryWriter != null)
                    lBinaryWriter.Close();
                if (lStream != null)
                    lStream.Close();
            }
            return bSuccess;
        }
        #endregion
        #region ThumbnailImg
        static public Bitmap GetThumbnailView(string pFilePathName, Color[] pColorMapRef, Size ThumbSizepx, out String sError)
        {

            Bitmap Thumb = new Bitmap(ThumbSizepx.Width, ThumbSizepx.Height);
            sError = "";
            FileStream lStream = null;
            BinaryReader lBinaryReader = null;

            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.Read);
                lBinaryReader = new BinaryReader(lStream);
                int lVersion = lBinaryReader.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. LSHazeResults Reading failed.");

                // on ne lit que le Premier 
                int nbData = lBinaryReader.ReadInt32();
                LSHazeData dt = null;
                if (nbData > 0)
                {
                    dt = new LSHazeData();
                    dt.Read(lBinaryReader);
                }
                // on libère vite le fichier
                if (lBinaryReader != null)
                {
                    lBinaryReader.Close();
                    lBinaryReader.Dispose();
                    lBinaryReader = null;
                }
                if (lStream != null)
                {
                    lStream.Close();
                    lStream.Dispose();
                    lStream = null;
                }

                if (dt != null)
                {
                    int nSample = 10; // on va prende 1 valeur / nSample ; à titre d'optim
                    int nNewW = (int)Math.Floor((double)dt.nWidth / (double)nSample);
                    int nNewH = (int)Math.Floor((double)dt.nHeigth / (double)nSample);

                    // Creation de L'image avec la color map donnée de min à max
                    Bitmap bmp = new Bitmap(nNewW, nNewH, PixelFormat.Format24bppRgb);
                    int nColoMaxIdx = pColorMapRef.Length;
                    float fColoMaxIdx = (float)nColoMaxIdx;
                    float a = fColoMaxIdx / (dt.max_ppm - dt.min_ppm);
                    float b = -dt.min_ppm * fColoMaxIdx / (dt.max_ppm - dt.min_ppm);
                    unsafe
                    {

                        //lock the new bitmap in memory
                        BitmapData newData = bmp.LockBits(new Rectangle(0, 0, nNewW, nNewH), ImageLockMode.WriteOnly, bmp.PixelFormat);

                        // get source bitmap pixel format size
                        int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
                        // Get color components count
                        int cCount = nDepth / 8;
                        if (nDepth == 24) // For 24 bpp set Red, Green and Blue
                        {
                            fixed (float* pfDATA = dt.HazeMeasures)
                            {
                                float* pStartArray = pfDATA;
                                Parallel.For(0, nNewH, new ParallelOptions { MaxDegreeOfParallelism = 2 }, y =>
                                //for (int y = 0; y < nNewH; y++)
                                {
                                    if ((nSample * y) < dt.nHeigth)
                                    {
                                        //get the data from the new image
                                        byte* pRow = (byte*)newData.Scan0 + (y * newData.Stride);
                                        float* pfRow = (float*)(pStartArray + (nSample * y * dt.nWidth));

                                        for (int x = 0; (x < nNewW) && ((x * nSample) < dt.nWidth); x++)
                                        {
                                            // convert float to index
                                            // float fVal = m_fData[y * m_nWidth + x] * a + b;
                                            float fVal = pfRow[0] * a + b;
                                            int nVal = (int)Math.Round(fVal);
                                            if (nVal < 0)
                                                nVal = 0;
                                            else if (nVal >= nColoMaxIdx)
                                                nVal = nColoMaxIdx - 1;
                                            //set the new image's pixel with current color map
                                            pRow[0] = pColorMapRef[nVal].B;
                                            pRow[1] = pColorMapRef[nVal].G;
                                            pRow[2] = pColorMapRef[nVal].R;
                                            pRow += cCount;

                                            pfRow += nSample;
                                        }
                                    }
                                }); // Parallel.For
                            }
                        }
                        //unlock the bitmaps
                        bmp.UnlockBits(newData);
                    }

                    // Resize to Thumbnail size
                    using (Graphics g = Graphics.FromImage(Thumb))
                    {
                        if (bmp != null)
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                            g.DrawImage(bmp, 0, 0, Thumb.Width, Thumb.Height);
                        }
                    }
                }


            }
            catch (System.Exception ex)
            {
                sError = ex.Message;
            }
            finally
            {
                if (lBinaryReader != null)
                    lBinaryReader.Close();
                if (lStream != null)
                    lStream.Close();
            }

            return Thumb;

        }
        #endregion

    }
}
