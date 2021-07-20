// <copyright file="ControllerCollection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using YukinoshitaBot.Data.Attributes;

    /// <summary>
    /// 控制器容器
    /// </summary>
    public class ControllerCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerCollection"/> class.
        /// </summary>
        public ControllerCollection()
        {
            this.ResolvedControllers = new SortedSet<YukinoshitaControllerInfo>();
            this.Handlers = new Dictionary<Type, SortedSet<YukinoshitaHandlerInfo>>();
        }

        /// <summary>
        /// 已解析的控制器
        /// </summary>
        public SortedSet<YukinoshitaControllerInfo> ResolvedControllers { get; init; }

        /// <summary>
        /// 已解析的Handler
        /// </summary>
        public Dictionary<Type, SortedSet<YukinoshitaHandlerInfo>> Handlers { get; init; }

        /// <summary>
        /// 添加一个控制器
        /// </summary>
        /// <param name="controllerType">控制器类型</param>
        public void AddController(Type controllerType)
        {
            this.ResolvedControllers.Add(new YukinoshitaControllerInfo(controllerType));

            // 对每一个控制器类型，获取其方法
            var allMethods = controllerType.GetMethods();
            var handlers = new SortedSet<YukinoshitaHandlerInfo>();

            foreach (var method in allMethods)
            {
                // 筛选含有YukinoshitaHandlerAttribute的方法
                if (method.GetCustomAttribute<YukinoshitaHandlerAttribute>() is YukinoshitaHandlerAttribute)
                {
                    handlers.Add(new YukinoshitaHandlerInfo(method));
                }
            }

            this.Handlers.Add(controllerType, handlers);
        }
    }
}
