using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using OStorageTools.Ole;

namespace FormatGTR
{
    public class DataGTR
    {

        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress(byte[] dest, out uint destlen, byte[] source, uint sourcelen);

        [DllImport("zlibwapi.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] dest, uint[] destlen, byte[] source, uint sourcelen);


        //----- MEMBERS

        // INPUTS
        //                ^
        //                | 
        //                Y2 
        //                | 
        //                |
        //   ------ X1----+------X2---->     X1< WaferCenter < X2
        //                |
        //                |
        //               Y1
        //                |
        //       Y1 < WaferCenter < Y2
        //
        public float m_fX1_um = 0.0f;        // en coord wafer µm /!\ referenciel centre wafer et non X0/Y0 comme le klarf
        public float m_fX2_um = 0.0f;        // en coord wafer µm /!\ referenciel centre wafer et non X0/Y0 comme le klarf
        public float m_fY1_um = 0.0f;        // en coord wafer µm /!\ referenciel centre wafer et non X0/Y0 comme le klarf
        public float m_fY2_um = 0.0f;        // en coord wafer µm /!\ referenciel centre wafer et non X0/Y0 comme le klarf

        public float m_fEdgeExclusion_um = 0.0f;
        public float m_fRadiusCenterBow_um;  // radius value in µm for Centerbow Calculation

        public List<RectangleF> m_BoxesExclusion; // /!\  in paramertes inputs boxes are express in mm wafer coordinates
                                                  // X and Y as the center of the rectangle but width and height also in mm (but total lenght)
                                                  // WARNING : Boxes stores in file are in microns, X and Y represent the top left corner of rectangle, 
                                                  // and Y axis direction is to the bottom (like image referential, the opposite of the wafer referential)
                                                  // same width and height expressed in Microns

        public int m_nLowPassKernelSize = 3; // 3<=K<=21 //doit etre impaire !!
        public int m_nLowPassKernelType = 0; //0: none; 1:Smooth; 2: uniform; 3:Gaussian 
        public float m_fLowPassKernelGaussianSigma = 1.0f; // pour Low pass gaussian sigma

        public int m_nNSamples = 10; // echantillonage pour calcule du best fit plane

        // VID BOW/WARP
        public double valueTWARPVID { get { return m_fOut_TotalWarp; } }                 // numero VID TWARP
        public double valueMAXPOSWARPVID { get { return m_fOut_MaxPosWarp; } }            // numero VID TWARP
        public double valueMAXNEGWARPVID { get { return m_fOut_MinNegWarp; } }             // numero VID TWARP
        public double valueBOWBFVID { get { return m_fOut_BowBF; } }                  // numero VID TWARP
        public double valueBOXVID { get { return m_fOut_BowX; } }                    // numero VID TWARP
        public double valueBOYVID { get { return m_fOut_BowY; } }                // numero VID TWARP
        public double valueMAXBOWXYVID { get { return m_fOut_BowXY; } }             // numero VID TWARP
        public double valueBOCVID { get { return m_fOut_CenterBow; } }                  // numero VID TWARP


        // OUTPUTS
        public float m_fOut_MaxPosWarp = 0.0f;  //  Max Pos Warp (max Local Warp)
        public float m_fOut_MinNegWarp = 0.0f;  //  Max Neg Warp (min Local Warp)
        public float m_fOut_TotalWarp = 0.0f;   //  Total Warp (max - min)
        public float m_fOut_BowBF = 0.0f;       //  Bow BF (Bestfit Local Warp @center)
        public float m_fOut_BowX = 0.0f;       //  Bow X
        public float m_fOut_BowY = 0.0f;        //  Bow Y
        public float m_fOut_BowXY = 0.0f;       //  Bow XY  = (max (BowX,BowY))
        public float m_fOut_CenterBow = 0.0f;   //  Center Bow

        public float[] m_Out3DData = null;      // Buffer de données 3D
        public int m_Out3DData_SizeX = 0;       // buffer width
        public int m_Out3DData_SizeY = 0;       // buffer height        

        public int m_nWaferSize_mm = 0;         // wafer size in mm
        public double m_dPixelSizeX = 0;        // Pixel size along X axis
        public double m_dPixelSizeY = 0;        // Pixel size along Y axis

        public DataGTR()
        {
            m_BoxesExclusion = null;
        }

        public bool WriteFile(string p_sFilePath, out string p_ErrMsg, bool bCompressData = true)
        {
            p_ErrMsg = string.Empty;
            if (System.IO.File.Exists(p_sFilePath))
            {
                // le fichier existe déjà on l'efface ! 
                try
                {
                    File.Delete(p_sFilePath);
                }
                catch (System.Exception ex)
                {
                    string sexMsg = ex.Message;
                    p_ErrMsg = String.Format("GTR File already exists ! <{0}>\nUnable to delete previous result, check if this file is not use by another application", p_sFilePath);
                    return false;
                }
            }
            using (OleStorage storage = OleStorage.CreateWritableInstance(p_sFilePath))
            {
                if (storage != null)
                {
                    try
                    {
                        // [ROOT]
                        // ->Version
                        // ->Date
                        // ->Settings
                        // ->3DData
                        // ->RowWarpResults  

                        int nLength = 0;
                        byte[] strbuff = null;
                        OleStream oStream = null;

                        /*********************** Version  ********************************/
                        oStream = storage.CreateStream("Version");
                        string sCurrentVersion = "1.0.1";
                        nLength = sCurrentVersion.Length;
                        oStream.WriteInt(nLength);
                        strbuff = ASCIIEncoding.ASCII.GetBytes(sCurrentVersion);
                        oStream.Write(strbuff);
                        oStream.Close(); oStream = null;

                        /*********************** Date  ********************************/
                        oStream = storage.CreateStream("Date");
                        string sDate = String.Format("{0:yyyyMMdd_hhmmss}", DateTime.Now);
                        nLength = sDate.Length;
                        oStream.WriteInt(nLength);
                        strbuff = ASCIIEncoding.ASCII.GetBytes(sDate);
                        oStream.Write(strbuff);
                        oStream.Close(); oStream = null;

                        /*********************** Settings  ********************************/
                        oStream = storage.CreateStream("Settings");

                        oStream.WriteInt(m_nWaferSize_mm);
                        oStream.WriteDouble(m_dPixelSizeX);
                        oStream.WriteDouble(m_dPixelSizeY);

                        oStream.WriteFloat(m_fX1_um);
                        oStream.WriteFloat(m_fX2_um);
                        oStream.WriteFloat(m_fY1_um);
                        oStream.WriteFloat(m_fY2_um);
                        oStream.WriteFloat(m_fEdgeExclusion_um);
                        oStream.WriteFloat(m_fRadiusCenterBow_um);
                        oStream.WriteInt(m_nLowPassKernelSize);
                        oStream.WriteInt(m_nLowPassKernelType);
                        oStream.WriteFloat(m_fLowPassKernelGaussianSigma);
                        oStream.WriteInt(m_nNSamples);
                        if (m_BoxesExclusion == null)
                        {
                            oStream.WriteInt(0);
                        }
                        else
                        {
                            oStream.WriteInt(m_BoxesExclusion.Count);
                            for (int i = 0; i < m_BoxesExclusion.Count; i++)
                            {
                                RectangleF rc_um = m_BoxesExclusion[i];
                                oStream.WriteFloat(rc_um.X);
                                oStream.WriteFloat(rc_um.Y);
                                oStream.WriteFloat(rc_um.Width);
                                oStream.WriteFloat(rc_um.Height);
                            }
                        }
                        oStream.Close(); oStream = null;


                        /*********************** 3DData  ********************************/
                        oStream = storage.CreateStream("3DData");

                        // On respecte le format du 3DD - 3DA cf FloatDataFile
                        // WARNING - normalement ici venant de Demeter on ne devrait pas avoir le cas du depassement memoire- à monitorer avec les ev olution future
                        byte[] DataBuff = Format3DData_Buffer(m_Out3DData, m_Out3DData_SizeX, m_Out3DData_SizeY, bCompressData);
                        if (DataBuff != null)
                            oStream.Write(DataBuff);
                        oStream.Close(); oStream = null;

                        /*********************** RowWarpResults  ********************************/
                        oStream = storage.CreateStream("RowWarpResults");

                        oStream.WriteFloat(m_fOut_MaxPosWarp);
                        oStream.WriteFloat(m_fOut_MinNegWarp);
                        oStream.WriteFloat(m_fOut_TotalWarp);
                        oStream.WriteFloat(m_fOut_BowBF);
                        oStream.WriteFloat(m_fOut_BowX);
                        oStream.WriteFloat(m_fOut_BowY);
                        oStream.WriteFloat(m_fOut_BowXY);
                        oStream.WriteFloat(m_fOut_CenterBow);

                        oStream.Close(); oStream = null;

                    }
                    catch (Exception ex)
                    {
                        p_ErrMsg = "SaveGlobalTopoResultFile Error\n" + ex.Message;
                        return false;
                    }
                }
                else
                {
                    p_ErrMsg = "Could Not create GTR File Storage instance";
                    return false;
                }
            }
            return true;
        }


        private const int FORMAT_VERSION = 1;
        private byte[] Format3DData_Buffer(float[] p_Data, int p_nSizeX, int p_nSizeY, bool p_bCompress)
        {
            // WARNING - normalement ici venant de Demeter on ne devrait pas avoir le cas du depassement memoire- à monitorer avec les ev olution future

            // Format CFloatDataFile :
            // ------
            // int Version			= Numero de version du format de fichier. /!\ c'est un ancienne version 1
            // int image_width 		= Nb colonnes dans buffer de données.
            // int image_height		= Nb lignes dans buffer de données.
            // int Data_Format			= indique la taille element du buffers 1-2-3-4-8 en octets ; c'est un element de confort et de futur compatibilité.
            // int OriginalBufferSize  	= taille en octets du buffer non compressé en général width*height*dataformat.
            // int CompressedBufferSize	= taille en octets du buffer retourné aprés compression par ZlibwApi.dll; Si egal à 0 alors signifie que les données ne sont pas compressées.
            // [
            //     Data Buffer
            // ]

            byte[] bitBuffer = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(FORMAT_VERSION);
                    bw.Write(p_nSizeY);
                    bw.Write(p_nSizeX);
                    bw.Write(sizeof(float));
                    bw.Write(p_nSizeX * p_nSizeY * sizeof(float));

                    uint lDataCompressionSize = 0;
                    Byte[] lByteData = null;
                    Byte[] lDataCompressed = null;
                    lByteData = new Byte[p_Data.Length * sizeof(float)];
                    Buffer.BlockCopy(p_Data, 0, lByteData, 0, lByteData.Length);
                    if (p_bCompress)
                    {
                        lDataCompressed = new byte[lByteData.Length];
                        lDataCompressionSize = (uint)lByteData.Length; // initialized to original size    
                        // Compression
                        compress(lDataCompressed, out lDataCompressionSize, lByteData, (uint)lByteData.Length);
                        Array.Resize(ref lDataCompressed, (int)lDataCompressionSize);
                    }
                    else
                        lDataCompressed = lByteData;

                    bw.Write(lDataCompressionSize); // si compression size == 0 alors c pas compréssé ! ^^
                    bw.Write(lDataCompressed);

                    bw.Close();
                }
                stream.Close();
                bitBuffer = stream.ToArray();
            }

            return bitBuffer;
        }

