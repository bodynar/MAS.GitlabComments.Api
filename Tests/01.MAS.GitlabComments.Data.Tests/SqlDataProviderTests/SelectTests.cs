namespace MAS.GitlabComments.Data.Tests.SqlDataProviderTests
{
    using System;

    using MAS.GitlabComments.Data.Filter;
    using MAS.GitlabComments.Data.Select;
    using MAS.GitlabComments.Data.Services.Implementations;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="SqlDataProvider{TEntity}.Select{TProjection}(Select.SelectConfiguration)"/>
    /// </summary>
    public sealed class SelectTests : BaseSqlDataProviderTests
    {
        public class EmptyProjectedClass { }

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

            SelectConfiguration configuration = new SelectConfiguration
            {
                Filter = filter,
            };

            var exception = Record.Exception(
                () => TestedService.Select<EmptyProjectedClass>(configuration)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
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
        public void ShouldBuildQueryWithSpecificColumnWhenProjectedModelHasRequiredAttributes()
        {
            var expectedSql = $"SELECT [Table1].[Column1] FROM [{TestedTableName}]";

            SelectConfiguration configuration = new() { };
            ComplexColumnQueryBuilderResult = new ComplexColumnData()
            {
                Columns = new[]
                {
                    new ComplexColumn { Name = "Column1", TableAlias = "Table1" }
                },
            };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }

        [Fact]
        public void ShouldBuildQueryWithSpecificColumnsWhenProjectedModelHasRequiredAttributes()
        {
            var expectedSql = $"SELECT [Table1].[Column1],{Environment.NewLine}[Table2].[Column2] FROM [{TestedTableName}]";

            SelectConfiguration configuration = new() { };
            ComplexColumnQueryBuilderResult = new ComplexColumnData()
            {
                Columns = new[]
                {
                    new ComplexColumn { Name = "Column1", TableAlias = "Table1" },
                    new ComplexColumn { Name = "Column2", TableAlias = "Table2" },
                },
            };

            TestedService.Select<EmptyProjectedClass>(configuration);
            var lastQuery = LastQuery;

            Assert.NotNull(lastQuery);
            Assert.Equal(expectedSql, lastQuery.Value.Key);
        }
    }
}
