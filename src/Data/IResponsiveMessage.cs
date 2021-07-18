// <copyright file="IResponsiveMessage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Data
{
    /// <summary>
    /// 消息回复接口
    /// </summary>
    public interface IResponsiveMessage
    {
        /// <summary>
        /// 回复消息
        /// </summary>
        /// <param name="message">要回复的消息</param>
        void Response(string message);
    }
}
