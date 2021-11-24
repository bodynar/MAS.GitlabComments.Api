namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;

    using MAS.GitlabComments.Models;

    using Moq;

    using Xunit;

    public sealed class DeleteTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.Delete(It.IsAny<Guid>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Delete(It.IsAny<Guid>()));
        }
    }
}
