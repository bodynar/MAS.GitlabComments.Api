namespace MAS.GitlabComments
{
    using MAS.GitlabComments.Attributes;
    using MAS.GitlabComments.Data.Services;
    using MAS.GitlabComments.Data.Services.Implementations;
    using MAS.GitlabComments.Models;
    using MAS.GitlabComments.Services;
    using MAS.GitlabComments.Services.Implementations;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        /// <inheritdoc cref="IConfiguration"/>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializing <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.Exception("Connection string with name \"DefaultConnection\" is empty.");
            }

            var settings = Configuration.GetSection("GlobalSettings");
            bool isReadOnlyMode = default;

            if (settings != default)
            {
                isReadOnlyMode = settings.GetValue<bool>("ReadOnlyMode");
            }

            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp");

            services.AddTransient<ICommentService, CommentService>()
                .AddTransient(typeof(IDataProvider<>), typeof(SqlDataProvider<>))
                .AddTransient<IDbConnectionFactory, DbConnectionFactory>(x => new DbConnectionFactory(connectionString))
                .AddSingleton(new AppSettings(isReadOnlyMode))
                .AddTransient<IDbAdapter, DapperDbAdapter>()
                .AddControllers(opts => { opts.Filters.Add<UseReadOnlyModeAttribute>(); })
                .AddNewtonsoftJson()
            ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSpaStaticFiles();

            app.UseRouting()
                .UseStaticFiles()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseSpa(spa => { spa.Options.SourcePath = "ClientApp"; })
            ;
        }
    }
}
