using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class EventInfoExtensions
    {
        public static string ToFriendlyName(this EventInfo @event)
            => $"{@event.DeclaringType.ToFriendlyName()}.{@event.Name}";
    }
}
