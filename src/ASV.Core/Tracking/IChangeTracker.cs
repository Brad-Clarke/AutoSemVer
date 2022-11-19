using System.Reflection;
using ASV.Core.Enums;
using ASV.Core.Extensions;

namespace ASV.Core.Tracking
{
    public interface IChangeTracker
    {
        void Track(string description, ChangeType changeType);

        //void Track(ChangeType changeType, ReflectedType type, string source, string description);
        //void Track(ChangeType changeType, ReflectedType type, string primarySource, string secondarySource, string description);
    }

    public static class ChangeTrackerExtensions
    {
        //public static void TrackRemoved(this IChangeTracker tracker, MethodInfo method)
        //    => tracker.Track(ChangeType.Removal, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method was removed");

        //public static void TrackAdded(this IChangeTracker tracker, MethodInfo method)
        //    => tracker.Track(ChangeType.Removal, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method was added");

        //public static void TrackHidden(this IChangeTracker tracker, MethodInfo method)
        //    => tracker.Track(ChangeType.Removal, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method is no longer visible");
    }
}
