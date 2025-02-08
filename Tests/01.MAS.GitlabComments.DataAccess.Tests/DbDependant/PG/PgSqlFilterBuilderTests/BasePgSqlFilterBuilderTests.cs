namespace MAS.GitlabComments.DataAccess.Tests.PgSqlFilterBuilderTests
{
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG;

    /// <summary>
    /// Base class for <see cref="PgSqlFilterBuilder"/> tests
    /// </summary>
    public abstract class BasePgSqlFilterBuilderTests
    {
        /// <summary>
        /// Instance of <see cref="PgSqlFilterBuilder"/> for tests
        /// </summary>
        protected PgSqlFilterBuilder TestedService { get; }

        /// <summary>
        /// Initializing <see cref="BasePgSqlFilterBuilderTests"/>
        /// </summary>
        protected BasePgSqlFilterBuilderTests()
        {
            TestedService = new PgSqlFilterBuilder();
        }
    }
}
