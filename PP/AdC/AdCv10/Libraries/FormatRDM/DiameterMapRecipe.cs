using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace FormatRDM
{
    public class DiameterMapRecipe : ICloneable
    {
        private static int FORMAT_VERSION = 2;

        #region attributes
        public String SourceFolder { get; set; }
        public int BaseImageSizeX { get; set; } // in pixels // Bufffer Width
        public int BaseImageSizeY { get; set; } // in pixels // Bufffer Height
        public int WindowMarginX { get; set; } // in pixels
        public int WindowMarginY { get; set; } // in pixels
        public int NbImagesTrained { get; set; }
        public int AlgoMethod { get; set; }
        public double PixelSizeum { get; set; } // 0 , circle, 1= ring, 2= img, 3=mmf
        public bool UseMicrons { get; set; }
        public double CircleRadius { get; set; }
        public double RingInnerRadius { get; set; }
        public double RingOuterRadius { get; set; }
        public string ImgPath { get; set; }
        public string MMFPath { get; set; }
        public int ModelSizeX { get; set; }
        public int ModelSizeY { get; set; }
        public int MosaicPartionSize { get; set; }

        public byte[] ModelFinderBuffer = null;
        public uint ModelFinderBufferSize = 0;

        //
        // /!\ Les buffers doivent tous avoir la même taille (cf BaseImageSize)/!\
        //
        public byte[] GoldenImageBuffer = null;
        public uint GoldenCompressionSize = 0;

        public List<PointF> _ListTargetPos = new List<PointF>();
        #endregion

        public DiameterMapRecipe()
        {
            SourceFolder = "";
            BaseImageSizeX = 0;
            NbImagesTrained = 0;
            WindowMarginX = 0;
            WindowMarginY = 0;
            AlgoMethod = 0;

            PixelSizeum = 10.0;
            UseMicrons = false;
            CircleRadius = 0.0;
            RingInnerRadius = 0.0;
            RingOuterRadius = 0.0;
            ImgPath = String.Empty;
            MMFPath = String.Empty;

            MosaicPartionSize = 0;
        }

        #region Read/Write

        /// <summary>
        /// Read data from file
        /// </summary>
        /// <param name="pFilePathName">Path File name </param>
        /// <param name="sError">Error message if exception is thrown</param>
        /// <returns>true if success</returns>
        public bool ReadFromFile(String pFilePathName, out String sError)
        {
            bool bSuccess = false;
            sError = "";
            FileStream lStream = null;
            BinaryReader lBinaryReader = null;

            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                lBinaryReader = new BinaryReader(lStream);
                int lVersion = lBinaryReader.ReadInt32();
                if (lVersion < 0 || lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. HeightMapRecipe Reading failed.");

                int StringLength = lBinaryReader.ReadInt32();
                SourceFolder = lBinaryReader.ReadString();
                Debug.Assert(SourceFolder.Length == StringLength);

                BaseImageSizeX = lBinaryReader.ReadInt32();
                BaseImageSizeY = lBinaryReader.ReadInt32();

                WindowMarginX = lBinaryReader.ReadInt32();
                WindowMarginY = lBinaryReader.ReadInt32();

                ModelSizeX = lBinaryReader.ReadInt32();
                ModelSizeY = lBinaryReader.ReadInt32();

                NbImagesTrained = lBinaryReader.ReadInt32();

                AlgoMethod = lBinaryReader.ReadInt32();

                PixelSizeum = lBinaryReader.ReadDouble();
                UseMicrons = lBinaryReader.ReadBoolean();
                CircleRadius = lBinaryReader.ReadDouble();
                RingInnerRadius = lBinaryReader.ReadDouble();
                RingOuterRadius = lBinaryReader.ReadDouble();
                StringLength = lBinaryReader.ReadInt32();
                ImgPath = lBinaryReader.ReadString();
                Debug.Assert(ImgPath.Length == StringLength);
                StringLength = lBinaryReader.ReadInt32();
                MMFPath = lBinaryReader.ReadString();
                Debug.Assert(MMFPath.Length == StringLength);

                // Golden buffer
                GoldenCompressionSize = lBinaryReader.ReadUInt32();
                if (GoldenCompressionSize == 0)
                    GoldenImageBuffer = null;
                else
                {
                    GoldenImageBuffer = lBinaryReader.ReadBytes((int)GoldenCompressionSize);
                    Debug.Assert(GoldenImageBuffer.Length == (BaseImageSizeX * BaseImageSizeY));
                }

                // Model buffer
                ModelFinderBufferSize = lBinaryReader.ReadUInt32();
                if (ModelFinderBufferSize == 0)
                    ModelFinderBuffer = null;
                else
                {
                    ModelFinderBuffer = lBinaryReader.ReadBytes((int)ModelFinderBufferSize);
                }


                int nMeasureCount = lBinaryReader.ReadInt32();
                if (nMeasureCount > 256)
                    _ListTargetPos = new List<PointF>(nMeasureCount);
                else
                    _ListTargetPos = new List<PointF>();

                for (int i = 0; i < nMeasureCount; i++)
                {
                    float fx = lBinaryReader.ReadSingle();
                    float fy = lBinaryReader.ReadSingle();
                    _ListTargetPos.Add(new PointF(fx, fy));

                }
                if (lVersion >= 2)
                    MosaicPartionSize = lBinaryReader.ReadInt32();
                else
                    MosaicPartionSize = Math.Max(BaseImageSizeX, BaseImageSizeY);

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
        public bool WriteInFile(String pFilePathName, out String sError)
        {
            sError = "";
            FileStream lStream = null;
            BinaryWriter lBinaryWriter = null;
            bool bSuccess = false;
            try
            {
                lStream = new FileStream(pFilePathName, FileMode.Create);
                lBinaryWriter = new BinaryWriter(lStream);
                lBinaryWriter.Write(DiameterMapRecipe.FORMAT_VERSION);

                lBinaryWriter.Write(SourceFolder.Length);
                lBinaryWriter.Write(SourceFolder);

                lBinaryWriter.Write(BaseImageSizeX);
                lBinaryWriter.Write(BaseImageSizeY);

                lBinaryWriter.Write(WindowMarginX);
                lBinaryWriter.Write(WindowMarginY);

                lBinaryWriter.Write(ModelSizeX);
                lBinaryWriter.Write(ModelSizeY);

                lBinaryWriter.Write(NbImagesTrained);

                lBinaryWriter.Write(AlgoMethod);

                lBinaryWriter.Write(PixelSizeum);
                lBinaryWriter.Write(UseMicrons);
                lBinaryWriter.Write(CircleRadius);
                lBinaryWriter.Write(RingInnerRadius);
                lBinaryWriter.Write(RingOuterRadius);
                lBinaryWriter.Write(ImgPath.Length);
                lBinaryWriter.Write(ImgPath);
                lBinaryWriter.Write(MMFPath.Length);
                lBinaryWriter.Write(MMFPath);

                // Golden  Buffer
                if (GoldenImageBuffer == null)
                {
                    GoldenCompressionSize = 0;
                    lBinaryWriter.Write(GoldenCompressionSize);
                }
                else
                {
                    GoldenCompressionSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(byte);
                    lBinaryWriter.Write(GoldenCompressionSize);
                    if (GoldenCompressionSize != 0)
                        lBinaryWriter.Write(GoldenImageBuffer);
                }

                // Model finder Buffer
                if (ModelFinderBuffer == null)
                {
                    ModelFinderBufferSize = 0;
                    lBinaryWriter.Write(ModelFinderBufferSize);
                }
                else
                {
                    ModelFinderBufferSize = (uint)sizeof(byte) * (uint)ModelFinderBuffer.Length;
                    lBinaryWriter.Write(ModelFinderBufferSize);
                    if (ModelFinderBufferSize != 0)
                        lBinaryWriter.Write(ModelFinderBuffer);
                }

                int nMeasureCount = _ListTargetPos.Count;
                lBinaryWriter.Write(nMeasureCount);
                foreach (PointF fpos in _ListTargetPos)
                {
                    lBinaryWriter.Write(fpos.X);
                    lBinaryWriter.Write(fpos.Y);
                }

                lBinaryWriter.Write(MosaicPartionSize);

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


        public void SortMeasures()
        {
            if (_ListTargetPos == null || _ListTargetPos.Count == 0)
                return;
            float YrowTolerance = (float)ModelSizeY * 0.5f;

            _ListTargetPos.Sort((a, b) =>
            {
                //si dans la même ligne
                if ((b.Y - YrowTolerance) <= a.Y && a.Y <= (b.Y + YrowTolerance))
                {
                    //  même ligne on tri par X croissant
                    return a.X.CompareTo(b.X);
                }
                return a.Y.CompareTo(b.Y);
            });
        }


        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            DiameterMapRecipe cloned = MemberwiseClone() as DiameterMapRecipe;
            cloned._ListTargetPos = _ListTargetPos.Select(pt => new PointF(pt.X, pt.Y)).ToList();
            return cloned;
        }

        #endregion

    }
}
