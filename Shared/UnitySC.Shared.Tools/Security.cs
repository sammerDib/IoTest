using System;
using System.Security.Cryptography;

namespace UnitySC.Shared.Tools
{
    public class Security
    {
        /// <summary>
        /// Calculates the SHA256 hash of a string and returns the hash string in hexadecimal format
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ComputeHash(string input)
        {
            byte[] originalBytes;
            byte[] encodedBytes;
            string result = "";
            SHA256 Sha256;

            if (input != null)
            {
                if (input.Length > 0)
                {
                    Sha256 = new SHA256CryptoServiceProvider();
                    originalBytes = System.Text.ASCIIEncoding.Default.GetBytes(input);
                    encodedBytes = Sha256.ComputeHash(originalBytes);
                    result = BitConverter.ToString(encodedBytes);
                }
            }
            return result;
        }
    }
}