// <copyright file="YukinoshitaController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using YukinoshitaBot.Data.Attributes;
    using YukinoshitaBot.Data.Event;

    /// <summary>
    /// 实现控制器
    /// </summary>
    public class YukinoshitaController : IMessageHandler
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger logger;
        private readonly ControllerCollection controllers;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaController"/> class.
        /// </summary>
        /// <param name="serviceProvider">服务容器</param>
        /// <param name="controllerCollection">控制器容器</param>
        /// <param name="logger">日志</param>
        /// <param name="cache">缓存</param>
        public YukinoshitaController(
            IServiceProvider serviceProvider,
            ILogger<IMessageHandler> logger,
            ControllerCollection controllerCollection,
            IMemoryCache cache)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.controllers = controllerCollection;
            this.cache = cache;
        }

        /// <inheritdoc/>
        public void OnFriendPictureMsgRecieved(PictureMessage msg)
        {
            foreach (var controller in this.controllers.ResolvedControllers)
            {
                foreach (var method in this.controllers.Handlers[controller.ControllerType])
                {
                    if (CheckMatch(msg.Content, method.MethodAttribute.Command, method.MethodAttribute.MatchMethod))
                    {
                        var controllerObj = this.GetController(controller, msg.SenderInfo);
                        method.Method.Invoke(controllerObj, new object[] { msg });
                    }

                    if (method.MethodAttribute.Mode is HandleMode.Break)
                    {
                        break;
                    }
                }

                if (controller.ControllerAttribute.Mode is HandleMode.Break)
                {
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public void OnFriendTextMsgRecieved(TextMessage msg)
        {
            foreach (var controller in this.controllers.ResolvedControllers)
            {
                foreach (var method in this.controllers.Handlers[controller.ControllerType])
                {
                    if (CheckMatch(msg.Content, method.MethodAttribute.Command, method.MethodAttribute.MatchMethod))
                    {
                        var controllerObj = this.GetController(controller, msg.SenderInfo);
                        method.Method.Invoke(controllerObj, new object[] { msg });
                    }

                    if (method.MethodAttribute.Mode is HandleMode.Break)
                    {
                        break;
                    }
                }

                if (controller.ControllerAttribute.Mode is HandleMode.Break)
                {
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public void OnGroupPictureMsgRecieved(PictureMessage msg)
        {
            foreach (var controller in this.controllers.ResolvedControllers)
            {
                foreach (var method in this.controllers.Handlers[controller.ControllerType])
                {
                    if (CheckMatch(msg.Content, method.MethodAttribute.Command, method.MethodAttribute.MatchMethod))
                    {
                        var controllerObj = this.GetController(controller, msg.SenderInfo);
                        method.Method.Invoke(controllerObj, new object[] { msg });
                    }

                    if (method.MethodAttribute.Mode is HandleMode.Break)
                    {
                        break;
                    }
                }

                if (controller.ControllerAttribute.Mode is HandleMode.Break)
                {
                    break;
                }
            }
        }

        /// <inheritdoc/>
        public void OnGroupTextMsgRecieved(TextMessage msg)
        {
            foreach (var controller in this.controllers.ResolvedControllers)
            {
                var handled = false;
                foreach (var method in this.controllers.Handlers[controller.ControllerType])
                {
                    if (CheckMatch(msg.Content, method.MethodAttribute.Command, method.MethodAttribute.MatchMethod))
                    {
                        var controllerObj = this.GetController(controller, msg.SenderInfo);
                        method.Method.Invoke(controllerObj, new object[] { msg });
                        handled = true;

                        if (method.MethodAttribute.Mode is HandleMode.Break)
                        {
                            break;
                        }
                    }
                }

                if (handled && controller.ControllerAttribute.Mode is HandleMode.Break)
                {
                    break;
                }
            }
        }

        private static bool CheckMatch(string msg, string cmd, CommandMatchMethod method) => method switch
        {
            CommandMatchMethod.Strict => msg == cmd,
            CommandMatchMethod.StartWith => msg.StartsWith(cmd),
            CommandMatchMethod.Regex => Regex.IsMatch(msg, cmd),
            _ => false
        };

        private object GetController(YukinoshitaControllerInfo controllerInfo, SenderInfo msgSender)
        {
            return (controllerInfo.ControllerAttribute.SessionType, msgSender.SenderType) switch
            {
                (_, SenderType.Friend or SenderType.TempSession) => this.GetPersonalController(controllerInfo.ControllerType, msgSender.FromQQ ?? default),
                (SessionType.Person, SenderType.Group) => this.GetPersonalController(controllerInfo.ControllerType, msgSender.FromQQ ?? default),
                (SessionType.Group, SenderType.Group) => this.GetGroupController(controllerInfo.ControllerType, msgSender.FromGroupId ?? default),
                (_, _) => this.GetNoSessionController(controllerInfo.ControllerType)
            };
        }

        private object GetPersonalController(Type controllerType, long qq)
        {
            var cacheKey = $"PersonnalController_{controllerType.FullName}_{qq}";
            var cacheHit = this.cache.TryGetValue(cacheKey, out var controller);

            if (!cacheHit)
            {
                controller = this.serviceProvider.GetService(controllerType);
                this.logger.LogDebug("creating new controller {key}", cacheKey);
                if (controller is null)
                {
                    throw new InvalidOperationException("controller is not resolved.");
                }

                // 会话缓存10min
                this.cache.Set(cacheKey, controller, new TimeSpan(0, 10, 0));
            }
            else
            {
                // 刷新缓存
                this.logger.LogDebug("got controller from cache {key}", cacheKey);
                this.cache.Remove(cacheKey);
                this.cache.Set(cacheKey, controller, new TimeSpan(0, 10, 0));
            }

            return controller;
        }

        private object GetGroupController(Type controllerType, long groupId)
        {
            var cacheKey = $"GroupController_{controllerType.FullName}_{groupId}";
            var cacheHit = this.cache.TryGetValue(cacheKey, out var controller);

            if (!cacheHit)
            {
                controller = this.serviceProvider.GetService(controllerType);
                if (controller is null)
                {
                    throw new InvalidOperationException("controller is not resolved.");
                }

                // 会话缓存10min
                this.cache.Set(cacheKey, controller, new TimeSpan(0, 10, 0));
            }
            else
            {
                // 刷新缓存
                this.cache.Remove(cacheKey);
                this.cache.Set(cacheKey, controller, new TimeSpan(0, 10, 0));
            }

            return controller;
        }

        private object GetNoSessionController(Type controllerType)
        {
            var cacheKey = $"Controller_{controllerType.FullName}";
            var cacheHit = this.cache.TryGetValue(cacheKey, out var controller);

            if (!cacheHit)
            {
                controller = this.serviceProvider.GetService(controllerType);
                if (controller is null)
                {
                    throw new InvalidOperationException("controller is not resolved.");
                }

                // 会话缓存60min
                this.cache.Set(cacheKey, controller, new TimeSpan(1, 0, 0));
            }
            else
            {
                // 刷新缓存
                this.cache.Remove(cacheKey);
                this.cache.Set(cacheKey, controller, new TimeSpan(1, 0, 0));
            }

            return controller;
        }
    }
}
