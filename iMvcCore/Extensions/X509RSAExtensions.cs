using System;
using System.Security.Cryptography;
using System.Text;

namespace iMvcCore.Extensions
{
    public static class X509RSAExtensions
    {
        public static string Decrypt(this RSA rsa, string input)
        {
            if(string.IsNullOrEmpty(input)) return string.Empty;

            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(rsa.Decrypt(bytes, RSAEncryptionPadding.OaepSHA256));
        }
    }
}