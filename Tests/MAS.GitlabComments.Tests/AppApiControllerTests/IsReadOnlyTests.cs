namespace MAS.GitlabComments.Tests.AppApiControllerTests
{
    using Xunit;

    public sealed class IsReadOnlyTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnFalseWhenReadOnlyModeIsFalse()
        {
            SettingReadOnlyMode = false;

            var result = TestedController.IsReadOnly();

            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenReadOnlyModeIsTrue()
        {
            SettingReadOnlyMode = true;

            var result = TestedController.IsReadOnly();

            Assert.True(result);
        }
    }
}
