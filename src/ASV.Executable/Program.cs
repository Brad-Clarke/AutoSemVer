using ASV.Core;
using ASV.Core.Detection;
using ASV.Core.Extensions.Nuget;
using ASV.Core.Options;
using ASV.Core.Tracking;
using ASV.Core.Versioning;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

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

            Arguments arguments = parserResult.Value;

            Assembly currentAssembly = new FileSystemAssemblyLoader(arguments.CurrentBuildDirectory, arguments.PackageName).LoadAssembly();
            Assembly previousAssembly = LoadAssembly(arguments);

            using ServiceProvider serviceProvider = BuildServices(arguments).BuildServiceProvider();

            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Checking for semantic changes in {currentAssembly.GetName().Name}\r\n");
            Console.WriteLine("----------------------------------------------------------------------------------------------------");

            Version? previousVersion = previousAssembly.GetName().Version;
            Version version = serviceProvider.GetRequiredService<IAssemblyVersioner>().GetNewVersion(currentAssembly, previousAssembly);

            Console.WriteLine($"Previous Version:[{previousVersion}]");
            Console.Write($"Generated Version:[{version}]");
        }

        private static ServiceCollection BuildServices(IOptions options)
        {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton(options);

            services.AddChangeDetection();

            services.AddSingleton<FileSystemAssemblyLoader>();
            services.AddScoped<IAssemblyVersioner, AssemblyVersioner>();

            services.AddScoped<IChangeTracker, FileSystemChangeTracker>();

            return services;
        }

        private static Assembly LoadAssembly(Arguments arguments)
        {
            IAssemblyLoader assemblyLoader;

            if (string.IsNullOrWhiteSpace(arguments.PreviousBuildDirectory))
            {
                assemblyLoader = new NugetAssemblyLoader(arguments.PackageName, arguments.NuGetVersion, arguments.NuGetRepository);
            }
            else
            {
                assemblyLoader = new FileSystemAssemblyLoader(arguments.PreviousBuildDirectory, arguments.PackageName);
            }

            return assemblyLoader.LoadAssembly();
        }
    }
}