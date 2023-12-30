namespace MAS.GitlabComments.DataAccess.Services
{
    using System;

    /// <summary>
    /// Executor of custom sql scripts
    /// </summary>
    /// <remarks>
    ///     Temporary solution for data base update
    /// </remarks>
    [Obsolete("v1.4 | Will be removed in v1.5")]
    public interface ITempDatabaseModifier
    {
        /// <summary>
        /// Update database with latest changes
        /// </summary>
        void ApplyModifications();
    }
}
