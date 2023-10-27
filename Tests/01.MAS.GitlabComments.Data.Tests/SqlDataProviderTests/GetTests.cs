namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="SqlDataProvider{TEntity}.Get(Guid)"/>
    /// </summary>
    public sealed class GetTests : BaseSqlDataProviderTests
    {
        [Fact]
        public void ShouldExecuteQuery()
        {
            Guid testedId = Guid.NewGuid();
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}] WHERE Id = @P1";
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] { new KeyValuePair<string, object>("P1", testedId) };

            TestedService.Get(testedId);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.NotNull(lastQuery.Value.Value);
            AssertArguments(expectedArguments, lastQuery.Value.Value);
        }

        [Fact]
        public void ShouldThrowArgumentNullException_WhenIdIsDefault()
        {
            Guid entityId = default;

            Exception exception =
                Record.Exception(
                    () => TestedService.Get(entityId)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }
    }
}
