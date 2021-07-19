// <copyright file="BksJwcParser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace YukinoshitaBot.Services
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using YukinoshitaBot.Data.Whut;

    /// <summary>
    /// 本科生教务数据解析
    /// </summary>
    public class BksJwcParser
    {
        private static readonly Dictionary<int, int> bigToSmallSectionDic = new ()
        {
            { 1, 1 }, { 2, 3 }, { 3, 6 },
            { 4, 9 }, { 5, 11 }
        };

        /// <summary>
        /// 解析课程数据
        /// </summary>
        /// <param name="page">个人课表页</param>
        /// <returns>课程数据</returns>
        public IEnumerable<Course> ParseCourses(string page)
        {
            var content = Regex.Replace(page, @"[\r\t\n]", string.Empty);
            var divMatch = Regex.Match(content, "<td id=\"t(\\d)(\\d)\".*?>(.*?)</td>");
            while (divMatch.Success)
            {
                // 0, 2, 4, 6, 8 => 1, 2, 3, 4, 5
                var section = (int.Parse(divMatch.Groups[1].Value) >> 1) + 1;
                var weekday = int.Parse(divMatch.Groups[2].Value);
                var courseMatch = Regex.Match(divMatch.Groups[3].Value, @"(.*?)\(第(\d+)-(\d+)周((\d+)-(\d+)节)?,(.*?),(.*?)\)&nbsp;&nbsp;");
                while (courseMatch.Success)
                {
                    // 没有写节次的课程默认为两节
                    var sectionStart = string.IsNullOrEmpty(courseMatch.Groups[5].Value) ? bigToSmallSectionDic[section] : int.Parse(courseMatch.Groups[5].Value);
                    var sectionEnd = string.IsNullOrEmpty(courseMatch.Groups[6].Value) ? bigToSmallSectionDic[section] + 1 : int.Parse(courseMatch.Groups[6].Value);
                    yield return new Course
                    {
                        Name = courseMatch.Groups[1].Value,
                        WeekStart = int.Parse(courseMatch.Groups[2].Value),
                        WeekEnd = int.Parse(courseMatch.Groups[3].Value),
                        Teacher = courseMatch.Groups[7].Value,
                        Room = courseMatch.Groups[8].Value,
                        SectionStart = sectionStart,
                        SectionEnd = sectionEnd,
                        WeekDay = weekday
                    };
                    courseMatch = courseMatch.NextMatch();
                }

                divMatch = divMatch.NextMatch();
            }
        }
    }
}
