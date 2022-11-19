using ASV.Core.Enums;

namespace ASV.Core.Tracking
{
    public interface IChangeTracker
    {
        void Track(string description, ChangeType changeType);

        //void Track(ChangeType changeType, string source, string description);
    }
}
