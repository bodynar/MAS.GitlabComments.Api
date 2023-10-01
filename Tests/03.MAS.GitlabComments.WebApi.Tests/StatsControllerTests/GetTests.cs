namespace MAS.GitlabComments.WebApi.Tests.StatsControllerTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.WebApi.Models;

    using Moq;

    using Xunit;

    public sealed class GetTests : BaseStatsControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<IEnumerable<StoryRecordViewModel>> result = TestedController.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>());

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.Get(It.IsAny<DateTime>(), It.IsAny<DateTime>()));
        }
    }
}
