namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Application variable model
    /// </summary>
    public class SysVariable
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Unique code
        /// </summary>
        public string Code { get; init; }

        /// <summary>
        /// Readable caption
        /// </summary>
        public string Caption { get; init; }

        /// <summary>
        /// Data value type
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        /// Raw value
        /// </summary>
        public string RawValue { get; init; }

        /// <summary>
        /// System type, constructed from <see cref="SysVariable.Type"/>
        /// </summary>
        public Type UnderlyingType { get; init; }
    }
}
