﻿namespace MAS.GitlabComments.Tests.SqlDataProviderTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;

    using MAS.GitlabComments.Services;
    using MAS.GitlabComments.Services.Implementations;

    using Moq;

    using Xunit;

    /// <summary>
    /// Fake entity for <see cref="SqlDataProvider{TEntity}"/> tests
    /// </summary>
    public sealed class TestedDataProviderEntity
    {
        /// <summary>
        /// Fake identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mock field with DateTime type
        /// </summary>
        public DateTime DateTimeField { get; set; }

        /// <summary>
        /// Mock field with string type
        /// </summary>
        public string StringField { get; set; }

        /// <summary>
        /// Mock field with int type
        /// </summary>
        public int IntField { get; set; }
    }

    /// <summary>
    /// Base class for <see cref="SqlDataProvider{TEntity}"/> tests
    /// </summary>
    public abstract class BaseSqlDataProviderTests
    {
        /// <summary>
        /// Instance of <see cref="SqlDataProvider{TEntity}"/> for tests
        /// </summary>
        protected SqlDataProvider<TestedDataProviderEntity> TestedService { get; }

        /// <summary>
        /// Name of table for tests
        /// </summary>
        protected string TestedTableName
            => $"{nameof(TestedDataProviderEntity)}s";

        /// <summary>
        /// Last called command back-field
        /// </summary>
        private KeyValuePair<string, object>? lastCommand;

        /// <summary>
        /// Last called command of <see cref="IDbAdapter"/> - pair of sql command and arguments
        /// </summary>
        protected KeyValuePair<string, object>? LastCommand
        {
            get
            {
                if (!lastCommand.HasValue)
                {
                    return null;
                }

                var copy = new KeyValuePair<string, object>(lastCommand.Value.Key, lastCommand.Value.Value);

                lastCommand = null;

                return copy;
            }
        }

        /// <summary>
        /// Last called query back-field
        /// </summary>
        private KeyValuePair<string, object>? lastQuery;

        /// <summary>
        /// Last called query of <see cref="IDbAdapter"/> - pair of sql query and arguments
        /// </summary>
        protected KeyValuePair<string, object>? LastQuery
        {
            get
            {
                if (!lastQuery.HasValue)
                {
                    return null;
                }

                var copy = new KeyValuePair<string, object>(lastQuery.Value.Key, lastQuery.Value.Value);

                lastQuery = null;

                return copy;
            }
        }

        private IEnumerable<string> ParamNamesToExcludeFromCheck
            => new[] { "Id", "CreatedOn", "ModifiedOn" };

        /// <summary>
        /// Initializing <see cref="BaseSqlDataProviderTests"/> with setup'n all required environment
        /// </summary>
        protected BaseSqlDataProviderTests()
        {
            var (dbConnectionFactory, dbAdapter) = GetMockDbAdapter();
            TestedService = new SqlDataProvider<TestedDataProviderEntity>(dbConnectionFactory, dbAdapter);
        }

        #region Private methods

        /// <summary>
        /// Get all required dependencies for <see cref="SqlDataProvider{TEntity}"/> presented as mocks
        /// </summary>
        /// <exception cref="Exception">Last query has value before test execution</exception>
        /// <exception cref="Exception">Last command has value before test execution</exception>
        /// <returns>Pair of dependecies <see cref="IDbConnectionFactory"/> and <see cref="IDbAdapter"/></returns>
        private (IDbConnectionFactory, IDbAdapter) GetMockDbAdapter()
        {
            var mockConnectionFactory = new Mock<IDbConnectionFactory>();

            mockConnectionFactory
                .Setup(x => x.CreateDbConnection())
                .Returns<IDbConnection>(null);

            var mockDbAdapter = new Mock<IDbAdapter>();

            mockDbAdapter
                .Setup(x => x.Query<TestedDataProviderEntity>(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>()))
                .Callback<IDbConnection, string, object>((_, sql, args) =>
                {
                    if (lastQuery.HasValue)
                    {
                        throw new Exception($"{nameof(LastQuery)} is not empty");
                    }

                    lastQuery = new KeyValuePair<string, object>(sql, args);
                })
                .Returns(Enumerable.Empty<TestedDataProviderEntity>());

            mockDbAdapter
                .Setup(x => x.Execute(It.IsAny<IDbConnection>(), It.IsAny<string>(), It.IsAny<object>()))
                .Callback<IDbConnection, string, object>((_, sql, args) =>
                {
                    if (lastCommand.HasValue)
                    {
                        throw new Exception($"{nameof(LastCommand)} is not empty");
                    }

                    lastCommand = new KeyValuePair<string, object>(sql, args);
                })
                .Returns(0);

            return (mockConnectionFactory.Object, mockDbAdapter.Object);
        }

        /// <summary>
        /// Assert sql configuration arguments.
        /// Asserts equality of argument keys and values sequently
        /// </summary>
        /// <param name="expected">Expected arguments</param>
        /// <param name="actual">Actual arguments</param>
        private void AssertArguments(IEnumerable<KeyValuePair<string, object>> expected, ExpandoObject actual)
        {
            var actualArguments = actual.Where(x => !ParamNamesToExcludeFromCheck.Contains(x.Key));
            var objectKeyNames = actualArguments.Select(x => x.Key);

            var hasNotPresentedKeys = expected.Any(pair => !objectKeyNames.Contains(pair.Key));

            Assert.False(hasNotPresentedKeys);

            foreach (var pair in actualArguments)
            {
                var expectedValue = expected.First(x => x.Key == pair.Key).Value;

                Assert.Equal(expectedValue, pair.Value);
            }
        }

        #endregion
        
        /// <summary>
        /// Assert sql configuration arguments.
        /// Asserts equality of argument keys and values sequently
        /// </summary>
        /// <param name="expected">Expected arguments</param>
        /// <param name="actual">Actual arguments</param>
        protected void AssertArguments(IEnumerable<KeyValuePair<string, object>> expected, object actual)
        {
            if (actual is ExpandoObject)
            {
                AssertArguments(expected, actual as ExpandoObject);
            }
            else
            {
                var objectKeys = actual.GetType().GetProperties();
                var objectKeyNames = objectKeys.Select(x => x.Name).Except(ParamNamesToExcludeFromCheck);

                var hasNotPresentedKeys = expected.Any(pair => !objectKeyNames.Contains(pair.Key));

                Assert.False(hasNotPresentedKeys);

                foreach (var key in objectKeys)
                {
                    var actualValue = key.GetValue(actual);
                    var expectedValue = expected.First(x => x.Key == key.Name).Value;

                    Assert.Equal(expectedValue, actualValue);
                }
            }
        }
    }
}