namespace MAS.GitlabComments.Tests.CommentServiceTests
{
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Models;

    using Xunit;

    public sealed class GetAllTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldReturnMappedComments()
        {
            int expectedCommentsCount = 1;
            CommentModel expectedCommentModel = new()
            {
                Id = ReturnedTestedComment.Id,
                AppearanceCount = ReturnedTestedComment.AppearanceCount,
                Message = ReturnedTestedComment.Message
            };

            IEnumerable<CommentModel> result = TestedService.Get();
            var firstItem = result.FirstOrDefault();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(expectedCommentsCount, result.Count());
            Assert.NotNull(firstItem);
            Assert.Equal(expectedCommentModel.Id, firstItem.Id);
            Assert.Equal(expectedCommentModel.AppearanceCount, firstItem.AppearanceCount);
            Assert.Equal(expectedCommentModel.Message, firstItem.Message);
        }
    }
}
