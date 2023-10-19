namespace MAS.GitlabComments.Logic.Tests.SystemVariableProviderTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Moq;

    using Xunit;

    /// <summary>
    /// Base class for <see cref="CommentService"/> tests
    /// </summary>
    public abstract class BaseSystemVariableProviderTests
    {
        /// <summary>
        /// Last called command back-field
        /// </summary>
        private KeyValuePair<string, IEnumerable<object>>? lastCommand;

        /// <summary>
        /// Instance of <see cref="SystemVariableProvider"/> for tests
        /// </summary>
        protected SystemVariableProvider TestedService { get; }

        /// <summary>
        /// Pre-condition for returning <see cref="TestedEntity"/> from <see cref="IDataProvider{TEntity}"/>
        /// </summary>
        protected bool ShouldReturnEntityFromDataProvider { get; set; } = true;

        /// <summary>
        /// Single entity that could be obtain from <see cref="IDataProvider{TEntity}"/>
        /// </summary>
        protected SystemVariable TestedEntity { get; }

        /// <summary>
        /// Last applied filter from <see cref="IDataProvider{TEntity}.Where(FilterGroup)"/>
        /// </summary>
        protected FilterGroup LastFilter { get; private set; }

        /// <summary>
        /// Last called command of <see cref="IDataProvider{TEntity}"/> - pair of command name and arguments
        /// </summary>
        protected KeyValuePair<string, IEnumerable<object>>? LastCommand
        {
            get
            {
                if (!lastCommand.HasValue)
                {
                    return null;
                }

                var copy = new KeyValuePair<string, IEnumerable<object>>(lastCommand.Value.Key, lastCommand.Value.Value);

                lastCommand = null;

                return copy;
            }
        }

        /// <summary>
        /// Initializing <see cref="BaseSystemVariableProviderTests"/> with setup all required environment
        /// </summary>
        public BaseSystemVariableProviderTests()
        {
            var dataProvider = GetMockDataProvider();

            TestedService = new SystemVariableProvider(dataProvider);

            TestedEntity = new SystemVariable
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                Caption = "TestedCaption",
                Code = "TestedCode",
                RawValue = "TestedRawValue",
            };
        }

        /// <summary>
        /// Assert for last <see cref="IDataProvider{TEntity}"/> called command
        /// </summary>
        /// <param name="serviceAction">Tested service action</param>
        /// <param name="expectedCommandName">Expected called command name</param>
        /// <param name="expectedCommandArguments">Expected arguments of called command</param>
        protected void ShouldExecuteCommand(Action serviceAction, string expectedCommandName, IEnumerable<object> expectedCommandArguments)
        {
            int expectedArgumentsCount = expectedCommandArguments.Count();

            serviceAction.Invoke();
            KeyValuePair<string, IEnumerable<object>>? lastCommand = LastCommand;

            Assert.NotNull(lastCommand);

            var lastCommandAsKeyValuePair = lastCommand.Value;

            Assert.Equal(expectedCommandName, lastCommandAsKeyValuePair.Key);
            Assert.Equal(expectedArgumentsCount, lastCommandAsKeyValuePair.Value.Count());

            for (int i = 0; i < expectedArgumentsCount; i++)
            {
                var expectedArgument = expectedCommandArguments.ElementAt(i);
                var actualArgument = lastCommandAsKeyValuePair.Value.ElementAt(i);

                Assert.NotNull(actualArgument);
                Assert.Equal(actualArgument, expectedArgument);
            }
        }

        /// <summary>
        /// Configure mock object of system variable data provider for system variable service
        /// </summary>
        /// <returns>Configured mock object of <see cref="IDataProvider{TEntity}"/></returns>
        private IDataProvider<SystemVariable> GetMockDataProvider()
        {
            var mockDataProvider = new Mock<IDataProvider<SystemVariable>>();

            mockDataProvider
                .Setup(x => x.Where(It.IsAny<FilterGroup>()))
                .Callback<FilterGroup>(filter => LastFilter = filter)
                .Returns(() => ShouldReturnEntityFromDataProvider
                    ? new SystemVariable[] { TestedEntity }
                    : new SystemVariable[] { }
                );

            mockDataProvider
                .Setup(x => x.Get(It.IsAny<Guid>()))
                .Returns(() => ShouldReturnEntityFromDataProvider ? TestedEntity : default);

            mockDataProvider
                .Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<Guid, IDictionary<string, object>>((id, data) =>
                {
                    lastCommand = new KeyValuePair<string, IEnumerable<object>>(
                        nameof(mockDataProvider.Object.Update),
                        new object[] { id, data }
                    );
                });

            return mockDataProvider.Object;
        }
    }
}
