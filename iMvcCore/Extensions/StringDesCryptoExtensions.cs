using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace iMvcCore.Extensions
{
    public static class StringDesCryptoExtensions
    {
        public static string Decrypt(this string input, string keys)
        {
            if(string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));
            if(string.IsNullOrEmpty(keys)) throw new ArgumentNullException(nameof(keys));
            if(keys.Length != 8) throw new ArgumentException("Invalid key format", nameof(keys));

            // 定义DES加密服务提供类
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            // 加密字符串转换为byte数组
            byte[] inputBytes = Convert.FromBase64String(input); // Encoding.UTF8.GetBytes(input);

            // 加密密匙转化为byte数组
            byte[] key = Encoding.UTF8.GetBytes(keys); //DES密钥(必须8字节)
            des.Key = key;
            des.IV = key;

            // 创建其支持存储区为内存的流
            using MemoryStream ms = new MemoryStream();

            // 定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputBytes, 0, inputBytes.Length);
            cs.FlushFinalBlock();

            byte[] bytes = ms.ToArray();
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Encrypt(this string input, string keys)
        {
            if(string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));
            if(string.IsNullOrEmpty(keys)) throw new ArgumentNullException(nameof(keys));
            if(keys.Length != 8) throw new ArgumentException("Invalid key format", nameof(keys));

            // 定义DES加密服务提供类
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            // 加密字符串转换为byte数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // 加密密匙转化为byte数组
            byte[] key = Encoding.UTF8.GetBytes(keys); //DES密钥(必须8字节)
            des.Key = key;
            des.IV = key;

            // 创建其支持存储区为内存的流
            using MemoryStream ms = new MemoryStream();

            // 定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputBytes, 0, inputBytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}