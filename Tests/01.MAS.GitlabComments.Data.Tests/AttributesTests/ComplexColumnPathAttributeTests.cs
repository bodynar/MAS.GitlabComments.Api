namespace MAS.GitlabComments.DataAccess.Tests.AttributesTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Attributes;

    using Xunit;

    public sealed class ComplexColumnPathAttributeTests
    {
        [Fact]
        public void ShouldThrowExceptionWhenColumnPathIsNull()
        {
            Exception exception =
                Record.Exception(
                    () => new ComplexColumnPathAttribute(null)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("columnPath", (exception as ArgumentNullException).ParamName);
        }

        [Fact]
        public void ShouldThrowExceptionWhenColumnPathIsEmpty()
        {
            Exception exception =
                Record.Exception(
                    () => new ComplexColumnPathAttribute(string.Empty)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("columnPath", (exception as ArgumentNullException).ParamName);
        }

        [Fact]
        public void ShouldThrowExceptionWhenColumnPathIsWhiteSpaceOnly()
        {
            string whiteSpace = new string(' ', 10);

            Exception exception =
                Record.Exception(
                    () => new ComplexColumnPathAttribute(whiteSpace)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("columnPath", (exception as ArgumentNullException).ParamName);
        }

        [Fact]
        public void ShouldCreateAnInstance()
        {
            string opeatorName = "operator";

            ComplexColumnPathAttribute attribute = new ComplexColumnPathAttribute(opeatorName);

            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute.ColumnPath);
            Assert.Equal(opeatorName, attribute.ColumnPath);
        }
    }
}
