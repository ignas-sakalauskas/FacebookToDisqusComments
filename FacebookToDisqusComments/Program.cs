using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using FacebookToDisqusComments.ApiWrappers;
using FacebookToDisqusComments.DataServices;

namespace FacebookToDisqusComments
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);
            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<Startup>();

            // Entry point, async
            var returnCode = ReturnCodes.Success;
            Task.Run(async () => { returnCode = await app.RunAsync(); }).Wait();

            Console.WriteLine($"Application has finished with code: {returnCode}");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Options
            services.AddOptions();
            services.Configure<AppSettings>(_configuration.GetSection("appSettings"));

            // DI
            services.AddSingleton(_configuration);
            services.AddSingleton<Startup>();
            services.AddSingleton<IFacebookCommentsApiWrapper, FacebookCommentsApiWrapper>();
            services.AddSingleton<IDisqusCommentsFormatter, DisqusCommentsFormatter>();
            services.AddSingleton<IFileUtils, FileUtils>();
            services.AddSingleton<Func<HttpClient>>(_ => () => new HttpClient());
            services.AddSingleton<IFacebookResponseParser, FacebookResponseParser>();
        }
    }
}