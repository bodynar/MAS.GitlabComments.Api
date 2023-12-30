namespace MAS.GitlabComments.Logic.Tests.SystemVariableProviderTests
{
    using System;
    using System.Linq;

    using MAS.GitlabComments.Data;
    using MAS.GitlabComments.DataAccess.Filter;
    using MAS.GitlabComments.Logic.Services.Implementations;

    using Newtonsoft.Json;

    using Xunit;

    /// <summary>
    /// Tests for 2 API:
    ///     <see cref="SystemVariableProvider.GetValue{TValue}(Guid"/>,
    ///     <see cref="SystemVariableProvider.GetValue{TValue}(string)"/>
    /// </summary>
    public sealed class GetValueTests
    {
        public class TestClass
        {
            public int IntegerValue { get; set; }

            public bool BooleanValue { get; set; }

            public string StringValue { get; set; }
        }

        #region By variable id

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.GetValue{TValue}(Guid)"/>
        /// </summary>
        public sealed class GetValueByIdTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldNotReturnAnything_WhenTypeIsUshort()
            {
                Guid variableId = default;
                ushort expectedValue = default;

                ushort result = TestedService.GetValue<ushort>(variableId);

                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenTypeIsUint()
            {
                Guid variableId = default;
                uint expectedValue = default;

                uint result = TestedService.GetValue<ushort>(variableId);

                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenTypeIsUlong()
            {
                Guid variableId = default;
                ulong expectedValue = default;

                ulong result = TestedService.GetValue<ushort>(variableId);

                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableIdIsDefault()
            {
                Guid variableId = default;

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableIsNotFoundInDataStore()
            {
                Guid variableId = Guid.NewGuid();
                ShouldReturnEntityFromDataProvider = false;

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsEmpty()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = string.Empty;

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsWhiteSpaceOnly()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "      ";

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNotValid()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "NotValid";

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNone()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "None";

                string result = TestedService.GetValue<string>(variableId);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldReturnStringValueByStringVariable()
            {
                Guid variableId = Guid.NewGuid();
                string expectedValue = "rawValue";
                TestedEntity.Type = "string";
                TestedEntity.RawValue = expectedValue;

                string result = TestedService.GetValue<string>(variableId);

                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnLongValueByIntVariable()
            {
                Guid variableId = Guid.NewGuid();
                long expectedValue = 10;
                TestedEntity.Type = "Int";
                TestedEntity.RawValue = expectedValue.ToString();

                long result = TestedService.GetValue<long>(variableId);

                Assert.NotEqual(default(long), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnDoubleValueByDecimalVariable()
            {
                Guid variableId = Guid.NewGuid();
                double expectedValue = 10.0;
                TestedEntity.Type = "Decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                double result = TestedService.GetValue<double>(variableId);

                Assert.NotEqual(default(double), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnBoolValueByBoolVariable()
            {
                Guid variableId = Guid.NewGuid();
                bool expectedValue = true;
                TestedEntity.Type = "Bool";
                TestedEntity.RawValue = expectedValue.ToString();

                bool result = TestedService.GetValue<bool>(variableId);

                Assert.NotEqual(default(bool), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnObjectValueByJsonVariable()
            {
                Guid variableId = Guid.NewGuid();
                TestClass expectedValue = new TestClass { IntegerValue = 10, BooleanValue = true, StringValue = " string " };
                TestedEntity.Type = "Json";
                TestedEntity.RawValue = JsonConvert.SerializeObject(expectedValue);

                TestClass result = TestedService.GetValue<TestClass>(variableId);

                Assert.NotEqual(default(object), result);
                Assert.Equal(expectedValue.IntegerValue, result.IntegerValue);
                Assert.Equal(expectedValue.BooleanValue, result.BooleanValue);
                Assert.Equal(expectedValue.StringValue, result.StringValue);
            }

            [Fact]
            public void ShouldReturnCharValueByStringVariable()
            {
                Guid variableId = Guid.NewGuid();
                char expectedValue = 'r';
                TestedEntity.Type = "string";
                TestedEntity.RawValue = "rawValue";

                char result = TestedService.GetValue<char>(variableId);

                Assert.NotEqual(default(char), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnCharArrayValueByStringVariable()
            {
                Guid variableId = Guid.NewGuid();
                char[] expectedValue = new char[] { 'r', 'a', 'w', 'V', 'a', 'l', 'u', 'e' };
                TestedEntity.Type = "string";
                TestedEntity.RawValue = "rawValue";

                char[] result = TestedService.GetValue<char[]>(variableId);

                Assert.NotEqual(default(char[]), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnShortValueByIntVariable()
            {
                Guid variableId = Guid.NewGuid();
                short expectedValue = 10;
                TestedEntity.Type = "int";
                TestedEntity.RawValue = expectedValue.ToString();

                short result = TestedService.GetValue<short>(variableId);

                Assert.NotEqual(default(short), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnIntValueByIntVariable()
            {
                Guid variableId = Guid.NewGuid();
                short expectedValue = 10;
                TestedEntity.Type = "int";
                TestedEntity.RawValue = expectedValue.ToString();

                int result = TestedService.GetValue<int>(variableId);

                Assert.NotEqual(default(short), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetShortValueWithExceededRightRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "int";
                TestedEntity.RawValue = (short.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(short).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<short>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetShortValueWithExceededLeftRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "int";
                TestedEntity.RawValue = (short.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(short).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<short>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetIntValueWithExceededRightRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "int";
                TestedEntity.RawValue = ((long)int.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(int).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<int>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetIntValueWithExceededLeftRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "int";
                TestedEntity.RawValue = ((long)int.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(int).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<int>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldReturnDecimalValueByDecimalVariable()
            {
                Guid variableId = Guid.NewGuid();
                decimal expectedValue = 10.0M;
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                decimal result = TestedService.GetValue<decimal>(variableId);

                Assert.NotEqual(default(decimal), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnFloatValueByDecimalVariable()
            {
                Guid variableId = Guid.NewGuid();
                float expectedValue = 10.0f;
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                float result = TestedService.GetValue<float>(variableId);

                Assert.NotEqual(default(float), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetDecimalValueWithExceededRightRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = ((double)decimal.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(decimal).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<decimal>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetDecimalValueWithExceededLeftRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = ((double)decimal.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(decimal).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<decimal>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetFloatValueWithExceededRightRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = (float.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(float).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<float>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetFloatValueWithExceededLeftRange()
            {
                Guid variableId = Guid.NewGuid();
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = (float.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(float).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<float>(variableId)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }
        }

        #endregion

        #region By variable code

        /// <summary>
        /// Tests for <see cref="SystemVariableProvider.GetValue{TValue}(string)"/>
        /// </summary>
        public sealed class GetValueByCodeTests : BaseSystemVariableProviderTests
        {
            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsDefault()
            {
                string variableCode = default;

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsEmpty()
            {
                string variableCode = string.Empty;

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableCodeIsWhiteSpaceOnly()
            {
                string variableCode = "     ";

                string result = TestedService.GetValue<string>(variableCode);

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

                string result = TestedService.GetValue<string>(variableCode);

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

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsWhiteSpaceOnly()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "      ";

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNotValid()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "NotValid";

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldNotReturnAnything_WhenVariableTypeIsNone()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "None";

                string result = TestedService.GetValue<string>(variableCode);

                Assert.Null(result);
            }

            [Fact]
            public void ShouldReturnStringValueByStringVariable()
            {
                string variableCode = "variableCode";
                string expectedValue = "rawValue";
                TestedEntity.Type = "string";
                TestedEntity.RawValue = expectedValue;

                string result = TestedService.GetValue<string>(variableCode);

                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnLongValueByIntVariable()
            {
                string variableCode = "variableCode";
                long expectedValue = 10;
                TestedEntity.Type = "Int";
                TestedEntity.RawValue = expectedValue.ToString();

                long result = TestedService.GetValue<long>(variableCode);

                Assert.NotEqual(default(long), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnDoubleValueByDecimalVariable()
            {
                string variableCode = "variableCode";
                double expectedValue = 10.0;
                TestedEntity.Type = "Decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                double result = TestedService.GetValue<double>(variableCode);

                Assert.NotEqual(default(double), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnBoolValueByBoolVariable()
            {
                string variableCode = "variableCode";
                bool expectedValue = true;
                TestedEntity.Type = "Bool";
                TestedEntity.RawValue = expectedValue.ToString();

                bool result = TestedService.GetValue<bool>(variableCode);

                Assert.NotEqual(default(bool), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnObjectValueByJsonVariable()
            {
                string variableCode = "variableCode";
                TestClass expectedValue = new TestClass { IntegerValue = 10, BooleanValue = true, StringValue = " string " };
                TestedEntity.Type = "Json";
                TestedEntity.RawValue = JsonConvert.SerializeObject(expectedValue);

                TestClass result = TestedService.GetValue<TestClass>(variableCode);

                Assert.NotEqual(default(object), result);
                Assert.Equal(expectedValue.IntegerValue, result.IntegerValue);
                Assert.Equal(expectedValue.BooleanValue, result.BooleanValue);
                Assert.Equal(expectedValue.StringValue, result.StringValue);
            }

            [Fact]
            public void ShouldReturnCharValueByStringVariable()
            {
                string variableCode = "variableCode";
                char expectedValue = 'r';
                TestedEntity.Type = "string";
                TestedEntity.RawValue = "rawValue";

                char result = TestedService.GetValue<char>(variableCode);

                Assert.NotEqual(default(char), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnCharArrayValueByStringVariable()
            {
                string variableCode = "variableCode";
                char[] expectedValue = new char[] { 'r', 'a', 'w', 'V', 'a', 'l', 'u', 'e' };
                TestedEntity.Type = "string";
                TestedEntity.RawValue = "rawValue";

                char[] result = TestedService.GetValue<char[]>(variableCode);

                Assert.NotEqual(default(char[]), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnShortValueByIntVariable()
            {
                string variableCode = "variableCode";
                short expectedValue = 10;
                TestedEntity.Type = "int";
                TestedEntity.RawValue = expectedValue.ToString();

                short result = TestedService.GetValue<short>(variableCode);

                Assert.NotEqual(default(short), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnIntValueByIntVariable()
            {
                string variableCode = "variableCode";
                short expectedValue = 10;
                TestedEntity.Type = "int";
                TestedEntity.RawValue = expectedValue.ToString();

                int result = TestedService.GetValue<int>(variableCode);

                Assert.NotEqual(default(short), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetShortValueWithExceededRightRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "int";
                TestedEntity.RawValue = (short.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(short).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<short>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetShortValueWithExceededLeftRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "int";
                TestedEntity.RawValue = (short.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(short).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<short>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetIntValueWithExceededRightRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "int";
                TestedEntity.RawValue = ((long)int.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(int).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<int>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetIntValueWithExceededLeftRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "int";
                TestedEntity.RawValue = ((long)int.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(int).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<int>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldReturnDecimalValueByDecimalVariable()
            {
                string variableCode = "variableCode";
                decimal expectedValue = 10.0M;
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                decimal result = TestedService.GetValue<decimal>(variableCode);

                Assert.NotEqual(default(decimal), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldReturnFloatValueByDecimalVariable()
            {
                string variableCode = "variableCode";
                float expectedValue = 10.0f;
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = expectedValue.ToString();

                float result = TestedService.GetValue<float>(variableCode);

                Assert.NotEqual(default(float), result);
                Assert.Equal(expectedValue, result);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetDecimalValueWithExceededRightRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = ((double)decimal.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(decimal).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<decimal>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetDecimalValueWithExceededLeftRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = ((double)decimal.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(decimal).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<decimal>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetFloatValueWithExceededRightRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = (float.MaxValue + 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(float).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<float>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }

            [Fact]
            public void ShouldThrowAnException_WhenTryingToGetFloatValueWithExceededLeftRange()
            {
                string variableCode = "variableCode";
                TestedEntity.Type = "decimal";
                TestedEntity.RawValue = (float.MinValue - 1).ToString();
                string expectedExceptionMessage = $"System variable \"{TestedEntity.Code}\" with type \"{TestedEntity.Type}\" cannot be converted to \"{typeof(float).Name}\"";

                Exception exception =
                    Record.Exception(
                        () => TestedService.GetValue<float>(variableCode)
                    );

                Assert.NotNull(exception);
                Assert.IsType<ArgumentException>(exception);
                Assert.Equal(expectedExceptionMessage, exception.Message);
            }
        }

        #endregion
    }
}
