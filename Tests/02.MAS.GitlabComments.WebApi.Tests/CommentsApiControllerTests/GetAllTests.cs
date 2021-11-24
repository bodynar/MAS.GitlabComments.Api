namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System.Collections.Generic;

    using MAS.GitlabComments.Models;

    using Xunit;

    public sealed class GetAllTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<IEnumerable<CommentModel>> result = TestedController.Get();

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
