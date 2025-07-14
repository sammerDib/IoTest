using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCCommon
{
    public class CxValueObject
    {
        private List<CxValueObject> valuesList = new List<CxValueObject>();
        private Dictionary<int, CxValueObject> dicoValuesList = new Dictionary<int, CxValueObject>();
        public CxValueObject() { }
        public void AppendList(int index, out int iHandle)
        {
            iHandle = -1;
            if (index > valuesList.Count) return;

            int indexUsed = valuesList.Count;
            valuesList.Add(new CxValueObject());
            dicoValuesList.Add(indexUsed, valuesList[indexUsed]);
            iHandle = indexUsed;
        }

        public void AppendValueAscii(int iHandle, string strName)
        {
            throw new NotImplementedException();
        }

        public void SetDataType(int v1, int v2, object a)
        {
            throw new NotImplementedException();
        }

        public void SetValueAscii(int v1, int v2, string v3)
        {
            throw new NotImplementedException();
        }

        public void SetValueF8(int v1, int v2, double v3)
        {
            throw new NotImplementedException();
        }

        public void SetValueU4(int v1, int v2, int v3)
        {
            throw new NotImplementedException();
        }


        public void AppendValueF4(int iHandleIdentification, float v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueF8(int iHandleIdentification, double v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueU1(int iHandleIdentification, byte v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueU2(int iHandleIdentification, short v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueU4(int iHandleIdentification, int v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueU8(int iHandleIdentification, int v)
        {
            throw new NotImplementedException();
        }

        public void AppendValueI4(int iValuesColHandle, int v)
        {
            throw new NotImplementedException();
        }
    }
}
