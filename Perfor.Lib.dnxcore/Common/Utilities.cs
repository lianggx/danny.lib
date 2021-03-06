﻿using System;
using System.Text.RegularExpressions;

namespace Perfor.Lib.Common
{
    /// <summary>
    ///  公共功能处理类
    /// </summary>
    public partial class Utilities
    {
        #region Identity
        // 本地时区 1970.1.1格林威治时间
        public static DateTime Greenwich_Mean_Time = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        /**冒号**/
        public const String JSON_COLON = ":";
        /**双引号**/
        public const String JSON_QUOTES = "\"";
        /**大括号-左**/
        public const String JSON_BRACES_LEFT = "{";
        /**大括号-右**/
        public const String JSON_BRACES_RIGHT = "}";
        /**中括号-左**/
        public const String JSON_BRACKET_LEFT = "[";
        /**中括号-右**/
        public const String JSON_BRACKET_RIGHT = "]";
        #endregion

        /// <summary>
        ///  将时间转换为yyyy-MM-dd 00:00:00且dd为当月最后一天的格式
        /// </summary>
        /// <param name="date">传入的时间</param>
        /// <returns></returns>
        public static string GetLastMonth(DateTime date)
        {
            DateTime dt1 = new DateTime(date.Year, date.Month, 1);
            return string.Format("{0}-{1} 00:00:00", date.ToString("yyyy-MM"), dt1.AddMonths(1).AddDays(-1).Day);
        }

        /// <summary>
        ///  获取一个新的 GUID ，并且不包含 "-" 符号的字符串表现形式
        /// </summary>
        /// <returns></returns>
        public static string GetGuidNorString()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        ///  过滤 SQL 注入的字符
        /// </summary>
        /// <param name="sqlCmdText"></param>
        /// <returns></returns>
        public static string DetectSQLInjection(string sqlCmdText)
        {
            Regex regex = new Regex("'");
            sqlCmdText = regex.Replace(sqlCmdText, "''");
            regex = new Regex("-");
            sqlCmdText = regex.Replace(sqlCmdText, "");
            return sqlCmdText;
        }

        /// <summary>
        ///  根据长度判断是否需要写入逗号
        /// </summary>
        /// <param name="len">循环长度</param>
        /// <param name="index">当前下标</param>
        /// <param name="incremental">增量</param>
        /// <param name="chars">输入的分隔符号</param>
        /// <returns></returns>
        public static string IsWriterComma(int len, int index, int incremental, string chars = ",")
        {
            string result = (index + incremental == len) ? "" : chars;

            return result;
        }

        #region Function IsValidEmail/Phone/IdCard
        /// <summary>
        ///  验证字符串是否邮箱地址
        /// </summary>
        /// <param name="value">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsValidEmail(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        ///  验证字符串是否大陆身份证号码
        /// </summary>
        /// <param name="value">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsValidIdCard(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value, @"^\d{17}([0-9]|X)$");
        }

        /// <summary>
        ///  验证字符串是否手机号码
        /// </summary>
        /// <param name="value">待验证的字符串</param>
        /// <returns></returns>
        public static bool IsValidPhone(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return Regex.IsMatch(value, "1[3|5|7|8|][0-9]{9}");
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlSubParser(string url)
        {
            Uri uri = new Uri(url);
            object[] args = new object[] { uri.Scheme, uri.Host, (uri.Port == 80) ? "" : (":" + uri.Port.ToString()), uri.AbsolutePath };
            return string.Format("{0}://{1}{2}{3}", args);
        }
    }
}
