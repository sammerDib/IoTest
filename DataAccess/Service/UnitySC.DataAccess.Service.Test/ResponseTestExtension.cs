using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Test
{
    public static class ResponseTestExtension
    {
        public static T GetResultWithTest<T>(this Response<T> response)
        {
            var exception = response.Exception;
            if (exception != null)
            {
                Assert.Fail("Bad response" + exception.Message + exception.InnerException);
            }

            return response.Result;
        }
    }
}
