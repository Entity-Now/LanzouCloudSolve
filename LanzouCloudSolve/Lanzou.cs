using LanzouCloudSolve.Model;
using LanzouCloudSolve.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static LanzouCloudSolve.Utility.HttpHandle;
using static LanzouCloudSolve.Utility.utils;

namespace LanzouCloudSolve
{
    public static class Lanzou
    {
        /// <summary>
        /// 获取蓝奏云链接的下载直链
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<LResult > GetLink(string url)
        {
            // 获取蓝奏云的host
            var Host = new Regex(@"http.?://.+?(?=/)").Match(url);
            if (Host.Success)
            {
                // 获取iframe中的地址
                var http1 = await GetAsync(url);
                // 获取包含下载链接的地址
                string referer = Host + getIframeLink(http1);
                var http2 = await GetAsync(referer);
                // 获取随机sign值
                var randomStr = new Regex(@"(?=\?file=).*(?=')").Match(http2);
                if (!randomStr.Success)
                {
                    throw new Exception("获取随机参数失败~");
                }
                // 获取下载链接
                var http3 = await PostAsync(Host + "/ajaxm.php" + randomStr.Value, getSigns(http2), customHandle: (httpClient) =>
                {
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Referer", referer);
                });
                var result = StringToJson<LResult>(http3);

                return result;
            }
            throw new Exception("非法链接，请检查后重试！");
        }
        /// <summary>
        /// 用于下载任意网络文件
        /// </summary>
        /// <returns></returns>
        public static async Task DownFile(string url, string savePath, IProgress<double> progress, CancellationToken token)
        {
            var http = new HttpClient();
            AddHead(http, true);

            await DownloadFileAsync(http, url, savePath, progress, token);
        }

        static string getIframeLink(string source)
        {
            var regex = new Regex(@"(?<=<iframe class=""ifr2"" name=""([0-9]{0,11})"" src="")(.*)(?="" frameborder=""0"" scrolling=""no""></iframe>)");
            var findValue = regex.Matches(source);
            if (findValue.Count <= 0)
            {
                throw new Exception("未在结果中找到iframe，可能是请求失败~");
            }
            var isOk = findValue[findValue.Count - 1];
            if (!isOk.Success)
            {
                throw new Exception("匹配iframe时出错，请重新尝试~");
            }
            return isOk.Value;
        }
        static string getSigns(string source)
        {
            var wsk_sign = new Regex(@"(?<=var wsk_sign = ').*(?=';)").Match(source);
            var aihidcms = new Regex(@"(?<=var aihidcms = ').*(?=';)").Match(source);
            var ciucjdsdc = new Regex(@"(?<=var ciucjdsdc = ').*(?=';)").Match(source);
            var ws_sign = new Regex(@"(?<=var ws_sign = ').*(?=';)").Match(source);
            var s_sign = new Regex(@"(?<=sign':')(.+?)(?=')").Match(source);
            var ajaxdata = new Regex(@"(?<=var ajaxdata = ').*(?=';)").Match(source);
            if (!wsk_sign.Success || !ws_sign.Success || !s_sign.Success || !ajaxdata.Success)
            {
                throw new Exception("解析sign请求体的时候出错！");
            }
            return $"action=downprocess&signs={ajaxdata.Value}&sign={s_sign.Value}&websign={ciucjdsdc.Value}&websignkey={aihidcms.Value}&ves=1";
        }
    }
}
