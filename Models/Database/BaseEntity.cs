namespace MAS.GitlabComments.Models.Database
{
    using System;

    /// <summary>
    /// Base type for all database entities
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier of entity
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Date of creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }
    }
}
