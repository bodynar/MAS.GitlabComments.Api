namespace MAS.GitlabComments.Data.Select
{
    using MAS.GitlabComments.Data.Filter;

    /// <summary>
    /// Configuration for select call
    /// </summary>
    public class SelectConfiguration
    {
        /// <summary>
        /// Entities filter
        /// </summary>
        public FilterGroup Filter { get; set; }

        /// <summary>
        /// Amount of entities to select
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Amount of entities to skip while selecting
        /// </summary>
        public int Offset { get; set; }
    }
}
