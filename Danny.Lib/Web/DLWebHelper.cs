﻿using Danny.Lib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Danny.Lib.Web
{
    public class DLWebHelper
    {
        #region Identity
        private static Encoding utf8 = Encoding.UTF8;
        private static string accept = "gzip, deflate";
        private static bool keepalive = true;
        private static bool allowautoredirect = true;
        private static DecompressionMethods automaticdecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        private static string contenttype = "application/x-www-form-urlencoded;charset=utf8";
        private static string useragent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36";
        public DLWebHelper() { }
        #endregion

        /**
         * @ 创建请求的对象
         * @ url 远程服务器地址
         * @ method 提交类型
         * @ cookieContainer cookie 容器
         * */
        public HttpWebRequest Create(string url, ActionType method = ActionType.GET, CookieContainer cookieContainer = null)
        {
            return Create(url, method, accept, contenttype, useragent, automaticdecompression, allowautoredirect, keepalive, cookieContainer);
        }

        /**
         * @ 第2次方法重载
         * @ url 远程服务器地址
         * @ method 提交类型
         * @ acceptEncoding 标头编码
         * @ contentType 内容类型
         * @ userAgent 浏览器代理
         * @ automaticDecompression 数据压缩
         * @ allowAutoRedirect 是否运行重定向
         * @ keepAlive 是否保持连接
         * @ cookieContainer cookie 容器
         * */
        public HttpWebRequest Create(string url, ActionType method, string acceptEncoding, string contentType, string userAgent, DecompressionMethods automaticDecompression, bool allowAutoRedirect = true, bool keepAlive = true, CookieContainer cookieContainer = null)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Accept = acceptEncoding;
            request.AutomaticDecompression = automaticDecompression;
            request.Method = method.ToString();
            request.AllowAutoRedirect = allowautoredirect;
            request.KeepAlive = keepalive;
            request.ContentType = contentType;
            if (cookieContainer == null)
                cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.UserAgent = userAgent;

            return request;
        }

        /**
         * @ 写入 request 的数据
         * */
        public void UploadData(HttpWebRequest request, string paramStr)
        {
            if (string.IsNullOrEmpty(paramStr))
                return;

            byte[] postData = utf8.GetBytes(paramStr.Substring(0, paramStr.Length - 1));
            request.ContentLength = postData.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(postData, 0, postData.Length);
                stream.Flush();
            }
        }

        /**
         * @ 获取响应的内容
         * @ request 请求的对象
         * @ catchCookie 是否抓 cookie
         * @ catchHtml 是否下载整个网页
         * */
        public DLResponseData GetResponse(HttpWebRequest request, bool catchCookie = true, bool catchHtml = true)
        {
            DLResponseData rd = new DLResponseData();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                rd.Headers = new Dictionary<string, string>();
                foreach (var k in response.Headers.AllKeys)
                {
                    rd.Headers.Add(k, response.Headers[k]);
                }
                rd.Url = response.ResponseUri;
                rd.SetCookieString = response.Headers.Get("Set-Cookie");
                if (catchCookie)
                {
                    rd.Cookies = new CookieCollection();
                    for (int i = 0; i < response.Cookies.Count; i++)
                    {
                        rd.Cookies.Add(response.Cookies[i]);
                    }
                }
                if (catchHtml)
                    rd.Html = reader.ReadToEnd();
            }

            return rd;
        }

        /**
         * @ 正则匹配页面中的表单，并获取
         * @ lableName 表单标签名称
         * @ attrName 标签的属性名称
         * */
        public List<string> GetElementContent(string lableName, string attrName, string subElement)
        {
            StringBuilder regexStr = new StringBuilder("(?is)<");
            regexStr.Append(attrName).Append("[^>]*?").Append(subElement).Append(@"=(['""\s]?)([^'""\s]+)\1[^>]*?>");
            Regex regex = new Regex(regexStr.ToString());
            MatchCollection match = regex.Matches(lableName);
            List<string> values = new List<string>();
            foreach (Match m in match)
            {
                values.Add(m.Groups[2].Value);
            }
            return values;
        }

        /**
         * @ 获取表单的参数
         * @ specifyKey 指定的 key，除此之外都删除
         * */
        public Dictionary<string, string> GetFormParams(string data, string specifyKey = "")
        {
            List<string> valueList = GetElementContent(data, "input", "value");
            List<string> nameList = GetElementContent(data, "input", "name");
            Dictionary<string, string> formParams = new Dictionary<string, string>();
            for (int i = 0; i < nameList.Count; i++)
            {
                string key = nameList.ElementAtOrDefault(i);

                if (!string.IsNullOrEmpty(specifyKey) && !specifyKey.Contains(key))
                    continue;

                if (formParams.ContainsKey(key))
                    continue;
                formParams.Add(key, valueList.ElementAtOrDefault(i));
            }

            return formParams;
        }

        /**
         * @ 设置参数的值
         * */
        public void SetParams(string key, string value, Dictionary<string, string> dictionary)
        {
            for (int i = 0, len = dictionary.Keys.Count(); i < len; i++)
            {
                string k = dictionary.Keys.ElementAtOrDefault(i);
                if (key == k)
                {
                    dictionary[k] = value;
                }
            }
        }

        /**
         * @ 增加参数，如果参数已存在则修改其值
         * @ key 参数名称
         * @ value 参数值
         * @ dictionary 参数列表
         * */
        public void AddParams(string key, string value, Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }

        /**
         * @ url 参数编码
         * */
        public void UrlEncodeParams(Dictionary<string, string> formParams)
        {
            for (int i = 0, len = formParams.Keys.Count(); i < len; i++)
            {
                string key = formParams.Keys.ElementAtOrDefault(i);
                formParams[key] = HttpUtility.UrlEncode(formParams[key], Encoding.UTF8);
            }
        }

        /**
         * @ 将表单提交
         * */
        public DLResponseData PostForm(HttpWebRequest request, Dictionary<string, string> form, Dictionary<string, string> webHeaders)
        {
            DLResponseData rd = new DLResponseData();
            string postStr = string.Empty;
            foreach (string key in form.Keys)
            {
                postStr += key + "=" + form[key] + "&";
            }
            UploadData(request, postStr);
            // 下载数据
            rd = GetResponse(request);

            return rd;
        }
    }
}