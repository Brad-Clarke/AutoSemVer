using System.Reflection;
using ASV.Core.Detection;
using ASV.Core.Detection.Factory;
using ASV.Core.Enums;
using ASV.Core.Options;

namespace ASV.Core.Versioning
{
    public class AssemblyVersioner : IAssemblyVersioner
    {
        private readonly IChangeDetector<Assembly> _assemblyChangeDetector;

        private readonly Assembly _newAssembly;
        private readonly Assembly _previousAssembly;

        public AssemblyVersioner(IOptions options, IChangeDetectorFactory changeDetectorFactory)
        {
            _assemblyChangeDetector = changeDetectorFactory.Build<Assembly>();

            _newAssembly = Assembly.LoadFile(options.NewDllFilePath);
            _previousAssembly = Assembly.LoadFile(options.PreviousDllFilePath);
        }

        public Version GetVersion()
        {
            ChangeLevel changeLevel = _assemblyChangeDetector.DetectChanges(_newAssembly, _previousAssembly);

            Version previousVersion = _previousAssembly.GetName().Version;

            if (previousVersion == null)
            {
                return new Version(1,0,0,1);
            }

            return changeLevel switch
            {
                ChangeLevel.None
                    => new Version(1, 0, 0, 1),
                ChangeLevel.Patch
                    => new Version(previousVersion.Major, previousVersion.Minor, previousVersion.Build, previousVersion.Revision + 1),
                ChangeLevel.Minor
                    => new Version(previousVersion.Major, previousVersion.Minor, previousVersion.Build + 1, 1),
                ChangeLevel.Major
                    => new Version(previousVersion.Major, previousVersion.Minor + 1, 0, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
