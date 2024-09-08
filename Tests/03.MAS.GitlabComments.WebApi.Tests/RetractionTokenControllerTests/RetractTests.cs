namespace MAS.GitlabComments.WebApi.Tests.RetractionTokenControllerTests
{
    using System;

    using MAS.GitlabComments.WebApi.Models;

    using Moq;

    using Xunit;

    public sealed class RetractTests : BaseRetractionTokenControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.Retract(It.IsAny<Guid>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Retract(It.IsAny<Guid>()));
        }
    }
}
