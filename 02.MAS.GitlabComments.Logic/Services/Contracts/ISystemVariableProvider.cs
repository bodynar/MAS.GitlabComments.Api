namespace MAS.GitlabComments.Logic.Services
{
    using System;
    using System.Collections.Generic;

    using MAS.GitlabComments.Logic.Models;

    /// <summary>
    /// Provider of system variables with read\update access
    /// </summary>
    public interface ISystemVariableProvider
    {
        /// <summary>
        /// Get data for all system variables
        /// </summary>
        /// <returns>Stack of variables data</returns>
        IEnumerable<SysVariableDisplayModel> GetAllVariables();

        /// <summary>
        /// Get variable value
        /// </summary>
        /// <typeparam name="TValue">Variable value type</typeparam>
        /// <param name="variableId">Variable identifier</param>
        /// <returns>Variable current value</returns>
        TValue GetValue<TValue>(Guid variableId);

        /// <summary>
        /// Get variable value
        /// </summary>
        /// <typeparam name="TValue">Variable value type</typeparam>
        /// <param name="code">Variable code</param>
        /// <returns>Variable current value</returns>
        TValue GetValue<TValue>(string code);

        /// <summary>
        /// Get variable data
        /// </summary>
        /// <param name="variableId">Variable identifier</param>
        /// <returns>Variable read model</returns>
        SysVariable Get(Guid variableId);

        /// <summary>
        /// Get variable data
        /// </summary>
        /// <param name="code">Variable code</param>
        /// <returns>Variable read model</returns>
        SysVariable Get(string code);

        /// <summary>
        /// Update variable value
        /// </summary>
        /// <typeparam name="TValue">Variable value type</typeparam>
        /// <param name="variableId">Variable identifier</param>
        /// <param name="value">Variable new value</param>
        void Set<TValue>(Guid variableId, TValue value);

        /// <summary>
        /// Update variable value
        /// </summary>
        /// <typeparam name="TValue">Variable value type</typeparam>
        /// <param name="code">Variable code</param>
        /// <param name="value">Variable new value</param>
        void Set<TValue>(string code, TValue value);
    }
}
