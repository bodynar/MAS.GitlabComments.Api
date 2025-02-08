namespace MAS.GitlabComments.WebApi
{
    using System;

    using MAS.GitlabComments.Base;
    using MAS.GitlabComments.DataAccess;
    using MAS.GitlabComments.DataAccess.Services;
    using MAS.GitlabComments.DataAccess.Services.Implementations;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.MS;
    using MAS.GitlabComments.DataAccess.Services.Implementations.DbDependant.PG;
    using MAS.GitlabComments.Logic.Services;
    using MAS.GitlabComments.Logic.Services.Implementations;
    using MAS.GitlabComments.WebApi.Attributes;
    using MAS.GitlabComments.WebApi.HostedServices;
    using MAS.GitlabComments.WebApi.Logger;
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
                throw new ApplicationException("Connection string with name \"DefaultConnection\" is empty.");
            }

            var settings = Configuration.GetSection("GlobalSettings");

            if (settings == default)
            {
                throw new ApplicationException("\"GlobalSettings\" section not found in \"appsettings.json\" file");
            }

            var isReadOnlyMode = settings.GetValue<bool>("ReadOnlyMode");
            var commentNumberTemplate = settings.GetValue<string>("CommentNumberTemplate");
            var retractionTokenLifeSpanHours = settings.GetValue<int>("RetractionTokenLiveTimeHours");
            var dbType = settings.GetValue<DatabaseType>("DatabaseType");

            var appSettings = new AppSettings(isReadOnlyMode, commentNumberTemplate, retractionTokenLifeSpanHours, dbType);

            var maxQueryCount = settings.GetValue<int>("MaxQueryRows");

            var queryOptions = new DbConnectionQueryOptions
            {
                MaxRowCount = maxQueryCount
            };

            services.AddSpaStaticFiles(options => options.RootPath = "ClientApp");

            AddDbDependantServices(services, dbType);

            services
                .AddSingleton<IAppSettings>(appSettings)
                .AddSingleton<ILogger, Logger4Net>()
                // data registrations
                .AddSingleton<IDbConnectionFactory, DbConnectionFactory>(x => new DbConnectionFactory(connectionString, queryOptions, dbType))
                .AddSingleton<IDbAdapter, DapperDbAdapter>()

                .AddSingleton<IComplexColumnQueryBuilder, ComplexColumnQueryBuilder>()
                .AddSingleton<ITempDatabaseModifier, TempDatabaseModifier>()
                .AddSingleton<IDataAccessSettings>(appSettings)
                // /data registrations

                // logic registrations
                .AddSingleton<IBusinessLogicSettings>(appSettings)

                .AddTransient<IRetractionTokenManager, RetractionTokenManager>()
                .AddTransient<ICommentService, CommentService>()
                .AddTransient<ICommentStoryRecordService, CommentStoryRecordService>()
                .AddTransient<ISystemVariableProvider, SystemVariableProvider>()
                .AddTransient<ISystemVariableActionExecutor, SystemVariableActionExecutor>()
                .AddTransient<IDataImporter, DataImporter>()
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

        /// <summary>
        /// Add database type dependent services
        /// </summary>
        /// <param name="services">Instance of <see cref="IServiceCollection"/></param>
        /// <param name="dbType">Type of db server</param>
        private static void AddDbDependantServices(IServiceCollection services, DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.MSSQL:
                    services
                        .AddTransient(typeof(IDataProvider<>), typeof(MsSqlDataProvider<>))
                        .AddTransient<IFilterBuilder, MsSqlFilterBuilder>();
                    break;

                case DatabaseType.PGSQL:
                    services
                        .AddTransient(typeof(IDataProvider<>), typeof(PgSqlDataProvider<>))
                        .AddTransient<IFilterBuilder, PgSqlFilterBuilder>();
                    break;
                
                default:
                    throw new NotImplementedException($"Handler for DB type \"{dbType}\" not implemented yet.");
            }
        }
    }
}
