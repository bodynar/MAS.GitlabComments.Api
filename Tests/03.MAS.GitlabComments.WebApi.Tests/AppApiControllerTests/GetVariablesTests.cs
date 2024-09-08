namespace MAS.GitlabComments.WebApi.Tests.AppApiControllerTests
{
    using System.Collections.Generic;
    using System.Linq;

    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.WebApi.Models;

    using Xunit;

    public sealed class GetVariablesTests : BaseAppApiControllerTests
    {
        [Fact]
        public void ShouldReturnBaseServiceError_WhenExceptionOccurs()
        {
            ShouldThrowAnException = true;
            var result = TestedController.GetVariables();

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(ExceptionTestMessage, result.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnErrorResult_WhenExceptionOccurs()
        {
            ShouldThrowAnException = true;

            BaseServiceResult<IEnumerable<SysVariableDisplayModel>> baseServiceResult = TestedController.GetVariables();

            Assert.NotNull(baseServiceResult);
            Assert.False(baseServiceResult.IsSuccess);
            Assert.Equal(ExceptionTestMessage, baseServiceResult.ErrorMessage);
        }

        [Fact]
        public void ShouldReturnVariables()
        {
            BaseServiceResult<IEnumerable<SysVariableDisplayModel>> result = TestedController.GetVariables();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(AllVariables.Count(), result.Result.Count());
        }
    }
}
