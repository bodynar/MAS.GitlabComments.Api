namespace MAS.GitlabComments.WebApi
{
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.DataAccess.Services.Implementations;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.Models;

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
            var commentNumberTemplate = string.Empty;

            if (settings != default)
            {
                isReadOnlyMode = settings.GetValue<bool>("ReadOnlyMode");
                commentNumberTemplate = settings.GetValue<string>("CommentNumberTemplate");
            }
            var appSettings = new AppSettings(isReadOnlyMode, commentNumberTemplate);

            var maxQueryCount = settings.GetValue<int>("MaxQueryRows");

            var queryOptions = new DbConnectionQueryOptions
            {
                MaxRowCount = maxQueryCount
            };

            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp");

            services
                // data registrations
                .AddTransient(typeof(IDataProvider<>), typeof(SqlDataProvider<>))
                .AddTransient<IDbConnectionFactory, DbConnectionFactory>(x => new DbConnectionFactory(connectionString, queryOptions))
                .AddTransient<IDbAdapter, DapperDbAdapter>()
                .AddTransient<IFilterBuilder, MsSqlFilterBuilder>()
                .AddTransient<IComplexColumnQueryBuilder, ComplexColumnMssqlBuilder>()

                .AddSingleton<ITempDatabaseModifier, TempDatabaseModifier>()
                // /data registrations

                // logic registrations
                .AddSingleton<IApplicationSettings>(appSettings)

                .AddTransient<ICommentService, CommentService>()
                .AddTransient<ICommentStoryRecordService, CommentStoryRecordService>()
                .AddTransient<ISystemVariableProvider, SystemVariableProvider>()
                // /logic registrations

                // web registrations
                .AddSingleton<IApplicationWebSettings>(appSettings)
                // /web registrations

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
