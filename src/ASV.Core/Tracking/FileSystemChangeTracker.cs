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

        public void Track(string description, ChangeType changeType, bool isVisible)
        {
            _trackedChanges.Add(new TrackedChange(description, changeType, isVisible));
        }

        private class TrackedChange
        {
            public string Description { get; }

            public ChangeType ChangeType { get; }

            public bool IsVisible { get; }

            public TrackedChange(string description, ChangeType changeType, bool isVisible)
            {
                Description = description;
                ChangeType = changeType;
                IsVisible = isVisible;
            }

            public override string ToString()
            {
                return $"- {Description}";
            }
        }

        public void Dispose()
        {
        }
    }
}
