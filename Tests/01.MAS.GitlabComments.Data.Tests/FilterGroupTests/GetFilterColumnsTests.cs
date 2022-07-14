namespace MAS.GitlabComments.Data.Tests.FilterGroupTests
{
    using System.Linq;

    using MAS.GitlabComments.Data.Filter;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="FilterGroup.GetFilterColumns"/>
    /// </summary>
    public sealed class GetFilterColumnsTests
    {
        [Fact]
        public void ShouldReturnEmptyResultWhenFilterContainsNull()
        {
            FilterGroup testedFilter = new()
            {
                Items = null,
                NestedGroups = null
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ShouldReturnEmptyResultWhenFilterContainsEmptyProps()
        {
            FilterGroup testedFilter = new()
            {
                Items = Enumerable.Empty<FilterItem>(),
                NestedGroups = Enumerable.Empty<FilterGroup>()
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ShouldReturnColumnNamesFromItems()
        {
            var expectedColumnNames = new[] { "Column1", "Column2" };
            FilterGroup testedFilter = new()
            {
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "Column1",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c1",
                    },
                    new FilterItem
                    {
                        FieldName = "Column2",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c2",
                    },
                },
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            CommonAssert.CollectionsWithSameType(expectedColumnNames, result, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldReturnColumnNamesFromNestedGroups()
        {
            var expectedColumnNames = new[] { "Column1", "Column2", "Column3", "Column4" };
            FilterGroup testedFilter = new()
            {
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column1",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column2",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column3",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column4",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                }
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            CommonAssert.CollectionsWithSameType(expectedColumnNames, result, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldReturnColumnNamesFromItemsAndNestedGroups()
        {
            var expectedColumnNames = new[] { "Column01", "Column02", "Column1", "Column2", "Column3", "Column4" };
            FilterGroup testedFilter = new()
            {
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "Column01",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c1",
                    },
                    new FilterItem
                    {
                        FieldName = "Column02",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c2",
                    },
                },
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column1",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column2",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column3",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column4",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                }
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            CommonAssert.CollectionsWithSameType(expectedColumnNames, result, (e, a) => Assert.Equal(e, a));
        }

        [Fact]
        public void ShouldReturnDistinctedColumnNames()
        {
            var expectedColumnNames = new[] { "Column" };
            FilterGroup testedFilter = new()
            {
                Items = new[]
                {
                    new FilterItem
                    {
                        FieldName = "Column",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c1",
                    },
                    new FilterItem
                    {
                        FieldName = "Column",
                        LogicalComparisonType = ComparisonType.Equal,
                        Value = true,
                        Name = "c2",
                    },
                },
                NestedGroups = new[]
                {
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                    new FilterGroup
                    {
                        Items = new[]
                        {
                            new FilterItem
                            {
                                FieldName = "Column",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c1",
                            },
                            new FilterItem
                            {
                                FieldName = "Column",
                                LogicalComparisonType = ComparisonType.Equal,
                                Value = true,
                                Name = "c2",
                            },
                        },
                    },
                }
            };

            var result = testedFilter.GetFilterColumns();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            CommonAssert.CollectionsWithSameType(expectedColumnNames, result, (e, a) => Assert.Equal(e, a));
        }
    }
}
