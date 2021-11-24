namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;

    using MAS.GitlabComments.Models;

    using Moq;

    using Xunit;

    public sealed class GetDescriptionTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<string> result = TestedController.GetDescription(It.IsAny<Guid>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.GetDescription(It.IsAny<Guid>()));
        }
    }
}
