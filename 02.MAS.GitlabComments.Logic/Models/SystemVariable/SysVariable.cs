namespace MAS.GitlabComments.Logic.Models
{
    using System;

    /// <summary>
    /// Application variable model
    /// </summary>
    public class SysVariable: SysVariableDisplayModel
    {
        /// <summary>
        /// System type, constructed from <see cref="SysVariable.Type"/>
        /// </summary>
        public Type UnderlyingType { get; init; }
    }
}
