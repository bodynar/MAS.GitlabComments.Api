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
            public void ShouldDoNothingWhenVariableIdIsDefault()
            {
                Guid variableId = default;
                int value = 0;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableIsNotFoundInDataStore()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                ShouldReturnEntityFromDataProvider = false;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsEmpty()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = string.Empty;

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsWhiteSpaceOnly()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "      ";

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsNotValid()
            {
                Guid variableId = Guid.NewGuid();
                int value = 0;
                TestedEntity.Type = "NotValid";

                TestedService.Set(variableId, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsNone()
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
            public void ShouldDoNothingWhenVariableCodeIsDefault()
            {
                string variableCode = default;
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableCodeIsEmpty()
            {
                string variableCode = string.Empty;
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableCodeIsWhiteSpaceOnly()
            {
                string variableCode = "    ";
                int value = 0;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableIsNotFoundInDataStore()
            {
                string variableCode = "variableCode";
                int value = 0;
                ShouldReturnEntityFromDataProvider = false;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsEmpty()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = string.Empty;

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsWhiteSpaceOnly()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "      ";

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsNotValid()
            {
                string variableCode = "variableCode";
                int value = 0;
                TestedEntity.Type = "NotValid";

                TestedService.Set(variableCode, value);

                Assert.Null(LastCommand);
            }

            [Fact]
            public void ShouldDoNothingWhenVariableTypeIsNone()
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
