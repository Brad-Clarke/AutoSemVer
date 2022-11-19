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

        public void Track(ChangeType changeType, ChangeLevel level, ReflectedType type, string primarySource, string secondarySource, string description)
        {
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

        private class TrackedChangeV2
        {
            private readonly ChangeType _changeType;
            private readonly ChangeLevel _level;
            private readonly ReflectedType _type;
            private readonly string _primarySource;
            private readonly string _secondarySource;
            private readonly string _description;

            public TrackedChangeV2(ChangeType changeType, ChangeLevel level, ReflectedType type, string primarySource, string secondarySource, string description)
            {
                _changeType = changeType;
                _level = level;
                _type = type;
                _primarySource = primarySource;
                _secondarySource = secondarySource;
                _description = description;
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
