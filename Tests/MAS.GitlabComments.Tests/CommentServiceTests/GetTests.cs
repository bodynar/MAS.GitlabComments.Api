namespace MAS.GitlabComments.Tests.CommentServiceTests
{
    using System;

    using MAS.GitlabComments.Models;

    using Xunit;

    public sealed class GetTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInGet()
        {
            Guid testedId = default;

            Action testedAction = () => TestedService.Get(testedId);

            ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInternal(testedAction);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInGet()
        {
            Guid testedId = Guid.NewGuid();
            string expectedErrorMessage = $"Entity \"Comment\" - \"{testedId}\" not found.";

            Action testedAction = () => TestedService.Get(testedId);

            ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInternal(testedAction, expectedErrorMessage);
        }

        [Fact]
        public void ShouldReturnComment()
        {
            Guid testedId = Guid.NewGuid();
            CommentModel expectedComment = new()
            {
                Id = ReturnedTestedComment.Id,
                AppearanceCount = ReturnedTestedComment.AppearanceCount,
                Message = ReturnedTestedComment.Message,
            };

            CommentModel result = TestedService.Get(testedId);

            Assert.NotNull(result);
            Assert.Equal(expectedComment.Id, result.Id);
            Assert.Equal(expectedComment.AppearanceCount, result.AppearanceCount);
            Assert.Equal(expectedComment.Message, result.Message);
        }
    }
}
