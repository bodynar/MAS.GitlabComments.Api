namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class AddTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenAddWithEmptyModel()
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
        public void ShouldThrowArgumentNullExceptionWhenAddWithEmptyMessage()
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
            AddCommentModel model = new() { Message = "TestedMessage" };
            string expectedCommandName = "Add";

            Action testedAction = () => TestedService.Add(model);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { });
        }
    }
}
