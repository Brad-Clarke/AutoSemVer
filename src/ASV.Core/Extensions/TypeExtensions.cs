using System.Reflection;
using System.Runtime.CompilerServices;

namespace ASV.Core.Extensions
{
    public static class TypeExtensions
    {
        public static MethodInfo[] GetValidMethods(this Type type)
            => type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(t => !t.IsSpecialName && t.DeclaringType != typeof(object))
                .ToArray();

        public static FieldInfo[] GetValidFields(this Type type)
            => type
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(t => !t.IsSpecialName && t.DeclaringType != typeof(object))
                .ToArray();

        public static PropertyInfo[] GetValidProperties(this Type type)
            => type
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(t => !t.IsSpecialName && t.DeclaringType != typeof(object))
                .ToArray();

        public static ConstructorInfo[] GetValidConstructors(this Type type)
            => type
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(t => !t.IsSpecialName && t.DeclaringType != typeof(object))
                .ToArray();

        public static EventInfo[] GetValidEvents(this Type type)
            => type
                .GetEvents(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(t => !t.IsSpecialName && t.DeclaringType != typeof(object))
                .ToArray();

        public static string ToFriendlyName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name.TrimEnd('&');
            }

            string name = type.Name.Split('`').First();

            string arguments = string.Join(", ", type.GetGenericArguments().Select(a => a.ToFriendlyName()));

            return $"{name}<{arguments}>";
        }

        public static bool IsAnonymous(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static bool IsSystemGenerated(this Type type)
        {
            return type.ImplementsAttribute<CompilerGeneratedAttribute>();
        }
    }
}
