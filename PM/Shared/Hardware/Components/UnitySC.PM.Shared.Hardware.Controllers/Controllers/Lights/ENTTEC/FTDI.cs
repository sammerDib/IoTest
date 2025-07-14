using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class FTDI
    {
        private const uint FT_OPEN_BY_SERIAL_NUMBER = 1;
        private const uint FT_OPEN_BY_DESCRIPTION = 2;
        private const uint FT_OPEN_BY_LOCATION = 4;
        private const uint FT_DEFAULT_BAUD_RATE = 9600;
        private const uint FT_DEFAULT_DEADMAN_TIMEOUT = 5000;
        private const int FT_COM_PORT_NOT_ASSIGNED = -1;
        private const uint FT_DEFAULT_IN_TRANSFER_SIZE = 4096;
        private const uint FT_DEFAULT_OUT_TRANSFER_SIZE = 4096;
        private const byte FT_DEFAULT_LATENCY = 16;
        private const uint FT_DEFAULT_DEVICE_ID = 67330049;
        private IntPtr ftHandle = IntPtr.Zero;
        private IntPtr hFTD2XXDLL = IntPtr.Zero;
        private IntPtr pFT_CreateDeviceInfoList = IntPtr.Zero;
        private IntPtr pFT_GetDeviceInfoDetail = IntPtr.Zero;
        private IntPtr pFT_Open = IntPtr.Zero;
        private IntPtr pFT_OpenEx = IntPtr.Zero;
        private IntPtr pFT_Close = IntPtr.Zero;
        private IntPtr pFT_Read = IntPtr.Zero;
        private IntPtr pFT_Write = IntPtr.Zero;
        private IntPtr pFT_GetQueueStatus = IntPtr.Zero;
        private IntPtr pFT_GetModemStatus = IntPtr.Zero;
        private IntPtr pFT_GetStatus = IntPtr.Zero;
        private IntPtr pFT_SetBaudRate = IntPtr.Zero;
        private IntPtr pFT_SetDataCharacteristics = IntPtr.Zero;
        private IntPtr pFT_SetFlowControl = IntPtr.Zero;
        private IntPtr pFT_SetDtr = IntPtr.Zero;
        private IntPtr pFT_ClrDtr = IntPtr.Zero;
        private IntPtr pFT_SetRts = IntPtr.Zero;
        private IntPtr pFT_ClrRts = IntPtr.Zero;
        private IntPtr pFT_ResetDevice = IntPtr.Zero;
        private IntPtr pFT_ResetPort = IntPtr.Zero;
        private IntPtr pFT_CyclePort = IntPtr.Zero;
        private IntPtr pFT_Rescan = IntPtr.Zero;
        private IntPtr pFT_Reload = IntPtr.Zero;
        private IntPtr pFT_Purge = IntPtr.Zero;
        private IntPtr pFT_SetTimeouts = IntPtr.Zero;
        private IntPtr pFT_SetBreakOn = IntPtr.Zero;
        private IntPtr pFT_SetBreakOff = IntPtr.Zero;
        private IntPtr pFT_GetDeviceInfo = IntPtr.Zero;
        private IntPtr pFT_SetResetPipeRetryCount = IntPtr.Zero;
        private IntPtr pFT_StopInTask = IntPtr.Zero;
        private IntPtr pFT_RestartInTask = IntPtr.Zero;
        private IntPtr pFT_GetDriverVersion = IntPtr.Zero;
        private IntPtr pFT_GetLibraryVersion = IntPtr.Zero;
        private IntPtr pFT_SetDeadmanTimeout = IntPtr.Zero;
        private IntPtr pFT_SetChars = IntPtr.Zero;
        private IntPtr pFT_SetEventNotification = IntPtr.Zero;
        private IntPtr pFT_GetComPortNumber = IntPtr.Zero;
        private IntPtr pFT_SetLatencyTimer = IntPtr.Zero;
        private IntPtr pFT_GetLatencyTimer = IntPtr.Zero;
        private IntPtr pFT_SetBitMode = IntPtr.Zero;
        private IntPtr pFT_GetBitMode = IntPtr.Zero;
        private IntPtr pFT_SetUSBParameters = IntPtr.Zero;
        private IntPtr pFT_ReadEE = IntPtr.Zero;
        private IntPtr pFT_WriteEE = IntPtr.Zero;
        private IntPtr pFT_EraseEE = IntPtr.Zero;
        private IntPtr pFT_EE_UASize = IntPtr.Zero;
        private IntPtr pFT_EE_UARead = IntPtr.Zero;
        private IntPtr pFT_EE_UAWrite = IntPtr.Zero;
        private IntPtr pFT_EE_Read = IntPtr.Zero;
        private IntPtr pFT_EE_Program = IntPtr.Zero;
        private IntPtr pFT_EEPROM_Read = IntPtr.Zero;
        private IntPtr pFT_EEPROM_Program = IntPtr.Zero;
        private IntPtr pFT_VendorCmdGet = IntPtr.Zero;
        private IntPtr pFT_VendorCmdSet = IntPtr.Zero;
        private IntPtr pFT_VendorCmdSetX = IntPtr.Zero;
        private IntPtr pFT_SetDivisor = IntPtr.Zero;

        /// <summary>Constructor for the FTDI class.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI">`FTDI` on google.com</a></footer>
        public FTDI()
        {
            if (this.hFTD2XXDLL == IntPtr.Zero)
            {
                this.hFTD2XXDLL = FTDI.LoadLibrary("FTD2XX.DLL");
                if (this.hFTD2XXDLL == IntPtr.Zero)
                {
                    Console.WriteLine("Attempting to load FTD2XX.DLL from:\n" + Path.GetDirectoryName(this.GetType().Assembly.Location));
                    this.hFTD2XXDLL = FTDI.LoadLibrary(Path.GetDirectoryName(this.GetType().Assembly.Location) + "\\FTD2XX.DLL");
                }
            }
            if (this.hFTD2XXDLL != IntPtr.Zero)
                this.FindFunctionPointers();
            else
                Console.WriteLine("Failed to load FTD2XX.DLL.  Are the FTDI drivers installed?");
        }

        /// <summary>
        /// Non default constructor allowing passing of string for dll handle.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI">`FTDI` on google.com</a></footer>
        public FTDI(string path)
        {
            if (path == "")
                return;
            if (this.hFTD2XXDLL == IntPtr.Zero)
            {
                this.hFTD2XXDLL = FTDI.LoadLibrary(path);
                if (this.hFTD2XXDLL == IntPtr.Zero)
                    Console.WriteLine("Attempting to load FTD2XX.DLL from:\n" + Path.GetDirectoryName(this.GetType().Assembly.Location));
            }
            if (this.hFTD2XXDLL != IntPtr.Zero)
                this.FindFunctionPointers();
            else
                Console.WriteLine("Failed to load FTD2XX.DLL.  Are the FTDI drivers installed?");
        }

        private void FindFunctionPointers()
        {
            this.pFT_CreateDeviceInfoList = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_CreateDeviceInfoList");
            this.pFT_GetDeviceInfoDetail = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetDeviceInfoDetail");
            this.pFT_Open = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Open");
            this.pFT_OpenEx = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_OpenEx");
            this.pFT_Close = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Close");
            this.pFT_Read = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Read");
            this.pFT_Write = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Write");
            this.pFT_GetQueueStatus = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetQueueStatus");
            this.pFT_GetModemStatus = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetModemStatus");
            this.pFT_GetStatus = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetStatus");
            this.pFT_SetBaudRate = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetBaudRate");
            this.pFT_SetDataCharacteristics = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetDataCharacteristics");
            this.pFT_SetFlowControl = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetFlowControl");
            this.pFT_SetDtr = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetDtr");
            this.pFT_ClrDtr = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_ClrDtr");
            this.pFT_SetRts = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetRts");
            this.pFT_ClrRts = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_ClrRts");
            this.pFT_ResetDevice = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_ResetDevice");
            this.pFT_ResetPort = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_ResetPort");
            this.pFT_CyclePort = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_CyclePort");
            this.pFT_Rescan = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Rescan");
            this.pFT_Reload = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Reload");
            this.pFT_Purge = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_Purge");
            this.pFT_SetTimeouts = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetTimeouts");
            this.pFT_SetBreakOn = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetBreakOn");
            this.pFT_SetBreakOff = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetBreakOff");
            this.pFT_GetDeviceInfo = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetDeviceInfo");
            this.pFT_SetResetPipeRetryCount = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetResetPipeRetryCount");
            this.pFT_StopInTask = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_StopInTask");
            this.pFT_RestartInTask = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_RestartInTask");
            this.pFT_GetDriverVersion = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetDriverVersion");
            this.pFT_GetLibraryVersion = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetLibraryVersion");
            this.pFT_SetDeadmanTimeout = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetDeadmanTimeout");
            this.pFT_SetChars = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetChars");
            this.pFT_SetEventNotification = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetEventNotification");
            this.pFT_GetComPortNumber = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetComPortNumber");
            this.pFT_SetLatencyTimer = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetLatencyTimer");
            this.pFT_GetLatencyTimer = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetLatencyTimer");
            this.pFT_SetBitMode = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetBitMode");
            this.pFT_GetBitMode = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_GetBitMode");
            this.pFT_SetUSBParameters = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetUSBParameters");
            this.pFT_ReadEE = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_ReadEE");
            this.pFT_WriteEE = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_WriteEE");
            this.pFT_EraseEE = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EraseEE");
            this.pFT_EE_UASize = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EE_UASize");
            this.pFT_EE_UARead = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EE_UARead");
            this.pFT_EE_UAWrite = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EE_UAWrite");
            this.pFT_EE_Read = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EE_Read");
            this.pFT_EE_Program = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EE_Program");
            this.pFT_EEPROM_Read = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EEPROM_Read");
            this.pFT_EEPROM_Program = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_EEPROM_Program");
            this.pFT_VendorCmdGet = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_VendorCmdGet");
            this.pFT_VendorCmdSet = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_VendorCmdSet");
            this.pFT_VendorCmdSetX = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_VendorCmdSetX");
            this.pFT_SetDivisor = FTDI.GetProcAddress(this.hFTD2XXDLL, "FT_SetDivisor");
        }

        /// <summary>Destructor for the FTDI class.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Finalize">`FTDI.Finalize` on google.com</a></footer>
        ~FTDI()
        {
            FTDI.FreeLibrary(this.hFTD2XXDLL);
            this.hFTD2XXDLL = IntPtr.Zero;
        }

        /// <summary>
        /// Built-in Windows API functions to allow us to dynamically load our own DLL.
        /// Will allow us to use old versions of the DLL that do not have all of these functions available.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.LoadLibrary">`FTDI.LoadLibrary` on google.com</a></footer>
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>Gets the number of FTDI devices available.</summary>
        /// <returns>FT_STATUS value from FT_CreateDeviceInfoList in FTD2XX.DLL</returns>
        /// <param name="devcount">The number of FTDI devices available.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetNumberOfDevices">`FTDI.GetNumberOfDevices` on google.com</a></footer>
        public FTDI.FT_STATUS GetNumberOfDevices(ref uint devcount)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_CreateDeviceInfoList != IntPtr.Zero)
                ftStatus = ((FTDI.tFT_CreateDeviceInfoList)Marshal.GetDelegateForFunctionPointer(this.pFT_CreateDeviceInfoList, typeof(FTDI.tFT_CreateDeviceInfoList)))(ref devcount);
            else
                Console.WriteLine("Failed to load function FT_CreateDeviceInfoList.");
            return ftStatus;
        }

        /// <summary>
        /// Gets information on all of the FTDI devices available.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetDeviceInfoDetail in FTD2XX.DLL</returns>
        /// <param name="devicelist">An array of type FT_DEVICE_INFO_NODE to contain the device information for all available devices.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the supplied buffer is not large enough to contain the device info list.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetDeviceList">`FTDI.GetDeviceList` on google.com</a></footer>
        public FTDI.FT_STATUS GetDeviceList(FTDI.FT_DEVICE_INFO_NODE[] devicelist)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_CreateDeviceInfoList != IntPtr.Zero & this.pFT_GetDeviceInfoDetail != IntPtr.Zero)
            {
                uint numdevs = 0;
                FTDI.tFT_CreateDeviceInfoList forFunctionPointer1 = (FTDI.tFT_CreateDeviceInfoList)Marshal.GetDelegateForFunctionPointer(this.pFT_CreateDeviceInfoList, typeof(FTDI.tFT_CreateDeviceInfoList));
                FTDI.tFT_GetDeviceInfoDetail forFunctionPointer2 = (FTDI.tFT_GetDeviceInfoDetail)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDeviceInfoDetail, typeof(FTDI.tFT_GetDeviceInfoDetail));
                ftStatus = forFunctionPointer1(ref numdevs);
                byte[] numArray1 = new byte[16];
                byte[] numArray2 = new byte[64];
                if (numdevs > 0U)
                {
                    if ((long)devicelist.Length < (long)numdevs)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_BUFFER_SIZE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    for (uint index = 0; index < numdevs; ++index)
                    {
                        devicelist[index] = new FTDI.FT_DEVICE_INFO_NODE();
                        ftStatus = forFunctionPointer2(index, ref devicelist[index].Flags, ref devicelist[index].Type, ref devicelist[index].ID, ref devicelist[index].LocId, numArray1, numArray2, ref devicelist[index].ftHandle);
                        devicelist[index].SerialNumber = Encoding.ASCII.GetString(numArray1);
                        devicelist[index].Description = Encoding.ASCII.GetString(numArray2);
                        int length1 = devicelist[index].SerialNumber.IndexOf(char.MinValue);
                        if (length1 != -1)
                            devicelist[index].SerialNumber = devicelist[index].SerialNumber.Substring(0, length1);
                        int length2 = devicelist[index].Description.IndexOf(char.MinValue);
                        if (length2 != -1)
                            devicelist[index].Description = devicelist[index].Description.Substring(0, length2);
                    }
                }
            }
            else
            {
                if (this.pFT_CreateDeviceInfoList == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_CreateDeviceInfoList.");
                if (this.pFT_GetDeviceInfoDetail == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_GetDeviceInfoListDetail.");
            }
            return ftStatus;
        }

        /// <summary>Opens the FTDI device with the specified index.</summary>
        /// <returns>FT_STATUS value from FT_Open in FTD2XX.DLL</returns>
        /// <param name="index">Index of the device to open.
        /// Note that this cannot be guaranteed to open a specific device.</param>
        /// <remarks>Initialises the device to 8 data bits, 1 stop bit, no parity, no flow control and 9600 Baud.</remarks>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.OpenByIndex">`FTDI.OpenByIndex` on google.com</a></footer>
        public FTDI.FT_STATUS OpenByIndex(uint index)
        {
            FTDI.FT_STATUS ftStatus1 = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus1;
            if (this.pFT_Open != IntPtr.Zero & this.pFT_SetDataCharacteristics != IntPtr.Zero & this.pFT_SetFlowControl != IntPtr.Zero & this.pFT_SetBaudRate != IntPtr.Zero)
            {
                FTDI.tFT_Open forFunctionPointer1 = (FTDI.tFT_Open)Marshal.GetDelegateForFunctionPointer(this.pFT_Open, typeof(FTDI.tFT_Open));
                FTDI.tFT_SetDataCharacteristics forFunctionPointer2 = (FTDI.tFT_SetDataCharacteristics)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDataCharacteristics, typeof(FTDI.tFT_SetDataCharacteristics));
                FTDI.tFT_SetFlowControl forFunctionPointer3 = (FTDI.tFT_SetFlowControl)Marshal.GetDelegateForFunctionPointer(this.pFT_SetFlowControl, typeof(FTDI.tFT_SetFlowControl));
                FTDI.tFT_SetBaudRate forFunctionPointer4 = (FTDI.tFT_SetBaudRate)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBaudRate, typeof(FTDI.tFT_SetBaudRate));
                ftStatus1 = forFunctionPointer1(index, ref this.ftHandle);
                if (ftStatus1 != FTDI.FT_STATUS.FT_OK)
                    this.ftHandle = IntPtr.Zero;
                if (this.ftHandle != IntPtr.Zero)
                {
                    byte uWordLength = 8;
                    byte uStopBits = 0;
                    byte uParity = 0;
                    FTDI.FT_STATUS ftStatus2 = forFunctionPointer2(this.ftHandle, uWordLength, uStopBits, uParity);
                    ushort usFlowControl = 0;
                    byte uXon = 17;
                    byte uXoff = 19;
                    ftStatus2 = forFunctionPointer3(this.ftHandle, usFlowControl, uXon, uXoff);
                    uint dwBaudRate = 9600;
                    ftStatus1 = forFunctionPointer4(this.ftHandle, dwBaudRate);
                }
            }
            else
            {
                if (this.pFT_Open == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_Open.");
                if (this.pFT_SetDataCharacteristics == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
                if (this.pFT_SetFlowControl == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetFlowControl.");
                if (this.pFT_SetBaudRate == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBaudRate.");
            }
            return ftStatus1;
        }

        /// <summary>
        /// Opens the FTDI device with the specified serial number.
        /// </summary>
        /// <returns>FT_STATUS value from FT_OpenEx in FTD2XX.DLL</returns>
        /// <param name="serialnumber">Serial number of the device to open.</param>
        /// <remarks>Initialises the device to 8 data bits, 1 stop bit, no parity, no flow control and 9600 Baud.</remarks>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.OpenBySerialNumber">`FTDI.OpenBySerialNumber` on google.com</a></footer>
        public FTDI.FT_STATUS OpenBySerialNumber(string serialnumber)
        {
            FTDI.FT_STATUS ftStatus1 = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus1;
            if (this.pFT_OpenEx != IntPtr.Zero & this.pFT_SetDataCharacteristics != IntPtr.Zero & this.pFT_SetFlowControl != IntPtr.Zero & this.pFT_SetBaudRate != IntPtr.Zero)
            {
                FTDI.tFT_OpenEx forFunctionPointer1 = (FTDI.tFT_OpenEx)Marshal.GetDelegateForFunctionPointer(this.pFT_OpenEx, typeof(FTDI.tFT_OpenEx));
                FTDI.tFT_SetDataCharacteristics forFunctionPointer2 = (FTDI.tFT_SetDataCharacteristics)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDataCharacteristics, typeof(FTDI.tFT_SetDataCharacteristics));
                FTDI.tFT_SetFlowControl forFunctionPointer3 = (FTDI.tFT_SetFlowControl)Marshal.GetDelegateForFunctionPointer(this.pFT_SetFlowControl, typeof(FTDI.tFT_SetFlowControl));
                FTDI.tFT_SetBaudRate forFunctionPointer4 = (FTDI.tFT_SetBaudRate)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBaudRate, typeof(FTDI.tFT_SetBaudRate));
                ftStatus1 = forFunctionPointer1(serialnumber, 1U, ref this.ftHandle);
                if (ftStatus1 != FTDI.FT_STATUS.FT_OK)
                    this.ftHandle = IntPtr.Zero;
                if (this.ftHandle != IntPtr.Zero)
                {
                    byte uWordLength = 8;
                    byte uStopBits = 0;
                    byte uParity = 0;
                    FTDI.FT_STATUS ftStatus2 = forFunctionPointer2(this.ftHandle, uWordLength, uStopBits, uParity);
                    ushort usFlowControl = 0;
                    byte uXon = 17;
                    byte uXoff = 19;
                    ftStatus2 = forFunctionPointer3(this.ftHandle, usFlowControl, uXon, uXoff);
                    uint dwBaudRate = 9600;
                    ftStatus1 = forFunctionPointer4(this.ftHandle, dwBaudRate);
                }
            }
            else
            {
                if (this.pFT_OpenEx == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_OpenEx.");
                if (this.pFT_SetDataCharacteristics == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
                if (this.pFT_SetFlowControl == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetFlowControl.");
                if (this.pFT_SetBaudRate == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBaudRate.");
            }
            return ftStatus1;
        }

        /// <summary>
        /// Opens the FTDI device with the specified description.
        /// </summary>
        /// <returns>FT_STATUS value from FT_OpenEx in FTD2XX.DLL</returns>
        /// <param name="description">Description of the device to open.</param>
        /// <remarks>Initialises the device to 8 data bits, 1 stop bit, no parity, no flow control and 9600 Baud.</remarks>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.OpenByDescription">`FTDI.OpenByDescription` on google.com</a></footer>
        public FTDI.FT_STATUS OpenByDescription(string description)
        {
            FTDI.FT_STATUS ftStatus1 = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus1;
            if (this.pFT_OpenEx != IntPtr.Zero & this.pFT_SetDataCharacteristics != IntPtr.Zero & this.pFT_SetFlowControl != IntPtr.Zero & this.pFT_SetBaudRate != IntPtr.Zero)
            {
                FTDI.tFT_OpenEx forFunctionPointer1 = (FTDI.tFT_OpenEx)Marshal.GetDelegateForFunctionPointer(this.pFT_OpenEx, typeof(FTDI.tFT_OpenEx));
                FTDI.tFT_SetDataCharacteristics forFunctionPointer2 = (FTDI.tFT_SetDataCharacteristics)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDataCharacteristics, typeof(FTDI.tFT_SetDataCharacteristics));
                FTDI.tFT_SetFlowControl forFunctionPointer3 = (FTDI.tFT_SetFlowControl)Marshal.GetDelegateForFunctionPointer(this.pFT_SetFlowControl, typeof(FTDI.tFT_SetFlowControl));
                FTDI.tFT_SetBaudRate forFunctionPointer4 = (FTDI.tFT_SetBaudRate)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBaudRate, typeof(FTDI.tFT_SetBaudRate));
                ftStatus1 = forFunctionPointer1(description, 2U, ref this.ftHandle);
                if (ftStatus1 != FTDI.FT_STATUS.FT_OK)
                    this.ftHandle = IntPtr.Zero;
                if (this.ftHandle != IntPtr.Zero)
                {
                    byte uWordLength = 8;
                    byte uStopBits = 0;
                    byte uParity = 0;
                    FTDI.FT_STATUS ftStatus2 = forFunctionPointer2(this.ftHandle, uWordLength, uStopBits, uParity);
                    ushort usFlowControl = 0;
                    byte uXon = 17;
                    byte uXoff = 19;
                    ftStatus2 = forFunctionPointer3(this.ftHandle, usFlowControl, uXon, uXoff);
                    uint dwBaudRate = 9600;
                    ftStatus1 = forFunctionPointer4(this.ftHandle, dwBaudRate);
                }
            }
            else
            {
                if (this.pFT_OpenEx == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_OpenEx.");
                if (this.pFT_SetDataCharacteristics == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
                if (this.pFT_SetFlowControl == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetFlowControl.");
                if (this.pFT_SetBaudRate == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBaudRate.");
            }
            return ftStatus1;
        }

        /// <summary>
        /// Opens the FTDI device at the specified physical location.
        /// </summary>
        /// <returns>FT_STATUS value from FT_OpenEx in FTD2XX.DLL</returns>
        /// <param name="location">Location of the device to open.</param>
        /// <remarks>Initialises the device to 8 data bits, 1 stop bit, no parity, no flow control and 9600 Baud.</remarks>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.OpenByLocation">`FTDI.OpenByLocation` on google.com</a></footer>
        public FTDI.FT_STATUS OpenByLocation(uint location)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_OpenEx != IntPtr.Zero & this.pFT_SetDataCharacteristics != IntPtr.Zero & this.pFT_SetFlowControl != IntPtr.Zero & this.pFT_SetBaudRate != IntPtr.Zero)
            {
                FTDI.tFT_OpenExLoc forFunctionPointer1 = (FTDI.tFT_OpenExLoc)Marshal.GetDelegateForFunctionPointer(this.pFT_OpenEx, typeof(FTDI.tFT_OpenExLoc));
                FTDI.tFT_SetDataCharacteristics forFunctionPointer2 = (FTDI.tFT_SetDataCharacteristics)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDataCharacteristics, typeof(FTDI.tFT_SetDataCharacteristics));
                FTDI.tFT_SetFlowControl forFunctionPointer3 = (FTDI.tFT_SetFlowControl)Marshal.GetDelegateForFunctionPointer(this.pFT_SetFlowControl, typeof(FTDI.tFT_SetFlowControl));
                FTDI.tFT_SetBaudRate forFunctionPointer4 = (FTDI.tFT_SetBaudRate)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBaudRate, typeof(FTDI.tFT_SetBaudRate));
                ftStatus = forFunctionPointer1(location, 4U, ref this.ftHandle);
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    this.ftHandle = IntPtr.Zero;
                if (this.ftHandle != IntPtr.Zero)
                {
                    byte uWordLength = 8;
                    byte uStopBits = 0;
                    byte uParity = 0;
                    int num1 = (int)forFunctionPointer2(this.ftHandle, uWordLength, uStopBits, uParity);
                    ushort usFlowControl = 0;
                    byte uXon = 17;
                    byte uXoff = 19;
                    int num2 = (int)forFunctionPointer3(this.ftHandle, usFlowControl, uXon, uXoff);
                    uint dwBaudRate = 9600;
                    int num3 = (int)forFunctionPointer4(this.ftHandle, dwBaudRate);
                }
            }
            else
            {
                if (this.pFT_OpenEx == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_OpenEx.");
                if (this.pFT_SetDataCharacteristics == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
                if (this.pFT_SetFlowControl == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetFlowControl.");
                if (this.pFT_SetBaudRate == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBaudRate.");
            }
            return ftStatus;
        }

        /// <summary>Closes the handle to an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Close in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Close">`FTDI.Close` on google.com</a></footer>
        public FTDI.FT_STATUS Close()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Close != IntPtr.Zero)
            {
                ftStatus = ((FTDI.tFT_Close)Marshal.GetDelegateForFunctionPointer(this.pFT_Close, typeof(FTDI.tFT_Close)))(this.ftHandle);
                if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    this.ftHandle = IntPtr.Zero;
            }
            else if (this.pFT_Close == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Close.");
            return ftStatus;
        }

        /// <summary>Read data from an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Read in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">An array of bytes which will be populated with the data read from the device.</param>
        /// <param name="numBytesToRead">The number of bytes requested from the device.</param>
        /// <param name="numBytesRead">The number of bytes actually read.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Read">`FTDI.Read` on google.com</a></footer>
        public FTDI.FT_STATUS Read(byte[] dataBuffer, uint numBytesToRead, ref uint numBytesRead)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Read != IntPtr.Zero)
            {
                FTDI.tFT_Read forFunctionPointer = (FTDI.tFT_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_Read, typeof(FTDI.tFT_Read));
                if ((long)dataBuffer.Length < (long)numBytesToRead)
                    numBytesToRead = (uint)dataBuffer.Length;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, dataBuffer, numBytesToRead, ref numBytesRead);
            }
            else if (this.pFT_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Read.");
            return ftStatus;
        }

        /// <summary>Read data from an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Read in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">A string containing the data read</param>
        /// <param name="numBytesToRead">The number of bytes requested from the device.</param>
        /// <param name="numBytesRead">The number of bytes actually read.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Read">`FTDI.Read` on google.com</a></footer>
        public FTDI.FT_STATUS Read(
          out string dataBuffer,
          uint numBytesToRead,
          ref uint numBytesRead)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            dataBuffer = string.Empty;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Read != IntPtr.Zero)
            {
                FTDI.tFT_Read forFunctionPointer = (FTDI.tFT_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_Read, typeof(FTDI.tFT_Read));
                byte[] numArray = new byte[numBytesToRead];
                if (this.ftHandle != IntPtr.Zero)
                {
                    ftStatus = forFunctionPointer(this.ftHandle, numArray, numBytesToRead, ref numBytesRead);
                    dataBuffer = Encoding.ASCII.GetString(numArray);
                    dataBuffer = dataBuffer.Substring(0, (int)numBytesRead);
                }
            }
            else if (this.pFT_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Read.");
            return ftStatus;
        }

        /// <summary>Write data to an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Write in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">An array of bytes which contains the data to be written to the device.</param>
        /// <param name="numBytesToWrite">The number of bytes to be written to the device.</param>
        /// <param name="numBytesWritten">The number of bytes actually written to the device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Write">`FTDI.Write` on google.com</a></footer>
        public FTDI.FT_STATUS Write(
          byte[] dataBuffer,
          int numBytesToWrite,
          ref uint numBytesWritten)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Write != IntPtr.Zero)
            {
                FTDI.tFT_Write forFunctionPointer = (FTDI.tFT_Write)Marshal.GetDelegateForFunctionPointer(this.pFT_Write, typeof(FTDI.tFT_Write));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, dataBuffer, (uint)numBytesToWrite, ref numBytesWritten);
            }
            else if (this.pFT_Write == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Write.");
            return ftStatus;
        }

        /// <summary>Write data to an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Write in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">An array of bytes which contains the data to be written to the device.</param>
        /// <param name="numBytesToWrite">The number of bytes to be written to the device.</param>
        /// <param name="numBytesWritten">The number of bytes actually written to the device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Write">`FTDI.Write` on google.com</a></footer>
        public FTDI.FT_STATUS Write(
          byte[] dataBuffer,
          uint numBytesToWrite,
          ref uint numBytesWritten)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Write != IntPtr.Zero)
            {
                FTDI.tFT_Write forFunctionPointer = (FTDI.tFT_Write)Marshal.GetDelegateForFunctionPointer(this.pFT_Write, typeof(FTDI.tFT_Write));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, dataBuffer, numBytesToWrite, ref numBytesWritten);
            }
            else if (this.pFT_Write == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Write.");
            return ftStatus;
        }

        /// <summary>Write data to an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Write in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">A  string which contains the data to be written to the device.</param>
        /// <param name="numBytesToWrite">The number of bytes to be written to the device.</param>
        /// <param name="numBytesWritten">The number of bytes actually written to the device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Write">`FTDI.Write` on google.com</a></footer>
        public FTDI.FT_STATUS Write(
          string dataBuffer,
          int numBytesToWrite,
          ref uint numBytesWritten)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Write != IntPtr.Zero)
            {
                FTDI.tFT_Write forFunctionPointer = (FTDI.tFT_Write)Marshal.GetDelegateForFunctionPointer(this.pFT_Write, typeof(FTDI.tFT_Write));
                byte[] bytes = Encoding.ASCII.GetBytes(dataBuffer);
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, bytes, (uint)numBytesToWrite, ref numBytesWritten);
            }
            else if (this.pFT_Write == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Write.");
            return ftStatus;
        }

        /// <summary>Write data to an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_Write in FTD2XX.DLL</returns>
        /// <param name="dataBuffer">A  string which contains the data to be written to the device.</param>
        /// <param name="numBytesToWrite">The number of bytes to be written to the device.</param>
        /// <param name="numBytesWritten">The number of bytes actually written to the device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Write">`FTDI.Write` on google.com</a></footer>
        public FTDI.FT_STATUS Write(
          string dataBuffer,
          uint numBytesToWrite,
          ref uint numBytesWritten)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Write != IntPtr.Zero)
            {
                FTDI.tFT_Write forFunctionPointer = (FTDI.tFT_Write)Marshal.GetDelegateForFunctionPointer(this.pFT_Write, typeof(FTDI.tFT_Write));
                byte[] bytes = Encoding.ASCII.GetBytes(dataBuffer);
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, bytes, numBytesToWrite, ref numBytesWritten);
            }
            else if (this.pFT_Write == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Write.");
            return ftStatus;
        }

        /// <summary>Reset an open FTDI device.</summary>
        /// <returns>FT_STATUS value from FT_ResetDevice in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ResetDevice">`FTDI.ResetDevice` on google.com</a></footer>
        public FTDI.FT_STATUS ResetDevice()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_ResetDevice != IntPtr.Zero)
            {
                FTDI.tFT_ResetDevice forFunctionPointer = (FTDI.tFT_ResetDevice)Marshal.GetDelegateForFunctionPointer(this.pFT_ResetDevice, typeof(FTDI.tFT_ResetDevice));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle);
            }
            else if (this.pFT_ResetDevice == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_ResetDevice.");
            return ftStatus;
        }

        /// <summary>
        /// Purge data from the devices transmit and/or receive buffers.
        /// </summary>
        /// <returns>FT_STATUS value from FT_Purge in FTD2XX.DLL</returns>
        /// <param name="purgemask">Specifies which buffer(s) to be purged.  Valid values are any combination of the following flags: FT_PURGE_RX, FT_PURGE_TX</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Purge">`FTDI.Purge` on google.com</a></footer>
        public FTDI.FT_STATUS Purge(uint purgemask)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Purge != IntPtr.Zero)
            {
                FTDI.tFT_Purge forFunctionPointer = (FTDI.tFT_Purge)Marshal.GetDelegateForFunctionPointer(this.pFT_Purge, typeof(FTDI.tFT_Purge));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, purgemask);
            }
            else if (this.pFT_Purge == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Purge.");
            return ftStatus;
        }

        /// <summary>
        /// Set the speed of the output signal, the speed is equal to 3 MHz / divisor for the open DMX interface.
        /// Use a divisor of 12 for DMX control (250 kHz)
        /// </summary>
        public FTDI.FT_STATUS SetDivisor(ushort divisor)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Purge != IntPtr.Zero)
            {
                FTDI.tFT_SetDivisor forFunctionPointer = (FTDI.tFT_SetDivisor)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDivisor, typeof(FTDI.tFT_SetDivisor));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, divisor);
            }
            else if (this.pFT_Purge == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetDivisor.");
            return ftStatus;
        }

        /// <summary>Register for event notification.</summary>
        /// <returns>FT_STATUS value from FT_SetEventNotification in FTD2XX.DLL</returns>
        /// <remarks>After setting event notification, the event can be caught by executing the WaitOne() method of the EventWaitHandle.  If multiple event types are being monitored, the event that fired can be determined from the GetEventType method.</remarks>
        /// <param name="eventmask">The type of events to signal.  Can be any combination of the following: FT_EVENT_RXCHAR, FT_EVENT_MODEM_STATUS, FT_EVENT_LINE_STATUS</param>
        /// <param name="eventhandle">Handle to the event that will receive the notification</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetEventNotification">`FTDI.SetEventNotification` on google.com</a></footer>
        public FTDI.FT_STATUS SetEventNotification(uint eventmask, EventWaitHandle eventhandle)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetEventNotification != IntPtr.Zero)
            {
                FTDI.tFT_SetEventNotification forFunctionPointer = (FTDI.tFT_SetEventNotification)Marshal.GetDelegateForFunctionPointer(this.pFT_SetEventNotification, typeof(FTDI.tFT_SetEventNotification));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, eventmask, (SafeHandle)eventhandle.SafeWaitHandle);
            }
            else if (this.pFT_SetEventNotification == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetEventNotification.");
            return ftStatus;
        }

        /// <summary>Stops the driver issuing USB in requests.</summary>
        /// <returns>FT_STATUS value from FT_StopInTask in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.StopInTask">`FTDI.StopInTask` on google.com</a></footer>
        public FTDI.FT_STATUS StopInTask()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_StopInTask != IntPtr.Zero)
            {
                FTDI.tFT_StopInTask forFunctionPointer = (FTDI.tFT_StopInTask)Marshal.GetDelegateForFunctionPointer(this.pFT_StopInTask, typeof(FTDI.tFT_StopInTask));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle);
            }
            else if (this.pFT_StopInTask == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_StopInTask.");
            return ftStatus;
        }

        /// <summary>Resumes the driver issuing USB in requests.</summary>
        /// <returns>FT_STATUS value from FT_RestartInTask in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.RestartInTask">`FTDI.RestartInTask` on google.com</a></footer>
        public FTDI.FT_STATUS RestartInTask()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_RestartInTask != IntPtr.Zero)
            {
                FTDI.tFT_RestartInTask forFunctionPointer = (FTDI.tFT_RestartInTask)Marshal.GetDelegateForFunctionPointer(this.pFT_RestartInTask, typeof(FTDI.tFT_RestartInTask));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle);
            }
            else if (this.pFT_RestartInTask == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_RestartInTask.");
            return ftStatus;
        }

        /// <summary>Resets the device port.</summary>
        /// <returns>FT_STATUS value from FT_ResetPort in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ResetPort">`FTDI.ResetPort` on google.com</a></footer>
        public FTDI.FT_STATUS ResetPort()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_ResetPort != IntPtr.Zero)
            {
                FTDI.tFT_ResetPort forFunctionPointer = (FTDI.tFT_ResetPort)Marshal.GetDelegateForFunctionPointer(this.pFT_ResetPort, typeof(FTDI.tFT_ResetPort));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle);
            }
            else if (this.pFT_ResetPort == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_ResetPort.");
            return ftStatus;
        }

        /// <summary>
        /// Causes the device to be re-enumerated on the USB bus.  This is equivalent to unplugging and replugging the device.
        /// Also calls FT_Close if FT_CyclePort is successful, so no need to call this separately in the application.
        /// </summary>
        /// <returns>FT_STATUS value from FT_CyclePort in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.CyclePort">`FTDI.CyclePort` on google.com</a></footer>
        public FTDI.FT_STATUS CyclePort()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_CyclePort != IntPtr.Zero & this.pFT_Close != IntPtr.Zero)
            {
                FTDI.tFT_CyclePort forFunctionPointer1 = (FTDI.tFT_CyclePort)Marshal.GetDelegateForFunctionPointer(this.pFT_CyclePort, typeof(FTDI.tFT_CyclePort));
                FTDI.tFT_Close forFunctionPointer2 = (FTDI.tFT_Close)Marshal.GetDelegateForFunctionPointer(this.pFT_Close, typeof(FTDI.tFT_Close));
                if (this.ftHandle != IntPtr.Zero)
                {
                    ftStatus = forFunctionPointer1(this.ftHandle);
                    if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    {
                        ftStatus = forFunctionPointer2(this.ftHandle);
                        if (ftStatus == FTDI.FT_STATUS.FT_OK)
                            this.ftHandle = IntPtr.Zero;
                    }
                }
            }
            else
            {
                if (this.pFT_CyclePort == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_CyclePort.");
                if (this.pFT_Close == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_Close.");
            }
            return ftStatus;
        }

        /// <summary>
        /// Causes the system to check for USB hardware changes.  This is equivalent to clicking on the "Scan for hardware changes" button in the Device Manager.
        /// </summary>
        /// <returns>FT_STATUS value from FT_Rescan in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Rescan">`FTDI.Rescan` on google.com</a></footer>
        public FTDI.FT_STATUS Rescan()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Rescan != IntPtr.Zero)
                ftStatus = ((FTDI.tFT_Rescan)Marshal.GetDelegateForFunctionPointer(this.pFT_Rescan, typeof(FTDI.tFT_Rescan)))();
            else if (this.pFT_Rescan == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Rescan.");
            return ftStatus;
        }

        /// <summary>
        /// Forces a reload of the driver for devices with a specific VID and PID combination.
        /// </summary>
        /// <returns>FT_STATUS value from FT_Reload in FTD2XX.DLL</returns>
        /// <remarks>If the VID and PID parameters are 0, the drivers for USB root hubs will be reloaded, causing all USB devices connected to reload their drivers</remarks>
        /// <param name="VendorID">Vendor ID of the devices to have the driver reloaded</param>
        /// <param name="ProductID">Product ID of the devices to have the driver reloaded</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.Reload">`FTDI.Reload` on google.com</a></footer>
        public FTDI.FT_STATUS Reload(ushort VendorID, ushort ProductID)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_Reload != IntPtr.Zero)
                ftStatus = ((FTDI.tFT_Reload)Marshal.GetDelegateForFunctionPointer(this.pFT_Reload, typeof(FTDI.tFT_Reload)))(VendorID, ProductID);
            else if (this.pFT_Reload == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_Reload.");
            return ftStatus;
        }

        /// <summary>
        /// Puts the device in a mode other than the default UART or FIFO mode.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetBitMode in FTD2XX.DLL</returns>
        /// <param name="Mask">Sets up which bits are inputs and which are outputs.  A bit value of 0 sets the corresponding pin to an input, a bit value of 1 sets the corresponding pin to an output.
        /// In the case of CBUS Bit Bang, the upper nibble of this value controls which pins are inputs and outputs, while the lower nibble controls which of the outputs are high and low.</param>
        /// <param name="BitMode"> For FT232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_CBUS_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL, FT_BIT_MODE_SYNC_FIFO.
        /// For FT2232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL, FT_BIT_MODE_SYNC_FIFO.
        /// For FT4232H devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG.
        /// For FT232R devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_CBUS_BITBANG.
        /// For FT245R devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_SYNC_BITBANG.
        /// For FT2232 devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG, FT_BIT_MODE_MPSSE, FT_BIT_MODE_SYNC_BITBANG, FT_BIT_MODE_MCU_HOST, FT_BIT_MODE_FAST_SERIAL.
        /// For FT232B and FT245B devices, valid values are FT_BIT_MODE_RESET, FT_BIT_MODE_ASYNC_BITBANG.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not support the requested bit mode.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetBitMode">`FTDI.SetBitMode` on google.com</a></footer>
        public FTDI.FT_STATUS SetBitMode(byte Mask, byte BitMode)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetBitMode != IntPtr.Zero)
            {
                FTDI.tFT_SetBitMode forFunctionPointer = (FTDI.tFT_SetBitMode)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBitMode, typeof(FTDI.tFT_SetBitMode));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    switch (DeviceType)
                    {
                        case FTDI.FT_DEVICE.FT_DEVICE_AM:
                            FTDI.FT_ERROR ftErrorCondition1 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                            this.ErrorHandler(ftStatus, ftErrorCondition1);
                            break;

                        case FTDI.FT_DEVICE.FT_DEVICE_100AX:
                            FTDI.FT_ERROR ftErrorCondition2 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                            this.ErrorHandler(ftStatus, ftErrorCondition2);
                            break;

                        default:
                            if (DeviceType == FTDI.FT_DEVICE.FT_DEVICE_BM && BitMode != (byte)0)
                            {
                                if (((int)BitMode & 1) == 0)
                                {
                                    FTDI.FT_ERROR ftErrorCondition3 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition3);
                                    break;
                                }
                                break;
                            }
                            if (DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232 && BitMode != (byte)0)
                            {
                                if (((int)BitMode & 31) == 0)
                                {
                                    FTDI.FT_ERROR ftErrorCondition4 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition4);
                                }
                                if (BitMode == (byte)2 & this.InterfaceIdentifier != "A")
                                {
                                    FTDI.FT_ERROR ftErrorCondition5 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition5);
                                    break;
                                }
                                break;
                            }
                            if (DeviceType == FTDI.FT_DEVICE.FT_DEVICE_232R && BitMode != (byte)0)
                            {
                                if (((int)BitMode & 37) == 0)
                                {
                                    FTDI.FT_ERROR ftErrorCondition6 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition6);
                                    break;
                                }
                                break;
                            }
                            if ((DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232H || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2233HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232HA) && BitMode != (byte)0)
                            {
                                if (((int)BitMode & 95) == 0)
                                {
                                    FTDI.FT_ERROR ftErrorCondition7 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition7);
                                }
                                if ((BitMode == (byte)8 | BitMode == (byte)64) & this.InterfaceIdentifier != "A")
                                {
                                    FTDI.FT_ERROR ftErrorCondition8 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition8);
                                    break;
                                }
                                break;
                            }
                            if ((DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232H || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4233HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232HA) && BitMode != (byte)0)
                            {
                                if (((int)BitMode & 7) == 0)
                                {
                                    FTDI.FT_ERROR ftErrorCondition9 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition9);
                                }
                                if (BitMode == (byte)2 & this.InterfaceIdentifier != "A" & this.InterfaceIdentifier != "B")
                                {
                                    FTDI.FT_ERROR ftErrorCondition10 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                    this.ErrorHandler(ftStatus, ftErrorCondition10);
                                    break;
                                }
                                break;
                            }
                            if ((DeviceType == FTDI.FT_DEVICE.FT_DEVICE_232H || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_232HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_233HP) && BitMode != (byte)0 && BitMode > (byte)64)
                            {
                                FTDI.FT_ERROR ftErrorCondition11 = FTDI.FT_ERROR.FT_INVALID_BITMODE;
                                this.ErrorHandler(ftStatus, ftErrorCondition11);
                                break;
                            }
                            break;
                    }
                    ftStatus = forFunctionPointer(this.ftHandle, Mask, BitMode);
                }
            }
            else if (this.pFT_SetBitMode == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetBitMode.");
            return ftStatus;
        }

        /// <summary>Gets the instantaneous state of the device IO pins.</summary>
        /// <returns>FT_STATUS value from FT_GetBitMode in FTD2XX.DLL</returns>
        /// <param name="BitMode">A bitmap value containing the instantaneous state of the device IO pins</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetPinStates">`FTDI.GetPinStates` on google.com</a></footer>
        public FTDI.FT_STATUS GetPinStates(ref byte BitMode)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetBitMode != IntPtr.Zero)
            {
                FTDI.tFT_GetBitMode forFunctionPointer = (FTDI.tFT_GetBitMode)Marshal.GetDelegateForFunctionPointer(this.pFT_GetBitMode, typeof(FTDI.tFT_GetBitMode));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref BitMode);
            }
            else if (this.pFT_GetBitMode == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetBitMode.");
            return ftStatus;
        }

        /// <summary>
        /// Reads an individual word value from a specified location in the device's EEPROM.
        /// </summary>
        /// <returns>FT_STATUS value from FT_ReadEE in FTD2XX.DLL</returns>
        /// <param name="Address">The EEPROM location to read data from</param>
        /// <param name="EEValue">The WORD value read from the EEPROM location specified in the Address paramter</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadEEPROMLocation">`FTDI.ReadEEPROMLocation` on google.com</a></footer>
        public FTDI.FT_STATUS ReadEEPROMLocation(uint Address, ref ushort EEValue)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_ReadEE != IntPtr.Zero)
            {
                FTDI.tFT_ReadEE forFunctionPointer = (FTDI.tFT_ReadEE)Marshal.GetDelegateForFunctionPointer(this.pFT_ReadEE, typeof(FTDI.tFT_ReadEE));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, Address, ref EEValue);
            }
            else if (this.pFT_ReadEE == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_ReadEE.");
            return ftStatus;
        }

        /// <summary>
        /// Writes an individual word value to a specified location in the device's EEPROM.
        /// </summary>
        /// <returns>FT_STATUS value from FT_WriteEE in FTD2XX.DLL</returns>
        /// <param name="Address">The EEPROM location to read data from</param>
        /// <param name="EEValue">The WORD value to write to the EEPROM location specified by the Address parameter</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteEEPROMLocation">`FTDI.WriteEEPROMLocation` on google.com</a></footer>
        public FTDI.FT_STATUS WriteEEPROMLocation(uint Address, ushort EEValue)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_WriteEE != IntPtr.Zero)
            {
                FTDI.tFT_WriteEE forFunctionPointer = (FTDI.tFT_WriteEE)Marshal.GetDelegateForFunctionPointer(this.pFT_WriteEE, typeof(FTDI.tFT_WriteEE));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, Address, EEValue);
            }
            else if (this.pFT_WriteEE == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_WriteEE.");
            return ftStatus;
        }

        /// <summary>Erases the device EEPROM.</summary>
        /// <returns>FT_STATUS value from FT_EraseEE in FTD2XX.DLL</returns>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when attempting to erase the EEPROM of a device with an internal EEPROM such as an FT232R or FT245R.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.EraseEEPROM">`FTDI.EraseEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS EraseEEPROM()
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EraseEE != IntPtr.Zero)
            {
                FTDI.tFT_EraseEE forFunctionPointer = (FTDI.tFT_EraseEE)Marshal.GetDelegateForFunctionPointer(this.pFT_EraseEE, typeof(FTDI.tFT_EraseEE));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType == FTDI.FT_DEVICE.FT_DEVICE_232R)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    ftStatus = forFunctionPointer(this.ftHandle);
                }
            }
            else if (this.pFT_EraseEE == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EraseEE.");
            return ftStatus;
        }

        /// <summary>
        /// Reads the EEPROM contents of an FT232B or FT245B device.
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Read in FTD2XX DLL</returns>
        /// <param name="ee232b">An FT232B_EEPROM_STRUCTURE which contains only the relevant information for an FT232B and FT245B device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT232BEEPROM">`FTDI.ReadFT232BEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT232BEEPROM(FTDI.FT232B_EEPROM_STRUCTURE ee232b)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_BM)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee232b.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee232b.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee232b.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee232b.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee232b.VendorID = pData.VendorID;
                    ee232b.ProductID = pData.ProductID;
                    ee232b.MaxPower = pData.MaxPower;
                    ee232b.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee232b.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee232b.PullDownEnable = Convert.ToBoolean(pData.PullDownEnable);
                    ee232b.SerNumEnable = Convert.ToBoolean(pData.SerNumEnable);
                    ee232b.USBVersionEnable = Convert.ToBoolean(pData.USBVersionEnable);
                    ee232b.USBVersion = pData.USBVersion;
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>Reads the EEPROM contents of an FT2232 device.</summary>
        /// <returns>FT_STATUS value from FT_EE_Read in FTD2XX DLL</returns>
        /// <param name="ee2232">An FT2232_EEPROM_STRUCTURE which contains only the relevant information for an FT2232 device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT2232EEPROM">`FTDI.ReadFT2232EEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT2232EEPROM(FTDI.FT2232_EEPROM_STRUCTURE ee2232)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_2232)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee2232.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee2232.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee2232.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee2232.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee2232.VendorID = pData.VendorID;
                    ee2232.ProductID = pData.ProductID;
                    ee2232.MaxPower = pData.MaxPower;
                    ee2232.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee2232.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee2232.PullDownEnable = Convert.ToBoolean(pData.PullDownEnable5);
                    ee2232.SerNumEnable = Convert.ToBoolean(pData.SerNumEnable5);
                    ee2232.USBVersionEnable = Convert.ToBoolean(pData.USBVersionEnable5);
                    ee2232.USBVersion = pData.USBVersion5;
                    ee2232.AIsHighCurrent = Convert.ToBoolean(pData.AIsHighCurrent);
                    ee2232.BIsHighCurrent = Convert.ToBoolean(pData.BIsHighCurrent);
                    ee2232.IFAIsFifo = Convert.ToBoolean(pData.IFAIsFifo);
                    ee2232.IFAIsFifoTar = Convert.ToBoolean(pData.IFAIsFifoTar);
                    ee2232.IFAIsFastSer = Convert.ToBoolean(pData.IFAIsFastSer);
                    ee2232.AIsVCP = Convert.ToBoolean(pData.AIsVCP);
                    ee2232.IFBIsFifo = Convert.ToBoolean(pData.IFBIsFifo);
                    ee2232.IFBIsFifoTar = Convert.ToBoolean(pData.IFBIsFifoTar);
                    ee2232.IFBIsFastSer = Convert.ToBoolean(pData.IFBIsFastSer);
                    ee2232.BIsVCP = Convert.ToBoolean(pData.BIsVCP);
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>
        /// Reads the EEPROM contents of an FT232R or FT245R device.
        /// Calls FT_EE_Read in FTD2XX DLL
        /// </summary>
        /// <returns>An FT232R_EEPROM_STRUCTURE which contains only the relevant information for an FT232R and FT245R device.</returns>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT232REEPROM">`FTDI.ReadFT232REEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT232REEPROM(FTDI.FT232R_EEPROM_STRUCTURE ee232r)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_232R)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee232r.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee232r.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee232r.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee232r.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee232r.VendorID = pData.VendorID;
                    ee232r.ProductID = pData.ProductID;
                    ee232r.MaxPower = pData.MaxPower;
                    ee232r.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee232r.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee232r.UseExtOsc = Convert.ToBoolean(pData.UseExtOsc);
                    ee232r.HighDriveIOs = Convert.ToBoolean(pData.HighDriveIOs);
                    ee232r.EndpointSize = pData.EndpointSize;
                    ee232r.PullDownEnable = Convert.ToBoolean(pData.PullDownEnableR);
                    ee232r.SerNumEnable = Convert.ToBoolean(pData.SerNumEnableR);
                    ee232r.InvertTXD = Convert.ToBoolean(pData.InvertTXD);
                    ee232r.InvertRXD = Convert.ToBoolean(pData.InvertRXD);
                    ee232r.InvertRTS = Convert.ToBoolean(pData.InvertRTS);
                    ee232r.InvertCTS = Convert.ToBoolean(pData.InvertCTS);
                    ee232r.InvertDTR = Convert.ToBoolean(pData.InvertDTR);
                    ee232r.InvertDSR = Convert.ToBoolean(pData.InvertDSR);
                    ee232r.InvertDCD = Convert.ToBoolean(pData.InvertDCD);
                    ee232r.InvertRI = Convert.ToBoolean(pData.InvertRI);
                    ee232r.Cbus0 = pData.Cbus0;
                    ee232r.Cbus1 = pData.Cbus1;
                    ee232r.Cbus2 = pData.Cbus2;
                    ee232r.Cbus3 = pData.Cbus3;
                    ee232r.Cbus4 = pData.Cbus4;
                    ee232r.RIsD2XX = Convert.ToBoolean(pData.RIsD2XX);
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>Reads the EEPROM contents of an FT2232H device.</summary>
        /// <returns>FT_STATUS value from FT_EE_Read in FTD2XX DLL</returns>
        /// <param name="ee2232h">An FT2232H_EEPROM_STRUCTURE which contains only the relevant information for an FT2232H device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT2232HEEPROM">`FTDI.ReadFT2232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT2232HEEPROM(FTDI.FT2232H_EEPROM_STRUCTURE ee2232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_2232H)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 3U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee2232h.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee2232h.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee2232h.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee2232h.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee2232h.VendorID = pData.VendorID;
                    ee2232h.ProductID = pData.ProductID;
                    ee2232h.MaxPower = pData.MaxPower;
                    ee2232h.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee2232h.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee2232h.PullDownEnable = Convert.ToBoolean(pData.PullDownEnable7);
                    ee2232h.SerNumEnable = Convert.ToBoolean(pData.SerNumEnable7);
                    ee2232h.ALSlowSlew = Convert.ToBoolean(pData.ALSlowSlew);
                    ee2232h.ALSchmittInput = Convert.ToBoolean(pData.ALSchmittInput);
                    ee2232h.ALDriveCurrent = pData.ALDriveCurrent;
                    ee2232h.AHSlowSlew = Convert.ToBoolean(pData.AHSlowSlew);
                    ee2232h.AHSchmittInput = Convert.ToBoolean(pData.AHSchmittInput);
                    ee2232h.AHDriveCurrent = pData.AHDriveCurrent;
                    ee2232h.BLSlowSlew = Convert.ToBoolean(pData.BLSlowSlew);
                    ee2232h.BLSchmittInput = Convert.ToBoolean(pData.BLSchmittInput);
                    ee2232h.BLDriveCurrent = pData.BLDriveCurrent;
                    ee2232h.BHSlowSlew = Convert.ToBoolean(pData.BHSlowSlew);
                    ee2232h.BHSchmittInput = Convert.ToBoolean(pData.BHSchmittInput);
                    ee2232h.BHDriveCurrent = pData.BHDriveCurrent;
                    ee2232h.IFAIsFifo = Convert.ToBoolean(pData.IFAIsFifo7);
                    ee2232h.IFAIsFifoTar = Convert.ToBoolean(pData.IFAIsFifoTar7);
                    ee2232h.IFAIsFastSer = Convert.ToBoolean(pData.IFAIsFastSer7);
                    ee2232h.AIsVCP = Convert.ToBoolean(pData.AIsVCP7);
                    ee2232h.IFBIsFifo = Convert.ToBoolean(pData.IFBIsFifo7);
                    ee2232h.IFBIsFifoTar = Convert.ToBoolean(pData.IFBIsFifoTar7);
                    ee2232h.IFBIsFastSer = Convert.ToBoolean(pData.IFBIsFastSer7);
                    ee2232h.BIsVCP = Convert.ToBoolean(pData.BIsVCP7);
                    ee2232h.PowerSaveEnable = Convert.ToBoolean(pData.PowerSaveEnable);
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>Reads the EEPROM contents of an FT4232H device.</summary>
        /// <returns>FT_STATUS value from FT_EE_Read in FTD2XX DLL</returns>
        /// <param name="ee4232h">An FT4232H_EEPROM_STRUCTURE which contains only the relevant information for an FT4232H device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT4232HEEPROM">`FTDI.ReadFT4232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT4232HEEPROM(FTDI.FT4232H_EEPROM_STRUCTURE ee4232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_4232H)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 4U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee4232h.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee4232h.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee4232h.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee4232h.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee4232h.VendorID = pData.VendorID;
                    ee4232h.ProductID = pData.ProductID;
                    ee4232h.MaxPower = pData.MaxPower;
                    ee4232h.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee4232h.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee4232h.PullDownEnable = Convert.ToBoolean(pData.PullDownEnable8);
                    ee4232h.SerNumEnable = Convert.ToBoolean(pData.SerNumEnable8);
                    ee4232h.ASlowSlew = Convert.ToBoolean(pData.ASlowSlew);
                    ee4232h.ASchmittInput = Convert.ToBoolean(pData.ASchmittInput);
                    ee4232h.ADriveCurrent = pData.ADriveCurrent;
                    ee4232h.BSlowSlew = Convert.ToBoolean(pData.BSlowSlew);
                    ee4232h.BSchmittInput = Convert.ToBoolean(pData.BSchmittInput);
                    ee4232h.BDriveCurrent = pData.BDriveCurrent;
                    ee4232h.CSlowSlew = Convert.ToBoolean(pData.CSlowSlew);
                    ee4232h.CSchmittInput = Convert.ToBoolean(pData.CSchmittInput);
                    ee4232h.CDriveCurrent = pData.CDriveCurrent;
                    ee4232h.DSlowSlew = Convert.ToBoolean(pData.DSlowSlew);
                    ee4232h.DSchmittInput = Convert.ToBoolean(pData.DSchmittInput);
                    ee4232h.DDriveCurrent = pData.DDriveCurrent;
                    ee4232h.ARIIsTXDEN = Convert.ToBoolean(pData.ARIIsTXDEN);
                    ee4232h.BRIIsTXDEN = Convert.ToBoolean(pData.BRIIsTXDEN);
                    ee4232h.CRIIsTXDEN = Convert.ToBoolean(pData.CRIIsTXDEN);
                    ee4232h.DRIIsTXDEN = Convert.ToBoolean(pData.DRIIsTXDEN);
                    ee4232h.AIsVCP = Convert.ToBoolean(pData.AIsVCP8);
                    ee4232h.BIsVCP = Convert.ToBoolean(pData.BIsVCP8);
                    ee4232h.CIsVCP = Convert.ToBoolean(pData.CIsVCP8);
                    ee4232h.DIsVCP = Convert.ToBoolean(pData.DIsVCP8);
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>Reads the EEPROM contents of an FT232H device.</summary>
        /// <returns>FT_STATUS value from FT_EE_Read in FTD2XX DLL</returns>
        /// <param name="ee232h">An FT232H_EEPROM_STRUCTURE which contains only the relevant information for an FT232H device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadFT232HEEPROM">`FTDI.ReadFT232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadFT232HEEPROM(FTDI.FT232H_EEPROM_STRUCTURE ee232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Read != IntPtr.Zero)
            {
                FTDI.tFT_EE_Read forFunctionPointer = (FTDI.tFT_EE_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Read, typeof(FTDI.tFT_EE_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_232H && DeviceType != FTDI.FT_DEVICE.FT_DEVICE_232HP && DeviceType != FTDI.FT_DEVICE.FT_DEVICE_233HP)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 5U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    ee232h.Manufacturer = Marshal.PtrToStringAnsi(pData.Manufacturer);
                    ee232h.ManufacturerID = Marshal.PtrToStringAnsi(pData.ManufacturerID);
                    ee232h.Description = Marshal.PtrToStringAnsi(pData.Description);
                    ee232h.SerialNumber = Marshal.PtrToStringAnsi(pData.SerialNumber);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                    ee232h.VendorID = pData.VendorID;
                    ee232h.ProductID = pData.ProductID;
                    ee232h.MaxPower = pData.MaxPower;
                    ee232h.SelfPowered = Convert.ToBoolean(pData.SelfPowered);
                    ee232h.RemoteWakeup = Convert.ToBoolean(pData.RemoteWakeup);
                    ee232h.PullDownEnable = Convert.ToBoolean(pData.PullDownEnableH);
                    ee232h.SerNumEnable = Convert.ToBoolean(pData.SerNumEnableH);
                    ee232h.ACSlowSlew = Convert.ToBoolean(pData.ACSlowSlewH);
                    ee232h.ACSchmittInput = Convert.ToBoolean(pData.ACSchmittInputH);
                    ee232h.ACDriveCurrent = pData.ACDriveCurrentH;
                    ee232h.ADSlowSlew = Convert.ToBoolean(pData.ADSlowSlewH);
                    ee232h.ADSchmittInput = Convert.ToBoolean(pData.ADSchmittInputH);
                    ee232h.ADDriveCurrent = pData.ADDriveCurrentH;
                    ee232h.Cbus0 = pData.Cbus0H;
                    ee232h.Cbus1 = pData.Cbus1H;
                    ee232h.Cbus2 = pData.Cbus2H;
                    ee232h.Cbus3 = pData.Cbus3H;
                    ee232h.Cbus4 = pData.Cbus4H;
                    ee232h.Cbus5 = pData.Cbus5H;
                    ee232h.Cbus6 = pData.Cbus6H;
                    ee232h.Cbus7 = pData.Cbus7H;
                    ee232h.Cbus8 = pData.Cbus8H;
                    ee232h.Cbus9 = pData.Cbus9H;
                    ee232h.IsFifo = Convert.ToBoolean(pData.IsFifoH);
                    ee232h.IsFifoTar = Convert.ToBoolean(pData.IsFifoTarH);
                    ee232h.IsFastSer = Convert.ToBoolean(pData.IsFastSerH);
                    ee232h.IsFT1248 = Convert.ToBoolean(pData.IsFT1248H);
                    ee232h.FT1248Cpol = Convert.ToBoolean(pData.FT1248CpolH);
                    ee232h.FT1248Lsb = Convert.ToBoolean(pData.FT1248LsbH);
                    ee232h.FT1248FlowControl = Convert.ToBoolean(pData.FT1248FlowControlH);
                    ee232h.IsVCP = Convert.ToBoolean(pData.IsVCPH);
                    ee232h.PowerSaveEnable = Convert.ToBoolean(pData.PowerSaveEnableH);
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>Reads the EEPROM contents of an X-Series device.</summary>
        /// <returns>FT_STATUS value from FT_EEPROM_Read in FTD2XX DLL</returns>
        /// <param name="eeX">An FT_XSERIES_EEPROM_STRUCTURE which contains only the relevant information for an X-Series device.</param>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ReadXSeriesEEPROM">`FTDI.ReadXSeriesEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS ReadXSeriesEEPROM(FTDI.FT_XSERIES_EEPROM_STRUCTURE eeX)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EEPROM_Read != IntPtr.Zero)
            {
                FTDI.tFT_EEPROM_Read forFunctionPointer = (FTDI.tFT_EEPROM_Read)Marshal.GetDelegateForFunctionPointer(this.pFT_EEPROM_Read, typeof(FTDI.tFT_EEPROM_Read));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_X_SERIES)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    FTDI.FT_XSERIES_DATA ftXseriesData = new FTDI.FT_XSERIES_DATA();
                    FTDI.FT_EEPROM_HEADER ftEepromHeader = new FTDI.FT_EEPROM_HEADER();
                    byte[] numArray1 = new byte[32];
                    byte[] numArray2 = new byte[16];
                    byte[] numArray3 = new byte[64];
                    byte[] numArray4 = new byte[16];
                    ftEepromHeader.deviceType = 9U;
                    ftXseriesData.common = ftEepromHeader;
                    int cb = Marshal.SizeOf((object)ftXseriesData);
                    IntPtr num = Marshal.AllocHGlobal(cb);
                    Marshal.StructureToPtr((object)ftXseriesData, num, false);
                    ftStatus = forFunctionPointer(this.ftHandle, num, (uint)cb, numArray1, numArray2, numArray3, numArray4);
                    if (ftStatus == FTDI.FT_STATUS.FT_OK)
                    {
                        FTDI.FT_XSERIES_DATA structure = (FTDI.FT_XSERIES_DATA)Marshal.PtrToStructure(num, typeof(FTDI.FT_XSERIES_DATA));
                        UTF8Encoding utF8Encoding = new UTF8Encoding();
                        eeX.Manufacturer = utF8Encoding.GetString(numArray1);
                        eeX.ManufacturerID = utF8Encoding.GetString(numArray2);
                        eeX.Description = utF8Encoding.GetString(numArray3);
                        eeX.SerialNumber = utF8Encoding.GetString(numArray4);
                        eeX.VendorID = structure.common.VendorId;
                        eeX.ProductID = structure.common.ProductId;
                        eeX.MaxPower = structure.common.MaxPower;
                        eeX.SelfPowered = Convert.ToBoolean(structure.common.SelfPowered);
                        eeX.RemoteWakeup = Convert.ToBoolean(structure.common.RemoteWakeup);
                        eeX.SerNumEnable = Convert.ToBoolean(structure.common.SerNumEnable);
                        eeX.PullDownEnable = Convert.ToBoolean(structure.common.PullDownEnable);
                        eeX.Cbus0 = structure.Cbus0;
                        eeX.Cbus1 = structure.Cbus1;
                        eeX.Cbus2 = structure.Cbus2;
                        eeX.Cbus3 = structure.Cbus3;
                        eeX.Cbus4 = structure.Cbus4;
                        eeX.Cbus5 = structure.Cbus5;
                        eeX.Cbus6 = structure.Cbus6;
                        eeX.ACDriveCurrent = structure.ACDriveCurrent;
                        eeX.ACSchmittInput = structure.ACSchmittInput;
                        eeX.ACSlowSlew = structure.ACSlowSlew;
                        eeX.ADDriveCurrent = structure.ADDriveCurrent;
                        eeX.ADSchmittInput = structure.ADSchmittInput;
                        eeX.ADSlowSlew = structure.ADSlowSlew;
                        eeX.BCDDisableSleep = structure.BCDDisableSleep;
                        eeX.BCDEnable = structure.BCDEnable;
                        eeX.BCDForceCbusPWREN = structure.BCDForceCbusPWREN;
                        eeX.FT1248Cpol = structure.FT1248Cpol;
                        eeX.FT1248FlowControl = structure.FT1248FlowControl;
                        eeX.FT1248Lsb = structure.FT1248Lsb;
                        eeX.I2CDeviceId = structure.I2CDeviceId;
                        eeX.I2CDisableSchmitt = structure.I2CDisableSchmitt;
                        eeX.I2CSlaveAddress = structure.I2CSlaveAddress;
                        eeX.InvertCTS = structure.InvertCTS;
                        eeX.InvertDCD = structure.InvertDCD;
                        eeX.InvertDSR = structure.InvertDSR;
                        eeX.InvertDTR = structure.InvertDTR;
                        eeX.InvertRI = structure.InvertRI;
                        eeX.InvertRTS = structure.InvertRTS;
                        eeX.InvertRXD = structure.InvertRXD;
                        eeX.InvertTXD = structure.InvertTXD;
                        eeX.PowerSaveEnable = structure.PowerSaveEnable;
                        eeX.RS485EchoSuppress = structure.RS485EchoSuppress;
                        eeX.IsVCP = structure.DriverType;
                    }
                }
            }
            else if (this.pFT_EE_Read == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Read.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT232B or FT245B device.
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee232b">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT232BEEPROM">`FTDI.WriteFT232BEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT232BEEPROM(FTDI.FT232B_EEPROM_STRUCTURE ee232b)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_BM)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee232b.VendorID == (ushort)0 | ee232b.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee232b.Manufacturer.Length > 32)
                        ee232b.Manufacturer = ee232b.Manufacturer.Substring(0, 32);
                    if (ee232b.ManufacturerID.Length > 16)
                        ee232b.ManufacturerID = ee232b.ManufacturerID.Substring(0, 16);
                    if (ee232b.Description.Length > 64)
                        ee232b.Description = ee232b.Description.Substring(0, 64);
                    if (ee232b.SerialNumber.Length > 16)
                        ee232b.SerialNumber = ee232b.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee232b.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232b.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee232b.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee232b.SerialNumber);
                    pData.VendorID = ee232b.VendorID;
                    pData.ProductID = ee232b.ProductID;
                    pData.MaxPower = ee232b.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee232b.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee232b.RemoteWakeup);
                    pData.Rev4 = Convert.ToByte(true);
                    pData.PullDownEnable = Convert.ToByte(ee232b.PullDownEnable);
                    pData.SerNumEnable = Convert.ToByte(ee232b.SerNumEnable);
                    pData.USBVersionEnable = Convert.ToByte(ee232b.USBVersionEnable);
                    pData.USBVersion = ee232b.USBVersion;
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT2232 device.
        /// Calls FT_EE_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee2232">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT2232EEPROM">`FTDI.WriteFT2232EEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT2232EEPROM(FTDI.FT2232_EEPROM_STRUCTURE ee2232)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_2232)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee2232.VendorID == (ushort)0 | ee2232.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee2232.Manufacturer.Length > 32)
                        ee2232.Manufacturer = ee2232.Manufacturer.Substring(0, 32);
                    if (ee2232.ManufacturerID.Length > 16)
                        ee2232.ManufacturerID = ee2232.ManufacturerID.Substring(0, 16);
                    if (ee2232.Description.Length > 64)
                        ee2232.Description = ee2232.Description.Substring(0, 64);
                    if (ee2232.SerialNumber.Length > 16)
                        ee2232.SerialNumber = ee2232.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee2232.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee2232.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee2232.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee2232.SerialNumber);
                    pData.VendorID = ee2232.VendorID;
                    pData.ProductID = ee2232.ProductID;
                    pData.MaxPower = ee2232.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee2232.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee2232.RemoteWakeup);
                    pData.Rev5 = Convert.ToByte(true);
                    pData.PullDownEnable5 = Convert.ToByte(ee2232.PullDownEnable);
                    pData.SerNumEnable5 = Convert.ToByte(ee2232.SerNumEnable);
                    pData.USBVersionEnable5 = Convert.ToByte(ee2232.USBVersionEnable);
                    pData.USBVersion5 = ee2232.USBVersion;
                    pData.AIsHighCurrent = Convert.ToByte(ee2232.AIsHighCurrent);
                    pData.BIsHighCurrent = Convert.ToByte(ee2232.BIsHighCurrent);
                    pData.IFAIsFifo = Convert.ToByte(ee2232.IFAIsFifo);
                    pData.IFAIsFifoTar = Convert.ToByte(ee2232.IFAIsFifoTar);
                    pData.IFAIsFastSer = Convert.ToByte(ee2232.IFAIsFastSer);
                    pData.AIsVCP = Convert.ToByte(ee2232.AIsVCP);
                    pData.IFBIsFifo = Convert.ToByte(ee2232.IFBIsFifo);
                    pData.IFBIsFifoTar = Convert.ToByte(ee2232.IFBIsFifoTar);
                    pData.IFBIsFastSer = Convert.ToByte(ee2232.IFBIsFastSer);
                    pData.BIsVCP = Convert.ToByte(ee2232.BIsVCP);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT232R or FT245R device.
        /// Calls FT_EE_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee232r">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT232REEPROM">`FTDI.WriteFT232REEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT232REEPROM(FTDI.FT232R_EEPROM_STRUCTURE ee232r)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_232R)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee232r.VendorID == (ushort)0 | ee232r.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 2U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee232r.Manufacturer.Length > 32)
                        ee232r.Manufacturer = ee232r.Manufacturer.Substring(0, 32);
                    if (ee232r.ManufacturerID.Length > 16)
                        ee232r.ManufacturerID = ee232r.ManufacturerID.Substring(0, 16);
                    if (ee232r.Description.Length > 64)
                        ee232r.Description = ee232r.Description.Substring(0, 64);
                    if (ee232r.SerialNumber.Length > 16)
                        ee232r.SerialNumber = ee232r.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee232r.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232r.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee232r.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee232r.SerialNumber);
                    pData.VendorID = ee232r.VendorID;
                    pData.ProductID = ee232r.ProductID;
                    pData.MaxPower = ee232r.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee232r.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee232r.RemoteWakeup);
                    pData.PullDownEnableR = Convert.ToByte(ee232r.PullDownEnable);
                    pData.SerNumEnableR = Convert.ToByte(ee232r.SerNumEnable);
                    pData.UseExtOsc = Convert.ToByte(ee232r.UseExtOsc);
                    pData.HighDriveIOs = Convert.ToByte(ee232r.HighDriveIOs);
                    pData.EndpointSize = (byte)64;
                    pData.PullDownEnableR = Convert.ToByte(ee232r.PullDownEnable);
                    pData.SerNumEnableR = Convert.ToByte(ee232r.SerNumEnable);
                    pData.InvertTXD = Convert.ToByte(ee232r.InvertTXD);
                    pData.InvertRXD = Convert.ToByte(ee232r.InvertRXD);
                    pData.InvertRTS = Convert.ToByte(ee232r.InvertRTS);
                    pData.InvertCTS = Convert.ToByte(ee232r.InvertCTS);
                    pData.InvertDTR = Convert.ToByte(ee232r.InvertDTR);
                    pData.InvertDSR = Convert.ToByte(ee232r.InvertDSR);
                    pData.InvertDCD = Convert.ToByte(ee232r.InvertDCD);
                    pData.InvertRI = Convert.ToByte(ee232r.InvertRI);
                    pData.Cbus0 = ee232r.Cbus0;
                    pData.Cbus1 = ee232r.Cbus1;
                    pData.Cbus2 = ee232r.Cbus2;
                    pData.Cbus3 = ee232r.Cbus3;
                    pData.Cbus4 = ee232r.Cbus4;
                    pData.RIsD2XX = Convert.ToByte(ee232r.RIsD2XX);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT2232H device.
        /// Calls FT_EE_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee2232h">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT2232HEEPROM">`FTDI.WriteFT2232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT2232HEEPROM(FTDI.FT2232H_EEPROM_STRUCTURE ee2232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_2232H)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee2232h.VendorID == (ushort)0 | ee2232h.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 3U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee2232h.Manufacturer.Length > 32)
                        ee2232h.Manufacturer = ee2232h.Manufacturer.Substring(0, 32);
                    if (ee2232h.ManufacturerID.Length > 16)
                        ee2232h.ManufacturerID = ee2232h.ManufacturerID.Substring(0, 16);
                    if (ee2232h.Description.Length > 64)
                        ee2232h.Description = ee2232h.Description.Substring(0, 64);
                    if (ee2232h.SerialNumber.Length > 16)
                        ee2232h.SerialNumber = ee2232h.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee2232h.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee2232h.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee2232h.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee2232h.SerialNumber);
                    pData.VendorID = ee2232h.VendorID;
                    pData.ProductID = ee2232h.ProductID;
                    pData.MaxPower = ee2232h.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee2232h.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee2232h.RemoteWakeup);
                    pData.PullDownEnable7 = Convert.ToByte(ee2232h.PullDownEnable);
                    pData.SerNumEnable7 = Convert.ToByte(ee2232h.SerNumEnable);
                    pData.ALSlowSlew = Convert.ToByte(ee2232h.ALSlowSlew);
                    pData.ALSchmittInput = Convert.ToByte(ee2232h.ALSchmittInput);
                    pData.ALDriveCurrent = ee2232h.ALDriveCurrent;
                    pData.AHSlowSlew = Convert.ToByte(ee2232h.AHSlowSlew);
                    pData.AHSchmittInput = Convert.ToByte(ee2232h.AHSchmittInput);
                    pData.AHDriveCurrent = ee2232h.AHDriveCurrent;
                    pData.BLSlowSlew = Convert.ToByte(ee2232h.BLSlowSlew);
                    pData.BLSchmittInput = Convert.ToByte(ee2232h.BLSchmittInput);
                    pData.BLDriveCurrent = ee2232h.BLDriveCurrent;
                    pData.BHSlowSlew = Convert.ToByte(ee2232h.BHSlowSlew);
                    pData.BHSchmittInput = Convert.ToByte(ee2232h.BHSchmittInput);
                    pData.BHDriveCurrent = ee2232h.BHDriveCurrent;
                    pData.IFAIsFifo7 = Convert.ToByte(ee2232h.IFAIsFifo);
                    pData.IFAIsFifoTar7 = Convert.ToByte(ee2232h.IFAIsFifoTar);
                    pData.IFAIsFastSer7 = Convert.ToByte(ee2232h.IFAIsFastSer);
                    pData.AIsVCP7 = Convert.ToByte(ee2232h.AIsVCP);
                    pData.IFBIsFifo7 = Convert.ToByte(ee2232h.IFBIsFifo);
                    pData.IFBIsFifoTar7 = Convert.ToByte(ee2232h.IFBIsFifoTar);
                    pData.IFBIsFastSer7 = Convert.ToByte(ee2232h.IFBIsFastSer);
                    pData.BIsVCP7 = Convert.ToByte(ee2232h.BIsVCP);
                    pData.PowerSaveEnable = Convert.ToByte(ee2232h.PowerSaveEnable);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT4232H device.
        /// Calls FT_EE_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee4232h">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT4232HEEPROM">`FTDI.WriteFT4232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT4232HEEPROM(FTDI.FT4232H_EEPROM_STRUCTURE ee4232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_4232H)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee4232h.VendorID == (ushort)0 | ee4232h.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 4U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee4232h.Manufacturer.Length > 32)
                        ee4232h.Manufacturer = ee4232h.Manufacturer.Substring(0, 32);
                    if (ee4232h.ManufacturerID.Length > 16)
                        ee4232h.ManufacturerID = ee4232h.ManufacturerID.Substring(0, 16);
                    if (ee4232h.Description.Length > 64)
                        ee4232h.Description = ee4232h.Description.Substring(0, 64);
                    if (ee4232h.SerialNumber.Length > 16)
                        ee4232h.SerialNumber = ee4232h.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee4232h.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee4232h.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee4232h.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee4232h.SerialNumber);
                    pData.VendorID = ee4232h.VendorID;
                    pData.ProductID = ee4232h.ProductID;
                    pData.MaxPower = ee4232h.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee4232h.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee4232h.RemoteWakeup);
                    pData.PullDownEnable8 = Convert.ToByte(ee4232h.PullDownEnable);
                    pData.SerNumEnable8 = Convert.ToByte(ee4232h.SerNumEnable);
                    pData.ASlowSlew = Convert.ToByte(ee4232h.ASlowSlew);
                    pData.ASchmittInput = Convert.ToByte(ee4232h.ASchmittInput);
                    pData.ADriveCurrent = ee4232h.ADriveCurrent;
                    pData.BSlowSlew = Convert.ToByte(ee4232h.BSlowSlew);
                    pData.BSchmittInput = Convert.ToByte(ee4232h.BSchmittInput);
                    pData.BDriveCurrent = ee4232h.BDriveCurrent;
                    pData.CSlowSlew = Convert.ToByte(ee4232h.CSlowSlew);
                    pData.CSchmittInput = Convert.ToByte(ee4232h.CSchmittInput);
                    pData.CDriveCurrent = ee4232h.CDriveCurrent;
                    pData.DSlowSlew = Convert.ToByte(ee4232h.DSlowSlew);
                    pData.DSchmittInput = Convert.ToByte(ee4232h.DSchmittInput);
                    pData.DDriveCurrent = ee4232h.DDriveCurrent;
                    pData.ARIIsTXDEN = Convert.ToByte(ee4232h.ARIIsTXDEN);
                    pData.BRIIsTXDEN = Convert.ToByte(ee4232h.BRIIsTXDEN);
                    pData.CRIIsTXDEN = Convert.ToByte(ee4232h.CRIIsTXDEN);
                    pData.DRIIsTXDEN = Convert.ToByte(ee4232h.DRIIsTXDEN);
                    pData.AIsVCP8 = Convert.ToByte(ee4232h.AIsVCP);
                    pData.BIsVCP8 = Convert.ToByte(ee4232h.BIsVCP);
                    pData.CIsVCP8 = Convert.ToByte(ee4232h.CIsVCP);
                    pData.DIsVCP8 = Convert.ToByte(ee4232h.DIsVCP);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an FT232H device.
        /// Calls FT_EE_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EE_Program in FTD2XX DLL</returns>
        /// <param name="ee232h">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteFT232HEEPROM">`FTDI.WriteFT232HEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteFT232HEEPROM(FTDI.FT232H_EEPROM_STRUCTURE ee232h)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_Program != IntPtr.Zero)
            {
                FTDI.tFT_EE_Program forFunctionPointer = (FTDI.tFT_EE_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_Program, typeof(FTDI.tFT_EE_Program));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_232H)
                    {
                        FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                        this.ErrorHandler(ftStatus, ftErrorCondition);
                    }
                    if (ee232h.VendorID == (ushort)0 | ee232h.ProductID == (ushort)0)
                        return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                    FTDI.FT_PROGRAM_DATA pData = new FTDI.FT_PROGRAM_DATA();
                    pData.Signature1 = 0U;
                    pData.Signature2 = uint.MaxValue;
                    pData.Version = 5U;
                    pData.Manufacturer = Marshal.AllocHGlobal(32);
                    pData.ManufacturerID = Marshal.AllocHGlobal(16);
                    pData.Description = Marshal.AllocHGlobal(64);
                    pData.SerialNumber = Marshal.AllocHGlobal(16);
                    if (ee232h.Manufacturer.Length > 32)
                        ee232h.Manufacturer = ee232h.Manufacturer.Substring(0, 32);
                    if (ee232h.ManufacturerID.Length > 16)
                        ee232h.ManufacturerID = ee232h.ManufacturerID.Substring(0, 16);
                    if (ee232h.Description.Length > 64)
                        ee232h.Description = ee232h.Description.Substring(0, 64);
                    if (ee232h.SerialNumber.Length > 16)
                        ee232h.SerialNumber = ee232h.SerialNumber.Substring(0, 16);
                    pData.Manufacturer = Marshal.StringToHGlobalAnsi(ee232h.Manufacturer);
                    pData.ManufacturerID = Marshal.StringToHGlobalAnsi(ee232h.ManufacturerID);
                    pData.Description = Marshal.StringToHGlobalAnsi(ee232h.Description);
                    pData.SerialNumber = Marshal.StringToHGlobalAnsi(ee232h.SerialNumber);
                    pData.VendorID = ee232h.VendorID;
                    pData.ProductID = ee232h.ProductID;
                    pData.MaxPower = ee232h.MaxPower;
                    pData.SelfPowered = Convert.ToUInt16(ee232h.SelfPowered);
                    pData.RemoteWakeup = Convert.ToUInt16(ee232h.RemoteWakeup);
                    pData.PullDownEnableH = Convert.ToByte(ee232h.PullDownEnable);
                    pData.SerNumEnableH = Convert.ToByte(ee232h.SerNumEnable);
                    pData.ACSlowSlewH = Convert.ToByte(ee232h.ACSlowSlew);
                    pData.ACSchmittInputH = Convert.ToByte(ee232h.ACSchmittInput);
                    pData.ACDriveCurrentH = Convert.ToByte(ee232h.ACDriveCurrent);
                    pData.ADSlowSlewH = Convert.ToByte(ee232h.ADSlowSlew);
                    pData.ADSchmittInputH = Convert.ToByte(ee232h.ADSchmittInput);
                    pData.ADDriveCurrentH = Convert.ToByte(ee232h.ADDriveCurrent);
                    pData.Cbus0H = Convert.ToByte(ee232h.Cbus0);
                    pData.Cbus1H = Convert.ToByte(ee232h.Cbus1);
                    pData.Cbus2H = Convert.ToByte(ee232h.Cbus2);
                    pData.Cbus3H = Convert.ToByte(ee232h.Cbus3);
                    pData.Cbus4H = Convert.ToByte(ee232h.Cbus4);
                    pData.Cbus5H = Convert.ToByte(ee232h.Cbus5);
                    pData.Cbus6H = Convert.ToByte(ee232h.Cbus6);
                    pData.Cbus7H = Convert.ToByte(ee232h.Cbus7);
                    pData.Cbus8H = Convert.ToByte(ee232h.Cbus8);
                    pData.Cbus9H = Convert.ToByte(ee232h.Cbus9);
                    pData.IsFifoH = Convert.ToByte(ee232h.IsFifo);
                    pData.IsFifoTarH = Convert.ToByte(ee232h.IsFifoTar);
                    pData.IsFastSerH = Convert.ToByte(ee232h.IsFastSer);
                    pData.IsFT1248H = Convert.ToByte(ee232h.IsFT1248);
                    pData.FT1248CpolH = Convert.ToByte(ee232h.FT1248Cpol);
                    pData.FT1248LsbH = Convert.ToByte(ee232h.FT1248Lsb);
                    pData.FT1248FlowControlH = Convert.ToByte(ee232h.FT1248FlowControl);
                    pData.IsVCPH = Convert.ToByte(ee232h.IsVCP);
                    pData.PowerSaveEnableH = Convert.ToByte(ee232h.PowerSaveEnable);
                    ftStatus = forFunctionPointer(this.ftHandle, pData);
                    Marshal.FreeHGlobal(pData.Manufacturer);
                    Marshal.FreeHGlobal(pData.ManufacturerID);
                    Marshal.FreeHGlobal(pData.Description);
                    Marshal.FreeHGlobal(pData.SerialNumber);
                }
            }
            else if (this.pFT_EE_Program == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_Program.");
            return ftStatus;
        }

        /// <summary>
        /// Writes the specified values to the EEPROM of an X-Series device.
        /// Calls FT_EEPROM_Program in FTD2XX DLL
        /// </summary>
        /// <returns>FT_STATUS value from FT_EEPROM_Program in FTD2XX DLL</returns>
        /// <param name="eeX">The EEPROM settings to be written to the device</param>
        /// <remarks>If the strings are too long, they will be truncated to their maximum permitted lengths</remarks>
        /// <exception cref="T:FTD2XX_NET.FTDI.FT_EXCEPTION">Thrown when the current device does not match the type required by this method.</exception>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.WriteXSeriesEEPROM">`FTDI.WriteXSeriesEEPROM` on google.com</a></footer>
        public FTDI.FT_STATUS WriteXSeriesEEPROM(FTDI.FT_XSERIES_EEPROM_STRUCTURE eeX)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero || !(this.pFT_EEPROM_Program != IntPtr.Zero))
                return ftStatus;
            FTDI.tFT_EEPROM_Program forFunctionPointer = (FTDI.tFT_EEPROM_Program)Marshal.GetDelegateForFunctionPointer(this.pFT_EEPROM_Program, typeof(FTDI.tFT_EEPROM_Program));
            if (this.ftHandle != IntPtr.Zero)
            {
                FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                int deviceType = (int)this.GetDeviceType(ref DeviceType);
                if (DeviceType != FTDI.FT_DEVICE.FT_DEVICE_X_SERIES)
                {
                    FTDI.FT_ERROR ftErrorCondition = FTDI.FT_ERROR.FT_INCORRECT_DEVICE;
                    this.ErrorHandler(ftStatus, ftErrorCondition);
                }
                if (eeX.VendorID == (ushort)0 | eeX.ProductID == (ushort)0)
                    return FTDI.FT_STATUS.FT_INVALID_PARAMETER;
                FTDI.FT_XSERIES_DATA ftXseriesData = new FTDI.FT_XSERIES_DATA();
                byte[] numArray1 = new byte[32];
                byte[] numArray2 = new byte[16];
                byte[] numArray3 = new byte[64];
                byte[] numArray4 = new byte[16];
                if (eeX.Manufacturer.Length > 32)
                    eeX.Manufacturer = eeX.Manufacturer.Substring(0, 32);
                if (eeX.ManufacturerID.Length > 16)
                    eeX.ManufacturerID = eeX.ManufacturerID.Substring(0, 16);
                if (eeX.Description.Length > 64)
                    eeX.Description = eeX.Description.Substring(0, 64);
                if (eeX.SerialNumber.Length > 16)
                    eeX.SerialNumber = eeX.SerialNumber.Substring(0, 16);
                UTF8Encoding utF8Encoding = new UTF8Encoding();
                byte[] bytes1 = utF8Encoding.GetBytes(eeX.Manufacturer);
                byte[] bytes2 = utF8Encoding.GetBytes(eeX.ManufacturerID);
                byte[] bytes3 = utF8Encoding.GetBytes(eeX.Description);
                byte[] bytes4 = utF8Encoding.GetBytes(eeX.SerialNumber);
                ftXseriesData.common.deviceType = 9U;
                ftXseriesData.common.VendorId = eeX.VendorID;
                ftXseriesData.common.ProductId = eeX.ProductID;
                ftXseriesData.common.MaxPower = eeX.MaxPower;
                ftXseriesData.common.SelfPowered = Convert.ToByte(eeX.SelfPowered);
                ftXseriesData.common.RemoteWakeup = Convert.ToByte(eeX.RemoteWakeup);
                ftXseriesData.common.SerNumEnable = Convert.ToByte(eeX.SerNumEnable);
                ftXseriesData.common.PullDownEnable = Convert.ToByte(eeX.PullDownEnable);
                ftXseriesData.Cbus0 = eeX.Cbus0;
                ftXseriesData.Cbus1 = eeX.Cbus1;
                ftXseriesData.Cbus2 = eeX.Cbus2;
                ftXseriesData.Cbus3 = eeX.Cbus3;
                ftXseriesData.Cbus4 = eeX.Cbus4;
                ftXseriesData.Cbus5 = eeX.Cbus5;
                ftXseriesData.Cbus6 = eeX.Cbus6;
                ftXseriesData.ACDriveCurrent = eeX.ACDriveCurrent;
                ftXseriesData.ACSchmittInput = eeX.ACSchmittInput;
                ftXseriesData.ACSlowSlew = eeX.ACSlowSlew;
                ftXseriesData.ADDriveCurrent = eeX.ADDriveCurrent;
                ftXseriesData.ADSchmittInput = eeX.ADSchmittInput;
                ftXseriesData.ADSlowSlew = eeX.ADSlowSlew;
                ftXseriesData.BCDDisableSleep = eeX.BCDDisableSleep;
                ftXseriesData.BCDEnable = eeX.BCDEnable;
                ftXseriesData.BCDForceCbusPWREN = eeX.BCDForceCbusPWREN;
                ftXseriesData.FT1248Cpol = eeX.FT1248Cpol;
                ftXseriesData.FT1248FlowControl = eeX.FT1248FlowControl;
                ftXseriesData.FT1248Lsb = eeX.FT1248Lsb;
                ftXseriesData.I2CDeviceId = eeX.I2CDeviceId;
                ftXseriesData.I2CDisableSchmitt = eeX.I2CDisableSchmitt;
                ftXseriesData.I2CSlaveAddress = eeX.I2CSlaveAddress;
                ftXseriesData.InvertCTS = eeX.InvertCTS;
                ftXseriesData.InvertDCD = eeX.InvertDCD;
                ftXseriesData.InvertDSR = eeX.InvertDSR;
                ftXseriesData.InvertDTR = eeX.InvertDTR;
                ftXseriesData.InvertRI = eeX.InvertRI;
                ftXseriesData.InvertRTS = eeX.InvertRTS;
                ftXseriesData.InvertRXD = eeX.InvertRXD;
                ftXseriesData.InvertTXD = eeX.InvertTXD;
                ftXseriesData.PowerSaveEnable = eeX.PowerSaveEnable;
                ftXseriesData.RS485EchoSuppress = eeX.RS485EchoSuppress;
                ftXseriesData.DriverType = eeX.IsVCP;
                int cb = Marshal.SizeOf((object)ftXseriesData);
                IntPtr num = Marshal.AllocHGlobal(cb);
                Marshal.StructureToPtr((object)ftXseriesData, num, false);
                ftStatus = forFunctionPointer(this.ftHandle, num, (uint)cb, bytes1, bytes2, bytes3, bytes4);
            }
            return ftStatus;
        }

        /// <summary>Reads data from the user area of the device EEPROM.</summary>
        /// <returns>FT_STATUS from FT_UARead in FTD2XX.DLL</returns>
        /// <param name="UserAreaDataBuffer">An array of bytes which will be populated with the data read from the device EEPROM user area.</param>
        /// <param name="numBytesRead">The number of bytes actually read from the EEPROM user area.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.EEReadUserArea">`FTDI.EEReadUserArea` on google.com</a></footer>
        public FTDI.FT_STATUS EEReadUserArea(byte[] UserAreaDataBuffer, ref uint numBytesRead)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_UASize != IntPtr.Zero & this.pFT_EE_UARead != IntPtr.Zero)
            {
                FTDI.tFT_EE_UASize forFunctionPointer1 = (FTDI.tFT_EE_UASize)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_UASize, typeof(FTDI.tFT_EE_UASize));
                FTDI.tFT_EE_UARead forFunctionPointer2 = (FTDI.tFT_EE_UARead)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_UARead, typeof(FTDI.tFT_EE_UARead));
                if (this.ftHandle != IntPtr.Zero)
                {
                    uint dwSize = 0;
                    ftStatus = forFunctionPointer1(this.ftHandle, ref dwSize);
                    if ((long)UserAreaDataBuffer.Length >= (long)dwSize)
                        ftStatus = forFunctionPointer2(this.ftHandle, UserAreaDataBuffer, UserAreaDataBuffer.Length, ref numBytesRead);
                }
            }
            else
            {
                if (this.pFT_EE_UASize == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_EE_UASize.");
                if (this.pFT_EE_UARead == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_EE_UARead.");
            }
            return ftStatus;
        }

        /// <summary>Writes data to the user area of the device EEPROM.</summary>
        /// <returns>FT_STATUS value from FT_UAWrite in FTD2XX.DLL</returns>
        /// <param name="UserAreaDataBuffer">An array of bytes which will be written to the device EEPROM user area.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.EEWriteUserArea">`FTDI.EEWriteUserArea` on google.com</a></footer>
        public FTDI.FT_STATUS EEWriteUserArea(byte[] UserAreaDataBuffer)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_UASize != IntPtr.Zero & this.pFT_EE_UAWrite != IntPtr.Zero)
            {
                FTDI.tFT_EE_UASize forFunctionPointer1 = (FTDI.tFT_EE_UASize)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_UASize, typeof(FTDI.tFT_EE_UASize));
                FTDI.tFT_EE_UAWrite forFunctionPointer2 = (FTDI.tFT_EE_UAWrite)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_UAWrite, typeof(FTDI.tFT_EE_UAWrite));
                if (this.ftHandle != IntPtr.Zero)
                {
                    uint dwSize = 0;
                    ftStatus = forFunctionPointer1(this.ftHandle, ref dwSize);
                    if ((long)UserAreaDataBuffer.Length <= (long)dwSize)
                        ftStatus = forFunctionPointer2(this.ftHandle, UserAreaDataBuffer, UserAreaDataBuffer.Length);
                }
            }
            else
            {
                if (this.pFT_EE_UASize == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_EE_UASize.");
                if (this.pFT_EE_UAWrite == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_EE_UAWrite.");
            }
            return ftStatus;
        }

        /// <summary>Gets the chip type of the current device.</summary>
        /// <returns>FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL</returns>
        /// <param name="DeviceType">The FTDI chip type of the current device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetDeviceType">`FTDI.GetDeviceType` on google.com</a></footer>
        public FTDI.FT_STATUS GetDeviceType(ref FTDI.FT_DEVICE DeviceType)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetDeviceInfo != IntPtr.Zero)
            {
                FTDI.tFT_GetDeviceInfo forFunctionPointer = (FTDI.tFT_GetDeviceInfo)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDeviceInfo, typeof(FTDI.tFT_GetDeviceInfo));
                uint lpdwID = 0;
                byte[] pcSerialNumber = new byte[16];
                byte[] pcDescription = new byte[64];
                DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref DeviceType, ref lpdwID, pcSerialNumber, pcDescription, IntPtr.Zero);
            }
            else if (this.pFT_GetDeviceInfo == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the Vendor ID and Product ID of the current device.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL</returns>
        /// <param name="DeviceID">The device ID (Vendor ID and Product ID) of the current device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetDeviceID">`FTDI.GetDeviceID` on google.com</a></footer>
        public FTDI.FT_STATUS GetDeviceID(ref uint DeviceID)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetDeviceInfo != IntPtr.Zero)
            {
                FTDI.tFT_GetDeviceInfo forFunctionPointer = (FTDI.tFT_GetDeviceInfo)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDeviceInfo, typeof(FTDI.tFT_GetDeviceInfo));
                FTDI.FT_DEVICE pftType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                byte[] pcSerialNumber = new byte[16];
                byte[] pcDescription = new byte[64];
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref pftType, ref DeviceID, pcSerialNumber, pcDescription, IntPtr.Zero);
            }
            else if (this.pFT_GetDeviceInfo == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
            return ftStatus;
        }

        /// <summary>Gets the description of the current device.</summary>
        /// <returns>FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL</returns>
        /// <param name="Description">The description of the current device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetDescription">`FTDI.GetDescription` on google.com</a></footer>
        public FTDI.FT_STATUS GetDescription(out string Description)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            Description = string.Empty;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetDeviceInfo != IntPtr.Zero)
            {
                FTDI.tFT_GetDeviceInfo forFunctionPointer = (FTDI.tFT_GetDeviceInfo)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDeviceInfo, typeof(FTDI.tFT_GetDeviceInfo));
                uint lpdwID = 0;
                FTDI.FT_DEVICE pftType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                byte[] pcSerialNumber = new byte[16];
                byte[] numArray = new byte[64];
                if (this.ftHandle != IntPtr.Zero)
                {
                    ftStatus = forFunctionPointer(this.ftHandle, ref pftType, ref lpdwID, pcSerialNumber, numArray, IntPtr.Zero);
                    Description = Encoding.ASCII.GetString(numArray);
                    Description = Description.Substring(0, Description.IndexOf(char.MinValue));
                }
            }
            else if (this.pFT_GetDeviceInfo == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
            return ftStatus;
        }

        /// <summary>Gets the serial number of the current device.</summary>
        /// <returns>FT_STATUS value from FT_GetDeviceInfo in FTD2XX.DLL</returns>
        /// <param name="SerialNumber">The serial number of the current device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetSerialNumber">`FTDI.GetSerialNumber` on google.com</a></footer>
        public FTDI.FT_STATUS GetSerialNumber(out string SerialNumber)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            SerialNumber = string.Empty;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetDeviceInfo != IntPtr.Zero)
            {
                FTDI.tFT_GetDeviceInfo forFunctionPointer = (FTDI.tFT_GetDeviceInfo)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDeviceInfo, typeof(FTDI.tFT_GetDeviceInfo));
                uint lpdwID = 0;
                FTDI.FT_DEVICE pftType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                byte[] numArray = new byte[16];
                byte[] pcDescription = new byte[64];
                if (this.ftHandle != IntPtr.Zero)
                {
                    ftStatus = forFunctionPointer(this.ftHandle, ref pftType, ref lpdwID, numArray, pcDescription, IntPtr.Zero);
                    SerialNumber = Encoding.ASCII.GetString(numArray);
                    SerialNumber = SerialNumber.Substring(0, SerialNumber.IndexOf(char.MinValue));
                }
            }
            else if (this.pFT_GetDeviceInfo == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetDeviceInfo.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the number of bytes available in the receive buffer.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetQueueStatus in FTD2XX.DLL</returns>
        /// <param name="RxQueue">The number of bytes available to be read.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetRxBytesAvailable">`FTDI.GetRxBytesAvailable` on google.com</a></footer>
        public FTDI.FT_STATUS GetRxBytesAvailable(ref uint RxQueue)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetQueueStatus != IntPtr.Zero)
            {
                FTDI.tFT_GetQueueStatus forFunctionPointer = (FTDI.tFT_GetQueueStatus)Marshal.GetDelegateForFunctionPointer(this.pFT_GetQueueStatus, typeof(FTDI.tFT_GetQueueStatus));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref RxQueue);
            }
            else if (this.pFT_GetQueueStatus == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetQueueStatus.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the number of bytes waiting in the transmit buffer.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetStatus in FTD2XX.DLL</returns>
        /// <param name="TxQueue">The number of bytes waiting to be sent.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetTxBytesWaiting">`FTDI.GetTxBytesWaiting` on google.com</a></footer>
        public FTDI.FT_STATUS GetTxBytesWaiting(ref uint TxQueue)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetStatus != IntPtr.Zero)
            {
                FTDI.tFT_GetStatus forFunctionPointer = (FTDI.tFT_GetStatus)Marshal.GetDelegateForFunctionPointer(this.pFT_GetStatus, typeof(FTDI.tFT_GetStatus));
                uint lpdwAmountInRxQueue = 0;
                uint lpdwEventStatus = 0;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref lpdwAmountInRxQueue, ref TxQueue, ref lpdwEventStatus);
            }
            else if (this.pFT_GetStatus == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetStatus.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the event type after an event has fired.  Can be used to distinguish which event has been triggered when waiting on multiple event types.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetStatus in FTD2XX.DLL</returns>
        /// <param name="EventType">The type of event that has occurred.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetEventType">`FTDI.GetEventType` on google.com</a></footer>
        public FTDI.FT_STATUS GetEventType(ref uint EventType)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetStatus != IntPtr.Zero)
            {
                FTDI.tFT_GetStatus forFunctionPointer = (FTDI.tFT_GetStatus)Marshal.GetDelegateForFunctionPointer(this.pFT_GetStatus, typeof(FTDI.tFT_GetStatus));
                uint lpdwAmountInRxQueue = 0;
                uint lpdwAmountInTxQueue = 0;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref lpdwAmountInRxQueue, ref lpdwAmountInTxQueue, ref EventType);
            }
            else if (this.pFT_GetStatus == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetStatus.");
            return ftStatus;
        }

        /// <summary>Gets the current modem status.</summary>
        /// <returns>FT_STATUS value from FT_GetModemStatus in FTD2XX.DLL</returns>
        /// <param name="ModemStatus">A bit map representaion of the current modem status.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetModemStatus">`FTDI.GetModemStatus` on google.com</a></footer>
        public FTDI.FT_STATUS GetModemStatus(ref byte ModemStatus)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetModemStatus != IntPtr.Zero)
            {
                FTDI.tFT_GetModemStatus forFunctionPointer = (FTDI.tFT_GetModemStatus)Marshal.GetDelegateForFunctionPointer(this.pFT_GetModemStatus, typeof(FTDI.tFT_GetModemStatus));
                uint lpdwModemStatus = 0;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref lpdwModemStatus);
                ModemStatus = Convert.ToByte(lpdwModemStatus & (uint)byte.MaxValue);
            }
            else if (this.pFT_GetModemStatus == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetModemStatus.");
            return ftStatus;
        }

        /// <summary>Gets the current line status.</summary>
        /// <returns>FT_STATUS value from FT_GetModemStatus in FTD2XX.DLL</returns>
        /// <param name="LineStatus">A bit map representaion of the current line status.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetLineStatus">`FTDI.GetLineStatus` on google.com</a></footer>
        public FTDI.FT_STATUS GetLineStatus(ref byte LineStatus)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetModemStatus != IntPtr.Zero)
            {
                FTDI.tFT_GetModemStatus forFunctionPointer = (FTDI.tFT_GetModemStatus)Marshal.GetDelegateForFunctionPointer(this.pFT_GetModemStatus, typeof(FTDI.tFT_GetModemStatus));
                uint lpdwModemStatus = 0;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref lpdwModemStatus);
                LineStatus = Convert.ToByte(lpdwModemStatus >> 8 & (uint)byte.MaxValue);
            }
            else if (this.pFT_GetModemStatus == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetModemStatus.");
            return ftStatus;
        }

        /// <summary>Sets the current Baud rate.</summary>
        /// <returns>FT_STATUS value from FT_SetBaudRate in FTD2XX.DLL</returns>
        /// <param name="BaudRate">The desired Baud rate for the device.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetBaudRate">`FTDI.SetBaudRate` on google.com</a></footer>
        public FTDI.FT_STATUS SetBaudRate(uint BaudRate)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetBaudRate != IntPtr.Zero)
            {
                FTDI.tFT_SetBaudRate forFunctionPointer = (FTDI.tFT_SetBaudRate)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBaudRate, typeof(FTDI.tFT_SetBaudRate));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, BaudRate);
            }
            else if (this.pFT_SetBaudRate == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetBaudRate.");
            return ftStatus;
        }

        /// <summary>
        /// Sets the data bits, stop bits and parity for the device.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetDataCharacteristics in FTD2XX.DLL</returns>
        /// <param name="DataBits">The number of data bits for UART data.  Valid values are FT_DATA_BITS.FT_DATA_7 or FT_DATA_BITS.FT_BITS_8</param>
        /// <param name="StopBits">The number of stop bits for UART data.  Valid values are FT_STOP_BITS.FT_STOP_BITS_1 or FT_STOP_BITS.FT_STOP_BITS_2</param>
        /// <param name="Parity">The parity of the UART data.  Valid values are FT_PARITY.FT_PARITY_NONE, FT_PARITY.FT_PARITY_ODD, FT_PARITY.FT_PARITY_EVEN, FT_PARITY.FT_PARITY_MARK or FT_PARITY.FT_PARITY_SPACE</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetDataCharacteristics">`FTDI.SetDataCharacteristics` on google.com</a></footer>
        public FTDI.FT_STATUS SetDataCharacteristics(byte DataBits, byte StopBits, byte Parity)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetDataCharacteristics != IntPtr.Zero)
            {
                FTDI.tFT_SetDataCharacteristics forFunctionPointer = (FTDI.tFT_SetDataCharacteristics)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDataCharacteristics, typeof(FTDI.tFT_SetDataCharacteristics));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, DataBits, StopBits, Parity);
            }
            else if (this.pFT_SetDataCharacteristics == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetDataCharacteristics.");
            return ftStatus;
        }

        /// <summary>Sets the flow control type.</summary>
        /// <returns>FT_STATUS value from FT_SetFlowControl in FTD2XX.DLL</returns>
        /// <param name="FlowControl">The type of flow control for the UART.  Valid values are FT_FLOW_CONTROL.FT_FLOW_NONE, FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, FT_FLOW_CONTROL.FT_FLOW_DTR_DSR or FT_FLOW_CONTROL.FT_FLOW_XON_XOFF</param>
        /// <param name="Xon">The Xon character for Xon/Xoff flow control.  Ignored if not using Xon/XOff flow control.</param>
        /// <param name="Xoff">The Xoff character for Xon/Xoff flow control.  Ignored if not using Xon/XOff flow control.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetFlowControl">`FTDI.SetFlowControl` on google.com</a></footer>
        public FTDI.FT_STATUS SetFlowControl(ushort FlowControl, byte Xon, byte Xoff)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetFlowControl != IntPtr.Zero)
            {
                FTDI.tFT_SetFlowControl forFunctionPointer = (FTDI.tFT_SetFlowControl)Marshal.GetDelegateForFunctionPointer(this.pFT_SetFlowControl, typeof(FTDI.tFT_SetFlowControl));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, FlowControl, Xon, Xoff);
            }
            else if (this.pFT_SetFlowControl == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetFlowControl.");
            return ftStatus;
        }

        /// <summary>Asserts or de-asserts the Request To Send (RTS) line.</summary>
        /// <returns>FT_STATUS value from FT_SetRts or FT_ClrRts in FTD2XX.DLL</returns>
        /// <param name="Enable">If true, asserts RTS.  If false, de-asserts RTS</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetRTS">`FTDI.SetRTS` on google.com</a></footer>
        public FTDI.FT_STATUS SetRTS(bool Enable)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetRts != IntPtr.Zero & this.pFT_ClrRts != IntPtr.Zero)
            {
                FTDI.tFT_SetRts forFunctionPointer1 = (FTDI.tFT_SetRts)Marshal.GetDelegateForFunctionPointer(this.pFT_SetRts, typeof(FTDI.tFT_SetRts));
                FTDI.tFT_ClrRts forFunctionPointer2 = (FTDI.tFT_ClrRts)Marshal.GetDelegateForFunctionPointer(this.pFT_ClrRts, typeof(FTDI.tFT_ClrRts));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = !Enable ? forFunctionPointer2(this.ftHandle) : forFunctionPointer1(this.ftHandle);
            }
            else
            {
                if (this.pFT_SetRts == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetRts.");
                if (this.pFT_ClrRts == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_ClrRts.");
            }
            return ftStatus;
        }

        /// <summary>
        /// Asserts or de-asserts the Data Terminal Ready (DTR) line.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetDtr or FT_ClrDtr in FTD2XX.DLL</returns>
        /// <param name="Enable">If true, asserts DTR.  If false, de-asserts DTR.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetDTR">`FTDI.SetDTR` on google.com</a></footer>
        public FTDI.FT_STATUS SetDTR(bool Enable)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetDtr != IntPtr.Zero & this.pFT_ClrDtr != IntPtr.Zero)
            {
                FTDI.tFT_SetDtr forFunctionPointer1 = (FTDI.tFT_SetDtr)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDtr, typeof(FTDI.tFT_SetDtr));
                FTDI.tFT_ClrDtr forFunctionPointer2 = (FTDI.tFT_ClrDtr)Marshal.GetDelegateForFunctionPointer(this.pFT_ClrDtr, typeof(FTDI.tFT_ClrDtr));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = !Enable ? forFunctionPointer2(this.ftHandle) : forFunctionPointer1(this.ftHandle);
            }
            else
            {
                if (this.pFT_SetDtr == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetDtr.");
                if (this.pFT_ClrDtr == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_ClrDtr.");
            }
            return ftStatus;
        }

        /// <summary>Sets the read and write timeout values.</summary>
        /// <returns>FT_STATUS value from FT_SetTimeouts in FTD2XX.DLL</returns>
        /// <param name="ReadTimeout">Read timeout value in ms.  A value of 0 indicates an infinite timeout.</param>
        /// <param name="WriteTimeout">Write timeout value in ms.  A value of 0 indicates an infinite timeout.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetTimeouts">`FTDI.SetTimeouts` on google.com</a></footer>
        public FTDI.FT_STATUS SetTimeouts(uint ReadTimeout, uint WriteTimeout)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetTimeouts != IntPtr.Zero)
            {
                FTDI.tFT_SetTimeouts forFunctionPointer = (FTDI.tFT_SetTimeouts)Marshal.GetDelegateForFunctionPointer(this.pFT_SetTimeouts, typeof(FTDI.tFT_SetTimeouts));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ReadTimeout, WriteTimeout);
            }
            else if (this.pFT_SetTimeouts == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetTimeouts.");
            return ftStatus;
        }

        /// <summary>Sets or clears the break state.</summary>
        /// <returns>FT_STATUS value from FT_SetBreakOn or FT_SetBreakOff in FTD2XX.DLL</returns>
        /// <param name="Enable">If true, sets break on.  If false, sets break off.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetBreak">`FTDI.SetBreak` on google.com</a></footer>
        public FTDI.FT_STATUS SetBreak(bool Enable)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetBreakOn != IntPtr.Zero & this.pFT_SetBreakOff != IntPtr.Zero)
            {
                FTDI.tFT_SetBreakOn forFunctionPointer1 = (FTDI.tFT_SetBreakOn)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBreakOn, typeof(FTDI.tFT_SetBreakOn));
                FTDI.tFT_SetBreakOff forFunctionPointer2 = (FTDI.tFT_SetBreakOff)Marshal.GetDelegateForFunctionPointer(this.pFT_SetBreakOff, typeof(FTDI.tFT_SetBreakOff));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = !Enable ? forFunctionPointer2(this.ftHandle) : forFunctionPointer1(this.ftHandle);
            }
            else
            {
                if (this.pFT_SetBreakOn == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBreakOn.");
                if (this.pFT_SetBreakOff == IntPtr.Zero)
                    Console.WriteLine("Failed to load function FT_SetBreakOff.");
            }
            return ftStatus;
        }

        /// <summary>
        /// Gets or sets the reset pipe retry count.  Default value is 50.
        /// </summary>
        /// <returns>FT_STATUS vlaue from FT_SetResetPipeRetryCount in FTD2XX.DLL</returns>
        /// <param name="ResetPipeRetryCount">The reset pipe retry count.
        /// Electrically noisy environments may benefit from a larger value.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetResetPipeRetryCount">`FTDI.SetResetPipeRetryCount` on google.com</a></footer>
        public FTDI.FT_STATUS SetResetPipeRetryCount(uint ResetPipeRetryCount)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetResetPipeRetryCount != IntPtr.Zero)
            {
                FTDI.tFT_SetResetPipeRetryCount forFunctionPointer = (FTDI.tFT_SetResetPipeRetryCount)Marshal.GetDelegateForFunctionPointer(this.pFT_SetResetPipeRetryCount, typeof(FTDI.tFT_SetResetPipeRetryCount));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ResetPipeRetryCount);
            }
            else if (this.pFT_SetResetPipeRetryCount == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetResetPipeRetryCount.");
            return ftStatus;
        }

        /// <summary>Gets the current FTDIBUS.SYS driver version number.</summary>
        /// <returns>FT_STATUS value from FT_GetDriverVersion in FTD2XX.DLL</returns>
        /// <param name="DriverVersion">The current driver version number.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetDriverVersion">`FTDI.GetDriverVersion` on google.com</a></footer>
        public FTDI.FT_STATUS GetDriverVersion(ref uint DriverVersion)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetDriverVersion != IntPtr.Zero)
            {
                FTDI.tFT_GetDriverVersion forFunctionPointer = (FTDI.tFT_GetDriverVersion)Marshal.GetDelegateForFunctionPointer(this.pFT_GetDriverVersion, typeof(FTDI.tFT_GetDriverVersion));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref DriverVersion);
            }
            else if (this.pFT_GetDriverVersion == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetDriverVersion.");
            return ftStatus;
        }

        /// <summary>Gets the current FTD2XX.DLL driver version number.</summary>
        /// <returns>FT_STATUS value from FT_GetLibraryVersion in FTD2XX.DLL</returns>
        /// <param name="LibraryVersion">The current library version.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetLibraryVersion">`FTDI.GetLibraryVersion` on google.com</a></footer>
        public FTDI.FT_STATUS GetLibraryVersion(ref uint LibraryVersion)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetLibraryVersion != IntPtr.Zero)
                ftStatus = ((FTDI.tFT_GetLibraryVersion)Marshal.GetDelegateForFunctionPointer(this.pFT_GetLibraryVersion, typeof(FTDI.tFT_GetLibraryVersion)))(ref LibraryVersion);
            else if (this.pFT_GetLibraryVersion == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetLibraryVersion.");
            return ftStatus;
        }

        /// <summary>
        /// Sets the USB deadman timeout value.  Default is 5000ms.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetDeadmanTimeout in FTD2XX.DLL</returns>
        /// <param name="DeadmanTimeout">The deadman timeout value in ms.  Default is 5000ms.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetDeadmanTimeout">`FTDI.SetDeadmanTimeout` on google.com</a></footer>
        public FTDI.FT_STATUS SetDeadmanTimeout(uint DeadmanTimeout)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetDeadmanTimeout != IntPtr.Zero)
            {
                FTDI.tFT_SetDeadmanTimeout forFunctionPointer = (FTDI.tFT_SetDeadmanTimeout)Marshal.GetDelegateForFunctionPointer(this.pFT_SetDeadmanTimeout, typeof(FTDI.tFT_SetDeadmanTimeout));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, DeadmanTimeout);
            }
            else if (this.pFT_SetDeadmanTimeout == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetDeadmanTimeout.");
            return ftStatus;
        }

        /// <summary>
        /// Sets the value of the latency timer.  Default value is 16ms.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetLatencyTimer in FTD2XX.DLL</returns>
        /// <param name="Latency">The latency timer value in ms.
        /// Valid values are 2ms - 255ms for FT232BM, FT245BM and FT2232 devices.
        /// Valid values are 0ms - 255ms for other devices.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetLatency">`FTDI.SetLatency` on google.com</a></footer>
        public FTDI.FT_STATUS SetLatency(byte Latency)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetLatencyTimer != IntPtr.Zero)
            {
                FTDI.tFT_SetLatencyTimer forFunctionPointer = (FTDI.tFT_SetLatencyTimer)Marshal.GetDelegateForFunctionPointer(this.pFT_SetLatencyTimer, typeof(FTDI.tFT_SetLatencyTimer));
                if (this.ftHandle != IntPtr.Zero)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_UNKNOWN;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if ((DeviceType == FTDI.FT_DEVICE.FT_DEVICE_BM || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232) && Latency < (byte)2)
                        Latency = (byte)2;
                    ftStatus = forFunctionPointer(this.ftHandle, Latency);
                }
            }
            else if (this.pFT_SetLatencyTimer == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetLatencyTimer.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the value of the latency timer.  Default value is 16ms.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetLatencyTimer in FTD2XX.DLL</returns>
        /// <param name="Latency">The latency timer value in ms.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetLatency">`FTDI.GetLatency` on google.com</a></footer>
        public FTDI.FT_STATUS GetLatency(ref byte Latency)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetLatencyTimer != IntPtr.Zero)
            {
                FTDI.tFT_GetLatencyTimer forFunctionPointer = (FTDI.tFT_GetLatencyTimer)Marshal.GetDelegateForFunctionPointer(this.pFT_GetLatencyTimer, typeof(FTDI.tFT_GetLatencyTimer));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref Latency);
            }
            else if (this.pFT_GetLatencyTimer == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetLatencyTimer.");
            return ftStatus;
        }

        /// <summary>Sets the USB IN and OUT transfer sizes.</summary>
        /// <returns>FT_STATUS value from FT_SetUSBParameters in FTD2XX.DLL</returns>
        /// <param name="InTransferSize">The USB IN transfer size in bytes.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.InTransferSize">`FTDI.InTransferSize` on google.com</a></footer>
        public FTDI.FT_STATUS InTransferSize(uint InTransferSize)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetUSBParameters != IntPtr.Zero)
            {
                FTDI.tFT_SetUSBParameters forFunctionPointer = (FTDI.tFT_SetUSBParameters)Marshal.GetDelegateForFunctionPointer(this.pFT_SetUSBParameters, typeof(FTDI.tFT_SetUSBParameters));
                uint dwOutTransferSize = InTransferSize;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, InTransferSize, dwOutTransferSize);
            }
            else if (this.pFT_SetUSBParameters == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetUSBParameters.");
            return ftStatus;
        }

        /// <summary>
        /// Sets an event character, an error character and enables or disables them.
        /// </summary>
        /// <returns>FT_STATUS value from FT_SetChars in FTD2XX.DLL</returns>
        /// <param name="EventChar">A character that will be tigger an IN to the host when this character is received.</param>
        /// <param name="EventCharEnable">Determines if the EventChar is enabled or disabled.</param>
        /// <param name="ErrorChar">A character that will be inserted into the data stream to indicate that an error has occurred.</param>
        /// <param name="ErrorCharEnable">Determines if the ErrorChar is enabled or disabled.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.SetCharacters">`FTDI.SetCharacters` on google.com</a></footer>
        public FTDI.FT_STATUS SetCharacters(
          byte EventChar,
          bool EventCharEnable,
          byte ErrorChar,
          bool ErrorCharEnable)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_SetChars != IntPtr.Zero)
            {
                FTDI.tFT_SetChars forFunctionPointer = (FTDI.tFT_SetChars)Marshal.GetDelegateForFunctionPointer(this.pFT_SetChars, typeof(FTDI.tFT_SetChars));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, EventChar, Convert.ToByte(EventCharEnable), ErrorChar, Convert.ToByte(ErrorCharEnable));
            }
            else if (this.pFT_SetChars == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_SetChars.");
            return ftStatus;
        }

        /// <summary>Gets the size of the EEPROM user area.</summary>
        /// <returns>FT_STATUS value from FT_EE_UASize in FTD2XX.DLL</returns>
        /// <param name="UASize">The EEPROM user area size in bytes.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.EEUserAreaSize">`FTDI.EEUserAreaSize` on google.com</a></footer>
        public FTDI.FT_STATUS EEUserAreaSize(ref uint UASize)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_EE_UASize != IntPtr.Zero)
            {
                FTDI.tFT_EE_UASize forFunctionPointer = (FTDI.tFT_EE_UASize)Marshal.GetDelegateForFunctionPointer(this.pFT_EE_UASize, typeof(FTDI.tFT_EE_UASize));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref UASize);
            }
            else if (this.pFT_EE_UASize == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_EE_UASize.");
            return ftStatus;
        }

        /// <summary>
        /// Gets the corresponding COM port number for the current device.  If no COM port is exposed, an empty string is returned.
        /// </summary>
        /// <returns>FT_STATUS value from FT_GetComPortNumber in FTD2XX.DLL</returns>
        /// <param name="ComPortName">The COM port name corresponding to the current device.  If no COM port is installed, an empty string is passed back.</param>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.GetCOMPort">`FTDI.GetCOMPort` on google.com</a></footer>
        public FTDI.FT_STATUS GetCOMPort(out string ComPortName)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            ComPortName = string.Empty;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_GetComPortNumber != IntPtr.Zero)
            {
                FTDI.tFT_GetComPortNumber forFunctionPointer = (FTDI.tFT_GetComPortNumber)Marshal.GetDelegateForFunctionPointer(this.pFT_GetComPortNumber, typeof(FTDI.tFT_GetComPortNumber));
                int dwComPortNumber = -1;
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, ref dwComPortNumber);
                ComPortName = dwComPortNumber != -1 ? "COM" + dwComPortNumber.ToString() : string.Empty;
            }
            else if (this.pFT_GetComPortNumber == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_GetComPortNumber.");
            return ftStatus;
        }

        /// <summary>
        /// Get data from the FT4222 using the vendor command interface.
        /// </summary>
        /// <returns>FT_STATUS value from FT_VendorCmdSet in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.VendorCmdGet">`FTDI.VendorCmdGet` on google.com</a></footer>
        public FTDI.FT_STATUS VendorCmdGet(ushort request, byte[] buf, ushort len)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_VendorCmdGet != IntPtr.Zero)
            {
                FTDI.tFT_VendorCmdGet forFunctionPointer = (FTDI.tFT_VendorCmdGet)Marshal.GetDelegateForFunctionPointer(this.pFT_VendorCmdGet, typeof(FTDI.tFT_VendorCmdGet));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, request, buf, len);
            }
            else if (this.pFT_VendorCmdGet == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_VendorCmdGet.");
            return ftStatus;
        }

        /// <summary>
        /// Set data from the FT4222 using the vendor command interface.
        /// </summary>
        /// <returns>FT_STATUS value from FT_VendorCmdSet in FTD2XX.DLL</returns>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.VendorCmdSet">`FTDI.VendorCmdSet` on google.com</a></footer>
        public FTDI.FT_STATUS VendorCmdSet(ushort request, byte[] buf, ushort len)
        {
            FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OTHER_ERROR;
            if (this.hFTD2XXDLL == IntPtr.Zero)
                return ftStatus;
            if (this.pFT_VendorCmdSet != IntPtr.Zero)
            {
                FTDI.tFT_VendorCmdSet forFunctionPointer = (FTDI.tFT_VendorCmdSet)Marshal.GetDelegateForFunctionPointer(this.pFT_VendorCmdSet, typeof(FTDI.tFT_VendorCmdSet));
                if (this.ftHandle != IntPtr.Zero)
                    ftStatus = forFunctionPointer(this.ftHandle, request, buf, len);
            }
            else if (this.pFT_VendorCmdSet == IntPtr.Zero)
                Console.WriteLine("Failed to load function FT_VendorCmdSet.");
            return ftStatus;
        }

        /// <summary>Gets the open status of the device.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.IsOpen">`FTDI.IsOpen` on google.com</a></footer>
        public bool IsOpen => !(this.ftHandle == IntPtr.Zero);

        /// <summary>Gets the interface identifier.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.InterfaceIdentifier">`FTDI.InterfaceIdentifier` on google.com</a></footer>
        private string InterfaceIdentifier
        {
            get
            {
                string empty = string.Empty;
                if (this.IsOpen)
                {
                    FTDI.FT_DEVICE DeviceType = FTDI.FT_DEVICE.FT_DEVICE_BM;
                    int deviceType = (int)this.GetDeviceType(ref DeviceType);
                    if (DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232H || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232H || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2233HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4233HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232HP || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232HA || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_4232HA || DeviceType == FTDI.FT_DEVICE.FT_DEVICE_2232)
                    {
                        string Description;
                        int description = (int)this.GetDescription(out Description);
                        return Description.Substring(Description.Length - 1);
                    }
                }
                return empty;
            }
        }

        /// <summary>
        /// Method to check ftStatus and ftErrorCondition values for error conditions and throw exceptions accordingly.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.ErrorHandler">`FTDI.ErrorHandler` on google.com</a></footer>
        private void ErrorHandler(FTDI.FT_STATUS ftStatus, FTDI.FT_ERROR ftErrorCondition)
        {
            switch (ftStatus)
            {
                case FTDI.FT_STATUS.FT_INVALID_HANDLE:
                    throw new FTDI.FT_EXCEPTION("Invalid handle for FTDI device.");
                case FTDI.FT_STATUS.FT_DEVICE_NOT_FOUND:
                    throw new FTDI.FT_EXCEPTION("FTDI device not found.");
                case FTDI.FT_STATUS.FT_DEVICE_NOT_OPENED:
                    throw new FTDI.FT_EXCEPTION("FTDI device not opened.");
                case FTDI.FT_STATUS.FT_IO_ERROR:
                    throw new FTDI.FT_EXCEPTION("FTDI device IO error.");
                case FTDI.FT_STATUS.FT_INSUFFICIENT_RESOURCES:
                    throw new FTDI.FT_EXCEPTION("Insufficient resources.");
                case FTDI.FT_STATUS.FT_INVALID_PARAMETER:
                    throw new FTDI.FT_EXCEPTION("Invalid parameter for FTD2XX function call.");
                case FTDI.FT_STATUS.FT_INVALID_BAUD_RATE:
                    throw new FTDI.FT_EXCEPTION("Invalid Baud rate for FTDI device.");
                case FTDI.FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_ERASE:
                    throw new FTDI.FT_EXCEPTION("FTDI device not opened for erase.");
                case FTDI.FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_WRITE:
                    throw new FTDI.FT_EXCEPTION("FTDI device not opened for write.");
                case FTDI.FT_STATUS.FT_FAILED_TO_WRITE_DEVICE:
                    throw new FTDI.FT_EXCEPTION("Failed to write to FTDI device.");
                case FTDI.FT_STATUS.FT_EEPROM_READ_FAILED:
                    throw new FTDI.FT_EXCEPTION("Failed to read FTDI device EEPROM.");
                case FTDI.FT_STATUS.FT_EEPROM_WRITE_FAILED:
                    throw new FTDI.FT_EXCEPTION("Failed to write FTDI device EEPROM.");
                case FTDI.FT_STATUS.FT_EEPROM_ERASE_FAILED:
                    throw new FTDI.FT_EXCEPTION("Failed to erase FTDI device EEPROM.");
                case FTDI.FT_STATUS.FT_EEPROM_NOT_PRESENT:
                    throw new FTDI.FT_EXCEPTION("No EEPROM fitted to FTDI device.");
                case FTDI.FT_STATUS.FT_EEPROM_NOT_PROGRAMMED:
                    throw new FTDI.FT_EXCEPTION("FTDI device EEPROM not programmed.");
                case FTDI.FT_STATUS.FT_INVALID_ARGS:
                    throw new FTDI.FT_EXCEPTION("Invalid arguments for FTD2XX function call.");
                case FTDI.FT_STATUS.FT_OTHER_ERROR:
                    throw new FTDI.FT_EXCEPTION("An unexpected error has occurred when trying to communicate with the FTDI device.");
                default:
                    switch (ftErrorCondition)
                    {
                        case FTDI.FT_ERROR.FT_NO_ERROR:
                            return;

                        case FTDI.FT_ERROR.FT_INCORRECT_DEVICE:
                            throw new FTDI.FT_EXCEPTION("The current device type does not match the EEPROM structure.");
                        case FTDI.FT_ERROR.FT_INVALID_BITMODE:
                            throw new FTDI.FT_EXCEPTION("The requested bit mode is not valid for the current device.");
                        case FTDI.FT_ERROR.FT_BUFFER_SIZE:
                            throw new FTDI.FT_EXCEPTION("The supplied buffer is not big enough.");
                        default:
                            return;
                    }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_CreateDeviceInfoList(ref uint numdevs);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetDeviceInfoDetail(
          uint index,
          ref uint flags,
          ref FTDI.FT_DEVICE chiptype,
          ref uint id,
          ref uint locid,
          byte[] serialnumber,
          byte[] description,
          ref IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Open(uint index, ref IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_OpenEx(
          string devstring,
          uint dwFlags,
          ref IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_OpenExLoc(
          uint devloc,
          uint dwFlags,
          ref IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Close(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Read(
          IntPtr ftHandle,
          byte[] lpBuffer,
          uint dwBytesToRead,
          ref uint lpdwBytesReturned);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Write(
          IntPtr ftHandle,
          byte[] lpBuffer,
          uint dwBytesToWrite,
          ref uint lpdwBytesWritten);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetQueueStatus(
          IntPtr ftHandle,
          ref uint lpdwAmountInRxQueue);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetModemStatus(
          IntPtr ftHandle,
          ref uint lpdwModemStatus);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetStatus(
          IntPtr ftHandle,
          ref uint lpdwAmountInRxQueue,
          ref uint lpdwAmountInTxQueue,
          ref uint lpdwEventStatus);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetBaudRate(IntPtr ftHandle, uint dwBaudRate);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetDataCharacteristics(
          IntPtr ftHandle,
          byte uWordLength,
          byte uStopBits,
          byte uParity);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetFlowControl(
          IntPtr ftHandle,
          ushort usFlowControl,
          byte uXon,
          byte uXoff);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetDtr(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_ClrDtr(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetRts(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_ClrRts(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_ResetDevice(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_ResetPort(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_CyclePort(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Rescan();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Reload(ushort wVID, ushort wPID);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_Purge(IntPtr ftHandle, uint dwMask);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetDivisor(IntPtr ftHandle, ushort divisor);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetTimeouts(
          IntPtr ftHandle,
          uint dwReadTimeout,
          uint dwWriteTimeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetBreakOn(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetBreakOff(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetDeviceInfo(
          IntPtr ftHandle,
          ref FTDI.FT_DEVICE pftType,
          ref uint lpdwID,
          byte[] pcSerialNumber,
          byte[] pcDescription,
          IntPtr pvDummy);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetResetPipeRetryCount(IntPtr ftHandle, uint dwCount);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_StopInTask(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_RestartInTask(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetDriverVersion(
          IntPtr ftHandle,
          ref uint lpdwDriverVersion);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetLibraryVersion(ref uint lpdwLibraryVersion);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetDeadmanTimeout(
          IntPtr ftHandle,
          uint dwDeadmanTimeout);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetChars(
          IntPtr ftHandle,
          byte uEventCh,
          byte uEventChEn,
          byte uErrorCh,
          byte uErrorChEn);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetEventNotification(
          IntPtr ftHandle,
          uint dwEventMask,
          SafeHandle hEvent);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetComPortNumber(
          IntPtr ftHandle,
          ref int dwComPortNumber);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetLatencyTimer(IntPtr ftHandle, byte ucLatency);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetLatencyTimer(IntPtr ftHandle, ref byte ucLatency);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetBitMode(IntPtr ftHandle, byte ucMask, byte ucMode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_GetBitMode(IntPtr ftHandle, ref byte ucMode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_SetUSBParameters(
          IntPtr ftHandle,
          uint dwInTransferSize,
          uint dwOutTransferSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_ReadEE(
          IntPtr ftHandle,
          uint dwWordOffset,
          ref ushort lpwValue);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_WriteEE(
          IntPtr ftHandle,
          uint dwWordOffset,
          ushort wValue);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EraseEE(IntPtr ftHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EE_UASize(IntPtr ftHandle, ref uint dwSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EE_UARead(
          IntPtr ftHandle,
          byte[] pucData,
          int dwDataLen,
          ref uint lpdwDataRead);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EE_UAWrite(
          IntPtr ftHandle,
          byte[] pucData,
          int dwDataLen);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EE_Read(IntPtr ftHandle, FTDI.FT_PROGRAM_DATA pData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EE_Program(IntPtr ftHandle, FTDI.FT_PROGRAM_DATA pData);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EEPROM_Read(
          IntPtr ftHandle,
          IntPtr eepromData,
          uint eepromDataSize,
          byte[] manufacturer,
          byte[] manufacturerID,
          byte[] description,
          byte[] serialnumber);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_EEPROM_Program(
          IntPtr ftHandle,
          IntPtr eepromData,
          uint eepromDataSize,
          byte[] manufacturer,
          byte[] manufacturerID,
          byte[] description,
          byte[] serialnumber);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_VendorCmdGet(
          IntPtr ftHandle,
          ushort request,
          byte[] buf,
          ushort len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_VendorCmdSet(
          IntPtr ftHandle,
          ushort request,
          byte[] buf,
          ushort len);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FTDI.FT_STATUS tFT_VendorCmdSetX(
          IntPtr ftHandle,
          ushort request,
          byte[] buf,
          ushort len);

        /// <summary>Status values for FTDI devices.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_STATUS">`FTDI.FT_STATUS` on google.com</a></footer>
        public enum FT_STATUS
        {
            /// <summary>Status OK</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_OK">`FT_STATUS.FT_OK` on google.com</a></footer>
            FT_OK,

            /// <summary>The device handle is invalid</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_INVALID_HANDLE">`FT_STATUS.FT_INVALID_HANDLE` on google.com</a></footer>
            FT_INVALID_HANDLE,

            /// <summary>Device not found</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_DEVICE_NOT_FOUND">`FT_STATUS.FT_DEVICE_NOT_FOUND` on google.com</a></footer>
            FT_DEVICE_NOT_FOUND,

            /// <summary>Device is not open</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_DEVICE_NOT_OPENED">`FT_STATUS.FT_DEVICE_NOT_OPENED` on google.com</a></footer>
            FT_DEVICE_NOT_OPENED,

            /// <summary>IO error</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_IO_ERROR">`FT_STATUS.FT_IO_ERROR` on google.com</a></footer>
            FT_IO_ERROR,

            /// <summary>Insufficient resources</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_INSUFFICIENT_RESOURCES">`FT_STATUS.FT_INSUFFICIENT_RESOURCES` on google.com</a></footer>
            FT_INSUFFICIENT_RESOURCES,

            /// <summary>A parameter was invalid</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_INVALID_PARAMETER">`FT_STATUS.FT_INVALID_PARAMETER` on google.com</a></footer>
            FT_INVALID_PARAMETER,

            /// <summary>The requested baud rate is invalid</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_INVALID_BAUD_RATE">`FT_STATUS.FT_INVALID_BAUD_RATE` on google.com</a></footer>
            FT_INVALID_BAUD_RATE,

            /// <summary>Device not opened for erase</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_DEVICE_NOT_OPENED_FOR_ERASE">`FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_ERASE` on google.com</a></footer>
            FT_DEVICE_NOT_OPENED_FOR_ERASE,

            /// <summary>Device not poened for write</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_DEVICE_NOT_OPENED_FOR_WRITE">`FT_STATUS.FT_DEVICE_NOT_OPENED_FOR_WRITE` on google.com</a></footer>
            FT_DEVICE_NOT_OPENED_FOR_WRITE,

            /// <summary>Failed to write to device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_FAILED_TO_WRITE_DEVICE">`FT_STATUS.FT_FAILED_TO_WRITE_DEVICE` on google.com</a></footer>
            FT_FAILED_TO_WRITE_DEVICE,

            /// <summary>Failed to read the device EEPROM</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_EEPROM_READ_FAILED">`FT_STATUS.FT_EEPROM_READ_FAILED` on google.com</a></footer>
            FT_EEPROM_READ_FAILED,

            /// <summary>Failed to write the device EEPROM</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_EEPROM_WRITE_FAILED">`FT_STATUS.FT_EEPROM_WRITE_FAILED` on google.com</a></footer>
            FT_EEPROM_WRITE_FAILED,

            /// <summary>Failed to erase the device EEPROM</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_EEPROM_ERASE_FAILED">`FT_STATUS.FT_EEPROM_ERASE_FAILED` on google.com</a></footer>
            FT_EEPROM_ERASE_FAILED,

            /// <summary>An EEPROM is not fitted to the device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_EEPROM_NOT_PRESENT">`FT_STATUS.FT_EEPROM_NOT_PRESENT` on google.com</a></footer>
            FT_EEPROM_NOT_PRESENT,

            /// <summary>Device EEPROM is blank</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_EEPROM_NOT_PROGRAMMED">`FT_STATUS.FT_EEPROM_NOT_PROGRAMMED` on google.com</a></footer>
            FT_EEPROM_NOT_PROGRAMMED,

            /// <summary>Invalid arguments</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_INVALID_ARGS">`FT_STATUS.FT_INVALID_ARGS` on google.com</a></footer>
            FT_INVALID_ARGS,

            /// <summary>An other error has occurred</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STATUS.FT_OTHER_ERROR">`FT_STATUS.FT_OTHER_ERROR` on google.com</a></footer>
            FT_OTHER_ERROR,
        }

        /// <summary>Error states not supported by FTD2XX DLL.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_ERROR">`FTDI.FT_ERROR` on google.com</a></footer>
        private enum FT_ERROR
        {
            FT_NO_ERROR,
            FT_INCORRECT_DEVICE,
            FT_INVALID_BITMODE,
            FT_BUFFER_SIZE,
        }

        /// <summary>Permitted data bits for FTDI devices</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_DATA_BITS">`FTDI.FT_DATA_BITS` on google.com</a></footer>
        public class FT_DATA_BITS
        {
            /// <summary>8 data bits</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DATA_BITS.FT_BITS_8">`FT_DATA_BITS.FT_BITS_8` on google.com</a></footer>
            public const byte FT_BITS_8 = 8;

            /// <summary>7 data bits</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DATA_BITS.FT_BITS_7">`FT_DATA_BITS.FT_BITS_7` on google.com</a></footer>
            public const byte FT_BITS_7 = 7;
        }

        /// <summary>Permitted stop bits for FTDI devices</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_STOP_BITS">`FTDI.FT_STOP_BITS` on google.com</a></footer>
        public class FT_STOP_BITS
        {
            /// <summary>1 stop bit</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STOP_BITS.FT_STOP_BITS_1">`FT_STOP_BITS.FT_STOP_BITS_1` on google.com</a></footer>
            public const byte FT_STOP_BITS_1 = 0;

            /// <summary>2 stop bits</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_STOP_BITS.FT_STOP_BITS_2">`FT_STOP_BITS.FT_STOP_BITS_2` on google.com</a></footer>
            public const byte FT_STOP_BITS_2 = 2;
        }

        /// <summary>Permitted parity values for FTDI devices</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_PARITY">`FTDI.FT_PARITY` on google.com</a></footer>
        public class FT_PARITY
        {
            /// <summary>No parity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PARITY.FT_PARITY_NONE">`FT_PARITY.FT_PARITY_NONE` on google.com</a></footer>
            public const byte FT_PARITY_NONE = 0;

            /// <summary>Odd parity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PARITY.FT_PARITY_ODD">`FT_PARITY.FT_PARITY_ODD` on google.com</a></footer>
            public const byte FT_PARITY_ODD = 1;

            /// <summary>Even parity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PARITY.FT_PARITY_EVEN">`FT_PARITY.FT_PARITY_EVEN` on google.com</a></footer>
            public const byte FT_PARITY_EVEN = 2;

            /// <summary>Mark parity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PARITY.FT_PARITY_MARK">`FT_PARITY.FT_PARITY_MARK` on google.com</a></footer>
            public const byte FT_PARITY_MARK = 3;

            /// <summary>Space parity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PARITY.FT_PARITY_SPACE">`FT_PARITY.FT_PARITY_SPACE` on google.com</a></footer>
            public const byte FT_PARITY_SPACE = 4;
        }

        /// <summary>Permitted flow control values for FTDI devices</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_FLOW_CONTROL">`FTDI.FT_FLOW_CONTROL` on google.com</a></footer>
        public class FT_FLOW_CONTROL
        {
            /// <summary>No flow control</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLOW_CONTROL.FT_FLOW_NONE">`FT_FLOW_CONTROL.FT_FLOW_NONE` on google.com</a></footer>
            public const ushort FT_FLOW_NONE = 0;

            /// <summary>RTS/CTS flow control</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLOW_CONTROL.FT_FLOW_RTS_CTS">`FT_FLOW_CONTROL.FT_FLOW_RTS_CTS` on google.com</a></footer>
            public const ushort FT_FLOW_RTS_CTS = 256;

            /// <summary>DTR/DSR flow control</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLOW_CONTROL.FT_FLOW_DTR_DSR">`FT_FLOW_CONTROL.FT_FLOW_DTR_DSR` on google.com</a></footer>
            public const ushort FT_FLOW_DTR_DSR = 512;

            /// <summary>Xon/Xoff flow control</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLOW_CONTROL.FT_FLOW_XON_XOFF">`FT_FLOW_CONTROL.FT_FLOW_XON_XOFF` on google.com</a></footer>
            public const ushort FT_FLOW_XON_XOFF = 1024;
        }

        /// <summary>Purge buffer constant definitions</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_PURGE">`FTDI.FT_PURGE` on google.com</a></footer>
        public class FT_PURGE
        {
            /// <summary>Purge Rx buffer</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PURGE.FT_PURGE_RX">`FT_PURGE.FT_PURGE_RX` on google.com</a></footer>
            public const byte FT_PURGE_RX = 1;

            /// <summary>Purge Tx buffer</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_PURGE.FT_PURGE_TX">`FT_PURGE.FT_PURGE_TX` on google.com</a></footer>
            public const byte FT_PURGE_TX = 2;
        }

        /// <summary>Modem status bit definitions</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_MODEM_STATUS">`FTDI.FT_MODEM_STATUS` on google.com</a></footer>
        public class FT_MODEM_STATUS
        {
            /// <summary>Clear To Send (CTS) modem status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_MODEM_STATUS.FT_CTS">`FT_MODEM_STATUS.FT_CTS` on google.com</a></footer>
            public const byte FT_CTS = 16;

            /// <summary>Data Set Ready (DSR) modem status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_MODEM_STATUS.FT_DSR">`FT_MODEM_STATUS.FT_DSR` on google.com</a></footer>
            public const byte FT_DSR = 32;

            /// <summary>Ring Indicator (RI) modem status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_MODEM_STATUS.FT_RI">`FT_MODEM_STATUS.FT_RI` on google.com</a></footer>
            public const byte FT_RI = 64;

            /// <summary>Data Carrier Detect (DCD) modem status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_MODEM_STATUS.FT_DCD">`FT_MODEM_STATUS.FT_DCD` on google.com</a></footer>
            public const byte FT_DCD = 128;
        }

        /// <summary>Line status bit definitions</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_LINE_STATUS">`FTDI.FT_LINE_STATUS` on google.com</a></footer>
        public class FT_LINE_STATUS
        {
            /// <summary>Overrun Error (OE) line status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_LINE_STATUS.FT_OE">`FT_LINE_STATUS.FT_OE` on google.com</a></footer>
            public const byte FT_OE = 2;

            /// <summary>Parity Error (PE) line status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_LINE_STATUS.FT_PE">`FT_LINE_STATUS.FT_PE` on google.com</a></footer>
            public const byte FT_PE = 4;

            /// <summary>Framing Error (FE) line status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_LINE_STATUS.FT_FE">`FT_LINE_STATUS.FT_FE` on google.com</a></footer>
            public const byte FT_FE = 8;

            /// <summary>Break Interrupt (BI) line status</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_LINE_STATUS.FT_BI">`FT_LINE_STATUS.FT_BI` on google.com</a></footer>
            public const byte FT_BI = 16;
        }

        /// <summary>FTDI device event types that can be monitored</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_EVENTS">`FTDI.FT_EVENTS` on google.com</a></footer>
        public class FT_EVENTS
        {
            /// <summary>Event on receive character</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EVENTS.FT_EVENT_RXCHAR">`FT_EVENTS.FT_EVENT_RXCHAR` on google.com</a></footer>
            public const uint FT_EVENT_RXCHAR = 1;

            /// <summary>Event on modem status change</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EVENTS.FT_EVENT_MODEM_STATUS">`FT_EVENTS.FT_EVENT_MODEM_STATUS` on google.com</a></footer>
            public const uint FT_EVENT_MODEM_STATUS = 2;

            /// <summary>Event on line status change</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EVENTS.FT_EVENT_LINE_STATUS">`FT_EVENTS.FT_EVENT_LINE_STATUS` on google.com</a></footer>
            public const uint FT_EVENT_LINE_STATUS = 4;
        }

        /// <summary>
        /// Permitted bit mode values for FTDI devices.  For use with SetBitMode
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_BIT_MODES">`FTDI.FT_BIT_MODES` on google.com</a></footer>
        public class FT_BIT_MODES
        {
            /// <summary>Reset bit mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_RESET">`FT_BIT_MODES.FT_BIT_MODE_RESET` on google.com</a></footer>
            public const byte FT_BIT_MODE_RESET = 0;

            /// <summary>Asynchronous bit-bang mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_ASYNC_BITBANG">`FT_BIT_MODES.FT_BIT_MODE_ASYNC_BITBANG` on google.com</a></footer>
            public const byte FT_BIT_MODE_ASYNC_BITBANG = 1;

            /// <summary>
            /// MPSSE bit mode - only available on FT2232, FT2232H, FT4232H and FT232H
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_MPSSE">`FT_BIT_MODES.FT_BIT_MODE_MPSSE` on google.com</a></footer>
            public const byte FT_BIT_MODE_MPSSE = 2;

            /// <summary>Synchronous bit-bang mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG">`FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG` on google.com</a></footer>
            public const byte FT_BIT_MODE_SYNC_BITBANG = 4;

            /// <summary>
            /// MCU host bus emulation mode - only available on FT2232, FT2232H, FT4232H and FT232H
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_MCU_HOST">`FT_BIT_MODES.FT_BIT_MODE_MCU_HOST` on google.com</a></footer>
            public const byte FT_BIT_MODE_MCU_HOST = 8;

            /// <summary>
            /// Fast opto-isolated serial mode - only available on FT2232, FT2232H, FT4232H and FT232H
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_FAST_SERIAL">`FT_BIT_MODES.FT_BIT_MODE_FAST_SERIAL` on google.com</a></footer>
            public const byte FT_BIT_MODE_FAST_SERIAL = 16;

            /// <summary>
            /// CBUS bit-bang mode - only available on FT232R and FT232H
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG">`FT_BIT_MODES.FT_BIT_MODE_CBUS_BITBANG` on google.com</a></footer>
            public const byte FT_BIT_MODE_CBUS_BITBANG = 32;

            /// <summary>
            /// Single channel synchronous 245 FIFO mode - only available on FT2232H channel A and FT232H
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_BIT_MODES.FT_BIT_MODE_SYNC_FIFO">`FT_BIT_MODES.FT_BIT_MODE_SYNC_FIFO` on google.com</a></footer>
            public const byte FT_BIT_MODE_SYNC_FIFO = 64;
        }

        /// <summary>
        /// Available functions for the FT232R CBUS pins.  Controlled by FT232R EEPROM settings
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_CBUS_OPTIONS">`FTDI.FT_CBUS_OPTIONS` on google.com</a></footer>
        public class FT_CBUS_OPTIONS
        {
            /// <summary>FT232R CBUS EEPROM options - Tx Data Enable</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_TXDEN">`FT_CBUS_OPTIONS.FT_CBUS_TXDEN` on google.com</a></footer>
            public const byte FT_CBUS_TXDEN = 0;

            /// <summary>FT232R CBUS EEPROM options - Power On</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_PWRON">`FT_CBUS_OPTIONS.FT_CBUS_PWRON` on google.com</a></footer>
            public const byte FT_CBUS_PWRON = 1;

            /// <summary>FT232R CBUS EEPROM options - Rx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_RXLED">`FT_CBUS_OPTIONS.FT_CBUS_RXLED` on google.com</a></footer>
            public const byte FT_CBUS_RXLED = 2;

            /// <summary>FT232R CBUS EEPROM options - Tx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_TXLED">`FT_CBUS_OPTIONS.FT_CBUS_TXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXLED = 3;

            /// <summary>FT232R CBUS EEPROM options - Tx and Rx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_TXRXLED">`FT_CBUS_OPTIONS.FT_CBUS_TXRXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXRXLED = 4;

            /// <summary>FT232R CBUS EEPROM options - Sleep</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_SLEEP">`FT_CBUS_OPTIONS.FT_CBUS_SLEEP` on google.com</a></footer>
            public const byte FT_CBUS_SLEEP = 5;

            /// <summary>FT232R CBUS EEPROM options - 48MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_CLK48">`FT_CBUS_OPTIONS.FT_CBUS_CLK48` on google.com</a></footer>
            public const byte FT_CBUS_CLK48 = 6;

            /// <summary>FT232R CBUS EEPROM options - 24MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_CLK24">`FT_CBUS_OPTIONS.FT_CBUS_CLK24` on google.com</a></footer>
            public const byte FT_CBUS_CLK24 = 7;

            /// <summary>FT232R CBUS EEPROM options - 12MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_CLK12">`FT_CBUS_OPTIONS.FT_CBUS_CLK12` on google.com</a></footer>
            public const byte FT_CBUS_CLK12 = 8;

            /// <summary>FT232R CBUS EEPROM options - 6MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_CLK6">`FT_CBUS_OPTIONS.FT_CBUS_CLK6` on google.com</a></footer>
            public const byte FT_CBUS_CLK6 = 9;

            /// <summary>FT232R CBUS EEPROM options - IO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_IOMODE">`FT_CBUS_OPTIONS.FT_CBUS_IOMODE` on google.com</a></footer>
            public const byte FT_CBUS_IOMODE = 10;

            /// <summary>FT232R CBUS EEPROM options - Bit-bang write strobe</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_BITBANG_WR">`FT_CBUS_OPTIONS.FT_CBUS_BITBANG_WR` on google.com</a></footer>
            public const byte FT_CBUS_BITBANG_WR = 11;

            /// <summary>FT232R CBUS EEPROM options - Bit-bang read strobe</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_CBUS_OPTIONS.FT_CBUS_BITBANG_RD">`FT_CBUS_OPTIONS.FT_CBUS_BITBANG_RD` on google.com</a></footer>
            public const byte FT_CBUS_BITBANG_RD = 12;
        }

        /// <summary>
        /// Available functions for the FT232H CBUS pins.  Controlled by FT232H EEPROM settings
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_232H_CBUS_OPTIONS">`FTDI.FT_232H_CBUS_OPTIONS` on google.com</a></footer>
        public class FT_232H_CBUS_OPTIONS
        {
            /// <summary>FT232H CBUS EEPROM options - Tristate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_TRISTATE">`FT_232H_CBUS_OPTIONS.FT_CBUS_TRISTATE` on google.com</a></footer>
            public const byte FT_CBUS_TRISTATE = 0;

            /// <summary>FT232H CBUS EEPROM options - Rx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_RXLED">`FT_232H_CBUS_OPTIONS.FT_CBUS_RXLED` on google.com</a></footer>
            public const byte FT_CBUS_RXLED = 1;

            /// <summary>FT232H CBUS EEPROM options - Tx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_TXLED">`FT_232H_CBUS_OPTIONS.FT_CBUS_TXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXLED = 2;

            /// <summary>FT232H CBUS EEPROM options - Tx and Rx LED</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_TXRXLED">`FT_232H_CBUS_OPTIONS.FT_CBUS_TXRXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXRXLED = 3;

            /// <summary>FT232H CBUS EEPROM options - Power Enable#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_PWREN">`FT_232H_CBUS_OPTIONS.FT_CBUS_PWREN` on google.com</a></footer>
            public const byte FT_CBUS_PWREN = 4;

            /// <summary>FT232H CBUS EEPROM options - Sleep</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_SLEEP">`FT_232H_CBUS_OPTIONS.FT_CBUS_SLEEP` on google.com</a></footer>
            public const byte FT_CBUS_SLEEP = 5;

            /// <summary>FT232H CBUS EEPROM options - Drive pin to logic 0</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_DRIVE_0">`FT_232H_CBUS_OPTIONS.FT_CBUS_DRIVE_0` on google.com</a></footer>
            public const byte FT_CBUS_DRIVE_0 = 6;

            /// <summary>FT232H CBUS EEPROM options - Drive pin to logic 1</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_DRIVE_1">`FT_232H_CBUS_OPTIONS.FT_CBUS_DRIVE_1` on google.com</a></footer>
            public const byte FT_CBUS_DRIVE_1 = 7;

            /// <summary>FT232H CBUS EEPROM options - IO Mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_IOMODE">`FT_232H_CBUS_OPTIONS.FT_CBUS_IOMODE` on google.com</a></footer>
            public const byte FT_CBUS_IOMODE = 8;

            /// <summary>FT232H CBUS EEPROM options - Tx Data Enable</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_TXDEN">`FT_232H_CBUS_OPTIONS.FT_CBUS_TXDEN` on google.com</a></footer>
            public const byte FT_CBUS_TXDEN = 9;

            /// <summary>FT232H CBUS EEPROM options - 30MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_CLK30">`FT_232H_CBUS_OPTIONS.FT_CBUS_CLK30` on google.com</a></footer>
            public const byte FT_CBUS_CLK30 = 10;

            /// <summary>FT232H CBUS EEPROM options - 15MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_CLK15">`FT_232H_CBUS_OPTIONS.FT_CBUS_CLK15` on google.com</a></footer>
            public const byte FT_CBUS_CLK15 = 11;

            /// <summary>FT232H CBUS EEPROM options - 7.5MHz clock</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_232H_CBUS_OPTIONS.FT_CBUS_CLK7_5">`FT_232H_CBUS_OPTIONS.FT_CBUS_CLK7_5` on google.com</a></footer>
            public const byte FT_CBUS_CLK7_5 = 12;
        }

        /// <summary>
        /// Available functions for the X-Series CBUS pins.  Controlled by X-Series EEPROM settings
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_XSERIES_CBUS_OPTIONS">`FTDI.FT_XSERIES_CBUS_OPTIONS` on google.com</a></footer>
        public class FT_XSERIES_CBUS_OPTIONS
        {
            /// <summary>FT X-Series CBUS EEPROM options - Tristate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_TRISTATE">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_TRISTATE` on google.com</a></footer>
            public const byte FT_CBUS_TRISTATE = 0;

            /// <summary>FT X-Series CBUS EEPROM options - RxLED#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_RXLED">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_RXLED` on google.com</a></footer>
            public const byte FT_CBUS_RXLED = 1;

            /// <summary>FT X-Series CBUS EEPROM options - TxLED#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXLED">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXLED = 2;

            /// <summary>FT X-Series CBUS EEPROM options - TxRxLED#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXRXLED">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXRXLED` on google.com</a></footer>
            public const byte FT_CBUS_TXRXLED = 3;

            /// <summary>FT X-Series CBUS EEPROM options - PwrEn#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_PWREN">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_PWREN` on google.com</a></footer>
            public const byte FT_CBUS_PWREN = 4;

            /// <summary>FT X-Series CBUS EEPROM options - Sleep#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_SLEEP">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_SLEEP` on google.com</a></footer>
            public const byte FT_CBUS_SLEEP = 5;

            /// <summary>FT X-Series CBUS EEPROM options - Drive_0</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_Drive_0">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_Drive_0` on google.com</a></footer>
            public const byte FT_CBUS_Drive_0 = 6;

            /// <summary>FT X-Series CBUS EEPROM options - Drive_1</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_Drive_1">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_Drive_1` on google.com</a></footer>
            public const byte FT_CBUS_Drive_1 = 7;

            /// <summary>FT X-Series CBUS EEPROM options - GPIO</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_GPIO">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_GPIO` on google.com</a></footer>
            public const byte FT_CBUS_GPIO = 8;

            /// <summary>FT X-Series CBUS EEPROM options - TxdEn</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXDEN">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_TXDEN` on google.com</a></footer>
            public const byte FT_CBUS_TXDEN = 9;

            /// <summary>FT X-Series CBUS EEPROM options - Clk24MHz</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK24MHz">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK24MHz` on google.com</a></footer>
            public const byte FT_CBUS_CLK24MHz = 10;

            /// <summary>FT X-Series CBUS EEPROM options - Clk12MHz</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK12MHz">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK12MHz` on google.com</a></footer>
            public const byte FT_CBUS_CLK12MHz = 11;

            /// <summary>FT X-Series CBUS EEPROM options - Clk6MHz</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK6MHz">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_CLK6MHz` on google.com</a></footer>
            public const byte FT_CBUS_CLK6MHz = 12;

            /// <summary>FT X-Series CBUS EEPROM options - BCD_Charger</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_BCD_Charger">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_BCD_Charger` on google.com</a></footer>
            public const byte FT_CBUS_BCD_Charger = 13;

            /// <summary>FT X-Series CBUS EEPROM options - BCD_Charger#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_BCD_Charger_N">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_BCD_Charger_N` on google.com</a></footer>
            public const byte FT_CBUS_BCD_Charger_N = 14;

            /// <summary>FT X-Series CBUS EEPROM options - I2C_TXE#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_I2C_TXE">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_I2C_TXE` on google.com</a></footer>
            public const byte FT_CBUS_I2C_TXE = 15;

            /// <summary>FT X-Series CBUS EEPROM options - I2C_RXF#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_I2C_RXF">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_I2C_RXF` on google.com</a></footer>
            public const byte FT_CBUS_I2C_RXF = 16;

            /// <summary>FT X-Series CBUS EEPROM options - VBUS_Sense</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_VBUS_Sense">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_VBUS_Sense` on google.com</a></footer>
            public const byte FT_CBUS_VBUS_Sense = 17;

            /// <summary>FT X-Series CBUS EEPROM options - BitBang_WR#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_BitBang_WR">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_BitBang_WR` on google.com</a></footer>
            public const byte FT_CBUS_BitBang_WR = 18;

            /// <summary>FT X-Series CBUS EEPROM options - BitBang_RD#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_BitBang_RD">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_BitBang_RD` on google.com</a></footer>
            public const byte FT_CBUS_BitBang_RD = 19;

            /// <summary>FT X-Series CBUS EEPROM options - Time_Stampe</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_Time_Stamp">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_Time_Stamp` on google.com</a></footer>
            public const byte FT_CBUS_Time_Stamp = 20;

            /// <summary>FT X-Series CBUS EEPROM options - Keep_Awake#</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_CBUS_OPTIONS.FT_CBUS_Keep_Awake">`FT_XSERIES_CBUS_OPTIONS.FT_CBUS_Keep_Awake` on google.com</a></footer>
            public const byte FT_CBUS_Keep_Awake = 21;
        }

        /// <summary>
        /// Flags that provide information on the FTDI device state
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_FLAGS">`FTDI.FT_FLAGS` on google.com</a></footer>
        public class FT_FLAGS
        {
            /// <summary>Indicates that the device is open</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLAGS.FT_FLAGS_OPENED">`FT_FLAGS.FT_FLAGS_OPENED` on google.com</a></footer>
            public const uint FT_FLAGS_OPENED = 1;

            /// <summary>
            /// Indicates that the device is enumerated as a hi-speed USB device
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_FLAGS.FT_FLAGS_HISPEED">`FT_FLAGS.FT_FLAGS_HISPEED` on google.com</a></footer>
            public const uint FT_FLAGS_HISPEED = 2;
        }

        /// <summary>
        /// Valid values for drive current options on FT2232H, FT4232H and FT232H devices.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_DRIVE_CURRENT">`FTDI.FT_DRIVE_CURRENT` on google.com</a></footer>
        public class FT_DRIVE_CURRENT
        {
            /// <summary>4mA drive current</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DRIVE_CURRENT.FT_DRIVE_CURRENT_4MA">`FT_DRIVE_CURRENT.FT_DRIVE_CURRENT_4MA` on google.com</a></footer>
            public const byte FT_DRIVE_CURRENT_4MA = 4;

            /// <summary>8mA drive current</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DRIVE_CURRENT.FT_DRIVE_CURRENT_8MA">`FT_DRIVE_CURRENT.FT_DRIVE_CURRENT_8MA` on google.com</a></footer>
            public const byte FT_DRIVE_CURRENT_8MA = 8;

            /// <summary>12mA drive current</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DRIVE_CURRENT.FT_DRIVE_CURRENT_12MA">`FT_DRIVE_CURRENT.FT_DRIVE_CURRENT_12MA` on google.com</a></footer>
            public const byte FT_DRIVE_CURRENT_12MA = 12;

            /// <summary>16mA drive current</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DRIVE_CURRENT.FT_DRIVE_CURRENT_16MA">`FT_DRIVE_CURRENT.FT_DRIVE_CURRENT_16MA` on google.com</a></footer>
            public const byte FT_DRIVE_CURRENT_16MA = 16;
        }

        /// <summary>List of FTDI device types</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_DEVICE">`FTDI.FT_DEVICE` on google.com</a></footer>
        public enum FT_DEVICE
        {
            /// <summary>FT232B or FT245B device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_BM">`FT_DEVICE.FT_DEVICE_BM` on google.com</a></footer>
            FT_DEVICE_BM,

            /// <summary>FT8U232AM or FT8U245AM device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_AM">`FT_DEVICE.FT_DEVICE_AM` on google.com</a></footer>
            FT_DEVICE_AM,

            /// 1
            ///             <summary>FT8U100AX device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_100AX">`FT_DEVICE.FT_DEVICE_100AX` on google.com</a></footer>
            FT_DEVICE_100AX,

            /// <summary>Unknown device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_UNKNOWN">`FT_DEVICE.FT_DEVICE_UNKNOWN` on google.com</a></footer>
            FT_DEVICE_UNKNOWN,

            /// <summary>FT2232 device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_2232">`FT_DEVICE.FT_DEVICE_2232` on google.com</a></footer>
            FT_DEVICE_2232,

            /// <summary>FT232R or FT245R device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_232R">`FT_DEVICE.FT_DEVICE_232R` on google.com</a></footer>
            FT_DEVICE_232R,

            /// 5
            ///             <summary>FT2232H device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_2232H">`FT_DEVICE.FT_DEVICE_2232H` on google.com</a></footer>
            FT_DEVICE_2232H,

            /// 6
            ///             <summary>FT4232H device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4232H">`FT_DEVICE.FT_DEVICE_4232H` on google.com</a></footer>
            FT_DEVICE_4232H,

            /// 7
            ///             <summary>FT232H device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_232H">`FT_DEVICE.FT_DEVICE_232H` on google.com</a></footer>
            FT_DEVICE_232H,

            /// 8
            ///             <summary>FT X-Series device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_X_SERIES">`FT_DEVICE.FT_DEVICE_X_SERIES` on google.com</a></footer>
            FT_DEVICE_X_SERIES,

            /// 9
            ///             <summary>FT4222 hi-speed device Mode 0 - 2 interfaces</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4222H_0">`FT_DEVICE.FT_DEVICE_4222H_0` on google.com</a></footer>
            FT_DEVICE_4222H_0,

            /// 10
            ///             <summary>FT4222 hi-speed device Mode 1 or 2 - 4 interfaces</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4222H_1_2">`FT_DEVICE.FT_DEVICE_4222H_1_2` on google.com</a></footer>
            FT_DEVICE_4222H_1_2,

            /// 11
            ///             <summary>FT4222 hi-speed device Mode 3 - 1 interface</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4222H_3">`FT_DEVICE.FT_DEVICE_4222H_3` on google.com</a></footer>
            FT_DEVICE_4222H_3,

            /// 12
            ///             <summary>OTP programmer board for the FT4222.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4222_PROG">`FT_DEVICE.FT_DEVICE_4222_PROG` on google.com</a></footer>
            FT_DEVICE_4222_PROG,

            /// 13
            ///             <summary>OTP programmer board for the FT900.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_FT900">`FT_DEVICE.FT_DEVICE_FT900` on google.com</a></footer>
            FT_DEVICE_FT900,

            /// 14
            ///             <summary>OTP programmer board for the FT930.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_FT930">`FT_DEVICE.FT_DEVICE_FT930` on google.com</a></footer>
            FT_DEVICE_FT930,

            /// 15
            ///             <summary>Flash programmer board for the UMFTPD3A.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_UMFTPD3A">`FT_DEVICE.FT_DEVICE_UMFTPD3A` on google.com</a></footer>
            FT_DEVICE_UMFTPD3A,

            /// 16
            ///             <summary>FT2233HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_2233HP">`FT_DEVICE.FT_DEVICE_2233HP` on google.com</a></footer>
            FT_DEVICE_2233HP,

            /// 17
            ///             <summary>FT4233HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4233HP">`FT_DEVICE.FT_DEVICE_4233HP` on google.com</a></footer>
            FT_DEVICE_4233HP,

            /// 18
            ///             <summary>FT2233HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_2232HP">`FT_DEVICE.FT_DEVICE_2232HP` on google.com</a></footer>
            FT_DEVICE_2232HP,

            /// 19
            ///             <summary>FT4233HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4232HP">`FT_DEVICE.FT_DEVICE_4232HP` on google.com</a></footer>
            FT_DEVICE_4232HP,

            /// 20
            ///             <summary>FT233HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_233HP">`FT_DEVICE.FT_DEVICE_233HP` on google.com</a></footer>
            FT_DEVICE_233HP,

            /// 21
            ///             <summary>FT232HP hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_232HP">`FT_DEVICE.FT_DEVICE_232HP` on google.com</a></footer>
            FT_DEVICE_232HP,

            /// 22
            ///             <summary>FT2233HA hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_2232HA">`FT_DEVICE.FT_DEVICE_2232HA` on google.com</a></footer>
            FT_DEVICE_2232HA,

            /// 23
            ///             <summary>FT4233HA hi-speed device.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE.FT_DEVICE_4232HA">`FT_DEVICE.FT_DEVICE_4232HA` on google.com</a></footer>
            FT_DEVICE_4232HA,
        }

        /// <summary>
        /// Type that holds device information for GetDeviceInformation method.
        /// Used with FT_GetDeviceInfo and FT_GetDeviceInfoDetail in FTD2XX.DLL
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE">`FTDI.FT_DEVICE_INFO_NODE` on google.com</a></footer>
        public class FT_DEVICE_INFO_NODE
        {
            /// <summary>
            /// Indicates device state.  Can be any combination of the following: FT_FLAGS_OPENED, FT_FLAGS_HISPEED
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.Flags">`FT_DEVICE_INFO_NODE.Flags` on google.com</a></footer>
            public uint Flags;

            /// <summary>
            /// Indicates the device type.  Can be one of the following: FT_DEVICE_232R, FT_DEVICE_2232C, FT_DEVICE_BM, FT_DEVICE_AM, FT_DEVICE_100AX or FT_DEVICE_UNKNOWN
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.Type">`FT_DEVICE_INFO_NODE.Type` on google.com</a></footer>
            public FTDI.FT_DEVICE Type;

            /// <summary>The Vendor ID and Product ID of the device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.ID">`FT_DEVICE_INFO_NODE.ID` on google.com</a></footer>
            public uint ID;

            /// <summary>The physical location identifier of the device</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.LocId">`FT_DEVICE_INFO_NODE.LocId` on google.com</a></footer>
            public uint LocId;

            /// <summary>The device serial number</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.SerialNumber">`FT_DEVICE_INFO_NODE.SerialNumber` on google.com</a></footer>
            public string SerialNumber;

            /// <summary>The device description</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.Description">`FT_DEVICE_INFO_NODE.Description` on google.com</a></footer>
            public string Description;

            /// <summary>
            /// The device handle.  This value is not used externally and is provided for information only.
            /// If the device is not open, this value is 0.
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_DEVICE_INFO_NODE.ftHandle">`FT_DEVICE_INFO_NODE.ftHandle` on google.com</a></footer>
            public IntPtr ftHandle;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private class FT_PROGRAM_DATA
        {
            public uint Signature1;
            public uint Signature2;
            public uint Version;
            public ushort VendorID;
            public ushort ProductID;
            public IntPtr Manufacturer;
            public IntPtr ManufacturerID;
            public IntPtr Description;
            public IntPtr SerialNumber;
            public ushort MaxPower;
            public ushort PnP;
            public ushort SelfPowered;
            public ushort RemoteWakeup;
            public byte Rev4;
            public byte IsoIn;
            public byte IsoOut;
            public byte PullDownEnable;
            public byte SerNumEnable;
            public byte USBVersionEnable;
            public ushort USBVersion;
            public byte Rev5;
            public byte IsoInA;
            public byte IsoInB;
            public byte IsoOutA;
            public byte IsoOutB;
            public byte PullDownEnable5;
            public byte SerNumEnable5;
            public byte USBVersionEnable5;
            public ushort USBVersion5;
            public byte AIsHighCurrent;
            public byte BIsHighCurrent;
            public byte IFAIsFifo;
            public byte IFAIsFifoTar;
            public byte IFAIsFastSer;
            public byte AIsVCP;
            public byte IFBIsFifo;
            public byte IFBIsFifoTar;
            public byte IFBIsFastSer;
            public byte BIsVCP;
            public byte UseExtOsc;
            public byte HighDriveIOs;
            public byte EndpointSize;
            public byte PullDownEnableR;
            public byte SerNumEnableR;
            public byte InvertTXD;
            public byte InvertRXD;
            public byte InvertRTS;
            public byte InvertCTS;
            public byte InvertDTR;
            public byte InvertDSR;
            public byte InvertDCD;
            public byte InvertRI;
            public byte Cbus0;
            public byte Cbus1;
            public byte Cbus2;
            public byte Cbus3;
            public byte Cbus4;
            public byte RIsD2XX;
            public byte PullDownEnable7;
            public byte SerNumEnable7;
            public byte ALSlowSlew;
            public byte ALSchmittInput;
            public byte ALDriveCurrent;
            public byte AHSlowSlew;
            public byte AHSchmittInput;
            public byte AHDriveCurrent;
            public byte BLSlowSlew;
            public byte BLSchmittInput;
            public byte BLDriveCurrent;
            public byte BHSlowSlew;
            public byte BHSchmittInput;
            public byte BHDriveCurrent;
            public byte IFAIsFifo7;
            public byte IFAIsFifoTar7;
            public byte IFAIsFastSer7;
            public byte AIsVCP7;
            public byte IFBIsFifo7;
            public byte IFBIsFifoTar7;
            public byte IFBIsFastSer7;
            public byte BIsVCP7;
            public byte PowerSaveEnable;
            public byte PullDownEnable8;
            public byte SerNumEnable8;
            public byte ASlowSlew;
            public byte ASchmittInput;
            public byte ADriveCurrent;
            public byte BSlowSlew;
            public byte BSchmittInput;
            public byte BDriveCurrent;
            public byte CSlowSlew;
            public byte CSchmittInput;
            public byte CDriveCurrent;
            public byte DSlowSlew;
            public byte DSchmittInput;
            public byte DDriveCurrent;
            public byte ARIIsTXDEN;
            public byte BRIIsTXDEN;
            public byte CRIIsTXDEN;
            public byte DRIIsTXDEN;
            public byte AIsVCP8;
            public byte BIsVCP8;
            public byte CIsVCP8;
            public byte DIsVCP8;
            public byte PullDownEnableH;
            public byte SerNumEnableH;
            public byte ACSlowSlewH;
            public byte ACSchmittInputH;
            public byte ACDriveCurrentH;
            public byte ADSlowSlewH;
            public byte ADSchmittInputH;
            public byte ADDriveCurrentH;
            public byte Cbus0H;
            public byte Cbus1H;
            public byte Cbus2H;
            public byte Cbus3H;
            public byte Cbus4H;
            public byte Cbus5H;
            public byte Cbus6H;
            public byte Cbus7H;
            public byte Cbus8H;
            public byte Cbus9H;
            public byte IsFifoH;
            public byte IsFifoTarH;
            public byte IsFastSerH;
            public byte IsFT1248H;
            public byte FT1248CpolH;
            public byte FT1248LsbH;
            public byte FT1248FlowControlH;
            public byte IsVCPH;
            public byte PowerSaveEnableH;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct FT_EEPROM_HEADER
        {
            public uint deviceType;
            public ushort VendorId;
            public ushort ProductId;
            public byte SerNumEnable;
            public ushort MaxPower;
            public byte SelfPowered;
            public byte RemoteWakeup;
            public byte PullDownEnable;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct FT_XSERIES_DATA
        {
            public FTDI.FT_EEPROM_HEADER common;
            public byte ACSlowSlew;
            public byte ACSchmittInput;
            public byte ACDriveCurrent;
            public byte ADSlowSlew;
            public byte ADSchmittInput;
            public byte ADDriveCurrent;
            public byte Cbus0;
            public byte Cbus1;
            public byte Cbus2;
            public byte Cbus3;
            public byte Cbus4;
            public byte Cbus5;
            public byte Cbus6;
            public byte InvertTXD;
            public byte InvertRXD;
            public byte InvertRTS;
            public byte InvertCTS;
            public byte InvertDTR;
            public byte InvertDSR;
            public byte InvertDCD;
            public byte InvertRI;
            public byte BCDEnable;
            public byte BCDForceCbusPWREN;
            public byte BCDDisableSleep;
            public ushort I2CSlaveAddress;
            public uint I2CDeviceId;
            public byte I2CDisableSchmitt;
            public byte FT1248Cpol;
            public byte FT1248Lsb;
            public byte FT1248FlowControl;
            public byte RS485EchoSuppress;
            public byte PowerSaveEnable;
            public byte DriverType;
        }

        /// <summary>
        /// Common EEPROM elements for all devices.  Inherited to specific device type EEPROMs.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_EEPROM_DATA">`FTDI.FT_EEPROM_DATA` on google.com</a></footer>
        public class FT_EEPROM_DATA
        {
            /// <summary>Vendor ID as supplied by the USB Implementers Forum</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.VendorID">`FT_EEPROM_DATA.VendorID` on google.com</a></footer>
            public ushort VendorID = 1027;

            /// <summary>Product ID</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.ProductID">`FT_EEPROM_DATA.ProductID` on google.com</a></footer>
            public ushort ProductID = 24577;

            /// <summary>Manufacturer name string</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.Manufacturer">`FT_EEPROM_DATA.Manufacturer` on google.com</a></footer>
            public string Manufacturer = nameof(FTDI);

            /// <summary>
            /// Manufacturer name abbreviation to be used as a prefix for automatically generated serial numbers
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.ManufacturerID">`FT_EEPROM_DATA.ManufacturerID` on google.com</a></footer>
            public string ManufacturerID = "FT";

            /// <summary>Device description string</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.Description">`FT_EEPROM_DATA.Description` on google.com</a></footer>
            public string Description = "USB-Serial Converter";

            /// <summary>Device serial number string</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.SerialNumber">`FT_EEPROM_DATA.SerialNumber` on google.com</a></footer>
            public string SerialNumber = "";

            /// <summary>Maximum power the device needs</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.MaxPower">`FT_EEPROM_DATA.MaxPower` on google.com</a></footer>
            public ushort MaxPower = 144;

            /// <summary>
            /// Indicates if the device has its own power supply (self-powered) or gets power from the USB port (bus-powered)
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.SelfPowered">`FT_EEPROM_DATA.SelfPowered` on google.com</a></footer>
            public bool SelfPowered;

            /// <summary>
            /// Determines if the device can wake the host PC from suspend by toggling the RI line
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EEPROM_DATA.RemoteWakeup">`FT_EEPROM_DATA.RemoteWakeup` on google.com</a></footer>
            public bool RemoteWakeup;
        }

        /// <summary>
        /// EEPROM structure specific to FT232B and FT245B devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT232B_EEPROM_STRUCTURE">`FTDI.FT232B_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT232B_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232B_EEPROM_STRUCTURE.PullDownEnable">`FT232B_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232B_EEPROM_STRUCTURE.SerNumEnable">`FT232B_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if the USB version number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232B_EEPROM_STRUCTURE.USBVersionEnable">`FT232B_EEPROM_STRUCTURE.USBVersionEnable` on google.com</a></footer>
            public bool USBVersionEnable = true;

            /// <summary>
            /// The USB version number.  Should be either 0x0110 (USB 1.1) or 0x0200 (USB 2.0)
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232B_EEPROM_STRUCTURE.USBVersion">`FT232B_EEPROM_STRUCTURE.USBVersion` on google.com</a></footer>
            public ushort USBVersion = 512;
        }

        /// <summary>
        /// EEPROM structure specific to FT2232 devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT2232_EEPROM_STRUCTURE">`FTDI.FT2232_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT2232_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.PullDownEnable">`FT2232_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.SerNumEnable">`FT2232_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if the USB version number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.USBVersionEnable">`FT2232_EEPROM_STRUCTURE.USBVersionEnable` on google.com</a></footer>
            public bool USBVersionEnable = true;

            /// <summary>
            /// The USB version number.  Should be either 0x0110 (USB 1.1) or 0x0200 (USB 2.0)
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.USBVersion">`FT2232_EEPROM_STRUCTURE.USBVersion` on google.com</a></footer>
            public ushort USBVersion = 512;

            /// <summary>Enables high current IOs on channel A</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.AIsHighCurrent">`FT2232_EEPROM_STRUCTURE.AIsHighCurrent` on google.com</a></footer>
            public bool AIsHighCurrent;

            /// <summary>Enables high current IOs on channel B</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.BIsHighCurrent">`FT2232_EEPROM_STRUCTURE.BIsHighCurrent` on google.com</a></footer>
            public bool BIsHighCurrent;

            /// <summary>Determines if channel A is in FIFO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFAIsFifo">`FT2232_EEPROM_STRUCTURE.IFAIsFifo` on google.com</a></footer>
            public bool IFAIsFifo;

            /// <summary>Determines if channel A is in FIFO target mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFAIsFifoTar">`FT2232_EEPROM_STRUCTURE.IFAIsFifoTar` on google.com</a></footer>
            public bool IFAIsFifoTar;

            /// <summary>Determines if channel A is in fast serial mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFAIsFastSer">`FT2232_EEPROM_STRUCTURE.IFAIsFastSer` on google.com</a></footer>
            public bool IFAIsFastSer;

            /// <summary>Determines if channel A loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.AIsVCP">`FT2232_EEPROM_STRUCTURE.AIsVCP` on google.com</a></footer>
            public bool AIsVCP = true;

            /// <summary>Determines if channel B is in FIFO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFBIsFifo">`FT2232_EEPROM_STRUCTURE.IFBIsFifo` on google.com</a></footer>
            public bool IFBIsFifo;

            /// <summary>Determines if channel B is in FIFO target mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFBIsFifoTar">`FT2232_EEPROM_STRUCTURE.IFBIsFifoTar` on google.com</a></footer>
            public bool IFBIsFifoTar;

            /// <summary>Determines if channel B is in fast serial mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.IFBIsFastSer">`FT2232_EEPROM_STRUCTURE.IFBIsFastSer` on google.com</a></footer>
            public bool IFBIsFastSer;

            /// <summary>Determines if channel B loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232_EEPROM_STRUCTURE.BIsVCP">`FT2232_EEPROM_STRUCTURE.BIsVCP` on google.com</a></footer>
            public bool BIsVCP = true;
        }

        /// <summary>
        /// EEPROM structure specific to FT232R and FT245R devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT232R_EEPROM_STRUCTURE">`FTDI.FT232R_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT232R_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Disables the FT232R internal clock source.
            /// If the device has external oscillator enabled it must have an external oscillator fitted to function
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.UseExtOsc">`FT232R_EEPROM_STRUCTURE.UseExtOsc` on google.com</a></footer>
            public bool UseExtOsc;

            /// <summary>Enables high current IOs</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.HighDriveIOs">`FT232R_EEPROM_STRUCTURE.HighDriveIOs` on google.com</a></footer>
            public bool HighDriveIOs;

            /// <summary>
            /// Sets the endpoint size.  This should always be set to 64
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.EndpointSize">`FT232R_EEPROM_STRUCTURE.EndpointSize` on google.com</a></footer>
            public byte EndpointSize = 64;

            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.PullDownEnable">`FT232R_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.SerNumEnable">`FT232R_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Inverts the sense of the TXD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertTXD">`FT232R_EEPROM_STRUCTURE.InvertTXD` on google.com</a></footer>
            public bool InvertTXD;

            /// <summary>Inverts the sense of the RXD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertRXD">`FT232R_EEPROM_STRUCTURE.InvertRXD` on google.com</a></footer>
            public bool InvertRXD;

            /// <summary>Inverts the sense of the RTS line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertRTS">`FT232R_EEPROM_STRUCTURE.InvertRTS` on google.com</a></footer>
            public bool InvertRTS;

            /// <summary>Inverts the sense of the CTS line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertCTS">`FT232R_EEPROM_STRUCTURE.InvertCTS` on google.com</a></footer>
            public bool InvertCTS;

            /// <summary>Inverts the sense of the DTR line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertDTR">`FT232R_EEPROM_STRUCTURE.InvertDTR` on google.com</a></footer>
            public bool InvertDTR;

            /// <summary>Inverts the sense of the DSR line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertDSR">`FT232R_EEPROM_STRUCTURE.InvertDSR` on google.com</a></footer>
            public bool InvertDSR;

            /// <summary>Inverts the sense of the DCD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertDCD">`FT232R_EEPROM_STRUCTURE.InvertDCD` on google.com</a></footer>
            public bool InvertDCD;

            /// <summary>Inverts the sense of the RI line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.InvertRI">`FT232R_EEPROM_STRUCTURE.InvertRI` on google.com</a></footer>
            public bool InvertRI;

            /// <summary>
            /// Sets the function of the CBUS0 pin for FT232R devices.
            /// Valid values are FT_CBUS_TXDEN, FT_CBUS_PWRON , FT_CBUS_RXLED, FT_CBUS_TXLED,
            /// FT_CBUS_TXRXLED, FT_CBUS_SLEEP, FT_CBUS_CLK48, FT_CBUS_CLK24, FT_CBUS_CLK12,
            /// FT_CBUS_CLK6, FT_CBUS_IOMODE, FT_CBUS_BITBANG_WR, FT_CBUS_BITBANG_RD
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.Cbus0">`FT232R_EEPROM_STRUCTURE.Cbus0` on google.com</a></footer>
            public byte Cbus0 = 5;

            /// <summary>
            /// Sets the function of the CBUS1 pin for FT232R devices.
            /// Valid values are FT_CBUS_TXDEN, FT_CBUS_PWRON , FT_CBUS_RXLED, FT_CBUS_TXLED,
            /// FT_CBUS_TXRXLED, FT_CBUS_SLEEP, FT_CBUS_CLK48, FT_CBUS_CLK24, FT_CBUS_CLK12,
            /// FT_CBUS_CLK6, FT_CBUS_IOMODE, FT_CBUS_BITBANG_WR, FT_CBUS_BITBANG_RD
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.Cbus1">`FT232R_EEPROM_STRUCTURE.Cbus1` on google.com</a></footer>
            public byte Cbus1 = 5;

            /// <summary>
            /// Sets the function of the CBUS2 pin for FT232R devices.
            /// Valid values are FT_CBUS_TXDEN, FT_CBUS_PWRON , FT_CBUS_RXLED, FT_CBUS_TXLED,
            /// FT_CBUS_TXRXLED, FT_CBUS_SLEEP, FT_CBUS_CLK48, FT_CBUS_CLK24, FT_CBUS_CLK12,
            /// FT_CBUS_CLK6, FT_CBUS_IOMODE, FT_CBUS_BITBANG_WR, FT_CBUS_BITBANG_RD
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.Cbus2">`FT232R_EEPROM_STRUCTURE.Cbus2` on google.com</a></footer>
            public byte Cbus2 = 5;

            /// <summary>
            /// Sets the function of the CBUS3 pin for FT232R devices.
            /// Valid values are FT_CBUS_TXDEN, FT_CBUS_PWRON , FT_CBUS_RXLED, FT_CBUS_TXLED,
            /// FT_CBUS_TXRXLED, FT_CBUS_SLEEP, FT_CBUS_CLK48, FT_CBUS_CLK24, FT_CBUS_CLK12,
            /// FT_CBUS_CLK6, FT_CBUS_IOMODE, FT_CBUS_BITBANG_WR, FT_CBUS_BITBANG_RD
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.Cbus3">`FT232R_EEPROM_STRUCTURE.Cbus3` on google.com</a></footer>
            public byte Cbus3 = 5;

            /// <summary>
            /// Sets the function of the CBUS4 pin for FT232R devices.
            /// Valid values are FT_CBUS_TXDEN, FT_CBUS_PWRON , FT_CBUS_RXLED, FT_CBUS_TXLED,
            /// FT_CBUS_TXRXLED, FT_CBUS_SLEEP, FT_CBUS_CLK48, FT_CBUS_CLK24, FT_CBUS_CLK12,
            /// FT_CBUS_CLK6
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.Cbus4">`FT232R_EEPROM_STRUCTURE.Cbus4` on google.com</a></footer>
            public byte Cbus4 = 5;

            /// <summary>Determines if the VCP driver is loaded</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232R_EEPROM_STRUCTURE.RIsD2XX">`FT232R_EEPROM_STRUCTURE.RIsD2XX` on google.com</a></footer>
            public bool RIsD2XX;
        }

        /// <summary>
        /// EEPROM structure specific to FT2232H devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT2232H_EEPROM_STRUCTURE">`FTDI.FT2232H_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT2232H_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.PullDownEnable">`FT2232H_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.SerNumEnable">`FT2232H_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if AL pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.ALSlowSlew">`FT2232H_EEPROM_STRUCTURE.ALSlowSlew` on google.com</a></footer>
            public bool ALSlowSlew;

            /// <summary>Determines if the AL pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.ALSchmittInput">`FT2232H_EEPROM_STRUCTURE.ALSchmittInput` on google.com</a></footer>
            public bool ALSchmittInput;

            /// <summary>
            /// Determines the AL pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.ALDriveCurrent">`FT2232H_EEPROM_STRUCTURE.ALDriveCurrent` on google.com</a></footer>
            public byte ALDriveCurrent = 4;

            /// <summary>Determines if AH pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.AHSlowSlew">`FT2232H_EEPROM_STRUCTURE.AHSlowSlew` on google.com</a></footer>
            public bool AHSlowSlew;

            /// <summary>Determines if the AH pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.AHSchmittInput">`FT2232H_EEPROM_STRUCTURE.AHSchmittInput` on google.com</a></footer>
            public bool AHSchmittInput;

            /// <summary>
            /// Determines the AH pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.AHDriveCurrent">`FT2232H_EEPROM_STRUCTURE.AHDriveCurrent` on google.com</a></footer>
            public byte AHDriveCurrent = 4;

            /// <summary>Determines if BL pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BLSlowSlew">`FT2232H_EEPROM_STRUCTURE.BLSlowSlew` on google.com</a></footer>
            public bool BLSlowSlew;

            /// <summary>Determines if the BL pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BLSchmittInput">`FT2232H_EEPROM_STRUCTURE.BLSchmittInput` on google.com</a></footer>
            public bool BLSchmittInput;

            /// <summary>
            /// Determines the BL pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BLDriveCurrent">`FT2232H_EEPROM_STRUCTURE.BLDriveCurrent` on google.com</a></footer>
            public byte BLDriveCurrent = 4;

            /// <summary>Determines if BH pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BHSlowSlew">`FT2232H_EEPROM_STRUCTURE.BHSlowSlew` on google.com</a></footer>
            public bool BHSlowSlew;

            /// <summary>Determines if the BH pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BHSchmittInput">`FT2232H_EEPROM_STRUCTURE.BHSchmittInput` on google.com</a></footer>
            public bool BHSchmittInput;

            /// <summary>
            /// Determines the BH pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BHDriveCurrent">`FT2232H_EEPROM_STRUCTURE.BHDriveCurrent` on google.com</a></footer>
            public byte BHDriveCurrent = 4;

            /// <summary>Determines if channel A is in FIFO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFAIsFifo">`FT2232H_EEPROM_STRUCTURE.IFAIsFifo` on google.com</a></footer>
            public bool IFAIsFifo;

            /// <summary>Determines if channel A is in FIFO target mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFAIsFifoTar">`FT2232H_EEPROM_STRUCTURE.IFAIsFifoTar` on google.com</a></footer>
            public bool IFAIsFifoTar;

            /// <summary>Determines if channel A is in fast serial mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFAIsFastSer">`FT2232H_EEPROM_STRUCTURE.IFAIsFastSer` on google.com</a></footer>
            public bool IFAIsFastSer;

            /// <summary>Determines if channel A loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.AIsVCP">`FT2232H_EEPROM_STRUCTURE.AIsVCP` on google.com</a></footer>
            public bool AIsVCP = true;

            /// <summary>Determines if channel B is in FIFO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFBIsFifo">`FT2232H_EEPROM_STRUCTURE.IFBIsFifo` on google.com</a></footer>
            public bool IFBIsFifo;

            /// <summary>Determines if channel B is in FIFO target mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFBIsFifoTar">`FT2232H_EEPROM_STRUCTURE.IFBIsFifoTar` on google.com</a></footer>
            public bool IFBIsFifoTar;

            /// <summary>Determines if channel B is in fast serial mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.IFBIsFastSer">`FT2232H_EEPROM_STRUCTURE.IFBIsFastSer` on google.com</a></footer>
            public bool IFBIsFastSer;

            /// <summary>Determines if channel B loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.BIsVCP">`FT2232H_EEPROM_STRUCTURE.BIsVCP` on google.com</a></footer>
            public bool BIsVCP = true;

            /// <summary>
            /// For self-powered designs, keeps the FT2232H in low power state until BCBUS7 is high
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT2232H_EEPROM_STRUCTURE.PowerSaveEnable">`FT2232H_EEPROM_STRUCTURE.PowerSaveEnable` on google.com</a></footer>
            public bool PowerSaveEnable;
        }

        /// <summary>
        /// EEPROM structure specific to FT4232H devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT4232H_EEPROM_STRUCTURE">`FTDI.FT4232H_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT4232H_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.PullDownEnable">`FT4232H_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.SerNumEnable">`FT4232H_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if A pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.ASlowSlew">`FT4232H_EEPROM_STRUCTURE.ASlowSlew` on google.com</a></footer>
            public bool ASlowSlew;

            /// <summary>Determines if the A pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.ASchmittInput">`FT4232H_EEPROM_STRUCTURE.ASchmittInput` on google.com</a></footer>
            public bool ASchmittInput;

            /// <summary>
            /// Determines the A pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.ADriveCurrent">`FT4232H_EEPROM_STRUCTURE.ADriveCurrent` on google.com</a></footer>
            public byte ADriveCurrent = 4;

            /// <summary>Determines if B pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.BSlowSlew">`FT4232H_EEPROM_STRUCTURE.BSlowSlew` on google.com</a></footer>
            public bool BSlowSlew;

            /// <summary>Determines if the B pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.BSchmittInput">`FT4232H_EEPROM_STRUCTURE.BSchmittInput` on google.com</a></footer>
            public bool BSchmittInput;

            /// <summary>
            /// Determines the B pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.BDriveCurrent">`FT4232H_EEPROM_STRUCTURE.BDriveCurrent` on google.com</a></footer>
            public byte BDriveCurrent = 4;

            /// <summary>Determines if C pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.CSlowSlew">`FT4232H_EEPROM_STRUCTURE.CSlowSlew` on google.com</a></footer>
            public bool CSlowSlew;

            /// <summary>Determines if the C pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.CSchmittInput">`FT4232H_EEPROM_STRUCTURE.CSchmittInput` on google.com</a></footer>
            public bool CSchmittInput;

            /// <summary>
            /// Determines the C pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.CDriveCurrent">`FT4232H_EEPROM_STRUCTURE.CDriveCurrent` on google.com</a></footer>
            public byte CDriveCurrent = 4;

            /// <summary>Determines if D pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.DSlowSlew">`FT4232H_EEPROM_STRUCTURE.DSlowSlew` on google.com</a></footer>
            public bool DSlowSlew;

            /// <summary>Determines if the D pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.DSchmittInput">`FT4232H_EEPROM_STRUCTURE.DSchmittInput` on google.com</a></footer>
            public bool DSchmittInput;

            /// <summary>
            /// Determines the D pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.DDriveCurrent">`FT4232H_EEPROM_STRUCTURE.DDriveCurrent` on google.com</a></footer>
            public byte DDriveCurrent = 4;

            /// <summary>RI of port A acts as RS485 transmit enable (TXDEN)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.ARIIsTXDEN">`FT4232H_EEPROM_STRUCTURE.ARIIsTXDEN` on google.com</a></footer>
            public bool ARIIsTXDEN;

            /// <summary>RI of port B acts as RS485 transmit enable (TXDEN)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.BRIIsTXDEN">`FT4232H_EEPROM_STRUCTURE.BRIIsTXDEN` on google.com</a></footer>
            public bool BRIIsTXDEN;

            /// <summary>RI of port C acts as RS485 transmit enable (TXDEN)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.CRIIsTXDEN">`FT4232H_EEPROM_STRUCTURE.CRIIsTXDEN` on google.com</a></footer>
            public bool CRIIsTXDEN;

            /// <summary>RI of port D acts as RS485 transmit enable (TXDEN)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.DRIIsTXDEN">`FT4232H_EEPROM_STRUCTURE.DRIIsTXDEN` on google.com</a></footer>
            public bool DRIIsTXDEN;

            /// <summary>Determines if channel A loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.AIsVCP">`FT4232H_EEPROM_STRUCTURE.AIsVCP` on google.com</a></footer>
            public bool AIsVCP = true;

            /// <summary>Determines if channel B loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.BIsVCP">`FT4232H_EEPROM_STRUCTURE.BIsVCP` on google.com</a></footer>
            public bool BIsVCP = true;

            /// <summary>Determines if channel C loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.CIsVCP">`FT4232H_EEPROM_STRUCTURE.CIsVCP` on google.com</a></footer>
            public bool CIsVCP = true;

            /// <summary>Determines if channel D loads the VCP driver</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT4232H_EEPROM_STRUCTURE.DIsVCP">`FT4232H_EEPROM_STRUCTURE.DIsVCP` on google.com</a></footer>
            public bool DIsVCP = true;
        }

        /// <summary>
        /// EEPROM structure specific to FT232H devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT232H_EEPROM_STRUCTURE">`FTDI.FT232H_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT232H_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.PullDownEnable">`FT232H_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.SerNumEnable">`FT232H_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if AC pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ACSlowSlew">`FT232H_EEPROM_STRUCTURE.ACSlowSlew` on google.com</a></footer>
            public bool ACSlowSlew;

            /// <summary>Determines if the AC pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ACSchmittInput">`FT232H_EEPROM_STRUCTURE.ACSchmittInput` on google.com</a></footer>
            public bool ACSchmittInput;

            /// <summary>
            /// Determines the AC pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ACDriveCurrent">`FT232H_EEPROM_STRUCTURE.ACDriveCurrent` on google.com</a></footer>
            public byte ACDriveCurrent = 4;

            /// <summary>Determines if AD pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ADSlowSlew">`FT232H_EEPROM_STRUCTURE.ADSlowSlew` on google.com</a></footer>
            public bool ADSlowSlew;

            /// <summary>Determines if the AD pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ADSchmittInput">`FT232H_EEPROM_STRUCTURE.ADSchmittInput` on google.com</a></footer>
            public bool ADSchmittInput;

            /// <summary>
            /// Determines the AD pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.ADDriveCurrent">`FT232H_EEPROM_STRUCTURE.ADDriveCurrent` on google.com</a></footer>
            public byte ADDriveCurrent = 4;

            /// <summary>
            /// Sets the function of the CBUS0 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN, FT_CBUS_CLK30,
            /// FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus0">`FT232H_EEPROM_STRUCTURE.Cbus0` on google.com</a></footer>
            public byte Cbus0;

            /// <summary>
            /// Sets the function of the CBUS1 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN, FT_CBUS_CLK30,
            /// FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus1">`FT232H_EEPROM_STRUCTURE.Cbus1` on google.com</a></footer>
            public byte Cbus1;

            /// <summary>
            /// Sets the function of the CBUS2 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus2">`FT232H_EEPROM_STRUCTURE.Cbus2` on google.com</a></footer>
            public byte Cbus2;

            /// <summary>
            /// Sets the function of the CBUS3 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus3">`FT232H_EEPROM_STRUCTURE.Cbus3` on google.com</a></footer>
            public byte Cbus3;

            /// <summary>
            /// Sets the function of the CBUS4 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus4">`FT232H_EEPROM_STRUCTURE.Cbus4` on google.com</a></footer>
            public byte Cbus4;

            /// <summary>
            /// Sets the function of the CBUS5 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_IOMODE,
            /// FT_CBUS_TXDEN, FT_CBUS_CLK30, FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus5">`FT232H_EEPROM_STRUCTURE.Cbus5` on google.com</a></footer>
            public byte Cbus5;

            /// <summary>
            /// Sets the function of the CBUS6 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_IOMODE,
            /// FT_CBUS_TXDEN, FT_CBUS_CLK30, FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus6">`FT232H_EEPROM_STRUCTURE.Cbus6` on google.com</a></footer>
            public byte Cbus6;

            /// <summary>
            /// Sets the function of the CBUS7 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus7">`FT232H_EEPROM_STRUCTURE.Cbus7` on google.com</a></footer>
            public byte Cbus7;

            /// <summary>
            /// Sets the function of the CBUS8 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_IOMODE,
            /// FT_CBUS_TXDEN, FT_CBUS_CLK30, FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus8">`FT232H_EEPROM_STRUCTURE.Cbus8` on google.com</a></footer>
            public byte Cbus8;

            /// <summary>
            /// Sets the function of the CBUS9 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_IOMODE,
            /// FT_CBUS_TXDEN, FT_CBUS_CLK30, FT_CBUS_CLK15, FT_CBUS_CLK7_5
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.Cbus9">`FT232H_EEPROM_STRUCTURE.Cbus9` on google.com</a></footer>
            public byte Cbus9;

            /// <summary>Determines if the device is in FIFO mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.IsFifo">`FT232H_EEPROM_STRUCTURE.IsFifo` on google.com</a></footer>
            public bool IsFifo;

            /// <summary>Determines if the device is in FIFO target mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.IsFifoTar">`FT232H_EEPROM_STRUCTURE.IsFifoTar` on google.com</a></footer>
            public bool IsFifoTar;

            /// <summary>Determines if the device is in fast serial mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.IsFastSer">`FT232H_EEPROM_STRUCTURE.IsFastSer` on google.com</a></footer>
            public bool IsFastSer;

            /// <summary>Determines if the device is in FT1248 mode</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.IsFT1248">`FT232H_EEPROM_STRUCTURE.IsFT1248` on google.com</a></footer>
            public bool IsFT1248;

            /// <summary>Determines FT1248 mode clock polarity</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.FT1248Cpol">`FT232H_EEPROM_STRUCTURE.FT1248Cpol` on google.com</a></footer>
            public bool FT1248Cpol;

            /// <summary>
            /// Determines if data is ent MSB (0) or LSB (1) in FT1248 mode
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.FT1248Lsb">`FT232H_EEPROM_STRUCTURE.FT1248Lsb` on google.com</a></footer>
            public bool FT1248Lsb;

            /// <summary>Determines if FT1248 mode uses flow control</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.FT1248FlowControl">`FT232H_EEPROM_STRUCTURE.FT1248FlowControl` on google.com</a></footer>
            public bool FT1248FlowControl;

            /// <summary>Determines if the VCP driver is loaded</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.IsVCP">`FT232H_EEPROM_STRUCTURE.IsVCP` on google.com</a></footer>
            public bool IsVCP = true;

            /// <summary>
            /// For self-powered designs, keeps the FT232H in low power state until ACBUS7 is high
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT232H_EEPROM_STRUCTURE.PowerSaveEnable">`FT232H_EEPROM_STRUCTURE.PowerSaveEnable` on google.com</a></footer>
            public bool PowerSaveEnable;
        }

        /// <summary>
        /// EEPROM structure specific to X-Series devices.
        /// Inherits from FT_EEPROM_DATA.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_XSERIES_EEPROM_STRUCTURE">`FTDI.FT_XSERIES_EEPROM_STRUCTURE` on google.com</a></footer>
        public class FT_XSERIES_EEPROM_STRUCTURE : FTDI.FT_EEPROM_DATA
        {
            /// <summary>
            /// Determines if IOs are pulled down when the device is in suspend
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.PullDownEnable">`FT_XSERIES_EEPROM_STRUCTURE.PullDownEnable` on google.com</a></footer>
            public bool PullDownEnable;

            /// <summary>Determines if the serial number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.SerNumEnable">`FT_XSERIES_EEPROM_STRUCTURE.SerNumEnable` on google.com</a></footer>
            public bool SerNumEnable = true;

            /// <summary>Determines if the USB version number is enabled</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.USBVersionEnable">`FT_XSERIES_EEPROM_STRUCTURE.USBVersionEnable` on google.com</a></footer>
            public bool USBVersionEnable = true;

            /// <summary>The USB version number: 0x0200 (USB 2.0)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.USBVersion">`FT_XSERIES_EEPROM_STRUCTURE.USBVersion` on google.com</a></footer>
            public ushort USBVersion = 512;

            /// <summary>Determines if AC pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ACSlowSlew">`FT_XSERIES_EEPROM_STRUCTURE.ACSlowSlew` on google.com</a></footer>
            public byte ACSlowSlew;

            /// <summary>Determines if the AC pins have a Schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ACSchmittInput">`FT_XSERIES_EEPROM_STRUCTURE.ACSchmittInput` on google.com</a></footer>
            public byte ACSchmittInput;

            /// <summary>
            /// Determines the AC pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ACDriveCurrent">`FT_XSERIES_EEPROM_STRUCTURE.ACDriveCurrent` on google.com</a></footer>
            public byte ACDriveCurrent;

            /// <summary>Determines if AD pins have a slow slew rate</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ADSlowSlew">`FT_XSERIES_EEPROM_STRUCTURE.ADSlowSlew` on google.com</a></footer>
            public byte ADSlowSlew;

            /// <summary>Determines if AD pins have a schmitt input</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ADSchmittInput">`FT_XSERIES_EEPROM_STRUCTURE.ADSchmittInput` on google.com</a></footer>
            public byte ADSchmittInput;

            /// <summary>
            /// Determines the AD pins drive current in mA.  Valid values are FT_DRIVE_CURRENT_4MA, FT_DRIVE_CURRENT_8MA, FT_DRIVE_CURRENT_12MA or FT_DRIVE_CURRENT_16MA
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.ADDriveCurrent">`FT_XSERIES_EEPROM_STRUCTURE.ADDriveCurrent` on google.com</a></footer>
            public byte ADDriveCurrent;

            /// <summary>
            /// Sets the function of the CBUS0 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_GPIO, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus0">`FT_XSERIES_EEPROM_STRUCTURE.Cbus0` on google.com</a></footer>
            public byte Cbus0;

            /// <summary>
            /// Sets the function of the CBUS1 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_GPIO, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus1">`FT_XSERIES_EEPROM_STRUCTURE.Cbus1` on google.com</a></footer>
            public byte Cbus1;

            /// <summary>
            /// Sets the function of the CBUS2 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_GPIO, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus2">`FT_XSERIES_EEPROM_STRUCTURE.Cbus2` on google.com</a></footer>
            public byte Cbus2;

            /// <summary>
            /// Sets the function of the CBUS3 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_GPIO, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus3">`FT_XSERIES_EEPROM_STRUCTURE.Cbus3` on google.com</a></footer>
            public byte Cbus3;

            /// <summary>
            /// Sets the function of the CBUS4 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus4">`FT_XSERIES_EEPROM_STRUCTURE.Cbus4` on google.com</a></footer>
            public byte Cbus4;

            /// <summary>
            /// Sets the function of the CBUS5 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus5">`FT_XSERIES_EEPROM_STRUCTURE.Cbus5` on google.com</a></footer>
            public byte Cbus5;

            /// <summary>
            /// Sets the function of the CBUS6 pin for FT232H devices.
            /// Valid values are FT_CBUS_TRISTATE, FT_CBUS_RXLED, FT_CBUS_TXLED, FT_CBUS_TXRXLED,
            /// FT_CBUS_PWREN, FT_CBUS_SLEEP, FT_CBUS_DRIVE_0, FT_CBUS_DRIVE_1, FT_CBUS_TXDEN, FT_CBUS_CLK24,
            /// FT_CBUS_CLK12, FT_CBUS_CLK6, FT_CBUS_BCD_CHARGER, FT_CBUS_BCD_CHARGER_N, FT_CBUS_VBUS_SENSE, FT_CBUS_BITBANG_WR,
            /// FT_CBUS_BITBANG_RD, FT_CBUS_TIME_STAMP, FT_CBUS_KEEP_AWAKE
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.Cbus6">`FT_XSERIES_EEPROM_STRUCTURE.Cbus6` on google.com</a></footer>
            public byte Cbus6;

            /// <summary>Inverts the sense of the TXD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertTXD">`FT_XSERIES_EEPROM_STRUCTURE.InvertTXD` on google.com</a></footer>
            public byte InvertTXD;

            /// <summary>Inverts the sense of the RXD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertRXD">`FT_XSERIES_EEPROM_STRUCTURE.InvertRXD` on google.com</a></footer>
            public byte InvertRXD;

            /// <summary>Inverts the sense of the RTS line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertRTS">`FT_XSERIES_EEPROM_STRUCTURE.InvertRTS` on google.com</a></footer>
            public byte InvertRTS;

            /// <summary>Inverts the sense of the CTS line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertCTS">`FT_XSERIES_EEPROM_STRUCTURE.InvertCTS` on google.com</a></footer>
            public byte InvertCTS;

            /// <summary>Inverts the sense of the DTR line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertDTR">`FT_XSERIES_EEPROM_STRUCTURE.InvertDTR` on google.com</a></footer>
            public byte InvertDTR;

            /// <summary>Inverts the sense of the DSR line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertDSR">`FT_XSERIES_EEPROM_STRUCTURE.InvertDSR` on google.com</a></footer>
            public byte InvertDSR;

            /// <summary>Inverts the sense of the DCD line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertDCD">`FT_XSERIES_EEPROM_STRUCTURE.InvertDCD` on google.com</a></footer>
            public byte InvertDCD;

            /// <summary>Inverts the sense of the RI line</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.InvertRI">`FT_XSERIES_EEPROM_STRUCTURE.InvertRI` on google.com</a></footer>
            public byte InvertRI;

            /// <summary>
            /// Determines whether the Battery Charge Detection option is enabled.
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.BCDEnable">`FT_XSERIES_EEPROM_STRUCTURE.BCDEnable` on google.com</a></footer>
            public byte BCDEnable;

            /// <summary>
            /// Asserts the power enable signal on CBUS when charging port detected.
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.BCDForceCbusPWREN">`FT_XSERIES_EEPROM_STRUCTURE.BCDForceCbusPWREN` on google.com</a></footer>
            public byte BCDForceCbusPWREN;

            /// <summary>Forces the device never to go into sleep mode.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.BCDDisableSleep">`FT_XSERIES_EEPROM_STRUCTURE.BCDDisableSleep` on google.com</a></footer>
            public byte BCDDisableSleep;

            /// <summary>I2C slave device address.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.I2CSlaveAddress">`FT_XSERIES_EEPROM_STRUCTURE.I2CSlaveAddress` on google.com</a></footer>
            public ushort I2CSlaveAddress;

            /// <summary>I2C device ID</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.I2CDeviceId">`FT_XSERIES_EEPROM_STRUCTURE.I2CDeviceId` on google.com</a></footer>
            public uint I2CDeviceId;

            /// <summary>Disable I2C Schmitt trigger.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.I2CDisableSchmitt">`FT_XSERIES_EEPROM_STRUCTURE.I2CDisableSchmitt` on google.com</a></footer>
            public byte I2CDisableSchmitt;

            /// <summary>
            /// FT1248 clock polarity - clock idle high (1) or clock idle low (0)
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.FT1248Cpol">`FT_XSERIES_EEPROM_STRUCTURE.FT1248Cpol` on google.com</a></footer>
            public byte FT1248Cpol;

            /// <summary>FT1248 data is LSB (1) or MSB (0)</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.FT1248Lsb">`FT_XSERIES_EEPROM_STRUCTURE.FT1248Lsb` on google.com</a></footer>
            public byte FT1248Lsb;

            /// <summary>FT1248 flow control enable.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.FT1248FlowControl">`FT_XSERIES_EEPROM_STRUCTURE.FT1248FlowControl` on google.com</a></footer>
            public byte FT1248FlowControl;

            /// <summary>Enable RS485 Echo Suppression</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.RS485EchoSuppress">`FT_XSERIES_EEPROM_STRUCTURE.RS485EchoSuppress` on google.com</a></footer>
            public byte RS485EchoSuppress;

            /// <summary>Enable Power Save mode.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.PowerSaveEnable">`FT_XSERIES_EEPROM_STRUCTURE.PowerSaveEnable` on google.com</a></footer>
            public byte PowerSaveEnable;

            /// <summary>Determines whether the VCP driver is loaded.</summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_XSERIES_EEPROM_STRUCTURE.IsVCP">`FT_XSERIES_EEPROM_STRUCTURE.IsVCP` on google.com</a></footer>
            public byte IsVCP;
        }

        /// <summary>Exceptions thrown by errors within the FTDI class.</summary>
        /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI.FT_EXCEPTION">`FTDI.FT_EXCEPTION` on google.com</a></footer>
        [Serializable]
        public class FT_EXCEPTION : Exception
        {
            /// <summary>
            ///
            /// </summary>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EXCEPTION">`FT_EXCEPTION` on google.com</a></footer>
            public FT_EXCEPTION()
            {
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="message"></param>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EXCEPTION">`FT_EXCEPTION` on google.com</a></footer>
            public FT_EXCEPTION(string message)
              : base(message)
            {
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="message"></param>
            /// <param name="inner"></param>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EXCEPTION">`FT_EXCEPTION` on google.com</a></footer>
            public FT_EXCEPTION(string message, Exception inner)
              : base(message, inner)
            {
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="info"></param>
            /// <param name="context"></param>
            /// <footer><a href="https://www.google.com/search?q=FTD2XX_NET.FTDI%2BFT_EXCEPTION">`FT_EXCEPTION` on google.com</a></footer>
            protected FT_EXCEPTION(SerializationInfo info, StreamingContext context)
              : base(info, context)
            {
            }
        }
    }
}
