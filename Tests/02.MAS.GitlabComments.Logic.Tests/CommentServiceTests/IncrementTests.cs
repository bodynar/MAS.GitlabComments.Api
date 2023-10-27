namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    public sealed class IncrementTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenCommentIdIsDefaultInIncrement()
        {
            Guid testedCommentId = default;

            Action testedAction = () => TestedService.Increment(testedCommentId);

            ShouldThrowArgumentNullException_WhenCommentIdIsDefaultInternal(testedAction);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundException_WhenEntityNotFoundByIdInIncrement()
        {
            Guid testedCommentId = Guid.NewGuid();
            string expectedErrorMessage = $"Entity \"Comment\" - \"{testedCommentId}\" not found.";

            Action testedAction = () => TestedService.Increment(testedCommentId);

            ShouldThrowEntityNotFoundException_WhenEntityNotFoundByIdInternal(testedAction, expectedErrorMessage);
        }

        [Fact]
        public void ShouldIncrementCommentAppearanceCount()
        {
            Guid testedCommentId = Guid.NewGuid();
            string expectedCommandName = "Update";
            long newAppearanceCount = ReturnedTestedComment.AppearanceCount + 1;
            IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "AppearanceCount", newAppearanceCount } };

            Action testedAction = () => TestedService.Increment(testedCommentId);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { testedCommentId, secondExpectedArgument });

            Assert.NotNull(LastAddedStoryRecord);
            Assert.Equal(LastAddedStoryRecord.CommentId, testedCommentId);
        }
    }
}
