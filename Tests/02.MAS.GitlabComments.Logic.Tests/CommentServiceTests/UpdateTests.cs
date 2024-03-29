﻿namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class UpdateTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenUpdateWithEmptyModel()
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
        public void ShouldThrowArgumentNullException_WhenUpdateWithEmptyMessage()
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
        public void ShouldThrowArgumentNullException_WhenUpdateWithEmptyLinkToRule()
        {
            UpdateCommentModel model = new() { Message = "", CommentWithLinkToRule = string.Empty };

            var exception =
                Record.Exception(
                    () => TestedService.Update(model)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullException_WhenCommentIdIsDefaultInUpdate()
        {
            UpdateCommentModel model = new() { Id = default, Message = "TestedMessage", CommentWithLinkToRule = "CommentWithLinkToRule" };

            Action testedAction = () => TestedService.Update(model);

            ShouldThrowArgumentNullException_WhenCommentIdIsDefaultInternal(testedAction);
        }

        [Fact]
        public void ShouldThrowEntityNotFoundException_WhenEntityNotFoundByIdInUpdate()
        {
            UpdateCommentModel model = new() { Id = Guid.NewGuid(), Message = "TestedMessage", CommentWithLinkToRule = "CommentWithLinkToRule" };
            string expectedErrorMessage = $"Entity \"Comment\" - \"{model.Id}\" not found.";

            Action testedAction = () => TestedService.Update(model);

            ShouldThrowEntityNotFoundException_WhenEntityNotFoundByIdInternal(testedAction, expectedErrorMessage);
        }

        [Fact]
        public void ShouldUpdateComment()
        {
            UpdateCommentModel model = new() { Id = Guid.NewGuid(), Message = "TestedMessage", Description = "TestedDescription", CommentWithLinkToRule = "TestedCommentWithLinkToRule" };
            string expectedCommandName = "Update";
            Guid firstExpectedArgument = model.Id;
            IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> {
                { "Message", "TestedMessage" },
                { "Description", "TestedDescription" },
                { "CommentWithLinkToRule", "TestedCommentWithLinkToRule" }
            };

            Action testedAction = () => TestedService.Update(model);

            ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { firstExpectedArgument, secondExpectedArgument });
        }
    }
}
