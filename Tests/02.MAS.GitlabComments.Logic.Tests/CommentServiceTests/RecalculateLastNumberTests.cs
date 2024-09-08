namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class RecalculateLastNumberTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldDoNothing_WhenVariableNumberIsEqualToMaxNumberFromDatabase()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "10", };

            TestedService.RecalculateLastNumber();

            Assert.False(IsSetNumberVariableCalled);
        }

        [Fact]
        public void ShouldUpdateLastCommentNumberVariable()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "1", };
            ProjectedNumberOfCommentTestComment.Number = "TEST_100";
            int expectedNewNumber = 100;

            TestedService.RecalculateLastNumber();

            Assert.True(IsSetNumberVariableCalled);
            Assert.Equal(expectedNewNumber, LastCommentNumber);
        }
    }
}
