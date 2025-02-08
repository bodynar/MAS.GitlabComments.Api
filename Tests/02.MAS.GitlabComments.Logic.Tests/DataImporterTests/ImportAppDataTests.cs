namespace MAS.GitlabComments.Logic.Tests.DataImporterTests
{
    using System;

    using MAS.GitlabComments.Logic.Models.Import;

    using Xunit;

    sealed public class ImportAppDataTests : BaseDataImporterTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenCommentsIsNull()
        {
            var exception =
                Record.Exception(
                    () => TestedService.ImportAppData(null)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentException_WhenCommentsContaisNull()
        {
            var expectedExceptionMessage = "Exported data contains 'null' value";

            var exception =
                Record.Exception(
                    () => TestedService.ImportAppData(new CommentExportModel[] { null })
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            var startsWithExpected = exception.Message.StartsWith(expectedExceptionMessage); // suppress ParameterName part
            Assert.True(startsWithExpected);
        }

        [Fact]
        public void ShouldThrowArgumentException_WhenCommentsIsEmpty()
        {
            var expectedExceptionMessage = "No data to import";
            var exception =
                Record.Exception(
                    () => TestedService.ImportAppData(new CommentExportModel[] { })
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldAddImportedData()
        {
            var exportedToken = new RetractionTokenExportModel
            {
                ValidUntil = DateTime.UtcNow.AddMonths(5),
            };

            var exportedStoryRecord = new StoryRecordExportModel
            {
                CreatedOn = DateTime.UtcNow.AddYears(-5),
                IsRetracted = true,
                RetractionTokens = new[] { exportedToken }
            };

            var exportedComment = new CommentExportModel
            {
                AppearanceCount = 15,
                CommentWithLinkToRule = "TestedCommentWithLinkToRule",
                Description = "TestedDescription",
                Message = "TestedMessage",
                StoryRecords = new[] { exportedStoryRecord },
            };

            var testedData = new[] { exportedComment };


            TestedService.ImportAppData(testedData);


            Assert.NotNull(AddedComment);
            Assert.Equal(exportedComment.Message, AddedComment.Message);
            Assert.Equal(exportedComment.AppearanceCount, AddedComment.AppearanceCount);
            Assert.Equal(exportedComment.CommentWithLinkToRule, AddedComment.CommentWithLinkToRule);
            Assert.Equal(exportedComment.Description, AddedComment.Description);

            Assert.NotNull(AddedStoryRecord);
            Assert.Equal(exportedStoryRecord.CreatedOn, AddedStoryRecord.CreatedOn);
            Assert.Equal(DateTimeKind.Utc, AddedStoryRecord.CreatedOn.Kind);
            Assert.Equal(exportedStoryRecord.IsRetracted, AddedStoryRecord.IsRetracted);

            Assert.NotNull(AddedToken);
            Assert.Equal(exportedToken.ValidUntil, AddedToken.ValidUntil);
            Assert.Equal(DateTimeKind.Utc, AddedToken.ValidUntil.Kind);

            Assert.True(IsRecalculateLastNumberCalled);
        }
    }
}
