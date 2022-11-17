using System;
using System.Reflection;
using ASV.Core.Detection;
using ASV.Core.Detection.Detectors;
using ASV.Core.Detection.Factory;
using ASV.Core.Options;
using ASV.Core.Tracking;
using ASV.Core.Versioning;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<IAssemblyVersioner, AssemblyVersioner>();

            services.AddScoped<IChangeTracker, FileSystemChangeTracker>();

            services.AddScoped<IChangeDetectorFactory>(p => new ChangeDetectorFactory(p));

            services.AddScoped<IChangeDetector<Assembly>, AssemblyChangeDetector>();
            services.AddScoped<IChangeDetector<Type>, TypeChangeDetector>();

            return services;
        }
    }
}