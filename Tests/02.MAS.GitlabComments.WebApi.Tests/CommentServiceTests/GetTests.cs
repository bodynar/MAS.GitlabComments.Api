namespace MAS.GitlabComments.WebApi.Tests.CommentServiceTests
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
            ExtendedCommentModel expectedComment = new()
            {
                Id = ReturnedTestedComment.Id,
                Message = ReturnedTestedComment.Message,
                Description = ReturnedTestedComment.Description
            };

            ExtendedCommentModel result = TestedService.Get(testedId);

            Assert.NotNull(result);
            Assert.Equal(expectedComment.Id, result.Id);
            Assert.Equal(expectedComment.Description, result.Description);
            Assert.Equal(expectedComment.Message, result.Message);
        }
    }
}
