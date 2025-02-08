namespace MAS.GitlabComments.DataAccess.Tests.ModelsTests.ComplexColumnTests
{
    using System;

    using MAS.GitlabComments.DataAccess.Select;

    using Xunit;

    public sealed class ToQueryTests
    {
        [Fact]
        public void ShouldThrowArgumentExceptionWhenTableAliasIsEmpty()
        {
            string expectedExceptionMessage = "Required string data is empty: TableAlias = \"\", Name = \"\"";
            ComplexColumn testedInstance = new ComplexColumn();

            Exception exception = Record.Exception(
                () => testedInstance.ToQuery(DatabaseType.MSSQL)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldThrowArgumentExceptionWhenNameIsEmpty()
        {
            string expectedExceptionMessage = "Required string data is empty: TableAlias = \"TableAlias\", Name = \"\"";
            ComplexColumn testedInstance = new ComplexColumn() { TableAlias = "TableAlias" };

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
            ComplexColumn testedInstance = new ComplexColumn() { TableAlias = "TableAlias", Name = "Name" };

            Exception exception = Record.Exception(
                () => testedInstance.ToQuery((DatabaseType)100)
            );

            Assert.NotNull(exception);
            Assert.IsType<NotImplementedException>(exception);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }

        [Fact]
        public void ShouldReturnQueryForMsSqlWithoutAliasWhenAliasIsEmpty()
        {
            string name = "Name";
            string tableAlias = "TableAlias";

            string expectedResult = $"[{tableAlias}].[{name}]";
            ComplexColumn testedInstance = new ComplexColumn() {
                TableAlias = tableAlias,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.MSSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForMsSqlWithoutAliasWhenAliasIsEqualToName()
        {
            string name = "Name";
            string tableAlias = "TableAlias";

            string expectedResult = $"[{tableAlias}].[{name}]";
            ComplexColumn testedInstance = new ComplexColumn()
            {
                TableAlias = tableAlias,
                Alias = name,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.MSSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForMsSqlWithAlias()
        {
            string name = "Name";
            string alias = "Alias";
            string tableAlias = "TableAlias";

            string expectedResult = $"[{tableAlias}].[{name}] AS [{alias}]";
            ComplexColumn testedInstance = new ComplexColumn()
            {
                TableAlias = tableAlias,
                Alias = alias,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.MSSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForPgSqlWithoutAliasWhenAliasIsEmpty()
        {
            string name = "Name";
            string tableAlias = "TableAlias";

            string expectedResult = $"\"{tableAlias}\".\"{name}\"";
            ComplexColumn testedInstance = new ComplexColumn()
            {
                TableAlias = tableAlias,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.PGSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForPgSqlWithoutAliasWhenAliasIsEqualToName()
        {
            string name = "Name";
            string tableAlias = "TableAlias";

            string expectedResult = $"\"{tableAlias}\".\"{name}\"";
            ComplexColumn testedInstance = new ComplexColumn()
            {
                TableAlias = tableAlias,
                Alias = name,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.PGSQL);


            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ShouldReturnQueryForPgSqlWithAlias()
        {
            string name = "Name";
            string alias = "Alias";
            string tableAlias = "TableAlias";

            string expectedResult = $"\"{tableAlias}\".\"{name}\" AS \"{alias}\"";
            ComplexColumn testedInstance = new ComplexColumn()
            {
                TableAlias = tableAlias,
                Alias = alias,
                Name = name
            };


            string result = testedInstance.ToQuery(DatabaseType.PGSQL);


            Assert.Equal(expectedResult, result);
        }
    }
}
