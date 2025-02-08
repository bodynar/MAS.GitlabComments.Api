namespace MAS.GitlabComments.DataAccess.Tests.PgSqlDataProviderTests
{
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="PgSqlDataProvider{TEntity}.Get"/>
    /// </summary>
    public sealed class GetAllTests : BasePgSqlDataProviderTests
    {
        [Fact]
        public void ShouldExecuteQuery()
        {
            string expectedSqlQuery = $"SELECT * FROM \"{TestedTableName}\"";

            TestedService.Get();
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }
    }
}
