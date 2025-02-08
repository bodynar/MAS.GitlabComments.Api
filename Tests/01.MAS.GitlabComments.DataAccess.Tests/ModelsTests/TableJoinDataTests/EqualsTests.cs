namespace MAS.GitlabComments.DataAccess.Tests.ModelsTests.TableJoinDataTests
{
    using MAS.GitlabComments.DataAccess.Select;

    using Xunit;

    public sealed class EqualsTests
    {
        [Fact]
        public void ShouldReturnTrueWhenCompareWithIdenticalConfiguration()
        {
            string configuration = "configuration";
            TableJoinData testedInstance1 = new TableJoinData(configuration);
            TableJoinData testedInstance2 = new TableJoinData(configuration);

            bool result = testedInstance1.Equals(testedInstance2);

            Assert.True(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenCompareWithNotIdenticalConfiguration()
        {
            TableJoinData testedInstance1 = new TableJoinData("configuration 1");
            TableJoinData testedInstance2 = new TableJoinData("configuration 2");

            bool result = testedInstance1.Equals(testedInstance2);

            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnFalseWhenCompareWithOtherType()
        {
            TableJoinData testedInstance1 = new TableJoinData("configuration 1");
            EqualsTests testedInstance2 = new EqualsTests();

            bool result = testedInstance1.Equals(testedInstance2);

            Assert.False(result);
        }
    }
}
