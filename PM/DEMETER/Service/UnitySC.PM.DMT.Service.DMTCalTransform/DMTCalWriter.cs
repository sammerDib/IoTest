using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matrox.MatroxImagingLibrary;
using System.IO;

using UnitySC.PM.DMT.Service.DMTCalTransform.Ole;

namespace UnitySC.PM.DMT.Service.DMTCalTransform
{
    public class DMTCalWriter
    {
        protected MIL_ID m_MilSystem;   // MIL system Object 
        protected MIL_ID m_MilCalib;    // MIL calibration OBJECT

        protected int m_nSourceX_px;    // Source Image Width in pixels (image to be redressed) 
        protected int m_nSourceY_px;    // Source Image Height in pixels (image to be redressed) 
        protected int m_nTargetX_px;    // Destination Image Width in pixels (image redressed) 
        protected int m_nTargetY_px;    // Destination Image Height in pixels (image redressed) 
        protected double m_dPixelSize;  // Pixel size of redressed image  

        // For information
        protected double m_dMarginTop_um;  // Top Margin use above calibration wafer in µm
        protected double m_dMarginBottom_um;  // Bottom Margin use below calibration wafer in µm
        protected double m_dMarginRight_um;   // Right Margin use side calibration wafer in µm
        protected double m_dMarginLeft_um;    // Left Margin use below calibration wafer in µm
        protected double m_dWaferSize_um; // wafer diameter size in µm

        public DMTCalWriter(MIL_ID milSystem, MIL_ID milCalib, int nSrcX_px, int nSrcY_px, int nTargetX_px, int nTargetY_px, double dPixelSize, double p_dMarginTop, double p_dMarginBottom, double p_dMarginRight, double p_dMarginLeft, double p_dWaferDiameter = 0.0)
        {
            m_MilSystem = milSystem;
            m_MilCalib = milCalib;
            m_nSourceX_px = nSrcX_px;
            m_nSourceY_px = nSrcY_px;
            m_nTargetX_px = nTargetX_px;
            m_nTargetY_px = nTargetY_px;
            m_dPixelSize = dPixelSize;

            m_dMarginTop_um = p_dMarginTop;
            m_dMarginBottom_um = p_dMarginBottom;
            m_dMarginRight_um = p_dMarginRight;
            m_dMarginLeft_um = p_dMarginLeft;

            m_dWaferSize_um = p_dWaferDiameter;

        }

        public void Save(string filename)
        {
            try
            {
                using (OleStorage storage = OleStorage.CreateWritableInstance(filename))
                {
                    // [ROOT]
                    // ->Version
                    // ->Date
                    // ->Settings
                    // ->MilCalibObj

                    int nLength = 0;
                    byte[] strbuff = null;

                    /*********************** Version  ********************************/
                    using (OleStream oStream = storage.CreateStream("Version"))
                    {
                        string sCurrentVersion = "2.0";
                        nLength = sCurrentVersion.Length;
                        oStream.WriteInt(nLength);
                        strbuff = ASCIIEncoding.ASCII.GetBytes(sCurrentVersion);
                        oStream.Write(strbuff);
                        oStream.Close();
                    }

                    /*********************** Date  ********************************/
                    using (OleStream oStream = storage.CreateStream("Date"))
                    {
                        string sDate = String.Format("{0:yyyyMMdd_hhmmss}", DateTime.Now);
                        nLength = sDate.Length;
                        oStream.WriteInt(nLength);
                        strbuff = ASCIIEncoding.ASCII.GetBytes(sDate);
                        oStream.Write(strbuff);
                        oStream.Close();
                    }

                    /*********************** Settings  ********************************/
                    using (OleStream oStream = storage.CreateStream("Settings"))
                    {
                        oStream.WriteInt(m_nSourceX_px);
                        oStream.WriteInt(m_nSourceY_px);
                        oStream.WriteInt(m_nTargetX_px);
                        oStream.WriteInt(m_nTargetY_px);
                        oStream.WriteDouble(m_dPixelSize);

                        oStream.WriteDouble(m_dMarginTop_um);
                        oStream.WriteDouble(m_dMarginBottom_um);
                        oStream.WriteDouble(m_dMarginRight_um);
                        oStream.WriteDouble(m_dMarginLeft_um);

                        oStream.WriteDouble(m_dWaferSize_um);

                        oStream.Close();
                    }

                    /*********************** MilCalibObj  ********************************/
                    using (OleStream oStream = storage.CreateStream("MilCalibObj"))
                    {
                        // get calibration object size
                        MIL_INT nCalibObjSize = 0; // in bytes
                        MIL.McalStream(MIL.M_NULL, MIL.M_NULL, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_MilCalib, ref nCalibObjSize);

                        // get buffer stream
                        byte[] bufcal = new byte[nCalibObjSize];
                        MIL.McalStream(bufcal, MIL.M_NULL, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_MilCalib, MIL.M_NULL);

                        // write in file
                        oStream.WriteInt((int)nCalibObjSize); // buffer size
                        oStream.Write(bufcal);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to save calibration in file " + filename, ex);
            }
        }

    }
}
