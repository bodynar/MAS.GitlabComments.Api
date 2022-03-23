namespace MAS.GitlabComments.Data.Tests.ComplexColumnMssqlBuilderTests
{
    using System.Linq;

    using MAS.GitlabComments.Data.Attributes;
    using MAS.GitlabComments.Data.Select;
    using MAS.GitlabComments.Data.Services;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="ComplexColumnMssqlBuilder.BuildComplexColumns{TProjection}(string)"/>
    /// </summary>
    public sealed class BuildComplexColumnsTests
    {
        /// <summary>
        /// Instance of <see cref="ComplexColumnMssqlBuilder"/> for tests
        /// </summary>
        private ComplexColumnMssqlBuilder TestedService { get; }

        /// <summary> Mock table name for tests </summary>
        private const string SourceTableName = "SourceTableName";

        /// <summary>
        /// Initializing <see cref="BuildComplexColumnsTests"/> with setup'n all required environment
        /// </summary>
        public BuildComplexColumnsTests()
        {
            TestedService = new ComplexColumnMssqlBuilder();
        }

        #region Null When Projected Type Has No Columns

        [Fact]
        public void ShouldReturnNullWhenProjectedTypeHasNoColumns()
        {
            var result = TestedService.BuildComplexColumns<EmptyProjectedClass>(string.Empty);

            Assert.Null(result);
        }

        private class EmptyProjectedClass { }

        #endregion

        #region Empty Data When Projected Model Has Empty Attributes

        [Fact]
        public void ShouldReturnEmptyDataWhenProjectedModelHasEmptyAttributes()
        {
            var expectedColumn = $"[{SourceTableName}].[SimplePath]";
            var result = TestedService.BuildComplexColumns<ProjectedClassWithEmptyAttributes>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault().ToString());
        }

        private class ProjectedClassWithEmptyAttributes
        {
            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data Without Join Data

        [Fact]
        public void ShouldBuildDataWithoutJoinData()
        {
            var expectedColumn = $"[{SourceTableName}].[SimplePath]";
            var result = TestedService.BuildComplexColumns<ProjectedClassWithSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault().ToString());
        }

        private class ProjectedClassWithSimplePath
        {
            [ComplexColumnPath("SimplePath")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data With Single Join Data

        [Fact]
        public void ShouldBuildDataWithSingleJoinData()
        {
            var expectedColumn = "[Right1].[Value] AS [SimplePath]";
            var expectedColumnsCount = 1;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault().ToString());
            Assert.True(expectedJoinData.Equals(result.Joins.FirstOrDefault()));
        }

        private class ProjectedClassWithComplexPath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data With Single Join Data And PrimitiveColumn

        [Fact]
        public void ShouldBuildDataWithSingleJoinDataAndPrimitiveColumn()
        {
            var expectedColumns = new[] { $"[{SourceTableName}].[SimplePath]", "[Right1].[Value] AS [ComplexPath]" };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathAndPrimitivePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.CollectionsWithSameType(expectedColumns, result.Columns.Select(x => x.ToString()),
                (expected, actual) => Assert.Equal(expected, actual)
            );
            Assert.True(expectedJoinData.Equals(result.Joins.FirstOrDefault()));
        }

        private class ProjectedClassWithComplexPathAndPrimitivePath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string ComplexPath { get; set; }

            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data With Single Join Data And Simple Column

        [Fact]
        public void ShouldBuildDataWithSingleJoinDataAndSimpleColumn()
        {
            var expectedColumns = new[] { "[Right1].[Value] AS [ComplexPath]", $"[{SourceTableName}].[SimplePath] AS [SimpleColumn]" };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathAndSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.CollectionsWithSameType(expectedColumns, result.Columns.Select(x => x.ToString()),
                (expected, actual) => Assert.Equal(expected, actual)
            );
            Assert.True(expectedJoinData.Equals(result.Joins.FirstOrDefault()));
        }

        private class ProjectedClassWithComplexPathAndSimplePath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string ComplexPath { get; set; }

            [ComplexColumnPath("SimplePath")]
            public string SimpleColumn { get; set; }
        }

        #endregion

        #region Build Data With Several Different Join Data

        [Fact]
        public void ShouldBuildDataWithSeveralDifferentJoinData()
        {
            var expectedColumns = new[] { "[Generic1].[Value] AS [ComplexColumn1]", "[Random1].[Sum] AS [ComplexColumn2]", "[Special1].[Average] AS [ComplexColumn3]" };
            var expectedColumnsCount = 3;
            var expectedJoinsCount = 3;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[GenericTable:GenericTableColumn:LeftTableGenericTableColumn]"),
                new TableJoinData("[RandomTable:RandomTableColumn:LeftTableRandomTableColumn]"),
                new TableJoinData("[SpecialTable:SpecialTableColumn:LeftTableSpecialTableColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithSeveralDifferentComplexPath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.CollectionsWithSameType(expectedColumns, result.Columns.Select(x => x.ToString()),
                (expected, actual) => Assert.Equal(expected, actual)
            );
            CommonAssert.CollectionsWithSameType(expectedJoinDataItems, result.Joins,
                (expected, actual) => Assert.True(expected.Equals(actual))
            );
        }

        private class ProjectedClassWithSeveralDifferentComplexPath
        {
            [ComplexColumnPath("[GenericTable:GenericTableColumn:LeftTableGenericTableColumn].Value")]
            public string ComplexColumn1 { get; set; }

            [ComplexColumnPath("[RandomTable:RandomTableColumn:LeftTableRandomTableColumn].Sum")]
            public string ComplexColumn2 { get; set; }

            [ComplexColumnPath("[SpecialTable:SpecialTableColumn:LeftTableSpecialTableColumn].Average")]
            public string ComplexColumn3 { get; set; }
        }

        #endregion

        #region Build Data With Several Different And Same Join Data

        [Fact]
        public void ShouldBuildDataWithSeveralDifferentAndSameJoinData()
        {
            var expectedColumns = new[] { "[Generic1].[Value] AS [ComplexColumn1]", "[Random1].[Sum] AS [ComplexColumn2]", "[Generic1].[Average] AS [ComplexColumn3]" };
            var expectedColumnsCount = 3;
            var expectedJoinsCount = 2;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[GenericTable:GenericTableColumn:LeftTableGenericTableColumn]"),
                new TableJoinData("[RandomTable:RandomTableColumn:LeftTableRandomTableColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithSeveralDifferentAndSameComplexPath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.CollectionsWithSameType(expectedColumns, result.Columns.Select(x => x.ToString()),
                (expected, actual) => Assert.Equal(expected, actual)
            );
            CommonAssert.CollectionsWithSameType(expectedJoinDataItems, result.Joins,
                (expected, actual) => Assert.True(expected.Equals(actual))
            );
        }

        private class ProjectedClassWithSeveralDifferentAndSameComplexPath
        {
            [ComplexColumnPath("[GenericTable:GenericTableColumn:LeftTableGenericTableColumn].Value")]
            public string ComplexColumn1 { get; set; }

            [ComplexColumnPath("[RandomTable:RandomTableColumn:LeftTableRandomTableColumn].Sum")]
            public string ComplexColumn2 { get; set; }

            [ComplexColumnPath("[GenericTable:GenericTableColumn:LeftTableGenericTableColumn].Average")]
            public string ComplexColumn3 { get; set; }
        }

        #endregion

        #region Build Data With Join Data For Same Table But Different Ways To Join

        [Fact]
        public void ShouldBuildDataWithJoinDataForSameTableButDifferentWaysToJoin()
        {
            var expectedColumns = new[] { "[Right1].[Value] AS [ComplexColumn1]", "[Right2].[Sum] AS [ComplexColumn2]", "[Right3].[Average] AS [ComplexColumn3]" };
            var expectedColumnsCount = 3;
            var expectedJoinsCount = 3;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]"),
                new TableJoinData("[RightTable:RandomTableColumn:LeftTableRandomTableColumn]"),
                new TableJoinData("[RightTable:SpecialTableColumn:LeftTableSpecialTableColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithSeveralDifferentComplexPathForSameTable>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.CollectionsWithSameType(expectedColumns, result.Columns.Select(x => x.ToString()),
                (expected, actual) => Assert.Equal(expected, actual)
            );
            CommonAssert.CollectionsWithSameType(expectedJoinDataItems, result.Joins,
                (expected, actual) => Assert.True(expected.Equals(actual))
            );
        }

        private class ProjectedClassWithSeveralDifferentComplexPathForSameTable
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string ComplexColumn1 { get; set; }

            [ComplexColumnPath("[RightTable:RandomTableColumn:LeftTableRandomTableColumn].Sum")]
            public string ComplexColumn2 { get; set; }

            [ComplexColumnPath("[RightTable:SpecialTableColumn:LeftTableSpecialTableColumn].Average")]
            public string ComplexColumn3 { get; set; }
        }

        #endregion

        /*
         * TODO:
         * 1. Several joins in single attribute
         * 2. Several joins with same table in few attributes
         * 3. Several joins with same table but different ways to join in few attributes
         * ...
         * 4. Column with invalid path [Table1:Table1Column:Table2Column].[Table1:Table1Column:Table2Column]
         * 5. Column with invalid path SomeColumn.[Table1:Table1Column:Table2Column]
         * N. <more>
         */

    }
}
