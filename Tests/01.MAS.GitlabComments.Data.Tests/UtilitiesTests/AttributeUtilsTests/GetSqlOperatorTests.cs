﻿namespace MAS.GitlabComments.Data.Tests.UtilitiesTests.AttributeUtilsTests
{
    using MAS.GitlabComments.DataAccess.Attributes;
    using MAS.GitlabComments.DataAccess.Utilities;

    using Xunit;

    public sealed class GetSqlOperatorTests
    {
        /// <summary> Tested sql attribute value </summary>
        private const string SqlAttributeValue = "A";

        [Fact]
        public void ShouldReturnEmptyStringWhenEnumValueIsNull()
        {
            string sqlOperator = AttributeUtils.GetSqlOperator(null);

            Assert.Empty(sqlOperator);
        }

        [Fact]
        public void ShouldReturnEmptyStringWhenEnumDoesNotContainSpecifiedValue()
        {
            string sqlOperator = AttributeUtils.GetSqlOperator((TestedEnum)0);

            Assert.Empty(sqlOperator);
        }

        [Fact]
        public void ShouldReturnEmptyStringWhenEnumValueDoesNotContainValueWithSqlOperatorAttribute()
        {
            string sqlOperator = AttributeUtils.GetSqlOperator(TestedEnum.ValueWithoutAttribute);

            Assert.Empty(sqlOperator);
        }

        [Fact]
        public void ShouldReturnAttributePropertyValueWhenEnumValueHaveSqlOperatorAttribute()
        {
            string sqlOperator = AttributeUtils.GetSqlOperator(TestedEnum.ValueWithAttribute);

            Assert.NotEmpty(sqlOperator);
            Assert.Equal(SqlAttributeValue, sqlOperator);
        }

        private enum TestedEnum
        {
            EmptyValue = -1,

            ValueWithoutAttribute = 1,

            [SqlOperator(SqlAttributeValue)]
            ValueWithAttribute = 2,
        }
    }
}
