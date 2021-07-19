// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using YukinoshitaBot.Extensions;
    using YukinoshitaBot.Services;

    // TODO 图片消息处理 消息回复 事件系统

    /// <summary>
    /// This is the main class of the application.
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            // 配置Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            try
            {
                Log.Information("Starting host");
                CreateBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 创建并配置HostBuilder
        /// </summary>
        /// <param name="args">控制台参数</param>
        /// <returns><see cref="IHostBuilder"/>的实例</returns>
        private static IHostBuilder CreateBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.AddSerilog();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddYukinoshitaBot();
                services.AddScoped<BksJwcSpider>();
                services.AddScoped<BksJwcParser>();
            })
            .UseSerilog();
    }
}