        public bool ReadFile(string p_sGTRPath, out string p_ErrMsg)
        {
            p_ErrMsg = String.Empty;

            // check if file exist
            if (!File.Exists(p_sGTRPath))
            {
                p_ErrMsg = "This file path {" + p_sGTRPath + "} doesn't exist !";
                return false;
            }

            // check if extension is correct
            string sExt = System.IO.Path.GetExtension(p_sGTRPath);
            if (false == (sExt.Equals(".gtr", StringComparison.InvariantCultureIgnoreCase)))
            {
                p_ErrMsg = "This file extension cannot be read {" + p_sGTRPath + "} !";
                return false;
            }

            string sShortFileName = System.IO.Path.GetFileNameWithoutExtension(p_sGTRPath);
            string sFilePath = System.IO.Path.GetDirectoryName(p_sGTRPath);

            bool bRes = true;
            OleStorage storage = null;
            try
            {
                // [ROOT]
                // ->Version
                // ->Date
                // ->Settings
                // ->3DData
                // ->RowWarpResults  

                storage = OleStorage.CreateInstance(p_sGTRPath);

                byte[] strbuff;
                int nLen = 0;

                /*********************** Version  ********************************/
                string sFileVersion = "#Error#";
                OleStream oStreamVersion = storage.OpenStream("Version");
                if (oStreamVersion != null)
                {
                    nLen = oStreamVersion.ReadInt();
                    strbuff = oStreamVersion.ReadBuffer(nLen);
                    sFileVersion = ASCIIEncoding.ASCII.GetString(strbuff);
                    oStreamVersion.Close();
                }

                /*********************** Date  ********************************/
                //              // Not Useful @this time
                //                 OleStream oStreamDate = storage.OpenStream("Date");
                //                 if (oStreamDate != null)
                //                 {
                //                     nLen = oStreamDate.ReadInt();
                //                     strbuff = oStreamDate.ReadBuffer(nLen);
                //                     String sDate = ASCIIEncoding.ASCII.GetString(strbuff);
                //                     oStreamDate.Close();
                //                 }

                /*********************** Settings  ********************************/
                OleStream oStreamSettings = storage.OpenStream("Settings");
                if (oStreamSettings != null)
                {
                    m_nWaferSize_mm = oStreamSettings.ReadInt();
                    m_dPixelSizeX = oStreamSettings.ReadDouble();
                    m_dPixelSizeY = oStreamSettings.ReadDouble();

                    m_fX1_um = oStreamSettings.ReadFloat();
                    m_fX2_um = oStreamSettings.ReadFloat();
                    m_fY1_um = oStreamSettings.ReadFloat();
                    m_fY2_um = oStreamSettings.ReadFloat();
                    m_fEdgeExclusion_um = oStreamSettings.ReadFloat();

                    // the other settings are not used @this time                  
                    m_fRadiusCenterBow_um = oStreamSettings.ReadFloat();
                    m_nLowPassKernelSize = oStreamSettings.ReadInt();
                    m_nLowPassKernelType = oStreamSettings.ReadInt();
                    m_fLowPassKernelGaussianSigma = oStreamSettings.ReadFloat();
                    m_nNSamples = oStreamSettings.ReadInt();
                    // Box exclusion after that 
                    //int nb rectangleF ReadInt()
                    //pour chaque RectangleF 4x WriteFloat (X,Y,width, height)
                    int nbcount = oStreamSettings.ReadInt();
                    if (nbcount == 0)
                    {
                        m_BoxesExclusion = null;
                    }
                    else
                    {
                        m_BoxesExclusion = new List<RectangleF>(nbcount);
                        for (int i = 0; i < nbcount; i++)
                        {
                            float rcX = oStreamSettings.ReadFloat();
                            float rcY = oStreamSettings.ReadFloat();
                            float rcW = oStreamSettings.ReadFloat();
                            float rcH = oStreamSettings.ReadFloat();
                            m_BoxesExclusion.Add(new RectangleF(rcX, rcY, rcW, rcH));
                        }
                    }


                    oStreamSettings.Close();
                }

                /*********************** 3DData  ********************************/
                OleStream oStream3DData = storage.OpenStream("3DData");
                if (oStream3DData != null)
                {
                    // et là c'est lka merde il faut decoder la stream ici
                    byte[] byteArray = oStream3DData.ReadToEnd();
                    oStream3DData.Close();

                    bRes = DecodeFormat3DData_Buffer(byteArray);
                    if (bRes == false)
                        p_ErrMsg = "Error while decoding 3ddata";
                }

                /*********************** RowWarpResults  ********************************/
                OleStream oStreamRowWarpResults = storage.OpenStream("RowWarpResults");
                if (oStreamRowWarpResults != null)
                {
                    m_fOut_MaxPosWarp = oStreamRowWarpResults.ReadFloat();
                    m_fOut_MinNegWarp = oStreamRowWarpResults.ReadFloat();
                    m_fOut_TotalWarp = oStreamRowWarpResults.ReadFloat();
                    m_fOut_BowBF = oStreamRowWarpResults.ReadFloat();
                    m_fOut_BowX = oStreamRowWarpResults.ReadFloat();
                    m_fOut_BowY = oStreamRowWarpResults.ReadFloat();
                    m_fOut_BowXY = oStreamRowWarpResults.ReadFloat();
                    m_fOut_CenterBow = oStreamRowWarpResults.ReadFloat();
                    oStreamRowWarpResults.Close();
                }
            }
            catch (Exception e)
            {
                p_ErrMsg = String.Format("Exception {0}", e);
                bRes = false;
            }
            finally
            {
                storage.Close();
            }
            return bRes;
        }

