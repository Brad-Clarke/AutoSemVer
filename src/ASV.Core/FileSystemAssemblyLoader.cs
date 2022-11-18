using System.Reflection;

namespace ASV.Core
{
    public sealed class FileSystemAssemblyLoader : IAssemblyLoader
    {
        private readonly string _dllDirectory;
        private readonly string _dllFile;

        private readonly Dictionary<string, Assembly> _loadedAssemblies = new();

        public FileSystemAssemblyLoader(string dllDirectory, string dllFile)
        {
            _dllDirectory = dllDirectory;

            if (dllFile.EndsWith(".dll"))
            {
                _dllFile = dllFile;
            }
            else
            {
                _dllFile = $"{dllFile}.dll";
            }
        }

        public Assembly LoadAssembly()
        {
            Assembly? assembly = null;

            foreach (string newFile in Directory.GetFiles(_dllDirectory, "*.dll"))
            {
                Assembly loadedAssembly = Assembly.LoadFile(newFile);

                if (Path.GetFileName(newFile) == _dllFile)
                {
                    assembly = loadedAssembly;
                }
                else
                {
                    _loadedAssemblies.TryAdd(loadedAssembly.FullName!, loadedAssembly);
                }
            }

            if (assembly == null)
            {
                throw new FileNotFoundException("Could not load file or assembly");
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;

            return assembly;
        }

        private Assembly? CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            _loadedAssemblies.TryGetValue(args.Name, out Assembly? assembly);

            return assembly;
        }
    }
}
