namespace ASV.Core.Options
{
    public interface IOptions
    {
        public bool Verbose { get; }
        public string? ChangeLogFilePath { get; }
        public string NewBuildDirectory { get; }
        public string PreviousBuildDirectory { get; }

        public string DllFileName { get; }
    }
}
