namespace MAS.GitlabComments.DataAccess.Exceptions
{
    using System;

    /// <summary>
    /// Exception that indicates that entity not found in data storage
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Entity name
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// Entity identifier
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Initializing <see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="entityName">Entity name</param>
        /// <param name="entityId">Entity identifier</param>
        public EntityNotFoundException(string entityName, Guid entityId)
            : base($"Entity \"{entityName}\" - \"{entityId}\" not found.")
        {
            EntityId = entityId;
            EntityName = entityName;
        }
    }
}