        private bool DecodeFormat3DData_Buffer(byte[] p_Buffer)
        {
            bool bres = false;
            using (MemoryStream stream = new MemoryStream(p_Buffer))
            {
                byte[] buffertoDecompress;
                int nVersion;
                int nW;
                int nH;
                int nDataFormat;
                int nOriginalBufferSize;
                int nCompressedBufferSize;
                uint nBufLen;
                bool bCompressed = false;

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    nVersion = reader.ReadInt32();
                    nH = reader.ReadInt32();
                    nW = reader.ReadInt32();
                    nDataFormat = reader.ReadInt32();
                    nOriginalBufferSize = reader.ReadInt32();
                    nCompressedBufferSize = reader.ReadInt32();
                    bCompressed = (nCompressedBufferSize != 0);
                    if (bCompressed)
                        nBufLen = (uint)nCompressedBufferSize;
                    else
                        nBufLen = (uint)nOriginalBufferSize;
                    buffertoDecompress = reader.ReadBytes((int)nBufLen);
                    reader.Close();
                }
                stream.Close();

                // decompress buffer if needed
                byte[] buffer_out;
                if (bCompressed)
                {
                    buffer_out = new byte[nW * nH * sizeof(float)];
                    uint[] destlen = new uint[1];
                    destlen[0] = (uint)(nW * nH * sizeof(float));
                    int nzres = uncompress(buffer_out, destlen, buffertoDecompress, (uint)buffertoDecompress.Length);
                }
                else
                {
                    buffer_out = buffertoDecompress;
                }

                // convert buffer to float
                float[] fDataFloat = new float[nW * nH];
                Buffer.BlockCopy(buffer_out, 0, fDataFloat, 0, buffer_out.Length);

                m_Out3DData = fDataFloat;

                bres = true;
            }

            return bres;
        }

    }
}
