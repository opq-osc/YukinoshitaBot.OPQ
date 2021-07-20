// <copyright file="RepeaterController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using YukinoshitaBot.Data.Attributes;
    using YukinoshitaBot.Data.Event;
    using YukinoshitaBot.Data.OpqApi;

    /// <summary>
    /// 测试控制器
    /// </summary>
    [YukinoshitaController(Mode = HandleMode.Break, Priority = 1)]
    public class RepeaterController
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeaterController"/> class.
        /// </summary>
        /// <param name="logger">logger</param>
        public RepeaterController(ILogger<RepeaterController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 文本消息复读
        /// </summary>
        /// <param name="message">消息</param>
        [YukinoshitaHandler(Command = "read ", MatchMethod = CommandMatchMethod.StartWith, Mode = HandleMode.Break, Priority = 2)]
        public void TextRepeater(Message message)
        {
            if (message is TextMessage textMsg)
            {
                this.logger.LogInformation("from TextRepeater: {message}", textMsg.Content);
                message.Reply(new TextMessageRequest(textMsg.Content[5..]));
            }
        }

        /// <summary>
        /// 图片消息复读
        /// </summary>
        /// <param name="message">消息</param>
        [YukinoshitaHandler(Command = "read", MatchMethod = CommandMatchMethod.StartWith, Mode = HandleMode.Pass, Priority = 1)]
        public void PictureRepeater(Message message)
        {
            if (message is PictureMessage picMsg)
            {
                this.logger.LogInformation("from PictureRepeater: {url}", picMsg.FirstPicture);
                message.Reply(new PictureMessageRequest(new Uri(picMsg.FirstPicture)));
            }
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="message">消息</param>
        [YukinoshitaHandler(Command = "老婆", MatchMethod = CommandMatchMethod.StartWith, Mode = HandleMode.Pass, Priority = 3)]
        public void Hello(Message message)
        {
            if (message is TextMessage textMessage)
            {
                message.Reply(new TextMessageRequest("真是不知廉耻的家伙"));
            }
        }
    }
}
