using ASV.Core.Options;
using ASV.Core.Tracking;
using ASV.Core.Versioning;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ASV.Executable
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IOptions options = Parser.Default.ParseArguments<Arguments>(args).Value;

            using ServiceProvider serviceProvider = BuildServices(options).BuildServiceProvider();

            Version version = serviceProvider.GetRequiredService<IAssemblyVersioner>().GetVersion();

            Console.Write($"Generated Version:{version}");
        }

        private static ServiceCollection BuildServices(IOptions options)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(options);

            services.AddChangeDetection();

            services.AddScoped<IAssemblyVersioner, AssemblyVersioner>();

            services.AddScoped<IChangeTracker, FileSystemChangeTracker>();



            return services;
        }
    }
}