namespace MAS.GitlabComments.DataAccess.Services.Implementations
{
    using System;

    /// <summary>
    /// Implementation of <see cref="ITempDatabaseModifier"/>
    /// </summary>
    public class TempDatabaseModifier : ITempDatabaseModifier
    {
        /// <inheritdoc cref="IDbConnectionFactory"/>
        private IDbConnectionFactory DbConnectionFactory { get; }

        /// <summary>
        /// Initializing <see cref="TempDatabaseModifier"/>
        /// </summary>
        /// <param name="dbConnectionFactory">Factory providing database connection</param>
        /// <exception cref="ArgumentNullException">Parameter dbConnectionFactory is null</exception>
        public TempDatabaseModifier(IDbConnectionFactory dbConnectionFactory)
        {
            DbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(DbConnectionFactory));
        }

        /// <inheritdoc cref="ITempDatabaseModifier.ApplyModifications"/>
        public void ApplyModifications()
        {
            using (var connection = DbConnectionFactory.CreateDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = CommentUniqueConstraint;

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Script for making comment table Number column unique
        /// </summary>
        private const string CommentUniqueConstraint
            = "ALTER TABLE [dbo].[Comments]"
            + " "
            + "ADD CONSTRAINT [UQ_Comments_Number] UNIQUE ([Number]);"
            ;
    }
}
