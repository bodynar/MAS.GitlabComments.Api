namespace MAS.GitlabComments.Logic.Models
{
    using MAS.GitlabComments.Data;

    /// <summary>
    /// Segment of <see cref="Comment"/>, representing only <see cref="BaseNumberedEntity.Number"/> property
    /// </summary>
    public class NumberOfComment
    {
        /// <summary>
        /// Unique personal number
        /// </summary>
        public virtual string Number { get; set; }
    }
}
