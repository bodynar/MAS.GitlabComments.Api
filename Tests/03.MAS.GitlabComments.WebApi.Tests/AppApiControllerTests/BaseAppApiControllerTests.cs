namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Models.Import;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.WebApi.Controllers;
    using MAS.GitlabComments.WebApi.Models;

    using Moq;

    /// <summary>
    /// Base class for <see cref="AppApiController"/> tests
    /// </summary>
    public abstract class BaseAppApiControllerTests
    {
        /// <summary>
        /// Instance of <see cref="AppApiController"/> for tests
        /// </summary>
        protected AppApiController TestedController { get; }

        /// <summary>
        /// Value returned by <see cref="AppSettings.ReadOnlyMode"/> property
        /// </summary>
        protected bool SettingReadOnlyMode { get; set; }

        /// <summary>
        /// Should tested method throw an exception
        /// </summary>
        protected bool ShouldThrowAnException { get; set; }

        /// <summary>
        /// Return result of call <see cref="ISystemVariableProvider.GetAllVariables()"/>
        /// </summary>
        protected IEnumerable<SysVariableDisplayModel> AllVariables { get; }
            = new[] { new SysVariableDisplayModel() { Id = Guid.NewGuid(), Code = "Test", Caption = "Test", RawValue = "Test", Type = "Test" } };

        /// <summary>
        /// Thrown exception message
        /// </summary>
        protected const string ExceptionTestMessage = "ExceptionTestMessage";

        /// <summary>
        /// Initializing <see cref="BaseAppApiControllerTests"/> with setup of all required environment
        /// </summary>
        protected BaseAppApiControllerTests()
        {
            var dependencies = GetDependencies();
            TestedController = new AppApiController(dependencies.Item1, dependencies.Item2, dependencies.Item3, dependencies.Item4, dependencies.Item5);
        }

        #region Private members

        /// <summary>
        /// Configure mock objects of required dependencies for tested class
        /// </summary>
        /// <returns>Mock objects, required in ctor <see cref="AppApiController"/></returns>
        private (IApplicationWebSettings, ILogger, ISystemVariableProvider, ISystemVariableActionExecutor, IDataImporter) GetDependencies()
        {
            var mockSettings = new Mock<IApplicationWebSettings>();
            var mockLogger = new Mock<ILogger>();
            var variableMock = new Mock<ISystemVariableProvider>();
            var variableExecutorMock = new Mock<ISystemVariableActionExecutor>();
            var dataImporterMock = new Mock<IDataImporter>();

            Action commentServiceExceptionCallback = () =>
            {
                if (ShouldThrowAnException)
                {
                    throw new Exception(ExceptionTestMessage);
                }
            };

            mockSettings
                .SetupGet(x => x.ReadOnlyMode)
                .Callback(commentServiceExceptionCallback)
                .Returns(() => SettingReadOnlyMode);

            variableMock
                .Setup(x => x.GetAllVariables())
                .Callback(commentServiceExceptionCallback)
                .Returns(AllVariables);

            variableExecutorMock
                .Setup(x => x.ExecuteAction(It.IsAny<string>()))
                .Callback(commentServiceExceptionCallback);

            dataImporterMock
                .Setup(x => x.ExportAppData())
                .Callback(commentServiceExceptionCallback)
                .Returns(() => Enumerable.Empty<CommentExportModel>());

            dataImporterMock
                .Setup(x => x.ImportAppData(It.IsAny<IEnumerable<CommentExportModel>>()))
                .Callback(commentServiceExceptionCallback);

            return (
                mockSettings.Object,
                mockLogger.Object,
                variableMock.Object,
                variableExecutorMock.Object,
                dataImporterMock.Object
            );
        }

        #endregion
    }
}
