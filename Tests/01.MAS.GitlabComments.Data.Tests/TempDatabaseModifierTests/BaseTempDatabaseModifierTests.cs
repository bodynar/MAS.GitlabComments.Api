namespace MAS.GitlabComments.DataAccess.Tests.TempDatabaseModifierTests
{
    using System.Data;

    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.DataAccess.Services.Implementations;

    using Moq;

    public abstract class BaseTempDatabaseModifierTests
    {
        /// <summary>
        /// Instance of <see cref="TempDatabaseModifier"/> for tests
        /// </summary>
        protected TempDatabaseModifier TestedService { get; }

        /// <summary>
        /// Is sql command was executed
        /// </summary>
        protected bool NonQueryExecuted { get; private set; }

        /// <summary>
        /// Value, saved in command sql text
        /// </summary>
        protected string CommandText { get; private set; }

        /// <summary>
        /// Initializing <see cref="BaseTempDatabaseModifierTests"/> with setting up test context
        /// </summary>
        protected BaseTempDatabaseModifierTests()
        {
            var connectionFactory = GetDbConnectionFactory();

            TestedService = new TempDatabaseModifier(connectionFactory);
        }

        /// <summary>
        /// Get mock of <see cref="IDbConnectionFactory"/>, required by tested service
        /// </summary>
        /// <returns>Instance of <see cref="IDbConnectionFactory"/></returns>
        private IDbConnectionFactory GetDbConnectionFactory()
        {
            var mock = new Mock<IDbConnectionFactory>();

            var connectionMock = new Mock<IDbConnection>();
            var commandMock = new Mock<IDbCommand>();

            commandMock
                .SetupSet(x => x.CommandText = It.IsAny<string>())
                .Callback<string>(sql => CommandText = sql);

            commandMock
                .Setup(x => x.ExecuteNonQuery())
                .Callback(() => NonQueryExecuted = true)
                .Returns(0);

            connectionMock
                .Setup(x => x.CreateCommand())
                .Returns(commandMock.Object);

            mock
                .Setup(x => x.CreateDbConnection())
                .Returns(connectionMock.Object);

            return mock.Object;
        }
    }
}
