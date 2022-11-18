namespace ASV.Core.Options
{
    public interface IOptions
    {
        public bool Verbose { get; }
        public string? ChangeLogFilePath { get; }
    }
}
