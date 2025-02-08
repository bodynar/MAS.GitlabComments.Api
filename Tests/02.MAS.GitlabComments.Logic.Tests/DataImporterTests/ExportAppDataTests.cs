namespace MAS.GitlabComments.Logic.Tests.DataImporterTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;

    using Xunit;

    sealed public class ExportAppDataTests : BaseDataImporterTests
    {
        [Fact]
        public void ShouldExportData()
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),

                Message = "TestedMessage",
                AppearanceCount = 159357,
                CommentWithLinkToRule = "TestedCommentWithLinkToRule",
                Description = "TestedDescription",
                Number = "TestedNumber",
            };
            ReturnedComments = new List<Comment> { comment };

            var storyRecord = new StoryRecord
            {
                Id = Guid.NewGuid(),

                CreatedOn = DateTime.Now.AddYears(-5),
                IsRetracted = true,
                CommentId = comment.Id,
            };
            ReturnedStoryRecords = new List<StoryRecord> { storyRecord };

            var retractionToken = new RetractionToken
            {
                Id = Guid.NewGuid(),

                CommentId = comment.Id,
                StoryRecordId = storyRecord.Id,

                ValidUntil = DateTime.UtcNow.AddMonths(5),
            };
            ReturnedTokens = new List<RetractionToken> { retractionToken };

            var expectedCommentsCount = 1;
            var expectedStoryRecordsCount = 1;
            var expectedTokensCount = 1;


            var result = TestedService.ExportAppData();


            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(expectedCommentsCount, result.Count());

            var firstRecord = result.First();

            Assert.Equal(comment.Message, firstRecord.Message);
            Assert.Equal(comment.AppearanceCount, firstRecord.AppearanceCount);
            Assert.Equal(comment.CommentWithLinkToRule, firstRecord.CommentWithLinkToRule);
            Assert.Equal(comment.Description, firstRecord.Description);

            Assert.Equal(expectedStoryRecordsCount, firstRecord.StoryRecords.Count());

            var firstStoryExported = firstRecord.StoryRecords.First();
            Assert.Equal(storyRecord.CreatedOn, firstStoryExported.CreatedOn);
            Assert.Equal(storyRecord.IsRetracted, firstStoryExported.IsRetracted);

            Assert.Equal(expectedTokensCount, firstStoryExported.RetractionTokens.Count());

            var firstTokenExported = firstStoryExported.RetractionTokens.First();
            Assert.Equal(retractionToken.ValidUntil, firstTokenExported.ValidUntil);
        }
    }
}
