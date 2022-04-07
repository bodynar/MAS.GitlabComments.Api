namespace MAS.GitlabComments.WebApi
{
    using Logger;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>())
                .ConfigureLogging((hostBuilder, loggerBuilder) =>
                    loggerBuilder.AddFileLogger(options =>
                        hostBuilder.Configuration.GetSection("Logging").GetSection("FileLogger").GetSection("Options").Bind(options)));
    }
}
