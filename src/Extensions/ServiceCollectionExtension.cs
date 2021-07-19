// <copyright file="ServiceCollectionExtension.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using YukinoshitaBot.Data.Attributes;
    using YukinoshitaBot.Services;

    /// <summary>
    /// 依赖注入拓展
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 添加YukinoshitaBot服务，使用自定义消息处理器
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

        /// <summary>
        /// 添加YukinoshitaBot服务，使用YukinoshitaController处理消息
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <returns>链式调用服务容器</returns>
        public static IServiceCollection AddYukinoshitaBot(this IServiceCollection services)
        {
            services.AddSingleton<OpqApi>();
            services.AddHostedService<MessageQueueScanner>();

            services.AddHostedService<MainWorker>();

            // 扫描当前程序集，添加所有带有YukinoshitaControllerAttribute的服务作为控制器
            var ass = Assembly.GetExecutingAssembly();
            var controllerTypes = new List<Type>();
            foreach (var type in ass.GetTypes())
            {
                if (type.GetCustomAttribute<YukinoshitaControllerAttribute>() is not null)
                {
                    services.AddSingleton(type);
                    controllerTypes.Add(type);
                }
            }

            services.AddScoped<IMessageHandler, YukinoshitaController>(services =>
            {
                var controllerCollection = new YukinoshitaController();

                // 将以注入的Controller添加进ControllerService
                foreach (var type in controllerTypes)
                {
                    if (services.GetService(type) is object controller)
                    {
                        controllerCollection.AddController(controller);
                    }
                }

                return controllerCollection;
            });

            return services;
        }
    }
}

