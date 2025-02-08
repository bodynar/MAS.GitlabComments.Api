namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Models;

    using Microsoft.AspNetCore.Mvc;
    using MAS.GitlabComments.Base;

    [ApiController]
    [UseReadOnlyMode]
    [Route("api/stats")]
    public class StatsApiController : ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc cref="ICommentStoryRecordService"/>
        private ICommentStoryRecordService StoryRecordService { get; }

        /// <summary>
        /// Initialize <see cref="StatsApiController"/>
        /// </summary>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="storyRecordService">Service for story records managing</param>
        /// <exception cref="ArgumentNullException">Parameter storyRecordService is null</exception>
        public StatsApiController(
            ILogger logger,
            ICommentStoryRecordService storyRecordService
        )
        {
            Logger = logger;
            StoryRecordService = storyRecordService ?? throw new ArgumentNullException(nameof(storyRecordService));
        }

        /// <summary>
        /// Get top comments in specific data range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Boxed result of query</returns>
        [AllowInReadOnly]
        [HttpGet("top")]
        public BaseServiceResult<IEnumerable<StoryRecordViewModel>> Get([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = StoryRecordService.Get(startDate, endDate);

                return BaseServiceResult<IEnumerable<StoryRecordViewModel>>.Success(result);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Trying to: Get top stats");
                return BaseServiceResult<IEnumerable<StoryRecordViewModel>>.Error(e);
            }
        }
    }
}
