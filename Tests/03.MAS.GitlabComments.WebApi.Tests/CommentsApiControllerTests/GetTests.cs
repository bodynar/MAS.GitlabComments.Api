namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;

    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.Logic.Models;

    using Moq;

    using Xunit;

    public sealed class GetTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<ExtendedCommentModel> result = TestedController.Get(It.IsAny<Guid>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Get());
        }
    }
}
