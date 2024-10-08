﻿namespace MAS.GitlabComments.Logic.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Comments story records service
    /// </summary>
    public interface ICommentStoryRecordService
    {
        /// <summary>
        /// Read comments modification records by specified filters
        /// </summary>
        /// <param name="startDate">Min date to filter records</param>
        /// <param name="endDate">Max date to filter records</param>
        /// <param name="count">Comments count</param>
        /// <returns>Collection of comments modification actions</returns>
        IEnumerable<StoryRecordViewModel> Get(DateTime? startDate, DateTime? endDate, int? count = null);
    }
}
