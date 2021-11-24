namespace MAS.GitlabComments.Data.Services
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection();
    }
}
