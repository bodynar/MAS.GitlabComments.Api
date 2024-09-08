namespace MAS.GitlabComments.WebApi.Tests.RetractionTokenControllerTests
{
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class GetAvailableTests : BaseRetractionTokenControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceResultSuccess()
        {
            BaseServiceResult<IEnumerable<RetractionTokenReadModel>> result = TestedController.GetAvailable();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.ErrorMessage);

            int count = result.Result.Count();
            RetractionTokenReadModel firstOrDefault = result.Result.FirstOrDefault();

            Assert.Equal(GetAvailableResult.Count(), count);
            Assert.Equal(GetAvailableResult.FirstOrDefault(), firstOrDefault);
        }

        [Fact]
        public void ShouldReturnBaseServiceResultError()
        {
            AssertBaseServiceResultError(() => TestedController.GetAvailable());
        }
    }
}
