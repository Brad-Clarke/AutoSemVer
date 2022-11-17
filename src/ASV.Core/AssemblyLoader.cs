using ASV.Core.Options;
using System.Reflection;

namespace ASV.Core
{
    public sealed class AssemblyLoader
    {
        private readonly IOptions _options;

        private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
        public Assembly NewAssembly { get; }
        public Assembly PreviousAssembly { get; }

        public AssemblyLoader(IOptions options)
        {
            _options = options;

            foreach (string newFile in Directory.GetFiles(options.NewBuildDirectory, "*.dll"))
            {
                Assembly loadedAssembly = Assembly.LoadFile(newFile);

                if (Path.GetFileName(newFile) == options.DllFileName)
                {
                    NewAssembly = loadedAssembly;
                }
                else
                {
                    _loadedAssemblies.TryAdd(loadedAssembly.FullName, loadedAssembly);
                }
            }

            foreach (string oldFile in Directory.GetFiles(options.PreviousBuildDirectory, "*.dll"))
            {
                Assembly loadedAssembly = Assembly.LoadFile(oldFile);

                if (Path.GetFileName(oldFile) == options.DllFileName)
                {
                    PreviousAssembly = loadedAssembly;
                }
                else
                {
                    _loadedAssemblies.TryAdd(loadedAssembly.FullName, loadedAssembly);
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        private Assembly? CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            _loadedAssemblies.TryGetValue(args.Name, out Assembly? assembly);

            return assembly;
        }
    }
}
