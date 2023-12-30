namespace MAS.GitlabComments.DataAccess.Select
{
    using MAS.GitlabComments.DataAccess.Filter;

    /// <summary>
    /// Configuration for select call
    /// </summary>
    public class SelectConfiguration
    {
        /// <summary>
        /// Entities filter
        /// </summary>
        public FilterGroup Filter { get; set; }

        // TODO: Use later
        ///// <summary>
        ///// Amount of entities to select
        ///// </summary>
        //public int Count { get; set; }

        ///// <summary>
        ///// Amount of entities to skip while selecting
        ///// </summary>
        //public int Offset { get; set; }
    }
}
