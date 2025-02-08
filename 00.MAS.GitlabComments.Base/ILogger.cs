namespace MAS.GitlabComments.Base
{
    using System;

    /// <summary>
    /// Core logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log debug message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Log warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warning(string message);

        /// <summary>
        /// Log error message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log exception
        /// </summary>
        /// <param name="exception">Raised exception</param>
        /// <param name="message">Additional message</param>
        void Error(Exception exception, string message = null);
    }
}
