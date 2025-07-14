using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Service.Interface
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
    }

    [DataContract(Namespace = "")]
    public class CalibPaths
    {
        [DataMember] public string CamCalibIntrinsicParamsFile;

        [DataMember] public string CamCalibLogSubfolder;

        [DataMember] public string CamCalibSubfolder;

        [DataMember] public string CheckMesureFile;

        [DataMember] public string InputSettingsFile;

        [DataMember] public string LastCalibPath;

        [DataMember] public string MatrixScreenToCamFile;

        [DataMember] public string MatrixWaferToCameraFile;

        [DataMember] public string PhaseReferenceXFile;

        [DataMember] public string PhaseReferenceYFile;

        [DataMember] public string SysCalibSubfolder;

        [DataMember] public string UWMirrorMaskFile;

        [DataMember] public string UWPhiSubfolder;

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
            UWMirrorMaskFile = Marshal.PtrToStringAnsi(calibPaths._UWMirrorMaskFile);
        }

        public string InputSettingsPath(string calibFolder)
        {
            return Path.Combine(calibFolder, InputSettingsFile);
        }

        public string CamCalibFolder(string calibFolder)
        {
            return Path.Combine(calibFolder, CamCalibSubfolder);
        }

        public string SysCalibFolder(string calibFolder)
        {
            return Path.Combine(calibFolder, SysCalibSubfolder);
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
            return Path.Combine(calibFolder, SysCalibSubfolder, PhaseReferenceXFile);
        }

        public string UWMirrorMaskPath(string calibFolder)
        {
            return Path.Combine(calibFolder, UWPhiSubfolder, UWMirrorMaskFile);
        }
    }
}
