namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Services.Implementations;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="SqlDataProvider{TEntity}.Update(Guid, IDictionary{string, object})"/>
    /// </summary>
    public sealed class UpdateTests : BaseSqlDataProviderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenEntityIdIsDefault()
        {
            Guid entityId = default;
            IDictionary<string, object> newValues = new Dictionary<string, object>();

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenEntityValuesIsNull()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = null;

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldThrowArgumentNullExceptionWhenEntityValuesIsEmpty()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>();

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ShouldNotExecuteSqlCommandWhenEntityValuesContainsOnlyDefaultEntityFields()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "CreatedOn", DateTime.UtcNow } };

            TestedService.Update(entityId, newValues);
            var lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }

        [Fact]
        public void ShouldNotExecuteSqlCommandWhenEntityValuesContainsFieldsNotPresentedInEntity()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "TestedNotExistedPropeprty", DateTime.UtcNow } };

            TestedService.Update(entityId, newValues);
            var lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }

        [Fact]
        public void ShouldNotExecuteSqlCommandWhenEntityValuesContainsDefaultValues()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "IntField", 0 } };

            TestedService.Update(entityId, newValues);
            var lastCommand = LastCommand;

            Assert.Null(lastCommand);
        }

        [Fact]
        public void ShouldThrowExceptionWhenAffectedRowsIsZero()
        {
            string expectedExceptionMessage = "Update command performed with empty result, no record was updated.";
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "IntField", 10 } };
            TestedAffectedRowsCount = 0;

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldUpdateRecord()
        {
            Guid entityId = Guid.NewGuid();
            int newIntFieldValue = 15;
            string expectedSqlQuery = $"UPDATE [{TestedTableName}] SET [IntField] = @P1, [ModifiedOn] = @P2 WHERE [Id] = @P3";

            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "IntField", newIntFieldValue } };
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] {
                new KeyValuePair<string, object>("@P1", newIntFieldValue),
                new KeyValuePair<string, object>("@P2", DateTime.UtcNow),
                new KeyValuePair<string, object>("@P3", entityId),
            };
            TestedAffectedRowsCount = 10;

            TestedService.Update(entityId, newValues);
            var lastCommand = LastCommand;


            Assert.NotNull(lastCommand);
            Assert.Equal(expectedSqlQuery, lastCommand.Value.Key);
            Assert.NotNull(lastCommand.Value.Value);
            AssertArguments(expectedArguments, lastCommand.Value.Value);
        }
    }
}
