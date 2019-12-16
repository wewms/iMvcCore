using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace iMvcCore.Utils
{
    public static class X509
    {
        public const string CertFileName = "CertFileName";
        public const string CertFileKey = "CertFileKey";

        public static RSA GetRSAPrivateKey(string certFileName, string certFileKey)
        {
            var x509 = GetX509Certificate2(certFileName, certFileKey);
            return x509.GetRSAPrivateKey();
        }

        public static RSA GetRSAPublicKey(string certFileName, string certFileKey)
        {
            var x509 = GetX509Certificate2(certFileName, certFileKey);
            return x509.GetRSAPublicKey();
        }

        public static X509Certificate2 GetX509Certificate2(string certFileName, string certFileKey)
        {
            var cert = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certs", certFileName);
            if(!File.Exists(cert)) throw new FileNotFoundException($"{cert}");
            if(string.IsNullOrEmpty(certFileKey)) throw new ArgumentNullException($"{nameof(certFileKey)}");

            return new X509Certificate2(cert, certFileKey);
        }
    }
}