namespace MAS.GitlabComments.Data
{
    /// <summary>
    /// Application variable, stored in database
    /// </summary>
    public class SystemVariable : BaseEntity
    {
        /// <summary>
        /// Unique code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Readable caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Data value type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Raw value
        /// </summary>
        public string RawValue { get; set; }

        /// <summary>
        /// Caption for action, which can be executed for variable
        /// </summary>
        public string ActionCaption { get; set; }
    }
}
