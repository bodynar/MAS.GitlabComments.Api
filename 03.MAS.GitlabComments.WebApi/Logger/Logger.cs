namespace MAS.GitlabComments.WebApi.Logger
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Microsoft.Extensions.Logging;

    // todo: use log4net
    public class Logger : ILogger
    {
        protected LoggerProvider LoggerFileProvider { get; }

        public Logger([NotNull] LoggerProvider loggerProvider)
        {
            LoggerFileProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var fileName = LoggerFileProvider.Options.FilePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"));
            var filePath = $"{LoggerFileProvider.Options.FolderPath}/{fileName}";
            
            var record = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss+00:00}] | {logLevel}: {formatter(state, exception)} {(exception?.StackTrace ?? "")}";

            using (var streamWriter = new StreamWriter(filePath, true))
            {
                streamWriter.WriteLine(record);
            }
        }
    }
}
