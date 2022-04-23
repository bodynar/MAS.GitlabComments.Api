namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;

    using MAS.GitlabComments.WebApi.Models;

    using Moq;

    using Xunit;

    public sealed class IncrementTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.Increment(It.IsAny<Guid>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Increment(It.IsAny<Guid>()));
        }
    }
}
