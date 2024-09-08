namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Tests;
    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class MergeTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenSourceIdIsDefault()
        {
            var exception =
                Record.Exception(
                    () => TestedService.Merge(default, default, default)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullException_WhenTargetIdIsDefault()
        {
            var exception =
                Record.Exception(
                    () => TestedService.Merge(Guid.NewGuid(), default, default)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundException_WhenSourceEntityNotFound()
        {
            Guid sourceId = Guid.NewGuid();
            DataProviderGetReplacer = (x) => null;

            var exception =
                Record.Exception(
                    () => TestedService.Merge(sourceId, Guid.NewGuid(), default)
                );

            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Equal(nameof(Comment), ((EntityNotFoundException)exception).EntityName);
            Assert.Equal(sourceId, ((EntityNotFoundException)exception).EntityId);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundException_WhenTargetEntityNotFound()
        {
            Guid targetId = Guid.NewGuid();

            DataProviderGetReplacer = (x) => x == targetId ? null : ReturnedTestedComment;

            var exception =
                Record.Exception(
                    () => TestedService.Merge(Guid.NewGuid(), targetId, default)
                );

            Assert.NotNull(exception);
            Assert.IsType<EntityNotFoundException>(exception);
            Assert.Equal(nameof(Comment), ((EntityNotFoundException)exception).EntityName);
            Assert.Equal(targetId, ((EntityNotFoundException)exception).EntityId);
        }

        [Fact]
        public void ShouldUpdateSourceAndDeleteTarget()
        {
            Guid targetId = Guid.NewGuid();
            Guid sourceId = Guid.NewGuid();
            Comment sourceComment = new Comment { AppearanceCount = 10 };
            Comment targetComment = new Comment { AppearanceCount = 5 };
            MergeCommentUpdateModel values = new MergeCommentUpdateModel
            {
                Message = "TestedMessage",
                Description = "TestedDescription",
                CommentWithLinkToRule = "TestedCommentWithLinkToRule",
            };

            DataProviderGetReplacer = (x) => x == targetId ? targetComment : sourceComment;

            IReadOnlyDictionary<string, object> expectedDeleteArgs = new Dictionary<string, object> {
                { nameof(Comment.AppearanceCount), (long)15 },
                { nameof(Comment.Message), "TestedMessage" },
                { nameof(Comment.Description), "TestedDescription" },
                { nameof(Comment.CommentWithLinkToRule), "TestedCommentWithLinkToRule" }
            };


            TestedService.Merge(sourceId, targetId, values);


            Assert.Equal(2, Commands.Count());

            var firstCommand = Commands.First();
            Assert.Equal("Delete", LastCommand.Value.Key);
            Assert.Single(LastCommand.Value.Value);

            var firstArgument = (Guid[])LastCommand.Value.Value.First();
            Assert.Equal(sourceId, firstArgument.First());

            Assert.Equal("Update", firstCommand.Key);
            CommonAssert.Collections(
                expectedDeleteArgs,
                (IReadOnlyDictionary<string, object>)((object[])firstCommand.Value).LastOrDefault(),
                (e, a) =>
                {
                    Assert.Equal(e.Key, a.Key);
                    Assert.Equal(e.Value, a.Value);
                }
            );
        }
    }
}
