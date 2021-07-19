// <copyright file="YukinoshitaControllerAttribute.cs" company="PlaceholderCompany">
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
    /// 定义为YukinoshitaController
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class YukinoshitaControllerAttribute : Attribute
    {
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
