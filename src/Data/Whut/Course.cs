// <copyright file="Course.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Data.Whut
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 课程数据
    /// </summary>
    public class Course
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Course"/> class.
        /// </summary>
        public Course()
        {
            this.Name = string.Empty;
            this.Teacher = string.Empty;
            this.Room = string.Empty;
        }

        /// <summary>
        /// 课程名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 授课教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 上课地点
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// 开始周
        /// </summary>
        public int WeekStart { get; set; }

        /// <summary>
        /// 结束周
        /// </summary>
        public int WeekEnd { get; set; }

        /// <summary>
        /// 开始节
        /// </summary>
        public int SectionStart { get; set; }

        /// <summary>
        /// 结束节
        /// </summary>
        public int SectionEnd { get; set; }

        /// <summary>
        /// 星期(1-7)
        /// </summary>
        public int WeekDay { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Name}@{this.Room},{this.Teacher},{this.WeekDay},{this.WeekStart}-{this.WeekEnd}周,{this.SectionStart}-{this.SectionEnd}节";
        }
    }
}
