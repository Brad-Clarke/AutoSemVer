namespace ASV.Core.Options
{
    public interface IOptions
    {
        public bool Verbose { get; }
        public string? OutputLogFilePath { get; }
        public string NewDllFilePath { get; }
        public string PreviousDllFilePath { get; }
    }
}
