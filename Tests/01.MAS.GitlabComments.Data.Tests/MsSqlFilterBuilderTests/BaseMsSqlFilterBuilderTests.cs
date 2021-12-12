namespace MAS.GitlabComments.Data.Tests.MsSqlFilterBuilderTests
{
    using System;

    using MAS.GitlabComments.Data.Services;

    /// <summary>
    /// Base class for <see cref="MsSqlFilterBuilder"/> tests
    /// </summary>
    public abstract class BaseMsSqlFilterBuilderTests
    {
        /// <summary>
        /// Instance of <see cref="MsSqlFilterBuilder"/> for tests
        /// </summary>
        protected MsSqlFilterBuilder TestedService { get; }

        /// <summary> New line </summary>
        protected static string nl = Environment.NewLine;

        /// <summary>
        /// Initializing <see cref="BaseSqlDataProviderTests"/> with setup'n all required environment
        /// </summary>
        protected BaseMsSqlFilterBuilderTests()
        {
            TestedService = new MsSqlFilterBuilder();
        }
    }
}
