// <copyright file="TextMessageRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Data.OpqApi
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    /// <summary>
    /// 文本消息
    /// </summary>
    public class TextMessageRequest : MessageRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessageRequest"/> class.
        /// </summary>
        /// <param name="content">消息内容</param>
        public TextMessageRequest(string content) : base()
        {
            this.Content = content;
            this.SendMsgType = "TextMsg";
        }

        /// <summary>
        /// 临时消息的群号
        /// </summary>
        public long? GroupID { get; set; }

        /// <inheritdoc/>
        public override HttpRequestMessage SendToFriend(long friendQQ)
        {
            this.SendToType = 1;
            this.ToUserUid = friendQQ;
            this.GroupID = null;

            this.HttpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(this, typeof(TextMessageRequest)), Encoding.UTF8, "application/json");

            return base.SendToFriend(friendQQ);
        }

        /// <inheritdoc/>
        public override HttpRequestMessage SendToGroup(long groupId)
        {
            this.SendToType = 2;
            this.ToUserUid = groupId;
            this.GroupID = null;

            string content = JsonSerializer.Serialize(this, typeof(TextMessageRequest));
            this.HttpRequestMessage.Content = new StringContent(content, Encoding.UTF8, "application/json");

            return base.SendToGroup(groupId);
        }

        /// <inheritdoc/>
        public override HttpRequestMessage SendToGroupMember(long userQQ, long groupId)
        {
            this.SendToType = 3;
            this.ToUserUid = userQQ;
            this.GroupID = groupId;

            this.HttpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(this, typeof(TextMessageRequest)), Encoding.UTF8, "application/json");

            return base.SendToGroupMember(userQQ, groupId);
        }
    }
}
