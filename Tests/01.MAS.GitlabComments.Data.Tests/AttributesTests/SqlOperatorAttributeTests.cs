namespace MAS.GitlabComments.DataAccess.Tests.AttributesTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Attributes;

    using Xunit;

    public sealed class SqlOperatorAttributeTests
    {
        [Fact]
        public void ShouldThrowException_WhenOperatorIsNull()
        {
            Exception exception =
                Record.Exception(
                    () => new SqlOperatorAttribute(null)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
            Assert.Equal("operator", (exception as ArgumentNullException).ParamName);
        }

        [Fact]
        public void ShouldChangeOperatorCaseToUpperAfterCreatingAnInstance()
        {
            string expectedOperatorName = "OPERATOR";
            string opeatorName = "operator";

            SqlOperatorAttribute attribute = new SqlOperatorAttribute(opeatorName);

            Assert.NotNull(attribute);
            Assert.NotEmpty(attribute.Operator);
            Assert.Equal(expectedOperatorName, attribute.Operator);
        }
    }
}
