using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace FacebookToDisqusComments
{
    class Program
    {
        private static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);
            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<Startup>();

            // Entry point, async
            var returnCode = 0;
            Task.Run(async () => { returnCode = (int) await app.Run(); }).Wait();

            Console.WriteLine($"Application has finished with code: {returnCode}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Options
            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("appSettings"));

            // DI
            services.AddSingleton(Configuration);
            services.AddSingleton<Startup>();
            services.AddSingleton<IFacebookCommentsApiWrapper, FacebookCommentsApiWrapper>();
            services.AddSingleton<IDisqusCommentsFormatter, DisqusCommentsFormatter>();
            services.AddSingleton<IFileUtils, FileUtils>();
            services.AddTransient<Func<HttpClient>>(_ => () => new HttpClient());
        }
    }
}