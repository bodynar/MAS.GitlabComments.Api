namespace MAS.GitlabComments.Data.Tests.MsSqlFilterBuilderTests
{
    using System;

    using MAS.GitlabComments.Data.Filter;

    using Xunit;

    public sealed class BuildTests : BaseMsSqlFilterBuilderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsNull()
        {
            FilterGroup filter = null;

            Exception exception =
                Record.Exception(
                    () => TestedService.Build(filter)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenFilterIsEmpty()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                LogicalJoinType = FilterJoinType.None
            };

            Exception exception =
                Record.Exception(
                    () => TestedService.Build(filter)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldNotBuildFilterWhenItemsHaveNotValidComparisonTypes()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = "TestValue",
                        LogicalComparisonType = ComparisonType.None
                    }
                }
            };

            var (sql, _) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.Equal(0, sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilterWhenNestedGroupsAreEmpty()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "NestedFilter1",
                        NestedGroups = new[]
                        {
                            new FilterGroup
                            {
                                Name = "NestedFilter2",
                                NestedGroups = new[]
                                {
                                    new FilterGroup
                                    {
                                        Name = "NestedFilter3",
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var (sql, _) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.Equal(0, sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFulterWhenJoinTypeIsInvalid()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                LogicalJoinType = FilterJoinType.None,
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "NestedFilter1",
                        Items = new[] {
                            new FilterItem
                            {
                                FieldName = "TestFieldName1",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = "TestValue1"
                            }
                        },
                    },
                    new FilterGroup
                    {
                        Name = "NestedFilter2",
                        Items = new[] {
                            new FilterItem
                            {
                                FieldName = "TestFieldName2",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = "TestValue2"
                            }
                        },
                    }
                }
            };

            var (sql, _) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.Equal(0, sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilterWhenNestedGroupsContainsItemsWithInvalidComparisonTypes()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                LogicalJoinType = FilterJoinType.And,
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "NestedFilter1",
                        LogicalJoinType = FilterJoinType.And,
                        NestedGroups = new[]
                        {
                            new FilterGroup
                            {
                                Name = "NestedFilter3",
                                LogicalJoinType = FilterJoinType.And,
                                Items = new[]
                                {
                                    new FilterItem
                                    {
                                        FieldName = "TestFieldName",
                                        Name = "TestFilter",
                                        Value = "TestValue",
                                        LogicalComparisonType = ComparisonType.None
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var (sql, _) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.Equal(0, sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilterWhenJoinOperatorIsInvalid()
        {
            FilterGroup filter = new()
            {
                Name = "EmptyFilter",
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = "TestValue",
                        LogicalComparisonType = ComparisonType.Equal
                    }
                }
            }; ;

            var (sql, _) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.Equal(0, sql.Length);
        }

        #region Proper filter build

        #region Comparisons

        [Fact]
        public void ShouldBuildFilterWithEqualComparison()
        {
            var expectedSql = "[TestFieldName] = @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "EqualComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithNotEqualComparison()
        {
            var expectedSql = "[TestFieldName] != @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "NotEqualComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.NotEqual
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithLessComparison()
        {
            var expectedSql = "[TestFieldName] < @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "LessComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Less
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithLessOrEqualComparison()
        {
            var expectedSql = "[TestFieldName] <= @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "LessOrEqualComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.LessOrEqual
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithGreaterComparison()
        {
            var expectedSql = "[TestFieldName] > @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "GreaterComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Greater
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithGreaterOrEqualComparison()
        {
            var expectedSql = "[TestFieldName] >= @FilterValue0";
            var expectedParamsCount = 1;
            var expectedParamNames = new[] { "FilterValue0" };
            var expectedParamValues = new[] { true };
            FilterGroup filter = new()
            {
                Name = "GreaterOrEqualComparison",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.GreaterOrEqual
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamsCount, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        #endregion

        #region Nesting

        [Fact]
        public void ShouldBuildFilterFromFlatModelWithSeveralComparisons()
        {
            var expectedSql = "[TestFieldName1] = @FilterValue0" + nl + "AND [TestFieldName2] = @FilterValue1";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1" };
            var expectedParamValues = new object[] { true, true };
            FilterGroup filter = new()
            {
                Name = "FlatModelWithSeveralComparisons",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName1",
                        Name = "TestFilter1",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    },
                    new FilterItem
                    {
                        FieldName = "TestFieldName2",
                        Name = "TestFilter2",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    }
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamNames.Length, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromFewGroupsWithFlatFilters()
        {
            var expectedSql = "([TestFieldName1] = @FilterValue0)" + nl + "AND ([TestFieldName2] = @FilterValue1)";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1" };
            var expectedParamValues = new object[] { true, true };
            FilterGroup filter = new()
            {
                Name = "FewGroupsWithFlatFilters",
                LogicalJoinType = FilterJoinType.And,
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "Group1",
                        LogicalJoinType = FilterJoinType.And,
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "TestFieldName1",
                                Name = "TestFilter1",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            },
                        }
                    },
                    new FilterGroup
                    {
                        Name = "Group2",
                        LogicalJoinType = FilterJoinType.And,
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "TestFieldName2",
                                Name = "TestFilter2",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            }
                        }
                    },
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamNames.Length, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromFewGroupsWithSeveralComparisons()
        {
            var expectedSql = "([TestFieldName1] = @FilterValue0" + nl + "OR [TestFieldName2] = @FilterValue1)"
                + nl + "AND ([TestFieldName3] = @FilterValue2" + nl + "AND [TestFieldName4] = @FilterValue3)";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1", "FilterValue2", "FilterValue3" };
            var expectedParamValues = new object[] { true, true, true, true };
            FilterGroup filter = new()
            {
                Name = "FewGroupsWithSeveralComparisons",
                LogicalJoinType = FilterJoinType.And,
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "Group1",
                        LogicalJoinType = FilterJoinType.Or,
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "TestFieldName1",
                                Name = "TestFilter1",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            },
                            new FilterItem
                            {
                                FieldName = "TestFieldName2",
                                Name = "TestFilter2",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            },
                        }
                    },
                    new FilterGroup
                    {
                        Name = "Group2",
                        LogicalJoinType = FilterJoinType.And,
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "TestFieldName3",
                                Name = "TestFilter3",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            },
                            new FilterItem
                            {
                                FieldName = "TestFieldName4",
                                Name = "TestFilter4",
                                Value = true,
                                LogicalComparisonType = ComparisonType.Equal
                            },
                        }
                    },
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamNames.Length, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromSingleDeepFilterWithSeveralComparisons()
        {
            var expectedSql = "[TestFieldName1] = @FilterValue0" + nl + "OR [TestFieldName2] = @FilterValue1" + nl + "OR [TestFieldName3] = @FilterValue2" + nl + "OR [TestFieldName4] = @FilterValue3";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1", "FilterValue2", "FilterValue3" };
            var expectedParamValues = new object[] { true, true, true, true };
            FilterGroup filter = new()
            {
                Name = "SingleDeepFilterWithSeveralComparisons",
                LogicalJoinType = FilterJoinType.Or,
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Name = "Group1",
                        LogicalJoinType = FilterJoinType.Or,
                        NestedGroups = new[]
                        {
                            new FilterGroup
                            {
                                Name = "Group2",
                                LogicalJoinType = FilterJoinType.Or,
                                NestedGroups = new[]
                                {
                                    new FilterGroup
                                    {
                                        Name = "Group3",
                                        LogicalJoinType = FilterJoinType.Or,
                                        NestedGroups = new[]
                                        {
                                            new FilterGroup
                                            {
                                                Name = "Group4",
                                                LogicalJoinType = FilterJoinType.Or,
                                                NestedGroups = new[]
                                                {
                                                    new FilterGroup
                                                    {
                                                        Name = "Group5",
                                                        LogicalJoinType = FilterJoinType.Or,
                                                        Items = new[]
                                                        {
                                                            new FilterItem
                                                            {
                                                                FieldName = "TestFieldName1",
                                                                Name = "TestFilter1",
                                                                Value = true,
                                                                LogicalComparisonType = ComparisonType.Equal
                                                            },
                                                            new FilterItem
                                                            {
                                                                FieldName = "TestFieldName2",
                                                                Name = "TestFilter2",
                                                                Value = true,
                                                                LogicalComparisonType = ComparisonType.Equal
                                                            },
                                                            new FilterItem
                                                            {
                                                                FieldName = "TestFieldName3",
                                                                Name = "TestFilter3",
                                                                Value = true,
                                                                LogicalComparisonType = ComparisonType.Equal
                                                            },
                                                            new FilterItem
                                                            {
                                                                FieldName = "TestFieldName4",
                                                                Name = "TestFilter4",
                                                                Value = true,
                                                                LogicalComparisonType = ComparisonType.Equal
                                                            },
                                                        }
                                                    },
                                                }
                                            },
                                        }
                                    },
                                }
                            },
                        }
                    },
                }
            };

            var (sql, args) = TestedService.Build(filter);

            Assert.NotNull(sql);
            Assert.NotEqual(0, sql.Length);
            Assert.Equal(expectedSql, sql);

            Assert.NotNull(args);
            Assert.NotEmpty(args);
            Assert.Equal(expectedParamNames.Length, args.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, args.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, args.Values, (e, a) => Assert.Equal(e, a));
        }

        #endregion

        #endregion
    }
}
