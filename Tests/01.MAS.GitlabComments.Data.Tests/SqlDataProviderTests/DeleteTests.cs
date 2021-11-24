namespace MAS.GitlabComments.Data.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    public sealed class DeleteTests : BaseSqlDataProviderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenArgumentIsNull()
        {
            Guid[] entityIds = null;

            Exception exception =
                Record.Exception(
                    () => TestedService.Delete(entityIds)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldNotExecuteCommandWhenEntityIdContainsOnlyDefaultValues()
        {
            Guid[] entityIds = new Guid[] { default, default, default };

            TestedService.Delete(entityIds);
            var lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }

        [Fact]
        public void ShouldExecuteCommand()
        {
            Guid[] entityIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            string expectedSql = $"DELETE FROM [{TestedTableName}] WHERE [Id] IN @P1";
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] { new KeyValuePair<string, object>("P1", entityIds) };

            TestedService.Delete(entityIds);
            var lastCommand = LastCommand;

            Assert.NotNull(lastCommand);
            Assert.Equal(expectedSql, lastCommand.Value.Key);
            Assert.NotNull(lastCommand.Value.Value);
            AssertArguments(expectedArguments, lastCommand.Value.Value);
        }
    }
}
