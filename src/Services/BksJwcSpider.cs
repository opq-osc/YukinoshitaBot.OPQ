// <copyright file="BksJwcSpider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 本科生教务处爬虫
    /// </summary>
    public class BksJwcSpider
    {
        private readonly HttpClient httpClient;

        #region URL声明
        private const string UriGetFingerCode = "http://sso.jwc.whut.edu.cn/Certification/getCode.do";
        private const string UriLogin = "http://sso.jwc.whut.edu.cn/Certification/login.do";
        private const string UriToIndex = "http://sso.jwc.whut.edu.cn/Certification/toIndex.do";
        private const string UriEOTPage = "http://202.114.50.130/EOT";
        private const string UriScoreQuerryPage = "http://202.114.50.130/Score";
        private const string UriEOTQuerry = "http://202.114.50.130/EOT/pjkcList.do";
        private const string UriScoreQuerry = "http://202.114.50.130/Score/lscjList.do";
        private const string UriCourse = "http://218.197.102.183/Course";
        private const string UriCourseQuerry = "http://218.197.102.183/Course/grkbList.do";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BksJwcSpider"/> class.
        /// </summary>
        public BksJwcSpider()
        {
            this.httpClient = new HttpClient(new HttpClientHandler { UseCookies = true });
            this.httpClient.DefaultRequestHeaders.Add("User-Agent", GetRandomUserAgent());
            this.httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        }

        /// <summary>
        /// 登录教务处
        /// </summary>
        /// <param name="sno">学号</param>
        /// <param name="password">密码</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<bool?> LoginAsync(string sno, string password)
        {
            var request = await this.GetLoginRequestAsync(sno, password).ConfigureAwait(false);
            var resp = await this.httpClient.PostAsync(UriLogin, request).ConfigureAwait(false);
            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            return !content.Contains("本网站禁止爬虫采集或转载商业化");
        }

        /// <summary>
        /// 爬取教务处主页
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string?> GetIndexPageAsync()
        {
            var resp = await this.httpClient.GetAsync(UriToIndex).ConfigureAwait(false);
            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (content.Contains("本网站禁止爬虫采集或转载商业化"))
            {
                return null;
            }

            return content;
        }

        /// <summary>
        /// 导航到评教系统
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task NavigateToEotPageAsync()
        {
            await this.httpClient.GetAsync(UriEOTPage).ConfigureAwait(false);
        }

        /// <summary>
        /// 导航到选课系统
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task NavigateToCoursePageAsync()
        {
            await this.httpClient.GetAsync(UriCourse).ConfigureAwait(false);
        }

        /// <summary>
        /// 爬取选课系统个人课表
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string?> GetCoursesAsync()
        {
            var resp = await this.httpClient.GetAsync(UriCourseQuerry).ConfigureAwait(false);
            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (content.Contains("登录超时") || content.Contains("无法进行此操作"))
            {
                return null;
            }

            return content;
        }

        /// <summary>
        /// 爬取评教信息
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string?> GetEotInfoAsync()
        {
            var resp = await this.httpClient.GetAsync(UriEOTQuerry).ConfigureAwait(false);
            try
            {
                resp.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                return null;
            }

            var content = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (content.Contains("登录超时") || content.Contains("无法进行此操作"))
            {
                return null;
            }

            return content;
        }

        private static string GenerateFakeFinger()
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var finger = new string(Enumerable.Repeat(chars, 32).Select(s => s[random.Next(chars.Length)]).ToArray());
            return finger;
        }

        private static string GetRandomUserAgent()
        {
            var uaList = new List<string>
            {
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.11 (KHTML, like Gecko) Chrome/20.0.1132.11 TaoBrowser/2.0 Safari/536.11",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.71 Safari/537.1 LBBROWSER",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E; LBBROWSER)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; QQDownload 732; .NET4.0C; .NET4.0E; LBBROWSER)",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.84 Safari/535.11 LBBROWSER",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E; QQBrowser/7.0.3698.400)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; QQDownload 732; .NET4.0C; .NET4.0E)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SV1; QQDownload 732; .NET4.0C; .NET4.0E; 360SE)",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; QQDownload 732; .NET4.0C; .NET4.0E)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E)",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/21.0.1180.89 Safari/537.1",
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; QQDownload 732; .NET4.0C; .NET4.0E)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729;Media Center PC 6.0; .NET4.0C; .NET4.0E)",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.84 Safari/535.11 SE 2.X MetaSr 1.0",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; SV1; QQDownload 732; .NET4.0C; .NET4.0E; SE 2.X MetaSr 1.0)",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:16.0) Gecko/20121026 Firefox/16.0",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:2.0b13pre) Gecko/20110307 Firefox/4.0b13pre",
                "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:16.0) Gecko/20100101 Firefox/16.0",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.133 Safari/534.16",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)",
                "Mozilla/5.0 (X11; U; Linux x86_64; zh-CN; rv:1.9.2.10) Gecko/20100922 Ubuntu/10.10 (maverick) Firefox/3.6.10",
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.221 Safari/537.36 SE 2.X MetaSr 1.0"
            };
            var index = new Random().Next(0, uaList.Count);

            return uaList[index];
        }

        private static string Md5(string origin)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(origin));
            var strResult = BitConverter.ToString(result);
            string result3 = strResult.Replace("-", string.Empty);
            result3 = result3.ToLower();
            return result3;
        }

        private static string Sha1(string origin)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(origin));
            var shaStr = BitConverter.ToString(hash);
            shaStr = shaStr.Replace("-", string.Empty);
            shaStr = shaStr.ToLower();
            return shaStr;
        }

        private async Task<FormUrlEncodedContent> GetLoginRequestAsync(string sno, string password)
        {
            var form = new List<KeyValuePair<string?, string?>>();
            var random = new Random();
            var rnd = random.Next(10000, 99999);
            var finger = GenerateFakeFinger();
            var resp = await this.httpClient.GetAsync($"{UriGetFingerCode}?webfinger={finger}").ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            var code = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            form.Add(new ("MsgID", string.Empty));
            form.Add(new ("KeyID", string.Empty));
            form.Add(new ("UserName", string.Empty));
            form.Add(new ("Password", string.Empty));
            form.Add(new ("rnd", rnd.ToString()));
            form.Add(new ("return_EncData", string.Empty));
            form.Add(new ("code", code));
            form.Add(new ("userName1", Md5(sno)));
            form.Add(new ("password1", Sha1(sno + password)));
            form.Add(new ("webfinger", finger));
            form.Add(new ("type", "xs"));
            form.Add(new ("userName", sno));
            form.Add(new ("password", password));

            return new FormUrlEncodedContent(form);
        }
    }
}
