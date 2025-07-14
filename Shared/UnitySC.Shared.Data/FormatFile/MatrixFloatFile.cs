using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace UnitySC.Shared.Data.FormatFile
{
    /// <summary>
    /// Matrix Float file - 3da Format
    /// ----------------------
    /// Best pratice :
    /// *** Retrieve matrix size or any additionnaal info
    /// 1) bool sucess = MatrixFloatFile.GetSizeFromFile(filename, out int width, out int height));
    /// 2) bool sucess = MatrixFloatFile.GetHeaderFromFile(filename, out HeaderData header));
    ///    int width = header.Width;
    ///    int height = header.Height;
    ///    int version = header.Version;
    ///    ...
    ///    string UnitZLabel = header.UnitLabelZ;
    ///
    ///
    /// *** To read (Mono chunk) : /!\ handle Memory allocation exception
    ///  float[] Matrix;
    ///  using (var format3daFile = new MatrixFloatFile())
    ///  {
    ///       var status = format3daFile.ReadChunksFromFile(filename, 0);
    ///       width = format3daFile.Header.Width;
    ///       height = format3daFile.Header.Height;
    ///       ...
    ///       Matrix = MatrixFloatFile.AggregateChunks(status, format3daFile.Chunks);
    ///      // implicit close file and clear data - dispose
    ///  }
    /// -- OR --
    /// float[] Matrix;
    ///  using (var format3daFile = new MatrixFloatFile(filename, 0))
    ///  {
    ///       width = format3daFile.Header.Width;
    ///       height = format3daFile.Header.Height;
    ///       ...
    ///       Matrix = MatrixFloatFile.AggregateChunks(format3daFile.GetChunkStatus(), format3daFile.Chunks);
    ///      // implicit close file and clear data - dispose
    ///  }
    ///
    ///
    /// *** To read (Multi chunks)
    ///  List<float[]> MatrixChunks;
    ///  using (var format3daFile = new MatrixFloatFile())
    ///  {
    ///        var status = format3daFile.ReadChunksFromFile(filename, 0);
    ///        width = format3daFile.Header.Width;
    ///        height = format3daFile.Header.Height;
    ///        ...
    ///        MatrixChunks = format3daFile.Chunks;
    ///      // implicit close file and clear data - dispose
    ///  }
    ///  -- OR --
    ///  List<float[]> MatrixChunks;
    ///  using (var format3daFile = new MatrixFloatFile(filename, 0)
    ///  {
    ///        width = format3daFile.Header.Width;
    ///        height = format3daFile.Header.Height;
    ///        ...
    ///        MatrixChunks = format3daFile.Chunks;
    ///      // implicit close file and clear data - dispose
    ///  }
    ///
    ///
    /// *** To write (<2 Go)
    /// float[] Matrix;                         --- OR --- float[,] Matrix = new float[height, width];
    /// MatrixFloatFile.HeaderData headerdata;  --- OR --- MatrixFloatFile.AdditionnalHeaderData AddData;
    /// ...
    /// using (var format3daFile = new MatrixFloatFile())
    /// {
    ///      format3daFile.WriteInFile(filename, headerData, Matrix, UseCompression)
    ///      // implicit close file and clear data - dispose
    /// }
    ///  -- OR --
    /// float[] Matrix;                         --- OR --- float[,] Matrix = new float[height, width];
    /// MatrixFloatFile.HeaderData headerdata;  --- OR --- MatrixFloatFile.AdditionnalHeaderData AddData;
    /// using (var format3daFile = new MatrixFloatFile(filename, headerData))
    /// {
    ///      format3daFile.WriteInFile(filename, headerData, Matrix, UseCompression)
    ///      // implicit close file and clear data - dispose
    /// }
    ///
    ///
    /// *** To write (>2 or 4 Go)
    /// MatrixFloatFile.HeaderData headerdata;
    /// using (var format3daFile = new MatrixFloatFile(filename, headerData))
    /// {
    ///     foreach matrix data parts
    ///     {
    ///         /// Retrieve data part from acqusition
    ///         ...
    ///         format3daFile.FeedDataToWrite( matrixdatapart);
    ///     }
    ///     // implicit close file and clear data - dispose
    /// }
    ///
    /// --------------------
    /// For MEMORY READ
    /// -------------------
    /// byte[] InputMemoryFileBuffer;
    /// float[] Matrix;
    ///  using (var format3daFile = new MatrixFloatFile(InputMemoryFileBuffer, 2))
    ///  {
    ///       width = format3daFile.Header.Width;
    ///       height = format3daFile.Header.Height;
    ///       ...
    ///       Matrix = MatrixFloatFile.AggregateChunks(format3daFile.GetChunkStatus(), format3daFile.Chunks);
    ///      // implicit close file and clear data - dispose
    ///  }
    /// -------------------
    /// For MEMORY WRITE
    /// -------------------
    ///
    /// float[] Matrix;                         --- OR --- float[,] Matrix = new float[height, width];
    /// MatrixFloatFile.HeaderData headerdata;  --- OR --- MatrixFloatFile.AdditionnalHeaderData AddData;
    /// ...
    /// byte[] OutputMemoryFileBuffer;
    /// using (var format3daFile = new MatrixFloatFile())
    /// {
    ///     OutputMemoryFileBuffer = format3daFile.WriteInMemory(headerData, Matrix, UseCompression)
    ///      // implicit close file and clear data - dispose
    /// }
    ///
    /// --------------------
    /// From File to Memory
    /// -------------------
    /// byte[] OutputMemoryFileBuffer;
    /// using (var format3daFile = new MatrixFloatFile(filename, 0)
    /// {
    ///     OutputMemoryFileBuffer= format3daFile.WriteInMemory()
    /// }
    ///
    /// --------------------
    /// From Memory to File
    /// -------------------
    /// byte[] InputMemoryFileBuffer;
    /// using (var format3daFile = new MatrixFloatFile(InputMemoryFileBuffer, 0)
    /// {
    ///     format3daFile.WriteInFile(filename, UseCompression)
    /// }
    ///
    /// </summary>
    public class MatrixFloatFile : IDisposable
    {
        #region Struct Header Data

        public struct HeaderData
        {
            public HeaderData(int height, int width, AdditionnalHeaderData additionnalHeaderData)
            {
                Version = FORMAT_VERSION;
                Height = height;
                Width = width;
                AdditionnalHeaderData = additionnalHeaderData ?? new AdditionnalHeaderData();
            }

            public HeaderData(int height, int width)
                : this(height, width, null)
            { }

            public int Version { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
            public AdditionnalHeaderData AdditionnalHeaderData { get; set; }

            public bool IsEmpty()
            { return (Height == 0 || Width == 0); }
        }

        #endregion Struct Header Data

        #region Additionnal Header Data

        public class AdditionnalHeaderData // Added in version 3
        {
            //default constructor
            public AdditionnalHeaderData() : this(1.0, 1.0, string.Empty, string.Empty, string.Empty) { }

            public AdditionnalHeaderData(double pixelSizeX, double pixelSizeY, string unitLabelX, string unitLabelY, string unitLabelZ)
            {
                PixelSizeX = pixelSizeX;
                PixelSizeY = pixelSizeY;
                UnitLabelX = unitLabelX;
                UnitLabelY = unitLabelY;
                UnitLabelZ = unitLabelZ;
            }

            public double PixelSizeX { get; set; }
            public double PixelSizeY { get; set; }
            public string UnitLabelX { get; set; }
            public string UnitLabelY { get; set; }
            public string UnitLabelZ { get; set; }
        }

        #endregion Additionnal Header Data

        #region Struct Compressed Chunks

        private struct CompressedChunk
        {
            public uint OriginalSize;
            public uint CompressedSize;
            public byte[] Data;
        }

        #endregion Struct Compressed Chunks

        public enum ChunkStatus
        {
            Empty = 0,
            MonoChunk,
            MultiChunks,
        }

        /// <summary>
        /// Maximum uncompressed Chunk size in float elements
        /// </summary>
        public const uint ChunkSizeFloat = 268435456;

        /// summary>
        /// Maximum uncompressed Chunk size in bytes
        /// </summary>
        public const uint ChunkSizeByte = ChunkSizeFloat * 4;

        public const int FORMAT_VERSION = 3; // Version 2 and ealier verions, please check floatdatafile for detail

        public HeaderData Header { get; set; } = new HeaderData(0, 0);
        public List<float[]> Chunks { get; private set; } = new List<float[]>();

        public bool IsChunksEmpty()
        { return (Chunks.Count == 0); }

        public bool IsMonoChunk()
        { return (Chunks.Count == 1); }

        public bool IsMultiChunks()
        { return (Chunks.Count > 1); }

        protected BinaryWriter FBinaryWriter = null;
        protected BinaryReader FBinaryReader = null;
        protected Stream FStream = null;
        protected string StreamName = string.Empty;
        private readonly object _sync = new object();

        #region Dll import methods (External ZLIBWAPI.DLL)

        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(IntPtr dest, out uint destlen, IntPtr source, uint sourcelen);

        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(IntPtr dest, uint[] destlen, IntPtr source, uint sourcelen);

        #endregion Dll import methods (External ZLIBWAPI.DLL)

        #region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public MatrixFloatFile()
        {
        }

        /// <summary>
        /// Constructor for direct READING
        /// </summary>
        public MatrixFloatFile(string filePathName, int nBParrallelCores = 0)
        {
            ReadChunksFromFile(filePathName, nBParrallelCores);
        }

        /// <summary>
        /// Constructor for direct WRITING - to use for  multi chunks writing with FeedDataToWrite
        /// </summary>
        public MatrixFloatFile(string filePathName, HeaderData headerData)
        {
            OpenFileToWrite(filePathName);
            WriteHeader(headerData);
        }

        /// <summary>
        /// Constructor for direct MEMORY READING
        /// </summary>
        public MatrixFloatFile(byte[] bufferFile, int nBParrallelCores = 0)
        {
            ReadChunksFromMemory(bufferFile, nBParrallelCores);
        }

        /// <summary>
        /// Constructor for direct MEMORY WRITING - to use for  multi chunks writing with FeedDataToWrite
        /// </summary>
        public MatrixFloatFile(HeaderData headerData)
        {
            OpenMemoryToWrite(headerData);
            WriteHeader(headerData);
        }

        #endregion Constructors

        #region IDisposable interface

        public void Dispose()
        {
            CloseFile();
            ClearData();
        }

        #endregion IDisposable interface

        #region CLOSE Method

        public void CloseFile()
        {
            lock (_sync)
            {
                if (FBinaryWriter != null)
                {
                    FBinaryWriter.Close();
                    FBinaryWriter = null;
                }
                if (FBinaryReader != null)
                {
                    FBinaryReader.Close();
                    FBinaryReader = null;
                }
                if (FStream != null)
                {
                    FStream.Close();
                    FStream = null;
                    StreamName = string.Empty;
                }
            }
        }

        public void ClearData()
        {
            Chunks = new List<float[]>();
            Header = new HeaderData(0, 0);
        }

        #endregion CLOSE Method

        #region WRITE Methods

        /// <summary>
        /// Write a Matrix file from float data array buffer (2D - stored by lines). (MONO CHUNCK Recommended Method to use (buffer size < 2Go))
        /// </summary>
        /// <param name="filePathName">File path name.</param>
        /// <param name="headerData">Height, width and other additionnal data V3 by default</param>
        /// <param name="data">Data float matrix to save </param>
        /// <param name="useCompression">Use compression or not</param>
        public void WriteInFile(string filePathName, HeaderData headerData, float[] data, bool useCompression)
        {
            if (FStream == null)
            {
                OpenFileToWrite(filePathName);
                WriteHeader(headerData);
            }
            WriteInStream(data, useCompression);
            CloseFile();
        }

        /// <summary>
        /// Write a Matrix file from float 2D array buffer.
        /// </summary>
        /// <param name="filePathName">File path name.</param>
        /// <param name="headerData">Additionnal data for V3</param>
        /// <param name="data">Data float matrix to save</param>
        /// <param name="pbCompression">Use compression or not</param>
        public void WriteInFile(string filePathName, AdditionnalHeaderData additionnalHeaderData, float[,] data, bool useCompression)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            float[] dataToCompress = new float[height * width];

            // Adjust array from [,] to []  - (2D - stored by lines)
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    dataToCompress[i * width + j] = data[i, j];
                }
            }

            var headerData = new HeaderData(height, width, additionnalHeaderData);
            WriteInFile(filePathName, headerData, dataToCompress, useCompression);
        }

        /// <summary>
        /// In case of extreme huge matrix, feed several data to be split in chunks, compressed then write in file
        /// OpenWriteHeader need to be called first !
        /// </summary>
        /// <param name="data">part</param>
        public void FeedDataToWrite(float[] data)
        {
            if (data == null)
                throw new Exception("Empty Data to processed");

            lock (_sync)
            {
                if (FBinaryWriter == null)
                    throw new Exception("Feed Error : OpenWriteHeader has not been called or Writer is in error");

                var compressedChunks = CompressData(data);
                WriteAlreadyCompressedData_sync(compressedChunks);
                FBinaryWriter.Flush();
            }
        }

        /// <summary>
        /// Write  Current data to a file. Warning : close any previous stream prio to this call
        /// </summary>
        /// <param name="filePathName">File path name.</param>
        /// <param name="useCompression">Use compression or not</param>
        public void WriteInFile(string filePathName, bool useCompression)
        {
            OpenFileToWrite(filePathName);
            WriteHeader(Header);
            if (IsMonoChunk())
                WriteInStream(Chunks[0], useCompression);
            else
            {
                foreach (var chunk in Chunks)
                    FeedDataToWrite(chunk);
            }
            CloseFile();
        }

        /// <summary>
        /// Write a Matrix file Memory buffer from float data array buffer (2D - stored by lines) into Memory. (MONO CHUNCK Recommended Method to use (buffer size < 2Go))
        /// </summary>
        /// <param name="headerData">Height, width and other additionnal data V3 by default</param>
        /// <param name="data">Data float matrix to save </param>
        /// <param name="useCompression">Use compression or not</param>
        public byte[] WriteInMemory(HeaderData headerData, float[] data, bool useCompression)
        {
            if (FStream == null)
            {
                OpenMemoryToWrite(headerData);
                WriteHeader(headerData);
            }
            WriteInStream(data, useCompression);
            FStream.SetLength(FStream.Position);
            byte[] memory = (FStream as MemoryStream).ToArray();
            CloseFile();
            return memory;
        }

        /// <summary>
        /// Write a Matrix file from float 2D array buffer into Memory.
        /// </summary>
        /// <param name="headerData">Additionnal data for V3</param>
        /// <param name="data">Data float matrix to save</param>
        /// <param name="pbCompression">Use compression or not</param>
        public byte[] WriteInMemory(AdditionnalHeaderData additionnalHeaderData, float[,] data, bool useCompression)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            float[] dataToCompress = new float[height * width];

            // Adjust array from [,] to []  - (2D - stored by lines)
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    dataToCompress[i * width + j] = data[i, j];
                }
            }

            var headerData = new HeaderData(height, width, additionnalHeaderData);
            return WriteInMemory(headerData, dataToCompress, useCompression); ;
        }

        /// <summary>
        /// Write Current data file into Memory. Warning : close any previous stream prio to this call
        /// </summary>
        /// <param name="filePathName">File path name.</param>
        /// <param name="useCompression">Use compression or not</param>
        public byte[] WriteInMemory(bool useCompression)
        {
            OpenMemoryToWrite(Header);
            WriteHeader(Header);
            if (IsMonoChunk())
                WriteInStream(Chunks[0], useCompression);
            else
            {
                foreach (var chunk in Chunks)
                    FeedDataToWrite(chunk);
            }
            FStream.SetLength(FStream.Position);
            byte[] memory = (FStream as MemoryStream).ToArray();
            CloseFile();
            return memory;
        }

        /// <summary>
        /// Open File & Binary Stream for Writing File
        /// </summary>
        /// <param name="filePathName">File path name.</param>
        private void OpenFileToWrite(string filePathName)
        {
            lock (_sync)
            {
                StreamName = filePathName;
                FStream = new FileStream(filePathName, FileMode.Create);
                FBinaryWriter = new BinaryWriter(FStream);
            }
        }

        /// <summary>
        /// Open Memory & Binary Stream for Writing in Buffer
        /// </summary>
        /// <param name="headerData">Header data.</param>
        private void OpenMemoryToWrite(HeaderData headerData)
        {
            lock (_sync)
            {
                StreamName = "MemoryWrite";
                int sizeBuf = 128 + headerData.Height * headerData.Width * sizeof(float);
                byte[] memBuf = new byte[sizeBuf];
                FStream = new MemoryStream(memBuf, 0, sizeBuf, true, true);
                FBinaryWriter = new BinaryWriter(FStream);
            }
        }

        /// <summary>
        /// Write Header
        /// </summary>
        /// <param name="headerData"></param>
        private void WriteHeader(HeaderData headerData)
        {
            Header = headerData;
            lock (_sync)
            {
                FBinaryWriter.Write(headerData.Version);
                FBinaryWriter.Write(headerData.Height);
                FBinaryWriter.Write(headerData.Width);
                FBinaryWriter.Write(sizeof(float));

                if (headerData.Version > 2)
                {
                    // we try to be robust here in case no additionnal header data has been set
                    if (headerData.AdditionnalHeaderData == null)
                        headerData.AdditionnalHeaderData = new AdditionnalHeaderData();

                    //Version 3 - specific
                    FBinaryWriter.Write(headerData.AdditionnalHeaderData.PixelSizeX);
                    FBinaryWriter.Write(headerData.AdditionnalHeaderData.PixelSizeY);
                    if (!String.IsNullOrEmpty(headerData.AdditionnalHeaderData.UnitLabelX))
                        FBinaryWriter.Write(headerData.AdditionnalHeaderData.UnitLabelX);
                    else
                        FBinaryWriter.Write(String.Empty);
                    if (!String.IsNullOrEmpty(headerData.AdditionnalHeaderData.UnitLabelY))
                        FBinaryWriter.Write(headerData.AdditionnalHeaderData.UnitLabelY);
                    else
                        FBinaryWriter.Write(String.Empty);
                    if (!String.IsNullOrEmpty(headerData.AdditionnalHeaderData.UnitLabelZ))
                        FBinaryWriter.Write(headerData.AdditionnalHeaderData.UnitLabelZ);
                    else
                        FBinaryWriter.Write(String.Empty);
                }
            }
        }

        /// <summary>
        ///  Internal MONO Chunk matrix float write Method - buffer ~< 2.7 Go (see ChunkSizeFloat)
        ///  OpenFileToWrite need to be called first ! Data should not be empty.
        /// </summary>
        /// <param name="data">Data to save</param>
        /// <param name="useCompression">Use compression or not</param>
        private void WriteInStream(float[] data, bool useCompression)
        {
            try
            {
                if (data == null)
                    throw new Exception("Empty Data to processed");

                lock (_sync)
                {
                    if (FBinaryWriter == null)
                        throw new Exception("OpenWriteHeader has not been called or Writer is in error");

                    if (Header.IsEmpty())
                        throw new Exception("Header is Empty");

                    if (data.Length % Header.Width != 0)
                        throw new Exception("Buffer is not aligned with parent data");

                    for (uint i = 0; i < (uint)data.Length; i += ChunkSizeFloat)
                    {
                        uint SegmentSize = (uint)Math.Min(ChunkSizeFloat, (uint)data.Length - i);
                        uint SegmentSizeByte = SegmentSize * (uint)sizeof(float);
                        FBinaryWriter.Write(SegmentSizeByte);
                        if (data != null)
                        {
                            uint lDataCompressionSize = 0;
                            float[] pTempData = data.Skip((int)i).Take((int)SegmentSize).ToArray();
                            Chunks.Add(pTempData);

                            byte[] lDataCompressed = null;
                            if (useCompression)
                            {
                                lDataCompressed = Compression(pTempData, out lDataCompressionSize);
                            }
                            else
                            {
                                lDataCompressed = ConvertFloatToByte_BlockCopyMethod(pTempData);
                            }
                            FBinaryWriter.Write(lDataCompressionSize);
                            FBinaryWriter.Write(lDataCompressed);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                throw new Exception($"Unexpected Error : in WriteInFile <{StreamName}>\n ErrMsg={e.Message}");
            }
        }

        /// <summary>
        /// split data in chunks and compressed it
        /// </summary>
        /// <param name="data"></param>
        /// <returns> compressed chunks list to be written </returns>
        private List<CompressedChunk> CompressData(float[] data)
        {
            var compressedData = new List<CompressedChunk>();
            try
            {
                if (data.Length % Header.Width != 0)
                    throw new Exception("Buffer is not aligned with parent data");

                for (uint i = 0; i < (uint)data.Length; i += ChunkSizeFloat)
                {
                    uint SegmentSize = (uint)Math.Min(ChunkSizeByte, (uint)data.Length - i);
                    float[] pTempData = data.Skip((int)i).Take((int)SegmentSize).ToArray();
                    Chunks.Add(pTempData);

                    byte[] lDataCompressed = null;
                    lDataCompressed = Compression(pTempData, out uint lDataCompressionSize);
                    var chunk = new CompressedChunk() { OriginalSize = SegmentSize * sizeof(float), CompressedSize = lDataCompressionSize, Data = lDataCompressed };
                    compressedData.Add(chunk);
                }
                return compressedData;
            }
            catch (System.Exception e)
            {
                throw new Exception($"Unexpected Error : in CompressData <{StreamName}>\n ErrMsg={e.Message} - {e.StackTrace}");
            }
        }

        /// <summary>
        /// Write compressed chunks in file.
        /// </summary>
        /// <param name="compressedData"></param>
        private void WriteAlreadyCompressedData_sync(List<CompressedChunk> compressedData)
        {
            try
            {
                if (FBinaryWriter == null)
                    throw new Exception("OpenWriteHeader has not been called or Writer is in error");

                for (int i = 0; i < compressedData.Count; i++)
                {
                    FBinaryWriter.Write(compressedData[i].OriginalSize);
                    FBinaryWriter.Write(compressedData[i].CompressedSize);
                    FBinaryWriter.Write(compressedData[i].Data);
                }
            }
            catch (System.Exception e)
            {
                throw new Exception($"unexpected Error : in WriteAlreadyCompressedData <{StreamName}>\n ErrMsg={e.Message} - {e.StackTrace}");
            }
        }

        #endregion WRITE Methods

        #region READ Methods

        /// <summary>
        /// Read matrix float data chunks
        /// </summary>
        /// <param name="filePathName">Path file name</param>
        /// <param name="nBParrallelCores"> Number of parallel cores used to decompressed matrix, if <0 NoParalleliztion (default 0 = use max cores)</param>
        /// <returns> Matrix buffer chunks & header data (height, width and other additional data if it is V3)</returns>
        public ChunkStatus ReadChunksFromFile(string filePathName, int nBParrallelCores = 0)
        {
            OpenFileToRead(filePathName);
            Header = ReadHeader(FBinaryReader);

            List<float[]> chunks;
            if (nBParrallelCores < 0)
            {
                chunks = ReadDataChunks();
            }
            else
            {
                chunks = ReadDataChunks_Parallel(nBParrallelCores);
            }
            Chunks = chunks;
            CloseFile();
            return GetChunkStatus();
        }

        /// <summary>
        /// Read matrix float data chunks
        /// </summary>
        /// <param name="buffer">Matrix float file buffer</param>
        /// <param name="nBParrallelCores"> Number of parallel cores used to decompressed matrix, if <0 NoParalleliztion (default 0 = use max cores)</param>
        /// <returns> Matrix buffer chunks & header data (height, width and other additional data if it is V3)</returns>
        public ChunkStatus ReadChunksFromMemory(byte[] buffer, int nBParrallelCores = 0)
        {
            OpenMemoryToRead(buffer);
            Header = ReadHeader(FBinaryReader);

            List<float[]> chunks;
            if (nBParrallelCores < 0)
            {
                chunks = ReadDataChunks();
            }
            else
            {
                chunks = ReadDataChunks_Parallel(nBParrallelCores);
            }
            Chunks = chunks;
            CloseFile();
            return GetChunkStatus();
        }

        public static float[] AggregateChunks(ChunkStatus status, MatrixFloatFile mff)
        {
            if (status == ChunkStatus.Empty)
                return null;

            if (status == ChunkStatus.MonoChunk)
                return mff.Chunks[0];

            // Pray to have enougth memory avalable, then you will need to handle out of memory axception
            //   --> after 4.5 check  <gcAllowVeryLargeObjects enabled="true" />  in app config to increase buffer size
            float[] bufferMatrix = new float[mff.Header.Width * mff.Header.Height];
            int buffeed_element = 0;
            foreach (float[] chunk in mff.Chunks)
            {
                Array.Copy(chunk, 0, bufferMatrix, buffeed_element, chunk.Length);
                buffeed_element += chunk.Length;
            }
            return bufferMatrix;
        }

        /// <summary>
        /// Read partial File to get only Matrix global Buffer Sizes .
        /// </summary>
        /// <param name="filePathName">Path file name</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if success </returns>
        public static bool GetSizeFromFile(string filePathName, out int width, out int height)
        {
            BinaryReader binaryReader = null;
            FileStream fStream = null;
            bool bReturn = false;
            height = 0;
            width = 0;
            try
            {
                fStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                binaryReader = new BinaryReader(fStream);
                int lVersion = binaryReader.ReadInt32();
                height = binaryReader.ReadInt32();
                width = binaryReader.ReadInt32();
                bReturn = true;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in GetSizeFromFile <{filePathName}>\n ErrMsg={e.Message}");
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                }
                if (fStream != null)
                {
                    fStream.Close();
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Read partial File to get only Matrix global Buffer Sizes .
        /// </summary>
        /// <param name="filePathName">Path file name</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if success </returns>
        public static bool GetSizeFromMemory(byte[] memoryFileBuffer, out int width, out int height)
        {
            BinaryReader binaryReader = null;
            MemoryStream fStream = null;
            bool bReturn = false;
            height = 0;
            width = 0;
            try
            {
                fStream = new MemoryStream(memoryFileBuffer, false);
                binaryReader = new BinaryReader(fStream);
                int lVersion = binaryReader.ReadInt32();
                height = binaryReader.ReadInt32();
                width = binaryReader.ReadInt32();
                bReturn = true;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in GetSizeFromMemory\n ErrMsg={e.Message}");
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                }
                if (fStream != null)
                {
                    fStream.Close();
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Read partial File to get only Matrix global Buffer Sizes .
        /// </summary>
        /// <param name="filePathName">Path file name</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if success </returns>
        public static bool GetHeaderFromFile(string filePathName, out HeaderData header)
        {
            BinaryReader binaryReader = null;
            FileStream fStream = null;
            bool bReturn = false;

            try
            {
                fStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                binaryReader = new BinaryReader(fStream);
                header = ReadHeader(binaryReader);
                bReturn = true;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in GetSizeFromFile <{filePathName}>\n ErrMsg={e.Message}");
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                    binaryReader = null;
                }
                if (fStream != null)
                {
                    fStream.Close();
                    fStream = null;
                }
            }
            return bReturn;
        }

        /// <summary>
        /// Read partial File to get only Matrix global Buffer Sizes .
        /// </summary>
        /// <param name="filePathName">Path file name</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if success </returns>
        public static bool GetHeaderFromMemory(byte[] memoryFileBuffer, out HeaderData header)
        {
            BinaryReader binaryReader = null;
            MemoryStream fStream = null;
            bool bReturn = false;

            try
            {
                fStream = new MemoryStream(memoryFileBuffer, false);
                binaryReader = new BinaryReader(fStream);
                header = ReadHeader(binaryReader);
                bReturn = true;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in GetSizeFromMemory\n ErrMsg={e.Message}");
            }
            finally
            {
                if (binaryReader != null)
                {
                    binaryReader.Close();
                    binaryReader = null;
                }
                if (fStream != null)
                {
                    fStream.Close();
                    fStream = null;
                }
            }
            return bReturn;
        }

        /// <summary>
        /// to determine internals structure of file
        /// </summary>
        /// <returns> chunks status</returns>
        public ChunkStatus GetChunkStatus()
        {
            if (IsChunksEmpty())
                return ChunkStatus.Empty;

            if (IsMonoChunk())
                return ChunkStatus.MonoChunk;

            return ChunkStatus.MultiChunks;
        }

        private void OpenFileToRead(string filePathName)
        {
            lock (_sync)
            {
                StreamName = filePathName;
                FStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                FBinaryReader = new BinaryReader(FStream);
            }
        }

        private void OpenMemoryToRead(byte[] buffer)
        {
            lock (_sync)
            {
                StreamName = "MemoryRead";
                FStream = new MemoryStream(buffer, false);
                FBinaryReader = new BinaryReader(FStream);
            }
        }

        private static HeaderData ReadHeader(BinaryReader binaryReader)
        {
            if (binaryReader == null)
                throw new Exception($"Null binaryReader");

            int version = binaryReader.ReadInt32();
            if (version < 1)
                throw new Exception($"Obsolete file format version number v{version}. Should be at least v1 or superior");

            var height = binaryReader.ReadInt32();
            var width = binaryReader.ReadInt32();
            var headerData = new HeaderData(height, width);
            headerData.Version = version;

            int lDataFormat = binaryReader.ReadInt32();
            if (lDataFormat != sizeof(float))
                throw new Exception("Bad data format. Can use only float format (32bits)");

            if (version >= 3)
            {
                var pixelSizeX = binaryReader.ReadDouble();
                var pixelSizeY = binaryReader.ReadDouble();
                var unitLabelX = binaryReader.ReadString();
                var unitLabelY = binaryReader.ReadString();
                var unitLabelZ = binaryReader.ReadString();
                headerData.AdditionnalHeaderData = new AdditionnalHeaderData(pixelSizeX, pixelSizeY, unitLabelX, unitLabelY, unitLabelZ);
            }
            return headerData;
        }

        private List<float[]> ReadDataChunks()
        {
            try
            {
                var FinalArray = new List<float[]>();
                try
                {
                    lock (_sync)
                    {
                        do
                        {
                            uint lDataOriginalSize = FBinaryReader.ReadUInt32();
                            uint lDataCompressedSize = FBinaryReader.ReadUInt32();

                            // Read data
                            uint nsizeToRead = lDataCompressedSize;
                            if (lDataCompressedSize == 0) // data non compréssée ! (par convention)
                                nsizeToRead = lDataOriginalSize;

                            // Read data
                            byte[] lCompressedData;
                            lCompressedData = FBinaryReader.ReadBytes((int)nsizeToRead);
                            float[] lDataFloatUncompressed = Uncompression(lDataOriginalSize, lDataCompressedSize, lCompressedData);
                            FinalArray.Add(lDataFloatUncompressed);
                        }
                        while (FBinaryReader.BaseStream.Position != FBinaryReader.BaseStream.Length);
                    }
                }
                catch (System.IO.EndOfStreamException)
                {
                    // le fichier a ete lu entierement
                }
                return FinalArray;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in ReadInFile <{StreamName}>\n ErrMsg={e.Message}");
            }
        }

        private List<float[]> ReadDataChunks_Parallel(int coreCount = 0)
        {
            if (coreCount <= 0)
                coreCount = Environment.ProcessorCount;

            var chunks = ReadDataChunks_ToDecompress();
            return Decompress_Parallel(chunks, coreCount);
        }

        private List<CompressedChunk> ReadDataChunks_ToDecompress()
        {
            try
            {
                var CompressedArray = new List<CompressedChunk>();
                lock (_sync)
                {
                    do
                    {
                        uint lDataOriginalSize = FBinaryReader.ReadUInt32();
                        uint lDataCompressedSize = FBinaryReader.ReadUInt32();

                        // Read data
                        uint nsizeToRead = lDataCompressedSize;
                        if (lDataCompressedSize == 0) // data non compréssée ! (par convention)
                            nsizeToRead = lDataOriginalSize;

                        // Read data
                        byte[] lCompressedData;
                        lCompressedData = FBinaryReader.ReadBytes((int)nsizeToRead);
                        var value = new CompressedChunk() { OriginalSize = lDataOriginalSize, CompressedSize = lDataCompressedSize, Data = lCompressedData };
                        CompressedArray.Add(value);
                    }
                    while (FBinaryReader.BaseStream.Position != FBinaryReader.BaseStream.Length);
                }
                return CompressedArray;
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in ReadFromFile_ToDecompress <{StreamName}>\n ErrMsg={e.Message}");
            }
        }

        private List<float[]> Decompress_Parallel(List<CompressedChunk> compressArray, int coreCount)
        {
            List<float[]> FinalArray = null;
            try
            {
                int IterationsNumber = compressArray.Count;
                FinalArray = new List<float[]>(IterationsNumber);
                for (int i = 0; i < IterationsNumber; i++)
                {
                    FinalArray.Add(null);
                }
                Parallel.For(0, IterationsNumber, new ParallelOptions { MaxDegreeOfParallelism = coreCount }, i =>
                {
                    var chunk = compressArray[i];
                    FinalArray[i] = Uncompression(chunk.OriginalSize, chunk.CompressedSize, chunk.Data);
                });
            }
            catch (System.Exception e)
            {
                throw new Exception($"MatrixFloatFile unexpected Error : in Decompress_Paralell\n ErrMsg={e.Message}");
            }
            return FinalArray;
        }

        #endregion READ Methods

        #region Compression/Uncompression methods

        /// <summary>
        /// Compression with ZLibWApi
        /// </summary>
        /// <param name="data">Byte data buffer to compress</param>
        /// <param name="compressedBufferSize">Size of byte data buffer after compression</param>
        /// <returns>Byte data buffer compressed</returns>
        private byte[] Compression(float[] data, out uint compressedBufferSize)
        {
            // Init
            compressedBufferSize = 0;
            uint lDataCompressionSize = (uint)data.Length * sizeof(float) + 100; // initialized to original size, +100 pour les cas où le compress a du mal à compresser les données
            byte[] lDataCompressed = new byte[lDataCompressionSize];
            // Compression
            var hdl1 = GCHandle.Alloc(data, GCHandleType.Pinned);
            var hdl2 = GCHandle.Alloc(lDataCompressed, GCHandleType.Pinned);
            try
            {
                int x = compress(hdl2.AddrOfPinnedObject(), out lDataCompressionSize, hdl1.AddrOfPinnedObject(), (uint)data.Length * sizeof(float));
                if (x < 0)
                    throw new ApplicationException("unable to compress");

                // return result
                compressedBufferSize = lDataCompressionSize;
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
        /// <param name="originalBufferSize">Original buffer size before compression</param>
        /// <param name="compressedBufferSize">Buffer size after compression, ou 0 si pas compressé</param>
        /// <param name="pData">Byte data buffer</param>
        /// <returns>Byte data buffer uncompressed</returns>
        private float[] Uncompression(uint originalBufferSize, uint compressedBufferSize, byte[] pData)
        {
            // Init
            GCHandle hdl1;
            GCHandle hdl2;

            float[] lDataByteUncompressed = new float[originalBufferSize / 4];

            if (compressedBufferSize == 0)
            {
                // it is a non-compressed buffer
                if (originalBufferSize != pData.Length)
                    throw new ApplicationException("Invalid data length");
                lDataByteUncompressed = ConvertByteToFloat_BlockCopyMethod(pData);
            }
            else
            {
                // Uncompress data
                if (compressedBufferSize != pData.Length)
                    throw new ApplicationException("Invalid data length");

                uint[] lDataUncompressionSize = new uint[1] { originalBufferSize };
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

        /// <summary>
        /// Convert Byte[] to Float[] with Block Copy Method
        /// </summary>
        /// <param name="data">Byte data buffer</param>
        /// <returns></returns>
        private float[] ConvertByteToFloat_BlockCopyMethod(byte[] data)
        {
            // Init
            float[] lDataFloat = null;
            if ((data.Length % 4) == 0)
            {
                uint lFloatDataSize = (uint)(data.Length / sizeof(float));
                // Convert float to byte
                //System.Diagnostics.Debug.Assert(lFloatDataSize >= (uint.MaxValue/4));
                lDataFloat = new float[lFloatDataSize];
                Buffer.BlockCopy(data, 0, lDataFloat, 0, data.Length);
            }
            return lDataFloat;
        }

        /// <summary>
        /// Convert Float[] to Byte[] methods using Buffer.BlockCopy
        /// <param name="data">Float data buffer to convert</param>
        /// <returns>Byte data buffer after conversion</returns>
        private byte[] ConvertFloatToByte_BlockCopyMethod(float[] data)
        {
            byte[] lDataConverted = null;
            if (data != null)
            {
                lDataConverted = new byte[data.Length * sizeof(float)];
                Buffer.BlockCopy(data, 0, lDataConverted, 0, lDataConverted.Length);
            }
            return lDataConverted;
        }

        #endregion Compression/Uncompression methods

        #region BCRF Convertion

        public void ToBCRF_File(string filenameBCRF)
        {
            if (filenameBCRF == null)
                throw new ArgumentNullException("Null filename");

            string extension = Path.GetExtension(filenameBCRF).ToLower();
            if (".bcrf" != extension)
                throw new ArgumentException("Bad Extension filename (expect .BCRF)");

            string targetDir = Path.GetDirectoryName(filenameBCRF);
            if (!string.IsNullOrEmpty(targetDir))
                Directory.CreateDirectory(targetDir);

            System.Text.Encoding encoder = System.Text.Encoding.ASCII;
            using (var fs = new FileStream(filenameBCRF, FileMode.Create))
            {
                ToBRCF(fs);
            }
        }

        public void ToBCRF_Buffer(out byte[] bcrfFileBuffer)
        {
            bcrfFileBuffer = null;
            System.Text.Encoding encoder = System.Text.Encoding.ASCII;
            using (var ms = new MemoryStream())
            {
                ToBRCF(ms);
                bcrfFileBuffer = ms.ToArray();
            }
        }

        private void ToBRCF(Stream s)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture != System.Globalization.CultureInfo.InvariantCulture)
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Text.Encoding encoder = System.Text.Encoding.ASCII;
            using (BinaryWriter bw = new BinaryWriter(s, encoder))
            using (StreamWriter sw = new StreamWriter(s, encoder))
            {
                sw.WriteLine("fileformat = bcrf");
                sw.WriteLine("headersize = 2048");
                sw.WriteLine($"xpixels = {Header.Width}");
                sw.WriteLine($"ypixels = {Header.Height}");
                sw.WriteLine($"xlength =  {Header.Width * Header.AdditionnalHeaderData?.PixelSizeX}");
                sw.WriteLine($"ylength = {Header.Height * Header.AdditionnalHeaderData?.PixelSizeY}");
                sw.WriteLine($"xunit = {Header.AdditionnalHeaderData?.UnitLabelX}");
                sw.WriteLine($"yunit = {Header.AdditionnalHeaderData?.UnitLabelY}");
                sw.WriteLine($"zunit = {Header.AdditionnalHeaderData?.UnitLabelZ}");
                sw.WriteLine($"xlabel = x");
                sw.WriteLine($"ylabel = y");
                sw.WriteLine($"zlabel = z");
                //sw.WriteLine("current = 0.0"); //optional
                //sw.WriteLine("bias = 0.1");//optional
                //sw.WriteLine("starttime = 10 18 93 16:15:55:99");//optional
                //sw.WriteLine("scanspeed = 10.1");//optional
                sw.WriteLine("intelmode = 1"); // 1 indicates that the binary data is written in Little Endian byte order used by Intel processors in PCs. 0 otherwise (unix and motorola)
                sw.WriteLine("bit2nm = 1.0"); // scale factor for scaling the integer height data to nm.
                sw.WriteLine("xoffset = 0.0");
                sw.WriteLine("yoffset = 0.0");
                sw.WriteLine("voidpixels = 0");
                sw.Flush();
                long fspos2 = sw.BaseStream.Position;
                long val = 2048 - fspos2 - 2;
                byte cval = (byte)' ';
                sw.Flush();
                bw.Write(Enumerable.Repeat<byte>(cval, (int)val).ToArray());
                bw.Flush();
                sw.WriteLine();
                sw.Flush();

                for (int c = 0; c < Chunks.Count; c++)
                {
                    float[] chunk = Chunks[c];
                    Byte[] lData = new Byte[chunk.Length * sizeof(float)]; ;
                    Buffer.BlockCopy(chunk, 0, lData, 0, lData.Length);
                    bw.Write(lData);
                    bw.Flush();
                }
                s.SetLength(s.Position);
            }
        }

        public void FromBCRF_File(string filenameBCRF)
        {
            if (filenameBCRF == null)
                throw new ArgumentNullException("Null filename");

            string extension = Path.GetExtension(filenameBCRF).ToLower();
            if (".bcrf" != extension)
                throw new ArgumentException("Bad Extension filename (expect .BCRF)");

            using (var fs = new FileStream(filenameBCRF, FileMode.Open))
            {
                FromBRCF(fs);
            }
        }

        public void FromBCRF_Buffer(byte[] bcrfFileBuffer)
        {
            System.Text.Encoding encoder = System.Text.Encoding.ASCII;
            using (var ms = new MemoryStream(bcrfFileBuffer))
            {
                FromBRCF(ms);
            }
        }

        private void FromBRCF(Stream s)
        {
            ClearData();

            if (System.Threading.Thread.CurrentThread.CurrentCulture != System.Globalization.CultureInfo.InvariantCulture)
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Text.Encoding encoder = System.Text.Encoding.ASCII;
            using (BinaryReader br = new BinaryReader(s, encoder))
            using (StreamReader sr = new StreamReader(s, encoder))
            {
                char[] headerBlock = new char[2048];
                sr.ReadBlock(headerBlock, 0, headerBlock.Length);
                string headerbuff = new string(headerBlock);

                var splitsarray = headerbuff.Split(new char[] { '=', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var cat = new Dictionary<string, string>();

                int mod = splitsarray.Length % 2;
                for (int i = 0; i < (splitsarray.Length - mod); i += 2)
                {
                    cat.Add(splitsarray[i].Trim(), splitsarray[i + 1].Trim());
                }

                int height = 0;
                int width = 0;
                double pixsizX = 0.0;
                double pixsizY = 0.0;
                string xunit = string.Empty;
                string yunit = string.Empty;
                string zunit = string.Empty;

                // Mandatory values
                if (!cat.TryGetValue("xpixels", out string sX))
                    throw new Exception("Cannot find bcrf <xpixels>");
                else if (!int.TryParse(sX, out width))
                    throw new Exception($"Cannot Parse bcrf <xpixels> : <{sX}>");

                if (!cat.TryGetValue("ypixels", out string sY))
                    throw new Exception("Cannot find bcrf <ypixels>");
                else if (!int.TryParse(sY, out height))
                    throw new Exception($"Cannot Parse bcrf <ypixels> : <{sY}>");

                // optionnal values for 3da
                if (cat.TryGetValue("xlength", out string spixsizX))
                {
                    if (!double.TryParse(spixsizX, out pixsizX))
                        throw new Exception($"Cannot Parse bcrf <xlength> : <{spixsizX}>");
                }
                if (cat.TryGetValue("ylength", out string spixsizY))
                {
                    if (!double.TryParse(spixsizY, out pixsizY))
                        throw new Exception($"Cannot Parse bcrf <ylength> : <{spixsizY}>");
                }

                if (!cat.TryGetValue("xunit", out xunit))
                    xunit = String.Empty;

                if (!cat.TryGetValue("yunit", out yunit))
                    yunit = String.Empty;

                if (!cat.TryGetValue("zunit", out zunit))
                    zunit = String.Empty;

                var addData = new AdditionnalHeaderData(pixsizX / width, pixsizY / height, xunit, yunit, zunit);
                Header = new HeaderData(height, width, addData);

                byte[] lbyteData = br.ReadBytes((int)(height * width * sizeof(float)));
                float[] totalfloatBuffer = ConvertByteToFloat_BlockCopyMethod(lbyteData);
                Chunks.Add(totalfloatBuffer);
            }
        }

        #endregion BCRF Convertion
    }
}
