namespace MAS.GitlabComments.WebApi.Logger
{
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<LoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, LoggerProvider>()
                .Configure(configure);

            return builder;
        }
    }
}
