namespace MAS.GitlabComments.Logic.Tests.SystemVariableProviderTests
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Services.Implementations;

    using Xunit;

    /// <summary>
    /// Tests for 2 API:
    ///     <see cref="SystemVariableProvider.Set{TValue}(string, TValue)"/>,
    ///     <see cref="SystemVariableProvider.Set{TValue}(Guid, TValue)"/>
    /// </summary>
    public sealed class SetTests
    {
        #region By variable id

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.Set{TValue}(Guid, TValue)"/>
        /// </summary>
        public sealed class SetByIdTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldDoNothing_WhenVariableIdIsDefault()
            {
                Guid variableId = default;
                int value = 0;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableIsNotFoundInDataStore()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                ShouldReturnEntityFromDataProvider = false;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsEmpty()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = string.Empty;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsWhiteSpaceOnly()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "      ";

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsNotValid()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "NotValid";

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsNone()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "None";

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldUpdateVariableWithEmptyValue()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "String";
                string expectedCommandName = "Update";
                IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "RawValue", "" } };

                Action testedAction = () => TestedService.Set<string>(variableId, null);

                ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { variableId, secondExpectedArgument });
            }

            [Fact]
            public void ShouldUpdateVariableWithNewValue()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "String";
                string expectedCommandName = "Update";
                IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "RawValue", "0" } };

                Action testedAction = () => TestedService.Set(variableId, value);

                ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { variableId, secondExpectedArgument });
            }
        }

        #endregion

        #region By variable code

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.Set{TValue}(string, TValue)"/>
        /// </summary>
        public sealed class SetByCodeTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldDoNothing_WhenVariableCodeIsDefault()
            {
                string variableCode = default;
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableCodeIsEmpty()
            {
                string variableCode = string.Empty;
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableCodeIsWhiteSpaceOnly()
            {
                string variableCode = "    ";
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableIsNotFoundInDataStore()
            {
                string variableCode = "variableCode";
                int value = 0;
                ShouldReturnEntityFromDataProvider = false;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsEmpty()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = string.Empty;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsWhiteSpaceOnly()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "      ";

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsNotValid()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "NotValid";

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothing_WhenVariableTypeIsNone()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "None";

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldUpdateVariableWithEmptyValue()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "String";
                string expectedCommandName = "Update";
                IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "RawValue", "" } };

                Action testedAction = () => TestedService.Set<string>(variableCode, null);

                ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { TestedEntity.Id, secondExpectedArgument });
            }

            [Fact]
            public void ShouldUpdateVariableWithNewValue()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "String";
                string expectedCommandName = "Update";
                IDictionary<string, object> secondExpectedArgument = new Dictionary<string, object> { { "RawValue", "0" } };

                Action testedAction = () => TestedService.Set(variableCode, value);

                ShouldExecuteCommand(testedAction, expectedCommandName, new object[] { TestedEntity.Id, secondExpectedArgument });
            }
        }

        #endregion
    }
}
