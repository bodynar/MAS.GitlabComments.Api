namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class GetIncompleteTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultError_WhenExceptionOccurs()
        {
            AssertBaseServiceResultError(() => TestedController.GetIncomplete());
        }

        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<IEnumerable<IncompleteCommentData>> result = TestedController.GetIncomplete();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }
    }
}
