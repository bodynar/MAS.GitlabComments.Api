namespace MAS.GitlabComments.Data.Tests.ComplexColumnMssqlBuilderTests
{
    using System.Linq;

    using MAS.GitlabComments.Data.Attributes;
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

        #region ShouldReturnNullWhenProjectedTypeHasNoColumns

        [Fact]
        public void ShouldReturnNullWhenProjectedTypeHasNoColumns()
        {
            var result = TestedService.BuildComplexColumns<EmptyProjectedClass>(string.Empty);

            Assert.Null(result);
        }

        private class EmptyProjectedClass { }

        #endregion

        #region ShouldReturnEmptyDataWhenProjectedModelHasEmptyAttributes

        [Fact]
        public void ShouldReturnEmptyDataWhenProjectedModelHasEmptyAttributes()
        {
            var expectedColumn = "SimplePath";
            var result = TestedService.BuildComplexColumns<ProjectedClassWithEmptyAttributes>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault());
        }

        private class ProjectedClassWithEmptyAttributes
        {
            public string SimplePath { get; set; }
        }

        #endregion

        #region ShouldBuildDataWithoutJoinData

        [Fact]
        public void ShouldBuildDataWithoutJoinData()
        {
            var expectedColumn = "SimplePath";
            var result = TestedService.BuildComplexColumns<ProjectedClassWithSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault());
        }

        private class ProjectedClassWithSimplePath
        {
            [ComplexColumnPath("SimplePath")]
            public string SimplePath { get; set; }
        }

        #endregion

        #region ShouldBuildDataWithSingleJoinData

        [Fact]
        public void ShouldBuildDataWithSingleJoinData()
        {
            // TODO:
            var expectedColumn = "Right1.Value"; 
            var result = TestedService.BuildComplexColumns<ProjectedClassWithSimplePath>(SourceTableName);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Columns);
            Assert.Empty(result.Joins);

            Assert.Equal(expectedColumn, result.Columns.FirstOrDefault());
        }

        private class ProjectedClassWithComplexPath
        {
            [ComplexColumnPath("[RightTable:RightTableColumn:LeftTableColumn].Value")]
            public string SimplePath { get; set; }
        }

        #endregion
    }
}
