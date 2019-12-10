using System;

namespace iMvcCore.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        ///     银行家舍入法 4舍6入5取偶
        /// </summary>
        /// <param name="val"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static decimal RoundToFixed(this decimal val, int scale = 2)
        {
            var pow = (int) Math.Pow(10, scale);
            return Math.Round(val * pow) / pow;
        }
    }
}