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
        public void ShouldThrowArgumentException_WhenConfigurationContainsFilterWithInvalidColumns()
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
        public void ShouldDoNotThrowArgumentException_WhenConfigurationContainsFilterWithValidColumns()
        {
            FilterGroup filter = new()
            {
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = nameof(TestedDataProviderEntity.CreatedOn),
                        LogicalComparisonType = ComparisonType.Equal,
                        Name = "c",
                        Value = true
                    },
                    new FilterItem
                    {
                        FieldName = nameof(TestedDataProviderEntity.StringField),
                        LogicalComparisonType = ComparisonType.Equal,
                        Name = "c",
                        Value = true
                    },
                }
            };

            SelectConfiguration configuration = new SelectConfiguration
            {
                Filter = filter,
            };

            Exception exception =
                Record.Exception(
                    () => TestedService.Select<EmptyProjectedClass>(configuration)
                );

            Assert.Null(exception);
        }

        [Fact]
        public void ShouldBuildQueryWithoutFilter_WhenFilterCannotBeBuilt()
        {
            FilterGroup filter = new()
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
            };
            var expectedSql = $"SELECT * FROM [{TestedTableName}]";

            SelectConfiguration configuration = new SelectConfiguration
            {
                Filter = filter,
            };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithAllColumns_WhenProjectedModelDoesNotHasRequiredAttributes()
        {
            var expectedSql = $"SELECT * FROM [{TestedTableName}]";

            SelectConfiguration configuration = new() { };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithAllColumns_WhenConfigurationIsNull()
        {
            var expectedSql = $"SELECT * FROM [{TestedTableName}]";

            TestedService.Select<EmptyProjectedClass>();
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithSpecificColumns_WhenProjectedModelHasRequiredAttributes()
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
        public void ShouldBuildQueryWithSpecificColumnsAndFilter_WhenProjectedModelHasRequiredAttributes()
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

        [Fact]
        public void ShouldBuildQueryWithoutFilter_WhenFilterBuildAnEmptyFilter()
        {
            var expectedSql = $"SELECT [{TestedTableName}].[Column1], [RightTableName1].[Column2] FROM [{TestedTableName}]" +
                $" INNER JOIN [RightTableName1] AS [Alias1] WITH(NOLOCK)" +
                    $" ON ([Alias1].[RightTableRelationColumn1] = [{TestedTableName}].[LeftTableRelationColumn1])";

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
                Sql = string.Empty,
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
            Assert.Empty(lastQuery.Value.Value);
        }
    }
}
