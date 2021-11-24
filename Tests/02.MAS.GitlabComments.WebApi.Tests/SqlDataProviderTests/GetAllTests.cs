namespace MAS.GitlabComments.Tests.SqlDataProviderTests
{
    using Xunit;

    public sealed class GetAllTests : BaseSqlDataProviderTests
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
