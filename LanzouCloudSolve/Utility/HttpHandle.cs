using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static LanzouCloudSolve.Utility.utils;
using LanzouCloudSolve.Model;
using System.Net;
using System.Linq;
using System.Threading;

namespace LanzouCloudSolve.Utility
{
    public static class HttpHandle
    {

        public static void AddHead(HttpClient client, bool downCookie = false)
        {
            Dictionary<string, string> head = new Dictionary<string, string>()
            {
                ["Connection"] = "keep-alive",
                ["Upgrade-Insecure-Requests"] = "1",
                ["Cache-Control"] = "max-age=0",
                ["Sec-Fetch-Site"] = "none",
                ["Sec-Fetch-Mode"] = "navigate",
                ["Sec-Fetch-Dest"] = "document",
                ["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*",
                ["Accept-Encoding"] = "gzip, deflate, br, zstd",
                ["Accept-Language"] = "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6",
                ["Cookie"] = downCookie ? Global.LanZouDownCookie : Global.LanZouCookie,
                ["User-Agent"] = GetRandomElement(Global.UserAgents)
            };
            foreach (var item in head)
            {
                client.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
        }
        public static async Task<string> GZIP_Body(HttpResponseMessage response)
        {
            var read = await response.Content.ReadAsStreamAsync();
            StreamReader streamReader = new StreamReader(new GZipStream(read, CompressionMode.Decompress), Encoding.UTF8);

            return await streamReader.ReadToEndAsync();
        }
        /// <summary>
        /// 发送get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            AddHead(httpClient);
            var reuslt = await httpClient.GetAsync(url);
            return await GZIP_Body(reuslt);
        }
        /// <summary>
        /// 发送post请求
        /// </summary>
        /// <param name="url">请求链接</param>
        /// <param name="Body">请求体</param>
        /// <param name="AllowAutoRedirect">是否自动重定向</param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url,
            string Body,
            bool AllowAutoRedirect = false, 
            string ContentType = "application/x-www-form-urlencoded",
            Action<HttpClient> customHandle = null)
        {
            HttpClientHandler Handler = new HttpClientHandler()
            {
                AllowAutoRedirect = AllowAutoRedirect
            };
            HttpClient httpClient = new HttpClient(Handler);
            AddHead(httpClient);
            if (customHandle != null)
            {
                customHandle(httpClient);
            }
            
            var result = await httpClient.PostAsync(url, new StringContent(Body, Encoding.UTF8, ContentType));

            return await result.Content.ReadAsStringAsync();
        }

        public static async Task DownloadFileAsync(HttpClient client, string url, string fileName, IProgress<double> progress, CancellationToken token)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentNullException("fileName");
                }

                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("The request returned with HTTP status code {0}", response.StatusCode));
                }

                var total = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                var canReportProgress = total != -1 && progress != null;

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var file = File.Create(fileName))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[1024 * 1024];
                        var isMoreToRead = true;
                        var progressPercentage = 0;

                        do
                        {
                            token.ThrowIfCancellationRequested();

                            var read = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                var data = new byte[read];
                                buffer.ToList().CopyTo(0, data, 0, read);

                                // TODO: put here the code to write the file to disk
                                file.Write(data, 0, read);

                                totalRead += read;

                                if (canReportProgress)
                                {
                                    var percent = Convert.ToInt32((totalRead * 1d) / (total * 1d) * 100);
                                    if (progressPercentage != percent && percent % 10 == 0)
                                    {
                                        progressPercentage = percent;
                                        progress.Report(percent);
                                    }
                                }
                            }
                        } while (isMoreToRead);
                        file.Close();
                        if (canReportProgress)
                        {
                            progress.Report(101);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
