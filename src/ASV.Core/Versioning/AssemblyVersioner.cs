using ASV.Core.Detection;
using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Options;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace ASV.Core.Versioning
{
    public class AssemblyVersioner : IAssemblyVersioner
    {
        private readonly IOptions _options;
        private readonly IChangeDetector<Assembly> _assemblyChangeDetector;

        public AssemblyVersioner(IOptions options, IChangeDetectorFactory changeDetectorFactory)
        {
            _options = options;
            _assemblyChangeDetector = changeDetectorFactory.Build<Assembly>();

        }

        public Version GetNewVersion(Assembly current, Assembly previous)
        {
            ChangeLevel changeLevel = ChangeLevel.None;

            if (AreAssembliesEqual())
            {
                if (_options.Verbose)
                {
                    Console.WriteLine("No changes detected - assemblies files match byte for byte.\r\n");
                }
            }
            else
            {
                changeLevel = _assemblyChangeDetector.DetectChanges(current, previous);

                if (_options.Verbose)
                {
                    Console.WriteLine($"\r\nOutcome: {changeLevel.ToString().ToUpper()}\r\n");
                }
            }

            Version? previousVersion = previous.GetName().Version;

            if (previousVersion == null)
            {
                return new Version(1, 0, 0, 1);
            }

            return changeLevel switch
            {
                ChangeLevel.None
                    => previousVersion,
                ChangeLevel.Patch
                    => new Version(previousVersion.Major, previousVersion.Minor, previousVersion.Build, previousVersion.Revision + 1),
                ChangeLevel.Minor
                    => new Version(previousVersion.Major, previousVersion.Minor, previousVersion.Build + 1, 1),
                ChangeLevel.Major
                    => new Version(previousVersion.Major, previousVersion.Minor + 1, 0, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private bool AreAssembliesEqual(Assembly current, Assembly previous)
        {
           
            FileInfo first = new FileInfo(Path.Join(_options.NewBuildDirectory, _options.DllFileName));
            FileInfo second = new FileInfo(Path.Join(_options.PreviousBuildDirectory, _options.DllFileName));

            if (first.Length != second.Length)
            {
                return false;
            }

            if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }


            using FileStream fs1 = first.OpenRead();
            using FileStream fs2 = second.OpenRead();

            for (int i = 0; i < first.Length; i++)
            {
                if (fs1.ReadByte() != fs2.ReadByte())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
