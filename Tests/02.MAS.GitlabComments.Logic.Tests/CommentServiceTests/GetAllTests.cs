namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Logic.Models;

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
                Message = ReturnedTestedComment.Message,
                CommentWithLinkToRule = ReturnedTestedComment.CommentWithLinkToRule,
                Description = ReturnedTestedComment.Description,
                Number = ReturnedTestedComment.Number,
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
            Assert.Equal(expectedCommentModel.CommentWithLinkToRule, firstItem.CommentWithLinkToRule);
            Assert.Equal(expectedCommentModel.Description, firstItem.Description);
            Assert.Equal(expectedCommentModel.Number, firstItem.Number);
        }
    }
}
