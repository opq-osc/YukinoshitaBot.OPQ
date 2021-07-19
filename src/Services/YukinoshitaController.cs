// <copyright file="YukinoshitaController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using YukinoshitaBot.Data.Event;

    /// <summary>
    /// 实现控制器
    /// </summary>
    public class YukinoshitaController : IMessageHandler
    {
        private readonly ArrayList controllers;

        /// <summary>
        /// Initializes a new instance of the <see cref="YukinoshitaController"/> class.
        /// </summary>
        public YukinoshitaController()
        {
            this.controllers = new ArrayList();
        }

        /// <summary>
        /// 添加控制器
        /// </summary>
        /// <param name="controller">要添加的控制器</param>
        public void AddController(object controller)
        {
            this.controllers.Add(controller);
        }

        /// <inheritdoc/>
        public void OnFriendPictureMsgRecieved(PictureMessage msg)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnFriendTextMsgRecieved(TextMessage msg)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnGroupPictureMsgRecieved(PictureMessage msg)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void OnGroupTextMsgRecieved(TextMessage msg)
        {
            throw new NotImplementedException();
        }
    }
}
