namespace MAS.GitlabComments
{
    using MAS.GitlabComments.Services;
    using MAS.GitlabComments.Services.Implementations;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddTransient<ICommentService, CommentService>()
                .AddTransient(typeof(IDataProvider<>), typeof(SqlDataProvider<>))
                .AddTransient<IDbConnectionFactory, DbConnectionFactory>(x => new DbConnectionFactory(connectionString))
                .AddTransient<IDbAdapter, DapperDbAdapter>()
                .AddControllers()
                .AddNewtonsoftJson()
            ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers())
            ;
        }
    }
}
