namespace MAS.GitlabComments.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Models.Import;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    [ApiController]
    [Route("api/app")]
    public class AppApiController: ControllerBase
    {
        /// <summary>
        /// Logger to store error information
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc cref="IApplicationWebSettings"/>
        private IApplicationWebSettings AppSettings { get; }

        /// <inheritdoc cref="ISystemVariableProvider"/>
        private ISystemVariableProvider SystemVariableProvider { get; }

        /// <inheritdoc cref="ISystemVariableActionExecutor" />
        private ISystemVariableActionExecutor SystemVariableActionExecutor { get; }

        /// <inheritdoc cref="IDataImporter" />
        private IDataImporter DataImporter { get; }

        /// <summary>
        /// Initialize <see cref="AppApiController"/>
        /// </summary>
        /// <param name="appSettings">Global application settings</param>
        /// <param name="logger">Logger to store error information</param>
        /// <param name="systemVariableProvider">Provider of system variables</param>
        /// <param name="systemVariableActionExecutor">Executor of actions for system variables</param>
        /// <param name="dataImporter"></param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="appSettings"/> is null</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="systemVariableProvider"/> is null</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="systemVariableActionExecutor"/> is null</exception>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="dataImporter"/> is null</exception>
        public AppApiController(
            IApplicationWebSettings appSettings,
            ILogger logger,
            ISystemVariableProvider systemVariableProvider,
            ISystemVariableActionExecutor systemVariableActionExecutor,
            IDataImporter dataImporter
        )
        {
            Logger = logger;
            AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            SystemVariableProvider = systemVariableProvider ?? throw new ArgumentNullException(nameof(systemVariableProvider));
            SystemVariableActionExecutor = systemVariableActionExecutor ?? throw new ArgumentNullException(nameof(systemVariableActionExecutor));
            DataImporter = dataImporter ?? throw new ArgumentNullException(nameof(dataImporter));
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
                Logger.Error(e, $"Getting ReadOnlyMode");
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
                Logger.Error(e, "Get variables");
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
                Logger.Error(e, $"Trying to execute action for variable with code \"{code}\"");
                return BaseServiceResult.Error(e);
            }
        }

        /// <summary>
        /// Export app data
        /// </summary>
        /// <returns>File with json exported data</returns>
        [HttpGet("exportData")]
        public IActionResult ExportAppData()
        {
            try
            {
                var appData = DataImporter.ExportAppData();
                var json = JsonConvert.SerializeObject(appData);

                var byteArray = Encoding.UTF8.GetBytes(json);
                var stream = new MemoryStream(byteArray);

                return File(stream, "application/json", $"MAS.GC_Export_{DateTime.UtcNow:dd.MM.yyyy_HH:mm}.json");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Trying to export app data");

                return Problem("Error during exporting app data");
            }
        }

        /// <summary>
        /// Import app data from file
        /// </summary>
        /// <param name="file">JSON file with data</param>
        /// <returns>Instance of <see cref="BaseServiceResult"/> representing result of api call</returns>
        [HttpPost("importData")]
        public async Task<BaseServiceResult> ImportAppData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BaseServiceResult.Error("No file imported");
            }

            if (file.ContentType != "application/json")
            {
                return BaseServiceResult.Error("File is not a json");
            }

            try
            {
                using var stream = new StreamReader(file.OpenReadStream());
                var jsonContent = await stream.ReadToEndAsync();

                var data = JsonConvert.DeserializeObject<IEnumerable<CommentExportModel>>(jsonContent);

                if (data == null)
                {
                    return BaseServiceResult.Error("JSON is incorrect");
                }

                DataImporter.ImportAppData(data);

                return BaseServiceResult.Success();
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Trying to import data from file \"{file?.FileName}\"");
                return BaseServiceResult.Error(e);
            }
        }
    }
}
