namespace MAS.GitlabComments.DataAccess.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Exceptions;

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
        public void ShouldThrowQueryExecutionExceptionWhenEntityValuesContainsOnlyDefaultEntityFields()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "CreatedOn", DateTime.UtcNow } };
            string expectedExceptionMessage = "Operation cannot be performed due to empty entity values dictionary";
            QueryExecutionExceptionState executionExceptionState = QueryExecutionExceptionState.Before;

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);
            Assert.Equal(expectedExceptionMessage, castedException.Message);
            Assert.Equal(executionExceptionState, castedException.State);
        }

        [Fact]
        public void ShouldThrowQueryExecutionExceptionWhenEntityValuesContainsFieldsNotPresentedInEntity()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "TestedNotExistedPropeprty", DateTime.UtcNow } };
            string expectedExceptionMessage = "Operation cannot be performed due to empty entity values dictionary";
            QueryExecutionExceptionState executionExceptionState = QueryExecutionExceptionState.Before;

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);
            Assert.Equal(expectedExceptionMessage, castedException.Message);
            Assert.Equal(executionExceptionState, castedException.State);
        }

        [Fact]
        public void ShouldThrowQueryExecutionExceptionWhenEntityValuesContainsOnlyPairWithNullValues()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "IntField", null } };
            string expectedExceptionMessage = "Operation cannot be performed due to empty entity values dictionary";
            QueryExecutionExceptionState executionExceptionState = QueryExecutionExceptionState.Before;

            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );

            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);
            Assert.Equal(expectedExceptionMessage, castedException.Message);
            Assert.Equal(executionExceptionState, castedException.State);
        }

        [Fact]
        public void ShouldThrowExceptionWhenAffectedRowsIsZero()
        {
            Guid entityId = Guid.NewGuid();
            IDictionary<string, object> newValues = new Dictionary<string, object>() { { "IntField", 10 } };
            TestedAffectedRowsCount = 0;

            var expectedMessage = "Update command performed with empty result, no record was updates";
            var expectedState = QueryExecutionExceptionState.After;


            Exception exception =
                Record.Exception(
                    () => TestedService.Update(entityId, newValues)
                );


            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);

            Assert.Equal(expectedState, castedException.State);
            Assert.Equal(expectedMessage, castedException.Message);
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
