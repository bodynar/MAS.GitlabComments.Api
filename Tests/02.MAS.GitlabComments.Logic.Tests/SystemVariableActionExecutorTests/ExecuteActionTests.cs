namespace MAS.GitlabComments.Logic.Tests.SystemVariableActionExecutorTests
{
    using System;

    using MAS.GitlabComments.Data;

    using Xunit;

    public sealed class ExecuteActionTests : BaseSystemVariableActionExecutorTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenCodeIsNull()
        {
            var exception =
                Record.Exception(
                    () => TestedService.ExecuteAction(null)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowNotImplementedException_WhenCodeIsCouldBeExecuted()
        {
            string code = "SOME_RANDOM_STRING";
            string expectedExceptionMessage = "Action for variable \"SOME_RANDOM_STRING\" is not defined";

            var exception =
                Record.Exception(
                    () => TestedService.ExecuteAction(code)
                );

            Assert.NotNull(exception);
            Assert.IsType<NotImplementedException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldRecalculateLastNumber_WhenCodeIsLastCommentNumber()
        {
            string code = BuiltInSysVariables.LastCommentNumber;

            TestedService.ExecuteAction(code);

            Assert.True(IsRecalculationCalled);
        }
    }
}
