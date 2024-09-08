namespace MAS.GitlabComments.Logic.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Application variable model
    /// </summary>
    public class SysVariableDisplayModel
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Date of last update
        /// </summary>
        public virtual DateTime ModifiedOn { get; set; }

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
        /// Caption for action, which can be executed for variable
        /// </summary>
        public string ActionCaption { get; set; }
    }
}
