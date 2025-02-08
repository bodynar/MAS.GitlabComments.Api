namespace MAS.GitlabComments.WebApi.Logger
{
    using System;

    using log4net;

    using MAS.GitlabComments.Base;

    /// <summary>
    /// Implementation of <see cref="ILogger"/> with log4net
    /// </summary>
    public class Logger4Net : ILogger
    {
        /// <summary>
        /// Log4net logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("MAS.GitlabComments");

        /// <summary>
        /// Debug Log4net logger
        /// </summary>
        private static readonly ILog debugLog = LogManager.GetLogger("MAS.GitlabComments.Debug");

        /// <inheritdoc />
        public void Debug(string message)
            => debugLog.Debug(message);

        /// <inheritdoc />
        public void Error(string message)
            => log.Error(message);

        /// <inheritdoc />
        public void Error(Exception exception, string message = null)
            => log.Error(message, exception);

        /// <inheritdoc />
        public void Warning(string message)
            => log.Error(message);
    }
}
