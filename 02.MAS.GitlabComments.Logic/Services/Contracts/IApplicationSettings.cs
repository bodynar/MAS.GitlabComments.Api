namespace MAS.GitlabComments.Logic.Services
{
    /// <summary>
    /// Application settings
    /// <para>Business layer level</para>
    /// </summary>
    public interface IApplicationSettings
    {
        /// <summary>
        /// Comment number template
        /// </summary>
        string CommentNumberTemplate { get; }

        /// <summary>
        /// Life span of retraction token (in hours)
        /// </summary>
        int RetractionTokenLifeSpanHours { get; }
    }
}
