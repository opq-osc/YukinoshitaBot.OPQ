// <copyright file="MainWorker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SocketIOClient;
    using YukinoshitaBot.Data;
    using YukinoshitaBot.Data.Content;
    using YukinoshitaBot.Data.Event;
    using YukinoshitaBot.Data.OpqApi;
    using YukinoshitaBot.Extensions;
    using YukinoshitaBot.Services;

    /// <summary>
    /// 工作线程
    /// </summary>
    public class MainWorker : BackgroundService
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private readonly OpqApi opqApi;
        private readonly IMessageHandler msgHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWorker"/> class.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="configuration">config</param>
        /// <param name="opqApi">opqApi</param>
        /// <param name="msgHandler">message handler</param>
        public MainWorker(ILogger<MainWorker> logger, IConfiguration configuration, OpqApi opqApi, IMessageHandler msgHandler)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.opqApi = opqApi;
            this.msgHandler = msgHandler;
        }

        /// <inheritdoc/>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var botConfig = this.configuration.GetSection("MeowBotSettings");
            var httpApi = botConfig.GetValue<string>("HttpApi");
            var loginQQ = botConfig.GetValue<long>("LoginQQ");
            var wsApi = botConfig.GetValue<string>("WebSocketApi");

            this.logger.LogInformation("Starting YukinoshitaBot...");
            this.logger.LogInformation("HttpApi: {httpApi}", httpApi);
            this.logger.LogInformation("LoginQQ: {loginQQ}", loginQQ);
            this.logger.LogInformation("WsApi: {wsApi}", wsApi);

            var client = new SocketIO(wsApi);
            client.OnConnected += (s, e) => 
            {
                this.logger.LogInformation("YukinoshitaBot is now connected.");
            };
            client.On("OnGroupMsgs", resp =>
            {
                var respData = resp.GetValue<SocketResponse<GroupMessage>>();

                // 过滤自身消息
                if (respData.CurrentPacket?.Data?.FromUserId == respData.CurrentQQ)
                {
                    return;
                }

                Message? msg = null;
                try 
                {
                    msg = Message.Parse(respData.CurrentPacket?.Data);
                    msg.OpqApi = this.opqApi;
                }
                catch (Exception e)
                {
                    this.logger.LogError("Message parse failed : {error}", e);
                    return;
                }

                switch (msg)
                {
                    case TextMessage textMsg:
                        this.msgHandler.OnGroupTextMsgRecieved(textMsg);
                        break;
                    case PictureMessage picMsg:
                        this.msgHandler.OnGroupPictureMsgRecieved(picMsg);
                        break;
                    default:
                        this.logger.LogWarning("Unresolved message object Type {type}", msg.GetType());
                        break;
                }
            });
            client.On("OnFriendMsgs", resp =>
            {
                var respData = resp.GetValue<SocketResponse<FriendMessage>>();

                // 过滤自身消息
                if (respData.CurrentPacket?.Data?.FromUin == respData.CurrentQQ)
                {
                    return;
                }

                Message? msg = null;
                try
                {
                    msg = Message.Parse(respData.CurrentPacket?.Data);
                    msg.OpqApi = this.opqApi;
                }
                catch (Exception e)
                {
                    this.logger.LogError("Message parse failed : {error}", e);
                    return;
                }

                switch (msg)
                {
                    case TextMessage textMsg:
                        this.msgHandler.OnFriendTextMsgRecieved(textMsg);
                        break;
                    case PictureMessage picMsg:
                        this.msgHandler.OnFriendPictureMsgRecieved(picMsg);
                        break;
                    default:
                        this.logger.LogWarning("Unresolved message object Type {type}", msg.GetType());
                        break;
                }
            });

            await client.ConnectAsync();
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
