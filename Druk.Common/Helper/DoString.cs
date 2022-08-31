using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Druk.Common
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public static class DoString
    {
        #region //字符串或者数字补位
        /// <summary>
        /// 给字符串或者数字在前边或者后边补充指定的字符到指定的长度
        /// </summary>
        /// <param name="Target">原始值 可以是int 也可以是string</param>
        /// <param name="length">要补充到的长度</param>
        /// <param name="bu">要在前边补充的字符 默认0</param>
        /// <param name="isBefore">指示在字符串的前边或者后边补位  默认在前</param>
        /// <returns></returns>
        public static string Bu0(object Target, int length, char bu = '0', bool isBefore = true)
        {
            if (Target == null) return null;
            return isBefore ? Target.ToString().PadLeft(length, bu) : Target.ToString().PadRight(length, bu);
        }
        #endregion

        #region //驼峰命名

        /// <summary>
        /// 首字母小写
        /// </summary>
        public static string ToCamel(this string obj) { if (string.IsNullOrEmpty(obj)) return obj; return obj[0].ToString().ToLower() + obj.Substring(1); }
        /// <summary>
        /// 首字母大写
        /// </summary>
        public static string ToPascal(this string obj) { if (string.IsNullOrEmpty(obj)) return obj; return obj[0].ToString().ToUpper() + obj.Substring(1); }
        #endregion

        #region //获取GUID
        /// <summary>
        /// 获取GUID
        /// </summary>
        /// <param name="IsSymbol">是否删除GUID中间的-号</param>
        /// <returns>新的GUID</returns>
        public static string GUID(bool IsSymbol = false)
        {
            var result = Guid.NewGuid().ToString().ToLower();
            return IsSymbol ? result.Replace("-", "") : result;
        }
        #endregion

        #region //身份证号码转化成4307036****86X形式
        /// <summary>
        /// 扩展方法:身份证号码转化成4307036****86X形式
        /// </summary>
        /// <param name="idCardNumber">身份证号码</param>
        /// <returns></returns>
        public static string HideIdCardNumber(string idCardNumber)
        {
            if (!string.IsNullOrWhiteSpace(idCardNumber) && idCardNumber.Length == 18)
            {
                return idCardNumber.Substring(0, 5) + "******" + idCardNumber.Substring(15);
            }
            return string.Empty;
        }
        #endregion

        #region //证件号隐藏中间

        /// <summary>
        /// 扩展方法:身份证号码转化成4307036****86X形式
        /// </summary>
        /// <param name="idCardNumber">身份证号</param>
        /// <returns></returns>
        public static string HideIdCardNumbers(string idCardNumber)
        {

            if (!string.IsNullOrWhiteSpace(idCardNumber))
            {

                var str = "";
                for (int i = 0; i < (idCardNumber.Length - 4); i++)
                {
                    str += "*";
                }

                return str + idCardNumber.Substring(idCardNumber.Length - 4, 4);
            }
            return string.Empty;
        }
        #endregion

        #region //手机号隐藏中间四位
        /// <summary>
        /// 扩展方法:手机号转化成189****6547形式
        /// </summary>
        /// <param name="phoneNumber">手机号码</param>
        /// <returns></returns>
        public static string HideMobileNumber(string phoneNumber)
        {
            if (phoneNumber != null && phoneNumber.Length == 11)
            {
                return phoneNumber.Substring(0, 3) + "****" + phoneNumber.Substring(7, 4);
            }
            return string.Empty;
        }
        #endregion

        #region //判断是否手机号码
        /// <summary>
        /// 判断字符串是否是手机号码(大陆和香港)
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public static bool IsMobile(this string Phone)
        {
            if (string.IsNullOrEmpty(Phone)) { return false; }
            //大陆手机号
            var dalu = new Regex(@"^1\d{10}");
            if (Phone.Length == 11 && dalu.IsMatch(Phone)) return true;

            //新加坡
            var xjp = new Regex(@"^65\d{8}");
            if (Phone.Length <= 10 && xjp.IsMatch(Phone)) return true;

            return false;
        }
        #endregion
         
        #region //判断是否会员号
        /// <summary>
        /// 判断是否会员号(86和K86)
        /// </summary>
        /// <param name="memberNo"></param>
        /// <returns></returns>
        public static bool IsMemberNo(this string memberNo)
        {
            if (string.IsNullOrEmpty(memberNo)) { return false; }
            if (memberNo.Length != 15 && memberNo.Length != 16) { return false; }

            return new Regex(@"^(K99|k99|99*)00000\d{8}$").IsMatch(memberNo);
        }
        #endregion

        #region //科学计数法
        /// <summary>
        /// 扩展方法：将整形转成有逗号分隔的货币类型(100,000,000)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToThousand(int value)
        {
            var ts = value.ToString(CultureInfo.InvariantCulture);
            ts = ts.Reverse();
            var sb = new StringBuilder();
            for (var i = 0; i < ts.Length; i++)
            {
                sb.Append((i % 3 == 0 && i != 0) ? "," + ts.Substring(i, 1) : ts.Substring(i, 1));
            }
            return sb.ToString().Reverse();
        }
        #endregion

        #region //字符串逆转
        /// <summary>
        /// 扩展方法：将字符串逆转
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Reverse(this string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                char[] temp = source.ToCharArray();
                Array.Reverse(temp);
                return new string(temp);
            }
            return string.Empty;
        }
        #endregion

        #region //将字符串按长度切割
        /// <summary>
        /// 扩展方法:将字符串按长度切割
        /// </summary>
        /// <param name="source"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Cut(this string source, int? len = 10)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                var oldStrLen = source.Length;
                if (oldStrLen <= len.Value)
                {
                    return source;
                }
                else
                {
                    return source.Substring(0, len.Value - 3) + "...";
                }
            }
            return string.Empty;
        }
        #endregion

        #region //生成重复字符串
        /// <summary>
        /// 生成指定数量的重复字符串
        /// </summary>
        public static string GetStrByCount(string str, int nSpaces)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nSpaces; i++)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }
        #endregion


        #region //删除不可见字符
        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder(131);
            for (int i = 0; i < sourceString.Length; i++)
            {
                int Unicode = sourceString[i];
                if (Unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString());
                }
            }
            return sBuilder.ToString();
        }
        #endregion

        #region //获取某一字符串出现的次数
        /// <summary>
        /// 获取某一字符串在字符串中出现的次数
        /// </summary>
        public static int GetStringCount(string source, string target)
        {
            return (source.Length - source.Replace(target, "").Length) / target.Length;
        }
        #endregion

        #region //从指定字符开始截取字符串
        /// <summary>
        /// 截取从startString开始到原字符串结尾的所有字符   
        /// </summary>
        public static string GetSubString(string sourceString, string startString, bool IsCareUpper = false)
        {
            try
            {
                int index = IsCareUpper ? sourceString.ToUpper().IndexOf(startString.ToUpper()) : sourceString.IndexOf(startString);
                if (index > 0)
                {
                    return sourceString.Substring(index);
                }
                return sourceString;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region //用指定的开始和结束字符来截取字符串
        /// <summary>
        /// 用指定的开始和结束字符来截取字符串
        /// </summary>
        public static string GetSubString(string sourceString, string beginRemovedString, string endRemovedString)
        {
            try
            {
                if (!sourceString.Contains(beginRemovedString))
                    beginRemovedString = "";

                if (sourceString.LastIndexOf(endRemovedString, sourceString.Length - endRemovedString.Length) < 0)
                    endRemovedString = "";

                int startIndex = beginRemovedString.Length;
                int length = sourceString.Length - beginRemovedString.Length - endRemovedString.Length;
                if (length > 0)
                {
                    return sourceString.Substring(startIndex, length);
                }
                return sourceString;
            }
            catch
            {
                return sourceString;
            }
        }
        #endregion

        #region //按字节数取出字符串的长度 中文算2个
        /// <summary>
        /// 按字节数取出字符串的长度 中文算2个
        /// </summary>
        public static int GetByteCount(string strTmp)
        {
            int Length = 0;
            int i = 0;
            while (i < strTmp.Length)
            {
                Length += strTmp.Substring(i, 1).ToBytes().Length == 3 ? 2 : 1;
            }

            return Length;
        }
        #endregion

        #region //磁盘空间文字转换
        /// <summary>
        /// 磁盘空间文字转换
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="Unit">转换出的单位,默认是自动匹配,可以设定值为 PB TB GB MB KB b</param>
        /// <returns></returns>
        public static string GetDiskSizeStr(long Size, string Unit = "auto")
        {
            var PB = 1024D * 1024 * 1024 * 1024 * 1024;
            var TB = 1024D * 1024 * 1024 * 1024;
            var GB = 1024D * 1024 * 1024;
            var MB = 1024D * 1024;
            var KB = 1024D;
            switch (Unit.ToUpper())
            {
                case "P": case "PB": return Math.Round(Size / PB, 2) + " PB";
                case "T": case "TB": return Math.Round(Size / TB, 2) + " TB";
                case "G": case "GB": return Math.Round(Size / GB, 2) + " GB";
                case "M": case "MB": return Math.Round(Size / MB, 2) + " MB";
                case "K": case "KB": return Math.Round(Size / KB, 2) + " KB";
                case "B": case "BYTE": return Size + " b";
                default:
                    string result = string.Empty;
                    if (Size > PB) { return Math.Round(Size / PB, 2) + " PB"; }
                    if (Size > TB) { return Math.Round(Size / TB, 2) + " TB"; }
                    if (Size > GB) { return Math.Round(Size / GB, 2) + " GB"; }
                    if (Size > MB) { return Math.Round(Size / MB, 2) + " MB"; }
                    if (Size > KB) { return Math.Round(Size / KB, 2) + " KB"; }
                    return Size + " b";
            }
        }
        #endregion

        #region 按字节数要在字符串的位置
        /// <summary>
        /// 按字节数要在字符串的位置
        /// </summary>
        /// <param name="intIns">字符串的位置</param>
        /// <param name="strTmp">要计算的字符串</param>
        /// <returns>字节的位置</returns>
        public static int GetByteIndex(int intIns, string strTmp)
        {
            int intReIns = 0;
            if (strTmp.Trim() == "")
            {
                return intIns;
            }
            for (int i = 0; i < strTmp.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(strTmp.Substring(i, 1)) == 3)
                {
                    intReIns = intReIns + 2;
                }
                else
                {
                    intReIns = intReIns + 1;
                }
                if (intReIns >= intIns)
                {
                    intReIns = i + 1;
                    break;
                }
            }
            return intReIns;
        }
        #endregion

        #region 截取字符串
        /// <summary>
        /// 从右边截取字符串
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string CutRightString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return string.Empty;

            var input = Reverse(inputString);
            var output = CutString(input, len, "...");
            return Reverse(output);
        }


        /// <summary>
        /// 从包含中英文的字符串中截取固定长度的一段，inputString为传入字符串，len为截取长度（一个汉字占两个位）。
        /// </summary>
        public static string CutString(string inputString, int len, string end)
        {
            inputString = inputString.Trim();
            byte[] myByte = System.Text.Encoding.Default.GetBytes(inputString);
            if (myByte.Length > len)
            {
                string result = "";
                for (int i = 0; i < inputString.Length; i++)
                {
                    byte[] tempByte = System.Text.Encoding.Default.GetBytes(result);
                    if (tempByte.Length < len)
                    {
                        result += inputString.Substring(i, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                return result + end;
            }
            else
            {
                return inputString;
            }
        } 
        #endregion
         
        #region 删除换行 空格 隔位符
        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEnd(string str)
        {
            return TrimEnd(str, " ");
        }

        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimEnd(string str, string oper)
        {
            str = str.TrimEnd(new char[] { ' ', '\r', '\n', '\t' });
            return str.TrimEnd(Convert.ToChar(oper));
        }
        /// <summary>
        /// 删除字符开始的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimStart(string str)
        {
            return TrimStart(str, " ");
        }
        /// <summary>
        /// 删除字符开始的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimStart(string str, string oper)
        {
            str = str.TrimStart(new char[] { ' ', '\r', '\n', '\t' });
            return str.TrimStart(Convert.ToChar(oper));
        }
        /// <summary>
        /// 删除字符左右的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim(string str)
        {
            return Trim(str, " ");
        }
        /// <summary>
        /// 删除字符左右的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Trim(string str, string oper)
        {
            str = str.Trim(new char[] { ' ', '\r', '\n', '\t' });
            return str.Trim(Convert.ToChar(oper));
        }


        #endregion
         
        #region 从字符串的指定位置截取指定长度的子字符串
        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }


                if (startIndex > str.Length)
                {
                    return "";
                }


            }
            else
            {
                if (length < 0)
                {
                    return "";
                }
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string SubString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {


            string myResult = p_SrcString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (p_StartIndex >= p_SrcString.Length)
                {
                    return "";
                }
                else
                {
                    return p_SrcString.Substring(p_StartIndex,
                                                   ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }


            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }



                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                    {
                        nRealLength = p_Length + 1;
                    }

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }

        /// <summary>
        /// 自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string SourceString, string SearchString, string ReplaceString, bool IsCaseInsensetive)
        {
            return Regex.Replace(SourceString, Regex.Escape(SearchString), ReplaceString, IsCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        #endregion

        #region 将字符串转换为数组
        /// <summary>
        /// 使用逗号将字符串拆分成字符串数组
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <param name="fuhao"></param>
        /// <param name="isDelEmpty"></param>
        /// <returns>字符串数组</returns>
        public static string[] ToSplit(this string str, char fuhao = ',', bool isDelEmpty = true)
        {
            return str.Trim().Split(new char[] { fuhao }, isDelEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }
        #endregion

        #region 将数组转换为字符串
        /// <summary>
        /// 将数组转为包含有间隔符的字符串
        /// </summary>
        /// <param name="list">目标数组</param>
        /// <param name="speater">间隔符</param>
        /// <returns></returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }

            return sb.ToString();
        }
        #endregion

        #region 删除最后结尾的一个逗号
        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion

        #region 删除最后结尾的指定字符后的字符
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }
        #endregion

        #region 生成指定长度的字符串
        /// <summary>
        /// 生成指定长度的字符串,即生成strLong个str字符串
        /// </summary>
        /// <param name="strLong">生成的长度</param>
        /// <param name="str">以str生成字符串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }
            return ReturnStr;
        }
        #endregion

        #region 格式化复杂的字符串
        /// <summary>
        /// 格式化复杂的字符串
        /// </summary>
        /// <param name="indexStr">这个字符串之后的要格式化</param>
        /// <param name="str">要格式话的字符串</param>
        /// <param name="oldStr">原来的字符符号("or')</param>
        /// <param name="newStr">转换后的字符符号（"or'）</param>
        /// <returns></returns>
        public static string ForMttingString(string indexStr, string str, string oldStr, string newStr)
        {
            string ReturnStr = "";
            if (str.Contains(indexStr))
            {
                int index = str.IndexOf(indexStr);
                int indexLength = indexStr.Length;
                string subStr = str.Substring(index + indexLength, str.Length - index - indexLength);
                string starStr = str.Substring(0, index + indexLength);
                subStr = subStr.Replace(oldStr, newStr);
                int last = subStr.LastIndexOf(newStr);
                subStr = subStr.Substring(0, last) + oldStr + subStr.Substring(last + 1, subStr.Length - last - 1);
                return starStr + subStr;
            }
            return ReturnStr;
        }
        #endregion

        #region 生成日期随机码
        /// <summary>
        /// 生成时间随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamTimeCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
            #endregion
        }

        /// <summary>
        /// 生成日期随机码
        /// </summary>
        /// <returns></returns>
        public static string GetRamDateCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMdd");
            #endregion
        }
        #endregion

        #region 截取字符长度
        /// <summary>
        /// 截取字符长度
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (inputString == "")
            {
                return "";
            }
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }
            //如果截过则加上半个省略号 
            //byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            //if (mybyte.Length > len)
            //    tempString += "...";
            return tempString;
        }
        #endregion

        #region 清除HTML标记
        ///// <summary>
        ///// 清除自定字符串的html格式，并进行HMTL编码
        ///// </summary>
        ///// <param name="Htmlstring">目标字符串</param>
        ///// <returns>字符串的html编码</returns>
        //public static string DropHTML(string Htmlstring)
        //{
        //    //删除脚本  
        //    Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
        //    //删除HTML  
        //    Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
        //    Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
        //    Htmlstring = HttpContext.Current != null ? HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim() : Htmlstring;

        //    return Htmlstring;
        //}
        /// <summary>
        /// 清除自定字符串的html格式，并进行HMTL编码
        /// </summary>
        /// <param name="HTML">目标字符串</param>
        /// <returns>字符串的html编码</returns>
        public static string ClearHTMLTags1(string HTML)
        {
            string[] Regexs ={
                        @"<script[^>]*?>.*?</script>",
                        @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
                        @"([\r\n])[\s]+",
                        @"&(quot|#34);",
                        @"&(amp|#38);",
                        @"&(lt|#60);",
                        @"&(gt|#62);",
                        @"&(nbsp|#160);",
                        @"&(iexcl|#161);",
                        @"&(cent|#162);",
                        @"&(pound|#163);",
                        @"&(copy|#169);",
                        @"&#(\d+);",
                        @"-->",
                        @"<!--.*\n",
        };


            string[] Replaces ={
                            "",
                            "",
                            "",
                            "\"",
                            "&",
                            "<",
                            ">",
                            " ",
                            "\xa1", //chr(161),
                            "\xa2", //chr(162),
                            "\xa3", //chr(163),
                            "\xa9", //chr(169),
                            "",
                            "\r\n",
                            "",
                            ""
        };


            string Htmlstring = HTML;
            for (int i = 0; i < Regexs.Length; i++)
            {
                Htmlstring = new Regex(Regexs[i], RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(Htmlstring, Replaces[i]);
            }
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            return Htmlstring;
        }
        #endregion

        #region 清除HTML标记且返回相应的长度
        ///// <summary>
        ///// 清除目标字符串的HTML格式，并返回指定的长度
        ///// </summary>
        ///// <param name="Htmlstring">目标字符串</param>
        ///// <param name="strLen">指定的长度</param>
        ///// <returns>返回的字符串</returns>
        //public static string DropHTML(string Htmlstring, int strLen)
        //{
        //    return CutString(DropHTML(Htmlstring), strLen);
        //}
        #endregion

        #region TXT代码转换成HTML格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="Input">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把TXT代码转换成HTML格式
        public static String ToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            sb.Replace("“", "&ldquo;");
            sb.Replace("”", "&rdquo;");
            sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }
        #endregion

        #region HTML代码转换成TXT格式
        /// <summary>
        /// 字符串字符处理
        /// </summary>
        /// <param name="Input">等待处理的字符串</param>
        /// <returns>处理后的字符串</returns>
        /// //把HTML代码转换成TXT格式
        public static String ToTxt(String Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("&ldquo;", "“");
            sb.Replace("&rdquo;", "”");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        #endregion

        #region 检查危险字符
        /// <summary>
        /// 检查危险字符
        /// </summary>
        /// <param name="sInput"></param>
        /// <returns></returns>
        public static bool Filter(ref string sInput)
        {
            if (string.IsNullOrEmpty(sInput)) return true;
            string sInput1 = sInput.ToLower();
            string output = sInput;
            string pattern = @"*|and|=|or|exec|insert|select|delete|update|count|master|truncate|declare|char(|mid(|chr(|'";
            if (Regex.Match(sInput1, Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
            {
                return false;
            }
            else
            {
                output = output.Replace("'", "''");
                return true;
            }
        }
        #endregion

        #region 过滤特殊字符
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Htmls(string Input)
        {
            if (Input != string.Empty && Input != null)
            {
                string ihtml = Input.ToLower();
                ihtml = ihtml.Replace("<script", "&lt;script");
                ihtml = ihtml.Replace("script>", "script&gt;");
                ihtml = ihtml.Replace("<%", "&lt;%");
                ihtml = ihtml.Replace("%>", "%&gt;");
                ihtml = ihtml.Replace("<$", "&lt;$");
                ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region //判断是否OpenId
        /// <summary>
        /// 判断字符串是否是OpenID, 或者UnionID
        /// </summary>
        /// <param name="OpenID"></param>
        /// <returns></returns>
        public static bool IsOpenID(string OpenID)
        {
            var aa = new Regex(@"[a-zA-Z\d_-]{28}");
            if (string.IsNullOrEmpty(OpenID))
            {
                return false;
            }
            return aa.IsMatch(OpenID);
        }
        #endregion

        #region ///生成随机字符串

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        #endregion



        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="Expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object Expression, bool defValue)
        {
            if (Expression != null)
            {
                if (string.Compare(Expression.ToString(), "true", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(Expression.ToString(), "false", true) == 0)
                {
                    return false;
                }
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="Expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object Expression, int defValue)
        {

            if (Expression != null)
            {
                string str = Expression.ToString();
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return Convert.ToInt32(str);
                    }
                }
            }
            return defValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            if ((strValue == null) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue.ToString(), @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                {
                    intValue = Convert.ToSingle(strValue);
                }
            }
            return intValue;
        }
        /// <summary>
        /// sql字段太长分批查询
        /// </summary>
        /// <param name="ids">要查询的字段</param>
        /// <param name="count">传入数量</param>
        /// <param name="field">要查询字段</param>
        /// <returns></returns>
        public static string getSQLIn(List<int> ids, int count, string field)
        {
            if (count == 0)
                return field + " in ('')";
            count = Math.Min(count, 1000);
            int len = ids.Count();
            int size = len % count;
            if (size == 0)
            {
                size = len / count;
            }
            else
            {
                size = (len / count) + 1;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                int fromIndex = i * count;
                int toIndex = Math.Min(fromIndex + count, len);
                string yjdNbr = "";
                for (int j = fromIndex; j < toIndex; j++)
                {
                    yjdNbr += ids[j] + ",";
                }
                if (i != 0)
                {
                    builder.Append(" or ");
                }
                builder.Append(field).Append(" in (").Append(yjdNbr.TrimEnd(',')).Append(")");
            }
            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                if (builder.ToString().Contains("or"))
                {
                    return "(" + builder.ToString() + ")";
                }
                return builder.ToString();
            }
            else
            {
                return field + " in ('')";
            }
        }


        /// <summary>
        /// string型转换为decimal型
        /// </summary>
        /// <param name="expression">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(string expression, decimal defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            decimal intValue = defValue;
            if (expression != null)
            {
                bool IsDecimal = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsDecimal)
                    decimal.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// sql字段太长分批查询
        /// </summary>
        /// <param name="ids">要查询的字段</param>
        /// <param name="count">传入数量</param>
        /// <param name="field">要查询字段</param>
        /// <returns></returns>
        public static string getSQLIn(List<string> ids, int count, string field)
        {
            if (count == 0)
                return field + " in ('')";
            count = Math.Min(count, 1000);
            int len = ids.Count();
            int size = len % count;
            if (size == 0)
            {
                size = len / count;
            }
            else
            {
                size = (len / count) + 1;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                int fromIndex = i * count;
                int toIndex = Math.Min(fromIndex + count, len);
                string yjdNbr = "";
                for (int j = fromIndex; j < toIndex; j++)
                {
                    yjdNbr += ids[j] + ",";
                }
                if (i != 0)
                {
                    builder.Append(" or ");
                }
                builder.Append(field).Append(" in (").Append(yjdNbr.TrimEnd(',')).Append(")");
            }
            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                if (builder.ToString().Contains("or"))
                {
                    return "(" + builder.ToString() + ")";
                }
                return builder.ToString();
            }
            else
            {
                return field + " in ('')";
            }
        }

        /// <summary>
        /// 将集合内参数值的参数按照参数名ASCII码从小到大排序（字典序）
        /// </summary>
        /// <param name="paramsMap"></param>
        /// <returns></returns>
        public static String getParamSrc(Dictionary<string, string> paramsMap)
        {
            var vDic = (from objDic in paramsMap orderby objDic.Key ascending select objDic);
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                str.Append(pkey + "=" + pvalue + "&");
            }
            String result = str.ToString().Substring(0, str.ToString().Length - 1);
            return result;
        }

        /// <summary>
        /// 将集合内参数值的参数按照参数名ASCII码从小到大排序（字典序）
        /// </summary>
        /// <param name="paramsMap"></param>
        /// <returns></returns>
        public static String getParamASCII(Dictionary<string, string> paramsMap)
        {
            var vDic = (from objDic in paramsMap orderby objDic.Key ascending select objDic);
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in vDic)
            {
                string pkey = kv.Key;
                string pvalue = kv.Value;
                str.Append(pkey + pvalue);
            }
            String result = str.ToString();
            return result;
        }
    }
}
