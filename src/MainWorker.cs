// <copyright file="MainWorker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot
{
    using System.Threading;
    using System.Threading.Tasks;
    using MeowIOTBot;
    using MeowIOTBot.NetworkHelper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 工作线程
    /// </summary>
    public class MainWorker : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWorker"/> class.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="configuration">config</param>
        public MainWorker(ILogger<MainWorker> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var botConfig = this.configuration.GetSection("MeowBotSettings");
            var httpApi = botConfig.GetValue<string>("HttpApi");
            var loginQQ = botConfig.GetValue<string>("LoginQQ");
            var wsApi = botConfig.GetValue<string>("WebSocketApi");

            this.logger.LogInformation("Starting MeowBot...");
            this.logger.LogInformation("HttpApi: {httpApi}", httpApi);
            this.logger.LogInformation("LoginQQ: {loginQQ}", loginQQ);
            this.logger.LogInformation("WsApi: {wsApi}", wsApi);

            PostHelper.CallerUrl = httpApi;
            PostHelper.LoginQQ = loginQQ;
            using var recv = await new MeowIOTClient(wsApi, LogType.Verbose).Connect().ConfigureAwait(false);

            this.logger.LogInformation("MeowIOTBot is now connected.");

            recv._FriendTextMsgRecieve += this.Recv_FriendTextMsgRecieve;
            recv._GroupTextMsgRecieve += this.Recv_GroupTextMsgRecieve;
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private void Recv_FriendTextMsgRecieve(MeowIOTBot.QQ.QQMessage.QQRecieveMessage.QQRecieveMessage sender, MeowIOTBot.QQ.QQMessage.QQRecieveMessage.TextMsg e)
        {
            var fromQQ = sender.IOBody.MsgFromQQ;
            var msg = e.Content;

            if (fromQQ != sender.CurrentQQ)
            {
                // 好友消息处理
            }
        }

        private void Recv_GroupTextMsgRecieve(MeowIOTBot.QQ.QQMessage.QQRecieveMessage.QQRecieveMessage sender, MeowIOTBot.QQ.QQMessage.QQRecieveMessage.TextMsg e)
        {
            var fromGroup = sender.IOBody.FromGroupId;
            var fromQQ = sender.IOBody.MsgFromQQ;
            var msg = e.Content;

            if (fromQQ != sender.CurrentQQ)
            {
                // 群消息处理
            }
        }
    }
}
