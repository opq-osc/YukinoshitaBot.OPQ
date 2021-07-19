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
    using YukinoshitaBot.Data.Attributes;
    using YukinoshitaBot.Data.Event;

    /// <summary>
    /// 实现控制器
    /// </summary>
    public class YukinoshitaController : IMessageHandler
    {
        private readonly Dictionary<object, YukinoshitaControllerAttribute> controllers;
        private IEnumerable<Tuple<Type, object, YukinoshitaControllerAttribute>>? controllerInfo;
        private Dictionary<Type, List<Tuple<MethodInfo, YukinoshitaHandlerAttribute>>>? controllerMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaController"/> class.
        /// </summary>
        public YukinoshitaController()
        {
            this.controllers = new Dictionary<object, YukinoshitaControllerAttribute>();
        }

        private static bool CheckMatch(string msg, string cmd, CommandMatchMethod method) => method switch
        {
            CommandMatchMethod.Strict => msg == cmd,
            CommandMatchMethod.StartWith => msg.StartsWith(cmd),
            CommandMatchMethod.Regex => Regex.IsMatch(msg, cmd),
            _ => false
        };

        /// <summary>
        /// 解析消息处理器
        /// </summary>
        public void ResolveHandlers()
        {
            // 对控制器进行排序
            this.controllerInfo = from KeyValuePair<object, YukinoshitaControllerAttribute> item in this.controllers
                                  orderby item.Value.Priority ascending
                                  let type = item.Key.GetType()
                                  select new Tuple<Type, object, YukinoshitaControllerAttribute>(type, item.Key, item.Value);

            this.controllerMethods = new Dictionary<Type, List<Tuple<MethodInfo, YukinoshitaHandlerAttribute>>>();

            foreach (var item in this.controllerInfo)
            { 
                // 对每一个控制器类型，获取其方法
                var allMethods = item.Item1.GetMethods();
                var unorderdMethodInfo = new List<Tuple<MethodInfo, YukinoshitaHandlerAttribute>>(allMethods.Length);
                foreach (var method in allMethods)
                {
                    // 筛选含有YukinoshitaHandlerAttribute的方法
                    if (method.GetCustomAttribute<YukinoshitaHandlerAttribute>() is YukinoshitaHandlerAttribute attribute)
                    {
                        unorderdMethodInfo.Add(new (method, attribute));
                    }
                }

                // 对控制器中的处理方法进行排序
                var orderdList = from methodInfo in unorderdMethodInfo
                                 orderby methodInfo.Item2.Priority ascending
                                 select methodInfo;
                this.controllerMethods.Add(item.Item1, orderdList.ToList());
            }
        }

        /// <summary>
        /// 添加控制器
        /// </summary>
        /// <param name="controller">要添加的控制器</param>
        /// <param name="controllerInfo">控制器Attribute信息</param>
        public void AddController(object controller, YukinoshitaControllerAttribute controllerInfo)
        {
            this.controllers.Add(controller, controllerInfo);
        }

        /// <inheritdoc/>
        public void OnFriendPictureMsgRecieved(PictureMessage msg)
        {
            if (this.controllerInfo is not null && this.controllerMethods is not null)
            {
                foreach (var controller in this.controllerInfo)
                {
                    foreach (var method in this.controllerMethods[controller.Item1])
                    {
                        var isMatch = CheckMatch(msg.Content, method.Item2.Command, method.Item2.MatchMethod);
                        if (isMatch)
                        {
                            method.Item1.Invoke(controller.Item2, new object[] { msg });
                            if (method.Item2.Mode == HandleMode.Break)
                            {
                                break;
                            }
                        }
                    }

                    if (controller.Item3.Mode == HandleMode.Break)
                    {
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnFriendTextMsgRecieved(TextMessage msg)
        {
            if (this.controllerInfo is not null && this.controllerMethods is not null)
            {
                foreach (var controller in this.controllerInfo)
                {
                    foreach (var method in this.controllerMethods[controller.Item1])
                    {
                        var isMatch = CheckMatch(msg.Content, method.Item2.Command, method.Item2.MatchMethod);
                        if (isMatch)
                        {
                            method.Item1.Invoke(controller.Item2, new object[] { msg });
                            if (method.Item2.Mode == HandleMode.Break)
                            {
                                break;
                            }
                        }
                    }

                    if (controller.Item3.Mode == HandleMode.Break)
                    {
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnGroupPictureMsgRecieved(PictureMessage msg)
        {
            if (this.controllerInfo is not null && this.controllerMethods is not null)
            {
                foreach (var controller in this.controllerInfo)
                {
                    foreach (var method in this.controllerMethods[controller.Item1])
                    {
                        var isMatch = CheckMatch(msg.Content, method.Item2.Command, method.Item2.MatchMethod);
                        if (isMatch)
                        {
                            method.Item1.Invoke(controller.Item2, new object[] { msg });
                            if (method.Item2.Mode == HandleMode.Break)
                            {
                                break;
                            }
                        }
                    }

                    if (controller.Item3.Mode == HandleMode.Break)
                    {
                        break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void OnGroupTextMsgRecieved(TextMessage msg)
        {
            if (this.controllerInfo is not null && this.controllerMethods is not null)
            {
                foreach (var controller in this.controllerInfo)
                {
                    foreach (var method in this.controllerMethods[controller.Item1])
                    {
                        var isMatch = CheckMatch(msg.Content, method.Item2.Command, method.Item2.MatchMethod);
                        if (isMatch)
                        {
                            method.Item1.Invoke(controller.Item2, new object[] { msg });
                            if (method.Item2.Mode == HandleMode.Break)
                            {
                                break;
                            }
                        }
                    }

                    if (controller.Item3.Mode == HandleMode.Break)
                    {
                        break;
                    }
                }
            }
        }
    }
}
