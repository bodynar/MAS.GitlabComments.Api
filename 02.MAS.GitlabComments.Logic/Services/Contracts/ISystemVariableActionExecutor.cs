namespace MAS.GitlabComments.Logic.Services
{
    using MAS.GitlabComments.Data;

    /// <summary>
    /// Executor of <see cref="SystemVariable"/> actions
    /// </summary>
    public interface ISystemVariableActionExecutor
    {
        /// <summary>
        /// Execute additional action for variable
        /// </summary>
        /// <param name="code">Variable code</param>
        void ExecuteAction(string code);
    }
}
