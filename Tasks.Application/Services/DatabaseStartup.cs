using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Infrastructure.Data;

namespace Tasks.Application.Services
{
    public static class DatabaseStartup
    {
        private static IConfiguration _configuration;

        public static void SetConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TasksDbContext>(options =>
              options.UseSqlite(_configuration.GetConnectionString("DBContext")), ServiceLifetime.Transient);


            // Other service configurations...
        }
    }
}
