namespace UnitySC.Shared.Data.FormatFile
{
    static public class Converter3DA
    {
        // File to File
        public static void ToBCRF(string filename3da, string filenamebcrf)
        {
            using (var mff = new MatrixFloatFile(filename3da))
            {
                mff.ToBCRF_File(filenamebcrf);
            }
        }
        public static void FromBCRF(string filenamebcrf, string filename3da)
        {
            using (var mff = new MatrixFloatFile())
            {
                mff.FromBCRF_File(filenamebcrf);
                mff.WriteInFile(filename3da, true);
            }
        }

        // Memory to File
        public static void ToBCRF(byte[] filebuffer3da, string filenamebcrf)
        {
            using (var mff = new MatrixFloatFile(filebuffer3da))
            {
                mff.ToBCRF_File(filenamebcrf);
            }
        }
        public static void FromBCRF(byte[] filebufferbcrf, string filename3da)
        {
            using (var mff = new MatrixFloatFile())
            {
                mff.FromBCRF_Buffer(filebufferbcrf);
                mff.WriteInFile(filename3da, true);
            }
        }

        // File to Memory
        public static void ToBCRF(string filename3da, out byte[] filebufferbcrf)
        {
            using (var mff = new MatrixFloatFile(filename3da))
            {
                mff.ToBCRF_Buffer(out filebufferbcrf);
            }
        }
        public static void FromBCRF(string filenamebcrf, out byte[] filebuffer3da)
        {
            using (var mff = new MatrixFloatFile())
            {
                mff.FromBCRF_File(filenamebcrf);
                filebuffer3da = mff.WriteInMemory(true);
            }
        }

        // Memory to Memory
        public static void ToBCRF(byte[] filebuffer3da, byte[] filebufferbcrf)
        {
            using (var mff = new MatrixFloatFile(filebuffer3da))
            {
                mff.ToBCRF_Buffer(out filebufferbcrf);
            }
        }
        public static void FromBCRF(byte[] filebufferbcrf, out byte[] filebuffer3da)
        {
            using (var mff = new MatrixFloatFile())
            {
                mff.FromBCRF_Buffer(filebufferbcrf);
                filebuffer3da = mff.WriteInMemory(true);
            }
        }


    }
}
