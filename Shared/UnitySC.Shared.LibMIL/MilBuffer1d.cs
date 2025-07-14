using System;

using Matrox.MatroxImagingLibrary;

namespace UnitySC.Shared.LibMIL
{
    public class MilBuffer1d : AMilId
    {
        //=================================================================
        // Allocate
        //=================================================================
        public void Alloc1d(int sizeX, MIL_INT type, long attribute)
        {
            Alloc1d(Mil.Instance.HostSystem, sizeX, type, attribute);
        }

        public void Alloc1d(MIL_ID systemId, int sizeX, MIL_INT type, long attribute)
        {
            if (_milId != MIL.M_NULL)
                throw new ApplicationException("reusing image");

            _milId = MIL.MbufAlloc1d(systemId, sizeX, type, attribute, MIL.M_NULL);

            GC.AddMemoryPressure(SizeByte);
        }

        //=================================================================
        // Properties
        //=================================================================
        public int SizeX
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_X); }
        }

        public int Pitch
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_PITCH_BYTE); }
        }

        public int Type
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_TYPE); }
        }

        public int Attribute
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_ATTRIBUTE); }
        }

        public long SizeByte
        {
            get { return (long)MIL.MbufInquire(MilId, MIL.M_SIZE_BYTE); }
        }

        public int SizeBand
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_BAND); }
        }

        public int Depth
        {
            get { return (int)MIL.MbufInquire(MilId, MIL.M_SIZE_BIT); }
        }

        public MIL_ID OwnerSystem
        {
            get { return (MIL_ID)MIL.MbufInquire(_milId, MIL.M_OWNER_SYSTEM); }
        }

        //=================================================================
        // Get/Put value
        //=================================================================
        public double GetValue(int offX)
        {
            if ((Type & 0xFF) == 32)
            {
                float[] val = new float[1];
                Get1d(offX, 1, val);
                return val[0];
            }
            else
            {
                byte[] val = new byte[1];
                Get1d(offX, 1, val);
                return val[0];
            }
        }

        public void PutValue(int offX, byte value)
        {
            byte[] val = new byte[1];
            val[0] = value;
            Put1d(offX, 1, val);
        }

        public void PutValue(int offX, float value)
        {
            float[] val = new float[1];
            val[0] = value;
            Put1d(offX, 1, val);
        }

        public void Get1d(MIL_INT offX, MIL_INT sizeX, byte[] userArrayPtr)
        {
            MIL.MbufGet1d(_milId, offX, sizeX, userArrayPtr);
            checkMilError("MbufGet1d");
        }

        public void Get1d(MIL_INT offX, MIL_INT sizeX, float[] userArrayPtr)
        {
            MIL.MbufGet1d(_milId, offX, sizeX, userArrayPtr);
            checkMilError("MbufGet1d");
        }

        public void Get(byte[] userArrayPtr)
        {
            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(byte[,] userArrayPtr)
        {
            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(float[] userArrayPtr)
        {
            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void Get(float[,] userArrayPtr)
        {
            MIL.MbufGet(_milId, userArrayPtr);
            checkMilError("MbufGet");
        }

        public void GetColor(long dataFormat, MIL_INT band, byte[] userArrayPtr)
        {
            MIL.MbufGetColor(_milId, dataFormat, band, userArrayPtr);
            checkMilError("MbufGetColor");
        }

        public void Put1d(MIL_INT offX, MIL_INT sizeX, byte[] userArrayPtr)
        {
            MIL.MbufPut1d(_milId, offX, sizeX, userArrayPtr);
            checkMilError("MbufPut1d");
        }

        public void Put1d(MIL_INT offX, MIL_INT sizeX, float[] userArrayPtr)
        {
            MIL.MbufPut1d(_milId, offX, sizeX, userArrayPtr);
            checkMilError("MbufPut1d");
        }

        public void Put(byte[] userArrayPtr)
        {
            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        public void Put(int[] userArrayPtr)
        {
            MIL.MbufPut(_milId, userArrayPtr);
            checkMilError("MbufPut");
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            MIL.MbufFree(_milId);
            _milId = MIL.M_NULL;

            base.Dispose(disposing);
        }
    }
}