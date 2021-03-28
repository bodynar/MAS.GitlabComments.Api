namespace MAS.GitlabComments.Logger
{
    using System;
    using System.IO;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [ProviderAlias("Logger")]
    public class LoggerProvider : ILoggerProvider
    {
        public LoggerOptions Options { get; }

        public LoggerProvider(IOptions<LoggerOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));

            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(this);
        }

        public void Dispose()
        {
        }
    }
}
