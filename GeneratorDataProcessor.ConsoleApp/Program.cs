using GeneratorDataProcessor.Core.Interfaces;
using GeneratorDataProcessor.Core.Services;
using GeneratorDataProcessor.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GeneratorDataProcessor.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var fileMonitor = serviceProvider.GetService<IFileMonitorService>();

            fileMonitor.StartMonitoring();
            Console.WriteLine("Monitoring for new files...");

            // Keep the application running to monitor new files
            Console.ReadLine();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IGeneratorRepository, XmlGeneratorRepository>();
            services.AddSingleton<IDataProcessor, DataProcessor>();
            services.AddSingleton<IFileMonitorService>(
                provider => new FileMonitorService(
                    provider.GetRequiredService<IDataProcessor>(),
                    configuration["InputFolder"]));

            return services.BuildServiceProvider();
        }
    }
}
