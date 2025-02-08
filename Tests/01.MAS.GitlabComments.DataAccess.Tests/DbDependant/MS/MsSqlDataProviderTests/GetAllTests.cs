namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="MsSqlDataProvider{TEntity}.Get"/>
    /// </summary>
    public sealed class GetAllTests : BaseMsSqlDataProviderTests
    {
        [Fact]
        public void ShouldExecuteQuery()
        {
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}]";

            TestedService.Get();
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }
    }
}
