namespace MAS.GitlabComments.Tests.AppApiControllerTests
{
    using MAS.GitlabComments.Controllers;
    using MAS.GitlabComments.Models;

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
        /// Initializing <see cref="BaseAppApiControllerTests"/> with setup of all required environment
        /// </summary>
        protected BaseAppApiControllerTests()
        {
            var appSettings = GetDependencies();
            TestedController = new AppApiController(appSettings);
        }

        #region Private members

        /// <summary>
        /// Configure mock object of data provider for comment service
        /// </summary>
        /// <returns>Mock object of <see cref="AppSettings"/></returns>
        private AppSettings GetDependencies()
        {
            var mockSettings = new Mock<AppSettings>(SettingReadOnlyMode);

            mockSettings
                .SetupGet(x => x.ReadOnlyMode)
                .Returns(() => SettingReadOnlyMode);

            return mockSettings.Object;
        }

        #endregion
    }
}
