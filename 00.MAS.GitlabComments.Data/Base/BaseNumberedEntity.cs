namespace MAS.GitlabComments.Data
{
    /// <summary>
    /// Base model for any entity with sequence number
    /// </summary>
    public abstract class BaseNumberedEntity: BaseEntity
    {
        /// <summary>
        /// Unique personal number
        /// </summary>
        public virtual string Number { get; set; }
    }
}
