namespace MAS.GitlabComments.WebApi.Tests.CommentsApiControllerTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.WebApi.Models;
    using MAS.GitlabComments.WebApi.Controllers;

    using Xunit;

    /// <summary>
    /// Tests for <see cref="CommentsApiController.Merge(Guid, Guid, IReadOnlyDictionary{string, object})"/>
    /// </summary>
    public sealed class MergeTests : BaseCommentsApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult result = TestedController.Merge(new MergeCommentModel { });

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Merge(new MergeCommentModel { }));
        }
    }
}
