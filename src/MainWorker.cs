﻿// <copyright file="MainWorker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SocketIOClient;
    using YukinoshitaBot.Data;

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
            });
            client.On("OnFriendMsgs", resp =>
            {
                var respData = resp.GetValue<SocketResponse<FriendMessage>>();
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