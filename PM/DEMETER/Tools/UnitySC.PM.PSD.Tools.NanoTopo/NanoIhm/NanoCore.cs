using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.IniFile;
using System.IO;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    enum TreatID
    {
        TreatDegauchy = 0,
        TreatPrepareData = 1,
        TreatReconstruct = 2,
        TreatFilter = 3,
        TreatGenerateResults = 4,
    }

    public class NanoCore
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct sCalibPaths
        {
            public IntPtr _LastCalibPath;
            public IntPtr _InputSettingsFile;
            public IntPtr _CheckMesureFile;

            public IntPtr _CamCalibSubfolder;
            public IntPtr _CamCalibIntrinsicParamsFile;
            public IntPtr _CamCalibLogSubfolder;

            public IntPtr _SysCalibSubfolder;
            public IntPtr _MatrixScreenToCamFile;
            public IntPtr _MatrixWaferToCameraFile;
            public IntPtr _PhaseReferenceXFile;
            public IntPtr _PhaseReferenceYFile;

            public IntPtr _UWPhiSubfolder;
            public IntPtr _UWMirrorMaskFile;
        };

        public class CalibPaths
        {
            public string LastCalibPath;
            public string InputSettingsFile;
            public string CheckMesureFile;

            public string CamCalibSubfolder;
            public string CamCalibIntrinsicParamsFile;
            public string CamCalibLogSubfolder;

            public string SysCalibSubfolder;
            public string MatrixScreenToCamFile;
            public string MatrixWaferToCameraFile;
            public string PhaseReferenceXFile;
            public string PhaseReferenceYFile;

            public string UWPhiSubfolder;
            public string UWMirrorMaskFile;

            public CalibPaths(sCalibPaths calibPaths)
            {
                LastCalibPath = Marshal.PtrToStringAnsi(calibPaths._LastCalibPath);
                InputSettingsFile = Marshal.PtrToStringAnsi(calibPaths._InputSettingsFile);
                CheckMesureFile = Marshal.PtrToStringAnsi(calibPaths._CheckMesureFile);

                CamCalibSubfolder = Marshal.PtrToStringAnsi(calibPaths._CamCalibSubfolder);
                CamCalibIntrinsicParamsFile = Marshal.PtrToStringAnsi(calibPaths._CamCalibIntrinsicParamsFile);
                CamCalibLogSubfolder = Marshal.PtrToStringAnsi(calibPaths._CamCalibLogSubfolder);

                SysCalibSubfolder = Marshal.PtrToStringAnsi(calibPaths._SysCalibSubfolder);
                MatrixScreenToCamFile = Marshal.PtrToStringAnsi(calibPaths._MatrixScreenToCamFile);
                MatrixWaferToCameraFile = Marshal.PtrToStringAnsi(calibPaths._MatrixWaferToCameraFile);
                PhaseReferenceXFile = Marshal.PtrToStringAnsi(calibPaths._PhaseReferenceXFile);
                PhaseReferenceYFile = Marshal.PtrToStringAnsi(calibPaths._PhaseReferenceYFile);

                UWPhiSubfolder = Marshal.PtrToStringAnsi(calibPaths._UWPhiSubfolder);
                UWMirrorMaskFile= Marshal.PtrToStringAnsi(calibPaths._UWMirrorMaskFile);
            }

            public string InputSettingsPath(string calibFolder)
            {
                return Path.Combine(calibFolder, InputSettingsFile);
            }

            public string CamCalibIntrinsicParamsPath(string calibFolder)
            {
                return Path.Combine(calibFolder, CamCalibSubfolder, CamCalibIntrinsicParamsFile);
            }

            public string MatrixWaferToCameraPath(string calibFolder)
            {
                return Path.Combine(calibFolder, SysCalibSubfolder, MatrixWaferToCameraFile);
            }

            public string MatrixScreenToCamPath(string calibFolder)
            {
                return Path.Combine(calibFolder, SysCalibSubfolder, MatrixScreenToCamFile);
            }

            public string PhaseReferenceXPath(string calibFolder)
            {
                return Path.Combine(calibFolder, SysCalibSubfolder, PhaseReferenceXFile);
            }

            public string PhaseReferenceYPath(string calibFolder)
            {
                return Path.Combine(calibFolder, SysCalibSubfolder, PhaseReferenceYFile);
            }

            public string UWMirrorMaskPath(string calibFolder)
            {
                return Path.Combine(calibFolder, UWPhiSubfolder, UWMirrorMaskFile);
            }
        }

        [DllImport("NanoCore.dll")]
        public static extern sCalibPaths GetCalibFolderStructure();

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoInit(string calibFolder);
        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoRelease();

        /// <summary>
        /// When loading source images from disk, which format to use.
        /// </summary>
        public enum FilesType { Tiff = -1, Bin, Hbf }

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoLaunchMesure(string p_csIn, string p_csOut, string p_csLotID, int p_nPixelPeriod, string p_csFOUPID, FilesType filesType, bool bUnwrappedPhases, IntPtr phaseX = new IntPtr(), IntPtr phaseY = new IntPtr(), IntPtr phaseMask = new IntPtr());

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoStopMesure(int p_nMode);

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetFilesGeneration(long p_lFlags);

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetTreatmentName(uint p_nTreatmentID, string p_csTreatName);
        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetTreatmentDbgFlag(uint p_nTreatmentID, uint p_uDbgFlag);
        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetTreatmentPrm(uint p_nTreatmentID, string p_csTreatPrmName, string p_csTreatPrmValue);

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetExpandOffsets(int p_nOffsetX, int p_nOffsetY);

        [DllImport("NanoCore.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NanoTopoSetNUI(int p_nNUIEnble);

        public string[] m_sTreatName;
        public uint[] m_uTreatDbgFlags;
        public Dictionary<string, string>[] m_MapPrmTreat;
        public IniFile m_oIniFile;

        public const string g_sIniPath = @"C:\Altasight\Nano\IniRep\NanoTopo.ini";

        public NanoCore()
        {
            m_sTreatName = new string[Enum.GetNames(typeof(TreatID)).Length];
            m_uTreatDbgFlags = new uint[Enum.GetNames(typeof(TreatID)).Length];
            m_MapPrmTreat = new Dictionary<string, string>[Enum.GetNames(typeof(TreatID)).Length];
            for (uint uId = 0; uId < Enum.GetNames(typeof(TreatID)).Length; uId++)
            {
                m_MapPrmTreat[uId] = new Dictionary<string, string>();
            }

            try
            {
                m_oIniFile = new IniFile(g_sIniPath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Fatal Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                // System.Windows.Forms.Application.Exit(); // warning l'exe reste ca kill pas le process
            }

            for (uint uId = 0; uId < Enum.GetNames(typeof(TreatID)).Length; uId++)
            {
                // read treatment Dbg Flag
                string sDbgi = "Dbg" + uId.ToString();
                uint uDbgFlag = (uint)m_oIniFile.IniReadValue("Treatments", sDbgi, 0);
                m_uTreatDbgFlags[uId] = uDbgFlag;

                // read treatment name
                string sTi = "T" + uId.ToString();
                string sTreatName = m_oIniFile.IniReadValue("Treatments", sTi, "");

                if (!String.IsNullOrWhiteSpace(sTreatName))
                {
                    m_sTreatName[uId] = sTreatName;
                    String sTreatNameRdx = System.IO.Path.GetFileNameWithoutExtension(sTreatName);
                    string[] sKeyz = m_oIniFile.ReadSectionKeysNames(sTreatNameRdx);
                    foreach (string sCurKey in sKeyz)
                    {
                        if (!String.IsNullOrEmpty(sCurKey))
                        {
                            string sValue = m_oIniFile.IniReadValue(sTreatNameRdx, sCurKey, "");
                            m_MapPrmTreat[uId].Add(sCurKey, sValue);
                        }
                    }
                }
            }

            ConfigureNanoCoreDLL();
        }

        public void UpdateTreatPrmFromIni()
        {
            for (uint uId = 0; uId < Enum.GetNames(typeof(TreatID)).Length; uId++)
            {
                String sTreatNameRdx = System.IO.Path.GetFileNameWithoutExtension(m_sTreatName[uId]);
                string[] sKeyz = m_oIniFile.ReadSectionKeysNames(sTreatNameRdx);
                foreach (string sCurKey in sKeyz)
                {
                    if (!String.IsNullOrEmpty(sCurKey))
                    {
                        string sValue = m_oIniFile.IniReadValue(sTreatNameRdx, sCurKey, "");
                        (m_MapPrmTreat[uId])[sCurKey] = sValue;
                    }
                }
            }
            ConfigureNanoCoreDLL();
        }

        public void ConfigureNanoCoreDLL()
        {
            for (uint uId = 0; uId < Enum.GetNames(typeof(TreatID)).Length; uId++)
            {
                if (!String.IsNullOrEmpty(m_sTreatName[uId]))
                {
                    NanoTopoSetTreatmentName(uId, m_sTreatName[uId]);
                    NanoTopoSetTreatmentDbgFlag(uId, m_uTreatDbgFlags[uId]);
                    if (m_MapPrmTreat[uId] != null)
                    {
                        foreach (KeyValuePair<string, string> pair in m_MapPrmTreat[uId])
                        {
                            NanoTopoSetTreatmentPrm(uId, pair.Key, pair.Value);
                        }
                    }
                }
            }

        }

        public void InitNanoCoreDLL()
        {
            int nRes = NanoTopoInit(new CalibPaths(GetCalibFolderStructure()).LastCalibPath);
        }

        public void ReleaseNanoCoreDLL()
        {
            int nRes = NanoTopoRelease();
        }

        public int LaunchMesureDLL(string p_csIn, string p_csOut, string p_csLotID, FilesType filesType, int p_nPixelPeriod, string p_csFOUPID, bool bUnwrappedPhases)
        {
            return NanoTopoLaunchMesure(p_csIn, p_csOut, p_csLotID, p_nPixelPeriod, p_csFOUPID, filesType, bUnwrappedPhases);
        }

        public int SetFilesGenerationDLL(long p_lFlags)
        {
            return NanoTopoSetFilesGeneration(p_lFlags);
        }

        public int SetExpandOffsetsDLL(int p_nOffsetX, int p_nOffsetY)
        {
            return NanoTopoSetExpandOffsets(p_nOffsetX, p_nOffsetY);
        }

        public int SetNUIDLL(int p_nNUIEnable)
        {
            return NanoTopoSetNUI(p_nNUIEnable);
        }
        public int EmergencyStop()
        {
            return NanoTopoStopMesure(0);
        }
    }
}
