using System;
using System.Text;

namespace iMvcCore.Utils
{
    public static class Captcha
    {
        public static string GenerateCode(int length)
        {
            char[] s = {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' //,
                //'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                //'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };

            Random r = new Random();
            StringBuilder builder = new StringBuilder();

            for(int i = 0; i < length; i++)
            {
                builder.Append(s[r.Next(0, s.Length - 1)]);
            }

            return builder.ToString();
        }
    }
}