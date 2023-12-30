namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public class CanMakeNumberColumnUniqueTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldReturnFalse_WhenFlagVariableHasTrueValue()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "True", };

            bool result = TestedService.CanMakeNumberColumnUnique();

            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalse_WhenFlagVariableNotFound()
        {
            UseManualIncompleteData = true;
            ManualIncompleteData = Enumerable.Empty<IncompleteCommentData>();

            bool result = TestedService.CanMakeNumberColumnUnique();

            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalse_WhenDataStoreHasIncompleteComments()
        {
            UseManualIncompleteData = true;
            ManualIncompleteData = new[]
            {
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 10, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 20, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 30, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 40, },
            };

            bool result = TestedService.CanMakeNumberColumnUnique();

            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnTrue_WhenDataStoreHasNotIncompleteCommentsAndVariableFlagIsFalse()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "False", };
            UseManualIncompleteData = true;
            ManualIncompleteData = Enumerable.Empty<IncompleteCommentData>();

            bool result = TestedService.CanMakeNumberColumnUnique();

            Assert.True(result);
        }
    }
}
