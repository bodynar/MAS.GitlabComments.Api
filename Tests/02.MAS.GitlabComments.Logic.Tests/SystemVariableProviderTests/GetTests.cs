namespace MAS.GitlabComments.Logic.Tests.SystemVariableProviderTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.Logic.Models;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Xunit;

    /// <summary>
    /// Tests for 2 API:
    ///     <see cref="SystemVariableProvider.Get(Guid)"/>,
    ///     <see cref="SystemVariableProvider.Get(string)"/>
    /// </summary>
    public sealed class GetTests
    {
        #region By variable id

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.Get(Guid)"/>
        /// </summary>
        public sealed class GetByIdTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldNotReturnAnything_WhenVariableIdIsDefault()
            {
                Guid variableId = default;

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableIsNotFoundInDataStore()
            {
                Guid variableId = Guid.NewGuid();
                ShouldReturnEntityFromDataProvider = false;

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsEmpty()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = string.Empty;

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsWhiteSpaceOnly()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "      ";

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNotValid()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "NotValid";

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNone()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "None";

                SysVariable result = TestedService.Get(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldModelWithStringType()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "String";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithIntegerType()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "Int";
                Type expectedType = typeof(long);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithDecimalType()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "Decimal";
                Type expectedType = typeof(double);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithBooleanType()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "Bool";
                Type expectedType = typeof(bool);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithObjectType()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "Json";
                Type expectedType = typeof(object);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithStringTypeWithLowerCase()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "string";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithStringTypeWithDifferentCase()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "sTrInG";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableId);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }
        }

        #endregion

        #region By variable code

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.Get(string)"/>
        /// </summary>
        public sealed class GetByCodeTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsDefault()
            {
                string variableCode = default;

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsEmpty()
            {
                string variableCode = string.Empty;

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsWhiteSpaceOnly()
            {
                string variableCode = "     ";

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnythingByFilter_WhenVariableIsNotFoundInDataStore()
            {
                string variableCode = "variableCode";
                string filterName = "CodeEquality";
                string expectedFieldName = nameof(SystemVariable.Code);
                ComparisonType expectedComparisonType = ComparisonType.Equal;
                int expectedFiltersCount = 1;

                ShouldReturnEntityFromDataProvider = false;

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
                Assert.Equal(expectedFiltersCount, LastFilter.Items.Count());

                FilterItem firstFiltere = LastFilter.Items.First();
                Assert.Equal(filterName, firstFiltere.Name);
                Assert.Equal(expectedComparisonType, firstFiltere.LogicalComparisonType);
                Assert.Equal(expectedFieldName, firstFiltere.FieldName);
                Assert.Equal(variableCode, firstFiltere.Value);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsEmpty()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = string.Empty;

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsWhiteSpaceOnly()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "      ";

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNotValid()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "NotValid";

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNone()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "None";

                SysVariable result = TestedService.Get(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldModelWithStringType()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "String";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithIntegerType()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "Int";
                Type expectedType = typeof(long);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithDecimalType()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "Decimal";
                Type expectedType = typeof(double);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithBooleanType()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "Bool";
                Type expectedType = typeof(bool);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithObjectType()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "Json";
                Type expectedType = typeof(object);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithStringTypeWithLowerCase()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "string";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }

            [Fact]
            public void ShouldModelWithStringTypeWithDifferentCase()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "sTrInG";
                Type expectedType = typeof(string);

                SysVariable result = TestedService.Get(variableCode);

                Assert.NotNull(result);
                Assert.Equal(expectedType, result.UnderlyingType);
            }
        }

        #endregion
    }
}
