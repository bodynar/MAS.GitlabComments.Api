namespace MAS.GitlabComments.DataAccess.Tests.MsSqlFilterBuilderTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG;
    using MAS.GitlabComments.DataAccess.Tests.PgSqlFilterBuilderTests;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="PgSqlFilterBuilder.Build(FilterGroup queryFilterGroup)"/>
    /// </summary>
    public sealed class BuildTests : BasePgSqlFilterBuilderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenFilterIsNull()
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
        public void ShouldThrowArgumentNullException_WhenFilterIsEmpty()
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
        public void ShouldNotBuildFilter_WhenItemsHaveNotValidComparisonTypes()
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.Equal(0, result.Sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilter_WhenNestedGroupsAreEmpty()
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.Equal(0, result.Sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilter_WhenJoinTypeIsInvalid()
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.Equal(0, result.Sql.Length);
        }

        [Fact]
        public void ShouldNotBuildFilter_WhenNestedGroupsContainsItemsWithInvalidComparisonTypes()
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.Equal(0, result.Sql.Length);
        }

        #region Proper filter build

        #region Comparisons

        [Fact]
        public void ShouldBuildFilterWithEqualComparison()
        {
            var expectedSql = "\"TestFieldName\" = @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithNotEqualComparison()
        {
            var expectedSql = "\"TestFieldName\" != @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithLessComparison()
        {
            var expectedSql = "\"TestFieldName\" < @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithLessOrEqualComparison()
        {
            var expectedSql = "\"TestFieldName\" <= @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithGreaterComparison()
        {
            var expectedSql = "\"TestFieldName\" > @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterWithGreaterOrEqualComparison()
        {
            var expectedSql = "\"TestFieldName\" >= @FilterValue0";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        #endregion

        #region Join type

        [Fact]
        public void ShouldBuildAndJoinType()
        {
            var expectedSql = "\"TestFieldName1\" = @FilterValue0 AND \"TestFieldName2\" = @FilterValue1";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1" };
            var expectedParamValues = new[] { true, true };
            FilterGroup filter = new()
            {
                Name = "AndJoinType",
                LogicalJoinType = FilterJoinType.And,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName1",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    },
                    new FilterItem
                    {
                        FieldName = "TestFieldName2",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    },
                }
            };

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildOrJoinType()
        {
            var expectedSql = "\"TestFieldName1\" = @FilterValue0 OR \"TestFieldName2\" = @FilterValue1";
            var expectedParamNames = new[] { "FilterValue0", "FilterValue1" };
            var expectedParamValues = new[] { true, true };
            FilterGroup filter = new()
            {
                Name = "OrJoinType",
                LogicalJoinType = FilterJoinType.Or,
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "TestFieldName1",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    },
                    new FilterItem
                    {
                        FieldName = "TestFieldName2",
                        Name = "TestFilter",
                        Value = true,
                        LogicalComparisonType = ComparisonType.Equal
                    },
                }
            };

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        #endregion

        #region Nesting

        [Fact]
        public void ShouldBuildFilterFromFlatModelWithSeveralComparisons()
        {
            var expectedSql = "\"TestFieldName1\" = @FilterValue0 AND \"TestFieldName2\" = @FilterValue1";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromFewGroupsWithFlatFilters()
        {
            var expectedSql = "(\"TestFieldName1\" = @FilterValue0) AND (\"TestFieldName2\" = @FilterValue1)";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromFewGroupsWithSeveralComparisons()
        {
            var expectedSql = "(\"TestFieldName1\" = @FilterValue0 OR \"TestFieldName2\" = @FilterValue1)"
                + " AND (\"TestFieldName3\" = @FilterValue2 AND \"TestFieldName4\" = @FilterValue3)";
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

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromSingleDeepFilterWithSeveralComparisons()
        {
            var expectedSql = "\"TestFieldName1\" = @FilterValue0 OR \"TestFieldName2\" = @FilterValue1 OR \"TestFieldName3\" = @FilterValue2 OR \"TestFieldName4\" = @FilterValue3";
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


            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldBuildFilterFromGroupsInDifferentLevels()
        {
            var expectedSql = "(\"TestFieldName1\" = @FilterValue0 AND \"TestFieldName2\" = @FilterValue1) OR (\"TestFieldName3\" = @FilterValue2 OR \"TestFieldName4\" = @FilterValue3)";
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
                                                            },
                                                        }
                                                    },
                                                }
                                            },
                                        }
                                    },
                                }
                            },
                            new FilterGroup
                            {
                                Name = "Group6",
                                LogicalJoinType = FilterJoinType.Or,
                                Items = new[]
                                {
                                    new FilterItem
                                    {
                                        FieldName = "TestFieldName3",
                                        Name = "TestFilter1",
                                        Value = true,
                                        LogicalComparisonType = ComparisonType.Equal
                                    },
                                    new FilterItem
                                    {
                                        FieldName = "TestFieldName4",
                                        Name = "TestFilter2",
                                        Value = true,
                                        LogicalComparisonType = ComparisonType.Equal
                                    },
                                }
                            }
                        }
                    },
                }
            };

            var result = TestedService.Build(filter);

            Assert.NotNull(result);
            Assert.NotNull(result.Sql);
            Assert.NotEqual(0, result.Sql.Length);
            Assert.Equal(expectedSql, result.Sql);

            Assert.NotNull(result.Values);
            Assert.NotEmpty(result.Values);
            Assert.Equal(expectedParamNames.Length, result.Values.Count);
            CommonAssert.CollectionsWithSameType(expectedParamNames, result.Values.Keys, (e, a) => Assert.Equal(e, a));
            CommonAssert.Collections(expectedParamValues, result.Values.Values, (e, a) => Assert.Equal(e, a));
        }

        #endregion

        #endregion
    }
}
