namespace MAS.GitlabComments.Services
{
    using System.Data;

    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection();
    }
}
