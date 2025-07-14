using System;

namespace UnitySC.Shared.Tools
{
    public static class Md5
    {
        public static string ComputeHash(byte[] data)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
