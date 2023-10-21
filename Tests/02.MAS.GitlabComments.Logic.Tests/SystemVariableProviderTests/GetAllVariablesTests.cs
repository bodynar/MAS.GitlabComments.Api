namespace MAS.GitlabComments.Logic.Tests.SystemVariableProviderTests
{
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;

    using Xunit;

    public sealed class GetAllVariablesTests : BaseSystemVariableProviderTests
    {
        [Fact]
        public void ShouldReadDataAndReturnIt()
        {
            IEnumerable<SysVariableDisplayModel> result = TestedService.GetAllVariables();

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
