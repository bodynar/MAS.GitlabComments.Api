namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using Xunit;

    public sealed class IsReadOnlyTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnFalse_WhenReadOnlyModeIsFalse()
        {
            SettingReadOnlyMode = false;

            var result = TestedController.IsReadOnly();

            Assert.NotNull(result);
            Assert.False(result.Result);
        }

        [Fact]
        public void ShouldReturnFalse_WhenReadOnlyModeIsTrue()
        {
            SettingReadOnlyMode = true;

            var result = TestedController.IsReadOnly();

            Assert.NotNull(result);
            Assert.True(result.Result);
        }
    }
}
