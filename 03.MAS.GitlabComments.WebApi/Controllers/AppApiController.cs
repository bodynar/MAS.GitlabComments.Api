namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/app")]
    public class AppApiController
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger<AppApiController> Logger { get; }

        /// <inheritdoc cref="IApplicationWebSettings"/>
        private IApplicationWebSettings AppSettings { get; }

        /// <inheritdoc cref="ISystemVariableProvider"/>
        private ISystemVariableProvider SystemVariableProvider { get; }

        /// <summary>
        /// Initialize <see cref="AppApiController"/>
        /// </summary>
        /// <param name="appSettings">Global application settings</param>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="systemVariableProvider">Provider of system variables</param>
        /// <exception cref="ArgumentNullException">Parameter appSettings is null</exception>
        /// <exception cref="ArgumentNullException">Parameter systemVariableProvider is null</exception>
        public AppApiController(
            IApplicationWebSettings appSettings,
            ILogger<AppApiController> logger,
            ISystemVariableProvider systemVariableProvider
        )
        {
            Logger = logger;
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            SystemVariableProvider = systemVariableProvider ?? throw new ArgumentNullException(nameof(systemVariableProvider));
        }

        /// <summary>
        /// Get read only mode is on
        /// </summary>
        [AllowInReadOnly]
        [HttpGet("getIsReadOnly")]
        public BaseServiceResult<bool> IsReadOnly()
        {
            return BaseServiceResult<bool>.Success(AppSettings.ReadOnlyMode);
        }

        /// <summary>
        /// Get all system variables
        /// </summary>
        [HttpGet("getVariables")] // TODO: tests
        public BaseServiceResult<IEnumerable<SysVariableDisplayModel>> GetVariables()
        {
            try
            {
                var result = SystemVariableProvider.GetAllVariables();

                return BaseServiceResult<IEnumerable<SysVariableDisplayModel>>.Success(result);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, "Get variables");
                return BaseServiceResult<IEnumerable<SysVariableDisplayModel>>.Error(e);
            }
        }
    }
}
