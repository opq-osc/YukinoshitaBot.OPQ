// <copyright file="CourseController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using YukinoshitaBot.Data.Attributes;
    using YukinoshitaBot.Data.Event;
    using YukinoshitaBot.Data.OpqApi;
    using YukinoshitaBot.Data.Whut;
    using YukinoshitaBot.Services;

    /// <summary>
    /// 测试控制器
    /// </summary>
    [YukinoshitaController(Mode = HandleMode.Break, Priority = 0, SessionType = SessionType.Person)]
    public class CourseController
    {
        private readonly ILogger logger;
        private readonly BksJwcSpider bksJwcSpider;
        private readonly BksJwcParser bksJwcParser;

        private List<Course> courses;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseController"/> class.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="bksJwcParser">parser</param>
        /// <param name="bksJwcSpider">spider</param>
        public CourseController(ILogger<RepeaterController> logger, BksJwcParser bksJwcParser, BksJwcSpider bksJwcSpider)
        {
            this.logger = logger;
            this.bksJwcSpider = bksJwcSpider;
            this.bksJwcParser = bksJwcParser;
            this.courses = new List<Course>();
        }

        /// <summary>
        /// 教务处登录测试
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [YukinoshitaHandler(Command = @"jwclogin\s(.*?)\s(.*?)", MatchMethod = CommandMatchMethod.Regex, Mode = HandleMode.Break, Priority = 2)]
        public async Task JwcLoginAsync(Message message)
        {
            if (message is TextMessage textMsg)
            {
                var regMatch = Regex.Match(textMsg.Content.TrimEnd(' ', '\r', '\n'), @"jwclogin\s(.*?)\s(.+)");
                var sno = regMatch.Groups[1].Value;
                var password = regMatch.Groups[2].Value;

                var success = await this.bksJwcSpider.LoginAsync(sno, password);

                message.Reply(success switch
                {
                    true => new TextMessageRequest("login success."),
                    false => new TextMessageRequest("password incorrect."),
                    null => new TextMessageRequest("network error.")
                });
            }
        }

        /// <summary>
        /// 课程爬取测试
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [YukinoshitaHandler(Command = "jwcgetcourse", MatchMethod = CommandMatchMethod.StartWith, Mode = HandleMode.Break, Priority = 3)]
        public async Task GetCourseAsync(Message message)
        {
            if (message is TextMessage)
            {
                if (!this.courses.Any())
                {
                    await this.bksJwcSpider.NavigateToCoursePageAsync();
                    var coursePage = await this.bksJwcSpider.GetCoursesAsync().ConfigureAwait(false);

                    if (coursePage is null)
                    {
                        message.Reply(new TextMessageRequest($"spider failed."));
                        return;
                    }

                    var courses = this.bksJwcParser.ParseCourses(coursePage);

                    this.courses.AddRange(courses);
                }

                var sb = new StringBuilder();
                foreach (var course in this.courses)
                {
                    sb.Append(course).Append(Environment.NewLine);
                }

                message.Reply(new TextMessageRequest(sb.ToString()));
            }
        }
    }
}
