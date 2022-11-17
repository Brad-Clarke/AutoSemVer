using ASV.Core.Enums;
using ASV.Core.Options;

namespace ASV.Core.Tracking
{
    public class FileSystemChangeTracker : IChangeTracker, IDisposable
    {
        private readonly List<TrackedChange> _trackedChanges = new();

        private readonly IOptions _options;

        public FileSystemChangeTracker(IOptions options)
        {
            _options = options;
        }

        public void Track(string description, ChangeType changeType)
        {
            if (_options.Verbose)
            {
                string changeString = changeType.ToString().ToUpper().PadRight(8, ' ');

                Console.WriteLine($"{changeString} | {description}");
            }

            _trackedChanges.Add(new TrackedChange(description, changeType));
        }

        private class TrackedChange
        {
            public string Description { get; }

            public ChangeType ChangeType { get; }

            public TrackedChange(string description, ChangeType changeType)
            {
                Description = description;
                ChangeType = changeType;
            }

            public override string ToString()
            {
                return $"- {Description}";
            }
        }

        public void Dispose()
        {
            if (string.IsNullOrWhiteSpace(_options.ChangeLogFilePath))
            {
                return;
            }

            if (File.Exists(_options.ChangeLogFilePath))
            {
                File.Delete(_options.ChangeLogFilePath);
            }

            if (!_trackedChanges.Any())
            {
                return;
            }

            File.WriteAllLines(_options.ChangeLogFilePath, _trackedChanges.Select(s => s.ToString()));
        }
    }
}
