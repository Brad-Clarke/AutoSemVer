using ASV.Core;
using ASV.Core.Detection;
using ASV.Core.Options;
using ASV.Core.Tracking;
using ASV.Core.Versioning;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ASV.Executable
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ParserResult<Arguments>? parserResult = Parser.Default.ParseArguments<Arguments>(args);

            if (parserResult == null || parserResult.Errors.Any())
            {
                return;
            }

            IOptions options = parserResult.Value;

            using ServiceProvider serviceProvider = BuildServices(options).BuildServiceProvider();

            Version? previousVersion = serviceProvider.GetRequiredService<IAssemblyVersioner>().GetCurrentVersion();
            Version version = serviceProvider.GetRequiredService<IAssemblyVersioner>().GetNewVersion();

            Console.WriteLine($"Previous Version:[{previousVersion}]");
            Console.Write($"Generated Version:[{version}]");
        }

        private static ServiceCollection BuildServices(IOptions options)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(options);

            services.AddChangeDetection();

            services.AddSingleton<AssemblyLoader>();
            services.AddScoped<IAssemblyVersioner, AssemblyVersioner>();

            services.AddScoped<IChangeTracker, FileSystemChangeTracker>();


            return services;
        }
    }
}