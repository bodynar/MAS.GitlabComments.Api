namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="MsSqlDataProvider{TEntity}.Delete(Guid[])(TEntity)"/>
    /// </summary>
    public sealed class DeleteTests : BaseMsSqlDataProviderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenArgumentIsNull()
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
        public void ShouldNotExecuteCommand_WhenEntityIdContainsOnlyDefaultValues()
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

        [Fact]
        public void ShouldExecuteCommandWithoutDefaultParameters()
        {
            Guid[] entityIds = new Guid[] { default, Guid.NewGuid(), default };
            string expectedSql = $"DELETE FROM [{TestedTableName}] WHERE [Id] IN @P1";
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] { new KeyValuePair<string, object>("P1", new[] { entityIds[1] }) };

            TestedService.Delete(entityIds);
            var lastCommand = LastCommand;

            Assert.NotNull(lastCommand);
            Assert.Equal(expectedSql, lastCommand.Value.Key);
            Assert.NotNull(lastCommand.Value.Value);
            AssertArguments(expectedArguments, lastCommand.Value.Value);
        }
    }
}
