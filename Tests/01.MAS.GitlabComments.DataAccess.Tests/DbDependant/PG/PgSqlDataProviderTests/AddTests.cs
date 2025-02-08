namespace MAS.GitlabComments.DataAccess.Tests.PgSqlDataProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Exceptions;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG;

    using Xunit;

    /// <summary>
    /// Test suit for method <see cref="MsSqlDataProvider{TEntity}.Add(TEntity)"/>
    /// </summary>
    public sealed class AddTests : BasePgSqlDataProviderTests
    {
        [Fact]
        public void ShouldThrowArgumentNullException_WhenEntityIsNull()
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
        public void ShouldAddEntity()
        {
            string expectedSqlQuery = $"INSERT INTO \"{TestedTableName}\" (\"StringField\", \"IntField\", \"Id\", \"CreatedOn\", \"ModifiedOn\") VALUES (@P1, @P2, @P3, @P4, @P5)";

            TestedDataProviderEntity entity = new() { Id = Guid.NewGuid(), StringField = "Some test data" };
            IEnumerable<KeyValuePair<string, object>> expectedArguments = new[] {
                new KeyValuePair<string, object>("@P1", entity.StringField),
                new KeyValuePair<string, object>("@P2", entity.IntField),
                new KeyValuePair<string, object>("@P3", entity.Id),
                new KeyValuePair<string, object>("@P4", DateTime.UtcNow),
                new KeyValuePair<string, object>("@P5", DateTime.UtcNow),
            };
            TestedAffectedRowsCount = 10;


            var result = TestedService.Add(entity);
            var lastCommand = LastCommand;


            Assert.NotNull(lastCommand);
            Assert.Equal(expectedSqlQuery, lastCommand.Value.Key);
            Assert.NotNull(lastCommand.Value.Value);
            Assert.NotEqual(Guid.Empty, result);
            AssertArguments(expectedArguments, lastCommand.Value.Value, new[] { "@P3" });
        }

        [Fact]
        public void ShouldThrowException_WhenAffectedRowsIsZero()
        {
            string expectedErrorMessage = "Insert command performed with empty result, no record was added";
            TestedDataProviderEntity entity = new() { Id = Guid.NewGuid(), StringField = "Some test data" };
            TestedAffectedRowsCount = 0;

            Exception exception =
                Record.Exception(
                    () => TestedService.Add(entity)
                );

            Assert.NotNull(exception);
            Assert.Equal(expectedErrorMessage, exception.Message);
        }

        [Fact]
        public void ShouldThrowQueryExecutionExceptionWithBeforeState_WhenEntityClassIsEmpty()
        {
            var (df, da, fb, ccqb) = GetServiceDependencies();
            var testedService = new PgSqlDataProvider<TestedEmptyDataProviderEntity>(df, da, fb, ccqb); // manually activate because of entity type
            var entity = new TestedEmptyDataProviderEntity();
            var expectedMessage = "Operation cannot be performed due to empty model \"MAS.GitlabComments.DataAccess.Tests.PgSqlDataProviderTests.TestedEmptyDataProviderEntity\"";
            var expectedState = QueryExecutionExceptionState.Before;


            Exception exception =
                Record.Exception(
                    () => testedService.Add(entity)
                );


            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);

            Assert.Equal(expectedState, castedException.State);
            Assert.Equal(expectedMessage, castedException.Message);
        }

        [Fact]
        public void ShouldThrowQueryExecutionExceptionWithAfterState_WhenEntityNotInserted()
        {
            var expectedSqlQuery = $"INSERT INTO \"{TestedTableName}\" (\"StringField\", \"IntField\", \"Id\", \"CreatedOn\", \"ModifiedOn\") VALUES (@P1, @P2, @P3, @P4, @P5)";

            var entity = new TestedDataProviderEntity() { Id = Guid.NewGuid(), StringField = "Some test data" };
            var expectedArguments = new[] {
                new KeyValuePair<string, object>("@P1", entity.StringField),
                new KeyValuePair<string, object>("@P2", entity.IntField),
                new KeyValuePair<string, object>("@P3", entity.Id),
                new KeyValuePair<string, object>("@P4", DateTime.UtcNow),
                new KeyValuePair<string, object>("@P5", DateTime.UtcNow),
            };
            TestedAffectedRowsCount = 0;


            var expectedMessage = "Insert command performed with empty result, no record was added";
            var expectedState = QueryExecutionExceptionState.After;

            Exception exception =
                Record.Exception(
                    () => TestedService.Add(entity)
                );


            Assert.NotNull(exception);
            Assert.IsType<QueryExecutionException>(exception);

            QueryExecutionException castedException = exception as QueryExecutionException;
            Assert.NotNull(castedException);

            Assert.Equal(expectedState, castedException.State);
            Assert.Equal(expectedMessage, castedException.Message);
        }
    }

    public sealed class TestedEmptyDataProviderEntity : BaseEntity { }
}
