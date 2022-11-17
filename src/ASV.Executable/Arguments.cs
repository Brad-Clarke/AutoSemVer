using ASV.Core.Options;
using CommandLine;

namespace ASV.Executable
{
    internal class Arguments : IOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Enable writing Change Log to Console.")]
        public bool Verbose { get; set; } = false;

        [Option('o', "output", Required = false, HelpText = "Set the output path for the Change Log.")]
        public string? ChangeLogFilePath { get; set; } = null;

        [Option('n', "new", Required = true, HelpText = "Set the directory for the new build")]
        public string NewBuildDirectory { get; set; } = string.Empty;

        [Option('p', "previous", Required = true, HelpText = "Set the directory for the previous build")]
        public string PreviousBuildDirectory { get; set; } = string.Empty;

        [Option('f', "file", Required = true, HelpText = "Set the dll filename")]
        public string DllFileName { get; set; } = string.Empty;
    }
}
