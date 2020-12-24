using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using TarkovLensBot.Services;

namespace TarkovLensBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            services.AddSingleton(Configuration);

            services.AddScoped<TarkovLensService>();
            services.AddHttpClient<TarkovLensService>();

            var serviceProvider = services.BuildServiceProvider();

            var config = new Config();
            Configuration.Bind(config);

            var bot = new Bot(serviceProvider, config);
            services.AddSingleton(bot);
        }

        public void Configure()
        {

        }
    }
}