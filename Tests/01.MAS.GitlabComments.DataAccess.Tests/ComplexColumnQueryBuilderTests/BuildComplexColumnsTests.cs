namespace MAS.GitlabComments.DataAccess.Tests.ComplexColumnMssqlBuilderTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.DataAccess.Attributes;
    using MAS.GitlabComments.DataAccess.Select;
    using MAS.GitlabComments.DataAccess.Services.Implementations;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="ComplexColumnQueryBuilder.BuildComplexColumns{TProjection}(string)"/>
    /// </summary>
    public sealed class BuildComplexColumnsTests
    {
        /// <summary>
        /// Instance of <see cref="ComplexColumnQueryBuilder"/> for tests
        /// </summary>
        private ComplexColumnQueryBuilder TestedService { get; }

        /// <summary> Mock table name for tests </summary>
        private const string SourceTableName = "SourceTableName";

        /// <summary>
        /// Initializing <see cref="BuildComplexColumnsTests"/> with setup'n all required environment
        /// </summary>
        public BuildComplexColumnsTests()
        {
            TestedService = new ComplexColumnQueryBuilder();
        }

        private class EmptyProjectedClass { }

        [Fact]
        public void ShouldReturnNull_WhenTableAliasIsEmpty()
        {
            var result = TestedService.BuildComplexColumns<EmptyProjectedClass>(string.Empty);

            Assert.Null(result);
        }

        [Fact]
        public void ShouldReturnNull_WhenTableAliasIsNull()
        {
            var result = TestedService.BuildComplexColumns<EmptyProjectedClass>(null);

            Assert.Null(result);
        }

        [Fact]
        public void ShouldReturnNull_WhenProjectedTypeHasNoColumns()
        {
            var result = TestedService.BuildComplexColumns<EmptyProjectedClass>(string.Empty);

            Assert.Null(result);
        }

        #region Empty Data When Projected Model Has Empty Attributes

        [Fact]
        public void ShouldReturnEmptyData_WhenProjectedModelHasEmptyAttributes()
        {
            var expectedColumn = $"[{SourceTableName}].[SimplePath]";
            var result = TestedService.BuildComplexColumns<ProjectedClassWithEmptyAttributes>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault().ToQuery(DatabaseType.MSSQL));
        }

        private class ProjectedClassWithEmptyAttributes
        {
            public string SimplePath { get; set; }
        }

        #endregion

        #region Empty Data When Projected Type Has Column With Invalid Path Only From Joins

        [Fact]
        public void ShouldReturnEmptyData_WhenProjectedTypeHasColumnWithInvalidPathOnlyFromJoins()
        {
            var result = TestedService.BuildComplexColumns<ProjectedClassWithInvalidPathFromJoins>("ProjectedClassWithInvalidPathFromJoins");

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

        #region Empty Data When Incorrect Attribute

        [Fact]
        public void ShouldBuildDataWithoutJoinData()
        {
            var result = TestedService.BuildComplexColumns<ProjectedClassWithSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.Empty(result.Columns);
            Assert.Empty(result.Joins);
        }

        private class ProjectedClassWithSimplePath
        {
            [ComplexColumnPath("SimplePath")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Empty Data When Projected Type Has Column With Invalid Path From Simple Column And Join

        [Fact]
        public void ShouldReturnEmptyData_WhenProjectedTypeHasColumnWithInvalidPathFromSimpleColumnAndJoin()
        {
            var result = TestedService.BuildComplexColumns<ProjectedClassWithInvalidPathFromJoins>(SourceTableName);

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

        #region Build Data With Single Join Data

        [Fact]
        public void ShouldBuildDataWithSingleJoinData()
        {
            var expectedTableAlias = "RigTab1";
            var expectedName = "Value";
            var expectedAlias = "SimplePath";
            var expectedColumnsCount = 1;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            var firstColumn = result.Columns.FirstOrDefault();

            Assert.Equal(expectedTableAlias, firstColumn.TableAlias);
            Assert.Equal(expectedName, firstColumn.Name);
            Assert.Equal(expectedAlias, firstColumn.Alias);
            Assert.True(expectedJoinData.Equals(result.Joins.FirstOrDefault()));
        }

        private class ProjectedClassWithComplexPath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region Build Data With Single Join Data And Primitive Column

        [Fact]
        public void ShouldBuildDataWithSingleJoinDataAndPrimitiveColumn()
        {
            var expectedColumns = new[] {
                new Tuple<string, string, string>(SourceTableName, "SimplePath", null),
                new Tuple<string, string, string>("RigTab1", "Value", "ComplexPath"),
            };
            var expectedColumnsCount = 2;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathAndPrimitivePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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

        #region Build Data With Single Join Data

        [Fact]
        public void ShouldBuildDataWithSingleJoinDataAndSimpleColumn()
        {
            var expectedColumns = new[] {
                new Tuple<string, string, string>("RigTab1", "Value", "ComplexPath"),
            };
            var expectedColumnsCount = 1;
            var expectedJoinsCount = 1;
            var expectedJoinData = new TableJoinData("[RightTable:RightTableColumn:LeftTableColumn]");

            var result = TestedService.BuildComplexColumns<ProjectedClassWithComplexPathAndSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.NotEmpty(result.Joins);

            Assert.Equal(expectedColumnsCount, result.Columns.Count());
            Assert.Equal(expectedJoinsCount, result.Joins.Count());

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("GenTab1", "Value", "ComplexColumn1"),
                new Tuple<string, string, string>("RanTab1", "Sum", "ComplexColumn2"),
                new Tuple<string, string, string>("SpeTab1", "Average", "ComplexColumn3"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("GenTab1", "Value", "ComplexColumn1"),
                new Tuple<string, string, string>("RanTab1", "Sum", "ComplexColumn2"),
                new Tuple<string, string, string>("GenTab1", "Average", "ComplexColumn3"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("RigTab1", "Value", "ComplexColumn1"),
                new Tuple<string, string, string>("RigTab2", "Sum", "ComplexColumn2"),
                new Tuple<string, string, string>("RigTab3", "Average", "ComplexColumn3"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("MidTab1", "Value", "SimplePath"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("MidTab1", "Value", "Column"),
                new Tuple<string, string, string>("MidTab1", "Average", "Path"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("MidTab1", "Value", "Column"),
                new Tuple<string, string, string>("AnoTesTab1", "Average", "Path"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
            var expectedColumns = new[] {
                new Tuple<string, string, string>("MidTab1", "Value", "Column"),
                new Tuple<string, string, string>("MidTab1", "Average", "Path"),
            };
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

            CommonAssert.Collections(
                expectedColumns,
                result.Columns,
                (expected, actual) =>
                {
                    Assert.Equal(expected.Item1, actual.TableAlias);
                    Assert.Equal(expected.Item2, actual.Name);

                    if (expected.Item3 != null)
                    {
                        Assert.Equal(expected.Item3, actual.Alias);
                    }
                }
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
