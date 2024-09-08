namespace MAS.GitlabComments.WebApi
{
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.DataAccess.Services.Implementations;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DataProvider;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.HostedServices;
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
            var retractionTokenLifeSpanHours = 0;

            if (settings != default)
            {
                isReadOnlyMode = settings.GetValue<bool>("ReadOnlyMode");
                commentNumberTemplate = settings.GetValue<string>("CommentNumberTemplate");
                retractionTokenLifeSpanHours = settings.GetValue<int>("RetractionTokenLiveTimeHours");
            }

            var appSettings = new AppSettings(isReadOnlyMode, commentNumberTemplate, retractionTokenLifeSpanHours);

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

                .AddTransient<IRetractionTokenManager, RetractionTokenManager>()
                .AddTransient<ICommentService, CommentService>()
                .AddTransient<ICommentStoryRecordService, CommentStoryRecordService>()
                .AddTransient<ISystemVariableProvider, SystemVariableProvider>()
                .AddTransient<ISystemVariableActionExecutor, SystemVariableActionExecutor>()
                // /logic registrations

                // web registrations
                .AddSingleton<IApplicationWebSettings>(appSettings)
                // /web registrations

                .AddHostedService<ClearExpiredTokensService>()

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
