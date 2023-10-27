namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class AddTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenAddWithEmptyModel()
        {
            AddCommentModel model = null;

            var exception =
                Record.Exception(
                    () => TestedService.Add(model)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullException_WhenAddWithEmptyMessage()
        {
            AddCommentModel model = new() { Message = string.Empty };

            var exception =
                Record.Exception(
                    () => TestedService.Add(model)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldAddCommentAndReturnNewId()
        {
            AddCommentModel model = new()
            {
                Message = "TestedMessage",
                CommentWithLinkToRule = "CommentWithLinkToRule",
                Description = "Description",
            };
            string expectedCommandName = "Add";
            string expectedCommentNumber = string.Format(CommentNumberTemplate, IntVariableValue + 1);
            Action testedAction = () => TestedService.Add(model);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { });

            Assert.NotNull(LastAddedComment);
            Assert.Equal(expectedCommentNumber, LastAddedComment.Number);
            Assert.Equal(model.Message, LastAddedComment.Message);
            Assert.Equal(model.CommentWithLinkToRule, LastAddedComment.CommentWithLinkToRule);
            Assert.Equal(model.Description, LastAddedComment.Description);
            Assert.False(IsSetNumberVariableCalled);
        }

        [Fact]
        public void ShouldSetSystemVariable_WhenProviderDoesNotCreatedEntity()
        {
            AddCommentModel model = new()
            {
                Message = "TestedMessage",
                CommentWithLinkToRule = "CommentWithLinkToRule",
                Description = "Description",
            };
            string expectedCommandName = "Add";
            string expectedCommentNumber = string.Format(CommentNumberTemplate, IntVariableValue + 1);
            Action testedAction = () => TestedService.Add(model);
            ReturnedCreatedCommentId = Guid.NewGuid();

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { });

            Assert.NotNull(LastAddedComment);
            Assert.Equal(expectedCommentNumber, LastAddedComment.Number);
            Assert.Equal(model.Message, LastAddedComment.Message);
            Assert.Equal(model.CommentWithLinkToRule, LastAddedComment.CommentWithLinkToRule);
            Assert.Equal(model.Description, LastAddedComment.Description);
            Assert.True(IsSetNumberVariableCalled);
        }
    }
}
