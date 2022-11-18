using ASV.Core;
using ASV.Core.Detection;
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
            
            Assembly currentAssembly = LoadAssembly(arguments, AssemblyType.Current);
            Assembly previousAssembly = LoadAssembly(arguments, AssemblyType.Previous);

            using ServiceProvider serviceProvider = BuildServices(arguments).BuildServiceProvider();

            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.WriteLine($"Checking for semantic changes in {currentAssembly.GetName().Name}\r\n");
            Console.WriteLine("----------------------------------------------------------------------------------------------------");

            Version? previousVersion = null;
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

        private static Assembly LoadAssembly(Arguments arguments, AssemblyType type)
        {
            string directory = type switch
            {
                AssemblyType.Current => arguments.CurrentBuildDirectory,
                AssemblyType.Previous => arguments.PreviousBuildDirectory,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            IAssemblyLoader assemblyLoader = new FileSystemAssemblyLoader(directory, arguments.PackageName);

            return assemblyLoader.LoadAssembly();
        }

        private enum AssemblyType
        {
            Current,
            Previous
        }
    }
}