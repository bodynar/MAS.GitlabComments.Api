namespace MAS.GitlabComments.DataAccess.Tests.TempDatabaseModifierTests
{
    using Xunit;

    public sealed class ApplyModificationsTests : BaseTempDatabaseModifierTests
    {
        [Fact]
        public void ShouldCreateAndExecuteCommand()
        {
            string expectedCommandText = "ALTER TABLE [dbo].[Comments] ADD CONSTRAINT [UQ_Comments_Number] UNIQUE ([Number]);";

            TestedService.ApplyModifications();

            Assert.True(NonQueryExecuted);
            Assert.Equal(expectedCommandText, CommandText);
        }
    }
}
