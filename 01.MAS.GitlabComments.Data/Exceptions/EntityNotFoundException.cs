namespace MAS.GitlabComments.Data.Exceptions
{
    using System;

    /// <summary>
    /// Exception that indicates that entity not found in data storage
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Initializing <see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityId">Entity identifier</param>
        public EntityNotFoundException(string entityName, Guid entityId)
            : base($"Entity \"{entityName}\" - \"{entityId}\" not found.")
        { }
    }
}
