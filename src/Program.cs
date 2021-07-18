// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using YukinoshitaBot.Services;

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
                services.AddSingleton<OpqApi>();
                services.AddHostedService<MessageQueueScanner>();
                services.AddHostedService<MainWorker>();
            })
            .UseSerilog();
    }
}
