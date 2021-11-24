namespace MAS.GitlabComments.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Models;

    using Xunit;

    public sealed class UpdateTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenUpdateWithEmptyModel()
        {
            UpdateCommentModel model = null;

            var exception =
                Record.Exception(
                    () => TestedService.Update(model)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenUpdateWithEmptyMessage()
        {
            UpdateCommentModel model = new() { Message = string.Empty };

            var exception =
                Record.Exception(
                    () => TestedService.Update(model)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInUpdate()
        {
            UpdateCommentModel model = new() { Id = default, Message = string.Empty };

            Action testedAction = () => TestedService.Update(model);

            ShouldThrowArgumentNullExceptionWhenCommentIdIsDefaultInternal(testedAction);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInUpdate()
        {
            UpdateCommentModel model = new() { Id = Guid.NewGuid(), Message = "TestedMessage" };
            string expectedErrorMessage = $"Entity \"Comment\" - \"{model.Id}\" not found.";

            Action testedAction = () => TestedService.Update(model);

            ShouldThrowEntityNotFoundExceptionWhenEntityNotFoundByIdInternal(testedAction, expectedErrorMessage);
        }

        [Fact]
        public void ShouldUpdateComment()
        {
            UpdateCommentModel model = new() { Id = Guid.NewGuid(), Message = "TestedMessage" };
            string expectedCommandName = "Update";
            Guid firstExpectedArgument = model.Id;
            IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "Message", "TestedMessage" } };

            Action testedAction = () => TestedService.Update(model);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { firstExpectedArgument, secondExpectedArgument });
        }

        [Fact]
        public void ShouldUpdateCommentDescriptionWhenDescriptionIsNotEmpty()
        {
            UpdateCommentModel model = new() { Id = Guid.NewGuid(), Message = "TestedMessage", Description = "TestedDescription" };
            string expectedCommandName = "Update";
            Guid firstExpectedArgument = model.Id;
            IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "Message", "TestedMessage" }, { "Description", "TestedDescription" } };

            Action testedAction = () => TestedService.Update(model);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { firstExpectedArgument, secondExpectedArgument });
        }
    }
}
