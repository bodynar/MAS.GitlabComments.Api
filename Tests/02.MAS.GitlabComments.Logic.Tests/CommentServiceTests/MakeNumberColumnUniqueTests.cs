namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class MakeNumberColumnUniqueTests : BaseCommentServiceTests
    {
        [Fact]
        public void ShouldDoNothing_WhenFlagVariableHasTrueValue()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "True", };

            TestedService.MakeNumberColumnUnique();

            Assert.False(IsTempModifierWasCalled);
        }

        [Fact]
        public void ShouldDoNothing_WhenDataStoreHasIncompleteComments()
        {
            UseManualIncompleteData = true;
            ManualIncompleteData = new[]
            {
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 10, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 20, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 30, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 40, },
            };

            TestedService.MakeNumberColumnUnique();

            Assert.False(IsTempModifierWasCalled);
        }

        [Fact]
        public void ShouldDoNothing_WhenFlagVariableNotFound()
        {
            UseManualIncompleteData = true;
            ManualIncompleteData = Enumerable.Empty<IncompleteCommentData>();

            TestedService.MakeNumberColumnUnique();

            Assert.False(IsTempModifierWasCalled);
        }

        [Fact]
        public void ShouldCallTempDataBaseModifierAndSetVariableFlag_WhenFlagNotSetAndDataStoreContainsNoIncompleteComments()
        {
            ReturnedSysVariable = new SysVariable() { RawValue = "False", };
            UseManualIncompleteData = true;
            ManualIncompleteData = Enumerable.Empty<IncompleteCommentData>();

            TestedService.MakeNumberColumnUnique();

            Assert.True(IsTempModifierWasCalled);
            Assert.True(IsChangeNumberUniqueValue);
        }
    }
}
