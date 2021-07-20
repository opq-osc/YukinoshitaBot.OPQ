// <copyright file="YukinoshitaHandlerInfo.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Data.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 消息处理方法信息
    /// </summary>
    public struct YukinoshitaHandlerInfo : IComparable<YukinoshitaHandlerInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaHandlerInfo"/> struct.
        /// </summary>
        /// <param name="method">方法</param>
        public YukinoshitaHandlerInfo(MethodInfo method)
        {
            this.Method = method;
            var attribute = method.GetCustomAttribute<YukinoshitaHandlerAttribute>();
            if (attribute is null)
            {
                throw new InvalidCastException($"Method '{method.Name}' is not a YukinoshitaHandler.");
            }

            this.MethodAttribute = attribute;
        }

        /// <summary>
        /// 方法
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// 方法属性
        /// </summary>
        public YukinoshitaHandlerAttribute MethodAttribute { get; set; }

        /// <inheritdoc/>
        public int CompareTo(YukinoshitaHandlerInfo other)
        {
            return this.MethodAttribute.Priority - other.MethodAttribute.Priority;
        }
    }
}
