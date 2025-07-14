using System;
using System.IO;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Data
{
    [Serializable]
    public class DefineError
    {
        public ErrorID ErrorID { get; set; }
        public int TestIndex { get; set; }
        public DefineError() { }
    }

    public static class TestErrors
    {
        private static DefineError GetError()
        {
            DefineError error = new DefineError();
            try
            {
                error = XML.Deserialize<DefineError>("C:\\Temp\\DefineError.xml");
            }
            catch
            {
                ResetError();
            }
            return error;
        }

        public static void CheckTestError(ErrorID errId)
        {
            CheckTestError(errId, 0);
        }
        public static void CheckTestError(ErrorID errId, int index)
        {
#if DEBUG
            DefineError found = TestErrors.GetError();
            if ((found.ErrorID == errId) && (found.TestIndex == index))
                throw new Exception("CrashTest");
#endif
        }

        public static void ResetError()
        {
#if DEBUG
            DefineError error = new DefineError();
            error.ErrorID = ErrorID.Undefined;
            error.TestIndex = 0;
            if (!Directory.Exists("C:\\Temp)"))
                Directory.CreateDirectory("C:\\Temp");
            XML.Serialize(error, "C:\\Temp\\DefineError.xml");
#endif
        }
    }


}
