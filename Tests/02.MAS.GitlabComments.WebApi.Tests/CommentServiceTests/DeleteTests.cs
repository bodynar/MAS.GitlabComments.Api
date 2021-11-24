namespace MAS.GitlabComments.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    public sealed class DeleteTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenParameterIsNull()
        {
            Guid[] commentIds = null;

            var exception =
                Record.Exception(
                    () => TestedService.Delete(commentIds)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldDeleteComments()
        {
            Guid[] commentIds = new[] { Guid.NewGuid() };
            string expectedCommandName = "Delete";

            Action testedAction = () => TestedService.Delete(commentIds);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { commentIds });
        }

        [Fact]
        public void ShouldNotDeleteCommentsWhenCommentIdsContainsOnlyGuidEmptyValues()
        {
            Guid[] commentIds = new[] { Guid.Empty };
            TestedService.Delete(commentIds);

            KeyValuePair<string, IEnumerable<object>>? lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }
    }
}
