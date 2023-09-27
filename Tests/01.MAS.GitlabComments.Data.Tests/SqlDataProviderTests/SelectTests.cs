namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Select;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="SqlDataProvider{TEntity}.Select{TProjection}(Select.SelectConfiguration)"/>
    /// </summary>
    public sealed class SelectTests : BaseSqlDataProviderTests
    {
        public sealed class EmptyProjectedClass { }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenConfigurationIsNull()
        {
            SelectConfiguration configuration = null;

            var exception = Record.Exception(
                () => TestedService.Select<EmptyProjectedClass>(configuration)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenConfigurationContainsFilterWithInvalidColumns()
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

            SelectConfiguration configuration = new SelectConfiguration
            {
                Filter = filter,
            };

            Exception exception =
                Record.Exception(
                    () => TestedService.Select<EmptyProjectedClass>(configuration)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldBuildQueryWithAllColumnsWhenProjectedModelDoesNotHasRequiredAttributes()
        {
            var expectedSql = $"SELECT * FROM [{TestedTableName}]";

            SelectConfiguration configuration = new() { };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithSpecificColumnsWhenProjectedModelHasRequiredAttributes()
        {
            var expectedSql = $"SELECT [{TestedTableName}].[Column1], [RightTableName1].[Column2] FROM [{TestedTableName}]" +
                $" INNER JOIN [RightTableName1] AS [Alias1] WITH(NOLOCK)" +
                    $" ON ([Alias1].[RightTableRelationColumn1] = [{TestedTableName}].[LeftTableRelationColumn1])";

            SelectConfiguration configuration = new() { };
            ComplexColumnQueryBuilderResult = new ComplexColumnData()
            {
                Columns = new[]
                {
                    new ComplexColumn { Name = "Column1", TableAlias = TestedTableName },
                    new ComplexColumn { Name = "Column2", TableAlias = "RightTableName1" },
                },
                Joins = new[]
                {
                    new TableJoinData("test_join_data")
                    {
                        JoinType = TableJoinType.Inner,
                        RightTableName = "RightTableName1",
                        RightTableRelationColumn = "RightTableRelationColumn1",
                        Alias = "Alias1",
                        LeftTableName = TestedTableName,
                        LeftTableRelationColumn = "LeftTableRelationColumn1",
                    }
                }
            };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithSpecificColumnsAndFilterWhenProjectedModelHasRequiredAttributes()
        {
            var expectedSql = $"SELECT [{TestedTableName}].[Column1], [RightTableName1].[Column2] FROM [{TestedTableName}]" +
                $" INNER JOIN [RightTableName1] AS [Alias1] WITH(NOLOCK)" +
                    $" ON ([Alias1].[RightTableRelationColumn1] = [{TestedTableName}].[LeftTableRelationColumn1])" +
                    $" WHERE |filterValue|";

            SelectConfiguration configuration = new()
            {
                Filter = new FilterGroup()
                {
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
                }
            };
            FilterBuilderResult = new FilterResult
            {
                Sql = "|filterValue|",
                Values = new Dictionary<string, object>() { { "Key1", "Value1" } }
            };
            ComplexColumnQueryBuilderResult = new ComplexColumnData()
            {
                Columns = new[]
                {
                    new ComplexColumn { Name = "Column1", TableAlias = TestedTableName },
                    new ComplexColumn { Name = "Column2", TableAlias = "RightTableName1" },
                },
                Joins = new[]
                {
                    new TableJoinData("test_join_data")
                    {
                        JoinType = TableJoinType.Inner,
                        RightTableName = "RightTableName1",
                        RightTableRelationColumn = "RightTableRelationColumn1",
                        Alias = "Alias1",
                        LeftTableName = TestedTableName,
                        LeftTableRelationColumn = "LeftTableRelationColumn1",
                    }
                }
            };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
            AssertArguments(FilterBuilderResult.Values, lastQuery.Value.Value);
        }
    }
}
