namespace MAS.GitlabComments.DataAccess.Tests.ComplexColumnMssqlBuilderTests
{
    using System.Linq;

    using MAS.GitlabComments.DataAccess.Attributes;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services.Implementations;

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

        #region Empty Data When Projected Type Has Column With Invalid Path Only From Joins

        [Fact]
        public void ShouldReturnEmptyDataWhenProjectedTypeHasColumnWithInvalidPathOnlyFromJoins()
        {
            var result = TestedService.BuildComplexColumns<ProjectedClassWithInvalidPathFromJoins>(string.Empty);

            Assert.NotNull(result);
            Assert.Empty(result.Columns);
            Assert.Empty(result.Joins);
        }

        private class ProjectedClassWithInvalidPathFromJoins
        {
            [ComplexColumnPath("[Table1:Table1Column:Table2Column].[Table1:Table1Column:Table2Column]")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Empty Data When Projected Type Has Column With Invalid Path From Simple Column And Join

        [Fact]
        public void ShouldReturnEmptyDataWhenProjectedTypeHasColumnWithInvalidPathFromSimpleColumnAndJoin()
        {
            var result = TestedService.BuildComplexColumns<ProjectedClassWithInvalidPathFromJoins>(string.Empty);

            Assert.NotNull(result);
            Assert.Empty(result.Columns);
            Assert.Empty(result.Joins);
        }

        private class ProjectedClassWithInvalidPathFromSimpleColumnAndJoin
        {
            [ComplexColumnPath("SomeColumn.[Table1:Table1Column:Table2Column]")]
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

        #region Build Data With Several Join Data In Single Attribute

        [Fact]
        public void ShouldBuildDataWithSeveralJoinDataInSingleAttribute()
        {
            var expectedColumns = new[] { "[Middle1].[Value] AS [SimplePath]" };
            var expectedColumnsCount = 1;
            var expectedJoinsCount = 2;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]"),
                new TableJoinData("[MiddleTable:MiddleTableColumn:RightTableMiddleColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathWithSeveralJoinsInSingleAttribute>(SourceTableName);

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

        private class ProjectedClassWithComplexPathWithSeveralJoinsInSingleAttribute
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Value")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data With Several Join Data In Several Attribute With Same Join Path

        [Fact]
        public void ShouldBuildDataWithSeveralJoinDataInSeveralAttributeWithSameJoinPath()
        {
            var expectedColumns = new[] { "[Middle1].[Value] AS [Column]", "[Middle1].[Average] AS [Path]", };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 2;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]"),
                new TableJoinData("[MiddleTable:MiddleTableColumn:RightTableMiddleColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithSameJoinPath>(SourceTableName);

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

        private class ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithSameJoinPath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Value")]
            public string Column { get; set; }

            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Average")]
            public string Path { get; set; }
        }

        #endregion

        #region Build Data With Several Join Data In Several Attribute With Same Join Path

        [Fact]
        public void ShouldBuildDataWithSeveralJoinDataInSeveralAttributeWithDifferentJoinPath()
        {
            var expectedColumns = new[] { "[Middle1].[Value] AS [Column]", "[Another1].[Average] AS [Path]", };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 4;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]"),
                new TableJoinData("[MiddleTable:MiddleTableColumn:RightTableMiddleColumn]"),

                new TableJoinData("[TestTable:TestTableColumn:LeftTableColumn]"),
                new TableJoinData("[AnotherTestTable:AnotherTestTableColumn:AnotherTestTestColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithDifferentJoinPath>(SourceTableName);

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

        private class ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithDifferentJoinPath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Value")]
            public string Column { get; set; }

            [ComplexColumnPath("[TestTable:TestTableColumn:LeftTableColumn].[AnotherTestTable:AnotherTestTableColumn:AnotherTestTestColumn].Average")]
            public string Path { get; set; }
        }

        #endregion

        #region Build Data With Several Join Data In Several Attribute With Same Table But Different Join Params

        // TODO: see ComplexColumnMssqlBuilder TODO p.1

        [Fact]
        public void ShouldBuildDataWithSeveralJoinDataInSeveralAttributeWithSameTableButDifferentJoinParams()
        {
            var expectedColumns = new[] { "[Middle1].[Value] AS [Column]", "[Middle1].[Average] AS [Path]", };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 3;
            var expectedJoinDataItems = new[] {
                new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]"),
                new TableJoinData("[MiddleTable:MiddleTableColumn:RightTableMiddleColumn]"),

                new TableJoinData("[RightTable:RightTableExtraColumn:LeftTableExtraColumn]"),
            };

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithSameTableButDifferentJoinParams>(SourceTableName);

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

        private class ProjectedClassWithComplexPathWithSeveralJoinsInSeveralAttributeWithSameTableButDifferentJoinParams
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Value")]
            public string Column { get; set; }

            [ComplexColumnPath("[RightTable:RightTableExtraColumn:LeftTableExtraColumn].[MiddleTable:MiddleTableColumn:RightTableMiddleColumn].Average")]
            public string Path { get; set; }
        }

        #endregion
    }
}
