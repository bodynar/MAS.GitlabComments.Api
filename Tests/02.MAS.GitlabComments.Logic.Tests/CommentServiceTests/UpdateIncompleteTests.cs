namespace MAS.GitlabComments.Logic.Tests.CommentServiceTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class UpdateIncompleteTests: BaseCommentServiceTests
    {
        [Fact]
        public void ShouldDoNothing_WhenIncompleteRecordsNotFound()
        {
            ManualIncompleteData = Enumerable.Empty<IncompleteCommentData>();
            UseManualIncompleteData = true;

            TestedService.UpdateIncomplete();

            Assert.Empty(UpdateDataProviderArguments);
            Assert.Null(LastCommentNumber);
            Assert.False(IsSetNumberVariableCalled);
        }

        [Fact]
        public void ShouldUpdateCommentsAndLastCommentNumberVariable_WhenDataStoreContainsIncomplete()
        {
            UseManualIncompleteData = true;
            ManualIncompleteData = new[]
            {
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 10, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 20, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 30, },
                new IncompleteCommentData() { Id = Guid.NewGuid(), AppearanceCount = 40, },
            };
            int expectedLastNumber = IntVariableValue + ManualIncompleteData.Count();
            int expectedUpdateArgs = 1;
            string expectedSingleKeyName = nameof(Comment.Number);


            TestedService.UpdateIncomplete();


            Assert.NotEmpty(UpdateDataProviderArguments);
            Assert.NotNull(LastCommentNumber);
            Assert.True(IsSetNumberVariableCalled);

            Assert.Equal(expectedLastNumber, LastCommentNumber);
            Assert.Equal(ManualIncompleteData.Count(), UpdateDataProviderArguments.Count());

            for (int i = 0; i < ManualIncompleteData.Count(); i++)
            {
                var arrangedData = ManualIncompleteData.ElementAt(i);
                var updateData = UpdateDataProviderArguments.ElementAt(i);

                string expectedNumber = string.Format(CommentNumberTemplate, IntVariableValue + i + 1);

                Assert.Equal(arrangedData.Id, updateData.Key);
                Assert.Equal(expectedUpdateArgs, updateData.Value.Keys.Count);
                Assert.Equal(expectedSingleKeyName, updateData.Value.Keys.First());
                Assert.Equal(expectedNumber, updateData.Value[updateData.Value.Keys.First()]);
            }
        }
    }
}
