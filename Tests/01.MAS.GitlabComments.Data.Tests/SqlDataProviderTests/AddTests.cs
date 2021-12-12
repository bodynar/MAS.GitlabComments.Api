namespace MAS.GitlabComments.Data.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using Xunit;

    public sealed class AddTests : BaseSqlDataProviderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenEntityIsNull()
        {
            TestedDataProviderEntity entity = null;

            Exception exception =
                Record.Exception(
                    () => TestedService.Add(entity)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldNotCallAnyCommandWhenEntityValuesAreDefault()
        {
            TestedDataProviderEntity entity = new() { Id = default, StringField = default };
            TestedAffectedRowsCount = 10;

            TestedService.Add(entity);
            var lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }

        [Fact]
        public void ShouldAddEntity()
        {
            string expectedSqlQuery = $"INSERT INTO [{TestedTableName}] ([StringField], [Id], [CreatedOn]) VALUES (@P1, @P2, @P3)";

            TestedDataProviderEntity entity = new() { Id = Guid.NewGuid(), StringField = "Some test data" };
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] {
                new KeyValuePair<string, object>("@P1", entity.StringField),
                new KeyValuePair<string, object>("@P2", entity.Id),
                new KeyValuePair<string, object>("@P3", DateTime.UtcNow),
            };
            TestedAffectedRowsCount = 10;


            TestedService.Add(entity);
            var lastCommand = LastCommand;


            Assert.NotNull(lastCommand);
            Assert.Equal(expectedSqlQuery, lastCommand.Value.Key);
            Assert.NotNull(lastCommand.Value.Value);
            AssertArguments(expectedArguments, lastCommand.Value.Value);
        }

        [Fact]
        public void ShouldThrowExceptionWhenAffectedRowsIsZero()
        {
            string expectedErrorMessage = "Insert command performed with empty result, no record was added.";
            TestedDataProviderEntity entity = new() { Id = Guid.NewGuid(), StringField = "Some test data" };
            TestedAffectedRowsCount = 0;

            Exception exception =
                Record.Exception(
                    () => TestedService.Add(entity)
                );

            Assert.NotNull(exception);
            Assert.Equal(expectedErrorMessage, exception.Message);
        }
    }
}
