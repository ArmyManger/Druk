using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Druk.Common
{
    public static class DoDateTime
    {

        #region //月份

        #region //月份第一天

        public static DateTime FirstDay_Of_Month(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1);
        }
        #endregion

        #region //月份最后一天

        public static DateTime LastDay_Of_Month(this DateTime time)
        {
            return time.AddMonths(1).FirstDay_Of_Month().AddDays(-1);
        }
        #endregion
        #endregion

        #region //季度

        #region //获取指定日期是当年的第几季度

        /// <summary>
        /// 获取指定日期是当年的第几季度
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int QuarterOfYear(this DateTime time)
        {
            return time.Month % 3 == 0 ? time.Month / 3 : time.Month / 3 + 1;
        }
        #endregion

        #region //季度第一天

        public static DateTime FirstDay_Of_Quarter(this DateTime time)
        {
            var quarter = time.QuarterOfYear();
            return new DateTime(time.Year, quarter * 3 - 2, 1);
        }
        #endregion

        #region //季度最后一天

        public static DateTime LastDay_Of_Quarter(this DateTime time)
        {
            return time.FirstDay_Of_Quarter().AddMonths(3).AddDays(-1);
        }
        #endregion 
        #endregion

        #region //星期

        #region //当年的第几周

        /// <summary>
        /// 获取指定日期是当年的第几周
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int WeekOfYear(this DateTime time)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
        #endregion

        #region //星期第一天

        public static DateTime FirstDay_Of_Week(this DateTime time)
        {
            //修改礼拜日值是7  默认为0
            //以礼拜一为第一天
            var weekNumber = time.DayOfWeek == DayOfWeek.Sunday ? 7 : time.DayOfWeek.GetHashCode();
            return time.AddDays(-(weekNumber - 1));
        }
        #endregion

        #region //星期最后一天

        public static DateTime LastDay_Of_Week(this DateTime time)
        {
            return time.FirstDay_Of_Week().AddDays(6);
        }
        #endregion

        #region //获取星期几
        /// <summary>
        /// 获取星期几(中文)
        /// </summary>
        public static string GetChinaWeek(DateTime now)
        {
            return new string[] { "星期天", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" }[(int)now.DayOfWeek];
        }
        #endregion

        #endregion

        #region //年份

        #region //年份第一天
        public static DateTime FirstDay_Of_Year(this DateTime time)
        {
            return new DateTime(time.Year, 1, 1);
        }
        #endregion

        #region //年份最后一天

        public static DateTime LastDay_Of_Year(this DateTime time)
        {
            return new DateTime(time.Year, 12, 31);
        }
        #endregion

        #endregion

        #region //格式化
        /// <summary>
        /// 格式化: yyyy-MM
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsSymbol">是否带符号,false时纯数字</param>
        /// <returns></returns>
        public static string Format_yyyyMM(this DateTime time, bool IsSymbol = true) { return time.ToString(IsSymbol ? "yyyy-MM" : "yyyyMM"); }
        /// <summary>
        /// 格式化: yyyy-MM-dd
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsSymbol">是否带符号,false时纯数字</param>
        /// <returns></returns>
        public static string Format_yyyyMMdd(this DateTime time, bool IsSymbol = true) { return time.ToString(IsSymbol ? "yyyy-MM-dd" : "yyyyMMdd"); }
        /// <summary>
        /// 格式化: yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsSymbol">是否带符号,false时纯数字</param>
        /// <returns></returns>
        public static string Format_yyyyMMddHHmmss(this DateTime time, bool IsSymbol = true) { return time.ToString(IsSymbol ? "yyyy-MM-dd HH:mm:ss" : "yyyyMMddHHmmss"); }
        /// <summary>
        /// 格式化: yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsSymbol">是否带符号,false时纯数字</param>
        /// <returns></returns>
        public static string Format_yyyyMMddHHmmssfff(this DateTime time, bool IsSymbol = true) { return time.ToString(IsSymbol ? "yyyy-MM-dd HH:mm:ss.fff" : "yyyyMMddHHmmssfff"); }

        /// <summary>
        /// 格式化: yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsSymbol">是否带符号,false时纯数字</param>
        /// <returns></returns>
        public static string Format_yyyyMMddHHmm(this DateTime time, bool IsSymbol = true) { return time.ToString(IsSymbol ? "yyyy-MM-dd HH:mm" : "yyyyMMddHHmm"); }

        #endregion

        #region //将没有间隔符的时间重置为时间对象
        /// <summary>
        /// 将没有间隔符的时间重置为时间对象
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateFromStr(string time)
        {
            int step = 2;
            var list = new List<string>() { "19", "00", "01", "01", "00", "00", "00" };
            for (int i = 0; i < time.Length / 2; i++)
            {
                list[i] = time.Substring(i * step, step);
            }
            string str = string.Format("{0}{1}-{2}-{3} {4}:{5}:{6}", list[0], list[1], list[2], list[3], list[4], list[5], list[6]);

            return str.ToDateTime();
        }
        #endregion

        #region //时间差文字转换

        public static string GetTimeSpanStr(this DateTime Time1, DateTime Time2)
        {
            return (Time1 - Time2).GetTimeSpanStr();
        }

        public static string GetTimeSpanStr(this TimeSpan span)
        {
            string result = string.Empty;
            if (Math.Abs(span.Days) > 0) result += $" {Math.Abs(span.Days)}天";
            if (Math.Abs(span.Hours) > 0) result += $" {Math.Abs(span.Hours)}小时";
            if (Math.Abs(span.Minutes) > 0) result += $" {Math.Abs(span.Minutes)}分钟";
            if (Math.Abs(span.Seconds) > 0) result += $" {Math.Abs(span.Seconds)}秒";
            if (Math.Abs(span.Milliseconds) > 0) result += $" {Math.Abs(span.Milliseconds)}毫秒";
            return result;
        }

        #endregion

        #region  //获取年龄
        /// <summary>
        ///  根据出生年月日获取年龄
        /// </summary>
        public static int GetAge(this DateTime dt)
        {
            return dt.Date <= DateTime.Now.Date ? (DateTime.Now.Date - dt.Date).Days / 365 : 0;
        }
        #endregion

        #region //获取农历
        // 十二天干
        static string[] tg = { "甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸" };
        // 十二地支
        static string[] dz = { "子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥" };
        // 十二生肖
        static string[] sx = { "鼠", "牛", "虎", "免", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪" };
        // 农历月
        static string[] months = { "正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二(腊)" };
        // 农历日
        private static string[] days1 = { "初", "十", "廿", "三" };
        // 农历日
        private static string[] days = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        #region //返回农历 年 月 日 生肖

        ///<summary>
        /// 根据DateTime 获取农历完整字符串  例:戊辰[龙]年闰三月初二
        ///</summary>
        public static string GetNongLi(this DateTime datetime, bool IsYear = true, bool IsMonth = true, bool IsDay = true)
        {
            //获取闰月， 0 则表示没有闰月
            int leapMonth = new ChineseLunisolarCalendar().GetLeapMonth(datetime.Year);

            bool isleap = datetime.Month == leapMonth;
            bool IsJianMonth = leapMonth > 0 && datetime.Month > leapMonth;

            return string.Concat(
                (IsYear ? GetNongLiYear(datetime) : string.Empty),
                (IsMonth ? (isleap ? "闰" : string.Empty) : string.Empty),
                (IsMonth ? GetNongLiMonth(datetime.AddMonths(-(IsJianMonth ? 1 : 0))) : string.Empty),
                (IsDay ? GetNongLiDay(datetime) : string.Empty)
            );
        }

        ///<summary>
        /// 返回农历年
        ///</summary>
        public static string GetNongLiYear(this DateTime time)
        {
            int tgIndex = (time.Year - 4) % 10;
            int dzIndex = (time.Year - 4) % 12;
            return string.Concat(tg[tgIndex], dz[dzIndex], "[", sx[dzIndex], "]") + "年";
        }
        ///<summary>
        /// 返回农历月
        ///</summary>
        public static string GetNongLiMonth(this DateTime time)
        {
            return months[time.Month - 1] + "月";
        }
        ///<summary>
        /// 返回农历日
        ///</summary>
        public static string GetNongLiDay(this DateTime time)
        {
            var day = time.Day;
            return day != 20 && day != 30
                ? string.Concat(days1[(day - 1) / 10], days[(day - 1) % 10])
                : string.Concat(days[(day - 1) / 10], days1[1]);
        }
        /// <summary>
        /// 获取年份生肖
        /// </summary>
        public static string GetNongLiShengXiao(this DateTime time)
        {
            return sx[(time.Year - 4) % 12];
        }
        #endregion


        #endregion

        #region //当天的开始 和结束

        /// <summary>
        /// 当天的开始 和结束
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsCustomCouponTime">是否精确到自定义的时分 true 是 false 否</param>
        /// <returns></returns>
        public static DateTime DayBegin(this DateTime time, bool IsCustomCouponTime = false)
        {
            if (IsCustomCouponTime)
            {
                return time.ToString("yyyy-MM-dd HH:mm:00").ToDateTime();
            }
            return time.ToString("yyyy-MM-dd 00:00:00").ToDateTime();
        }

        /// <summary>
        /// 当天的开始 和结束
        /// </summary>
        /// <param name="time"></param>
        /// <param name="IsCustomCouponTime">是否精确到自定义的时分 true 是 false 否</param>
        /// <returns></returns>
        public static DateTime DayEnd(this DateTime time, bool IsCustomCouponTime = false)
        {
            if (IsCustomCouponTime)
            {
                return time.ToString("yyyy-MM-dd HH:mm:00").ToDateTime();
            }
            return time.ToString("yyyy-MM-dd 23:59:59").ToDateTime();
        }
        #endregion

        #region 根据时间获取语义话述
        /// <summary>
        /// 根据时间获取语义话述 返回: 近一天 近一周
        /// </summary>
        public static string GetCloseTimeStr(this DateTime time)
        {
            int Realcha = (DateTime.Now - time).Days;
            var fixedStr = Realcha < 0 ? "未来" : "近";
            var cha = Math.Abs(Realcha);
            if (cha == 0) { return "当天"; }
            if (cha >= 0 && cha <= 1) { return fixedStr + "一天"; }
            if (cha > 1 && cha <= 2) { return fixedStr + "两天"; }
            if (cha > 2 && cha <= 3) { return fixedStr + "三天"; }
            if (cha > 3 && cha <= 7) { return fixedStr + "一周"; }
            if (cha > 7 && cha <= 14) { return fixedStr + "半个月"; }
            if (cha > 14 && cha <= 30) { return fixedStr + "一个月"; }
            if (cha > 30 && cha <= 60) { return fixedStr + "二个月"; }
            if (cha > 60 && cha <= 91) { return fixedStr + "三个月"; }
            if (cha > 90 && cha <= 182) { return fixedStr + "半年"; }
            if (cha > 182 && cha <= 365) { return fixedStr + "一年"; }
            return Realcha < 0 ? time.Format_yyyyMMdd() + "至今" : "到" + time.Format_yyyyMMdd();
        }
        #endregion

        #region //判断时间轴是否交叉
        /// <summary>
        /// 判断时间轴是否交叉
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="timeList"></param>
        /// <returns></returns>
        public static bool CheckTimeBetween(DateTime start, DateTime end, List<DateTime[]> timeList)
        {
            foreach (var t in timeList)
            {
                if (IsBetween2(start, t[0], t[1]) || IsBetween2(end, t[0], t[1]) || IsBetween2(t[0], start, end) || IsBetween2(t[1], start, end))
                {
                    return false;
                }
            }
            return true;
        }
        static bool IsBetween2(DateTime source, DateTime lower, DateTime upper)
        {
            return source > lower && source < upper;
        }
        #endregion

        #region // 获取当前时间戳

        /// <summary>
        /// 返回10位时间戳 Timestamp
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long Timestamp(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = dateTime.ToLocalTime();
            return (int)((dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }
        #endregion

        #region // 时间戳转为日期

        /// <summary>
        /// 时间戳转日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        #endregion


        #region //计算时间差

        /// <summary>
        /// 计算时间相差秒数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double DiffSeconds(DateTime startTime, DateTime endTime)
        {
            TimeSpan secondSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return secondSpan.TotalSeconds;

        }

        /// <summary>
        /// 计算时间相差分钟数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double DiffMinutes(DateTime startTime, DateTime endTime)
        {
            TimeSpan minuteSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return minuteSpan.TotalMinutes;
        }

        /// <summary>
        /// 计算时间相差小时数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double DiffHours(DateTime startTime, DateTime endTime)
        {
            TimeSpan hoursSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return hoursSpan.TotalHours;
        }

        /// <summary>
        /// 计算时间相差天数
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static double DiffDays(DateTime startTime, DateTime endTime)
        {
            TimeSpan daysSpan = new TimeSpan(endTime.Ticks - startTime.Ticks);
            return daysSpan.TotalDays;
        }
        #endregion

        /// <summary>
        /// 时间戳转换为时间
        /// </summary>
        /// <param name="dateTimeFromXml">时间戳</param>
        /// <returns></returns>
        public static DateTime UnixTimestampToDateTime(long dateTimeFromXml)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(dateTimeFromXml);
            return dt;
        }
    }
}
