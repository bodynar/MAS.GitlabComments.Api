namespace MAS.GitlabComments.Controllers
{
    using System;

    using MAS.GitlabComments.Attributes;
    using MAS.GitlabComments.Models;

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
        public bool IsReadOnly()
        {
            return AppSettings.ReadOnlyMode;
        }
    }
}
