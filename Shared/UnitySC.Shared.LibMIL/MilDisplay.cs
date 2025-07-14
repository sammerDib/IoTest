using System;
using System.Text;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing a display of the MIL library
    //
    ///////////////////////////////////////////////////////////////////////
    public class MilDisplay : AMilId
    {
        //=================================================================
        // Properties
        //=================================================================
        public string Title
        {
            get
            {
                long size = 0;
                MIL.MdispInquire(MilId, MIL.M_TITLE_SIZE, ref size);
                StringBuilder sb = new StringBuilder((int)size);
                MIL.MdispInquire(MilId, MIL.M_TITLE, sb);
                return sb.ToString();
            }
            set { MIL.MdispControl(MilId, MIL.M_TITLE, value); }
        }

        public int WindowSizeX
        {
            get { return (int)MIL.MdispInquire(MilId, MIL.M_WINDOW_SIZE_X); }
        }

        public int WindowSizeY
        {
            get { return (int)MIL.MdispInquire(MilId, MIL.M_WINDOW_SIZE_Y); }
        }

        public double ZoomFactorX
        {
            get
            {
                double zoom = 0.0;
                MIL.MdispInquire(MilId, MIL.M_ZOOM_FACTOR_X, ref zoom);
                return zoom;
            }
            set
            {
                MIL.MdispControl(MilId, MIL.M_ZOOM_FACTOR_Y, value);
            }
        }

        public double ZoomFactorY
        {
            get
            {
                double zoom = 0.0;
                MIL.MdispInquire(MilId, MIL.M_ZOOM_FACTOR_Y, ref zoom);
                return zoom;
            }
            set
            {
                MIL.MdispControl(MilId, MIL.M_ZOOM_FACTOR_X, value);
            }
        }

        public double PanOffsetX
        {
            get
            {
                double offset = 0.0;
                MIL.MdispInquire(MilId, MIL.M_PAN_OFFSET_X, ref offset);
                return offset;
            }
        }

        public double PanOffsetY
        {
            get
            {
                double offset = 0.0;
                MIL.MdispInquire(MilId, MIL.M_PAN_OFFSET_Y, ref offset);
                return offset;
            }
        }

        public MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MdispInquire(_milId, MIL.M_OWNER_SYSTEM); }
        }

        public int TransparentColor
        {
            get { return (int)MIL.MdispInquire(_milId, MIL.M_TRANSPARENT_COLOR); }
            set { MIL.MdispControl(_milId, MIL.M_TRANSPARENT_COLOR, value); }
        }

        public MIL_ID OverlayId
        {
            get { return (MIL_ID)MIL.MdispInquire(_milId, MIL.M_OVERLAY_ID); }
        }

        public MilImage OverlayImage
        {
            get { return new MilImage(OverlayId, transferOnwership: false); }
        }

        public double OverlayShow
        {
            set { MIL.MdispControl(_milId, MIL.M_OVERLAY_SHOW, value); }
        }

        // The delegate is passed to non-managed code.
        // We store it so that it is not garbage-collected.
        private MIL_DISP_HOOK_FUNCTION_PTR _hookHandlerRoiChangePtr;

        private MIL_DISP_HOOK_FUNCTION_PTR _hookHandlerRoiChangeEndPtr;
        private MIL_DISP_HOOK_FUNCTION_PTR _hookHandlerFrameStartPtr;

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            if (_milId != MIL.M_NULL)
            {
                MIL.MdispFree(_milId);
                _milId = MIL.M_NULL;
            }

            base.Dispose(disposing);
        }

        //=================================================================
        // Allocate
        //=================================================================
        public void Alloc(MIL_ID systemId, int dispNum, string dispFormat, int initFlag)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing display");

            MIL.MdispAlloc(systemId, dispNum, dispFormat, initFlag, ref _milId);
            AMilId.checkMilError("Failed to allocate display");
        }

        //=================================================================
        //
        //=================================================================
        public void Select(MilImage image)
        {
            MIL.MdispSelect(_milId, image == null ? MIL.M_NULL : image.MilId);
            AMilId.checkMilError("Failed to select image in display");
        }

        public void SelectWindow(MilImage image, IntPtr clientWindowHandle)
        {
            MIL.MdispSelectWindow(_milId, image.MilId, clientWindowHandle);
            AMilId.checkMilError("Failed to select window in display");
        }

        //=================================================================
        //
        //=================================================================
        public void Pan(double xOffset, double yOffset)
        {
            MIL.MdispPan(_milId, xOffset, yOffset);
            AMilId.checkMilError("Failed to pan (scroll)");
        }

        //=================================================================
        //
        //=================================================================
        public void Zoom(double xFactor, double yFactor)
        {
            MIL.MdispZoom(_milId, xFactor, yFactor);
            AMilId.checkMilError("Failed to zoom");
        }

        //=================================================================
        //
        //=================================================================
        public void HookFunction(MIL_INT hookType, MIL_DISP_HOOK_FUNCTION_PTR hookHandlerPtr, IntPtr userDataPtr)
        {
            if (hookType == MIL.M_ROI_CHANGE)
                _hookHandlerRoiChangePtr = hookHandlerPtr;
            else if (hookType == MIL.M_ROI_CHANGE_END)
                _hookHandlerRoiChangeEndPtr = hookHandlerPtr;
            else if (hookType == MIL.M_FRAME_START)
                _hookHandlerFrameStartPtr = hookHandlerPtr;

            MIL.MdispHookFunction(MilId, hookType, hookHandlerPtr, userDataPtr);
            AMilId.checkMilError("Failed to hook the display");

            if (hookType == MIL.M_ROI_CHANGE + MIL.M_UNHOOK)
                _hookHandlerRoiChangePtr = null;
            else if (hookType == MIL.M_ROI_CHANGE_END + MIL.M_UNHOOK)
                _hookHandlerRoiChangeEndPtr = null;
            else if (hookType == MIL.M_FRAME_START + MIL.M_UNHOOK)
                _hookHandlerFrameStartPtr = null;
        }

        //=================================================================
        // A convenient function to UnHook
        //=================================================================
        public void UnHookFunction(MIL_INT hookType)
        {
            MIL_DISP_HOOK_FUNCTION_PTR HookHandlerPtr;
            if (hookType == MIL.M_ROI_CHANGE)
                HookHandlerPtr = _hookHandlerRoiChangePtr;
            else if (hookType == MIL.M_ROI_CHANGE_END)
                HookHandlerPtr = _hookHandlerRoiChangeEndPtr;
            else if (hookType == MIL.M_FRAME_START)
                HookHandlerPtr = _hookHandlerFrameStartPtr;
            else
                throw new System.ApplicationException("Internal Error: Invalid call to UnHook");

            if (HookHandlerPtr == null)
                return;

            hookType += MIL.M_UNHOOK;
            HookFunction(hookType, HookHandlerPtr, IntPtr.Zero);
        }
    }

    ///////////////////////////////////////////////////////////////////////
    //
    // A class representing an ROI of the MIL display module
    //
    ///////////////////////////////////////////////////////////////////////
    public class MilRoi
    {
        //=================================================================
        // Properties
        //=================================================================
        public MilDisplay ParentDisplay { get; private set; }

        public int BufferOffsetX
        {
            get { return (int)MIL.MdispInquire(ParentDisplay.MilId, MIL.M_ROI_BUFFER_OFFSET_X); }
            set { MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_BUFFER_OFFSET_X, value); }
        }

        public int BufferOffsetY
        {
            get { return (int)MIL.MdispInquire(ParentDisplay.MilId, MIL.M_ROI_BUFFER_OFFSET_Y); }
            set { MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_BUFFER_OFFSET_Y, value); }
        }

        public int BufferSizeX
        {
            get { return (int)MIL.MdispInquire(ParentDisplay.MilId, MIL.M_ROI_BUFFER_SIZE_X); }
            set { MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_BUFFER_SIZE_X, value); }
        }

        public int BufferSizeY
        {
            get { return (int)MIL.MdispInquire(ParentDisplay.MilId, MIL.M_ROI_BUFFER_SIZE_Y); }
            set { MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_BUFFER_SIZE_Y, value); }
        }

        public System.Drawing.Rectangle Rectangle
        {
            get
            {
                return new System.Drawing.Rectangle(
                    BufferOffsetX,
                    BufferOffsetY,
                    BufferSizeX,
                    BufferSizeY
                    );
            }
            set
            {
                //System.Diagnostics.Debug.WriteLine("ROI-rect " + value.X + " " + value.Y + " " + value.Width + " " + value.Height);
                BufferOffsetX = value.X;
                BufferOffsetY = value.Y;
                BufferSizeX = value.Width;
                BufferSizeY = value.Height;
            }
        }

        public double LineColor
        {
            set { MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_LINE_COLOR, value); }
        }

        public bool IsEmpty
        {
            get { return BufferSizeX <= 0 || BufferSizeY <= 0; }
        }

        public bool HasHandles { get; private set; }

        //=================================================================
        // Constructor
        //=================================================================
        public MilRoi(MilDisplay parentDisplay)
        {
            ParentDisplay = parentDisplay;
            HasHandles = false;
        }

        //=================================================================
        // Show/Hide
        //=================================================================
        public void Show(MIL_INT controlValue)
        {
            MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_SHOW, controlValue);
            AMilId.checkMilError("Failed to show ROI");
            HasHandles = false;
        }

        //=================================================================
        // Define
        //=================================================================
        public void Define(int controlValue)
        {
            HasHandles = ((controlValue & MIL.M_START) != 0);
            MIL.MdispControl(ParentDisplay.MilId, MIL.M_ROI_DEFINE, controlValue);
            AMilId.checkMilError("Failed to define ROI");
        }
    }
}