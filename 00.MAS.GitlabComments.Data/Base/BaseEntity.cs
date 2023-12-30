namespace MAS.GitlabComments.Data
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

        /// <summary>
        /// Date of last update
        /// </summary>
        public virtual DateTime ModifiedOn { get; set; }
    }
}
