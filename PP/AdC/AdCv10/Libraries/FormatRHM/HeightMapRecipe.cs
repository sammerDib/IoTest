using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace FormatRHM
{
    public class HeightMapRecipe : ICloneable
    {
        private static int FORMAT_VERSION = 1;

        #region Dll import methods
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(byte[] dest, out uint destlen, byte[] source, uint sourcelen);
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] dest, uint[] destlen, byte[] source, uint sourcelen);
        #endregion


        public enum enMorphoFilter
        {
            Erode,
            Dilate,
            Open,
            Close,
            Invert,
            Median_3x3,
            Median_5x5,
        }


        #region attributes

        public String SourceFolder { get; set; }
        public int BaseImageSizeX { get; set; } // in pixels // Bufffer Width
        public int BaseImageSizeY { get; set; } // in pixels // Bufffer Height
        public int HeightCalcutationMethod { get; set; } // 0: Use whole background, 1:Use window cf margins)
        public int CoplanarityCalcutationMethod { get; set; } // reserved for later use ?

        public int WindowMarginX { get; set; } // in pixels
        public int WindowMarginY { get; set; } // in pixels
        public int NbImagesTrained { get; set; }
        public bool UseCompression { get; set; }
        public int AlgoMethod { get; set; } //0 mean; 1 max

        //
        // /!\ Les buffers doivent tous avoir la même taille (cf BaseImageSize)/!\
        //
        // buffer float
        public float[] GoldenImageBuffer = null;
        public uint GoldenCompressionSize = 0;

        // buffer byte 0 ou 1
        public byte[] MaskMeasureBuffer = null;
        public uint MaskMeasureCompressionSize = 0;

        // buffer byte 0 ou 1
        public byte[] MaskBackgroundBuffer = null;
        public uint MaskBackgroundCompressionSize = 0;

        public MaskGenParameters MskGenPrm_HeightMeasure = new MaskGenParameters();
        public MaskGenParameters MskGenPrm_Background = new MaskGenParameters();

        #endregion

        public HeightMapRecipe()
        {
            SourceFolder = "";
            BaseImageSizeX = 0;
            BaseImageSizeY = 0;
            UseCompression = true;
            NbImagesTrained = 0;
            HeightCalcutationMethod = 0;
            CoplanarityCalcutationMethod = 0;
            WindowMarginX = 0;
            WindowMarginY = 0;
            AlgoMethod = 0; // MEAN by default
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

                HeightCalcutationMethod = lBinaryReader.ReadInt32();
                WindowMarginX = lBinaryReader.ReadInt32();
                WindowMarginY = lBinaryReader.ReadInt32();
                CoplanarityCalcutationMethod = lBinaryReader.ReadInt32();

                NbImagesTrained = lBinaryReader.ReadInt32();

                int nCompression = lBinaryReader.ReadInt32();
                UseCompression = (0 != nCompression);

                if (lVersion >= 1)
                {
                    AlgoMethod = lBinaryReader.ReadInt32();
                }
                else
                    AlgoMethod = 0; // MEAN by default

                /*if(BaseImageSizeX != 0 || BaseImageSizeY == 0)
                {
                    GoldenImageBuffer = null;
                    MaskMeasureBuffer = null;
                    MaskBackgroundBuffer = null;
                    throw new Exception("One or Both Buffer Size is Empty. HeightMapRecipe Reading failed.");
                }*/

                // Golden buffer
                GoldenCompressionSize = lBinaryReader.ReadUInt32();
                if (GoldenCompressionSize == 0)
                    GoldenImageBuffer = null;
                else
                {
                    Byte[] lByteData = lBinaryReader.ReadBytes((int)GoldenCompressionSize);
                    Byte[] lFinalByteData = null;
                    if (UseCompression)
                    {
                        uint lOriginalSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(float);
                        lFinalByteData = Uncompression(lOriginalSize, lByteData);
                    }
                    else
                    {
                        lFinalByteData = lByteData;
                    }
                    GoldenImageBuffer = ConvertByteToFloat_BitConverterMethod(lFinalByteData);
                    Debug.Assert(GoldenImageBuffer.Length == (BaseImageSizeX * BaseImageSizeY));
                }

                // Mask Measure buffer
                MaskMeasureCompressionSize = lBinaryReader.ReadUInt32();
                if (MaskMeasureCompressionSize == 0)
                    MaskMeasureBuffer = null;
                else
                {
                    Byte[] lByteData = lBinaryReader.ReadBytes((int)MaskMeasureCompressionSize);
                    if (UseCompression)
                    {
                        uint lOriginalSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(byte);
                        MaskMeasureBuffer = Uncompression(lOriginalSize, lByteData);
                    }
                    else
                    {
                        MaskMeasureBuffer = lByteData;
                    }
                    Debug.Assert(MaskMeasureBuffer.Length == (BaseImageSizeX * BaseImageSizeY));
                }

                // Mask Background buffer
                MaskBackgroundCompressionSize = lBinaryReader.ReadUInt32();
                if (MaskBackgroundCompressionSize == 0)
                    MaskBackgroundBuffer = null;
                else
                {
                    Byte[] lByteData = lBinaryReader.ReadBytes((int)MaskBackgroundCompressionSize);
                    if (UseCompression)
                    {
                        uint lOriginalSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(byte);
                        MaskBackgroundBuffer = Uncompression(lOriginalSize, lByteData);
                    }
                    else
                    {
                        MaskBackgroundBuffer = lByteData;
                    }
                    Debug.Assert(MaskBackgroundBuffer.Length == (BaseImageSizeX * BaseImageSizeY));
                }

                MskGenPrm_HeightMeasure.Read(lBinaryReader);
                MskGenPrm_Background.Read(lBinaryReader);

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
                lBinaryWriter.Write(HeightMapRecipe.FORMAT_VERSION);

                lBinaryWriter.Write(SourceFolder.Length);
                lBinaryWriter.Write(SourceFolder);

                lBinaryWriter.Write(BaseImageSizeX);
                lBinaryWriter.Write(BaseImageSizeY);

                lBinaryWriter.Write(HeightCalcutationMethod);
                lBinaryWriter.Write(WindowMarginX);
                lBinaryWriter.Write(WindowMarginY);
                lBinaryWriter.Write(CoplanarityCalcutationMethod);

                lBinaryWriter.Write(NbImagesTrained);

                lBinaryWriter.Write(UseCompression ? 1 : 0);

                // Added in Version  = 1
                lBinaryWriter.Write(AlgoMethod);

                // Golden Float Buffer
                if (GoldenImageBuffer == null)
                {
                    GoldenCompressionSize = 0;
                    lBinaryWriter.Write(GoldenCompressionSize);
                }
                else
                {
                    Byte[] lByteData = null;
                    lByteData = ConverFloatToByte_BitConvertMethod(GoldenImageBuffer);
                    if (UseCompression)
                    {
                        Byte[] lDataCompressed = null;
                        lDataCompressed = Compression(lByteData, out GoldenCompressionSize);
                        lBinaryWriter.Write(GoldenCompressionSize);
                        if (GoldenCompressionSize != 0)
                            lBinaryWriter.Write(lDataCompressed);
                    }
                    else
                    {
                        GoldenCompressionSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(float);
                        lBinaryWriter.Write(GoldenCompressionSize);
                        if (GoldenCompressionSize != 0)
                            lBinaryWriter.Write(lByteData);
                    }
                }

                // Mask Measure - Byte
                if (MaskMeasureBuffer == null)
                {
                    MaskMeasureCompressionSize = 0;
                    lBinaryWriter.Write(MaskMeasureCompressionSize);
                }
                else
                {
                    if (UseCompression)
                    {
                        Byte[] lDataCompressed = null;
                        lDataCompressed = Compression(MaskMeasureBuffer, out MaskMeasureCompressionSize);
                        lBinaryWriter.Write(MaskMeasureCompressionSize);
                        if (MaskMeasureCompressionSize != 0)
                            lBinaryWriter.Write(lDataCompressed);
                    }
                    else
                    {
                        MaskMeasureCompressionSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(Byte);
                        lBinaryWriter.Write(MaskMeasureCompressionSize);
                        if (MaskMeasureCompressionSize != 0)
                            lBinaryWriter.Write(MaskMeasureBuffer);
                    }
                }

                // Mask Background - Byte
                if (MaskBackgroundBuffer == null)
                {
                    MaskBackgroundCompressionSize = 0;
                    lBinaryWriter.Write(MaskBackgroundCompressionSize);
                }
                else
                {
                    if (UseCompression)
                    {
                        Byte[] lDataCompressed = null;
                        lDataCompressed = Compression(MaskBackgroundBuffer, out MaskBackgroundCompressionSize);
                        lBinaryWriter.Write(MaskBackgroundCompressionSize);
                        if (MaskBackgroundCompressionSize != 0)
                            lBinaryWriter.Write(lDataCompressed);
                    }
                    else
                    {
                        MaskBackgroundCompressionSize = (uint)BaseImageSizeX * (uint)BaseImageSizeY * (uint)sizeof(Byte);
                        lBinaryWriter.Write(MaskBackgroundCompressionSize);
                        if (MaskBackgroundCompressionSize != 0)
                            lBinaryWriter.Write(MaskBackgroundBuffer);
                    }
                }

                MskGenPrm_HeightMeasure.Write(lBinaryWriter);
                MskGenPrm_Background.Write(lBinaryWriter);

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

        #region Convert Float[] to Byte[] methods
        /// <summary>
        /// Convert Float[] to Byte[] methods using buffer pointer and unsafe code 
        /// </summary>
        /// <param name="pData">Float data buffer to convert</param>
        /// <returns>Byte data buffer after conversion</returns>
        private Byte[] ConverFloatToByte_BitConvertMethod(float[] pData)
        {
            int lDataCount = pData.Length;
            byte[] lDataConverted = null;
            if (pData != null)
            {
                lDataConverted = new byte[lDataCount * sizeof(float)];
                uint lDataCompressionSize = (uint)(lDataCount * sizeof(float));

                // Convert float to byte
                for (int i = 0; i < lDataCount; i++)
                //Parallel.For(0, lDataLinesNbr, i =>
                {
                    byte[] lByteValues = BitConverter.GetBytes(pData[i]); // Return un tableau de 4 octets
                    lByteValues.CopyTo(lDataConverted, i * sizeof(float));
                }//);
            }
            return lDataConverted;
        }
        #endregion

        #region Convert Byte[] to float[] methods
        /// <summary>
        /// Convert Byte[] to Float[] with Bit Converter Method
        /// </summary>
        /// <param name="pOriginalBufferSize">Original buffer size before conversion</param>
        /// <param name="pData">Byte data buffer</param>
        /// <returns>float data buffer</returns>
        private float[] ConvertByteToFloat_BitConverterMethod(Byte[] pData)
        {
            // Init
            float[] lDataFloat = null;
            if ((pData.Length % 4) == 0)
            {
                uint lFloatDataSize = (uint)(pData.Length / sizeof(float));
                // Convert byte to float
                lDataFloat = new float[lFloatDataSize];
                for (int i = 0; i < lFloatDataSize; i++)
                {
                    lDataFloat[i] = BitConverter.ToSingle(pData, i * sizeof(float));
                }
            }
            return lDataFloat;
        }
        #endregion

        #region Compression/Uncompression methods
        /// <summary>
        /// Compression with ZLibWApi
        /// </summary>
        /// <param name="pData">Byte data buffer to compress</param>
        /// <param name="pCompressedBufferSize">Size of byte data buffer after compression</param>
        /// <returns>Byte data buffer compressed</returns>
        private Byte[] Compression(Byte[] pData, out uint pCompressedBufferSize)
        {
            // Init
            pCompressedBufferSize = 0;
            byte[] lDataCompressed = new byte[pData.Length];
            uint lDataCompressionSize = (uint)pData.Length; // initialized to original size            
            // Compression
            compress(lDataCompressed, out lDataCompressionSize, pData, (uint)pData.Length);
            // return result
            pCompressedBufferSize = lDataCompressionSize;
            Array.Resize(ref lDataCompressed, (int)lDataCompressionSize);
            return lDataCompressed;
        }

        /// <summary>
        /// Uncompression with ZLibWApi
        /// </summary>
        /// <param name="pOriginalBufferSize">Original buffer size before compression</param>
        /// <param name="pData">Byte data buffer</param>
        /// <returns>Byte data buffer uncompressed</returns>
        private Byte[] Uncompression(uint pOriginalBufferSize, Byte[] pData)
        {
            // Init
            Byte[] lDataByteUncompressed = new Byte[pOriginalBufferSize];
            uint[] lDataUncompressionSize = new uint[1] { pOriginalBufferSize };
            // Uncompress data    
            uncompress(lDataByteUncompressed, lDataUncompressionSize, pData, (uint)pData.Length);
            // Return result
            return lDataByteUncompressed;
        }
        #endregion

        #region ICloneable Members

        public virtual object Clone()
        {
            HeightMapRecipe cloned = MemberwiseClone() as HeightMapRecipe;
            return cloned;
        }

        #endregion
    }
}
