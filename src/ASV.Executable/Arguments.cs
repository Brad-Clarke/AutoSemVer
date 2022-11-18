using ASV.Core.Options;
using CommandLine;

namespace ASV.Executable
{
    internal class Arguments : IOptions
    {
        [Option('l', "verbose", Required = false, HelpText = "Enable writing Change Log to Console.")]
        public bool Verbose { get; set; } = false;

        [Option('o', "output", Required = false, HelpText = "Set the output path for the Change Log.")]
        public string? ChangeLogFilePath { get; set; } = null;

        [Option('c', "current", Required = true, HelpText = "Set the directory for the new build")]
        public string CurrentBuildDirectory { get; set; } = string.Empty;

        [Option('p', "previous", Required = false, HelpText = "Set the directory for the previous build")]
        public string PreviousBuildDirectory { get; set; } = string.Empty;

        [Option('f', "file", Required = true, HelpText = "Set the name of the package")]
        public string PackageName { get; set; } = string.Empty;

        [Option('n', "nuget", Required = false, HelpText = "Set the NuGet Repository")]
        public string NuGetRepository { get; set; } = string.Empty;

        [Option('v', "version", Required = false, HelpText = "Set the NuGet Package Version")]
        public string NuGetVersion { get; set; } = string.Empty;
    }
}
