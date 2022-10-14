using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 验证码
    /// </summary>
    public static class DoCheckCode
    {
        static string[] Nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        static string[] LetLower = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        static string[] LetUpper = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        #region //生成验证码
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="Length">验证码长度</param>
        /// <param name="IsNumber">是否使用数字</param>
        /// <param name="IsCantainLetter">是否使用小写字母</param>
        /// <param name="IsCantainLetterUpper">是否使用大写字母</param>
        /// <returns></returns>
        public static string Create(int Length, bool IsNumber = true, bool IsCantainLetter = false, bool IsCantainLetterUpper = false)
        {
            List<string> tempObj = new List<string>();
            if (IsNumber) { tempObj.AddRange(Nums.ToList()); }
            if (IsCantainLetter) { tempObj.AddRange(LetLower.ToList()); }
            if (IsCantainLetterUpper) { tempObj.AddRange(LetUpper.ToList()); }

            string resultStr = "";
            Random Rdm = new Random();
            var i = 0;
            while (i < Length) { resultStr += tempObj[Rdm.Next(tempObj.Count)]; i++; }
            return resultStr;
        }
        #endregion
    }
}
