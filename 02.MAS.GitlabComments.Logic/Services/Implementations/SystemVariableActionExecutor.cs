namespace MAS.GitlabComments.Logic.Services.Implementations
{
    using System;

    using MAS.GitlabComments.Data;

    /// <summary>
    /// Executor of custom system variables actions
    /// </summary>
    public class SystemVariableActionExecutor : ISystemVariableActionExecutor
    {
        /// <inheritdoc cref="ICommentService"/>
        private ICommentService CommentService { get; }

        /// <summary>
        /// Initializing <see cref="SystemVariableActionExecutor"/>
        /// </summary>
        /// <param name="commentService">Instance of <see cref="ICommentService"/></param>
        /// <exception cref="ArgumentNullException">Param commentService is null</exception>
        public SystemVariableActionExecutor(
            ICommentService commentService
        )
        {
            CommentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Argument "<paramref name="code"/>" is empty</exception>
        /// <exception cref="NotImplementedException">Handler for setting with code "<paramref name="code"/>" is not defined</exception>
        public void ExecuteAction(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            Action action = null;

            switch (code)
            {
                case BuiltInSysVariables.LastCommentNumber:
                    action = () => CommentService.RecalculateLastNumber();
                    break;
                default:
                    throw new NotImplementedException($"Action for variable \"{code}\" is not defined");
            }

            action?.Invoke();
        }
    }
}
