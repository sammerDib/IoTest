using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FloatDataFile
{
    public enum enumConversionMethod { cmBitConverter = 0, cmBufferBlockCopy }//, cmSampleBufferClass} //, cmBufferPointer } // Classer par ordre de performance    

    [Obsolete("Use UnitySC.Shared.Data.FormatFile.MatrixFloatFile instead", false)]
    public class CFloatDataFile : IDisposable
    {
        public const uint CHUNK_SIZE_FLOAT = 268435456;
        public const uint CHUNK_SIZE_BYTE = CHUNK_SIZE_FLOAT * 4;
        //public const uint CHUNK_SIZE = 536870897;
        protected BinaryWriter m_lBinaryWriter = null;
        protected BinaryReader m_lBinaryReader = null;
        protected FileStream m_lStream = null;

        #region Dll import methods
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(byte[] dest, out uint destlen, byte[] source, uint sourcelen);
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(IntPtr dest, out uint destlen, IntPtr source, uint sourcelen);
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] dest, uint[] destlen, byte[] source, uint sourcelen);
        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(IntPtr dest, uint[] destlen, IntPtr source, uint sourcelen);
        #endregion

        private int m_BufferHeight;
        private int m_BufferWidth;

        private const int FORMAT_VERSION = 2;

        public struct CompressedChunk
        {
            public uint OriginalSize;
            public uint CompressedSize;
            public byte[] Data;
        }

        public CFloatDataFile()
        {

        }

        public void Dispose()
        {
            if (m_lBinaryReader != null)
            {
                m_lBinaryReader.Close();
                m_lBinaryReader = null;
            }
            if (m_lBinaryWriter != null)
            {
                m_lBinaryWriter.Close();
                m_lBinaryWriter = null;
            }
            if (m_lStream != null)
            {
                m_lStream.Close();
                m_lStream = null;
            }

        }

        #region Read/Write in file

        /// <summary>
        /// Write a file from float data array in 2D with using buffer block copy conversion (float => byte).
        /// </summary>
        /// <param name="pFilePathName">File path name.</param>
        /// <param name="pLineNbr">Line number in data array</param>
        /// <param name="pColumnNbr">Column number in data array</param>
        /// <param name="pData">Data to save</param>
        /// <param name="pbCompression">Use compression or not</param>        
        public void WriteInFile(String pFilePathName, int pLineNbr, int pColumnNbr, float[] pData, bool pbCompression)
        {
            OpenWriteHeader(pFilePathName, pLineNbr, pColumnNbr);
            WriteInFile(pData, pbCompression, enumConversionMethod.cmBufferBlockCopy);
            CloseFile();
        }

        public void OpenWriteHeader(String pFilePathName, int pLineNbr, int pColumnNbr)
        {
            m_BufferWidth = pColumnNbr;
            m_BufferHeight = pLineNbr;
            m_lStream = new FileStream(pFilePathName, FileMode.Create);
            m_lBinaryWriter = new BinaryWriter(m_lStream);
            m_lBinaryWriter.Write(FORMAT_VERSION);
            m_lBinaryWriter.Write(pLineNbr);
            m_lBinaryWriter.Write(pColumnNbr);
            m_lBinaryWriter.Write(sizeof(float));
        }

        public void CloseFile()
        {
            if (m_lBinaryWriter != null)
            {
                m_lBinaryWriter.Close();
                m_lBinaryWriter = null;
            }
            if (m_lStream != null)
            {
                m_lStream.Close();
                m_lStream = null;
            }
        }

        /// <summary>
        /// Write a file from float data array in 2D with using conversion method in parameter (float => byte)
        /// </summary>
        /// <param name="pFilePathName">File path name.</param>
        /// <param name="pLineNbr">Line number in data array</param>
        /// <param name="pColumnNbr">Column number in data array</param>
        /// <param name="pData">Data to save</param>
        /// <param name="pbCompression">Use compression or not</param>
        /// <param name="pConversionMethod">conversion method used to optimize performance</param>
        public void WriteInFile(float[] pData, bool pbCompression, enumConversionMethod pConversionMethod)
        {
            try
            {
                //lBinaryWriter.Write(pLineNbr * pColumnNbr * sizeof(float));
                if (pData.Length % m_BufferWidth != 0)
                {
                    throw new Exception("Buffer is not aligned with parent data");
                }
                for (ulong i = 0; i < (uint)pData.Length * sizeof(float); i += CHUNK_SIZE_BYTE)
                {
                    uint SegmentSize = (uint)Math.Min(CHUNK_SIZE_BYTE, (uint)(pData.Length) * sizeof(float) - (ulong)i);
                    m_lBinaryWriter.Write(SegmentSize);
                    if (pData != null)
                    {
                        uint lDataCompressionSize = 0;
                        Byte[] lDataCompressed = null;
                        float[] pTempData = pData.Skip((int)(i / sizeof(float))).Take((int)(SegmentSize / sizeof(float))).ToArray();

                        if (pbCompression)
                        {
                            lDataCompressed = Compression(pTempData, out lDataCompressionSize);
                        }
                        else
                        {
                            lDataCompressed = ConvertFloatToByte_BlockCopyMethod(pTempData);
                        }
                        m_lBinaryWriter.Write(lDataCompressionSize);
                        m_lBinaryWriter.Write(lDataCompressed);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in WriteInFile <{0}>\n ErrMsg={1}", m_lStream.Name, e.Message));
            }
        }

        public List<CompressedChunk> CompressData(float[] pData)
        {
            List<CompressedChunk> compressedData = new List<CompressedChunk>();
            try
            {
                if (pData.Length % m_BufferWidth != 0)
                {
                    throw new Exception("Buffer is not aligned with parent data");
                }
                for (ulong i = 0; i < (uint)pData.Length * sizeof(float); i += CHUNK_SIZE_BYTE)
                {
                    uint SegmentSize = (uint)Math.Min(CHUNK_SIZE_BYTE, (uint)(pData.Length) * sizeof(float) - (ulong)i);
                    uint lDataCompressionSize = 0;
                    Byte[] lDataCompressed = null;
                    float[] pTempData = pData.Skip((int)(i / sizeof(float))).Take((int)(SegmentSize / sizeof(float))).ToArray();
                    lDataCompressed = Compression(pTempData, out lDataCompressionSize);
                    CompressedChunk chunk = new CompressedChunk() { OriginalSize = SegmentSize, CompressedSize = lDataCompressionSize, Data = lDataCompressed };
                    compressedData.Add(chunk);
                }
                return compressedData;
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in CompressData <{0}>\n ErrMsg={1}", m_lStream.Name, e.Message + "-" + e.StackTrace));
            }
        }

        public void WriteAlreadyCompressedData(List<CompressedChunk> CompressedData)
        {
            try
            {
                for (int i = 0; i < CompressedData.Count; i++)
                {
                    m_lBinaryWriter.Write((uint)CompressedData[i].OriginalSize);
                    m_lBinaryWriter.Write((uint)CompressedData[i].CompressedSize);
                    m_lBinaryWriter.Write(CompressedData[i].Data);
                }
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in WriteAlreadyCompressedData <{0}>\n ErrMsg={1}", m_lStream.Name, e.Message + "-" + e.StackTrace));
            }
        }

        /// <summary>
        /// Write a file from float data array in 2D with using buffer block copy conversion (float => byte).
        /// </summary>
        /// <param name="pFilePathName">File path name.</param>
        /// <param name="pData">Data to save</param>
        /// <param name="pbCompression">Use compression or not</param>
        public void WriteInFile(String pFilePathName, float[,] pData, bool pbCompression)
        {
            WriteInFile(pFilePathName, pData, pbCompression, enumConversionMethod.cmBufferBlockCopy);
        }

        /// <summary>
        /// Write a file from 1D float data array.
        /// </summary>
        public void WriteInFile(String FilePathName, float[] Data, int Width, int Height, bool bCompression)
        {
            m_BufferHeight = Height;
            m_BufferWidth = Width;

            OpenWriteHeader(FilePathName, m_BufferHeight, m_BufferWidth);
            WriteInFile(Data, bCompression, enumConversionMethod.cmBufferBlockCopy);
            CloseFile();
        }

        /// <summary>
        /// Write a file from float data array in 2D with using conversion method in parameter (float => byte)
        /// </summary>
        /// <param name="pFilePathName">File path name.</param>
        /// <param name="pData">Data to save</param>
        /// <param name="pbCompression">Use compression or not</param>
        /// <param name="pConversionMethod">conversion method used to optimize performance</param>
        public void WriteInFile(String pFilePathName, float[,] pData, bool pbCompression, enumConversionMethod pConversionMethod)
        {
            m_BufferHeight = pData.GetLength(0);
            m_BufferWidth = pData.GetLength(1);
            float[] lDataToCompress = null;

            lDataToCompress = new float[m_BufferHeight * m_BufferWidth];
            // Ajust array from [,] to []                                                        
            for (int i = 0; i < m_BufferHeight; i++)
            //Parallel.For(0, lDataLinesNbr, i =>
            {
                for (int j = 0; j < m_BufferWidth; j++)
                //Parallel.For(0, lDataColumnNbr, j =>
                {
                    lDataToCompress[i * m_BufferWidth + j] = pData[i, j];
                }//);
            }//);
            OpenWriteHeader(pFilePathName, m_BufferHeight, m_BufferWidth);
            WriteInFile(lDataToCompress, pbCompression, pConversionMethod);
            CloseFile();
        }



        /// <summary>
        /// Read a float binary file with using conversion method in parameter(byte => float).
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="pConversionMethod">conversion method used to optimize performance</param>
        /// <returns>Collection of Data array in 1D.</returns>
        public List<float[]> ReadFromFile(String pFilePathName, enumConversionMethod pConversionMethod)
        {
            try
            {
                m_lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_lBinaryReader = new BinaryReader(m_lStream);
                int lVersion = m_lBinaryReader.ReadInt32();
                bool bSameversion = (lVersion == FORMAT_VERSION);
                //if (lVersion != FORMAT_VERSION)
                //throw new Exception("Bad file format version number. Reading failed.");
                m_BufferHeight = m_lBinaryReader.ReadInt32();
                m_BufferWidth = m_lBinaryReader.ReadInt32();
                int lDataFormat = m_lBinaryReader.ReadInt32();
                if (lDataFormat != sizeof(float))
                    throw new Exception("Bad data format. Can use only float format (32bits)");

                List<float[]> FinalArray = new List<float[]>();
                try
                {
                    do
                    {
                        uint lDataOriginalSize = m_lBinaryReader.ReadUInt32();
                        uint lDataCompressedSize = m_lBinaryReader.ReadUInt32();

                        // Read data
                        uint nsizeToRead = lDataCompressedSize;
                        if (lDataCompressedSize == 0) // data non compréssée ! (bugfix RTI)
                            nsizeToRead = lDataOriginalSize;

                        // Read data
                        Byte[] lCompressedData;
                        lCompressedData = m_lBinaryReader.ReadBytes((int)nsizeToRead);
                        float[] lDataFloatUncompressed = Uncompression(lDataOriginalSize, lDataCompressedSize, lCompressedData);
                        FinalArray.Add(lDataFloatUncompressed);
                    }
                    while (m_lBinaryReader.BaseStream.Position != m_lBinaryReader.BaseStream.Length);
                }
                catch (System.IO.EndOfStreamException)
                {
                    // le fichier a ete lu entierement
                }
                return FinalArray;
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in ReadInFile <{0}>\n ErrMsg={1}", m_lStream.Name, e.Message));
            }
            finally
            {
                if (m_lBinaryReader != null)
                {
                    m_lBinaryReader.Close();
                    m_lBinaryReader = null;
                }
                if (m_lStream != null)
                {
                    m_lStream.Close();
                    m_lStream = null;
                }
            }
        }

        /// <summary>
        /// Read a float binary file with using conversion method in parameter(byte => float).
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="StartIndex">Start index to read a number of data specified in pCount parameter</param>
        /// <param name="pCount">Data number read</param>
        /// <param name="pConversionMethod">conversion method used to optimize performance</param>
        /// <returns></returns>
        private List<float[]> ReadFromFile(String pFilePathName, int StartIndex, int pCount, enumConversionMethod pConversionMethod)
        {
            List<float[]> lData = ReadFromFile(pFilePathName, pConversionMethod);
            List<float[]> lResultArray = new List<float[]>();
            //extraire les éléments 
            int Idx = 0;
            int IdxFloor = 0;
            foreach (float[] array in lData)
            {
                if (pCount == 0)
                {
                    //tout a été lu
                    break;
                }
                Idx += array.Length;
                if (Idx > StartIndex)
                {
                    float[] destArray = array.Skip(StartIndex - IdxFloor).Take(Math.Min(array.Length - (StartIndex - IdxFloor), pCount)).ToArray();
                    StartIndex += destArray.Length;
                    pCount -= destArray.Length;
                    lResultArray.Add(destArray);
                }

                IdxFloor += array.Length;
            }

            return lResultArray;
        }

        /// <summary>
        /// Read a float binary file with using "Bit Converter" conversion method (byte => float).
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="StartIndex">Start index to read a number of data specified in pCount parameter</param>
        /// <param name="pCount">Data number read</param>        
        /// <returns></returns>
        public List<float[]> ReadFromFile(String pFilePathName, int StartIndex, int pCount)
        {
            return ReadFromFile(pFilePathName, StartIndex, pCount, enumConversionMethod.cmBitConverter);
        }


        public List<float[]> ReadFromFile_Parallel(String pFilePathName, out int pBufferWidth, out int pBufferHeight, int coreCount = 0)
        {
            if (coreCount <= 0)
                coreCount = Environment.ProcessorCount;

            List<CompressedChunk> chunks = ReadFromFile_ToDecompress(pFilePathName, out pBufferWidth, out pBufferHeight);
            return Decompress_Parallel(chunks, coreCount);
        }

        private List<CompressedChunk> ReadFromFile_ToDecompress(String pFilePathName, out int pBufferWidth, out int pBufferHeight)
        {
            try
            {
                m_lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                m_lBinaryReader = new BinaryReader(m_lStream);
                int lVersion = m_lBinaryReader.ReadInt32();
                bool bSameversion = (lVersion == FORMAT_VERSION);
                //if (lVersion != FORMAT_VERSION)
                //      throw new Exception("Bad file format version number. Reading failed.");
                m_BufferHeight = m_lBinaryReader.ReadInt32();
                pBufferHeight = m_BufferHeight;
                m_BufferWidth = m_lBinaryReader.ReadInt32();
                pBufferWidth = m_BufferWidth;

                int lDataFormat = m_lBinaryReader.ReadInt32();
                if (lDataFormat != sizeof(float))
                    throw new Exception("Bad data format. Can use only float format (32bits)");

                List<CompressedChunk> CompressedArray = new List<CompressedChunk>();
                do
                {
                    uint lDataOriginalSize = m_lBinaryReader.ReadUInt32();
                    uint lDataCompressedSize = m_lBinaryReader.ReadUInt32();

                    // Read data
                    uint nsizeToRead = (uint)lDataCompressedSize;
                    if (lDataCompressedSize == 0) // data non compréssée ! (bugfix RTI)
                        nsizeToRead = lDataOriginalSize;

                    // Read data
                    Byte[] lCompressedData;//= new Byte[nsizeToRead]; Inutile : alloue de la mémoire pour rien [RBL]
                    lCompressedData = m_lBinaryReader.ReadBytes((int)nsizeToRead);
                    CompressedChunk value = new CompressedChunk() { OriginalSize = lDataOriginalSize, CompressedSize = lDataCompressedSize, Data = lCompressedData };
                    CompressedArray.Add(value);
                }
                while (m_lBinaryReader.BaseStream.Position != m_lBinaryReader.BaseStream.Length);
                return CompressedArray;
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in ReadFromFileCompressedData <{0}>\n ErrMsg={1}", m_lStream.Name, e.Message));
            }
            finally
            {
                if (m_lBinaryReader != null)
                {
                    m_lBinaryReader.Close();
                    m_lBinaryReader = null;
                }
                if (m_lStream != null)
                {
                    m_lStream.Close();
                    m_lStream = null;
                }
            }

        }

        public List<float[]> Decompress_Parallel(List<CompressedChunk> CompressArray, int coreCount)
        {
            List<float[]> FinalArray = null;
            try
            {
                int IterationsNumber = CompressArray.Count;
                FinalArray = new List<float[]>(IterationsNumber);
                for (int i = 0; i < IterationsNumber; i++)
                {
                    FinalArray.Add(null);
                }
                Parallel.For(0, IterationsNumber, new ParallelOptions { MaxDegreeOfParallelism = coreCount }, i =>
                //for (int i = 0; i < IterationsNumber; i++)
                {
                    CompressedChunk chunk = CompressArray[i];
                    FinalArray[i] = Uncompression(chunk.OriginalSize, chunk.CompressedSize, chunk.Data);
                });
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in Decompress_Paralell\n ErrMsg={1}", e.Message));
            }
            return FinalArray;
        }


        /// <summary>
        /// Read a float binary file with using conversion method in parameter(byte => float).
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="pBufferWidth"></param>
        /// <param name="pBufferHeight"></param>
        /// <param name="pConversionMethod">conversion method used to optimize performance (cmBitConverter fonctionne, les autres sont à faire/vérifer)</param>
        /// <returns></returns>
        public List<float[]> ReadFromFile(String pFilePathName, out int pBufferWidth, out int pBufferHeight, enumConversionMethod pConversionMethod)
        {
            List<float[]> lResult = ReadFromFile(pFilePathName, pConversionMethod);
            pBufferWidth = m_BufferWidth;
            pBufferHeight = m_BufferHeight;
            return lResult;
        }

        /// <summary>
        /// Read a float binary file with using "Bit Converter" conversion method (byte => float).
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="pBufferWidth"></param>
        /// <param name="pBufferHeight"></param>
        /// <returns></returns>
        public List<float[]> ReadFromFile(String pFilePathName, out int pBufferWidth, out int pBufferHeight)
        {
            List<float[]> lResult = ReadFromFile(pFilePathName, enumConversionMethod.cmBitConverter);
            pBufferWidth = m_BufferWidth;
            pBufferHeight = m_BufferHeight;
            return lResult;
        }

        /// <summary>
        /// Read File to get Buffer Size .
        /// </summary>
        /// <param name="pFilePathName">Path file name</param>
        /// <param name="pBufferWidth"></param>
        /// <param name="pBufferHeight"></param>
        /// <returns>true if success </returns>
        public static bool GetSizeFromFile(String pFilePathName, out int pBufferWidth, out int pBufferHeight)
        {
            BinaryReader l_lBinaryReader = null;
            FileStream l_lStream = null;
            bool bReturn = false;
            pBufferHeight = 0;
            pBufferWidth = 0;
            try
            {
                l_lStream = new FileStream(pFilePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                l_lBinaryReader = new BinaryReader(l_lStream);
                int lVersion = l_lBinaryReader.ReadInt32(); ;
                if (lVersion > FORMAT_VERSION)
                    throw new Exception("Bad file format version number. Reading failed.");
                pBufferHeight = l_lBinaryReader.ReadInt32();
                pBufferWidth = l_lBinaryReader.ReadInt32();
                bReturn = true;
            }
            catch (System.Exception e)
            {
                throw new Exception(String.Format("FloatDatafile.dll unexpected Error : in GetSizeFromFile <{0}>\n ErrMsg={1}", pFilePathName, e.Message));
            }
            finally
            {
                if (l_lBinaryReader != null)
                {
                    l_lBinaryReader.Close();
                    l_lBinaryReader = null;
                }
                if (l_lStream != null)
                {
                    l_lStream.Close();
                    l_lStream = null;
                }
            }
            return bReturn;
        }

        #endregion

        #region Convert Float[] to Byte[] methods       
        /// <summary>
        /// Convert Float[] to Byte[] methods using buffer pointer and unsafe code 
        /// </summary>
        /// <param name="pData">Float data buffer to convert</param>
        /// <returns>Byte data buffer after conversion</returns>
        private Byte[] ConvertFloatToByte_BitConvertMethod(float[] pData)
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

        /// <summary>
        /// Convert Float[] to Byte[] methods using Buffer.BlockCopy
        /// <param name="pData">Float data buffer to convert</param>
        /// <returns>Byte data buffer after conversion</returns>
        public Byte[] ConvertFloatToByte_BlockCopyMethod(float[] pData)
        {
            Byte[] lDataConverted = null;
            if (pData != null)
            {
                lDataConverted = new Byte[pData.Length * sizeof(float)];
                Buffer.BlockCopy(pData, 0, lDataConverted, 0, lDataConverted.Length);
            }
            return lDataConverted;
        }
        #endregion

        #region Convert Byte[] to float[] methods
        /// <summary>
        /// Convert Byte[] to Float[] with Bit Converter Method
        /// </summary>
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
                    //if (BitConverter.IsLittleEndian)
                    //    Array.Reverse(pData, i * sizeof(float), sizeof(float));
                    lDataFloat[i] = BitConverter.ToSingle(pData, i * sizeof(float));
                }
            }
            return lDataFloat;
        }

        /// <summary>
        /// Convert Byte[] to Float[] with Block Copy Method
        /// </summary>
        /// <param name="pData">Byte data buffer</param>
        /// <returns></returns>
        public float[] ConvertByteToFloat_BlockCopyMethod(Byte[] pData)
        {
            // Init
            float[] lDataFloat = null;
            if ((pData.Length % 4) == 0)
            {
                uint lFloatDataSize = (uint)(pData.Length / sizeof(float));
                // Convert float to byte 
                //System.Diagnostics.Debug.Assert(lFloatDataSize >= (uint.MaxValue/4));
                lDataFloat = new float[lFloatDataSize];
                Buffer.BlockCopy(pData, 0, lDataFloat, 0, pData.Length);
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
        private Byte[] Compression(float[] pData, out uint pCompressedBufferSize)
        {
            // Init
            pCompressedBufferSize = 0;
            uint lDataCompressionSize = (uint)pData.Length * sizeof(float) + 100; // initialized to original size, +100 pour les cas où le compress a du mal à compresser les données
            byte[] lDataCompressed = new byte[lDataCompressionSize];
            // Compression
            GCHandle hdl1 = GCHandle.Alloc(pData, GCHandleType.Pinned);
            GCHandle hdl2 = GCHandle.Alloc(lDataCompressed, GCHandleType.Pinned);
            try
            {
                int x = compress(hdl2.AddrOfPinnedObject(), out lDataCompressionSize, hdl1.AddrOfPinnedObject(), (uint)pData.Length * sizeof(float));
                if (x < 0)
                    throw new ApplicationException("unable to compress");

                // return result
                pCompressedBufferSize = lDataCompressionSize;
                Array.Resize(ref lDataCompressed, (int)lDataCompressionSize);
            }
            finally
            {
                hdl2.Free();
                hdl1.Free();
            }
            return lDataCompressed;
        }

        /// <summary>
        /// Uncompression with ZLibWApi
        /// </summary>
        /// <param name="pOriginalBufferSize">Original buffer size before compression</param>
        /// <param name="pCompressedBufferSize">Buffer size after compression, ou 0 si pas compressé</param>
        /// <param name="pData">Byte data buffer</param>
        /// <returns>Byte data buffer uncompressed</returns>
        private float[] Uncompression(uint pOriginalBufferSize, uint pCompressedBufferSize, Byte[] pData)
        {
            // Init
            GCHandle hdl1;
            GCHandle hdl2;

            float[] lDataByteUncompressed = new float[pOriginalBufferSize / 4];

            if (pCompressedBufferSize == 0)
            {
                // it is a non-compressed buffer
                if (pOriginalBufferSize != pData.Length)
                    throw new ApplicationException("Invalid data length");
                lDataByteUncompressed = ConvertByteToFloat_BlockCopyMethod(pData);
            }
            else
            {
                // Uncompress data    
                if (pCompressedBufferSize != pData.Length)
                    throw new ApplicationException("Invalid data length");

                uint[] lDataUncompressionSize = new uint[1] { pOriginalBufferSize };
                hdl1 = GCHandle.Alloc(lDataByteUncompressed, GCHandleType.Pinned);
                hdl2 = GCHandle.Alloc(pData, GCHandleType.Pinned);

                try
                {
                    int x = uncompress(hdl1.AddrOfPinnedObject(), lDataUncompressionSize, hdl2.AddrOfPinnedObject(), (uint)pData.Length);
                    if (x < 0)
                        throw new ApplicationException("error when decomprssing");
                }
                finally
                {
                    hdl2.Free();
                    hdl1.Free();
                }
            }

            // Return result
            return lDataByteUncompressed;
        }
        #endregion
    }
}
