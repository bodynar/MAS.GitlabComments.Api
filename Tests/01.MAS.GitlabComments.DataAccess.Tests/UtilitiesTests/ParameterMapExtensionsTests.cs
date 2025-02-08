namespace MAS.GitlabComments.DataAccess.Tests.UtilitiesTests
{
    using System.Collections.Generic;

    using MAS.GitlabComments.DataAccess.Models;

    using Xunit;

    public sealed class ParameterMapExtensionsTests
    {
        public sealed class SetTests
        {
            [Fact]
            public void ShouldAddItemToDictionaryWhenKeyIsNotPresented()
            {
                IDictionary<string, QueryParameter> map = new Dictionary<string, QueryParameter>();

                Assert.Equal(0, map.Count);

                map.Set("key", 10);

                Assert.Equal(1, map.Count);
            }

            [Fact]
            public void ShouldUpdateItemToDictionaryWhenKeyIsPresented()
            {
                string existedKey = "key";
                object existedValue = 10;

                object newValue = 999;

                IDictionary<string, QueryParameter> map = new Dictionary<string, QueryParameter>()
                {
                    { existedKey, new QueryParameter() { ColumnName = existedKey, Value = existedValue, } },
                };

                Assert.Equal(1, map.Count);

                map.Set(existedKey, newValue);

                Assert.Equal(1, map.Count);
                Assert.NotEqual(existedValue, map[existedKey].Value);
                Assert.Equal(newValue, map[existedKey].Value);
            }
        }
    }
}
