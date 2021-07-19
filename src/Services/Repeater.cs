// <copyright file="Repeater.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System;
    using YukinoshitaBot.Data.Event;
    using YukinoshitaBot.Data.OpqApi;

    /// <summary>
    /// 复读机
    /// </summary>
    public class Repeater : IMessageHandler
    {
        /// <inheritdoc/>
        public void OnFriendPictureMsgRecieved(PictureMessage msg)
        {
            if (msg.Content.StartsWith("read"))
            {
                msg.Reply(new PictureMessageRequest(new Uri(msg.FirstPicture)));
            }
        }

        /// <inheritdoc/>
        public void OnFriendTextMsgRecieved(TextMessage msg)
        {
            if (msg.Content.StartsWith("read "))
            {
                msg.Reply(new TextMessageRequest(msg.Content[5..]));
            }
        }

        /// <inheritdoc/>
        public void OnGroupPictureMsgRecieved(PictureMessage msg)
        {
            if (msg.Content.StartsWith("read"))
            {
                msg.Reply(new PictureMessageRequest(new Uri(msg.FirstPicture)));
            }
        }

        /// <inheritdoc/>
        public void OnGroupTextMsgRecieved(TextMessage msg)
        {
            if (msg.Content.StartsWith("read "))
            {
                msg.Reply(new TextMessageRequest(msg.Content[5..]));
            }
        }
    }
}
