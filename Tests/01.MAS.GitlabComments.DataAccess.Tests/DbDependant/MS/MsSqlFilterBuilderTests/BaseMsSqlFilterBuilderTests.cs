namespace MAS.GitlabComments.DataAccess.Tests.MsSqlFilterBuilderTests
{
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS;

    /// <summary>
    /// Base class for <see cref="MsSqlFilterBuilder"/> tests
    /// </summary>
    public abstract class BaseMsSqlFilterBuilderTests
    {
        /// <summary>
        /// Instance of <see cref="MsSqlFilterBuilder"/> for tests
        /// </summary>
        protected MsSqlFilterBuilder TestedService { get; }

        /// <summary>
        /// Initializing <see cref="BaseSqlDataProviderTests"/> with setup'n all required environment
        /// </summary>
        protected BaseMsSqlFilterBuilderTests()
        {
            TestedService = new MsSqlFilterBuilder();
        }
    }
}
