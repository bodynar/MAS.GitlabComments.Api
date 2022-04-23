namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;

    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;

    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/app")]
    public class AppApiController
    {
        /// <inheritdoc cref="AppSettings"/>
        private AppSettings AppSettings { get; }

        /// <summary>
        /// Initialize <see cref="AppApiController"/>
        /// </summary>
        /// <param name="appSettings">Global application settings</param>
        /// <exception cref="ArgumentNullException">Parameter commentService is null</exception>
        public AppApiController(AppSettings appSettings)
        {
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Get read only mode is on
        /// </summary>
        /// <returns><see langword="true"/> if read only mode is on; <see langword="false"/> otherwise</returns>
        [AllowInReadOnly]
        [HttpGet("getIsReadOnly")]
        public BaseServiceResult<bool> IsReadOnly()
        {
            return BaseServiceResult<bool>.Success(AppSettings.ReadOnlyMode);
        }
    }
}
