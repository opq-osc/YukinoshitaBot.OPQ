// <copyright file="ServiceCollectionExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using YukinoshitaBot.Services;

    /// <summary>
    /// 依赖注入拓展
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 添加YukinoshitaBot服务
        /// </summary>
        /// <typeparam name="MessageHandlerType">要使用的消息处理器类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <returns>链式调用服务容器</returns>
        public static IServiceCollection AddYukinoshitaBot<MessageHandlerType>(this IServiceCollection services) 
            where MessageHandlerType : class, IMessageHandler
        {
            services.AddSingleton<OpqApi>();
            services.AddHostedService<MessageQueueScanner>();
            services.AddScoped<IMessageHandler, MessageHandlerType>();
            services.AddHostedService<MainWorker>();
            return services;
        }
    }
}
