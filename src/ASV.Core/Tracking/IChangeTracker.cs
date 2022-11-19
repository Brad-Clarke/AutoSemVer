using System.Reflection;
using ASV.Core.Enums;
using ASV.Core.Extensions;

namespace ASV.Core.Tracking
{
    public interface IChangeTracker
    {
        void Track(string description, ChangeType changeType);

        //void Track(ChangeType changeType, ReflectedType type, string source, string description);
        void Track(ChangeType changeType, ChangeLevel level, ReflectedType type, string primarySource, string secondarySource, string description);
    }

    public static class ChangeTrackerExtensions
    {
        public static void TrackRemoved(this IChangeTracker tracker, MethodInfo method)
            => tracker.Track(ChangeType.Removal, method.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method was removed");

        public static void TrackAdded(this IChangeTracker tracker, MethodInfo method)
            => tracker.Track(ChangeType.Removal, method.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method was added");

        public static void TrackMadePrivate(this IChangeTracker tracker, MethodInfo method)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Major, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method is no longer visible");

        public static void TrackMadePublic(this IChangeTracker tracker, MethodInfo method)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Minor, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), "Method is now visible");

        public static void TrackChanged(this IChangeTracker tracker, MethodInfo method, string description)
            => tracker.Track(ChangeType.Removal, method.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, method.DeclaringType?.FullName ?? "N/A", method.ToFriendlyName(), description);



        public static void TrackRemoved(this IChangeTracker tracker, PropertyInfo property)
            => tracker.Track(ChangeType.Removal, property.IsPublic() ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, property.DeclaringType?.FullName ?? "N/A", property.ToFriendlyName(), "Property was removed");

        public static void TrackAdded(this IChangeTracker tracker, PropertyInfo property)
            => tracker.Track(ChangeType.Removal, property.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch, ReflectedType.Method, property.DeclaringType?.FullName ?? "N/A", property.ToFriendlyName(), "Property was added");

        public static void TrackMadePrivate(this IChangeTracker tracker, PropertyInfo property)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Major, ReflectedType.Method, property.DeclaringType?.FullName ?? "N/A", property.ToFriendlyName(), "Property is no longer visible");

        public static void TrackMadePublic(this IChangeTracker tracker, PropertyInfo property)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Minor, ReflectedType.Method, property.DeclaringType?.FullName ?? "N/A", property.ToFriendlyName(), "Property is now visible");

        public static void TrackChanged(this IChangeTracker tracker, PropertyInfo property, string description)
            => tracker.Track(ChangeType.Removal, property.IsPublic() ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, property.DeclaringType?.FullName ?? "N/A", property.ToFriendlyName(), description);



        public static void TrackRemoved(this IChangeTracker tracker, ConstructorInfo constructor)
            => tracker.Track(ChangeType.Removal, constructor.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, constructor.DeclaringType?.FullName ?? "N/A", constructor.ToFriendlyName(), "Constructor was removed");

        public static void TrackAdded(this IChangeTracker tracker, ConstructorInfo constructor)
            => tracker.Track(ChangeType.Removal, constructor.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch, ReflectedType.Method, constructor.DeclaringType?.FullName ?? "N/A", constructor.ToFriendlyName(), "Constructor was added");

        public static void TrackMadePrivate(this IChangeTracker tracker, ConstructorInfo constructor)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Major, ReflectedType.Method, constructor.DeclaringType?.FullName ?? "N/A", constructor.ToFriendlyName(), "Constructor is no longer visible");

        public static void TrackMadePublic(this IChangeTracker tracker, ConstructorInfo constructor)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Minor, ReflectedType.Method, constructor.DeclaringType?.FullName ?? "N/A", constructor.ToFriendlyName(), "Constructor is now visible");

        public static void TrackChanged(this IChangeTracker tracker, ConstructorInfo constructor, string description)
            => tracker.Track(ChangeType.Removal, constructor.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, constructor.DeclaringType?.FullName ?? "N/A", constructor.ToFriendlyName(), description);



        public static void TrackRemoved(this IChangeTracker tracker, EventInfo @event)
            => tracker.Track(ChangeType.Removal, @event.IsPublic() ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, @event.DeclaringType?.FullName ?? "N/A", @event.ToFriendlyName(), "Event was removed");

        public static void TrackAdded(this IChangeTracker tracker, EventInfo @event)
            => tracker.Track(ChangeType.Removal, @event.IsPublic() ? ChangeLevel.Minor : ChangeLevel.Patch, ReflectedType.Method, @event.DeclaringType?.FullName ?? "N/A", @event.ToFriendlyName(), "Event was added");

        public static void TrackMadePrivate(this IChangeTracker tracker, EventInfo @event)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Major, ReflectedType.Method, @event.DeclaringType?.FullName ?? "N/A", @event.ToFriendlyName(), "Event is no longer visible");

        public static void TrackMadePublic(this IChangeTracker tracker, EventInfo @event)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Minor, ReflectedType.Method, @event.DeclaringType?.FullName ?? "N/A", @event.ToFriendlyName(), "Event is now visible");

        public static void TrackChanged(this IChangeTracker tracker, EventInfo @event, string description)
            => tracker.Track(ChangeType.Removal, @event.IsPublic() ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, @event.DeclaringType?.FullName ?? "N/A", @event.ToFriendlyName(), description);



        public static void TrackRemoved(this IChangeTracker tracker, FieldInfo field)
            => tracker.Track(ChangeType.Removal, field.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, field.DeclaringType?.FullName ?? "N/A", field.ToFriendlyName(), "Field was removed");

        public static void TrackAdded(this IChangeTracker tracker, FieldInfo field)
            => tracker.Track(ChangeType.Removal, field.IsPublic ? ChangeLevel.Minor : ChangeLevel.Patch, ReflectedType.Method, field.DeclaringType?.FullName ?? "N/A", field.ToFriendlyName(), "Field was added");

        public static void TrackMadePrivate(this IChangeTracker tracker, FieldInfo field)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Major, ReflectedType.Method, field.DeclaringType?.FullName ?? "N/A", field.ToFriendlyName(), "Field is no longer visible");

        public static void TrackMadePublic(this IChangeTracker tracker, FieldInfo field)
            => tracker.Track(ChangeType.Removal, ChangeLevel.Minor, ReflectedType.Method, field.DeclaringType?.FullName ?? "N/A", field.ToFriendlyName(), "Field is now visible");

        public static void TrackChanged(this IChangeTracker tracker, FieldInfo field, string description)
            => tracker.Track(ChangeType.Removal, field.IsPublic ? ChangeLevel.Major : ChangeLevel.Minor, ReflectedType.Method, field.DeclaringType?.FullName ?? "N/A", field.ToFriendlyName(), description);
    }
}
