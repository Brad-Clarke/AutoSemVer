using ASV.Core.Options;
using CommandLine;

namespace ASV.Executable
{
    internal class Arguments : IOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; } = false;

        [Option('o', "output", Required = false, HelpText = "Set the output path for the change log.")]
        public string? OutputLogFilePath { get; set; } = null;

        [Option('n', "new", Required = true, HelpText = "Set the file path for the New DLL")]
        public string NewDllFilePath { get; set; } = string.Empty;

        [Option('p', "previous", Required = true, HelpText = "Set the file path for the Previous DLL")]
        public string PreviousDllFilePath { get; set; } = string.Empty;
    }
}
