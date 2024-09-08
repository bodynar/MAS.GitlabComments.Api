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

        /// <inheritdoc cref="ISystemVariableActionExecutor" />
        private ISystemVariableActionExecutor SystemVariableActionExecutor { get; }

        /// <summary>
        /// Initialize <see cref="AppApiController"/>
        /// </summary>
        /// <param name="appSettings">Global application settings</param>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="systemVariableProvider">Provider of system variables</param>
        /// <param name="systemVariableActionExecutor">Executor of actions for system variables</param>
        /// <exception cref="ArgumentNullException">Parameter appSettings is null</exception>
        /// <exception cref="ArgumentNullException">Parameter systemVariableProvider is null</exception>
        /// <exception cref="ArgumentNullException">Parameter systemVariableActionExecutor is null</exception>
        public AppApiController(
            IApplicationWebSettings appSettings,
            ILogger<AppApiController> logger,
            ISystemVariableProvider systemVariableProvider,
            ISystemVariableActionExecutor systemVariableActionExecutor
        )
        {
            Logger = logger;
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            SystemVariableProvider = systemVariableProvider ?? throw new ArgumentNullException(nameof(systemVariableProvider));
            SystemVariableActionExecutor = systemVariableActionExecutor ?? throw new ArgumentNullException(nameof(systemVariableActionExecutor));
        }

        /// <summary>
        /// Get read only mode is on
        /// </summary>
        /// <returns>Flag, wrapped in service request result model</returns>
        [AllowInReadOnly]
        [HttpGet("getIsReadOnly")]
        public BaseServiceResult<bool> IsReadOnly()
        {
            try
            {
                return BaseServiceResult<bool>.Success(AppSettings.ReadOnlyMode);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Getting ReadOnlyMode");
                return BaseServiceResult<bool>.Error(e);
            }
        }

        /// <summary>
        /// Get all system variables
        /// </summary>
        /// <returns>System variables, wrapped in service request result model</returns>
        [HttpGet("getVariables")]
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

        /// <summary>
        /// Execute additional action for variable
        /// </summary>
        /// <param name="code">System variable code</param>
        /// <returns>Service request result model</returns>
        [HttpPost("executeOnVariable")]
        public BaseServiceResult ExecuteActionOnVariable([FromBody]string code)
        {
            try
            {
                SystemVariableActionExecutor.ExecuteAction(code);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"Trying to execute action for variable with code \"{code}\"");
                return BaseServiceResult.Error(e);
            }
        }
    }
}
