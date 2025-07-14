using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helper
{
    public static class AssertFileEx
    {
        static public string GetFileHash(string filename)
        {
            var hash = new SHA1Managed();
            var clearBytes = File.ReadAllBytes(filename);
            var hashedBytes = hash.ComputeHash(clearBytes);
            return ConvertBytesToHex(hashedBytes);
        }

        static public string ConvertBytesToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x"));
            }
            return sb.ToString();
        }

        public static void AreContentEqual(string sFileName1, string sFileName2)
        {
            try
            {
                var originalHash = GetFileHash(sFileName1);
                var copiedHash = GetFileHash(sFileName2);

                Assert.AreEqual(copiedHash, originalHash);
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception raised {0} vs {1} : {2}", sFileName1, sFileName2, ex.Message);
            }
        }
    }
}