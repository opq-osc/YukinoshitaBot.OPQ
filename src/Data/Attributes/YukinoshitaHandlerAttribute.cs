// <copyright file="YukinoshitaHandlerAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Data.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 定义为Yukinoshita消息处理方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class YukinoshitaHandlerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaHandlerAttribute"/> class.
        /// </summary>
        public YukinoshitaHandlerAttribute()
        {
            this.Command = string.Empty;
            this.MatchMethod = CommandMatchMethod.StartWith;
            this.Priority = int.MaxValue;
            this.Mode = HandleMode.Break;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaHandlerAttribute"/> class.
        /// </summary>
        /// <param name="command">要匹配的指令</param>
        public YukinoshitaHandlerAttribute(string command)
        {
            this.Command = command;
            this.MatchMethod = CommandMatchMethod.StartWith;
            this.Priority = 10;
            this.Mode = HandleMode.Pass;
        }

        /// <summary>
        /// 指令识别方式
        /// </summary>
        public CommandMatchMethod MatchMethod { get; set; }

        /// <summary>
        /// 匹配的指令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 优先级，越小优先级越高
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 处理模式
        /// </summary>
        public HandleMode Mode { get; set; }
    }
}
