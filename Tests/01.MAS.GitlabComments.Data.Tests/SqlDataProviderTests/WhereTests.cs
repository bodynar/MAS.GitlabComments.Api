namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Filter;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="SqlDataProvider{TEntity}.Where(FilterGroup)"/>
    /// </summary>
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
        public void ShouldReturnAllEntitiesWhenFilterIsEmpty()
        {
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}]";
            FilterGroup filter = new()
            {
                Items = null,
                NestedGroups = null
            };

            TestedService.Where(filter);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Null(lastQuery.Value.Value);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenFilterContainsInvalidColumns()
        {
            FilterGroup filter = new()
            {
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "__SomeNotExistedFieldName__", /// <see cref="TestedDataProviderEntity"/>
                        LogicalComparisonType = ComparisonType.Equal,
                        Name = "c",
                        Value = true
                    }
                }
            };
            string expectedExceptionMessage = $"Filter contains columns not presented in entity: __SomeNotExistedFieldName__";

            Exception exception =
                Record.Exception(
                    () => TestedService.Where(filter)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldReturnAllEntitiesWhenFilterBuiltAsEmpty()
        {
            FilterBuilderResult = new FilterResult();
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
            FilterBuilderResult = new FilterResult() { Sql = "filter", };
            string expectedSqlQuery = $"SELECT * FROM [{TestedTableName}] WHERE filter";
            FilterGroup filter = new()
            {
                Name = "TestedFilter",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = nameof(TestedDataProviderEntity.IntField),
                        LogicalComparisonType = ComparisonType.Equal,
                        Name = "c",
                        Value = true
                    }
                }
            };

            TestedService.Where(filter);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSqlQuery, lastQuery.Value.Key);
            Assert.Empty(lastQuery.Value.Value);
        }
    }
}
