namespace MAS.GitlabComments.Data.Models
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

        // TODO: Move ModifiedOn here & create sql script for updating StoryRecord
    }
}
