namespace MAS.GitlabComments.WebApi.Tests.CommentServiceTests
{
    using System;

    using MAS.GitlabComments.Models;

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

            Guid result = TestedService.Add(model);

            Assert.NotEqual(Guid.Empty, result);
        }
    }
}
