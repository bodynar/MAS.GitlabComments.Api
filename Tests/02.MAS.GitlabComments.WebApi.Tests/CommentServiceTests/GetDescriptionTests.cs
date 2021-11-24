namespace MAS.GitlabComments.WebApi.Tests.CommentServiceTests
{
    using System;

    using Xunit;

    public sealed class GetDescriptionTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInGetDescription()
        {
            Guid testedId = default;

            Action testedAction = () => TestedService.GetDescription(testedId);

            ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInternal(testedAction);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInGetDescription()
        {
            Guid testedId = Guid.NewGuid();
            string expectedErrorMessage = $"Entity \"Comment\" - \"{testedId}\" not found.";

            Action testedAction = () => TestedService.GetDescription(testedId);

            ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInternal(testedAction, expectedErrorMessage);
        }

        [Fact]
        public void ShouldReturnDescription()
        {
            string expectedDescription = ReturnedTestedComment.Description;
            Guid testedId = Guid.NewGuid();

            string result = TestedService.GetDescription(testedId);

            Assert.NotNull(result);
            Assert.NotEqual(string.Empty, result);
            Assert.Equal(expectedDescription, result);
        }
    }
}
