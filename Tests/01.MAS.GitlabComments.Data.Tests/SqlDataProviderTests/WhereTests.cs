namespace MAS.GitlabComments.Data.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data.Filter;

    using Xunit;

    public sealed class WhereTests : BaseSqlDataProviderTests
    {
        [Fact]
        public void ShouldReturnAllEntitiesWhenFilterIsNull()
        {
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}]";
            FilterGroup filter = null;

            TestedService.Where(filter);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }

        [Fact]
        public void ShouldReturnAllEntitiesWhenFilterBuiltAsEmpty()
        {
            FilterBuilderResult = new Tuple<string, IReadOnlyDictionary<string, object>>(string.Empty, null);
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}]";
            FilterGroup filter = new()
            {
                Name = "TestedFilter",
                LogicalJoinType = FilterJoinType.And
            };

            TestedService.Where(filter);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }

        [Fact]
        public void ShouldReturnFilteredEntitiesWhenFilterBuilt()
        {
            FilterBuilderResult = new Tuple<string, IReadOnlyDictionary<string, object>>("filter", null);
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}] WHERE filter";
            FilterGroup filter = new()
            {
                Name = "TestedFilter",
                LogicalJoinType = FilterJoinType.And
            };

            TestedService.Where(filter);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }
    }
}
