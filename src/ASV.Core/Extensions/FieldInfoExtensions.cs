using System.Reflection;

namespace ASV.Core.Extensions
{
    internal static class FieldInfoExtensions
    {
        public static string ToFriendlyName(this FieldInfo field)
            => $"{field.DeclaringType.ToFriendlyName()}.{field.Name}";
    }
}
