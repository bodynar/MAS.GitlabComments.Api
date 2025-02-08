namespace MAS.GitlabComments.DataAccess.Tests.ModelsTests.TableJoinDataTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Select;

    using Xunit;

    public sealed class ToQueryTests
    {
        [Fact]
        public void ShouldThrowArgumentExceptionWhenJoinTypeIsNone()
        {
            string expectedExceptionMessage = "\"Join type\" must be set";
            TableJoinData testedInstance = new TableJoinData("configuration") {
                JoinType = TableJoinType.None
            };

            Exception exception = Record.Exception(
                () => testedInstance.ToQuery(DatabaseType.MSSQL)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldThrowNotImplementedExceptionWhenDatabaseTypeIsUnknown()
        {
            string expectedExceptionMessage = "Handler for DB type \"100\" not implemented yet.";
            TableJoinData testedInstance = new TableJoinData("configuration")
            {
                JoinType = TableJoinType.FullOuter
            };

            Exception exception = Record.Exception(
                () => testedInstance.ToQuery((DatabaseType)100)
            );

            Assert.NotNull(exception);
            Assert.IsType<NotImplementedException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldReturnQueryForMsSql()
        {
            string rightTableName = "rightTableName";
            string alias = "alias";
            string rightTableRelationColumn = "rightTableRelationColumn";
            string leftTableName = "leftTableName";
            string leftTableRelationColumn = "leftTableRelationColumn";

            string expectedResult = $"FULL OUTER JOIN [{rightTableName}] AS [{alias}] WITH(NOLOCK) ON ([{alias}].[{rightTableRelationColumn}] = [{leftTableName}].[{leftTableRelationColumn}])";
            TableJoinData testedInstance = new TableJoinData("configuration")
            {
                Alias = alias,
                JoinType = TableJoinType.FullOuter,
                LeftTableName = leftTableName,
                LeftTableRelationColumn = leftTableRelationColumn,
                RightTableName = rightTableName,
                RightTableRelationColumn = rightTableRelationColumn,
            };


            string result = testedInstance.ToQuery(DatabaseType.MSSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForPgSql()
        {
            string rightTableName = "rightTableName";
            string alias = "alias";
            string rightTableRelationColumn = "rightTableRelationColumn";
            string leftTableName = "leftTableName";
            string leftTableRelationColumn = "leftTableRelationColumn";

            string expectedResult = $"FULL OUTER JOIN \"{rightTableName}\" AS \"{alias}\" ON (\"{alias}\".\"{rightTableRelationColumn}\" = \"{leftTableName}\".\"{leftTableRelationColumn}\")";
            TableJoinData testedInstance = new TableJoinData("configuration")
            {
                Alias = alias,
                JoinType = TableJoinType.FullOuter,
                LeftTableName = leftTableName,
                LeftTableRelationColumn = leftTableRelationColumn,
                RightTableName = rightTableName,
                RightTableRelationColumn = rightTableRelationColumn,
            };


            string result = testedInstance.ToQuery(DatabaseType.PGSQL);


            Assert.Equal(expectedResult, result);
        }
    }
}
