namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Models;

    using Moq;

    using Xunit;

    public sealed class AddTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            AddCommentModel model = null;

            BaseServiceResult<Guid> result = TestedController.Add(model);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Add(It.IsAny<AddCommentModel>()));
        }
    }
}
